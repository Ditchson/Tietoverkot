using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace chatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 8888);
            palvelin.Bind(iep);
            List<EndPoint> asiakkaat = new List<EndPoint>();

            bool on = true;

            while(on)
            {
                byte[] rec = new byte[256];
                IPEndPoint asiakasIEP = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remoteEP = (EndPoint) asiakasIEP;
                int paljonko = palvelin.ReceiveFrom(rec, ref remoteEP);
                if (!asiakkaat.Contains(remoteEP))  //jos ei ole vielä listalla
                {
                    Console.WriteLine("Uusi Asiakas: [{0}:{1}]", ((IPEndPoint)remoteEP).Address, ((IPEndPoint)remoteEP).Port);
                    asiakkaat.Add(remoteEP);
                }
                string viesti = Encoding.UTF8.GetString(rec, 0, paljonko);
                char[] merkki = { ';' };
                string[] osat = viesti.Split(merkki, 2);
                if (osat.Length < 2)
                {
                    Console.WriteLine("väärä viesti");
                }
                else
                {
                    Console.WriteLine("{0}: {1}", osat[0], osat[1]);
                    foreach(EndPoint asiakas in asiakkaat)
                    {
                        palvelin.SendTo(Encoding.UTF8.GetBytes(viesti), asiakas);
                    }
                }
            }

            palvelin.Close();
        }
    }
}
