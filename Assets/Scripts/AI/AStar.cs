using UnityEngine;
using System.Collections.Generic;

public class Node
{
    public Vector2Int Position { get; private set; }
    public Vector3 WorldPosition { get; private set; } // Adiciona a posição no mundo
    public bool IsWalkable { get; set; }

    // Propriedades específicas do A*
    public float GCost { get; set; }
    public float HCost { get; set; }
    public float FCost { get { return GCost + HCost; } }
    public Node Parent { get; set; }

    // Construtor atualizado para incluir a posição no mundo
    public Node(Vector2Int position, bool isWalkable, Vector3 worldPosition)
    {
        Position = position;
        IsWalkable = isWalkable;
        WorldPosition = worldPosition;
    }
}

public class AStarPathfinder
{
    public Node[,] Grid;
    public List<Node> ClosedListDebug = new List<Node>();
    public List<Node> PathDebug = new List<Node>();

    public AStarPathfinder(Node[,] Grid)
    {
        this.Grid = Grid;
    }

    public List<Node> FindPath(Vector2Int start, Vector2Int end)
    {
        Grid = GameManager.Instance.MapGenerator.Grid;

        Node startNode = Grid[start.x, start.y];
        Node endNode = Grid[end.x, end.y];

        PathDebug.Clear();
        ClosedListDebug.Clear();

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        openList.Add(startNode);

        int maxIterations = 500;
        int iterations = 0;

        while (openList.Count > 0)
        {
            if (iterations > maxIterations)
            {
                break;
            }
            iterations++;

            Node currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                PathDebug = new List<Node>(ReconstructPath(endNode, startNode));
                return new List<Node>(PathDebug);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            ClosedListDebug = new List<Node>(closedList);

            foreach (Node neighbor in GetNeighborNodes(currentNode))
            {
                if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGCost = currentNode.GCost + GetDistance(currentNode, neighbor);
                if (tentativeGCost < neighbor.GCost || !openList.Contains(neighbor))
                {
                    if (neighbor == currentNode)
                    {
                        Debug.LogError("Tentativa de atribuir um nó como seu próprio pai.");
                        continue;
                    }
                    neighbor.Parent = currentNode;
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = GetDistance(neighbor, endNode);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null; // Caminho não encontrado
    }

    private List<Node> GetNeighborNodes(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int[] dx = new int[] { 0, 0, -1, 1 };
        int[] dy = new int[] { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int checkX = node.Position.x + dx[i];
            int checkY = node.Position.y + dy[i];

            if (checkX >= 0 && checkX < Grid.GetLength(0) && checkY >= 0 && checkY < Grid.GetLength(1))
            {
                neighbors.Add(Grid[checkX, checkY]);
            }
        }

        return neighbors;
    }

    private Node GetLowestFCostNode(List<Node> openList)
    {
        Node lowestFCostNode = openList[0];
        for (int i = 1; i < openList.Count; i++)
        {
            if (openList[i].FCost < lowestFCostNode.FCost)
            {
                lowestFCostNode = openList[i];
            }
        }
        return lowestFCostNode;
    }

    private List<Node> ReconstructPath(Node endNode, Node startNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        int safetyCounter = 0; // Contador para evitar loop infinito

        while (currentNode != null && currentNode != startNode)
        {
            if (safetyCounter++ > 200) // Um número grande o suficiente
            {
                Debug.LogError("Loop infinito detectado na reconstrução do caminho.");
                break; // Interrompe o loop para evitar estouro de memória
            }
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }


    private float GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.Position.x - b.Position.x);
        int distY = Mathf.Abs(a.Position.y - b.Position.y);
        return distX + distY;
    }
}
