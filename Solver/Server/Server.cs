using IcfpUtils;
using Owin;
using Newtonsoft.Json;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Solver;

namespace Server
{
    class Server
    {
        public static Dictionary<string, LispNode> Symbols = new Dictionary<string, LispNode>();
        
        static void Main(string[] args)
        {
            var lines = File.ReadLines(@"galaxy.txt");
            foreach (var line in lines)
            {
                var x = line.Split('=');
                var symbol = x[0].Trim();
                var value = Program.Parse(x[1]);

                Symbols[symbol] = value;
            }

            string baseAddress = "http://localhost:8080/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Press any key to continue or any other key to detonate");
                Console.ReadLine();
            }
        }

        public static LispNode JsonToLispNode(JToken o)
        {
            if (o.Type == JTokenType.String)
            {
                return new LispNode(o.ToObject<string>());
            }
            else
            {
                return new LispNode(JsonToLispNode(o.First), JsonToLispNode(o.Last));
            }
        }

    }

    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}"
                // defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);

            const string rootFolder = @".";

            appBuilder.UseFileServer(
                new FileServerOptions
                {
                    EnableDefaultFiles = true,
                    FileSystem = new PhysicalFileSystem(rootFolder)
                });
        }
    }

    public class GalaxyPostBody
    {
        public JToken state { get; set; }
        public string x { get; set; }
        public string y { get; set; }
    }

    public class GalaxyController : ApiController
    {
        // POST api/galaxy 
        public JToken Post([FromBody] GalaxyPostBody body)
        {
            return
                JsonConvert.DeserializeObject<JToken>(
                    Program.TreeToJson(
                        Program.Interact(
                            Server.Symbols["galaxy"],
                            Server.JsonToLispNode(body.state),
                            body.x,
                            body.y,
                            Server.Symbols)));
        }
    }
}
