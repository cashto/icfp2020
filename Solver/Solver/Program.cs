using System;
using System.Linq;
using System.Text;
using IcfpUtils;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Solver
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var serverUrl = args[0];
            var playerKey = args[1];

            var gameResponse = Send(serverUrl, MakeJoinRequest(playerKey));
            gameResponse = Send(serverUrl, MakeStartRequest(playerKey, gameResponse));
            while (true)
            {
                gameResponse = Send(serverUrl, MakeCommandsRequest(playerKey, gameResponse));
            }
        }

        static LispNode Send(string serverUrl, LispNode request)
        {
            if (!Uri.TryCreate(serverUrl + "/aliens/send", UriKind.Absolute, out var serverUri))
            {
                throw new Exception($"Failed to parse ServerUrl {serverUrl}");
            }
            
            using var httpClient = new HttpClient { BaseAddress = serverUri };
            var requestContent = new StringContent(Common.Modulate(Common.Unflatten(request)), Encoding.UTF8, MediaTypeNames.Text.Plain);
            using var response = httpClient.PostAsync("", requestContent).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Unexpected server response: {response}");
            }

            var responseString = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"Server response: {responseString}");

            return Common.Flatten(Common.Demodulate(responseString).Item1);
        }

        static LispNode MakeJoinRequest(string playerKey)
        {
            return 
                new LispNode() { 
                    new LispNode("2"), 
                    new LispNode(playerKey), 
                    new LispNode("nil") 
                };
        }

        static LispNode MakeStartRequest(string playerKey, LispNode gameResponse)
        {
            return 
                new LispNode() { 
                    new LispNode("3"), 
                    new LispNode(playerKey), 
                    new LispNode() {
                        new LispNode("0"),
                        new LispNode("0"),
                        new LispNode("0"),
                        new LispNode("1")
                    }
                };
        }

        static LispNode MakeCommandsRequest(string playerKey, LispNode gameResponse)
        {
            return
                new LispNode() {
                    new LispNode("4"),
                    new LispNode(playerKey),
                    new LispNode() {
                        new LispNode("0"),
                        new LispNode("0"),
                        new LispNode() {
                            new LispNode("0"),
                            new LispNode("0")
                        }
                    }
                };
        }
    }
}
