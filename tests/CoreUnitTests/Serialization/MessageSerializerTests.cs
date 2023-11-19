using Kuriba.Core.Messages;

namespace Kuriba.Core.Serialization
{
    public class MessageSerializerTests
    {
        [Message(1)]
        private class Message1
        {
            [MessageField(0)]
            public int A { get; set; }
        }

        [Fact]
        public void Serialize()
        {
            Message1 message = new()
            {
                A = 123,
            };

            byte[] result = MessageSerializer.Serialize(message);

            Assert.Equal(
                new byte[7] {
                    0x01, // message ID
                    0x00, // one field
                    0x04, // payload size : four bytes |
                    0x7b, // |                         |
                    0x00, // | payload                 | field 1
                    0x00, // |                         |
                    0x00  // |                         |
                },
                result
            );
        }
    }
}
