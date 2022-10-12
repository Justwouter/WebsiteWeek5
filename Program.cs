using System.Net.Sockets;

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
                Console.WriteLine("Started");
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
                if(!url.Contains("/css")){
                    string data;
                    Console.WriteLine(url);
                    if(url.Equals("/")){
                        data = File.ReadAllText("Pages\\Home.html");
                    }
                    else if(url.Equals("/contact", StringComparison.OrdinalIgnoreCase)){
                        data = File.ReadAllText("Pages\\Contact.html");
                    }
                    else if(url.Equals("/teller", StringComparison.OrdinalIgnoreCase)){
                        data = "<h1>"+counter+"</h1>";
                        counter++;
                    }
                    else if(url.Contains("/add", StringComparison.OrdinalIgnoreCase)){
                        data = File.ReadAllText("Pages\\Add.html");
                    }
                    else if(url.Contains("/mijnteller", StringComparison.OrdinalIgnoreCase)){
                        data = File.ReadAllText("Pages\\AddWithButton.html");
                    }
                    else{
                        data = File.ReadAllText("Pages\\404.html");
                    }
                    connectie.Send(System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nContent-Length: 11\r\n\r\n"+data));
                }
                else{
                    Console.WriteLine(url);
                    var data = url.Replace("/", "\\\\");
                    data = File.ReadAllText("Pages"+data);
                    connectie.Send(System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: text/css\r\nContent-Length: "+data.Length+"\r\n\r\n"+data));
                }

            }
           
        }
    }
}
