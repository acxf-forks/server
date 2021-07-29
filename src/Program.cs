﻿using System;
using System.Threading;
using System.Linq;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            StartLogger();
            StartServer();
        }

        private static void StartLogger() 
        {
            new Thread(Logger.InputThread).Start();
            new Thread(Logger.MessagesThread).Start();
        }

        public static void StartServer() 
        {
            new Thread(Server.WorkerThread).Start();
        }
    }
}
