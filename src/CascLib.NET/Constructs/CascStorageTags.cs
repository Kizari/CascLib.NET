using System.Runtime.InteropServices;

namespace CascLib.NET;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CascStorageTag
{
    /// <summary>
    /// Name of the tag.
    /// </summary>
    [MarshalAs(UnmanagedType.LPStr)]
    public string Name;

    /// <summary>
    /// Length of <see cref="Name"/>.
    /// </summary>
    public uint NameLength;

    /// <summary>
    /// Value of the tag.
    /// </summary>
    public uint Value;
}

/// <summary>
/// Holds <see cref="CascStorageTag"/>s for a storage.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CascStorageTags
{
    /// <summary>
    /// Number of items in the Tags array.
    /// </summary>
    public ulong TagCount;

    /// <summary>
    /// Reserved for future use.
    /// </summary>
    public ulong Reserved;

    /// <summary>
    /// Array of CASC tags.
    /// </summary>
    public CascStorageTag[] Tags;

    /// <summary>
    /// Marshals a <see cref="CascStorageTags"/> instance from an unmanaged buffer.
    /// </summary>
    /// <param name="pBuffer">Pointer to the unmanaged buffer containing the CASC_STORAGE_TAGS instance.</param>
    /// <returns>Managed copy of the CASC_STORAGE_TAGS instance.</returns>
    internal static CascStorageTags FromBuffer(IntPtr pBuffer)
    {
        var tagSize = Marshal.SizeOf<CascStorageTag>();
        var result = new CascStorageTags
        {
            TagCount = Marshal.PtrToStructure<ulong>(pBuffer)
        };

        result.Tags = new CascStorageTag[result.TagCount];
        
        for (var i = 0; i < (int)result.TagCount; i++)
        {
            var pTag = new IntPtr(pBuffer.ToInt64() + 16 + i * tagSize);
            result.Tags[i] = Marshal.PtrToStructure<CascStorageTag>(pTag);
        }

        return result;
    }
}