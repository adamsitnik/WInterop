﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using WInterop.Authentication.Types;
using WInterop.Authorization.Types;
using WInterop.ErrorHandling;
using WInterop.ErrorHandling.Types;
using WInterop.FileManagement.BufferWrappers;
using WInterop.FileManagement.Types;
using WInterop.Handles.Types;
using WInterop.Support;
using WInterop.Support.Buffers;

namespace WInterop.FileManagement
{
    public static partial class FileMethods
    {
        /// <summary>
        /// Direct usage of Imports isn't recommended. Use the wrappers that do the heavy lifting for you.
        /// </summary>
        public static partial class Imports
        {
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858.aspx
            [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            unsafe public static extern SafeFileHandle CreateFileW(
                string lpFileName,
                DesiredAccess dwDesiredAccess,
                ShareMode dwShareMode,
                SECURITY_ATTRIBUTES* lpSecurityAttributes,
                CreationDisposition dwCreationDisposition,
                uint dwFlagsAndAttributes,
                IntPtr hTemplateFile);

            // https://https://msdn.microsoft.com/en-us/library/windows/desktop/aa365497.aspx
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr ReOpenFile(
                SafeFileHandle hOriginalFile,
                DesiredAccess dwDesiredAccess,
                ShareMode dwShareMode,
                uint dwFlags);

            // Ex version is supported by WinRT apps
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364944.aspx
            [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            public static extern FileAttributes GetFileAttributesW(
                string lpFileName);

            // Ex version is supported by WinRT apps
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364952.aspx
            [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetFileInformationByHandle(
                SafeFileHandle hFile,
                out BY_HANDLE_FILE_INFORMATION lpFileInformation);

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364980.aspx (kernel32)
            [DllImport(ApiSets.api_ms_win_core_file_l1_1_0, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            public static extern uint GetLongPathNameW(
                string lpszShortPath,
                SafeHandle lpszLongPath,
                uint cchBuffer);

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364989.aspx (kernel32)
            [DllImport(ApiSets.api_ms_win_core_file_l1_1_0, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            public static extern uint GetShortPathNameW(
                string lpszLongPath,
                SafeHandle lpszShortPath,
                uint cchBuffer);

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364962.aspx (kernel32)
            [DllImport(ApiSets.api_ms_win_core_file_l1_1_0, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            public static extern uint GetFinalPathNameByHandleW(
                SafeFileHandle hFile,
                SafeHandle lpszFilePath,
                uint cchFilePath,
                GetFinalPathNameByHandleFlags dwFlags);

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363852.aspx
            // CopyFile calls CopyFileEx with COPY_FILE_FAIL_IF_EXISTS if fail if exists is set
            // (Neither are available in WinRT- use CopyFile2)
            [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CopyFileExW(
                string lpExistingFileName,
                string lpNewFileName,
                CopyProgressRoutine lpProgressRoutine,
                IntPtr lpData,
                [MarshalAs(UnmanagedType.Bool)] ref bool pbCancel,
                CopyFileFlags dwCopyFlags);

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363866.aspx
            // Note that CreateSymbolicLinkW returns a BOOLEAN (byte), not a BOOL (int)
            [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool CreateSymbolicLinkW(
                string lpSymlinkFileName,
                string lpTargetFileName,
                SYMBOLIC_LINK_FLAG dwFlags);

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364021.aspx
            [DllImport(Libraries.Advapi32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool EncryptFileW(
                string lpFileName);

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363903.aspx
            [DllImport(Libraries.Advapi32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DecryptFileW(
                string lpFileName,
                uint dwReserved);

            // Adding Users to an Encrypted File
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363765.aspx
            //
            // 1. LookupAccountName() to get SID
            // 2. CertOpenSystemStore((HCRYPTPROV)NULL,L"TrustedPeople") to get cert store
            // 3. CertFindCertificateInStore() to find the desired cert (PCCERT_CONTEXT)
            //
            //   EFS_CERTIFICATE.cbTotalLength = Marshal.Sizeof(EFS_CERTIFICATE)
            //   EFS_CERTIFICATE.pUserSid = &SID
            //   EFS_CERTIFICATE.pCertBlob.dwCertEncodingType = CCERT_CONTEXT.dwCertEncodingType
            //   EFS_CERTIFICATE.pCertBlob.cbData = CCERT_CONTEXT.cbCertEncoded
            //   EFS_CERTIFICATE.pCertBlob.pbData = CCERT_CONTEXT.pbCertEncoded

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363770.aspx
            //
            //  DWORD WINAPI AddUsersToEncryptedFile(
            //      _In_ LPCWSTR lpFileName,
            //      _In_ PENCRYPTION_CERTIFICATE_LIST pUsers
            //  );
            [DllImport(Libraries.Advapi32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
            public static extern uint AddUsersToEncryptedFile(
                string lpFileName,

                /// <summary>
                /// Pointer to ENCRYPTION_CERTIFICATE_LIST array
                /// </summary>
                IntPtr pUsers);

            // https://msdn.microsoft.com/en-us/library/windows/hardware/ff567052.aspx
            // http://www.pinvoke.net/default.aspx/ntdll/NtQueryInformationFile.html
            [DllImport(Libraries.Ntdll, CharSet = CharSet.Unicode, ExactSpelling = true)]
            unsafe public static extern NTSTATUS NtQueryInformationFile(
                SafeFileHandle FileHandle,
                out IO_STATUS_BLOCK IoStatusBlock,
                void* FileInformation,
                uint Length,
                FILE_INFORMATION_CLASS FileInformationClass);

            // https://msdn.microsoft.com/en-us/library/windows/hardware/ff546850.aspx
            // https://msdn.microsoft.com/en-us/library/hh551132.aspx
            [DllImport(Libraries.Ntdll, CharSet = CharSet.Unicode, ExactSpelling = true)]
            public unsafe static extern BOOLEAN RtlIsNameInExpression(
                UNICODE_STRING* Expression,
                UNICODE_STRING* Name,
                BOOLEAN IgnoreCase,
                IntPtr UpcaseTable);
        }

        /// <summary>
        /// Get the long (non 8.3) path version of the given path.
        /// </summary>
        public static string GetLongPathName(string path)
        {
            var wrapper = new LongPathNameWrapper { Path = path };
            return BufferHelper.ApiInvoke(ref wrapper, path);
        }

        /// <summary>
        /// Get the short (8.3) path version of the given path.
        /// </summary>
        public static string GetShortPathName(string path)
        {
            var wrapper = new ShortPathNameWrapper { Path = path };
            return BufferHelper.ApiInvoke(ref wrapper, path);
        }

        /// <summary>
        /// Gets a canonical version of the given handle's path.
        /// </summary>
        public static string GetFinalPathNameByHandle(
            SafeFileHandle fileHandle,
            GetFinalPathNameByHandleFlags flags = GetFinalPathNameByHandleFlags.FILE_NAME_NORMALIZED | GetFinalPathNameByHandleFlags.VOLUME_NAME_DOS)
        {
            var wrapper = new FinalPathNameByHandleWrapper { FileHandle = fileHandle, Flags = flags };
            return BufferHelper.ApiInvoke(ref wrapper);
        }

        /// <summary>
        /// Gets a canonical version of the given file's path.
        /// </summary>
        /// <param name="resolveLinks">True to get the destination of links rather than the link itself.</param>
        public static string GetFinalPathName(string path, GetFinalPathNameByHandleFlags finalPathFlags, bool resolveLinks)
        {
            if (path == null) return null;

            // BackupSemantics is needed to get directory handles
            FileFlags flags = FileFlags.FILE_FLAG_BACKUP_SEMANTICS;
            if (!resolveLinks) flags |= FileFlags.FILE_FLAG_OPEN_REPARSE_POINT;

            using (SafeFileHandle fileHandle = CreateFile(path, 0, ShareMode.ReadWrite,
                CreationDisposition.OpenExisting, FileAttributes.NONE, flags))
            {
                return GetFinalPathNameByHandle(fileHandle, finalPathFlags);
            }
        }

        /// <summary>
        /// Gets the file information for the given handle.
        /// </summary>
        public static BY_HANDLE_FILE_INFORMATION GetFileInformationByHandle(SafeFileHandle fileHandle)
        {
            if (!Imports.GetFileInformationByHandle(fileHandle, out BY_HANDLE_FILE_INFORMATION fileInformation))
                throw Errors.GetIoExceptionForLastError();

            return fileInformation;
        }

        /// <summary>
        /// Creates symbolic links.
        /// </summary>
        public static void CreateSymbolicLink(string symbolicLinkPath, string targetPath, bool targetIsDirectory = false)
        {
            if (!Imports.CreateSymbolicLinkW(symbolicLinkPath, targetPath,
                targetIsDirectory ? SYMBOLIC_LINK_FLAG.SYMBOLIC_LINK_FLAG_DIRECTORY : SYMBOLIC_LINK_FLAG.SYMBOLIC_LINK_FLAG_FILE))
                throw Errors.GetIoExceptionForLastError(symbolicLinkPath);
        }

        /// <summary>
        /// CreateFile wrapper. Desktop only. Prefer FileManagement.CreateFile() as it will handle all supported platforms.
        /// </summary>
        /// <remarks>Not available in Windows Store applications.</remarks>
        public static SafeFileHandle CreateFileW(
            string path,
            DesiredAccess desiredAccess,
            ShareMode shareMode,
            CreationDisposition creationDisposition,
            FileAttributes fileAttributes = FileAttributes.NONE,
            FileFlags fileFlags = FileFlags.NONE,
            SecurityQosFlags securityQosFlags = SecurityQosFlags.NONE)
        {
            uint flags = (uint)fileAttributes | (uint)fileFlags | (uint)securityQosFlags;

            unsafe
            {
                SafeFileHandle handle = Imports.CreateFileW(path, desiredAccess, shareMode, null, creationDisposition, flags, IntPtr.Zero);
                if (handle.IsInvalid)
                    throw Errors.GetIoExceptionForLastError(path);
                return handle;
            }
        }

        /// <summary>
        /// CopyFileEx wrapper. Desktop only. Prefer FileManagement.CopyFile() as it will handle all supported platforms.
        /// </summary>
        /// <param name="overwrite">Overwrite an existing file if true.</param>
        public static void CopyFileEx(string source, string destination, bool overwrite = false)
        {
            bool cancel = false;

            if (!Imports.CopyFileExW(
                lpExistingFileName: source,
                lpNewFileName: destination,
                lpProgressRoutine: null,
                lpData: IntPtr.Zero,
                pbCancel: ref cancel,
                dwCopyFlags: overwrite ? 0 : CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS))
            {
                throw Errors.GetIoExceptionForLastError(source);
            }
        }

        /// <summary>
        /// Gets file attributes for the given path.
        /// </summary>
        /// <remarks>Not available in Store apps, use FileMethods.GetFileInfo instead.</remarks>
        public static FileAttributes GetFileAttributes(string path)
        {
            FileAttributes attributes = Imports.GetFileAttributesW(path);
            if (attributes == FileAttributes.INVALID_FILE_ATTRIBUTES)
                throw Errors.GetIoExceptionForLastError(path);

            return attributes;
        }

        public static string GetFileName(SafeFileHandle fileHandle)
        {
            // https://msdn.microsoft.com/en-us/library/windows/hardware/ff545817.aspx

            //  typedef struct _FILE_NAME_INFORMATION
            //  {
            //      ULONG FileNameLength;
            //      WCHAR FileName[1];
            //  } FILE_NAME_INFORMATION, *PFILE_NAME_INFORMATION;

            return GetFileInformationString(fileHandle, FILE_INFORMATION_CLASS.FileNameInformation);
        }

        public static string GetVolumeName(SafeFileHandle fileHandle)
        {
            // Same basic structure as FILE_NAME_INFORMATION
            return GetFileInformationString(fileHandle, FILE_INFORMATION_CLASS.FileVolumeNameInformation);
        }

        /// <summary>
        /// This is the short name for the file/directory name, not the path. Available from WindowsStore.
        /// </summary>
        public static string GetShortName(SafeFileHandle fileHandle)
        {
            // Same basic structure as FILE_NAME_INFORMATION
            return GetFileInformationString(fileHandle, FILE_INFORMATION_CLASS.FileAlternateNameInformation);
        }

        private unsafe static string GetFileInformationString(SafeFileHandle fileHandle, FILE_INFORMATION_CLASS fileInformationClass)
        {
            return BufferHelper.BufferInvoke((HeapBuffer buffer) =>
            {
                NTSTATUS status = NTSTATUS.STATUS_BUFFER_OVERFLOW;

                // Start with MAX_PATH
                uint byteLength = 260 * sizeof(char);

                TrailingString.SizedInBytes* value = null;

                while (status == NTSTATUS.STATUS_BUFFER_OVERFLOW)
                {
                    // Add space for the FileNameLength
                    buffer.EnsureByteCapacity(byteLength + sizeof(uint));

                    unsafe
                    {
                        status = Imports.NtQueryInformationFile(
                            FileHandle: fileHandle,
                            IoStatusBlock: out _,
                            FileInformation: buffer.VoidPointer,
                            Length: checked((uint)buffer.ByteCapacity),
                            FileInformationClass: fileInformationClass);
                    }

                    if (status == NTSTATUS.STATUS_SUCCESS || status == NTSTATUS.STATUS_BUFFER_OVERFLOW)
                    {
                        value = (TrailingString.SizedInBytes*)buffer.VoidPointer;
                        byteLength = value->SizeInBytes;
                    }
                }

                if (status != NTSTATUS.STATUS_SUCCESS)
                    throw ErrorMethods.GetIoExceptionForNTStatus(status);

                return value->Value;
            });
        }

        unsafe private static void GetFileInformation(SafeFileHandle fileHandle, FILE_INFORMATION_CLASS fileInformationClass, void* value, uint size)
        {
            NTSTATUS status = Imports.NtQueryInformationFile(
                FileHandle: fileHandle,
                IoStatusBlock: out _,
                FileInformation: value,
                Length: size,
                FileInformationClass: fileInformationClass);

            if (status != NTSTATUS.STATUS_SUCCESS)
                throw ErrorMethods.GetIoExceptionForNTStatus(status);
        }

        /// <summary>
        /// Gets the file mode for the given handle.
        /// </summary>
        public static FILE_MODE_INFORMATION GetFileMode(SafeFileHandle fileHandle)
        {
            FILE_MODE_INFORMATION info;
            unsafe
            {
                GetFileInformation(fileHandle, FILE_INFORMATION_CLASS.FileModeInformation, &info, sizeof(FILE_MODE_INFORMATION));
            }
            return info;
        }

        /// <summary>
        /// Return whether or not the given expression matches the given name. Takes standard
        /// Windows wildcards (*, ?, &lt;, &gt; &quot;).
        /// </summary>
        public unsafe static bool IsNameInExpression(string expression, string name, bool ignoreCase)
        {
            // If ignore case is set, the API will uppercase the name *if* an UpcaseTable
            // is not provided. It then flips to case-sensitive. In this state the expression
            // has to be uppercase to match as expected.

            fixed (char* e = ignoreCase ? expression.ToUpperInvariant() : expression)
            fixed (char* n = name)
            {
                UNICODE_STRING* eus = null;
                UNICODE_STRING* nus = null;

                if (e != null)
                {
                    var temp = new UNICODE_STRING(e, expression);
                    eus = &temp;
                }
                if (n != null)
                {
                    var temp = new UNICODE_STRING(n, name);
                    nus = &temp;
                }

                return Imports.RtlIsNameInExpression(eus, nus, ignoreCase, IntPtr.Zero);
            }
        }
    }
}
