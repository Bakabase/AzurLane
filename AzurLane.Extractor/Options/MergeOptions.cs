using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzurLane.Extractor.Localization;
using CommandLine;

namespace AzurLane.Extractor.Options
{
    [Verb("merge", HelpText = nameof(MergeOptions), ResourceType = typeof(Cli))]
    class MergeOptions : CommonOptions
    {
        [Option("adb", HelpText = nameof(Adb), ResourceType = typeof(Cli))]
        public string Adb { get; set; }

        [Option("ip", HelpText = nameof(Ip), ResourceType = typeof(Cli))]
        public string Ip { get; set; }

        [Option("port", HelpText = nameof(Port), ResourceType = typeof(Cli))]
        public short Port { get; set; }

        [Option("assets-path-in-device", HelpText = nameof(AssetsPathInDevice), ResourceType = typeof(Cli))]
        public string AssetsPathInDevice { get; set; }

        [Option("apk", HelpText = nameof(Apk), ResourceType = typeof(Cli))]
        public string Apk { get; set; }

        [Option("local-assets-path", HelpText = nameof(LocalAssetsPath), ResourceType = typeof(Cli))]
        public string LocalAssetsPath { get; set; }

        [Option("asset-types", HelpText = nameof(AssetTypes), ResourceType = typeof(Cli))]
        public IEnumerable<string> AssetTypes { get; set; } = new List<string> { "painting", "paintingface" };
    }
}
