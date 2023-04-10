using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utils;

public class BarracksController : MonoBehaviour
{
    public int productCount;

    public List<Vector3> productSpawnPoints = new List<Vector3>();

    [SerializeField] List<GameObject> products = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeInformationOnInfoPanel()
    {
        
    }

    public void CreateProduct(int value)
    {
       
        PathNode pathNode = Pathfinding.Instance.GetGrid().GetGridObject(Pathfinding.Instance.GetGrid().GetWorldPosition((int)productSpawnPoints[value].x,(int)productSpawnPoints[value].y)+new Vector3(5,5,0));
        if (pathNode.IsWalkable)
        {
            GameObject product = Instantiate(products[value], Pathfinding.Instance.GetGrid().GetWorldPosition((int)productSpawnPoints[value].x,(int)productSpawnPoints[value].y)+new Vector3(5,5,0), quaternion.identity);
            pathNode.IsProductionItemPlaced = true;
            pathNode.ProductionType = value;
            pathNode.ProductionObj = product;
            pathNode.ProductionObj.GetComponent<SoldierController>().IncreaseSoldierCount(1);
            pathNode.SetPlacedObject(pathNode.GetPlacedObject());
            pathNode.SetIsWalkable(false);
           
        }
        else
        {
            if (pathNode.IsProductionItemPlaced&&pathNode.ProductionType==value)
            {
                pathNode.ProductionObj.GetComponent<SoldierController>().IncreaseSoldierCount(1);
            }
            else
            {
                productSpawnPoints[value]=BuildManager.Instance.placedObjectTypeSos[0]
                    .FindProductionSpawnPoint(new Vector2Int((int)productSpawnPoints[value].x, (int)productSpawnPoints[value].y),PlacedObjectTypeSO.Dir.Down,value)[0];
                CreateProduct(value);
            }
           
        }
       
    }
    
    
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
