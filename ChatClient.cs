using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket asiakas = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 8888);
            asiakas.ReceiveTimeout = 2000;
            Console.Write("Anna nimi>");
            string nimi = Console.ReadLine();
            string viesti = "";

            do
            {

                while (!Console.KeyAvailable)
                {
                    try
                    {
                        byte[] rec = new byte[256];
                        IPEndPoint palvelinIEP = new IPEndPoint(IPAddress.Any, 0);
                        EndPoint remoteEP = (EndPoint)palvelinIEP;
                        int paljonko = asiakas.ReceiveFrom(rec, ref remoteEP);
                        string palvelimelta = Encoding.UTF8.GetString(rec, 0, paljonko);
                        char[] merkki = { ';' };
                        string[] osat = palvelimelta.Split(merkki, 2);
                        Console.WriteLine("{0}: {1}", osat[0], osat[1]);
                    }
                    catch
                    {
                        //receive timeout
                    }
                }
                Console.Write(">");
                viesti = Console.ReadLine();
                asiakas.SendTo(Encoding.UTF8.GetBytes(nimi + ";" + viesti), iep);
            } while (viesti != "q");
            asiakas.Close();
        }
    }
}
