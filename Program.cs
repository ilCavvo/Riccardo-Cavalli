using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace lez1
{
    // Client app is the one sending messages to a Server/listener.
    // Both listener and client can send messages back and forth once a
    // communication is established.
    public class SocketClient
    {
        public static int Main(String[] args)
        {
            StartClient();
            return 0;
        }

        public static void StartClient()
        {
            byte[] bytes = new byte[1024];

            try
            {
                // Connect to a Remote server
                // Get Host IP Address that is used to establish a connection
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1
                // If a host has multiple addresses, you will get a list of addresses

                //IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = IPAddress.Parse("10.100.0.127");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.



                try
                {
                    // Connect to Remote EndPoint
                    sender.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());//socket collegato

                    var rt = new Thread(obj =>
                    {
                        ricezione((Socket)obj);

                    });
                        rt.Start(sender);

                    while (true)
                    {
                        string messaggio = Domanda();

                        // Encode the data string into a byte array
                        byte[] msg = Encoding.ASCII.GetBytes(messaggio);

                        // Send the data through the socket.
                        int bytesSent = sender.Send(msg);

                        //Stop communication when send string <STOP>
                        if (messaggio == "<STOP>")
                        {
                          rt.Abort();
                            break;
                        }
                       


                        // Receive the response from the remote device.
                        //int bytesRec = sender.Receive(bytes);
                        //Console.WriteLine("Echoed test = {0}",
                        //    Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    }
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ricezione(Socket sender)
        {
            // Incoming data from the server.
            string data = null;
            byte[] bytes = null;
            bytes = new byte[1024];
            string[] messaggio;

            while (true)
            {

                int bytesRec = sender.Receive(bytes);
                data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                messaggio = data.Split("#*!*#");

                if (messaggio[1] == "<STOP>")
                {
                    Console.WriteLine($"{messaggio[0]}: si è disconnesso.");
                    break;
                }
                else
                {
                    Console.WriteLine($"{messaggio[0]}: {messaggio[1]}");
                }

            }

        }

        public static string Domanda()
        {
            Console.WriteLine("Scrivi messaggio da inviare(<STOP> per fermare la comunicazione): ");
            return Console.ReadLine();
        }
    }
}