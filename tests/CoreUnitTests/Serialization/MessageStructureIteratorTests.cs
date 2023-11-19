using Kuriba.Core.Messages;

namespace Kuriba.Core.Serialization
{
    public class MessageStructureIteratorTests
    {
        [Message(0)]
        private class EmptyMessage { }

        [Fact]
        public void Empty()
        {
            MessageStructure structure = new(typeof(EmptyMessage));
            MessageStructureIterator iter = new(structure);

            Assert.False(iter.NextField());
            Assert.Throws<InvalidOperationException>(() => iter.Padding);
            Assert.Throws<InvalidOperationException>(() => iter.CurrentField);
        }

        [Message(0)]
        private class MessageWithoutPadding
        {
            [MessageField(0)]
            public int A { get; set; }

            [MessageField(1)]
            public int B { get; set; }

            [MessageField(2)]
            public int C { get; set; }
        }

        [Fact]
        public void NoPadding()
        {
            MessageStructure structure = new(typeof(MessageWithoutPadding));
            MessageStructureIterator iter = new(structure);

            MessageWithoutPadding message = new() { A = 1, B = 2, C = 3 };

            Assert.True(iter.NextField());
            Assert.Equal(0, iter.Padding);

            Assert.True(iter.NextField());
            Assert.Equal(0, iter.Padding);

            Assert.True(iter.NextField());
            Assert.Equal(0, iter.Padding);

            Assert.False(iter.NextField());
            Assert.Throws<InvalidOperationException>(() => iter.Padding);
            Assert.Throws<InvalidOperationException>(() => iter.CurrentField);
        }

        [Message(0)]
        private class MessageWithPadding
        {
            [MessageField(4)]
            public int A { get; set; }

            [MessageField(13)]
            public int B { get; set; }

            [MessageField(100)]
            public int C { get; set; }
        }

        [Fact]
        public void Padding()
        {
            MessageStructure structure = new(typeof(MessageWithPadding));
            MessageStructureIterator iter = new(structure);

            MessageWithPadding message = new() { A = 1, B = 2, C = 3 };

            Assert.True(iter.NextField());
            Assert.Equal(4, iter.Padding);

            Assert.True(iter.NextField());
            Assert.Equal(8, iter.Padding);

            Assert.True(iter.NextField());
            Assert.Equal(86, iter.Padding);

            Assert.False(iter.NextField());
            Assert.Throws<InvalidOperationException>(() => iter.Padding);
            Assert.Throws<InvalidOperationException>(() => iter.CurrentField);
        }
    }
}