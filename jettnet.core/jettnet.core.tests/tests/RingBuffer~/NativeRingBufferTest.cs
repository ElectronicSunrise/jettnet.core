using Xunit;
using Xunit.Abstractions;

namespace jettnet.core.tests
{
    public class NativeRingBufferTest
    {
        private readonly ITestOutputHelper output;

        public NativeRingBufferTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public unsafe void WriteReturnNullIfFull()
        {
            Logger logger = new Logger("TEST");

            NativeRingBuffer nativeRingBuffer = new NativeRingBuffer(10, 5, logger);

            bool isNull;
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(isNull);

        }

        [Fact]
        public unsafe void ReadReturnNullIfEmpty()
        {
            Logger logger = new Logger("TEST");

            NativeRingBuffer nativeRingBuffer = new NativeRingBuffer(10, 5, logger);

            bool isNull;
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Reserve() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Read() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Read() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Read() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Read() == null;
            Xunit.Assert.True(!isNull);
            isNull = nativeRingBuffer.Read() == null;
            Xunit.Assert.True(isNull);

        }

    }
}