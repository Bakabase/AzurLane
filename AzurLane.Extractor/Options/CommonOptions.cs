using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzurLane.Extractor.Localization;
using CommandLine;

namespace AzurLane.Extractor.Options
{
    class CommonOptions
    {
        [Option('o', "output", HelpText = nameof(Output), ResourceType = typeof(Cli), Required = true)]
        public string Output { get; set; }

        [Option('v', "verbose", HelpText = nameof(Verbose), ResourceType = typeof(Cli))]
        public bool Verbose { get; set; }
    }
}
