using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KA1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var figuresPositions = File.ReadAllText("in.txt")
                .Split(new [] {Environment.NewLine}, StringSplitOptions.None)
                .Select(TranslateFromChessNotation).ToArray();
            
            var route = DepthFirstSearch(figuresPositions[0], figuresPositions[1]);
            File.WriteAllLines("out.txt", route.Select(TranslateToChessNotation));
        }

        private static readonly IEnumerable<(int X, int Y)> KnightShifts = new[]
        {
            (-1, 2), (1, 2), (2, 1), (2, -1), (1, -2), (-1, -2), (-2, -1), (-2, 1)
        }.Reverse();

        private static IEnumerable<(int X, int Y)> EnumerateKnightNextPositions((int X, int Y) currentPosition)
            => KnightShifts
                .Select(shift => (X: currentPosition.X + shift.X, Y: currentPosition.Y + shift.Y))
                .Where(shift => shift.X >= 1 && shift.X <= 8 && shift.Y >= 1 && shift.Y <= 8);

        private static readonly IEnumerable<(int X, int Y)> PawnCapturingShifts = new[]
        {
            (-1, 1), (1, 1)
        };

        private static IEnumerable<(int X, int Y)> EnumeratePawnAttackedPositions((int X, int Y) currentPosition)
            => PawnCapturingShifts
                .Select(shift => (X: currentPosition.X + shift.X, Y: currentPosition.Y + shift.Y))
                .Where(shift => shift.X >= 1 && shift.X <= 8 && shift.Y >= 1 && shift.Y <= 8);
                    
        private static IEnumerable<(int, int)> DepthFirstSearch((int, int) knightPosition, (int, int) pawnPosition)
        {            
            var routeTracing = new Dictionary<(int, int), (int, int)>();
            var restrictedPositions = new HashSet<(int, int)>(EnumeratePawnAttackedPositions(pawnPosition));

            var stack = new Stack<(int, int)>();
            stack.Push(knightPosition);
            while (stack.Count > 0)
            {
                var currentKnightPosition = stack.Pop();
                foreach (var nextPosition in EnumerateKnightNextPositions(currentKnightPosition))
                {
                    if (routeTracing.ContainsKey(nextPosition) || restrictedPositions.Contains(nextPosition))
                        continue;
                                        
                    routeTracing[nextPosition] = currentKnightPosition;                                        
                    stack.Push(nextPosition);                    
                }

                if (routeTracing.ContainsKey(pawnPosition))
                {
                    var route = new List<(int, int)>();
                    var pathPoint = pawnPosition;
                    while (!pathPoint.Equals(knightPosition))
                    {
                        route.Add(pathPoint);
                        pathPoint = routeTracing[pathPoint];
                    }
                    route.Add(knightPosition);
                    return Enumerable.Reverse(route);
                }
            }

            return Enumerable.Empty<(int, int)>();
        }

        private static string TranslateToChessNotation((int X, int Y) position) 
            => $"{(char)('a' + position.X - 1)}{position.Y}";

        private static (int, int) TranslateFromChessNotation(string chessNotation)
            => (chessNotation[0] - 'a' + 1, int.Parse(chessNotation[1].ToString()));
    }
}
