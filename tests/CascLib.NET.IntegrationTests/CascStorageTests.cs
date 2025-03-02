namespace CascLib.NET.UnitTests;

public class CascStorageTests
{
    [Fact]
    public void Enumerator_WhenMoveNext_ReadsStructCorrectly()
    {
        using var storage = new CascStorage(Configuration.StoragePath);
        
        var nameTypes = Enum.GetValues<CascNameType>();
        
        foreach (var item in storage)
        {
            // Ensure that the plain name is a substring of the full name
            Assert.Contains(item.PlainName, item.FileName);
            
            // If any file is larger than 500GB, something probably went wrong with the reading
            Assert.True(item.FileSize < 500L * 1024 * 1024 * 1024);
            
            // Ensure no unknown name types appear in the record
            Assert.Contains(item.NameType, nameTypes);
        }
    }
}