﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Common.Networking.Packet;
using Common.Networking.IO;
using ENet;
using GameServer.Server.Packets;
using GameServer.Database;
using GameServer.Logging;

namespace GameServer.Server
{
    public class ENetServer
    {
        public static void WorkerThread() 
        {
            Thread.CurrentThread.Name = "SERVER";

            using (var db = new DatabaseContext())
            {
                // Create
                db.Add(new Player { Gold = 100 });
                db.SaveChanges();

                // Read
                Logger.Log("Query for player");
                var player = db.Players.First();

                // Update
                Logger.Log("Updating the player");
                player.Gold = 200;
                db.SaveChanges();

                // Delete
                Logger.Log("Delete the player");
                db.Remove(player);
                db.SaveChanges();
            }

            Library.Initialize();

            var maxClients = 100;
            ushort port = 8888;

            using (var server = new Host())
            {
                var address = new Address
                {
                    Port = port
                };

                server.Create(address, maxClients);

                while (!Console.KeyAvailable)
                {
                    var polled = false;

                    while (!polled)
                    {
                        if (server.CheckEvents(out Event netEvent) <= 0)
                        {
                            if (server.Service(15, out netEvent) <= 0)
                                break;

                            polled = true;
                        }

                        switch (netEvent.Type)
                        {
                            case EventType.None:
                                break;

                            case EventType.Connect:
                                Logger.Log("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                                break;

                            case EventType.Disconnect:
                                Logger.Log("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                                break;

                            case EventType.Timeout:
                                Logger.Log("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                                break;

                            case EventType.Receive:
                                Logger.Log("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);

                                var packet = netEvent.Packet;

                                var readBuffer = new byte[1024];
                                var readStream = new MemoryStream(readBuffer);
                                var reader = new BinaryReader(readStream);

                                readStream.Position = 0;
                                netEvent.Packet.CopyTo(readBuffer);
                                //var packetID = (ClientPacketType)reader.ReadByte();

                                //Console.WriteLine(packetID);

                                var data = new PacketPurchaseItem();
                                var packetReader = new PacketReader(readBuffer);
                                data.Read(packetReader);

                                Logger.Log(data.m_ID);
                                Logger.Log(data.m_ItemID);

                                packet.Dispose();
                                break;
                        }
                    }
                }

                server.Flush();
            }

            Library.Deinitialize();
        }
    }
}
