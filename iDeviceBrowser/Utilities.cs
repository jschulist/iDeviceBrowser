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

        public static void Copy(Stream from, Stream to)
        {
            Copy(from, to, null, () => false);
        }

        public static void Copy(Stream from, Stream to, Action<ulong> bytesTransfered, Func<bool> cancelled)
        {
            byte[] buffer = new byte[Constants.BUFFER_SIZE];

            int bytes = from.Read(buffer, 0, Constants.BUFFER_SIZE);
            while (bytes > 0 && !cancelled())
            {
                to.Write(buffer, 0, bytes);
                if (bytesTransfered != null)
                {
                    bytesTransfered((ulong)bytes);
                }
                bytes = from.Read(buffer, 0, Constants.BUFFER_SIZE);
            }
        }

        public static void CopyFileToDevice(iPhone iDeviceInterface, string source, string destination, Action<ulong> bytesTransfered, Func<bool> cancelled)
        {
            if (source.Equals("Thumbs.db") || source.Equals(".DS_Store"))
            {
                return;
            }

            // if it already exists
            if (iDeviceInterface.Exists(destination))
            {
                // TODO: DO SOMETHING WHEN THE FILE ALREADY EXISTS
            }

            if (File.Exists(source))
            {
                using (FileStream from = File.OpenRead(source))
                using (iPhoneFile to = iPhoneFile.OpenWrite(iDeviceInterface, destination))
                {
                    Utilities.Copy(from, to, bytesTransfered, cancelled);
                }
            }
        }

        // TODO: MOVE THIS INTO IPHONE CLASS
        public static void CopyFileFromDevice(iPhone iDeviceInterface, string source, string destination, Action<ulong> bytesTransfered, Func<bool> cancelled)
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
                    Utilities.Copy(from, to, bytesTransfered, cancelled);
                }
            }
        }

        public static string PathCombine(string left, string right)
        {
            if (right.Length > 0 && right[0].Equals(Constants.PATH_SEPARATOR))
            {
                throw new ArgumentException("Cannot combine paths when right is a root path (ie: '/bin')");
            }

            string result;

            if (left.Length == 0)
            {
                left += Constants.PATH_SEPARATOR;
            }
            
            if (left[left.Length - 1].Equals(Constants.PATH_SEPARATOR))
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
            double num = Math.Round(size / 1024.0, 0);

            if (size != 0 && num == 0.0)
            {
                num = 1.0;
            }

            string result = String.Format("{0:#,0.##} {1}", num, "KB");

            return result;
        }
    }
}
