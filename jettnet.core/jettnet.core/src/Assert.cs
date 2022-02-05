using System;
using System.Diagnostics;

namespace jettnet.core
{
    public class Assert
    {

        [Conditional("DEBUG")]
        public static void Check(object condition)
        {
            if (condition == null)
            {
                throw new AssertException();
            }
        }

        [Conditional("DEBUG")]
        public static unsafe void Check(void* condition)
        {
            if ((IntPtr) condition == IntPtr.Zero)
            {
                throw new AssertException();
            }
        }

        [Conditional("DEBUG")]
        public static void Check(bool condition)
        {
            if (!condition)
            {
                throw new AssertException();
            }
        }

        [Conditional("DEBUG")]
        public static void Check(bool condition, string error)
        {
            if (!condition)
            {
                throw new AssertException(error);
            }
        }

        public class AssertException : Exception
        {
            public AssertException()
            {
            }

            public AssertException(string msg)
              : base(msg)
            {
            }
        }

    }
}
