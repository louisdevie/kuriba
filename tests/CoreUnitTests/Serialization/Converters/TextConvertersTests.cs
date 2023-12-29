using Microsoft.VisualStudio.CodeCoverage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Xunit;

namespace Kuriba.Core.Serialization.Converters
{
    public class TextConvertersTests
    {
        [Fact]
        public void Char()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            MessageReader messageReader = new(new BinaryReader(output));
            Converter converter = new TextConverters.CharConverter();

            //=============== 1-BYTE CHARACTER ================//

            converter.Write('a', messageWriter);

            Assert.Equal(
                new byte[3]
                {
                    0x1f, // variable length         |
                    0x01, // actual data length is 1 | header
                    0x61, // character
                },
                output.ToArray()
            );

            output.Position = 0;
            object? result = converter.Read(typeof(char), messageReader);

            Assert.IsType<char>(result);
            Assert.Equal('a', result);
            
            // reset memory stream without reallocating
            output.SetLength(0);

            //=============== 2-BYTES CHARACTER ===============//

            converter.Write('ñ', messageWriter);

            Assert.Equal(
                new byte[4]
                {
                    0x1f, // variable length         |
                    0x02, // actual data length is 2 | header
                    0xc3, // |
                    0xb1, // | character
                },
                output.ToArray()
            );

            output.Position = 0;
            result = converter.Read(typeof(char), messageReader);

            Assert.IsType<char>(result);
            Assert.Equal('ñ', result);
            
            // reset memory stream without reallocating
            output.SetLength(0);

            //=============== 3-BYTES CHARACTER ===============//

            converter.Write('♫', messageWriter);

            Assert.Equal(
                new byte[5]
                {
                    0x1f, // variable length         |
                    0x03, // actual data length is 3 | header
                    0xe2, // |
                    0x99, // | character
                    0xab, // |
                },
                output.ToArray()
            );

            output.Position = 0;
            result = converter.Read(typeof(char), messageReader);

            Assert.IsType<char>(result);
            Assert.Equal('♫', result);
        }

        [Fact]
        public void Rune()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            MessageReader messageReader = new(new BinaryReader(output));
            Converter converter = new TextConverters.RuneConverter();

            //=============== 1-BYTE CHARACTER ================//

            converter.Write(new Rune('a'), messageWriter);

            Assert.Equal(
                new byte[3]
                {
                    0x1f, // variable length         |
                    0x01, // actual data length is 1 | header
                    0x61, // character
                },
                output.ToArray()
            );

            output.Position = 0;
            object? result = converter.Read(typeof(Rune), messageReader);

            Assert.IsType<Rune>(result);
            Assert.Equal(new Rune('a'), result);

            // reset memory stream without reallocating
            output.SetLength(0);

            //=============== 2-BYTES CHARACTER ===============//

            converter.Write(new Rune('ñ'), messageWriter);

            Assert.Equal(
                new byte[4]
                {
                    0x1f, // variable length         |
                    0x02, // actual data length is 2 | header
                    0xc3, // |
                    0xb1, // | character
                },
                output.ToArray()
            );

            output.Position = 0;
            result = converter.Read(typeof(Rune), messageReader);

            Assert.IsType<Rune>(result);
            Assert.Equal(new Rune('ñ'), result);

            // reset memory stream without reallocating
            output.SetLength(0);

            //=============== 3-BYTES CHARACTER ===============//

            converter.Write(new Rune('♫'), messageWriter);

            Assert.Equal(
                new byte[5]
                {
                    0x1f, // variable length         |
                    0x03, // actual data length is 3 | header
                    0xe2, // |
                    0x99, // | character
                    0xab, // |
                },
                output.ToArray()
            );

            output.Position = 0;
            result = converter.Read(typeof(Rune), messageReader);

            Assert.IsType<Rune>(result);
            Assert.Equal(new Rune('♫'), result);

            // reset memory stream without reallocating
            output.SetLength(0);

            //=============== 4-BYTES CHARACTER ===============//

            converter.Write(new Rune(127828), messageWriter);

            Assert.Equal(
                new byte[6]
                {
                    0x1f, // variable length         |
                    0x04, // actual data length is 4 | header
                    0xf0, // |
                    0x9f, // |
                    0x8d, // | hamburger :)
                    0x94, // |
                },
                output.ToArray()
            );

            output.Position = 0;
            result = converter.Read(typeof(Rune), messageReader);

            Assert.IsType<Rune>(result);
            Assert.Equal(new Rune(127828), result);
        }

        [Fact]
        public void String()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            MessageReader messageReader = new(new BinaryReader(output));
            Converter converter = new TextConverters.StringConverter();

            //================= EMPTY STRING ==================//

            converter.Write("", messageWriter);

            Assert.Equal(
                new byte[2]
                {
                    0x1f, // variable length         |
                    0x00, // actual data length is 0 | header
                },
                output.ToArray()
            );

            output.Position = 0;
            object? result = converter.Read(typeof(string), messageReader);

            Assert.IsType<string>(result);
            Assert.Equal("", result);

            // reset memory stream without reallocating
            output.SetLength(0);

            //================= SHORT STRING ==================//

            converter.Write("Hello, World!", messageWriter);

            Assert.Equal(
                new byte[15]
                {
                    0x1f, // variable length          |
                    0x0d, // actual data length is 13 | header
                    0x48, 0x65, 0x6C, // |
                    0x6C, 0x6F, 0x2C, // |
                    0x20, 0x57, 0x6F, // | text
                    0x72, 0x6C, 0x64, // |
                    0x21, //             |
                },
                output.ToArray()
            );

            output.Position = 0;
            result = converter.Read(typeof(string), messageReader);

            Assert.IsType<string>(result);
            Assert.Equal("Hello, World!", result);

            // reset memory stream without reallocating
            output.SetLength(0);

            //=============== VERY LONG STRING ================//

            // entire novel
            const string TEXT = """
                                “Shokuhou, do it now!”
                                Mikoto interrupted with a command to the other girl.
                                While still clinging to Mikoto from behind, Shokuhou Misaki pulled a TV remote
                                from the small bag she wore over her shoulder and gently pressed it against the
                                back of(daydreaming) Shirai Kuroko’s head.
                                She pressed a silent button and the chestnut twintails girl’s head wobbled slightly.
                                Shokuhou Misaki’s Mental Out was the strongest psychological power.
                                But it had such a broad range of applications that it was difficult to control
                                even for her, so she used different remotes as a form of self - suggestion to
                                create categories for her power.
                                And of course…
                                “Hey.”
                                The teachers knew what her power could do, so tension ran through the one monitoring
                                them once the blonde girl reached for her bag. She reflexively called out in a strict voice.
                                “Shirai, where did you find that remote ? Was the actual digital recorder not
                                thrown out along with it! ?”
                                “What ?”
                                She was focused on the remote, but not in the right way.
                                However, the teacher did not notice the shift in her own thoughts.
                                “What remote ? This is a kamaboko board.”
                                But the item the confused twintail girl waved in her defense was an empty chocolate bar box.
                                “No, you definitely had a remote.It has to be around there somewhere!”
                                “Again, this is a kamaboko board.”
                                “It is a remote!!”
                                The two of them got unnaturally particular about something meaningless.
                                Meanwhile, the honey - blonde girl with the real remote in hand was laughing. She
                                was of course within both arguers’ field of vision, but neither one mentioned her
                                at all. As usual, that girl had a knack for mischief.
                                The minor argument between student and teacher created a disturbance in their
                                ranks as the sheltered girls gathered around to watch.
                                “What about the smart glasses ?” asked Shokuhou with a wink.
                                “Already dealt with.”
                                Even if they were running off, they could not just throw out the tongs and
                                half-filled trash bags they were using, so they left them by the main road where
                                the robots would find them.
                                Now came the tricky part.
                                Mikoto gave her casual response and then slipped away from the crowd into an alley
                                between multi - tenant buildings.
                                She removed the GPS tracker locked to her right ankle and did the same for
                                Shokuhou who had come with her and she tossed them into the gap between beer
                                cases piled up nearby.Then she wrapped her arms around the blonde girl’s skinny
                                waist and leaped straight up.She used her power over magnetism to use the
                                reinforced concrete wall as a foothold and ran all the way up to the 5 - story
                                building’s rooftop.It was a lot like using the giant lifting magnet that cranes
                                used to move abandoned cars in a scrap yard.
                                This was an example of Academy City’s esper powers.
                                """;

            converter.Write(TEXT, messageWriter);

            int encodedSize = Encoding.UTF8.GetEncoder().GetByteCount(TEXT, true);
            byte[] expectedResult = new byte[encodedSize + 3];
            expectedResult[0] = 0x1f; // variable length                                           |
            expectedResult[1] = (byte)((encodedSize >> 7) & 0x7f | 0x80); // | actual size         | header
            expectedResult[2] = (byte)(encodedSize & 0x7f); //              | split over two bytes |
            Array.Copy(Encoding.UTF8.GetBytes(TEXT), 0, expectedResult, 3, encodedSize); // then the data

            Assert.Equal(expectedResult, output.ToArray());

            output.Position = 0;
            result = converter.Read(typeof(string), messageReader);

            Assert.IsType<string>(result);
            Assert.Equal(TEXT, result);
        }
    }
}
