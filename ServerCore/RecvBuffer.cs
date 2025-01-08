
namespace ServerCore
{
    class RecvBuffer
    {
        ArraySegment<byte> m_Buffer;
        int m_ReadPos = 0;
        int m_WritePos = 0;

        int DataSize { get { return m_WritePos - m_ReadPos; } }
        int FreeSize { get { return m_Buffer.Count - m_WritePos; } }

        public ArraySegment<byte> ReadSegemnt
        {
            get { return new ArraySegment<byte>(m_Buffer.Array, m_Buffer.Offset + m_ReadPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegemnt
        {
            get { return new ArraySegment<byte>(m_Buffer.Array, m_Buffer.Offset + m_WritePos, FreeSize); }
        }

        public RecvBuffer(int bufferSize)
        {
            m_Buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public bool OnRead(int readSize)
        {
            if (DataSize < readSize)
            {
                Console.WriteLine("RecvBuffer : 할당된 세그먼트보다 큰 크기를 읽을려고 합니다.");
                return false;
            }
                
            m_ReadPos += readSize;

            return true;
        }

        public bool OnWrite(int writeSize)
        {
            if (FreeSize < writeSize)
            {
                Console.WriteLine("RecvBuffer : 할당된 세그먼트보다 큰 크기를 쓰려고 합니다.");
                return false;
            }
             
            m_WritePos += writeSize;

            return true;
        }

        public void Clear()
        {
            if (DataSize == 0)
            {
                m_ReadPos = m_WritePos = 0;
            }
            else
            {
                Array.Copy(m_Buffer.Array, m_Buffer.Offset + m_ReadPos, m_Buffer.Array, m_Buffer.Offset, DataSize);
                m_ReadPos = 0;
                m_WritePos = DataSize;
            }
        }
    }
}