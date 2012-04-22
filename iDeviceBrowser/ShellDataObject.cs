using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Manzana;

namespace iDeviceBrowser
{
    public class ShellDataObject : DataObject
    {
        public event Action<string> StatusUpdate;

        // this flag is used to prevent multiple callings to "GetData" when dropping in explorer.
        private bool downloaded = false;
        private iPhone _iDeviceInterface;
        private Dictionary<string, string> _mapping = new Dictionary<string, string>();

        public ShellDataObject(iPhone iDeviceInterface, Dictionary<string, string> mapping)
        {
            _iDeviceInterface = iDeviceInterface;
            _mapping = mapping;
        }

        public override object GetData(String format)
        {
            Object obj = base.GetData(format);

            if (format == DataFormats.FileDrop && !InDragLoop() && !downloaded)
            {
                String[] files = (String[])obj;

                for (int i = 0; i < files.Length; i++)
                {
                    DownloadFile(files[i]);
                }

                downloaded = true;
            }

            return obj;
        }

        private void DownloadFile(String destination)
        {
            string source = _mapping[destination];
            //if (StatusUpdate != null)
            //{
            //    StatusUpdate("Copying: " + source + "; To: " + destination);
            //}
            Utilities.CopyFileFromDevice(_iDeviceInterface, source, destination, (bytes) => { });
        }

        private bool InDragLoop()
        {
            return (0 != (int)GetData(ShellClipboardFormats.CFSTR_INDRAGLOOP));
        }
    }
}
