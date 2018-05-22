using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace smtpClientti
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket asiakas = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            asiakas.Connect("localhost", 25000);
            bool on = true;

            while (on)
            {
                byte[] rec = new byte[256];
                int paljonko = asiakas.Receive(rec);
                string vastaus = Encoding.UTF8.GetString(rec, 0, paljonko);
                Console.WriteLine(vastaus);
                string[] palat = vastaus.Split(' ');

                switch (palat[0])    //statuskoodi
                {
                    case "220":
                        asiakas.Send(Encoding.UTF8.GetBytes("HELO jyu.fi\r\n")); // lähetä HELO
                        break;
                    case "221":
                        on = false;
                        break;
                    case "250": // ITKP204 Postipalvelin (jotain tekstiä) tai 2.1.0 tai 2.1.5 tai 2.0.0
                        switch (palat[1])
                        {
                            case "2.1.0":
                                asiakas.Send(Encoding.UTF8.GetBytes("RCPT TO:puri@puru.fi\r\n")); //lähetä RCPT TO
                                break;
                            case "2.1.5":
                                asiakas.Send(Encoding.UTF8.GetBytes("DATA\r\n"));//lähetä DATA
                                break;
                            case "2.0.0":
                                asiakas.Send(Encoding.UTF8.GetBytes("QUIT\r\n"));//lähetä QUIT
                                break;
                            default:   // ITKP204 Postipalvelin HELO ... (jotain tekstiä)
                                asiakas.Send(Encoding.UTF8.GetBytes("MAIL FROM: puru@puri.fi\r\n"));    //lähetä MAIL FROM
                                break;

                        } //switch palat[1] -250
                        break;
                    case "354": //lähetä email
                        string posti = Console.ReadLine();
                        asiakas.Send(Encoding.UTF8.GetBytes(posti + "\r\n\r\n"));
                        asiakas.Send(Encoding.UTF8.GetBytes("Send by posti"));
                        asiakas.Send(Encoding.UTF8.GetBytes("\r\n.\r\n"));
                        break;
                    default:
                        Console.WriteLine("virhe");
                        asiakas.Send(Encoding.UTF8.GetBytes("QUIT\r\n"));   //lähetä QUIT
                        break;
                }   //statuskoodi loppu
            }   //while(on) loppu
            //END TILA

            Console.ReadKey();
            asiakas.Close();
        }
    }
}
