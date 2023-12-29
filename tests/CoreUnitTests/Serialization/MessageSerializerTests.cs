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

        [Fact]
        public void RoundTrip()
        {
            var originalMessage1 = new Message1();
            var finalMessage1 = MessageSerializer.Deserialize(MessageSerializer.Serialize(originalMessage1));

            Assert.IsType<Message1>(finalMessage1);
            
            var originalMessage2 = new Message2 { A = 37, B = "RWBY", C = new List<double> { 1.3, -99.9 } };
            var finalMessage2 = MessageSerializer.Deserialize(MessageSerializer.Serialize(originalMessage2));

            Assert.IsType<Message2>(finalMessage2);
            var finalMessage2CorrectType = (Message2)finalMessage2;
            Assert.Equal(37, finalMessage2CorrectType.A);
            Assert.Equal("RWBY", finalMessage2CorrectType.B);
            Assert.Equal(2, finalMessage2CorrectType.C.Count);
            Assert.Equal(1.3, finalMessage2CorrectType.C[0]);
            Assert.Equal(-99.9, finalMessage2CorrectType.C[1]);
        }
    }
}
