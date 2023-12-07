using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization
{
    /// <summary>
    /// A class that reads from a message buffer.
    /// </summary>
    public interface IMessageReader
    {
        byte[] Read128BitsField();
        byte[] Read16BitsField();
        byte[] Read32BitsField();
        byte[] Read64BitsField();
        byte Read8BitsField();
        bool TryReadOptional128BitsField(out byte[] bytes);
        bool TryReadOptional16BitsField(out byte[] bytes);
        bool TryReadOptional32BitsField(out byte[] bytes);
        bool TryReadOptional64BitsField(out byte[] bytes);
        bool TryReadOptional8BitsField(out byte value);
    }
}
