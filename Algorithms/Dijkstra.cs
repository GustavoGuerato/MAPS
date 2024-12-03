using System;
using System.Collections.Generic;

public class Dijkstra
{
    public class Edge
    {
        public int To { get; set; }
        public double Weight { get; set; }
    }

    public class Graph
    {
        public List<Edge>[] AdjList { get; set; }

        public Graph(int nodes)
        {
            AdjList = new List<Edge>[nodes];
            for (int i = 0; i < nodes; i++)
            {
                AdjList[i] = new List<Edge>();
            }
        }

        public void AddEdge(int from, int to, double weight)
        {
            AdjList[from].Add(new Edge { To = to, Weight = weight });
        }

        public double[] DijkstraAlgorithm(int start)
        {
            int nodes = AdjList.Length;
            double[] distances = new double[nodes];
            bool[] visited = new bool[nodes];
            PriorityQueue<int, double> pq = new PriorityQueue<int, double>();

            Array.Fill(distances, double.MaxValue);
            distances[start] = 0;
            pq.Enqueue(start, 0);

            while (pq.Count > 0)
            {
                var current = pq.Dequeue();

                if (visited[current]) continue;
                visited[current] = true;

                foreach (var edge in AdjList[current])
                {
                    if (distances[current] + edge.Weight < distances[edge.To])
                    {
                        distances[edge.To] = distances[current] + edge.Weight;
                        pq.Enqueue(edge.To, distances[edge.To]);
                    }
                }
            }

            return distances;
        }
    }
}
