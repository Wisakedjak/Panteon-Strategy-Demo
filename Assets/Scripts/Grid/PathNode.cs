using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class PathNode
    {
        private Grid<PathNode> _grid;
        public int X;
        public int Y;

        public int GCost;
        public int HCost;
        public int FCost;

        public bool IsWalkable;
        public bool IsProductionItemPlaced;
        public int ProductionType;
        public GameObject ProductionObj;
        public PathNode CameFromNode;

        private PlacedObject _placedObject;
        
        public PathNode(Grid<PathNode> grid,int x,int y)
        {
            _grid = grid;
            X = x;
            Y = y;
            IsWalkable = true;
            
        }

        public void CalculateFCost()
        {
            FCost = GCost + HCost;
        }

        public void SetIsWalkable(bool isWalkable)
        {
            IsWalkable = isWalkable;
            
        }

        public override string ToString()
        {
            return X + "," + Y+"\n"+_placedObject; 
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            _placedObject = placedObject;
            _grid.TriggerGridObjectChanged(X,Y);
        }

        public void ClearPlacedObject()
        {
            _placedObject = null;
            _grid.TriggerGridObjectChanged(X,Y);
        }

        public bool CanBuild()
        {
            return _placedObject == null;
        }

        public PlacedObject GetPlacedObject()
        {
            return _placedObject;
        }
    }
}