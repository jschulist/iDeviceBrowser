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

        private static readonly string[] _sizes = { "B", "KB", "MB", "GB", "PB" };


        public static void Copy(Stream from, Stream to, Action<ulong> bytesTransfered)
        {
            byte[] buffer = new byte[Constants.BUFFER_SIZE];

            int bytes = from.Read(buffer, 0, Constants.BUFFER_SIZE);
            while (bytes > 0)
            {
                to.Write(buffer, 0, bytes);
                bytesTransfered((ulong)bytes);
                bytes = from.Read(buffer, 0, Constants.BUFFER_SIZE);
            }
        }

        public static void CopyFileToDevice(iPhone iDeviceInterface, string source, string destination, Action<ulong> bytesTransfered)
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
                    Utilities.Copy(from, to, bytesTransfered);
                }
            }
        }

        // TODO: MOVE THIS INTO IPHONE CLASS
        public static void CopyFileFromDevice(iPhone iDeviceInterface, string source, string destination, Action<ulong> bytesTransfered)
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
                    Utilities.Copy(from, to, bytesTransfered);
                }
            }
        }

        public static string PathCombine(string left, string right)
        {
            string result;

            if (left.Length == 0)
            {
                result = Constants.PATH_SEPARATOR + right;
            }
            else if (left[left.Length - 1].Equals(Constants.PATH_SEPARATOR))
            {
                result = left + right;
            }
            else
            {
                result = left + Constants.PATH_SEPARATOR + right;
            }

            return result;
        }

        public static string GetFileSize(ulong size)
        {
            int place = 0;
            double num = 0;
            if (size > 0)
            {
                place = Convert.ToInt32(Math.Floor(Math.Log(size, 1024)));
                num = Math.Round(size / Math.Pow(1024, place), 1);
            }

            string result = String.Format("{0:0.##} {1}", num, _sizes[place]);

            return result;
        }
    }
}
