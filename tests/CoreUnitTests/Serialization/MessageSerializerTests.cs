using Kuriba.Core.Messages;

namespace Kuriba.Core.Serialization
{
    public class MessageSerializerTests
    {
        [Message(1)]
        private class Message1
        {
        }

        [Message(2)]
        private class Message2
        {
            [MessageField(0)]
            public int A { get; set; }

            [MessageField(1)]
            public string B { get; set; } = string.Empty;

            [MessageField(2)]
            public List<double> C { get; set; } = new();
        }

        [Message(3)]
        private class Message3
        {
            [MessageField(0)]
            public Message2[] X { get; set; } = Array.Empty<Message2>();
        }
    }
}
