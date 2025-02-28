namespace CascLib.NET;

/// <summary>
/// Stream for reading data from a file in <see cref="CascStorage"/>.
/// </summary>
public class CascFileStream : Stream
{
    private readonly IntPtr _hFile;

    /// <summary>
    /// Opens a file in CASC storage for reading.
    /// </summary>
    /// <param name="hStorage">Handle to the CASC storage to open the file from.</param>
    /// <param name="filePath">Path to the file within the <see cref="CascStorage"/>.</param>
    /// <exception cref="FileNotFoundException">Thrown if no file was found at the given path.</exception>
    public CascFileStream(IntPtr hStorage, string filePath)
    {
        CascLib.CascOpenFile(hStorage, filePath, 0, 0, out _hFile);
        if (_hFile == IntPtr.Zero)
        {
            throw new FileNotFoundException("Could not find file in CASC storage", filePath);
        }
    }

    /// <summary>
    /// Whether the file can be read from.
    /// </summary>
    /// <remarks>Always returns <c>true</c>.</remarks>
    public override bool CanRead => true;
    
    /// <summary>
    /// Whether the consumer can move the cursor within the file.
    /// </summary>
    /// <remarks>Always returns <c>true</c>.</remarks>
    public override bool CanSeek => true;
    
    /// <summary>
    /// Whether the file can be written to.
    /// </summary>
    /// <remarks>Always returns <c>false</c>.</remarks>
    public override bool CanWrite => false;

    /// <summary>
    /// Total size of the file, in bytes.
    /// </summary>
    public override long Length
    {
        get
        {
            ulong result = 0;
            CascLib.CascGetFileSize64(_hFile, ref result);
            return (long)result;
        }
    }

    /// <summary>
    /// Gets or sets the position of the cursor within the open file.
    /// </summary>
    public override unsafe long Position
    {
        get
        {
            var pFile = TCascFile.IsValid(_hFile);
            return pFile == null ? 0 : (long)pFile->FilePointer;
        }

        set => CascLib.CascSetFilePointer64(_hFile, value, out _, SeekOrigin.Begin);
    }

    /// <summary>
    /// Clears all buffers for this stream, and causes all buffered data to be written to the underlying device.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown always, as <see cref="CascFileStream"/> does not support writing.
    /// </exception>
    public override void Flush()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Reads a sequence of bytes from the file, and advances the position of the stream by the number of bytes read.
    /// </summary>
    /// <param name="buffer">Buffer to read the data into.</param>
    /// <param name="offset">Index of the byte within the buffer to begin reading the data into.</param>
    /// <param name="count">Total number of bytes to read.</param>
    /// <returns>Number of bytes that were read during this operation.</returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
        uint result;

        if (offset == 0 && count == buffer.Length)
        {
            CascLib.CascReadFile(_hFile, buffer, (uint)count, out result);
        }
        else
        {
            var tempBuffer = new byte[count];
            CascLib.CascReadFile(_hFile, tempBuffer, (uint)count, out result);
            Array.Copy(tempBuffer, 0, buffer, offset, count);
        }

        return (int)result;
    }

    /// <summary>
    /// Sets the <see cref="Position"/> of this stream.
    /// </summary>
    /// <param name="offset">The number of bytes to move the cursor. Accepts negative values.</param>
    /// <param name="origin">Position within the stream that <paramref name="offset"/> is relative to.</param>
    /// <returns>New position of the cursor within the stream.</returns>
    public override long Seek(long offset, SeekOrigin origin)
    {
        CascLib.CascSetFilePointer64(_hFile, offset, out var result, SeekOrigin.Begin);
        return (long)result;
    }

    /// <summary>
    /// Sets the length of the stream.
    /// </summary>
    /// <param name="value">New length of the stream.</param>
    /// <exception cref="NotSupportedException">
    /// Always thrown as <see cref="CascFileStream"/> does not support write operations.
    /// </exception>
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Writes the given byte sequence to the underlying file.
    /// </summary>
    /// <param name="buffer">The bytes to write to the file.</param>
    /// <param name="offset">Position within the buffer to begin writing bytes from.</param>
    /// <param name="count">Number of bytes to write to the file.</param>
    /// <exception cref="NotSupportedException">
    /// Always thrown as <see cref="CascFileStream"/> does not support write operations.
    /// </exception>
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        CascLib.CascCloseFile(_hFile);
    }
}