using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace PmllibAnalyser
{
    public class Uda
    {
        public Uda(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public List<PmllibFile> UsedBy { get; set; }

        public override string ToString()
        {
            return Name + "[" + UsedBy.Count +  "]";
        }
    }
}
