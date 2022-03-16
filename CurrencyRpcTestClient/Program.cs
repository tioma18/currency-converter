
using Grpc.Net.Client;

namespace CurrencyRpcTestClient;

static class Program
{
    static async Task Main(string[] args)
    {
        // The port number(5001) must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("https://localhost:7085");
        var client = new Currency.CurrencyClient(channel);
        while (true)
        {
            Console.Write("Type currency in numbers: ");
            var input = Console.ReadLine();
            var reply = await client.ConvertAsync(new ConvertRequest { NumberInput = input });
            Console.WriteLine("Converted: " + reply.ConvertedOutput);
        }
    }
}