using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class Pathfinding
    {
        private const int MoveStraightCost = 10;
        private const int MoveDiagonalCost = 14;
        
        public static Pathfinding Instance { get; set; }
        
        private Grid<PathNode> _grid;
        private List<PathNode> _openList;
        private List<PathNode> _closedList;
        public Pathfinding(int width, int height)
        {
            
            _grid = new Grid<PathNode>(width, height, 10f, Vector3.zero,
                ((Grid<PathNode> g, int x, int y) => new PathNode(g, x, y)));
            Instance=this;
        }

        public Grid<PathNode> GetGrid()
        {
            return _grid;
        }

        public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
        {
            _grid.GetXY(startWorldPosition,out int startX,out int startY);
            _grid.GetXY(endWorldPosition,out int endX,out int endY);

            List<PathNode> pathNodes = FindPath(startX, startY, endX, endY);
            if (pathNodes==null)
            {
                return null;
            }
            else
            {
                List<Vector3> vectorPath = new List<Vector3>();
                foreach (PathNode pathNode in pathNodes)
                {
                    vectorPath.Add(new Vector3(pathNode.X,pathNode.Y)*_grid.GetCellSize()+Vector3.one*_grid.GetCellSize()*.5f);
                }
                pathNodes[0].SetIsWalkable(true);
                pathNodes[0].ClearPlacedObject();
                pathNodes[^1].SetIsWalkable(false);
                pathNodes[^1].SetPlacedObject(pathNodes[^1].GetPlacedObject());
                return vectorPath;
                
            }
        }
        public List<Vector3> FindPathForAttack(Vector3 startWorldPosition, Vector3 endWorldPosition)
        {
            _grid.GetXY(startWorldPosition,out int startX,out int startY);
            _grid.GetXY(endWorldPosition,out int endX,out int endY);

            List<PathNode> pathNodes = FindPathForAttack(startX, startY, endX, endY);
            if (pathNodes==null)
            {
                return null;
            }
            else
            {
                List<Vector3> vectorPath = new List<Vector3>();
                foreach (PathNode pathNode in pathNodes)
                {
                    if (pathNode.IsWalkable)
                    {
                        vectorPath.Add(new Vector3(pathNode.X,pathNode.Y)*_grid.GetCellSize()+Vector3.one*_grid.GetCellSize()*.5f);
                    }
                    
                }

                return vectorPath;
            }
        }

        public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            PathNode startNode = _grid.GetGridObject(startX, startY);
            PathNode endNode = _grid.GetGridObject(endX, endY);
            _openList = new List<PathNode>{startNode};
            _closedList = new List<PathNode>();

            for (int i = 0; i < _grid.GetWidth(); i++)
            {
                for (int j = 0; j < _grid.GetHeight(); j++)
                {
                    PathNode pathNode = _grid.GetGridObject(i, j);
                    pathNode.GCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.CameFromNode = null;
                }
            }

            startNode.GCost = 0;
            startNode.HCost = _calculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (_openList.Count>0)
            {
                PathNode currentNode = _GetLowestFCostNode(_openList);
                if (currentNode==endNode)
                {
                    return _calculatePath(endNode);
                }

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                foreach (PathNode neighbourNode in _getNeighbourList(currentNode))
                {
                    if (_closedList.Contains(neighbourNode))
                    {
                        continue;
                    }

                    if (!neighbourNode.IsWalkable)
                    {
                        _closedList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.GCost + _calculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost<neighbourNode.GCost)
                    {
                        neighbourNode.CameFromNode = currentNode;
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = _calculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!_openList.Contains(neighbourNode))
                        {
                            _openList.Add(neighbourNode);
                        }
                    }
                }
            }
            
            //No path available
            return null;
        }
        public List<PathNode> FindPathForAttack(int startX, int startY, int endX, int endY)
        {
            PathNode startNode = _grid.GetGridObject(startX, startY);
            PathNode endNode = _grid.GetGridObject(endX, endY);
            _openList = new List<PathNode>{startNode};
            _closedList = new List<PathNode>();

            for (int i = 0; i < _grid.GetWidth(); i++)
            {
                for (int j = 0; j < _grid.GetHeight(); j++)
                {
                    PathNode pathNode = _grid.GetGridObject(i, j);
                    pathNode.GCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.CameFromNode = null;
                }
            }

            startNode.GCost = 0;
            startNode.HCost = _calculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (_openList.Count>0)
            {
                PathNode currentNode = _GetLowestFCostNode(_openList);
                if (currentNode==endNode)
                {
                    return _calculatePath(endNode);
                }

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                foreach (PathNode neighbourNode in _getNeighbourList(currentNode))
                {
                    if (_closedList.Contains(neighbourNode))
                    {
                        continue;
                    }
                    
                    
                 

                    int tentativeGCost = currentNode.GCost + _calculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost<neighbourNode.GCost)
                    {
                        neighbourNode.CameFromNode = currentNode;
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = _calculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!_openList.Contains(neighbourNode))
                        {
                            _openList.Add(neighbourNode);
                        }
                    }
                }
            }
            
            //No path available
            return null;
        }

        private List<PathNode> _getNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            if (currentNode.X-1>=0)
            {
                neighbourList.Add(_getNode(currentNode.X-1,currentNode.Y));
                if (currentNode.Y-1>=0) 
                {
                    neighbourList.Add(_getNode(currentNode.X-1,currentNode.Y-1));
                }
                if (currentNode.Y+1<_grid.GetHeight()) 
                {
                    neighbourList.Add(_getNode(currentNode.X-1,currentNode.Y+1));
                }
            }

            if (currentNode.X+1<_grid.GetWidth())
            {
                neighbourList.Add(_getNode(currentNode.X+1,currentNode.Y));
                if (currentNode.Y-1>=0)
                {
                    neighbourList.Add(_getNode(currentNode.X+1,currentNode.Y-1));
                }

                if (currentNode.Y+1<_grid.GetHeight())
                {
                    neighbourList.Add(_getNode(currentNode.X+1,currentNode.Y+1));
                }
            }

            if (currentNode.Y-1>=0)
            {
                neighbourList.Add(_getNode(currentNode.X,currentNode.Y-1));
            }

            if (currentNode.Y+1<_grid.GetHeight())
            {
                neighbourList.Add(_getNode(currentNode.X,currentNode.Y+1));
            }

            return neighbourList;
        }

        public PathNode _getNode(int x, int y)
        {
            return _grid.GetGridObject(x, y);
        }

        private List<PathNode> _calculatePath(PathNode endNode)
        {
            List<PathNode> pathNodes = new List<PathNode>();
            pathNodes.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.CameFromNode!=null)
            {
                pathNodes.Add(currentNode.CameFromNode);
                currentNode = currentNode.CameFromNode;
            }
            
            pathNodes.Reverse();
            return pathNodes;
        }

        int _calculateDistanceCost(PathNode a, PathNode b)
        {
            int xDistance = Mathf.Abs(a.X - b.X);
            int yDistance = Mathf.Abs(a.Y - b.Y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return MoveDiagonalCost * Mathf.Min(xDistance, yDistance) + MoveStraightCost * remaining;
        }

        PathNode _GetLowestFCostNode(List<PathNode> pathNodes)
        {
            PathNode lowestFCostNode = pathNodes[0];
            for (int i = 0; i < pathNodes.Count; i++)
            {
                if (pathNodes[i].FCost<lowestFCostNode.FCost)
                {
                    lowestFCostNode = pathNodes[i];
                }
            }

            return lowestFCostNode;
        }
    }

   
}