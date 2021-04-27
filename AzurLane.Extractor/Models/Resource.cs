using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurLane.Extractor.Models
{
    public class Resource
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string MeshPath { get; set; }
        public List<(string ImagePath, string MeshPath)> ReplaceableParts { get; set; } = new();
        public List<string> Tags { get; set; }
        public int? No { get; set; }
    }
}