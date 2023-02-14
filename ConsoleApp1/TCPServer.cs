using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 5000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener server = new TcpListener(localAddr, port);
            server.Start();
            Console.WriteLine("Server started on " + localAddr + ":" + port);

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected");
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = 0;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                if (message.Contains("\u0002") && message.Contains("\u0003"))
                {
                    int startIndex = message.IndexOf("\u0002") + 1;
                    int endIndex = message.IndexOf("\u0003");
                    string astmMessage = message.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine("Received ASTm message: " + astmMessage);

                    // Translate the message here
                    string translatedMessage = TranslateAstmMessage(astmMessage);

                    // Send the ACK response
                    byte[] ackResponse = System.Text.Encoding.ASCII.GetBytes("ACK");
                    stream.Write(ackResponse, 0, ackResponse.Length);
                }
                else
                {
                    byte[] nackResponse = System.Text.Encoding.ASCII.GetBytes("NACK");
                    stream.Write(nackResponse, 0, nackResponse.Length);
                    Console.WriteLine("Received invalid message: " + message);
                }
            }

            stream.Close();
            client.Close();
        }

        static string TranslateAstmMessage(string astmMessage)
        {
            // Split the ASTm message into segments
            string[] segments = astmMessage.Split('|');

            // Extract some fields from the message
            string[] sampleIDMessage = segments[3].Split('^');

            string sampId = sampleIDMessage[2].Replace("?","");


            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                //Provider=SQLNCLI11;Server=LAPTOP-780M63QV\VGSM;Database=sm21_new;User Id=SM_21_2;Password=Orbis123;MARS Connection=true;
                builder.DataSource = "LAPTOP-780M63QV\\VGSM";
                builder.UserID = "SM_21_2";
                builder.Password = "Orbis123";
                builder.InitialCatalog = "sm21_new";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    String sql = "SELECT test_number, analysis FROM TEST where sample = " + sampId.Replace(" ","");

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }


            string translatedMessage = "Test result for ";

            return translatedMessage;
        }
    }
}