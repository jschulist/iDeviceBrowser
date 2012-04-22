using System;
using System.Collections.Generic;
using System.Text;

namespace iDeviceBrowser
{
    public struct SourceAndDestination
    {
        private string _source;
        private string _destination;
        private string _filename;
        private ulong _bytes;

        public string Source { get { return _source; } }
        public string Destination { get { return _destination; } }
        public string Filename { get { return _filename; } }
        public ulong Bytes { get { return _bytes; } }

        public SourceAndDestination(string source, string destination, string filename, ulong bytes)
        {
            _source = source;
            _destination = destination;
            _filename = filename;
            _bytes = bytes;
        }
    }
}
