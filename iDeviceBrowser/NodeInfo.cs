using System;
using System.Collections.Generic;
using System.Text;

namespace iDeviceBrowser
{
    public class NodeInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsRefreshed { get; set; }

        public NodeInfo(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        public NodeInfo(string name, string path, bool isRefreshed)
        {
            this.Name = name;
            this.Path = path;
            this.IsRefreshed = isRefreshed;
        }
    }
}
