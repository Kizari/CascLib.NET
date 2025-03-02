using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace CascLib.NET;

/// <summary>
/// Provides access to files in a CASC storage.<br/>
/// Can be enumerated to access files sequentially.
/// </summary>
/// <remarks>
/// Consumers must dispose the storage when finished with it to free up resources.
/// </remarks>
public sealed class CascStorage : IEnumerable<CascFindData>, IDataStorage
{
    private readonly Enumerator _enumerator;
    private readonly IntPtr _hStorage;

    /// <summary>
    /// Opens the CASC storage at the given file path.
    /// </summary>
    /// <param name="path">
    /// Path to the CASC storage on disk, usually the directory that contains the game executable.
    /// </param>
    /// <exception cref="Win32Exception">Thrown if the CASC storage fails to open.</exception>
    public CascStorage(string path)
    {
        if (!CascLib.CascOpenStorage(path, 0, out _hStorage))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        _enumerator = new Enumerator(_hStorage);
    }

    /// <summary>
    /// Closes the CASC storage to free up resources.
    /// </summary>
    public void Dispose()
    {
        _enumerator.Dispose();
        
        if (_hStorage != IntPtr.Zero)
        {
            CascLib.CascCloseStorage(_hStorage);
        }
    }

    /// <summary>
    /// Number of local files in the storage.
    /// </summary>
    /// <remarks>
    /// Files can exist under multiple different names, so the total number of files in the archive can be higher than
    /// the value returned by this info class.
    /// </remarks>
    public int LocalFileCount => 
        (int)CascLib.CascGetStorageInfo<uint>(_hStorage, CascStorageInfoClass.LocalFileCount);
    
    /// <summary>
    /// Total number of files in the storage, including files that have not been downloaded.
    /// </summary>
    public int TotalFileCount =>
        (int)CascLib.CascGetStorageInfo<uint>(_hStorage, CascStorageInfoClass.TotalFileCount);

    /// <summary>
    /// Features that are available on this storage.
    /// </summary>
    public CascFeature Features => 
        CascLib.CascGetStorageInfo<CascFeature>(_hStorage, CascStorageInfoClass.Features);

    /// <summary>
    /// Information about the product associated with this storage.
    /// </summary>
    public CascStorageProduct Product =>
        CascLib.CascGetStorageInfo<CascStorageProduct>(_hStorage, CascStorageInfoClass.Product);
    
    /// <summary>
    /// Information about tags included in the storage.
    /// </summary>
    public CascStorageTags Tags =>
        CascLib.CascGetStorageInfo<CascStorageTags>(_hStorage, CascStorageInfoClass.Tags);

    /// <summary>
    /// Open parameters of the storage.
    /// </summary>
    /// <remarks>
    /// AKA <c>LocalPath:ProductCode</c>.
    /// </remarks>
    public string PathProduct =>
        CascLib.CascGetStorageInfo<string>(_hStorage, CascStorageInfoClass.PathProduct);

    /// <inheritdoc />
    public Stream OpenFile(string path) => new CascFileStream(_hStorage, path);

    /// <inheritdoc />
    public bool TryOpenFile(string path, [NotNullWhen(true)]out Stream? stream)
    {
        try
        {
            stream = new CascFileStream(_hStorage, path);
            return true;
        }
        catch (FileNotFoundException)
        {
            stream = null;
            return false;
        }
    }

    /// <inheritdoc />
    public string ReadFileAsString(string path)
    {
        using var stream = OpenFile(path);
        var buffer = new byte[stream.Length];
        stream.ReadExactly(buffer);
        return Encoding.UTF8.GetString(buffer);
    }

    /// <inheritdoc />
    public IEnumerator<CascFindData> GetEnumerator() => _enumerator;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => _enumerator;

    /// <summary>
    /// Supports iteration over files in a <see cref="CascStorage"/>.
    /// </summary>
    /// <param name="hStorage">Handle to the open CASC storage.</param>
    private class Enumerator(IntPtr hStorage) : IEnumerator<CascFindData>
    {
        private CascFindData _current;
        private IntPtr _hFind;

        /// <summary>
        /// Advances the enumerator to the next file in the CASC storage.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the enumerator was successfully advanced to the next file,
        /// <c>false</c> if the enumerator has passed the end of the storage.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown if CascLib cannot find the first file in the storage.
        /// </exception>
        public bool MoveNext()
        {
            if (_hFind == IntPtr.Zero)
            {
                var pFirst = Marshal.AllocHGlobal(Marshal.SizeOf<CascFindData>());
                _hFind = CascLib.CascFindFirstFile(hStorage, "*", pFirst, null);
                if (_hFind == IntPtr.Zero)
                {
                    throw new FileNotFoundException("Failed to find first file in CASC storage");
                }
                
                _current = Marshal.PtrToStructure<CascFindData>(pFirst);
                Marshal.FreeHGlobal(pFirst);
                return true;
            }

            var pNext = Marshal.AllocHGlobal(Marshal.SizeOf<CascFindData>());
            CascLib.CascFindNextFile(_hFind, pNext);
            _current = Marshal.PtrToStructure<CascFindData>(pNext);
            Marshal.FreeHGlobal(pNext);

            if (_current.ContentKey.SequenceEqual(new byte[16]))
            {
                Reset();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resets the enumerator back to the start of the CASC storage.
        /// </summary>
        public void Reset()
        {
            if (_hFind != IntPtr.Zero)
            {
                CascLib.CascFindClose(_hFind);
            }
            
            _hFind = IntPtr.Zero;
        }

        /// <inheritdoc/>
        object IEnumerator.Current => _current;

        /// <inheritdoc/>
        public CascFindData Current => _current;

        /// <summary>
        /// Frees resources associated with the enumerator by closing the file cursor handle.
        /// </summary>
        public void Dispose()
        {
            if (_hFind != IntPtr.Zero)
            {
                CascLib.CascFindClose(_hFind);
            }
        }
    }
}