using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzurLane.Extractor.Localization;
using CommandLine;

namespace AzurLane.Extractor.Options
{
    [Verb("extract", HelpText = nameof(ExtractOptions), ResourceType = typeof(Cli))]
    class ExtractOptions : CommonOptions
    {
        [Option("tex-and-mesh-path", HelpText = nameof(TextureAndMeshPath), Required = true)]
        public string TextureAndMeshPath { get; set; }
    }
}