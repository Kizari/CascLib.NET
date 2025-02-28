using System.Runtime.InteropServices;

namespace CascLib.NET;

/// <summary>
/// Helper for invoking methods from CascLib.
/// </summary>
/// <remarks>
/// See <a href="http://www.zezula.net/en/casc/casclib.html">CascLib Documentation</a> for more information
/// about these methods.
/// </remarks>
internal static partial class CascLib
{
    private const string DllName = "CascLib.dll";

    [LibraryImport(DllName, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascOpenStorage(string szStorageName, uint dwLocaleMask, out IntPtr phStorage);

    [LibraryImport(DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascCloseStorage(IntPtr hStorage);

    [LibraryImport(DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascGetStorageInfo(IntPtr hStorage, IntPtr pInfoClass, IntPtr pvStorageInfo,
        ulong cbStorageInfo, IntPtr pcbLengthNeeded);

    [LibraryImport(DllName, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascOpenFile(IntPtr hStorage, string pvFileName, uint dwLocaleFlags, uint dwOpenFlags,
        out IntPtr PtrFileHandle);

    [LibraryImport(DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascGetFileSize64(IntPtr hFile, ref ulong result);

    [LibraryImport(DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascSetFilePointer64(IntPtr hFile, long distanceToMove,
        out ulong pNewPos, SeekOrigin dwMoveMethod);

    [LibraryImport(DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascReadFile(IntPtr hFile, [Out]byte[] lpBuffer, uint dwToRead, out uint pdwRead);

    [LibraryImport(DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascCloseFile(IntPtr hFile);

    [LibraryImport(DllName, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr CascFindFirstFile(IntPtr hStorage, string szMask,
        IntPtr pFindData, string? szListFile);

    [LibraryImport(DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascFindNextFile(IntPtr hFind, IntPtr pFindData);

    [LibraryImport(DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CascFindClose(IntPtr hFind);
}