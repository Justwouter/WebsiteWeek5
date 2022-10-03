﻿using System.Net.Sockets;

namespace Pretpark
{
    class Program
    {
        static int counter = 1;
        static void Main(string[] args)
        {

            TcpListener server = new TcpListener(new System.Net.IPAddress(new byte[] { 127,0,0,1 }), 5000);
            server.Start();
            while(true){
                //Console.WriteLine("Started");
                using Socket connectie = server.AcceptSocket();
                using Stream request = new NetworkStream(connectie);
                using StreamReader requestLezer = new StreamReader(request);
                string[]? regel1 = requestLezer.ReadLine()?.Split(" ");
                if (regel1 == null) return;
                (string methode, string url, string httpversie) = (regel1[0], regel1[1], regel1[2]);
                string? regel = requestLezer.ReadLine();
                int contentLength = 0;
                while (!string.IsNullOrEmpty(regel) && !requestLezer.EndOfStream)
                {
                    string[] stukjes = regel.Split(":");
                    (string header, string waarde) = (stukjes[0], stukjes[1]);
                    if (header.ToLower() == "content-length")
                        contentLength = int.Parse(waarde);
                    regel = requestLezer.ReadLine();
                }
                if (contentLength > 0) {
                    char[] bytes = new char[(int)contentLength];
                    requestLezer.Read(bytes, 0, (int)contentLength);
                }
                //URL routing
                string data = File.ReadAllText("Home.html");
                Console.WriteLine(url);
                if(url.Equals("/Contact",StringComparison.OrdinalIgnoreCase)){
                    data = File.ReadAllText("Contact.html");
                    break;
                }
                else if(url.Equals("/Teller",StringComparison.OrdinalIgnoreCase)){
                    data = "<h1>"+counter+"</h1>";
                    counter++;
                }

                connectie.Send(System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nContent-Length: 11\r\n\r\n"+data));

            }
           
        }
    }
}
