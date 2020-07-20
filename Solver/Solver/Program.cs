using System;
using System.Linq;
using System.Text;
using IcfpUtils;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualBasic.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;

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
            var result = Common.Flatten(Common.Demodulate(responseString).Item1);
            Console.WriteLine($"Sent [{request}] received [{result}]");

            return result;
        }

        static LispNode MakeJoinRequest(string playerKey)
        {
            return 
                new LispNode() { 
                    new LispNode("2"), 
                    new LispNode(playerKey), 
                    new LispNode() 
                };
        }

        static LispNode MakeStartRequest(string playerKey, LispNode gameResponse)
        {
            return 
                new LispNode() { 
                    new LispNode("3"), 
                    new LispNode(playerKey), 
                    new LispNode() {
                        new LispNode("10"),
                        new LispNode("10"),
                        new LispNode("10"),
                        new LispNode("1")
                    }
                };
        }

        static Point Scale(Point p, int scale)
        {
            return new Point((scale * p.X * p.X) / p.SquareMagnitude(), (scale * p.Y * p.Y) / p.SquareMagnitude());
        }

        public static LispNode MakeCommandsRequest(string playerKey, LispNode gameResponse)
        {
            var staticGameState = new StaticGameState(gameResponse[2]);
            var gameState = new GameState(gameResponse[3]);
            var myShips = gameState.Ships.Where(i => i.Role == staticGameState.Role);
            var theirShips = gameState.Ships.Where(i => i.Role != staticGameState.Role);
            var commands = new List<Command>();
            foreach (var ship in myShips)
            {
                var desiredVelocity1 = Scale(new Point(-ship.Position.Y, ship.Position.X), 4);
                var desiredVelocity2 = Scale(new Point(ship.Position.Y, -ship.Position.X), 4);
                var desiredVelocity = (ship.Velocity - desiredVelocity1).SquareMagnitude() > (ship.Velocity - desiredVelocity2).SquareMagnitude() ? 
                    desiredVelocity2 : desiredVelocity1;
                var accelVector = ship.Velocity - desiredVelocity;
                accelVector.X = Math.Max(accelVector.X, -1);
                accelVector.X = Math.Min(accelVector.X, 1);
                accelVector.Y = Math.Max(accelVector.Y, -1);
                accelVector.Y = Math.Min(accelVector.Y, 1);

                commands.Add(new Command(CommandType.Accelerate, ship.Id, new LispNode() { new LispNode(accelVector.X), new LispNode(accelVector.Y) }));

                if (staticGameState.Role == Role.Attacker && 
                    theirShips.Any(theirShip => (ship.Position + ship.Velocity).DiagonalDistanceTo(theirShip.Position + theirShip.Velocity) < 5))
                {
                    commands.Add(new Command(CommandType.Detonate, ship.Id));
                }
            }

            var result = new LispNode();
            foreach (var command in commands)
            {
                result.Add(command.ToLispNode());
            }

            return
                new LispNode() {
                    new LispNode("4"),
                    new LispNode(playerKey),
                    result
                };
        }

        struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Point(int x, int y) 
            {
                X = x;
                Y = y;
            }

            public int SquareMagnitude()
            {
                return X * X + Y * Y;
            }

            public static Point operator- (Point lhs, Point rhs)
            {
                return new Point(lhs.X - rhs.X, lhs.X - rhs.X);
            }

            public static Point operator+ (Point lhs, Point rhs)
            {
                return new Point(lhs.X + rhs.X, lhs.X + rhs.X);
            }

            public int DiagonalDistanceTo(Point other)
            {
                var t = this - other;
                return Math.Max(Math.Abs(t.X), Math.Abs(t.Y));
            }

            public static readonly Point Zero = new Point(0, 0);
        }

        enum CommandType
        {
            Accelerate,
            Detonate,
            Shoot,
            Split
        }

        class Command
        {
            public int ShipID { get; set; }
            public CommandType Type { get; set; }
            public LispNode Arguments { get; set; }

            public Command(CommandType type, int shipID, LispNode args = null)
            {
                ShipID = shipID;
                Type = type;
                Arguments = args;
            }

            public LispNode ToLispNode()
            {
                var ans  = new LispNode() { new LispNode((int)Type), new LispNode(ShipID) };
                if (Arguments != null)
                {
                    ans.Add(Arguments);
                }
                
                return ans;
            }
        }

        class GameState
        {
            public int Tick { get; set; }
            public List<Ship> Ships { get; set; }

            public GameState(LispNode node)
            {
                Tick = int.Parse(node[0].Text);
                Ships = node[2].Select(i => new Ship(i[0])).ToList();
            }
        }

        enum Role
        {
            Attacker,
            Defender
        }

        class Ship
        {
            public Role Role { get; set; }
            public int Id { get; set; }
            public Point Position { get; set; }
            public Point Velocity { get; set; }
            public int Life { get; set; }
            public int Weapon { get; set; }
            public int Fuel { get; set; }
            public int Splits { get; set; }
            public int Energy { get; set; }
            public int MaxEnergy { get; set; }
            public bool Alive { get; set; }

            public Ship(LispNode node)
            {
                Role = (Role)int.Parse(node[0].Text);
                Id = int.Parse(node[1].Text);
                Position = new Point() { X = int.Parse(node[2][0].Text), Y = int.Parse(node[2][1].Text) };
                Velocity = new Point() { X = int.Parse(node[3][0].Text), Y = int.Parse(node[3][1].Text) };
                Life = int.Parse(node[4][0].Text);
                Weapon = int.Parse(node[4][1].Text);
                Fuel = int.Parse(node[4][2].Text);
                Splits = int.Parse(node[4][3].Text);
                Energy = int.Parse(node[5].Text);
                MaxEnergy = int.Parse(node[6].Text);
                Alive = node[7].Text != "0";
            }
        }

        class StaticGameState
        {
            public int MaxTicks { get; set; }
            public Role Role { get; set; }
            public int PlanetSize { get; set; }
            public int UniverseSize { get; set; }

            public StaticGameState(LispNode node)
            {
                MaxTicks = int.Parse(node[0].Text);
                Role = (Role)int.Parse(node[1].Text);
                PlanetSize = int.Parse(node[3][0].Text);
                UniverseSize = int.Parse(node[3][1].Text);
            }
        }
    }
}
