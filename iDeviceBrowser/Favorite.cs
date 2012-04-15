using System;
using System.Collections.Generic;
using System.Text;

namespace iDeviceBrowser
{
    public class Favorite
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public Favorite(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        public Favorite()
        {
            
        }
    }
}
