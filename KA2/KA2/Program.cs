using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KA2
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
             var graph = new List<List<int>>();

            int n;
            using (var stream = new StreamReader("in.txt"))
            {
                n = int.Parse(stream.ReadLine());
                foreach (var i in Enumerable.Range(1, n))
                {
                    var neibours = ParseNeibours(stream.ReadLine());
                    graph.Add(neibours);
                }
            }

            var components = new List<IEnumerable<int>>();
            var unvisitedNodes = new LinkedList<int>(Enumerable.Range(1, n));
            using (var stream = new StreamWriter("out.txt"))
            {
                while (unvisitedNodes.Count != 0)
                {
                    var startNode = unvisitedNodes.First.Value;
                    var connectedComponent = BreadthFirstSearch(graph, startNode).ToList();                    

                    foreach (var node in connectedComponent)
                        unvisitedNodes.Remove(node);

                    connectedComponent.Add(0);
                    components.Add(connectedComponent);
                    
                }
                stream.WriteLine(components.Count);
                foreach (var component in components)
                    stream.WriteLine(string.Join(" ", component));
            }
        }

        private static IEnumerable<int> GetNeibours(List<List<int>> graph, int node)
        {
            return graph[node - 1];
        }

        private static IEnumerable<int> BreadthFirstSearch(List<List<int>> graph, int startNode)
        {
            var connectedComponent = new List<int>();
            var visitedNodes = new HashSet<int>();
            var queue = new Queue<int>();
            queue.Enqueue(startNode);            
            while (queue.Count != 0)
            {
                var currentNode = queue.Dequeue();
                foreach (var neibour in GetNeibours(graph, currentNode))
                {
                    if (visitedNodes.Contains(neibour))
                        continue;

                    queue.Enqueue(neibour);
                    connectedComponent.Add(neibour);
                    visitedNodes.Add(neibour);
                }
            }
            connectedComponent.Sort();
            return connectedComponent;
        }

        private static List<int> ParseNeibours(string line) 
            => line.Substring(0, line.Length - 2).Split().Select(int.Parse).ToList();

    }
}
