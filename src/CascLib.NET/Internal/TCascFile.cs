using System.Runtime.InteropServices;

namespace CascLib.NET;

/// <summary>
/// Information about a file in a <see cref="CascStorage"/>.
/// </summary>
/// <remarks>
/// This implementation is incomplete as the other fields are currently unused.<br/><br/>
/// See <a href="https://github.com/ladislav-zezula/CascLib/blob/07ab5f37ad282cc101d5c17793c550a0a6d4637f/src/CascCommon.h#L362">Original C++ Source</a>
/// for the full implementation.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct TCascFile
{
    private const ulong CASC_MAGIC_FILE = 0x454C494643534143; // 'CASCFILE'
    private static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

    /// <summary>
    /// Determines if a handle to a file in <see cref="CascStorage"/> is valid.
    /// </summary>
    /// <param name="hFile">File handle to check.</param>
    /// <returns>
    /// Pointer to the <see cref="TCascFile"/> instance if the file handle is valid, otherwise <c>null</c>.
    /// </returns>
    public static unsafe TCascFile* IsValid(IntPtr hFile)
    {
        var pFile = (TCascFile*)hFile;
        return hFile != INVALID_HANDLE_VALUE &&
               pFile != null &&
               pFile->ClassName == CASC_MAGIC_FILE &&
               pFile->pCKeyEntry != IntPtr.Zero
            ? pFile
            : null;
    }

    public ulong ClassName;
    public IntPtr hStorage;
    public IntPtr pCKeyEntry;
    public IntPtr pFileSpan;
    public ulong ContentSize;
    public ulong EncodedSize;
    public ulong FilePointer;

    // Incomplete
}