using System;
using System.IO;

namespace Plainion.GatedCheckIn.Services
{
    class ProducerConsumerStream : Stream
    {
        private readonly MemoryStream myInnerStream;
        private long myReadPosition;
        private long myWritePosition;

        public ProducerConsumerStream()
        {
            myInnerStream = new MemoryStream();
        }

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        public override void Flush()
        {
            lock (myInnerStream)
            {
                myInnerStream.Flush();
            }
        }

        public override long Length
        {
            get
            {
                lock (myInnerStream)
                {
                    return myInnerStream.Length;
                }
            }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (myInnerStream)
            {
                myInnerStream.Position = myReadPosition;
                int red = myInnerStream.Read(buffer, offset, count);
                myReadPosition = myInnerStream.Position;

                return red;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (myInnerStream)
            {
                myInnerStream.Position = myWritePosition;
                myInnerStream.Write(buffer, offset, count);
                myWritePosition = myInnerStream.Position;
            }
        }
    }
}
