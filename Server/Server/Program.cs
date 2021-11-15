﻿using System;
using System.Net;
using System.Threading;
using Server.Game;
using ServerCore;

namespace Server
{
    class Program
    {
        static readonly List listener = new();

        static void FlushRoom()
        {
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            RoomManager.Instance.Add(1);

           // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new(ipAddr, 7777);

            listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            //FlushRoom();
            //JobTimer.Instance.Push(FlushRoom);

            // TODO
            while (true)
            {
                //JobTimer.Instance.Flush();
                RoomManager.Instance.Find(1).Update();

                Thread.Sleep(100);
            }
        }
    }
}
