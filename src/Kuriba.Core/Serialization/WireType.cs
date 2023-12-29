namespace Kuriba.Core.Serialization
{
    internal enum WireType : byte
    {
        Padding = 0x00,

        Bit8 = 0x01,
        Bit16 = 0x02,
        Bit32 = 0x04,
        Bit64 = 0x08,
        Bit128 = 0x10,
        BitVar = 0x1f,
        
        Array = 0x80,

        Nothing = 0xff,
    }
}
