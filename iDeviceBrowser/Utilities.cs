using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Manzana;

namespace iDeviceBrowser
{
    public static class Utilities
    {
        public delegate void Callback();

        private const int BUFFER_SIZE = 8192;

        public static void Copy(Stream from, Stream to)
        {
            byte[] buffer = new byte[BUFFER_SIZE];

            int bytes = from.Read(buffer, 0, BUFFER_SIZE);
            while (bytes > 0)
            {
                to.Write(buffer, 0, bytes);
                bytes = from.Read(buffer, 0, BUFFER_SIZE);
            }
        }

        public static void CopyFileToDevice(iPhone iDeviceInterface, string source, string destination)
        {
            if (source.Equals("Thumbs.db") || source.Equals(".DS_Store"))
            {
                return;
            }

            // if it already exists
            if (iDeviceInterface.Exists(destination))
            {
                // TODO: DO SOMETHING WHEN THE FIRST ALREADY EXISTS
            }

            if (File.Exists(source))
            {
                using (FileStream from = File.OpenRead(source))
                using (iPhoneFile to = iPhoneFile.OpenWrite(iDeviceInterface, destination))
                {
                    Utilities.Copy(from, to);
                }
            }
        }

        // TODO: MOVE THIS INTO IPHONE CLASS
        public static void CopyFileFromDevice(iPhone iDeviceInterface, string source, string destination)
        {
            // remove our local file if it exists
            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            // make sure the source file exists
            if (iDeviceInterface.Exists(source))
            {
                using (iPhoneFile from = iPhoneFile.OpenRead(iDeviceInterface, source))
                using (FileStream to = File.OpenWrite(destination))
                {
                    Utilities.Copy(from, to);
                }
            }
        }
    }
}
