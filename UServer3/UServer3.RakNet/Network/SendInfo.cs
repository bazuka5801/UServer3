using System.Collections.Generic;

namespace RakNet.Network
{
    public struct SendInfo
    {
        public SendMethod method;

        public sbyte channel;

        public Priority priority;

        public Connection connection;
        
        public List<Connection> connections;
        
        
        public SendInfo(List<Connection> connections)
        {
            this = new SendInfo()
            {
                channel = 0,
                method = SendMethod.Reliable,
                priority = Priority.Medium,
                connections = connections
            };
        }

        public SendInfo(Connection connection)
        {
            this = new SendInfo()
            {
                channel = 0,
                method = SendMethod.Reliable,
                priority = Priority.Medium,
                connection = connection
            };
        }
    }

    public enum SendMethod
    {
        Reliable,
        ReliableUnordered,
        ReliableSequenced,
        Unreliable,
        UnreliableSequenced
    }
    
    public enum Priority
    {
        Immediate,
        High,
        Medium,
        Low
    }
}