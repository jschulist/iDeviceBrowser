// Software License Agreement (BSD License)
// 
// Copyright (c) 2007, Peter Dennis Bartok <PeterDennisBartok@gmail.com>
// All rights reserved.
// 
// Redistribution and use of this software in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above
//   copyright notice, this list of conditions and the
//   following disclaimer.
// 
// * Redistributions in binary form must reproduce the above
//   copyright notice, this list of conditions and the
//   following disclaimer in the documentation and/or other
//   materials provided with the distribution.
// 
// * Neither the name of Peter Dennis Bartok nor the names of its
//   contributors may be used to endorse or promote products
//   derived from this software without specific prior
//   written permission of Yahoo! Inc.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
// TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Manzana {
	/// <summary>
	/// Exposes access to the Apple iPhone
	/// </summary>
	public class iPhone {
		#region Locals
		private DeviceNotificationCallback			dnc;
		private DeviceRestoreNotificationCallback	drn1;
		private DeviceRestoreNotificationCallback	drn2;
		private DeviceRestoreNotificationCallback	drn3;
		private DeviceRestoreNotificationCallback	drn4;

		unsafe internal void* iPhoneHandle;
		unsafe internal void* hAFC;
		unsafe internal void* hService;
		private bool		connected;
		private string		current_directory;
        private bool wasAFC2 = false;
		#endregion	// Locals

		#region Constructors
		/// <summary>
		/// Initializes a new iPhone object.
		/// </summary>
		unsafe private void doConstruction() {
			dnc = new DeviceNotificationCallback(NotifyCallback);
			drn1 = new DeviceRestoreNotificationCallback(DfuConnectCallback);
			drn2 = new DeviceRestoreNotificationCallback(RecoveryConnectCallback);
			drn3 = new DeviceRestoreNotificationCallback(DfuDisconnectCallback);
			drn4 = new DeviceRestoreNotificationCallback(RecoveryDisconnectCallback);

			void* notification;
			int ret = MobileDevice.AMDeviceNotificationSubscribe(dnc, 0, 0, 0, out notification);
			if (ret != 0) {
				throw new Exception("AMDeviceNotificationSubscribe failed with error " + ret);
			}

			ret = MobileDevice.AMRestoreRegisterForDeviceNotifications(drn1, drn2, drn3, drn4, 0, null);
			if (ret != 0) {
				throw new Exception("AMRestoreRegisterForDeviceNotifications failed with error " + ret);
			}
			current_directory = "/";
		}

		/// <summary>
		/// Creates a new iPhone object. If an iPhone is connected to the computer, a connection will automatically be opened.
		/// </summary>
		public iPhone () {
			doConstruction();
		}

		/// <summary>
		/// Constructor for iPhone object
		/// </summary>
		/// <param name="myConnectHandler"></param>
		/// <param name="myDisconnectHandler"></param>
        public iPhone(ConnectEventHandler myConnectHandler, ConnectEventHandler myDisconnectHandler) {
			Connect += myConnectHandler;
			Disconnect += myDisconnectHandler;
			doConstruction();
        }
		#endregion	// Constructors

		#region Properties
		/// <summary>
		/// Gets the current activation state of the phone
		/// </summary>
		unsafe public string ActivationState {
			get {
				return MobileDevice.AMDeviceCopyValue(iPhoneHandle, "ActivationState");
			}
		}

		/// <summary>
		/// Returns true if an iPhone is connected to the computer
		/// </summary>
		public bool IsConnected {
			get {
				return connected;
			}
		}

		/// <summary>
		/// Returns the Device information about the connected iPhone
		/// </summary>
		unsafe public void* Device {
			get {
				return iPhoneHandle;
			}
		}

		///<summary>
		/// Returns the 40-character UUID of the device
		///</summary>
		unsafe public string DeviceId {
			get {
				return MobileDevice.AMDeviceCopyValue(iPhoneHandle, "UniqueDeviceID");
			}
		}

		///<summary>
		/// Returns the type of the device, should be either 'iPhone' or 'iPod'.
		///</summary>
		unsafe public string DeviceType {
			get {
				return MobileDevice.AMDeviceCopyValue(iPhoneHandle, "DeviceClass");
			}
		}

		///<summary>
		/// Returns the current OS version running on the device (2.0, 2.2, 3.0, 3.1, etc).
		///</summary>
		unsafe public string DeviceVersion {
			get {
				return MobileDevice.AMDeviceCopyValue(iPhoneHandle, "ProductVersion");
			}
		}
		///<summary>
		/// Returns the name of the device, like "Dan's iPhone"
		///</summary>
		unsafe public string DeviceName {
			get {
				return MobileDevice.AMDeviceCopyValue(iPhoneHandle, "DeviceName");
			}
		}

		/// <summary>
		/// Returns the handle to the iPhone com.apple.afc service
		/// </summary>
		unsafe public void* AFCHandle {
			get {
				return hAFC;
			}
		}

        /// <summary>
        /// Returns if we are connected to jailbroken iphone
        /// </summary>
        public Boolean IsJailbreak {
            get {
                return wasAFC2 || (connected ? Exists("/Applications") : false);
            }
        }

		/// <summary>
		/// Gets/Sets the current working directory, used by all file and directory methods
		/// </summary>
		public string CurrentDirectory {
			get {
				return current_directory;
			}

			set {
				string new_path = FullPath(current_directory, value);
				if (!IsDirectory(new_path)) {
					throw new Exception("Invalid directory specified");
				}
				current_directory = new_path;
			}
		}
		#endregion	// Properties

		#region Events
		/// <summary>
		/// The <c>Connect</c> event is triggered when a iPhone is connected to the computer
		/// </summary>
		public event ConnectEventHandler Connect;

		/// <summary>
		/// Raises the <see>Connect</see> event.
		/// </summary>
		/// <param name="args">A <see cref="ConnectEventArgs"/> that contains the event data.</param>
		protected void OnConnect(ConnectEventArgs args) {
			ConnectEventHandler handler = Connect;

			if (handler != null) {
				handler(this, args);
			}
		}

		/// <summary>
		/// The <c>Disconnect</c> event is triggered when the iPhone is disconnected from the computer
		/// </summary>
		public event ConnectEventHandler Disconnect;

		/// <summary>
		/// Raises the <see>Disconnect</see> event.
		/// </summary>
		/// <param name="args">A <see cref="ConnectEventArgs"/> that contains the event data.</param>
		protected void OnDisconnect(ConnectEventArgs args) {
			ConnectEventHandler handler = Disconnect;

			if (handler != null) {
				handler(this, args);
			}
		}

		/// <summary>
		/// Write Me
		/// </summary>
		public event EventHandler DfuConnect;

		/// <summary>
		/// Raises the <see>DfuConnect</see> event.
		/// </summary>
		/// <param name="args">A <see cref="DeviceNotificationEventArgs"/> that contains the event data.</param>
		protected void OnDfuConnect(DeviceNotificationEventArgs args) {
			EventHandler handler = DfuConnect;

			if (handler != null) {
				handler(this, args);
			}
		}

		/// <summary>
		/// Write Me
		/// </summary>
		public event EventHandler DfuDisconnect;

		/// <summary>
		/// Raises the <see>DfiDisconnect</see> event.
		/// </summary>
		/// <param name="args">A <see cref="DeviceNotificationEventArgs"/> that contains the event data.</param>
		protected void OnDfuDisconnect(DeviceNotificationEventArgs args) {
			EventHandler handler = DfuDisconnect;

			if (handler != null) {
				handler(this, args);
			}
		}

		/// <summary>
		/// The RecoveryModeEnter event is triggered when the attached iPhone enters Recovery Mode
		/// </summary>
		public event EventHandler RecoveryModeEnter;

		/// <summary>
		/// Raises the <see>RecoveryModeEnter</see> event.
		/// </summary>
		/// <param name="args">A <see cref="DeviceNotificationEventArgs"/> that contains the event data.</param>
		protected void OnRecoveryModeEnter(DeviceNotificationEventArgs args) {
			EventHandler handler = RecoveryModeEnter;

			if (handler != null) {
				handler(this, args);
			}
		}

		/// <summary>
		/// The RecoveryModeLeave event is triggered when the attached iPhone leaves Recovery Mode
		/// </summary>
		public event EventHandler RecoveryModeLeave;

		/// <summary>
		/// Raises the <see>RecoveryModeLeave</see> event.
		/// </summary>
		/// <param name="args">A <see cref="DeviceNotificationEventArgs"/> that contains the event data.</param>
		protected void OnRecoveryModeLeave(DeviceNotificationEventArgs args) {
			EventHandler handler = RecoveryModeLeave;

			if (handler != null) {
				handler(this, args);
			}
		}

		#endregion	// Events

		#region Filesystem
		/// <summary>
		/// Returns the names of files in a specified directory
		/// </summary>
		/// <param name="path">The directory from which to retrieve the files.</param>
		/// <returns>A <c>String</c> array of file names in the specified directory. Names are relative to the provided directory</returns>
		unsafe public string[] GetFiles(string path) {
			if (!IsConnected) {
				throw new Exception("Not connected to phone");
			}

			string full_path = FullPath(CurrentDirectory, path);

			void* hAFCDir = null;
			if (MobileDevice.AFCDirectoryOpen(hAFC, full_path, ref hAFCDir) != 0) {
				throw new Exception("Path does not exist");
			}

			string buffer = null;
			ArrayList paths = new ArrayList();
			MobileDevice.AFCDirectoryRead(hAFC, hAFCDir, ref buffer);

			while (buffer!=null) {
				if (!IsDirectory(FullPath(full_path, buffer))) {
					paths.Add(buffer);
				}
				MobileDevice.AFCDirectoryRead(hAFC, hAFCDir, ref buffer);
			}
			MobileDevice.AFCDirectoryClose(hAFC, hAFCDir);
			return (string[])paths.ToArray(typeof(string));
		}

        /// <summary>
        /// Returns the FileInfo dictionary
        /// </summary>
        /// <param name="path">The file or directory for which to retrieve information.</param>
        unsafe public Dictionary<string,string> GetFileInfo(string path) {
            Dictionary<string, string> ans = new Dictionary<string,string>();
            void* data = null;

			int ret = MobileDevice.AFCFileInfoOpen(hAFC, path, ref data);
			if (ret == 0 && data != null) {
                void* pname, pvalue;

				while (MobileDevice.AFCKeyValueRead(data, out pname, out pvalue) == 0 && pname != null && pvalue != null) {
                    string name = Marshal.PtrToStringAnsi(new IntPtr(pname));
                    string value = Marshal.PtrToStringAnsi(new IntPtr(pvalue));
					ans.Add(name, value);
				}

				MobileDevice.AFCKeyValueClose(data);
			}

            return ans;
        }

		/// <summary>
		/// Returns the st_ifmt of a path
		/// </summary>
		/// <param name="path">Path to query</param>
		/// <returns>string representing value of st_ifmt</returns>
		private string Get_st_ifmt(string path) {
			Dictionary<string, string> fi = GetFileInfo(path);
			return fi["st_ifmt"];
		}

		/// <summary>
		/// Returns the size and type of the specified file or directory.
		/// </summary>
		/// <param name="path">The file or directory for which to retrieve information.</param>
		/// <param name="size">Returns the size of the specified file or directory</param>
		/// <param name="directory">Returns <c>true</c> if the given path describes a directory, false if it is a file.</param>
		unsafe public void GetFileInfo(string path, out ulong size, out bool directory) {
			Dictionary<string, string> fi = GetFileInfo(path);

			size = fi.ContainsKey("st_size") ? System.UInt64.Parse(fi["st_size"]) : 0;

			bool SLink = false;
			directory = false;
			if (fi.ContainsKey("st_ifmt")) {
				switch (fi["st_ifmt"]) {
					case "S_IFDIR": directory = true; break;
					case "S_IFLNK": SLink = true; break;
				}
			}

			if (SLink) { // test for symbolic directory link
				void* hAFCDir = null;

				if (directory = (MobileDevice.AFCDirectoryOpen(hAFC, path, ref hAFCDir) == 0))
					MobileDevice.AFCDirectoryClose(hAFC, hAFCDir);
			}
		}

		/// <summary>
		/// Returns the size of the specified file or directory.
		/// </summary>
		/// <param name="path">The file or directory for which to obtain the size.</param>
		/// <returns></returns>
		public ulong FileSize(string path) {
			bool is_dir;
			ulong size;

			GetFileInfo(path, out size, out is_dir);
			return size;
		}

		/// <summary>
		/// Creates the directory specified in path
		/// </summary>
		/// <param name="path">The directory path to create</param>
		/// <returns>true if directory was created</returns>
		unsafe public bool CreateDirectory(string path) {
			return !(MobileDevice.AFCDirectoryCreate(hAFC, FullPath(CurrentDirectory, path)) != 0);
		}

		/// <summary>
		/// Gets the names of subdirectories in a specified directory.
		/// </summary>
		/// <param name="path">The path for which an array of subdirectory names is returned.</param>
		/// <returns>An array of type <c>String</c> containing the names of subdirectories in <c>path</c>.</returns>
		unsafe public string[] GetDirectories(string path) {
			if (!IsConnected) {
				throw new Exception("Not connected to phone");
			}

			void* hAFCDir = null;
			string full_path = FullPath(CurrentDirectory, path);
			//full_path = "/private"; // bug test

			int res = MobileDevice.AFCDirectoryOpen(hAFC, full_path, ref hAFCDir);
			if (res != 0) {
				throw new Exception("Path does not exist: " + res.ToString());
			}

			string buffer = null;
			ArrayList paths = new ArrayList();
			MobileDevice.AFCDirectoryRead(hAFC, hAFCDir, ref buffer);

			while (buffer!=null) {
				if ((buffer != ".") && (buffer != "..") && IsDirectory(FullPath(full_path, buffer))) {
					paths.Add(buffer);
				}
				MobileDevice.AFCDirectoryRead(hAFC, hAFCDir, ref buffer);
			}
			MobileDevice.AFCDirectoryClose(hAFC, hAFCDir);
			return (string[])paths.ToArray(typeof(string));
		}

		/// <summary>
		/// Moves a file or a directory and its contents to a new location or renames a file or directory if the old and new parent path matches.
		/// </summary>
		/// <param name="sourceName">The path of the file or directory to move or rename.</param>
		/// <param name="destName">The path to the new location for <c>sourceName</c>.</param>
		///	<remarks>Files cannot be moved across filesystem boundaries.</remarks>
		unsafe public bool Rename(string sourceName, string destName) {
			return MobileDevice.AFCRenamePath(hAFC, FullPath(CurrentDirectory, sourceName), FullPath(CurrentDirectory, destName)) == 0;
		}

		/// <summary>
		/// FIXME
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="destName"></param>
		public void Copy(string sourceName, string destName) {
			
		}

		/// <summary>
		/// Returns the root information for the specified path. 
		/// </summary>
		/// <param name="path">The path of a file or directory.</param>
		/// <returns>A string containing the root information for the specified path. </returns>
		public string GetDirectoryRoot(string path) {
			return "/";
		}

		/// <summary>
		/// Determines whether the given path refers to an existing file or directory on the phone. 
		/// </summary>
		/// <param name="path">The path to test.</param>
		/// <returns><c>true</c> if path refers to an existing file or directory, otherwise <c>false</c>.</returns>
		unsafe public bool Exists(string path) {
			void* data = null;

			int ret = MobileDevice.AFCFileInfoOpen(hAFC, path, ref data);
			if (ret == 0)
				MobileDevice.AFCKeyValueClose(data);

			return ret == 0;
		}

		/// <summary>
		/// Determines whether the given path refers to an existing directory on the phone. 
		/// </summary>
		/// <param name="path">The path to test.</param>
		/// <returns><c>true</c> if path refers to an existing directory or is a symbolic link to one, otherwise <c>false</c>.</returns>
		public bool IsDirectory(string path) {
			bool is_dir;
			ulong size;

			GetFileInfo(path, out size, out is_dir);
			return is_dir;
		}

		/// <summary>
		/// Test if path represents a regular file
		/// </summary>
		/// <param name="path">path to query</param>
		/// <returns>true if path refers to a regular file, false if path is a link or directory</returns>
		public bool IsFile(string path) {
			return Get_st_ifmt(path) == "S_IFREG";
		}

		/// <summary>
		/// Test if path represents a link
		/// </summary>
		/// <param name="path">path to test</param>
		/// <returns>true if path is a symbolic link</returns>
		public bool IsLink(string path) {
			return Get_st_ifmt(path) == "S_IFLNK";
		}

		/// <summary>
		/// Deletes an empty directory from a specified path.
		/// </summary>
		/// <param name="path">The name of the empty directory to remove. This directory must be writable and empty.</param>
		unsafe public void DeleteDirectory(string path) {
			string full_path = FullPath(CurrentDirectory, path);
			if (IsDirectory(full_path)) {
				MobileDevice.AFCRemovePath(hAFC, full_path);
			}
		}

		/// <summary>
		/// Deletes the specified directory and, if indicated, any subdirectories in the directory.
		/// </summary>
		/// <param name="path">The name of the directory to remove.</param>
		/// <param name="recursive"><c>true</c> to remove directories, subdirectories, and files in path; otherwise, <c>false</c>. </param>
		public void DeleteDirectory(string path, bool recursive) {
			if (!recursive) {
				DeleteDirectory(path);
				return;
			}

			string full_path = FullPath(CurrentDirectory, path);
			if (IsDirectory(full_path)) {
				InternalDeleteDirectory(path);
			}
				
		}

		/// <summary>
		/// Deletes the specified file.
		/// </summary>
		/// <param name="path">The name of the file to remove.</param>
		unsafe public void DeleteFile(string path) {
			string full_path = FullPath(CurrentDirectory, path);
			if (Exists(full_path)) {
				MobileDevice.AFCRemovePath(hAFC, full_path);
			}
		}
		#endregion	// Filesystem

		#region Public Methods
		/// <summary>
		/// Close and Reopen AFC Connection
		/// </summary>
		/// <returns>status from reopen</returns>
		unsafe public void ReConnect() {
			int ans = MobileDevice.AFCConnectionClose(hAFC);
			ans = MobileDevice.AMDeviceStopSession(iPhoneHandle);
			ans = MobileDevice.AMDeviceDisconnect(iPhoneHandle);
			ConnectToPhone();
		}
		#endregion // public Methods

		#region Private Methods
		unsafe private bool ConnectToPhone() {
			if (MobileDevice.AMDeviceConnect(iPhoneHandle) == 1) {
				//int connid;

				throw new Exception("Phone in recovery mode, support not yet implemented");
				//connid = MobileDevice.AMDeviceGetConnectionID(ref iPhoneHandle);
				//MobileDevice.AMRestoreModeDeviceCreate(0, connid, 0);
				//return false;
			}
			if (MobileDevice.AMDeviceIsPaired(iPhoneHandle) == 0) {
				return false;
			}
			int chk = MobileDevice.AMDeviceValidatePairing(iPhoneHandle);
			if (chk != 0) {
				return false;
			}

			if (MobileDevice.AMDeviceStartSession(iPhoneHandle) == 1) {
				return false;
			}

            if (MobileDevice.AMDeviceStartService(iPhoneHandle, MobileDevice.CFStringMakeConstantString("com.apple.afc2"), ref hService, null) != 0) {
                if (MobileDevice.AMDeviceStartService(iPhoneHandle, MobileDevice.CFStringMakeConstantString("com.apple.afc"), ref hService, null) != 0) {
                    return false;
                }
            }
            else
                wasAFC2 = true;

			if (MobileDevice.AFCConnectionOpen(hService, 0, ref hAFC) != 0) {
				return false;
			}

			connected = true;
			return true;
		}

		unsafe private void NotifyCallback(ref AMDeviceNotificationCallbackInfo callback) {
			if (callback.msg == NotificationMessage.Connected) {
				iPhoneHandle = callback.dev;
				if (ConnectToPhone()) {
					OnConnect(new ConnectEventArgs(callback));
				}
			}
			else if (callback.msg == NotificationMessage.Disconnected) {
				connected = false;
				OnDisconnect(new ConnectEventArgs(callback));
			}
		}

		private void DfuConnectCallback(ref AMRecoveryDevice callback) {
			OnDfuConnect(new DeviceNotificationEventArgs(callback));
		}

		private void DfuDisconnectCallback(ref AMRecoveryDevice callback) {
			OnDfuDisconnect(new DeviceNotificationEventArgs(callback));
		}

		private void RecoveryConnectCallback(ref AMRecoveryDevice callback) {
			OnRecoveryModeEnter(new DeviceNotificationEventArgs(callback));
		}

		private void RecoveryDisconnectCallback(ref AMRecoveryDevice callback) {
			OnRecoveryModeLeave(new DeviceNotificationEventArgs(callback));
		}

		private void InternalDeleteDirectory(string path) {
			string full_path = FullPath(CurrentDirectory, path);
			string[] contents = GetFiles(path);
			for (int i = 0; i < contents.Length; i++) {
				DeleteFile(full_path + "/" + contents[i]);
			}

			contents = GetDirectories(path);
			for (int i = 0; i < contents.Length; i++) {
				InternalDeleteDirectory(full_path + "/" + contents[i]);
			}

			DeleteDirectory(path);
		}

		static char[] path_separators = { '/' };
		internal string FullPath(string path1, string path2) {

			if ((path1 == null) || (path1 == String.Empty)) {
				path1 = "/";
			}

			if ((path2 == null) || (path2 == String.Empty)) {
				path2 = "/";
			}

			string[] path_parts;
			if (path2[0] == '/') {
				path_parts = path2.Split(path_separators);
			} else if (path1[0] == '/') {
				path_parts = (path1 + "/" + path2).Split(path_separators);
			} else {
				path_parts = ("/" + path1 + "/" + path2).Split(path_separators);
			}

			string[] result_parts = new string[path_parts.Length];
			int target_index = 0;

			for (int i = 0; i < path_parts.Length; i++) {
				if (path_parts[i] == "..") {
					if (target_index > 0) {
						target_index--;
					}
				} else if ((path_parts[i] == ".") || (path_parts[i] == "")) {
					// Do nothing
				} else {
					result_parts[target_index++] = path_parts[i];
				}
			}

			return "/" + String.Join("/", result_parts, 0, target_index);
		}
		#endregion	// Private Methods
	}
}
