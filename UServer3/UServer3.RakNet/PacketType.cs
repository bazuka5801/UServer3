﻿namespace RakNet
{
    public static class PacketType
    {
        public const byte NEW_INCOMING_CONNECTION = 19;

        public const byte CONNECTION_REQUEST_ACCEPTED = 16;

        public const byte CONNECTION_ATTEMPT_FAILED = 17;

        public const byte DISCONNECTION_NOTIFICATION = 21;

        public const byte NO_FREE_INCOMING_CONNECTIONS = 20;

        public const byte CONNECTION_LOST = 22;

        public const byte CONNECTION_BANNED = 23;
    }
}