using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace PeliAsiakas
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket asiakas = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 9999 );
            Console.WriteLine("Anna nimi> ");
            string nimi = Console.ReadLine();
            string TILA = "CLOSED";
            string erotin = " ";
            asiakas.SendTo(Encoding.UTF8.GetBytes("JOIN" + erotin + nimi), iep);
            TILA = "JOIN";
            bool on = true;

            while(on)
            {//while on
                byte[] rec = new byte[256];
                IPEndPoint iap = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remoteEP = (EndPoint)iap;
                int paljon = asiakas.ReceiveFrom(rec, ref remoteEP);

                string viesti = Encoding.UTF8.GetString(rec, 0, paljon);
                char[] erottimet = { ' ' };
                string[] osaset = viesti.Split(erottimet, 3);
                Console.WriteLine(viesti);
                switch (TILA)
                {   //switch TILA
                    case "JOIN":
                        switch (osaset[0])
                        {
                            case "ACK":
                                switch(osaset[1])
                                {
                                    case "201":
                                        Console.WriteLine("odotellaan toista pelaajaa...");
                                        break;
                                    case "202":
                                        if (osaset.Length == 3)
                                        {
                                            Console.WriteLine("Vastustajasi on: " + osaset[2]);
                                        }
                                        Console.Write("Arvaa numero: ");
                                        string arvaus = Console.ReadLine();
                                        asiakas.SendTo(Encoding.UTF8.GetBytes("DATA" + erotin + arvaus), iep);
                                        TILA = "GAME";
                                        break;
                                    case "203":
                                        if (osaset.Length == 3)
                                        {
                                            Console.WriteLine("Vastustajasi on: " + osaset[2]);
                                        }
                                        Console.WriteLine("vastustajasi aloittaa, odota...");
                                        TILA = "GAME";
                                        break;
                                    case "401":
                                        Console.WriteLine("pelissä oli jo 2 pelaajaa!");
                                        on = false;
                                        break;
                                    default:
                                        Console.WriteLine("Ei ollut ACK 2xx!");
                                        on = false;
                                        break;
                                }   //switch osaset 1
                                break;
                            default:
                                Console.WriteLine("Ei ollut ACK !");
                                on = false;
                                break;
                        }   //switch osaset 0
                        break;
                    case "GAME":
                        switch (osaset[0])
                        {
                            case "ACK":
                                if (osaset[1] == "407") //ei ollut numero!
                                {
                                    Console.Write("arvaa numero: ");
                                    string uusi_arvaus = Console.ReadLine();
                                    asiakas.SendTo(Encoding.UTF8.GetBytes("DATA" + erotin + uusi_arvaus), iep);
                                }
                                else if (osaset[1] != "300")
                                {
                                    Console.WriteLine("Ei ollut ACK 300 !");
                                    on = false;
                                }
                                break;
                            case "DATA":
                                asiakas.SendTo(Encoding.UTF8.GetBytes("ACK" + erotin + "300"), iep);
                                Console.Write("arvaa numero: ");
                                string arvaus = Console.ReadLine();
                                asiakas.SendTo(Encoding.UTF8.GetBytes("DATA" + erotin + arvaus), iep);
                                break;
                            case "QUIT":
                                asiakas.SendTo(Encoding.UTF8.GetBytes("ACK" + erotin + "500"), iep);
                                switch(osaset[1])
                                {
                                    case "501":
                                        Console.WriteLine("Voitin! :D");
                                        break;
                                    case "502":
                                        Console.WriteLine("Hävisit... :*(");
                                        break;
                                }//osaset 1
                                if (osaset.Length == 3)
                                {
                                    Console.WriteLine(osaset[2]);
                                }
                                on = false;
                                break;
                            default:
                                Console.WriteLine("Ei ollut ACK , DATA tai edes QUIT!");
                                on = false;
                                break;
                        }//switch osaset 0
                        break;

                }   //switch TILA
            }//while on

            Console.ReadKey();
            asiakas.Close();
        }
    }
}
