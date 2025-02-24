﻿using System;
using System.Linq;
using System.Text;
using IcfpUtils;
using System.Net.Http;
using System.Net.Mime;
using System.Threading;
using System.Collections.Generic;
using System.Numerics;

namespace Solver
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int SquareMagnitude()
        {
            return X * X + Y * Y;
        }

        public static Point operator -(Point lhs, Point rhs)
        {
            return new Point(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Point operator +(Point lhs, Point rhs)
        {
            return new Point(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        public int DiagonalDistanceTo(Point other)
        {
            var t = this - other;
            return Math.Max(Math.Abs(t.X), Math.Abs(t.Y));
        }

        public int ManhattanDistanceTo(Point other)
        {
            var t = this - other;
            return Math.Abs(t.X) + Math.Abs(t.Y);
        }

        public static readonly Point Zero = new Point(0, 0);
    }

    public enum CommandType
    {
        Accelerate,
        Detonate,
        Shoot,
        Split
    }

    public class Command
    {
        public int ShipID { get; set; }
        public CommandType Type { get; set; }
        public LispNode Argument1 { get; set; }
        public LispNode Argument2 { get; set; }
        public Point Vector { get; set; }

        public static Command Accelerate(int id, Point vec)
        {
            return new Command(
                CommandType.Accelerate,
                id,
                new LispNode(
                    new LispNode(
                        new LispNode("cons"),
                        new LispNode(vec.X)),
                    new LispNode(vec.Y)))
                { Vector = vec };
        }

        public static Command Detonate(int id)
        {
            return new Command(
                CommandType.Detonate,
                id);
        }

        public static Command Shoot(int id, Point target, int power)
        {
            return new Command(
                CommandType.Shoot,
                id,
                new LispNode(
                    new LispNode(
                        new LispNode("cons"),
                        new LispNode(target.X)),
                    new LispNode(target.Y)),
                new LispNode(power))
            { Vector = target };
        }

        public static Command Split(int id, LispNode argument)
        {
            return new Command(
                CommandType.Split,
                id,
                Common.Unflatten(argument));
        }

        private Command() { }

        private Command(CommandType type, int shipID, LispNode argument1 = null, LispNode argument2 = null)
        {
            ShipID = shipID;
            Type = type;
            Argument1 = argument1;
            Argument2 = argument2;
        }

        public LispNode ToLispNode()
        {
            var ans = Common.Unflatten(new LispNode() { new LispNode((int)Type), new LispNode(ShipID) });
            
            if (Argument1 != null)
            {
                ans[1].Children[1] =
                    new LispNode(
                        new LispNode(new LispNode("cons"), Argument1),
                        new LispNode("nil"));
            }

            if (Argument2 != null)
            {
                ans[1].Children[1].Children[1] =
                    new LispNode(
                        new LispNode(new LispNode("cons"), Argument2),
                        new LispNode("nil"));
            }

            return ans;
        }
    }

    public class GameState
    {
        public int Tick { get; set; }
        public List<Ship> Ships { get; set; }

        public GameState() { }

        public GameState(LispNode node)
        {
            Tick = int.Parse(node[0].Text);
            Ships = node[2].Select(i => new Ship(i[0])).ToList();
        }
    }

    public enum Role
    {
        Attacker,
        Defender
    }

    public class Ship
    {
        public Role Role { get; set; }
        public int Id { get; set; }
        public Point Position { get; set; }
        public Point Velocity { get; set; }
        public int Life { get; set; }
        public int Weapon { get; set; }
        public int Recharge { get; set; }
        public int Splits { get; set; }
        public int Energy { get; set; }
        public int EnergyLeft { get => MaxEnergy - Energy; }
        public int MaxEnergy { get; set; }
        public bool Alive { get; set; }

        public Ship()
        {
        }

        public Ship(LispNode node)
        {
            Role = (Role)int.Parse(node[0].Text);
            Id = int.Parse(node[1].Text);
            Position = new Point() { X = int.Parse(node[2][0].Text), Y = int.Parse(node[2][1].Text) };
            Velocity = new Point() { X = int.Parse(node[3][0].Text), Y = int.Parse(node[3][1].Text) };
            Life = int.Parse(node[4][0].Text);
            Weapon = int.Parse(node[4][1].Text);
            Recharge = int.Parse(node[4][2].Text);
            Splits = int.Parse(node[4][3].Text);
            Energy = int.Parse(node[5].Text);
            MaxEnergy = int.Parse(node[6].Text);
            Alive = node[7].Text != "0";
        }
    }

    public class StaticGameState
    {
        public int MaxTicks { get; set; }
        public Role Role { get; set; }
        public int PlanetSize { get; set; }
        public int UniverseSize { get; set; }
        public int DefaultLife { get; set; }
        public int DefaultWeapon { get; set; }
        public int DefaultRecharge { get; set; }
        public int DefaultSplit { get; set; }

        public StaticGameState()
        {
            PlanetSize = 5;
            UniverseSize = 64;
            DefaultLife = 1;
            DefaultSplit = 1;
        }

        public StaticGameState(LispNode node) : this()
        {
            if (node.Count() < 4)
            {
                return;
            }

            MaxTicks = int.Parse(node[0].Text);
            Role = (Role)int.Parse(node[1].Text);

            if (node[3].Count() >= 2)
            {
                PlanetSize = int.Parse(node[3][0].Text);
                UniverseSize = int.Parse(node[3][1].Text);
            }

            if (node[4].Count() >= 4)
            {
                DefaultLife = int.Parse(node[4][0].Text);
                DefaultWeapon = int.Parse(node[4][1].Text);
                DefaultRecharge = int.Parse(node[4][2].Text);
                DefaultSplit = int.Parse(node[4][3].Text);
            }
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var serverUrl = args[0];
            var playerKey = args[1];

            var gameResponse = Send(serverUrl, MakeJoinRequest(playerKey));

            do
            {
                gameResponse = Send(serverUrl, MakeStartRequest(playerKey, gameResponse));
            } while (!IsSuccess(gameResponse));

            while (true)
            {
                gameResponse = Send(serverUrl, MakeCommandsRequest(playerKey, gameResponse));
            }
        }

        static bool IsSuccess(LispNode node)
        {
            return node[0].Text == "1";
        }

        public static LispNode Send(string serverUrl, LispNode request, string queryParams = null)
        {
            if (!Uri.TryCreate(serverUrl + "/aliens/send" + (queryParams ?? ""), UriKind.Absolute, out var serverUri))
            {
                throw new Exception($"Failed to parse ServerUrl {serverUrl}");
            }

            using var httpClient = new HttpClient { BaseAddress = serverUri };
            var requestContent = new StringContent(Common.Modulate(request), Encoding.UTF8, MediaTypeNames.Text.Plain);
            using var response = httpClient.PostAsync("", requestContent).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Unexpected server response: {response}");
            }

            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = Common.Flatten(Common.Demodulate(responseString).Item1);
            Console.WriteLine($"Sent {Common.Flatten(request)}] received [{result}]]");
            //Console.WriteLine($"Sent {Common.Flatten(request)}] received [{result}] -- [{request}]");

            return result;
        }

        static LispNode MakeJoinRequest(string playerKey)
        {
            return
                Common.Unflatten(
                    new LispNode() {
                        new LispNode("2"),
                        new LispNode(playerKey),
                        new LispNode()
                    });
        }

        // CASHTO
        static int StartRequestIndex = 0;
        static readonly List<StaticGameState> StartRequests = new List<StaticGameState>() {
            new StaticGameState()
            {
                DefaultLife = 100,       // 450 .. 500  -> 100
                DefaultWeapon = 40,     // 100 .. 125  -> 40
                DefaultRecharge = 14,   // 33 .. 40    -> 12
                DefaultSplit = 9      // 16 .. ->2
            },
        };

        //    new StaticGameState()
        //    {
        //        DefaultLife = 350,
        //        DefaultWeapon = 0,
        //        DefaultRecharge = 8,
        //        DefaultSplit = 1
        //    },

        //    new StaticGameState()
        //    {
        //        DefaultLife = 10,
        //        DefaultWeapon = 10,
        //        DefaultRecharge = 10,
        //        DefaultSplit = 1
        //    },

        //    new StaticGameState()
        //    {
        //        DefaultLife = 0,
        //        DefaultWeapon = 16,
        //        DefaultRecharge = 16,
        //        DefaultSplit = 1
        //    },

        //    new StaticGameState()
        //    {
        //        DefaultLife = 1,
        //        DefaultWeapon = 0,
        //        DefaultRecharge = 0,
        //        DefaultSplit = 1
        //    },

        //    new StaticGameState()
        //    {
        //        DefaultLife = 0,
        //        DefaultWeapon = 0,
        //        DefaultRecharge = 0,
        //        DefaultSplit = 1
        //    },
        //};

        public static LispNode MakeStartRequest(string playerKey, LispNode gameResponse)
        {
            var staticGameState = StartRequests[0];

            return 
                Common.Unflatten(
                    new LispNode() { 
                        new LispNode("3"), 
                        new LispNode(playerKey), 
                        new LispNode() {
                            new LispNode(staticGameState.DefaultLife),
                            new LispNode(staticGameState.DefaultWeapon),
                            new LispNode(staticGameState.DefaultRecharge),
                            new LispNode(staticGameState.DefaultSplit)
                        }
                    });
        }

        static Point Scale(Point p, int scale)
        {
            var mag = Math.Sqrt(p.SquareMagnitude());
            return new Point((int)(scale * p.X / mag), (int)(scale * p.Y / mag));
        }

        public static LispNode MakeCommandsRequest(string playerKey, LispNode gameResponse)
        {
            var commands = !IsSuccess(gameResponse) ?
                new List<Command>() :
                MakeCommandsRequest(
                    new GameState(gameResponse[3]), 
                    new StaticGameState(gameResponse[2]));

            var ans = Common.Unflatten(
                new LispNode() {
                    new LispNode("4"),
                    new LispNode(playerKey) });

            var commandsList = new LispNode("nil");
            foreach (var command in commands)
            {
                commandsList = new LispNode(
                    new LispNode(new LispNode("cons"), command.ToLispNode()),
                    commandsList);
            }

            ans[1].Children[1] = new LispNode(new LispNode(new LispNode("cons"), commandsList), new LispNode("nil"));
            return ans;
        }

        public static Point CalculateGravity(Point ship, int planetSize)
        {
            if (ship.X > ship.Y && ship.X > -ship.Y)
            {
                return new Point(-1, 0);
            }
            else if (ship.X < ship.Y && ship.X < -ship.Y)
            {
                return new Point(1, 0);
            }
            else if (ship.Y > 0)
            {
                return new Point(0, -1);
            }
            else
            {
                return new Point(0, 1);
            }
        }

        static bool InUniverse(Point p, StaticGameState staticGameState)
        {
            var planetSize = (staticGameState.PlanetSize * 4) / 3;
            return 
                (Math.Abs(p.X) > planetSize || Math.Abs(p.Y) > planetSize) &&
                Math.Abs(p.X) < staticGameState.UniverseSize && Math.Abs(p.Y) < staticGameState.UniverseSize;
        }

        static int GetScore(GameState gameState, StaticGameState staticGameState, Ship myShip)
        {
            return FutureStates(staticGameState, gameState).Sum(state =>
            {
                var enemyShips = state.Ships.Where(s => s.Role != staticGameState.Role).ToList();

                var distanceMin = enemyShips.Select(s => s.Position.ManhattanDistanceTo(myShip.Position)).Min();

                return staticGameState.Role == Role.Attacker ? -distanceMin : distanceMin;
            });
        }

        static IEnumerable<GameState> FutureStates (StaticGameState staticGameState, GameState gameState)
        {
            foreach (var i in Enumerable.Range(0, 12))
            {
                gameState = new GameState()
                {
                    Ships = (
                        from s in gameState.Ships
                        let gravity = CalculateGravity(s.Position, staticGameState.PlanetSize)
                        let delta = Point.Zero
                        select new Ship()
                        {
                            Id = s.Id,
                            Role = s.Role,
                            Energy = Common.Bound(s.Energy - s.Recharge, 0, 64),
                            Position = s.Position + s.Velocity + gravity - delta,
                            Velocity = s.Velocity + gravity - delta,
                            Life = s.Life - (delta.SquareMagnitude() == 0 ? 0 : 1)
                        }).ToList()
                };

                yield return gameState;
            }
        }

        static bool ImStillInUniverse(GameState gameState, StaticGameState staticGameState, int shipId)
        {
            var myShip = GetShip(gameState, shipId);
            return InUniverse(myShip.Position, staticGameState);
        }

        static Command Search(GameState originalGameState, StaticGameState staticGameState, Ship ship)
        {
            var planetSize = staticGameState.PlanetSize;
            var universeSize = staticGameState.UniverseSize;

            var nodes = Algorithims.Search<GameState, Command>(
                originalGameState,
                new DepthFirstSearch<GameState, Command>(),
                CancellationToken.None,
                (sn) => GenerateMoves(sn, staticGameState, ship));

            nodes = nodes.ToList();

            var orderedNodes =
                from node in nodes
                where node.Depth == 3
                let myShip = GetShip(node.State, ship.Id)
                where InUniverse(myShip.Position, staticGameState)
                let alive = FutureStates(staticGameState, node.State).Count(state => ImStillInUniverse(state, staticGameState, ship.Id))
                let score = GetScore(node.State, staticGameState, myShip)
                orderby alive descending, score descending
                select node;

            orderedNodes = orderedNodes.ToList();

            var theNode = orderedNodes.First();
            Console.WriteLine("Moves: " + string.Join("; ", theNode.Moves.Select(move => move.Vector)));
            Console.WriteLine("Positions: " + string.Join("; ", theNode.States.Select(state => GetShip(state, ship.Id).Position)));
            Console.WriteLine("Velocities: " + string.Join("; ", theNode.States.Select(state => GetShip(state, ship.Id).Velocity)));

            return theNode.Move;
        }

        static Ship GetShip(GameState gameState, int shipId)
        {
            return gameState.Ships.First(s => s.Id == shipId);
        }

        static IEnumerable<SearchNode<GameState, Command>> GenerateMoves(
            SearchNode<GameState, Command> searchNode, 
            StaticGameState staticGameState,
            Ship ship)
        {
            if (searchNode.Depth >= 3)
            {
                yield break;
            }

            var myShip = GetShip(searchNode.State, ship.Id);
            if (!InUniverse(myShip.Position, staticGameState))
            {
                yield break;
            }

            foreach (var x in Enumerable.Range(-1, 3))
            {
                foreach (var y in Enumerable.Range(-1, 3))
                {
                    var command = Command.Accelerate(ship.Id, new Point(x, y));
                    var newState = new GameState()
                    {
                        Ships = (
                            from s in searchNode.State.Ships
                            let gravity = CalculateGravity(s.Position, staticGameState.PlanetSize)
                            let delta = s.Id == ship.Id ? command.Vector : Point.Zero
                            select new Ship()
                            {
                                Id = s.Id,
                                Role = s.Role,
                                Energy = Common.Bound(s.Energy - s.Recharge, 0, 64),
                                Position = s.Position + s.Velocity + gravity - delta ,
                                Velocity = s.Velocity + gravity - delta,
                                Life = s.Life - (delta.SquareMagnitude() == 0 ? 0 : 1)
                            }).ToList()
                    };

                    yield return searchNode.Create(newState, command);
                }
            }
        }

        public static Point GetDesiredVelocity(Ship ship, int planetSize)
        {
            var gravity = CalculateGravity(ship.Position, planetSize);
            var desiredVelocity1 = Scale(new Point(ship.Position.Y, -ship.Position.X), 8);
            var desiredVelocity2 = Scale(new Point(-ship.Position.Y, ship.Position.X), 8);
            var desiredVelocity = ((ship.Velocity + gravity) - desiredVelocity1).SquareMagnitude() > ((ship.Velocity + gravity) - desiredVelocity2).SquareMagnitude() ?
                desiredVelocity2 : desiredVelocity1;
            return desiredVelocity;
        }

        public static List<Command> MakeCommandsRequest(GameState gameState, StaticGameState staticGameState)
        {
            var myShips = gameState.Ships.Where(i => i.Role == staticGameState.Role);
            var enemyShips = gameState.Ships.Where(i => i.Role != staticGameState.Role).ToList();
            var commands = new List<Command>();
            foreach (var ship in myShips)
            {
                var energyLeft = ship.MaxEnergy - ship.Energy + ship.Recharge;
                var accelVector = Point.Zero;
                var gravity = CalculateGravity(ship.Position, staticGameState.PlanetSize);
                var desiredVelocity = GetDesiredVelocity(ship, staticGameState.PlanetSize);

                try
                {
                    accelVector = Search(gameState, staticGameState, ship).Vector;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Search exception {e})");
                    accelVector = (ship.Velocity + gravity) - desiredVelocity;
                }

                accelVector.X = Common.Bound(accelVector.X, -1, 1);
                accelVector.Y = Common.Bound(accelVector.Y, -1, 1);

                if (accelVector.X != 0 && accelVector.Y != 0 && energyLeft >= 8 && ship.Life > 0)
                {
                    commands.Add(Command.Accelerate(ship.Id, accelVector));
                    energyLeft -= 8;
                }

                if (staticGameState.Role == Role.Attacker &&
                    enemyShips.All(theirShip => (ship.Position + ship.Velocity + gravity).DiagonalDistanceTo(theirShip.Position + theirShip.Velocity + gravity) < 5))
                {
                    commands.Add(Command.Detonate(ship.Id));
                }

                if (energyLeft > 0)
                {
                    var sortedEnemies =
                        from enemyShip in enemyShips
                        let distance = (enemyShip.Position + enemyShip.Velocity + gravity).ManhattanDistanceTo(ship.Position + ship.Velocity + gravity)
                        orderby distance ascending
                        select enemyShip;

                    var closestEnemy = sortedEnemies.First();
                    var closestEnemyDistance = (closestEnemy.Position + closestEnemy.Velocity + gravity).ManhattanDistanceTo(ship.Position + ship.Velocity + gravity);

                    if (closestEnemyDistance < 32)
                    {
                        commands.Add(Command.Shoot(ship.Id, closestEnemy.Position + closestEnemy.Velocity + gravity, energyLeft));
                    }

                }

                if (staticGameState.Role == Role.Defender &&
                    ship.Splits > 1 &&
                    ship.Velocity.ManhattanDistanceTo(desiredVelocity) <= 2 &&
                    ship.Life >= 20)
                {
                    commands.Add(Command.Split(ship.Id, new LispNode() { new LispNode(10), new LispNode(0), new LispNode(1), new LispNode(1) }));
                }
            }

            return commands;
        }
    }
}
