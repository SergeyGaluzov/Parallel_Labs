﻿using System;
using System.Collections.Generic;
using Akka.Actor;
using lab2.Actors;

namespace Parallel_Labs
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientsList = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                clientsList.Add("client " + i);
            }

            using var system = ActorSystem.Create("PaydeskSystem");
            var paydeskSystem = system.ActorOf<PaydesksActor>("start");

            foreach(var client in clientsList)
            {
                system.ActorOf(Props.Create(() => new ClientActor(client, paydeskSystem)));
            }

            Console.ReadLine();
        }
    }
}