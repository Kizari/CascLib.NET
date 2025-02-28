using System.Diagnostics.CodeAnalysis;

namespace CascLib.NET;

/// <summary>
/// Represents a construct that can access files.
/// </summary>
public interface IDataStorage : IDisposable
{
    /// <summary>
    /// Opens a file for reading.
    /// </summary>
    /// <param name="path">The path to the file in storage.</param>
    /// <returns>An open <see cref="Stream"/> that wraps the target file.</returns>
    /// <remarks>
    /// Caller is responsible for disposing the <see cref="Stream"/> when finished with it.
    /// </remarks>
    /// <exception cref="FileNotFoundException">Thrown if a file cannot be found at the given path.</exception>
    Stream OpenFile(string path);
    
    /// <summary>
    /// Attempts to open a file for reading.
    /// </summary>
    /// <param name="path">The path to the file in storage.</param>
    /// <param name="stream">
    /// An open <see cref="Stream"/> that wraps the target file.
    /// Will be <c>null</c> if this method returns <c>false</c>.</param>
    /// <returns><c>true</c> if the file was opened successfully, otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Caller is responsible for disposing the <see cref="Stream"/> when finished with it.
    /// </remarks>
    bool TryOpenFile(string path, [NotNullWhen(true)] out Stream? stream);
    
    /// <summary>
    /// Reads the contents of an entire file as a UTF8 string.
    /// </summary>
    /// <param name="path">The path to the file in storage.</param>
    /// <returns>String content of the target file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if a file cannot be found at the given path.</exception>
    string ReadFileAsString(string path);
}