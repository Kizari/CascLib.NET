using System.Runtime.InteropServices;

namespace CascLib.NET;

/// <summary>
/// Represents a file in <see cref="CascStorage"/>.
/// </summary>
/// <remarks>
/// See also <a href="https://github.com/ladislav-zezula/CascLib/blob/07ab5f37ad282cc101d5c17793c550a0a6d4637f/src/CascLib.h#L227">Original C++ Code</a>
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public struct CascFindData
{
    private const int MAX_PATH = 260;
    private const int MD5_HASH_SIZE = 16;
    
    /// <summary>
    /// Represents an invalid identifier.
    /// </summary>
    public const uint CASC_INVALID_ID = 0xFFFFFFFF;

    /// <summary>
    /// Full name of the file.
    /// </summary>
    /// <remarks>
    /// When <see cref="NameType"/> is <see cref="CascNameType.CKey"/> or <see cref="CascNameType.EKey"/>, this is
    /// the name of the key instead.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
    public string szFileName;

    /// <summary>
    /// Content key.
    /// </summary>
    /// <remarks>
    /// This is present if the CASC_FEATURE_ROOT_CKEY is present on the storage.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = MD5_HASH_SIZE)]
    public byte[] CKey;

    /// <summary>
    /// Encoded key.
    /// </summary>
    /// <remarks>
    /// This is always present.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = MD5_HASH_SIZE)]
    public byte[] EKey;

    /// <summary>
    /// Tag mask.
    /// </summary>
    /// <remarks>Only valid if the storage supports tags, otherwise will be <c>0</c>.</remarks>
    public ulong TagBitMask;
    
    /// <summary>
    /// Size of the file, as retrieved from CKey entry.
    /// </summary>
    public ulong FileSize;
    
    /// <summary>
    /// Plain name of the file.
    /// </summary>
    /// <remarks>
    /// Underlying <c>char*</c> points to the substring within <see cref="szFileName"/> that excludes the path
    /// component and only includes the file name itself.
    /// </remarks>
    public IntPtr szPlainName;
    
    /// <summary>
    /// File data ID.
    /// </summary>
    /// <remarks>
    /// Only valid if the storage supports file data IDs, otherwise contains <see cref="CASC_INVALID_ID"/>.
    /// </remarks>
    public uint dwFileDataId;
    
    /// <summary>
    /// Locale flags.
    /// </summary>
    /// <remarks>
    /// Only valid if the storage supports locale flags, otherwise contains <see cref="CASC_INVALID_ID"/>.
    /// </remarks>
    public uint dwLocaleFlags;
    
    /// <summary>
    /// Content flags.
    /// </summary>
    /// <remarks>
    /// Only valid if the storage supports content flags, otherwise contains <see cref="CASC_INVALID_ID"/>.
    /// </remarks>
    public uint dwContentFlags;
    
    /// <summary>
    /// Span count.
    /// </summary>
    public uint dwSpanCount;
    
    /// <summary>
    /// If true, the file is available locally.
    /// </summary>
    /// <remarks>
    /// Only the first bit of this field is used for the boolean value.
    /// </remarks>
    public uint bFileAvailable;
    
    /// <summary>
    /// Type of the name contained in <see cref="szFileName"/>.
    /// </summary>
    /// <remarks>
    /// If the file name is not known, CascLib uses a FileDataId-like name or a string representation of CKey/EKey.
    /// </remarks>
    public CascNameType NameType;
}

/// <summary>
/// The type of the <see cref="CascFindData.szFileName"/> in <see cref="CascFindData"/>.
/// </summary>
/// <remarks>
/// See also <a href="https://github.com/ladislav-zezula/CascLib/blob/master/src/CascLib.h#L218">Original C++ Code</a>
/// </remarks>
public enum CascNameType
{
    /// <summary>
    /// <see cref="CascFindData.szFileName"/> contains a full file path.
    /// </summary>
    Full,
    
    /// <summary>
    /// <see cref="CascFindData.szFileName"/> contains the data identifier.
    /// </summary>
    DataId,
    
    /// <summary>
    /// <see cref="CascFindData.szFileName"/> contains the name of the content key.
    /// </summary>
    CKey,
    
    /// <summary>
    /// <see cref="CascFindData.szFileName"/> contains the name of the encoded key.
    /// </summary>
    EKey
}