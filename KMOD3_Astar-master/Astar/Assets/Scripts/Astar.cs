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
        Node startNode = new Node(startPos, null, 0,0);
        Node targetNode = new Node(endPos, null, 0, 0);

        OpenList.Add(startNode);

        Node currentNode = OpenList[0];

        int iterations = 0;

        while (OpenList.Count > 0)
        {
            iterations++;

            //Get lowest

            foreach (Node node in OpenList)
            {
                if (node.FScore < currentNode.FScore || node.FScore == currentNode.FScore && node.HScore < currentNode.HScore)
                {
                    currentNode = node;
                }
            }

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

                if (ClosedList.Any(n => n.position == neighbourPosition))
                {
                    continue;
                }
                int moveCost = (int)(GScore + Vector2.Distance(node.position, endPos));
                if (!OpenList.Any(n => n.position == neighbourPosition) || moveCost < currentNode.FScore)
                {
                    node.GScore = moveCost;
                    node.HScore = Vector2.Distance(new Vector2Int(node.position.x, node.position.y), endPos);
                    if(!OpenList.Contains(node))
                    {
                        OpenList.Add(node);
                    }
                }
            }

            if(iterations > 2500)
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
                if (Mathf.Abs(xx) == Mathf.Abs(yy))
                {
                    continue;
                }

                int x = gridPosition.x + xx;
                int y = gridPosition.y + yy;

                //Exclude diagonal neighbours
                if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
                {
                    continue;
                }
                
                Vector2Int neighbour = grid[x, y].gridPosition;
                neighbours.Add(neighbour);
            }
        }

        //Remove neighbours at walls
        Cell currentCell = grid[gridPosition.x, gridPosition.y];
        neighbours = CheckWalls(neighbours, currentCell);

        return neighbours;
    }

    public List<Vector2Int> CheckWalls(List<Vector2Int> neighbours, Cell cell)
    {
        List<Vector2Int> WallCheckedNeighbours = neighbours;

        if (cell.HasWall(Wall.UP))
        {
            WallCheckedNeighbours.Remove(new Vector2Int(cell.gridPosition.x , cell.gridPosition.y + 1));
        }
        if (cell.HasWall(Wall.DOWN))
        {
            WallCheckedNeighbours.Remove(new Vector2Int(cell.gridPosition.x, cell.gridPosition.y - 1));
        }
        if (cell.HasWall(Wall.LEFT))
        {
            WallCheckedNeighbours.Remove(new Vector2Int(cell.gridPosition.x - 1, cell.gridPosition.y));
        }
        if (cell.HasWall(Wall.RIGHT))
        {
            WallCheckedNeighbours.Remove(new Vector2Int(cell.gridPosition.x + 1, cell.gridPosition.y));
        }

        return WallCheckedNeighbours;
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
