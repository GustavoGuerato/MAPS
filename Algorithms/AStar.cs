using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class AStar
{
    private readonly HttpClient _httpClient;

    public AStar()
    {
        _httpClient = new HttpClient();
    }

    public class Node
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double GCost { get; set; }
        public double HCost { get; set; }
        public double FCost => GCost + HCost;
        public Node? Parent { get; set; }

        public Node(string name, double latitude, double longitude, Node? parent = null)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Parent = parent;
        }
    }

    private async Task<double> GetDistance(Node nodeA, Node nodeB)
    {
        string apiUrl = $"https://googlelatlog.azurewebsites.net/api/http_trigger?code=_Ka380mEoqT2bUQWBiG8olL2phzfPJPpJLZGp375kLCiAzFuouiTNw%3D%3D&lat1={nodeA.Latitude}&lon1={nodeA.Longitude}&lat2={nodeB.Latitude}&lon2={nodeB.Longitude}";

        var response = await _httpClient.GetStringAsync(apiUrl);
        var distance = JsonConvert.DeserializeObject<DistanceResponse>(response);

        return distance?.Distance ?? 0;
    }

    private async Task<Node> GetNodeFromAddress(string address)
    {
        string apiUrl = "http://localhost:5000/api/GeoLocation/GetCoordinates";

        var request = new AddressRequest { Endereco = address };
        var response = await _httpClient.PostAsJsonAsync(apiUrl, request);

        if (response.IsSuccessStatusCode)
        {
            var coordinates = await response.Content.ReadFromJsonAsync<CoordinatesResponse>();
            return new Node(address, coordinates.Latitude, coordinates.Longitude);
        }

        throw new Exception("Erro ao buscar coordenadas.");
    }

    public async Task<List<Node>?> FindPath(Node start, Node goal)
    {
        var openList = new List<Node> { start };
        var closedList = new List<Node>();

        while (openList.Any())
        {
            var currentNode = openList.OrderBy(node => node.FCost).First();

            if (currentNode.Name == goal.Name)
            {
                var path = new List<Node>();
                var temp = currentNode;
                while (temp != null)
                {
                    path.Insert(0, temp);
                    temp = temp.Parent;
                }
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            var neighbors = await GetNeighbors(currentNode);

            foreach (var neighbor in neighbors)
            {
                if (closedList.Contains(neighbor))
                    continue;

                var tentativeGCost = currentNode.GCost + await GetDistance(currentNode, neighbor);

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGCost >= neighbor.GCost)
                {
                    continue;
                }

                neighbor.Parent = currentNode;
                neighbor.GCost = tentativeGCost;
                neighbor.HCost = await GetDistance(neighbor, goal);
            }
        }

        return null;
    }

    private async Task<List<Node>> GetNeighbors(Node currentNode)
    {
        return new List<Node>
        {
            await GetNodeFromAddress("Endereço Exemplo 1"),
            await GetNodeFromAddress("Endereço Exemplo 2")
        };
    }

    public class DistanceResponse
    {
        public double Distance { get; set; }
    }

    public class AddressRequest
    {
        public required string Endereco { get; set; }
    }

    public class CoordinatesResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
