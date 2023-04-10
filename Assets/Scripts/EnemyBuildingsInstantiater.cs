using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utils;

public class EnemyBuildingsInstantiater : MonoBehaviour
{
    public static EnemyBuildingsInstantiater Instance { get; set; }

    [SerializeField] private List<GameObject> products = new List<GameObject>();
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void BuildEnemyBuildings()
    {
        _buildEnemyBarrack();
        _buildEnemyPowerPlant();
        _createSoldier();
    }
    
    

    private void _buildEnemyBarrack()
    {
        BuildManager.Instance.SelectBuildingTypes(2);
        BuildManager.Instance.BuildEnemyBuilding(7,8);
    } 
    private void _buildEnemyPowerPlant()
    {
        BuildManager.Instance.SelectBuildingTypes(3);
        BuildManager.Instance.BuildEnemyBuilding(3,4);
    }

    private void _createSoldier()
    {
        for (int i = 0; i < 3; i++)
        {
            PathNode pathNode = Pathfinding.Instance.GetGrid().GetGridObject(Pathfinding.Instance.GetGrid().GetWorldPosition(6,8+i)+new Vector3(5,5,0));
            GameObject product = Instantiate(products[i], Pathfinding.Instance.GetGrid().GetWorldPosition(6,8+i)+new Vector3(5,5,0), quaternion.identity);
            pathNode.IsProductionItemPlaced = true;
            pathNode.ProductionType = i;
            pathNode.ProductionObj = product;
            pathNode.SetPlacedObject(this.GetComponent<PlacedObject>());
            pathNode.SetIsWalkable(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
