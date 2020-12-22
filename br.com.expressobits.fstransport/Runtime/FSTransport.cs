using UnityEngine;
using MLAPI;
using MLAPI.Transports;
using System;
using MLAPI.Transports.Tasks;
using Steamworks;
using Steamworks.Data;
using System.Collections.Generic;

namespace ExpressoBits.Transport
{
    public class FSTransport : Transport
    {

        [Header("Relay Socket")]
        public bool useRelay = false;
        public ulong targetSteamId;

        [Header("Normal Socket")]
        public ushort Port = 7777;
        public string Address = "127.0.0.1";

        [Header("Auth Server")]
        public bool checkIdentity = false;
        public ushort SteamPort = 7778;
        public ushort QueryPort = 7779;

        [Header("Steam App Settings")]
        public int appId = 480;

        public string serverName = "Test server";
        internal bool isServer;
        private bool serverActive;
        public EBSocketManager Server { get; private set; }
        public EBConnectionManager Client { get; private set; }

        public override ulong ServerClientId => throw new NotImplementedException();

        public override bool IsSupported => UnityEngine.Application.platform != UnityEngine.RuntimePlatform.WebGLPlayer;

        public EBConnectionManager connectionManager;
        public EBSocketManager socketManager;

        public class NetworkPlayer
        {
            public Connection connection;
            public ConnectionInfo data;

            public NetworkPlayer(Connection connection, ConnectionInfo data, int index)
            {
                connection.UserData = index;
                this.connection = connection;
                this.data = data;
            }
        }

        public override void DisconnectLocalClient()
        {
            if (Client != null)
            {
                Client.Connection.Flush();
                Client.Connection.Close();
            }
        }

        public override void DisconnectRemoteClient(ulong clientId)
        {
            foreach ( var connection in Server.Connected)
            {
                if(connection.Id == clientId)
                {
                    connection.Close();
                }
                
            }
        }

        public override ulong GetCurrentRtt(ulong clientId)
        {
            // Not support
        }

        public override void Init()
        {
            
        }

        public override NetEventType PollEvent(out ulong clientId, out string channelName, out ArraySegment<byte> payload, out float receiveTime)
        {
            throw new NotImplementedException();
        }

        public override void Send(ulong clientId, ArraySegment<byte> data, string channelName)
        {
            connectionManager.Connection.SendMessage(data.Array);
        }

        public override void Shutdown()
        {
            throw new NotImplementedException();
        }

        public override SocketTasks StartClient()
        {
            if(useRelay)
            {
                Client = SteamNetworkingSockets.ConnectRelay<EBConnectionManager>(targetSteamId);
            }
            else
            {
                Client = SteamNetworkingSockets.ConnectNormal<EBConnectionManager>(NetAddress.From(Address,Port));
            }
            
            return SocketTask.Done.AsTasks();
        }

        public override SocketTasks StartServer()
        {
            Server = SteamNetworkingSockets.CreateNormalSocket<EBSocketManager>(
                NetAddress.AnyIp(Port));
            return SocketTask.Done.AsTasks();
        }

    }
}

