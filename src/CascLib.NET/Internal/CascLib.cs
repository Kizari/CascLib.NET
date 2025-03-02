using System.Runtime.CompilerServices;
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
    public static partial bool CascGetStorageInfo(IntPtr hStorage, CascStorageInfoClass InfoClass, IntPtr pvStorageInfo,
        UIntPtr cbStorageInfo, IntPtr pcbLengthNeeded);

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

    /// <summary>
    /// Convenience method for invoking
    /// <see cref="CascGetStorageInfo(System.IntPtr,CascStorageInfoClass,System.IntPtr,System.UIntPtr,System.IntPtr)"/>.
    /// </summary>
    /// <param name="hStorage">Handle to the native CASC storage instance.</param>
    /// <param name="type">Type of information to retrieve.</param>
    /// <typeparam name="TInfo">Type of the data associated with the <see cref="CascStorageInfoClass"/>.</typeparam>
    /// <returns>The retrieved information.</returns>
    public static TInfo CascGetStorageInfo<TInfo>(IntPtr hStorage, CascStorageInfoClass type)
    {
        // Allocate unmanaged memory
        var size = GetDataSize<TInfo>();
        var pBuffer = Marshal.AllocHGlobal(size);
        var pLengthNeeded = Marshal.AllocHGlobal(Unsafe.SizeOf<UIntPtr>());

        // Invoke the native method from CascLib
        if (!CascGetStorageInfo(hStorage, type, pBuffer, new UIntPtr((uint)size), pLengthNeeded)
            || Marshal.GetLastPInvokeError() > 0)
        {
            // Call failed, resize the buffer to the needed size and try again
            size = (int)Marshal.PtrToStructure<UIntPtr>(pLengthNeeded).ToUInt32();
            Marshal.FreeHGlobal(pLengthNeeded);
            Marshal.FreeHGlobal(pBuffer);
            pBuffer = Marshal.AllocHGlobal(size);
            pLengthNeeded = Marshal.AllocHGlobal(Unsafe.SizeOf<UIntPtr>());
            CascGetStorageInfo(hStorage, type, pBuffer, new UIntPtr((uint)size), pLengthNeeded);
        }

        // Marshal the result to managed types
        var result = MarshalData<TInfo>(pBuffer, (int)Marshal.PtrToStructure<UIntPtr>(pLengthNeeded).ToUInt32());
        
        // Free memory
        Marshal.FreeHGlobal(pLengthNeeded);
        Marshal.FreeHGlobal(pBuffer);

        return result;
        
        // Gets the size of a type used with the parent method
        static int GetDataSize<TData>()
        {
            if (typeof(TData).IsAssignableTo(typeof(Enum)))
            {
                return 4;
            }

            return typeof(TData) == typeof(string) ? CascFindData.MAX_PATH : Marshal.SizeOf<TData>();
        }
        
        // Marshals data of a type used with the parent method
        static TData MarshalData<TData>(IntPtr pBuffer, int size)
        {
            if (typeof(TData).IsAssignableTo(typeof(Enum)))
            {
                return (TData)(object)Marshal.PtrToStructure<uint>(pBuffer);
            }

            if (typeof(TData) == typeof(string))
            {
                return (TData)(object)Marshal.PtrToStringAnsi(pBuffer, size);
            }

            if (typeof(TData) == typeof(CascStorageTags))
            {
                return (TData)(object)CascStorageTags.FromBuffer(pBuffer);
            }

            return Marshal.PtrToStructure<TData>(pBuffer)!;
        }
    }
}