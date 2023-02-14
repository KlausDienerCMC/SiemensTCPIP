//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Net.Sockets;
//using System.Net;
//using System.IO;
//using System.Threading.Tasks;

//namespace ConsoleApplication1
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {

//            try
//            {
//                IPAddress ipAd = IPAddress.Parse("192.168.56.1"); //use local       m/c IP address, and use the same in the client

//                /* Initializes the Listener */
//                TcpListener myList = new TcpListener(ipAd, 8001);

//                /* Start Listeneting at the specified port */
//                myList.Start();

//                Console.WriteLine("The server is running at port 8001...");
//                Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);
//                Console.WriteLine("Waiting for a connection.....");

//                Socket s = myList.AcceptSocket();
//                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

//                byte[] b = new byte[100];
//                int k = s.Receive(b);
//                Console.WriteLine("Recieved...");
//                for (int i = 0; i < k; i++)
//                    Console.Write(Convert.ToChar(b[i]));
//                Console.WriteLine("Enter the string to be strasmitted");

//                String str = Console.ReadLine();

//                ASCIIEncoding asen = new ASCIIEncoding();
//                s.Send(asen.GetBytes(str));
//                Console.WriteLine("\nSent Acknowledgement");
//                /* clean up */
//                s.Close();
//                myList.Stop();

//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("Error..... " + e.StackTrace);
//            }
//            Console.ReadLine();
//        }

//    }
//}