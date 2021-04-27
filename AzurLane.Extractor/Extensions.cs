using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurLane.Extractor
{
    public static class Extensions
    {
        public static string BuildMessage(this Exception e)
        {
            var sb = new StringBuilder();
            sb.AppendLine(e.Message);
            sb.AppendLine(e.StackTrace);
            if (e.InnerException != null)
            {
                sb.Append(e.InnerException.BuildMessage());
            }

            return sb.ToString();
        }
    }
}