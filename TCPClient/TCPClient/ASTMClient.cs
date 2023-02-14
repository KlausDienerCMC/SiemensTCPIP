using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 5000);
            NetworkStream stream = client.GetStream();

            string astmMessage = "\u0002" + "<STX>3O|1||000002^06^              1^B^^N||R||||||N<CR><ETX>D4<CR><LF>\r" + "\u0003" + "FD\r\n";

            bool endOfMessages = false;
            while (!endOfMessages)
            {
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(astmMessage);
                stream.Write(buffer, 0, buffer.Length);

                byte[] response = new byte[1024];
                int bytesRead = stream.Read(response, 0, response.Length);
                string responseMessage = System.Text.Encoding.ASCII.GetString(response, 0, bytesRead);

                if (responseMessage == "ACK")
                {
                    Console.WriteLine("Received ACK from server");
                    if (astmMessage == "L|1|N\r")
                    {
                        astmMessage = "L|1|E\r";
                        endOfMessages = true;
                    }
                }
                else
                {
                    Console.WriteLine("Server response: " + responseMessage);
                    break;
                }
            }

            stream.Close();
            client.Close();
        }
    }
}
