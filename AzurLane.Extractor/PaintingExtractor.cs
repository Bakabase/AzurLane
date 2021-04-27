using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzurLane.Extractor.Models;
using ICSharpCode.SharpZipLib.Zip;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AzurLane.Extractor
{
    public class PaintingExtractor
    {
        private static readonly string[] NumberedNameShips = {"22", "33"};

        private static async Task<string> RunCommand(string filename, string arguments, bool showRawOutput)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filename, arguments)
                {
                    RedirectStandardOutput = !showRawOutput,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                },
            };
            process.Start();
            if (showRawOutput)
            {
                await process.WaitForExitAsync();
                return null;
            }

            var outputs = new List<char>();
            var buffer = new char[4096];
            while (!process.HasExited)
            {
                await Task.Yield();
                while (true)
                {
                    var readCount = await process.StandardOutput.ReadAsync(buffer, 0, buffer.Length);
                    if (readCount == 0)
                    {
                        break;
                    }

                    var data = buffer.Take(readCount).ToArray();
                    outputs.AddRange(data);
                }
            }

            return new string(outputs.ToArray());
        }

        public async Task Merge(string adb, string ip, short port, string deviceAssets, string apk, string localAssets,
            List<string> assetTypes,
            string output)
        {
            if (!string.IsNullOrEmpty(adb) && !string.IsNullOrEmpty(ip) && port > 0 &&
                !string.IsNullOrEmpty(deviceAssets))
            {
                Console.WriteLine($"Connecting device: {ip}:{port}.");
                var connect = await RunCommand(adb, $"connect {ip}:{port}", false);
                if (!connect.Contains("connected"))
                {
                    Console.WriteLine("Merging stopped.");
                    return;
                }

                Console.WriteLine("Connected.");
                Console.WriteLine("Getting file list in device.");
                var fileCounts = new Dictionary<string, int>();
                foreach (var targetDir in assetTypes.Select(ad =>
                    Path.Combine(deviceAssets, ad).Replace('\\', '/')))
                {
                    var fileList = (await RunCommand(adb, $"shell find {targetDir} -type f", false))
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim())
                        .Where(a => !a.Contains("No such file or directory")).ToList();
                    fileCounts[targetDir] = fileList.Count;
                }

                if (fileCounts.All(b => b.Value == 0))
                {
                    Console.WriteLine(
                        "All of following directories are not found, please make sure the device assets path or assets are set correctly.");
                    fileCounts.Keys.ToList().ForEach(Console.WriteLine);
                    Console.WriteLine("Merging stopped.");
                    return;
                }

                Console.WriteLine($"Got {fileCounts.Sum(a => a.Value)} files to pull. Start pulling...");
                foreach (var (d, _) in fileCounts)
                {
                    await RunCommand(adb, $"pull {d} {output}", true);
                }

                Console.WriteLine("Pull from device job is done.");
            }

            if (!string.IsNullOrEmpty(apk))
            {
                Console.WriteLine("Unzipping apk file...");
                var tmpOutput = Path.Combine(output, "tmp");
                Directory.CreateDirectory(tmpOutput);
                var fz = new FastZip();
                fz.ExtractZip(apk, tmpOutput, $".+AssetBundles.({string.Join('|', assetTypes)}).+");
                Console.WriteLine("Apk unzipped.");
                await BatchMove(tmpOutput, output, assetTypes);
                Console.WriteLine("Delete tmp directory.");
                Directory.Delete(tmpOutput, true);
            }

            if (!string.IsNullOrEmpty(localAssets))
            {
                await BatchMove(localAssets, output, assetTypes);
            }
        }

        private async Task BatchMove(string sourceDir, string outputDir, IReadOnlyCollection<string> assetTypes)
        {
            Console.WriteLine(
                $"Moving files from {sourceDir} to {outputDir} with asset types [{string.Join(',', assetTypes)}]");
            var subDirs = assetTypes.ToDictionary(a => a,
                a => Path.Combine("AssetBundles", a) + Path.DirectorySeparatorChar);
            foreach (var f in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                var subDir = subDirs.FirstOrDefault(a => f.Contains(a.Value)).Key;
                if (subDir != null)
                {
                    var targetDir = Path.Combine(outputDir, subDir);
                    var newFile = Path.Combine(targetDir, Path.GetFileName(f));
                    if (!File.Exists(newFile))
                    {
                        Directory.CreateDirectory(targetDir);
                        File.Move(f, newFile, false);
                    }
                }
            }

            Console.WriteLine("All files are moved");
        }

        private async Task<List<Resource>> ConvertToResources(string directory)
        {
            var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                .ToDictionary(Path.GetFileName, a => a);
            var imageFiles = files.Where(a => a.Key.EndsWith(".png"))
                .ToDictionary(a => Path.GetFileNameWithoutExtension(a.Key), a => a.Value);
            var meshFiles = files.Where(a => a.Key.EndsWith(".obj"))
                .ToDictionary(a => Path.GetFileNameWithoutExtension(a.Key), a => a.Value);
            var resources = new List<Resource>();
            foreach (var (name, fullname) in imageFiles)
            {
                if (Regex.IsMatch(name, "#\\d+$") ||
                    Regex.IsMatch(name, "^\\d+$") && !NumberedNameShips.Contains(name))
                {
                    continue;
                }

                var defaultMeshName = $"{name}-mesh";
                var nameSegments = name.Split('_', StringSplitOptions.RemoveEmptyEntries).ToList();
                var resource = new Resource
                {
                    Name = nameSegments[0],
                    ImagePath = fullname,
                    MeshPath = meshFiles.TryGetValue(defaultMeshName, out var v) ? v : null,
                };
                var replaceableMeshes = meshFiles.Keys.Where(a => a.StartsWith(defaultMeshName) && a != defaultMeshName)
                    .ToList();
                foreach (var rm in replaceableMeshes)
                {
                    var hash = Regex.Match(rm, "#(\\d+)$").Value;
                    var imageWithHash = imageFiles.Keys.FirstOrDefault(a => a.EndsWith(hash));
                    resource.ReplaceableParts.Add((
                        string.IsNullOrEmpty(imageWithHash) ? null : imageFiles[imageWithHash], meshFiles[rm]));
                }

                nameSegments.RemoveAt(0);
                if (int.TryParse(nameSegments.FirstOrDefault(), out var no))
                {
                    resource.No = no;
                    nameSegments.RemoveAt(0);
                }

                resource.Tags = nameSegments.ToList();

                resources.Add(resource);
            }

            var replaceableResources = resources.Where(t => t.ReplaceableParts.Any()).ToList();
            var replaceableWithImageResources =
                replaceableResources.Where(a => a.ReplaceableParts.Any(b => b.ImagePath != null)).ToList();

            return resources;
        }

        private async Task Extract(Resource resource, string outputDirectory)
        {
            var outputFilename = Path.Combine(outputDirectory, Path.GetFileName(resource.ImagePath)!);
            if (File.Exists(outputFilename))
            {
                return;
            }

            if (resource.MeshPath == null)
            {
                File.Copy(resource.ImagePath!, outputFilename);
            }
            else
            {
                List<(int X, int Y)> vs = new();
                List<(int X, int Y)> vTs = new();
                List<List<int>> groups = new();
                var img = await Image.LoadAsync(resource.ImagePath, new PngDecoder());
                var data = await File.ReadAllLinesAsync(resource.MeshPath);
                foreach (var d in data.Select(a => a.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
                {
                    switch (d[0])
                    {
                        case "v":
                            vs.Add((Math.Abs(int.Parse(d[1])), int.Parse(d[2])));
                            break;
                        case "vt":
                            vTs.Add(((int) Math.Round(double.Parse(d[1]) * img.Width),
                                (int) Math.Round((1 - double.Parse(d[2])) * img.Height)));
                            break;
                        case "f":
                            groups.Add(d.Skip(1)
                                .Select(a => int.Parse(a.Split('/', StringSplitOptions.RemoveEmptyEntries)[0]))
                                .ToList());
                            break;
                    }
                }

                var (width, height) = new Size(vs.Max(a => a.X) + 1, vs.Max(a => a.Y) + 1);
                vs = vs.Select(a => (a.X, height - a.Y - 1)).ToList();
                var outImg = new Image<Rgba32>(width, height);
                foreach (var g in groups)
                {
                    var groupVs = g.Select(a => vs[a - 1]).ToList();
                    var position = new Point(groupVs.Min(a => a.X), groupVs.Min(a => a.Y));
                    var groupVts = g.Select(a => vTs[a - 1]).ToList();
                    var cropPosition = new Point(groupVts.Min(a => a.X), groupVts.Min(a => a.Y));
                    var (brx, bry) = new Point(groupVts.Max(a => a.X), groupVts.Max(a => a.Y));
                    var cropSize = new Size(brx - cropPosition.X, bry - cropPosition.Y);
                    var crop = new Rectangle(cropPosition, cropSize);
                    if (cropSize.Width == 0 || cropSize.Height == 0)
                    {
                        Console.WriteLine(
                            $"{resource.ImagePath} has crop with 0px width or height, skipping this crop.");
                        continue;
                    }

                    try
                    {
                        var tImg = img.Clone(t => t.Crop(crop));
                        outImg.Mutate(t => t.DrawImage(tImg, position, 1));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error occurred");
                        Console.WriteLine(e.BuildMessage());
                    }
                }

                await outImg.SaveAsync(outputFilename, new PngEncoder());
            }

            Console.WriteLine($"All assets are merged and saved in {outputDirectory}");
        }

        public async Task Extract(string directory, string outputDirectory)
        {
            var resources = await ConvertToResources(directory);
            if (resources.Any())
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < resources.Count; i++)
            {
                var r = resources[i];
                await Extract(r, outputDirectory);
                var restCount = resources.Count - i - 1;
                var avgCost = (double) sw.ElapsedMilliseconds / (i + 1);
                var eta = TimeSpan.FromMilliseconds(restCount * avgCost);
                Console.Write($"\r{i + 1}/{resources.Count}, ETA: {eta.Minutes}:{eta.Seconds}");
            }

            Console.WriteLine("All resources are extracted.");
        }
    }
}
