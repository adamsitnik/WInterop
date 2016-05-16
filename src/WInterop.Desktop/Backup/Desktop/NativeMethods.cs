﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WInterop.FileManagement;
using WInterop.Handles;

namespace WInterop.Backup.Desktop
{
    /// <summary>
    /// These methods are only available from Windows desktop apps. Windows store apps cannot access them.
    /// </summary>

    public static partial class NativeMethods
    {
        /// <summary>
        /// Direct P/Invokes aren't recommended. Use the wrappers that do the heavy lifting for you.
        /// </summary>
        /// <remarks>
        /// By keeping the names exactly as they are defined we can reduce string count and make the initial P/Invoke call slightly faster.
        /// </remarks>
        public static class Direct
        {
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa362509.aspx
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool BackupRead(
                SafeFileHandle hFile,
                SafeHandle lpBuffer,
                uint nNumberOfBytesToRead,
                out uint lpNumberOfBytesRead,
                [MarshalAs(UnmanagedType.Bool)] bool bAbort,
                [MarshalAs(UnmanagedType.Bool)] bool bProcessSecurity,
                ref IntPtr context);

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa362510.aspx
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool BackupSeek(
                SafeFileHandle hFile,
                uint dwLowBytesToSeek,
                uint dwHighBytesToSeek,
                out uint lpdwLowByteSeeked,
                out uint lpdwHighByteSeeked,
                ref IntPtr context);
        }

        public static IEnumerable<StreamInformation> GetAlternateStreamInformation(string path)
        {
            List<StreamInformation> streams = new List<StreamInformation>();
            using (var fileHandle = FileManagement.NativeMethods.CreateFile(
                path: path,
                // To look at metadata we don't need read or write access
                desiredAccess: 0,
                shareMode: ShareMode.FILE_SHARE_READWRITE,
                creationDisposition: CreationDisposition.OPEN_EXISTING,
                fileAttributes: FileAttributes.NONE,
                fileFlags: FileFlags.FILE_FLAG_BACKUP_SEMANTICS))
            {
                using (BackupReader reader = new BackupReader(fileHandle))
                {
                    StreamInformation? info;
                    while ((info = reader.GetNextInfo()).HasValue)
                    {
                        if (info.Value.StreamType == BackupStreamType.BACKUP_ALTERNATE_DATA)
                        {
                            streams.Add(new StreamInformation { Name = info.Value.Name, Size = info.Value.Size });
                        }
                    }
                }
            }

            return streams;
        }
    }
}
