namespace Kuriba.Core.Serialization;

public class PeekableBinaryReaderTests
{
    [Fact]
    void ReadByte()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var reader = new PeekableBinaryReader(new BinaryReader(stream));
        
        Assert.Equal(1, reader.ReadByte());
        Assert.Equal(2, reader.ReadByte());
        Assert.Equal(3, reader.ReadByte());
        Assert.Throws<EndOfStreamException>(() => reader.ReadByte());
    }
    
    [Fact]
    void ReadBytes()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7 });
        var reader = new PeekableBinaryReader(new BinaryReader(stream));
        
        Assert.Equal(new byte[] {1, 2, 3, 4, 5}, reader.ReadBytes(5));
        Assert.Throws<EndOfStreamException>(() => reader.ReadBytes(3));
        Assert.Equal(new byte[] {6, 7}, reader.ReadBytes(2));
    }

    [Fact]
    void PeekByte()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3});
        var reader = new PeekableBinaryReader(new BinaryReader(stream));
        
        // calling again doesn't change the result
        Assert.Equal(1, reader.PeekByte());
        Assert.Equal(1, reader.PeekByte());
        Assert.Equal(1, reader.PeekByte());
        
        // .. until we read/skip the bytes
        reader.SkipByte();
        Assert.Equal(2, reader.PeekByte());
        
        Assert.Equal(2, reader.ReadByte());
        Assert.Equal(3, reader.PeekByte());
        Assert.Equal(3, reader.ReadByte());
    }

    [Fact]
    void SkipByte()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3});
        var reader = new PeekableBinaryReader(new BinaryReader(stream));
        
        reader.SkipByte();
        Assert.Equal(2, reader.ReadByte());
        reader.SkipByte();
        Assert.Throws<EndOfStreamException>(() => reader.ReadByte());
    }
}