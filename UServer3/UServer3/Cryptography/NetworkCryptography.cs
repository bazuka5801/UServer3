using System.IO;
using AntiRak.RakNet.Network;

namespace UServer3.Cryptography
{
    public abstract class NetworkCryptography : INetworkCryptocraphy
    {
        // Fields
        private MemoryStream buffer = new MemoryStream();

        // Methods
        protected NetworkCryptography()
        {
        }

        public void Decrypt(Connection connection, MemoryStream stream, int offset)
        {
            this.DecryptionHandler(connection, stream, offset, stream, offset);
        }

        public MemoryStream DecryptCopy(Connection connection, MemoryStream stream, int offset)
        {
            this.buffer.Position = 0L;
            this.buffer.SetLength(0L);
            this.buffer.Write(stream.GetBuffer(), 0, offset);
            this.DecryptionHandler(connection, stream, offset, this.buffer, offset);
            return this.buffer;
        }

        protected abstract void DecryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset);
        
        public void Encrypt(Connection connection, MemoryStream stream, int offset)
        {
            this.EncryptionHandler(connection, stream, offset, stream, offset);
        }

        public MemoryStream EncryptCopy(Connection connection, MemoryStream stream, int offset)
        {
            this.buffer.Position = 0L;
            this.buffer.SetLength(0L);
            this.buffer.Write(stream.GetBuffer(), 0, offset);
            this.EncryptionHandler(connection, stream, offset, this.buffer, offset);
            return this.buffer;
        }

        protected abstract void EncryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset);
        public bool IsEnabledIncoming(Connection connection)
        {
            return (((connection != null) && (connection.encryptionLevel > 0)) && connection.decryptIncoming);
        }

        public bool IsEnabledOutgoing(Connection connection)
        {
            return (((connection != null) && (connection.encryptionLevel > 0)) && connection.encryptOutgoing);
        }
    }


}