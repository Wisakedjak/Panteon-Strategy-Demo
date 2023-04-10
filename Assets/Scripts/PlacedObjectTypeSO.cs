using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[CreateAssetMenu()]
public class PlacedObjectTypeSO : ScriptableObject {

    public static Dir GetNextDir(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:      return Dir.Left;
            case Dir.Left:      return Dir.Up;
            case Dir.Up:        return Dir.Right;
            case Dir.Right:     return Dir.Down;
        }
    }

    public enum Dir {
        Down,
        Left,
        Up,
        Right,
    }

    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;
    public Transform[] productionPoints;


    public int GetRotationAngle(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return 0;
            case Dir.Left:  return 90;
            case Dir.Up:    return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return new Vector2Int(0, 0);
            case Dir.Left:  return new Vector2Int(0, width);
            case Dir.Up:    return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir) {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < height; x++) {
                    for (int y = 0; y < width; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }
    
     public List<Vector3> FindProductionSpawnPoint(Vector2Int offset, Dir dir,int typeEnum) {
         List<Vector3> gridPosition = new List<Vector3>();
        int tmpBoundary = 1;
        switch (dir) {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0-tmpBoundary; x < width+tmpBoundary; x++) {
                    for (int y = 0-tmpBoundary; y < height+tmpBoundary; y++)
                    {
                        var tmpGridObj = Pathfinding.Instance.GetGrid()
                            .GetGridObject(new Vector3(offset.x + x, offset.y + y));
                        if (tmpGridObj!=null&&tmpGridObj.IsWalkable&& tmpGridObj.CanBuild())
                        {
                            if (tmpGridObj.IsProductionItemPlaced)
                            {
                                if (typeEnum==tmpGridObj.ProductionType)
                                {
                                    gridPosition.Add(new Vector3(offset.x + x, offset.y + y));
                                    
                                }
                            }
                            else
                            {
                                gridPosition.Add(new Vector3(offset.x + x, offset.y + y));
                                
                            }
                        }
                        
                    }
                    
                }
                break;
                
            case Dir.Left:
            case Dir.Right:
                for (int x = 0-tmpBoundary; x < height+tmpBoundary; x++) {
                    for (int y = 0-tmpBoundary; y < width+tmpBoundary; y++)
                    {
                        var tmpGridObj = Pathfinding.Instance.GetGrid()
                            .GetGridObject(new Vector3(offset.x + x, offset.y + y));
                        if (tmpGridObj!=null&&tmpGridObj.IsWalkable)
                        {
                            if (tmpGridObj.IsProductionItemPlaced)
                            {
                                if (typeEnum==tmpGridObj.ProductionType)
                                {
                                    gridPosition.Add(new Vector3(offset.x + x, offset.y + y));
                                    
                                }
                            }
                            else
                            {
                                gridPosition.Add(new Vector3(offset.x + x, offset.y + y));
                                
                            }
                        }
                        
                    }
                    
                }
                break;
                FindProductionSpawnPoint(offset, dir,typeEnum);
        }
        return gridPosition;
    }

}
