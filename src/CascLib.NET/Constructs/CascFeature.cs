namespace CascLib.NET;

/// <summary>
/// Denotes which features are available on a <see cref="CascStorage"/>.
/// </summary>
[Flags]
public enum CascFeature : uint
{
    /// <summary>
    /// File names are supported by the storage.
    /// </summary>
    FileNames = 0x00000001,
    
    /// <summary>
    /// Present if the storage's ROOT returns CKey. If not present, the root contains EKeys.
    /// </summary>
    RootCKey = 0x00000002,
    
    /// <summary>
    /// Tags are supported by the storage.
    /// </summary>
    Tags = 0x00000004,
    
    /// <summary>
    /// The storage contains file name hashes on all files.
    /// </summary>
    FileNameHashes = 0x00000008,
    
    /// <summary>
    /// The storage contains file name hashes for some files.
    /// </summary>
    FileNameHashesOptional = 0x00000010,
    
    /// <summary>
    /// The storage indexes files by their FileDataId.
    /// </summary>
    FileDataIds = 0x00000020,
    
    /// <summary>
    /// Locale flags are supported by the storage.
    /// </summary>
    LocaleFlags = 0x00000040,
    
    /// <summary>
    /// Content flags are supported by the storage.
    /// </summary>
    ContentFlags = 0x00000080,
    
    /// <summary>
    /// The storage is an online storage.
    /// </summary>
    Online = 0x00000100
}