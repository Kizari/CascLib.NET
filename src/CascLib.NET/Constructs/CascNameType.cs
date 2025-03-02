namespace CascLib.NET;

/// <summary>
/// The type of the <see cref="CascFindData.FileName"/> in <see cref="CascFindData"/>.
/// </summary>
/// <remarks>
/// See also <a href="https://github.com/ladislav-zezula/CascLib/blob/master/src/CascLib.h#L218">Original C++ Code</a>
/// </remarks>
public enum CascNameType
{
    /// <summary>
    /// <see cref="CascFindData.FileName"/> contains a full file path.
    /// </summary>
    Full,
    
    /// <summary>
    /// <see cref="CascFindData.FileName"/> contains the data identifier.
    /// </summary>
    DataId,
    
    /// <summary>
    /// <see cref="CascFindData.FileName"/> contains the name of the content key.
    /// </summary>
    CKey,
    
    /// <summary>
    /// <see cref="CascFindData.FileName"/> contains the name of the encoded key.
    /// </summary>
    EKey
}