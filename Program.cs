using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace tcpasiakas
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket asiakas = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string server = "www.example.com";
            string resurssi = "/";
            string message = "GET " + resurssi + " HTTP/1.1\r\nHost:" + server + "\r\nConnection:Close\r\n\r\n";
            asiakas.Connect(server, 80);
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int paljon = 0;

            asiakas.Send(msg);

            do
            {
                byte[] rec = new byte[1024];
                paljon = asiakas.Receive(rec);
                Console.Write(Encoding.ASCII.GetString(rec));
            } while (paljon > 0);
            
            Console.ReadKey();
            asiakas.Close();
        }
    }
}
