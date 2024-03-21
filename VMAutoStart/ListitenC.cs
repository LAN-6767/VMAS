using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMAutoStart
{
    internal class ListitenC
    {
        private String www;
        private String color;
        private bool autorun;
        public string Path { get => www; set => www = value; }
        public string Color { get => color; set => color = value; }
        public bool Autorun { get => autorun; set => autorun = value; }

        public static implicit operator ConfigBase(ListitenC listitenC)
        {
            ConfigBase cBase = new ConfigBase();
            cBase.Path = listitenC.Path;
            cBase.IsAutorun = listitenC.Autorun;
            switch (listitenC.Color)
            {
                case "red":
                    cBase.Isrun = false;
                    break;
                case "green":
                    cBase.Isrun = true;
                    break;
                default:
                    cBase.Isrun = false;
                    break;
            }
            return cBase;
        }

        public static ConfigBase[] ToConfigBaseArray(List<ListitenC> listitenCs)
        {
            ConfigBase[] configBases = new ConfigBase[listitenCs.Count];
            for (int i = 0; i < listitenCs.Count; i++)
            {
                configBases[i] = new ConfigBase(); 
                configBases[i].Path = listitenCs[i].Path;
                configBases[i].Isrun = (listitenCs[i].Color.Equals("red")) ? false : true;
                configBases[i].IsAutorun = listitenCs[i].Autorun;
            }
            return configBases;
        }
    }
}
