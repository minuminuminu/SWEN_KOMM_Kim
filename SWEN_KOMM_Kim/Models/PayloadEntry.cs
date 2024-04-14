using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.Models
{
    internal class PayloadEntry
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public int DurationInSeconds { get; set; }

        public PayloadEntry(string name, int count, int durationInSeconds)
        {
            Name = name;
            Count = count;
            DurationInSeconds = durationInSeconds;
        }
    }
}
