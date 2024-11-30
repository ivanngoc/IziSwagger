using System.Net;
using System.Net.Sockets;

namespace ToolForReverse;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        TcpListener tcpListener = new TcpListener(IPAddress.Any, 5000);
        tcpListener.Start();
        await Task.CompletedTask;
        using FileStream fs = File.OpenRead(@"/home/user/Documents/Matriks/mockforequiron/docs/some.txt");
        using MemoryStream ms = new MemoryStream();
        await fs.CopyToAsync(ms);
        var bytes = ms.ToArray();

        while (true)
        {
            var client = await tcpListener.AcceptTcpClientAsync();
            Console.WriteLine("Client accepted");
            var ns = client.GetStream();
            // StreamReader sr = new StreamReader(s);
            // Console.WriteLine("Begin read...");
            // await sr.ReadToEndAsync();
            await ns.WriteAsync(bytes);
            Console.WriteLine("End read...");
            // await fs.CopyToAsync(s);
            Console.WriteLine("Completed");
        }
    }
}