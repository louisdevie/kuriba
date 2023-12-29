namespace Kuriba.Core.Serialization
{
    /// <summary>
    /// A class that reads from a message buffer.
    /// </summary>
    public interface IMessageReader
    {
        /// <summary>
        /// Reads a field with a fixed size of 128 bits.
        /// </summary>
        /// <returns>The read data in an array of bytes.</returns>
        byte[] Read128BitsField();
        
        /// <summary>
        /// Reads a field with a fixed size of 16 bits.
        /// </summary>
        /// <returns>The read data in an array of bytes.</returns>
        byte[] Read16BitsField();
        
        /// <summary>
        /// Reads a field with a fixed size of 32 bits.
        /// </summary>
        /// <returns>The read data in an array of bytes.</returns>
        byte[] Read32BitsField();
        
        /// <summary>
        /// Reads a field with a fixed size of 64 bits.
        /// </summary>
        /// <returns>The read data in an array of bytes.</returns>
        byte[] Read64BitsField();
        
        /// <summary>
        /// Reads a field with a fixed size of 8 bits.
        /// </summary>
        /// <returns>The read data in an array of bytes.</returns>
        byte Read8BitsField();

        /// <summary>
        /// Reads a field of variable size.
        /// </summary>
        /// <returns></returns>
        byte[] ReadVarField();

        /// <summary>
        /// Reads an array header.
        /// </summary>
        /// <returns>The size of the following array.</returns>
        ushort ReadArrayHeader();
    }
}
