using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWasmAntivirusProtection.Tasks
{
    public class Asset
    {
        public string hash { get; set; }
        public string url { get; set; }
    }

    public class AssetsManifest
    {
        public List<Asset> assets { get; set; }
        public string version { get; set; }
    }

}
