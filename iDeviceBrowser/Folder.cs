using System;
using System.Collections.Generic;
using System.Text;

namespace iDeviceBrowser
{
    public class Folder
    {
        public string Name { get; set; }
        public List<Folder> SubFolders { get; set; }

        public Folder(string name, List<Folder> subFolders)
        {
            this.Name = name;
            this.SubFolders = subFolders;
        }
    }
}
