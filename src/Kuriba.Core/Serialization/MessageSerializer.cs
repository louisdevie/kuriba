using System;
using System.IO;
using Kuriba.Core.Exceptions;

namespace Kuriba.Core.Serialization
{
    /// <summary>
    /// A static class providing methods to (de)serialize messages.
    /// </summary>
    public static class MessageSerializer
    {
        /// <summary>
        /// Serialize a message into a byte array.
        /// </summary>
        /// <param name="type">The type of the message to serialize.</param>
        /// <param name="value">The message to serialize.</param>
        /// <returns>A new byte array.</returns>
        public static byte[] Serialize(Type type, object? value)
        {
            MemoryStream stream = new MemoryStream();
            MessageWriter writer = new MessageWriter(new BinaryWriter(stream));
            writer.WriteMessage(type, value);
            return stream.ToArray();
        }

        /// <summary>
        /// Serialize a message into a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the message to serialize.</typeparam>
        /// <param name="message">The message to serialize.</param>
        /// <returns>A new byte array.</returns>
        public static byte[] Serialize<T>(T message)
        {
            return Serialize(typeof(T), message);
        }

        /// <summary>
        /// Serialize a message into a stream via a binary writer. 
        /// </summary>
        /// <param name="type">The type of the message to serialize.</param>
        /// <param name="value">The message to serialize.</param>
        /// <param name="output">A binary writer to the stream.</param>
        /// <param name="writeOnce">
        /// If <see langword="false"/> (the default), the message may be written
        /// bit by bit. <br/> Setting it to <see langword="true"/> will write
        /// the message to a buffer before passing it to the stream.
        /// </param>
        public static void SerializeTo(Type type, object? value, BinaryWriter output, bool writeOnce = false)
        {
            if (writeOnce)
            {
                byte[] buffer = Serialize(type, value);
                output.Write(buffer);
            }
            else
            {
                MessageWriter writer = new MessageWriter(output);
                writer.WriteMessage(type, value);
            }
        }

        /// <summary>
        /// Serialize a message into a stream via a binary writer. 
        /// </summary>
        /// <typeparam name="T">The type of the message to serialize.</typeparam>
        /// <param name="value">The message to serialize.</param>
        /// <param name="output">A binary writer to the stream.</param>
        /// <param name="writeOnce">
        /// If <see langword="false"/> (the default), the message may be written
        /// bit by bit. <br/> Setting it to <see langword="true"/> will write
        /// the message to a buffer before passing it to the stream.
        /// </param>
        public static void SerializeTo<T>(T value, BinaryWriter output, bool writeOnce = false)
        {
            SerializeTo(typeof(T), value, output, writeOnce);
        }

        /// <summary>
        /// Deserialize a message from a byte array.
        /// </summary>
        /// <param name="data">The binary data to deserialize.</param>
        /// <returns>The deserialized message.</returns>
        public static object? Deserialize(byte[] data)
        {
            var stream = new MemoryStream(data);
            var reader = new MessageReader(new BinaryReader(stream));
            return reader.ReadMessage();
        }

        /// <summary>
        /// Deserialize a message from a byte array and tries to cast it into a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the message to serialize.</typeparam>
        /// <param name="data">The binary data to deserialize.</param>
        /// <returns>The deserialized message.</returns>
        public static T Deserialize<T>(byte[] data)
        {
            var stream = new MemoryStream(data);
            var reader = new MessageReader(new BinaryReader(stream));
            return (T)reader.ReadMessage(typeof(T))!;
        }

        /// <summary>
        /// Deserialize a message from a stream via a binary reader. 
        /// </summary>
        /// <param name="input">A binary reader to the stream.</param>
        public static object? DeserializeFrom(BinaryReader input)
        {
            var reader = new MessageReader(input);
            return reader.ReadMessage();
        }

        /// <summary>
        /// Deserialize a message from a stream via a binary reader and tries to cast it into a <typeparamref name="T"/>. 
        /// </summary>
        /// <typeparam name="T">The type of the message to serialize.</typeparam>
        /// <param name="input">A binary reader to the stream.</param>
        public static T DeserializeFrom<T>(BinaryReader input)
        {
            var reader = new MessageReader(input);
            return (T)reader.ReadMessage(typeof(T))!;
        }
    }
}