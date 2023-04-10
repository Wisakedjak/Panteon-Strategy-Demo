using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Utils
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance{ get; set; }
        [SerializeField]private PlacedObjectTypeSO placedObjectTypeSo;
        private Transform _visualTransform;
        public PlacedObjectTypeSO[] placedObjectTypeSos;


        private void Start()
        {
            Instance = this;
        }

        public void Build()
        {
            _build();
        }
        public void BuildEnemyBuilding(int x,int y)
        {
            _buildEnemyBuilding(x,y);
        }

        
        private void _build()
        {
            Pathfinding.Instance.GetGrid().GetXY(Utils.GetMouseWorldPosition(),out int x,out int y);
            print(x+","+y);
            PathNode gridObject = Pathfinding.Instance.GetGrid().GetGridObject(x, y);

            List<Vector2Int>gridPositionList = placedObjectTypeSo.GetGridPositionList(new Vector2Int(x, y), PlacedObjectTypeSO.Dir.Down);
            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                var tmpGridObj = Pathfinding.Instance.GetGrid().GetGridObject(gridPosition.x, gridPosition.y);
                if (tmpGridObj==null||!tmpGridObj.CanBuild())
                {
                    canBuild = false;
                    print("Cannot Build Here");
                    Destroy(_visualTransform.gameObject);
                    break;
                }
            }
            if (canBuild)
            {
                print("here");
                PlacedObject placedObject = PlacedObject.Create(Pathfinding.Instance.GetGrid().GetWorldPosition(x, y), new Vector2Int(x, y),
                    PlacedObjectTypeSO.Dir.Down, placedObjectTypeSo);
                // GameObject building = Instantiate(placedObjectTypeSo.prefab.gameObject, Pathfinding.Instance.GetGrid().GetWorldPosition(x, y),
                //     quaternion.identity);
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    Pathfinding.Instance.GetGrid().GetGridObject(gridPosition.x,gridPosition.y).SetPlacedObject(placedObject);
                    Pathfinding.Instance.GetGrid().GetGridObject(gridPosition.x,gridPosition.y).SetIsWalkable(false);
                    
                }
                if (placedObject.GetPlacedObjectTransform().TryGetComponent<BarracksController>(out BarracksController barracksController))
                {
                    for (int i = 0; i < barracksController.productCount; i++)
                    {
                        var spawnPoint = placedObjectTypeSo.FindProductionSpawnPoint(new Vector2Int(x, y),
                            PlacedObjectTypeSO.Dir.Down, i);
                        for (int j = 0; j < spawnPoint.Count; j++)
                        {
                            if (!barracksController.productSpawnPoints.Contains(spawnPoint[j]))
                            {
                                barracksController.productSpawnPoints.Add(spawnPoint[j]);
                                break;
                            }
                        }
                            
                            
                            
                    }
                }
                Destroy(_visualTransform.gameObject);
                
            }
            else
            {
                print("Cannot Build Here");
                Destroy(_visualTransform.gameObject);
            }

            TouchControlManager.Instance.isBuilding = false;
            TouchControlManager.Instance.isVisualPlaced = false;
        }
        private void _buildEnemyBuilding(int x,int y)
        {
            //Pathfinding.Instance.GetGrid().GetXY(Utils.GetMouseWorldPosition(),out int x,out int y);
            

            List<Vector2Int>gridPositionList = placedObjectTypeSo.GetGridPositionList(new Vector2Int(x, y), PlacedObjectTypeSO.Dir.Down);
           
            PlacedObject placedObject = PlacedObject.Create(Pathfinding.Instance.GetGrid().GetWorldPosition(x, y), new Vector2Int(x, y),
                PlacedObjectTypeSO.Dir.Down, placedObjectTypeSo);
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    Pathfinding.Instance.GetGrid().GetGridObject(gridPosition.x,gridPosition.y).SetPlacedObject(placedObject);
                    Pathfinding.Instance.GetGrid().GetGridObject(gridPosition.x,gridPosition.y).SetIsWalkable(false);
                    
                }
                
            
           
        }
        
        

        public void VisualPlaced()
        {
            Pathfinding.Instance.GetGrid().GetXY(Utils.GetMouseWorldPosition(),out int x,out int y);
            GameObject building = Instantiate(placedObjectTypeSo.visual.gameObject, Pathfinding.Instance.GetGrid().GetWorldPosition(x, y),
                quaternion.identity);
            _visualTransform = building.transform;
            TouchControlManager.Instance.isVisualPlaced = true;
        }

        private void _checkIfBuildingCanBePlaced()
        {
            Pathfinding.Instance.GetGrid().GetXY(Utils.GetMouseWorldPosition(),out int x,out int y);
            List<Vector2Int>gridPositionList = placedObjectTypeSo.GetGridPositionList(new Vector2Int(x, y), PlacedObjectTypeSO.Dir.Down);
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                var tmpGridObj = Pathfinding.Instance.GetGrid().GetGridObject(gridPosition.x, gridPosition.y);
                if (tmpGridObj==null||!tmpGridObj.CanBuild())
                {
                    _visualTransform.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 120);
                    break;
                }
                else
                {
                    _visualTransform.gameObject.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 120);
                }
            }
        }

        public void VisualFollowMouse()
        {
            Pathfinding.Instance.GetGrid().GetXY(Utils.GetMouseWorldPosition(),out int x,out int y);
            _visualTransform.position = Pathfinding.Instance.GetGrid().GetWorldPosition(x, y);
            _checkIfBuildingCanBePlaced();
        }

        public void SelectBuildingTypes(int buildType)
        {
            placedObjectTypeSo = placedObjectTypeSos[buildType];
            TouchControlManager.Instance.isBuilding = true;
        }
    }
}