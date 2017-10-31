using System;

namespace RakNet.Network
{
    public abstract class NetworkPeer
    {
        public Write write;

        public Read read;

        public Action<Message> onMessage;
        public INetworkCryptocraphy cryptography;
        public Func<int, Read, uint, int, bool> onUnconnectedMessage;

        protected NetworkPeer()
        {
        }

        public abstract void Send();
        

        protected Message StartMessage(Message.Type type, Connection connection)
        {
            Message message = new Message
            {
                peer = this,
                type = type,
                connection = connection
            };
            return message;
        }
        
        public enum StatTypeLong
        {
            BytesSent,
            BytesSent_LastSecond,
            BytesReceived,
            BytesReceived_LastSecond,
            MessagesInSendBuffer,
            BytesInSendBuffer,
            MessagesInResendBuffer,
            BytesInResendBuffer,
            PacketLossAverage,
            PacketLossLastSecond,
            ThrottleBytes
        }
    }
}