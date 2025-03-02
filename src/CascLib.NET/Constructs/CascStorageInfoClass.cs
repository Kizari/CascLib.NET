namespace CascLib.NET;

/// <summary>
/// The type of information requested about a <see cref="CascStorage"/>.
/// </summary>
public enum CascStorageInfoClass
{
    /// <summary>
    /// Requests to number of local files in the storage.
    /// </summary>
    /// <remarks>
    /// Files can exist under multiple different names, so the total number of files in the archive can be higher than
    /// the value returned by this info class.
    /// </remarks>
    LocalFileCount,
    
    /// <summary>
    /// Requests the total file count, including files that have not been downloaded.
    /// </summary>
    TotalFileCount,
    
    /// <summary>
    /// Requests the flags that express which features are used on this storage.
    /// </summary>
    Features,
    
    /// <summary>
    /// Not supported by CascLib.
    /// </summary>
    InstalledLocales,
    
    /// <summary>
    /// Requests CASC_STORAGE_PRODUCT.
    /// </summary>
    Product,
    
    /// <summary>
    /// Requests the CASC_STORAGE_TAGS structure.
    /// </summary>
    Tags,
    
    /// <summary>
    /// Requests Path:Product as an LPTSTR buffer.
    /// </summary>
    PathProduct
}