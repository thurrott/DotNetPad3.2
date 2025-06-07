using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPad32
{
    internal class AppState
    {
        public int ZoomValue { get; set; } = 100;
        public bool SpellCheck { get; set; } = false;
        public bool WordCount { get; set; } = false;
        public string AppName { get; set; } = ".NETpad Windows 11";
        public bool AutoSave { get; set; } = false;
        public double FontSizeInSettings { get; set; } = 18;
        public static string Choice { get; set; } = "Don't save";
    }
}
