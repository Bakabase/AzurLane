using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AzurLane.Extractor.Localization;
using AzurLane.Extractor.Options;
using CommandLine;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AzurLane.Extractor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var testMergeArguments = new List<string>
            {
                "merge",
                "--ip",
                "127.0.0.1",
                "--port",
                "7555",
                "--adb",
                @"D:\OneDrive\Program Files\platform-tools\adb.exe",
                "--apk",
                @"D:\BaiduNetdiskDownload\BLHX.apk",
                "--assets-path-in-device",
                "/storage/emulated/0/Android/data/com.bilibili.azurlane/files/AssetBundles",
                "--local-assets-path",
                @"D:\BaiduNetdiskDownload\Full-Paintings",
                "-o",
                @"D:\BaiduNetdiskDownload\Test\Raw",
                "--asset-types",
                "painting",
                "paintingface"
            };
            var testExtractArguments = new List<string>
            {
                "extract",
                "--tex-and-mesh-path",
                @"D:\BaiduNetdiskDownload\Assets",
                "-o",
                @"D:\BaiduNetdiskDownload\Test\Output"
            };
            var r1 = await Parser.Default.ParseArguments<MergeOptions, ExtractOptions>(args)
                .WithParsedAsync<MergeOptions>(async o =>
                {
                    await new PaintingExtractor().Merge(o.Adb, o.Ip, o.Port, o.AssetsPathInDevice,
                        o.Apk, o.LocalAssetsPath, o.AssetTypes.ToList(), o.Output);
                });
            var r2 = await (r1)
                .WithParsedAsync<ExtractOptions>(async o =>
                {
                    await new PaintingExtractor().Extract(o.TextureAndMeshPath, o.Output);
                });
        }
    }
}