using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace jettnet.core 
{
    /// <summary>
    /// Single producer, single consumer, FIFO thread safe ring buffer
    /// </summary>
    public unsafe class NativeRingBuffer : IDisposable
    {
        private Logger _logger;

        private SpinLock _spinLock;

        private void* _buffer;
        private void* _readPointer;
        private void* _writePointer;
        private void* _tail;

        private int _elementSize;
        private int _elementCount;

        private bool _disposedValue;

        private int _lockContentionCount = 0;

        public NativeRingBuffer(int elementSize, int elementCount, Logger logger) 
        {
            // If compiled in debug mode thread tracking is enabled, otherwise disabled
#if DEBUG
            _spinLock = new SpinLock(true);
#else
            _spinLock = new SpinLock(false);
#endif

            this._elementSize = elementSize;
            this._elementCount = elementCount + 1; // plus 1 since there is always one element between read and write pointer which cant be written to

            _buffer = (void*) Marshal.AllocHGlobal(this._elementSize * this._elementCount);
            _writePointer = _buffer;
            _readPointer = _buffer;
            _tail = (byte*) _buffer + (this._elementSize * this._elementCount);
        }

        /// <summary>
        /// Reserves a new element in the buffer
        /// </summary>
        /// <returns>Returns a memory location in the buffer or null if buffer is full</returns>
        public void* Reserve() 
        {
            void* writePosition = null;

            bool lockTaken = false;
            try 
            {
                CheckForContention();
                _spinLock.Enter(ref lockTaken);

                // Get next element position
                void* nextElement = (byte*) _writePointer + _elementSize;
                if (nextElement == _tail) 
                {
                    nextElement = _buffer;
                }

                // Check if the buffer is full
                if (nextElement == _readPointer) 
                {
                    return null;
                }

                writePosition = _writePointer;

                // Advance write pointer
                _writePointer = nextElement;
            } 
            finally 
            {
                if (lockTaken)
                {
                    _spinLock.Exit(false);
                }
            }

            return writePosition;
        }

        /// <summary>
        /// Returns a memory location in the buffer to the next element that can be read
        /// </summary>
        /// <returns>Returns a memory location in the buffer or null if buffer is empty</returns>
        public void* Read() 
        {
            void* readPosition = null;

            bool lockTaken = false;
            try 
            {
                CheckForContention();
                _spinLock.Enter(ref lockTaken);

                // Get next element position
                void* nextElement = (byte*) _readPointer + _elementSize;
                if (nextElement == _tail) 
                {
                    nextElement = _buffer;
                }

                // Check if the buffer is empty, checks if the next element is the write pointer
                if (nextElement == _writePointer) 
                {
                    return null;
                }

                readPosition = _readPointer;

                // Advance read pointer
                _readPointer = nextElement;
            } 
            finally 
            {
                if (lockTaken) 
                { 
                    _spinLock.Exit(false);
                }
            }


            return readPosition;
        }

        [Conditional("DEBUG")]
        private void CheckForContention() 
        {
            Assert.Check(_spinLock.IsThreadOwnerTrackingEnabled, "Thread tracking is not enabled for spinlock in debug mode");
            if (_spinLock.IsHeld) 
            {
                int lockContentions = Interlocked.Increment(ref _lockContentionCount);
                _logger.Log($"NativeRingBuffer lock was contended. Total contention count {lockContentions}", LogLevel.Info);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer and set large fields to null
                Marshal.FreeHGlobal((IntPtr) _buffer);

                _disposedValue = true;
            }
        }

        ~NativeRingBuffer()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}