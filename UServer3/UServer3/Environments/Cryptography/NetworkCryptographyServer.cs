using System.IO;
using RakNet.Network;

namespace UServer3.Environments.Cryptography
{
    public class NetworkCryptographyServer : NetworkCryptography
    {
        // Methods
        protected override void DecryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
        {
            if (connection.encryptionLevel <= 1)
            {
                Cryptography.XOR(0x7eb, src, srcOffset, dst, dstOffset);
            }
            else
            {
                EACServer.Decrypt(connection, src, srcOffset, dst, dstOffset);
            }
        }

        protected override void EncryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
        {
            if (connection.encryptionLevel <= 1)
            {
                Cryptography.XOR(0x7eb, src, srcOffset, dst, dstOffset);
            }
            else
            {
                EACServer.Encrypt(connection, src, srcOffset, dst, dstOffset);
            }
        }
    }
}