using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>

    bool isSet = false;

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> OpenList = new List<Node>();
        List<Node> ClosedList = new List<Node>();

        //Initial Node
        Node currentNode = new Node(startPos, null, 0, Vector2.Distance(startPos, endPos));

        OpenList.Add(currentNode);

        int iterations = 0;

        while (OpenList.Count > 0)
        {
            iterations++;

            //Get lowest
            Node lowest = null;
            foreach (Node node in OpenList)
            {
                if(lowest == null || node.FScore < lowest.FScore || node.FScore == lowest.FScore && node.HScore < lowest.HScore)
                {
                    lowest = node;
                }
            }
            currentNode = lowest;

            //Add current to closed and remove from open
            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            if(currentNode.position == endPos)
            {
                break;
            }

            foreach (Vector2Int neighbourPosition in GetNeigbours(currentNode.position, grid))
            {
                Node parent = currentNode;
                int GScore = currentNode.GScore++;
                float HScore = Vector2.Distance(new Vector2Int(neighbourPosition.x, neighbourPosition.y), endPos);
                Node node = new Node(neighbourPosition, parent, GScore, HScore);

                if (ClosedList.Contains(node))
                {
                    continue;
                }
                float movementCost = currentNode.GScore + Vector2.Distance(currentNode.position, node.position);
                if(!OpenList.Contains(node) || node.GScore < movementCost)
                {
                    node.GScore = currentNode.GScore++;
                    node.HScore = Vector2.Distance(new Vector2Int(node.position.x, node.position.y), endPos);
                    node.parent = currentNode;
                    if(!OpenList.Contains(node))
                    {
                        OpenList.Add(node);
                    }
                }
            }
            if(iterations > 500)
            {
                break;
            }
        }

        List<Vector2Int> path = new List<Vector2Int>();
        while(currentNode.position != startPos)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    //Get neighbours of cell
    public List<Vector2Int> GetNeigbours(Vector2Int gridPosition, Cell[,] grid)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        //Find all neighbours
        for (int xx = -1; xx <= 1; xx++)
        {
            for (int yy = -1; yy <= 1; yy++)
            {
                int x = gridPosition.x + xx;
                int y = gridPosition.y + yy;

                //Exclude diagonal neighbours
                if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.Rank)
                {
                    continue;
                }

                if(Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }
                Vector2Int neighbour = grid[x, y].gridPosition;
                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public int GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, float HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
