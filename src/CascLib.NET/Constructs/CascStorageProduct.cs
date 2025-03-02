using System.Runtime.InteropServices;

namespace CascLib.NET;

/// <summary>
/// The product that a <see cref="CascStorage"/> is associated with.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public struct CascStorageProduct
{
    /// <summary>
    /// Code name of the product (e.g. "wowt" = "World of Warcraft PTR").
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x1C)]
    public string CodeName;

    /// <summary>
    /// Build number of the product.
    /// </summary>
    /// <remarks>
    /// If this is zero, then CascLib did not recognize the build number.
    /// </remarks>
    public uint BuildNumber;
}