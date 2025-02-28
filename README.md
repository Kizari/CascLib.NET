# CascLib.NET

Simple wrapper library for [CascLib by Ladislav Zezula](https://github.com/ladislav-zezula/CascLib) that provides
standardized .NET constructs for accessing files in a CASC storage using P/Invoke to call upon the original
native library. All credit for CascLib goes to Ladislav Zezula and the contributors of CascLib.

> [!WARNING]  
> This library was primarily developed with personal projects in mind, and is currently in alpha. Feel free to
> use it if you have a need for it, but please read the following limitations first!

## Limitations

* Does not currently support every possible function of CascLib, see [Features](#Features)
  for a list of what is supported
* Has the original CascLib DLL bundled in (the CascLib.NET version mirrors the CascLib version used), 
  so cannot be easily used with other versions
* Has only been tested on a couple of version of D2R, but no other games
* Not currently covered by any automated tests, use at your own risk


## Installation

CascLib.NET can be installed through NuGet. You may need to enable prerelease versions to be able to see it.  
Either use your IDE's NuGet package manager, or invoke the following command from a terminal:
```
dotnet add package CascLib.NET
```

## Usage

The following sections explain how to consume this library.


### Open a CASC storage

To open a CASC storage, simply use the `CascStorage` constructor by passing in the path to the storage on disk.

```csharp
const string path = @"C:\path\to\casc\storage";

using var storage = new CascStorage(path);
```

This storage can then be used to access files until disposed.

> [!WARNING]  
> You must dispose of the storage when you are done with it to avoid memory leaks. Either have .NET manage this with
> a `using` statement, or call `storage.Dispose()` manually if you cannot use a `using` statement for any reason.


### Finding files within CASC storage

`CascStorage` implements `IEnumerable<T>`, so it can be used like any other collection type in C# to enumerate files.

```csharp
// Write the name of every file in the CascStorage
foreach (var file in storage)
{
    Console.WriteLine(file.szFileName);
}
```

As such, LINQ operations are also supported.

```csharp
// Get the second group of 10 files with "model" in their path
var models = storage
    .Where(file => file.szFileName.Contains("model"))
    .Skip(10)
    .Take(10);
```

### Reading files

CascLib.NET provides `CascFileStream`, an implementation of `Stream` specifically for reading from files 
in a `CascStorage`.

```csharp
using var itemStream = storage.OpenFile(@"data:data\global\excel\items.txt");
```

This is used like any other stream. Here is an example of how you can write the file to disk in its raw form.

```csharp
using var outStream = new FileStream(@"C:\Output\items.txt", FileMode.Create, FileAccess.Write);
itemStream.CopyTo(outStream);
```

Any other standard read operations are supported, but write operations are not supported for `CascFileStream`.

> [!WARNING]  
> You must dispose of the `CascFileStream` when you are done with it to avoid memory leaks. Either have .NET manage this
> with a `using` statement, or call `stream.Dispose()` manually if you cannot use a `using` statement for any reason.


## Contributing

Any contributions to this repository are welcome, simply open a pull request, I'd be happy to review and merge it.


## License

This repository is licensed under the MIT license, just like the library it wraps.  
As CascLib itself is included in this repository, consumers of this library must also abide by the license of that
library as well. A copy of the license for the bundled version of CascLib is included in the
[Attribution](ATTRIBUTION.md) file in this repository and the distributed NuGet package.