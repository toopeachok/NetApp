using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
  class Server
  {

    class ConnectedClient
    {

      public Socket clientSocket;

      public static List<ConnectedClient> clients = new List<ConnectedClient>();

      public ConnectedClient(Socket s)
      {
        clientSocket = s;

        clients.Add(this);

        new Thread(() =>
        {
          Communicate();
        }).Start();

      }

      void Communicate()
      {
        if (clientSocket != null)
        {
          Console.WriteLine("Communicate with client was started");

          while (true)
          {

            try
            {

              String data = ReceiveData();

              Console.WriteLine($"Client sent: {data}");

            }

            catch(Exception e)
            {

              Console.WriteLine("Could not get data from the client");

              clients.Remove(this);

              break;

            }

          }

        }
      }

      String ReceiveData()
      {
        var result = new StringBuilder();

        if (clientSocket != null)
        {

        var bytesBuffer = new byte[256];

        var dataSize = 0;

        do
        {
          dataSize = clientSocket.Receive(bytesBuffer);

          result.Append(Encoding.UTF8.GetString(bytesBuffer, 0, dataSize));
        }
        while (clientSocket.Available > 0);

        Console.WriteLine("Data from the client was got.");

        }

        return result.ToString();
      }

      public void SendData(String data)
      {
        if (clientSocket != null)
        {

          try
          {

            if (data.Trim().Equals(""))
            {
              return;
            }

            var bytesBuffer = Encoding.UTF8.GetBytes(data);

            clientSocket.Send(bytesBuffer);

            Console.WriteLine("Message to the client was sent!");
          }

          catch (Exception e)
          {
            Console.WriteLine("Could not send a message.");
          }

        }
      }

    }

    private String host;

    private Socket serverSocket;

    private const int PORT = 8034;

    public Server()
    {
      try
      {

        host = Dns.GetHostName();

        Console.WriteLine($"Host name: {host}");

        serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        foreach (var address in Dns.GetHostEntry(host).AddressList)
        {
          try
          {
            serverSocket.Bind(
              new IPEndPoint(address, PORT)
            );

            Console.WriteLine($"Connected with {address}, {PORT}");

            break;
          }

          catch (Exception e)
          {
            Console.WriteLine($"Could not connect with {address}, {PORT}");
          }
        }

        serverSocket.Listen(10);

        Console.WriteLine("Listening was started...");

        while (true)
        {
          Console.WriteLine("Waiting for a new connection...");

          var clientSocket = serverSocket.Accept();

          Console.WriteLine("Connection with client was set.");

          new ConnectedClient(clientSocket);

        }

      }

      catch(Exception e)
      {
        Console.WriteLine("Something gone wrong.");
      }
    }

  }
}
