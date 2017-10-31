using RakNet.Network;
using SapphireEngine;
using UServer2.Struct;

namespace UServer3.Network
{
    public class NetworkManager : SapphireType
    {
        public static NetworkManager Instance;

        public UserInformation ConnectionInfromation;
        
        public override void OnAwake() => Instance = this;

        public bool IN_NetworkMessage(Message message)
        {

            return false;
        }

        public bool Out_NetworkMessage(Message message)
        {

            return false;
        }

        public void OnDisconnected()
        {
            
        }
    }
}