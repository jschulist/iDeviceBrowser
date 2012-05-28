using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Manzana;
using System.Diagnostics;

namespace iDeviceBrowser
{
    // TODO: CAN IFILEOPERATION (http://msdn.microsoft.com/en-us/magazine/cc163304.aspx) REPLACE THIS?

    // TODO: SHOULD WE PROMPT FOR OVERWRITES?

    public partial class FileDialog : Form
    {
        public FileDialog(iPhone iDeviceInterface)
        {
            InitializeComponent();

            _iDeviceInterface = iDeviceInterface;
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
        }

        public delegate void Callback();

        private readonly iPhone _iDeviceInterface;
        private int _fileCount = 0;
        private ulong _totalBytes = 0;
        private ulong _bytesCounter = 0;
        private ulong _progressBarBytesCounter = 0;
        private ulong _lastBytesValue = 0;
        private System.Timers.Timer _timer;
        private bool _isCancelled = false;

        #region REMOVE THIS IF WE DON'T END UP USING IT
        public SynchronizationContext SyncContext { get; set; }
        public void ShowAsync()
        {
            this.SyncContext.Post(s => this.Show(), null);
        }
        #endregion REMOVE THIS IF WE DON'T END UP USING IT

        public void CopyLocalSources(IEnumerable<string> sources, string destination)
        {
            Copy(
                () => GetLocalFiles(sources, destination),
                (file) =>
                {
                    // create matching directory on the device
                    if (!_iDeviceInterface.Exists(file.Destination))
                    {
                        _iDeviceInterface.CreateDirectory(file.Destination);
                    }

                    Stopwatch sp = new Stopwatch();
                    sp.Start();
                    Utilities.CopyFileToDevice(_iDeviceInterface, file.Source, Utilities.PathCombine(file.Destination, file.Filename), BytesCopied, () => _isCancelled);
                    sp.Stop();
                    long milliseconds = sp.ElapsedMilliseconds;
                }
            );
        }

        public void CopyRemoteSources(IEnumerable<string> sources, string destination, bool destinationIsFile = false)
        {
            Copy(
                () => GetRemoteFiles(sources, destination, destinationIsFile),
                (file) =>
                {
                    // create matching directory on the device
                    if (!Directory.Exists(file.Destination))
                    {
                        Directory.CreateDirectory(file.Destination);
                    }

                    Stopwatch sp = new Stopwatch();
                    sp.Start();
                    Utilities.CopyFileFromDevice(_iDeviceInterface, file.Source, Path.Combine(file.Destination, file.Filename), BytesCopied, () => _isCancelled);
                    sp.Stop();
                    long milliseconds = sp.ElapsedMilliseconds;
                }
            );
        }

        private void Copy(Func<IEnumerable<SourceAndDestination>> getFiles, Action<SourceAndDestination> copy)
        {
            Async(
                null,
                () =>
                {
                    IEnumerable<SourceAndDestination> files = getFiles();

                    foreach (SourceAndDestination file in files)
                    {
                        _fileCount += 1;
                        _totalBytes += file.Bytes;
                    }

                    _bytesCounter = _totalBytes;
                    _lastBytesValue = _totalBytes;

                    ShiftToUiThread(
                        () =>
                        {
                            SummaryLabel.Text = string.Format("Copying {0} items ({1})", _fileCount, Utilities.GetFileSize(_totalBytes));
                            // TODO: HANDLE THE OVERFLOW CASE CAUSED BY THIS INT DIVISION
                            BytesProgressBar.Maximum = (int)(_bytesCounter / Constants.BUFFER_SIZE);
                        });

                    _timer.Start();
                    foreach (SourceAndDestination file in files)
                    {
                        if (!_isCancelled)
                        {
                            SourceAndDestination closureFile = file;

                            ShiftToUiThread(
                                () =>
                                {
                                    this.NameLabel.Text = closureFile.Filename;
                                    this.FromLabel.Text = closureFile.Source;
                                    this.ToLabel.Text = closureFile.Destination;
                                    this.ItemsRemainingLabel.Text = _fileCount.ToString() + " (" + Utilities.GetFileSize(_bytesCounter) + ")";
                                });

                            copy(file);

                            _fileCount -= 1;
                        }
                    }
                    _timer.Stop();
                },
                this.Hide
            );
        }

        private void BytesCopied(ulong bytes)
        {
            _bytesCounter -= bytes;
            _progressBarBytesCounter += bytes;

            if (_progressBarBytesCounter >= Constants.BUFFER_SIZE)
            {
                ShiftToUiThread(
                    () =>
                    {
                        BytesProgressBar.PerformStep();
                    });

                _progressBarBytesCounter -= Constants.BUFFER_SIZE;
            }
        }

        private IEnumerable<SourceAndDestination> GetLocalFiles(IEnumerable<string> sources, string destination)
        {
            foreach (string source in sources)
            {
                if (File.Exists(source))
                {
                    string filename = Path.GetFileName(source);
                    FileInfo fi = new FileInfo(source);
                    SourceAndDestination lar = new SourceAndDestination(source, destination, filename, (ulong)fi.Length);

                    yield return lar;
                }
                else if (Directory.Exists(source))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(source);
                    string directoryName = directoryInfo.Name;

                    foreach (string file in Directory.GetFiles(source))
                    {
                        string filename = Path.GetFileName(file);
                        FileInfo fi = new FileInfo(file);
                        SourceAndDestination lar = new SourceAndDestination(file, Utilities.PathCombine(destination, directoryName), filename, (ulong)fi.Length);

                        yield return lar;
                    }

                    // copy all directories over recursively
                    IEnumerable<SourceAndDestination> files = GetLocalFiles(Directory.GetDirectories(source), Utilities.PathCombine(destination, directoryName));
                    foreach (SourceAndDestination file in files)
                    {
                        yield return file;
                    }
                }
            }
        }

        private IEnumerable<SourceAndDestination> GetRemoteFiles(IEnumerable<string> sources, string destination, bool destinationIsFile = false)
        {
            foreach (string source in sources)
            {
                // TODO: TEST WHEN THE ROOT DIRECTORY IS SELECTED
                string lastPart = source.Substring(source.LastIndexOf(Constants.PATH_SEPARATOR) + 1, source.Length - source.LastIndexOf(Constants.PATH_SEPARATOR) - 1);

                if (_iDeviceInterface.IsFile(source))
                {
                    string newDestination = destination;
                    string filename = lastPart;

                    if (destinationIsFile)
                    {
                        newDestination = Path.GetDirectoryName(destination);
                        filename = Path.GetFileName(destination);
                    }

                    SourceAndDestination lar = new SourceAndDestination(source, newDestination, filename, _iDeviceInterface.FileSize(source));

                    yield return lar;
                }
                else if (_iDeviceInterface.IsDirectory(source))
                {
                    foreach (string file in _iDeviceInterface.GetFiles(source))
                    {
                        // copy all files over recursively
                        IEnumerable<SourceAndDestination> files = GetRemoteFiles(new string[] { Utilities.PathCombine(source, file) }, Path.Combine(destination, lastPart));
                        foreach (SourceAndDestination f in files)
                        {
                            yield return f;
                        }
                    }

                    foreach (string directory in _iDeviceInterface.GetDirectories(source))
                    {
                        // copy all directories over recursively
                        IEnumerable<SourceAndDestination> filesFromDirectories = GetRemoteFiles(new string[] { Utilities.PathCombine(source, directory) }, Path.Combine(destination, lastPart));
                        foreach (SourceAndDestination file in filesFromDirectories)
                        {
                            yield return file;
                        }
                    }
                }
            }
        }

        private void Async(Callback before, Callback async, Callback after)
        {
            if (before != null)
            {
                before();
            }
            ThreadPool.QueueUserWorkItem(
                _ =>
                    {
                        async();

                        if (after != null)
                        {
                            ShiftToUiThread(after);
                        }
                    }
                );
        }

        private void ShiftToUiThread(Callback callback)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { ShiftToUiThread(callback); }));
                    return;
                }

                callback();
            }
            catch (ObjectDisposedException)
            {
                // swallow
            }
        }

        private void Cancel()
        {
            _isCancelled = true;
            _timer.Stop();
            Hide();
        }

        #region Events
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ulong difference = _lastBytesValue - _bytesCounter;

            ShiftToUiThread(
                () =>
                {
                    if (_bytesCounter > 0 && difference > 0)
                    {
                        ulong secondsRemaining = _bytesCounter / difference;
                        TimeSpan ts = new TimeSpan(0, 0, (int)secondsRemaining);
                        this.TimeRemainingLabel.Text = "About " + ts.ToString();
                    }
                    this.SpeedLabel.Text = Utilities.GetFileSize(difference) + "/second";
                });

            _lastBytesValue = _bytesCounter;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void FileDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cancel();
        }
        #endregion Events
    }
}
