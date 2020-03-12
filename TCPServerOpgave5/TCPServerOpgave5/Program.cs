using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TCPServerOpgave5
{
    public class Program
    {
        private static List<Bog> Boeger = new List<Bog>()
       {
           new Bog("Harry Potter and the Prisoner of Azkaban", "J.K.Rowling", 480 , "0000000000001"),
           new Bog("Lord of the Rings", "J.R.R.Tolkien",576 , "0000000000002"),
           new Bog("Dracula", "Bram Stoker", 418, "0000000000003"),
           new Bog("Les Miserables", "Victor Hugo",1232 , "0000000000004"),
           new Bog("The Pillars of the Earth", "Ken Follett",806, "0000000000005"),
           new Bog("Sejrens triumf","Lars Jørgensen", 451, "0000000000006"),
           new Bog("Danmarks Oldtid", "Jørgen Jensen", 619, "0000000000007"),
           new Bog("Mammal Bones And Teeth", "Simon Hillson", 132, "0000000000008")
       };

        static void Main(string[] args)
        {
            int port = 4646;
            int clientNr = 0;

            Console.WriteLine("Hello EchoServer"); IPAddress ip = IPAddress.Loopback;
            TcpListener ServerListener = StartServer(ip, port);

            do
            {
                TcpClient ClientConnection = GetConnectionSocket(ServerListener, ref clientNr);
                Task.Run(() => ReadWriteStream(ClientConnection, ref clientNr));


            } while (clientNr != 0);

            StopServer(ServerListener);
        }
        private static void StopServer(TcpListener serverListener)
        {
            serverListener.Stop();
            Console.WriteLine("listener stopped");
        }

        private static TcpClient GetConnectionSocket(TcpListener serverListener, ref int clientNr)
        {

            TcpClient connectionSocket = serverListener.AcceptTcpClient();
            clientNr++;
            //Socket connectionSocket = serverSocket.AcceptSocket();
            Console.WriteLine("Client " + clientNr + " connected");
            return connectionSocket;
        }

        private static void ReadWriteStream(TcpClient connectionSocket, ref int clientNr)
        {
            Stream ns = connectionSocket.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true; // enable automatic flushing

            string message = sr.ReadLine();
            Thread.Sleep(1000);
            string answer = "";
            while (message != null && message != "")
            {
                Console.WriteLine("Client: " + clientNr + " " + message);
                string[] messageArray = message.Split(' ');


                for (int i = 0; i < messageArray.Length; i++)
                {

                    if (messageArray[0] == "GetAll")
                    {
                        foreach (var v in Boeger)
                        {
                            answer = JsonConvert.SerializeObject(v);
                            sw.WriteLine(answer);
                        }

                        message = "";
                    }

                    if (messageArray[0] == "Get" && messageArray[1] != null)
                    {
                        answer = JsonConvert.SerializeObject(Boeger.Find(b => b.Isbn13 == messageArray[1]));
                        sw.WriteLine(answer);

                        message = "";
                    }

                    if (messageArray[0] == "Save" && messageArray[1] != null || messageArray[1] != "")
                    {
                        int bookCount = Boeger.Count;
                        Boeger.Add(JsonConvert.DeserializeObject<Bog>(messageArray[1]));
                        if (bookCount < Boeger.Count)
                        {
                            answer = "Saved Correctly";
                            sw.WriteLine(answer);
                        }
                    }

                    Thread.Sleep(1000);
                }
            }

            Console.WriteLine("Empty message detected");
            ns.Close();
            connectionSocket.Close();
            clientNr--;
            Console.WriteLine("connection socket " + clientNr + " closed");

        }


        private static TcpListener StartServer(IPAddress ip, int port)
        {
            TcpListener serverSocket = new TcpListener(ip, port);
            serverSocket.Start();

            Console.WriteLine("server started waiting for connection!");
            Console.WriteLine("Ip: " + ip);
            Console.WriteLine("Port: " + port);

            return serverSocket;
        }
    }
}
