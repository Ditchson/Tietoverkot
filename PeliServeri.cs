using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Peliserveri
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 9999);
            palvelin.Bind(iep);
            bool on = true;
            string TILA = "WAIT";
            int pelaajia = 0;
            EndPoint[] pelaajat = new EndPoint[2];
            string[] nimet = new string[2];
            int vuorot = -1, luku = -1, kuittauksia = 0;
            while(on)
            {
                byte[] rec = new byte[256];
                IPEndPoint iap = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remoteEP = (EndPoint)iap;
                int paljon = palvelin.ReceiveFrom(rec, ref remoteEP);

                string viesti = Encoding.UTF8.GetString(rec, 0, paljon);
                char[] erottimet = { ' ' };
                string[] osaset = viesti.Split(erottimet, 3);
                Console.WriteLine(viesti);

                switch (TILA)    //vastaanotto
                {
                    case "WAIT":
                        switch (osaset[0])
                        {
                            case "JOIN":
                                nimet[pelaajia] = osaset[1];
                                pelaajat[pelaajia] = remoteEP;
                                if (pelaajia == 0)
                                {
                                    palvelin.SendTo(Encoding.UTF8.GetBytes("ACK 201"), pelaajat[pelaajia]);
                                }
                                else if(pelaajia == 1)
                                {
                                    Random rand = new Random();
                                    int aloittaja = rand.Next(0,2);
                                    vuorot = aloittaja;
                                    luku = rand.Next(1,11);
                                    palvelin.SendTo(Encoding.UTF8.GetBytes("ACK 202 " + nimet[1 - aloittaja]), pelaajat[aloittaja]);
                                    palvelin.SendTo(Encoding.UTF8.GetBytes("ACK 203 " + nimet[aloittaja]), pelaajat[1 - aloittaja]);
                                    TILA = "GAME";
                                }
                                pelaajia++;
                                //else jotain pielessä
                                break;
                            default:
                                Console.WriteLine("WAIT tilassa odotetaan JOIN kahdelta pelaajalta");
                                break;
                        }//switch (osaset[0])
                        break;
                    case "GAME":
                        switch (osaset[0])
                        {
                            case "DATA":
                                if (remoteEP.Equals(pelaajat[vuorot]))
                                {
                                    try
                                    {
                                        if (int.Parse(osaset[1]) == luku)
                                        {
                                            palvelin.SendTo(Encoding.UTF8.GetBytes("QUIT 501 " + "Voitit arvaamalla: " + osaset[1]), pelaajat[vuorot]);
                                            palvelin.SendTo(Encoding.UTF8.GetBytes("QUIT 502 " + "Hävisit! Oikea luku oli: " + osaset[1]), pelaajat[1 - vuorot]);
                                            TILA = "END";
                                        }
                                        else //arvasit väärin
                                        {
                                            palvelin.SendTo(Encoding.UTF8.GetBytes("ACK 300"), pelaajat[vuorot]);
                                            palvelin.SendTo(Encoding.UTF8.GetBytes("DATA " + osaset[1]), pelaajat[1 - vuorot]);
                                            vuorot = 1 - vuorot;
                                            TILA = "WAIT_ACK";
                                        }
                                    }
                                    catch
                                    {
                                        palvelin.SendTo(Encoding.UTF8.GetBytes("ACK 407 arvauksesi ei ole numero"), remoteEP);
                                    }
                                }
                                else
                                {
                                    palvelin.SendTo(Encoding.UTF8.GetBytes("ACK 403 Ei ole vuorosi vielä"), remoteEP);
                                }
                                break;
                            case "JOIN":
                                palvelin.SendTo(Encoding.UTF8.GetBytes("ACK 401 Peli jo käynnissä"), remoteEP);
                                break;
                            default:
                                Console.WriteLine("Ei ollut DATA viesti");
                                break;
                        }
                        break;
                    case "WAIT_ACK":
                        if (remoteEP.Equals(pelaajat[vuorot]) && osaset[0] == "ACK" && osaset[1] == "300")
                        {
                            TILA = "GAME";
                        }
                        else
                        {
                            palvelin.SendTo(Encoding.UTF8.GetBytes("ACK 409 Odotetaan ACK 300 viestiä"), remoteEP);
                        }
                        break;
                    case "END":
                        if (osaset[0] == "ACK" && osaset[1] == "500")
                        {
                            kuittauksia++;
                            if (kuittauksia == pelaajia)
                            {
                                on = false;
                            }
                        }
                        break;
                }//switch(TILA)
            }//while(on)
            Console.ReadKey();
            palvelin.Close();
        }
    }
}
