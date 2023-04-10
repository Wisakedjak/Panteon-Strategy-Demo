using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool isBuilding;
    [SerializeField] private int damage, healthPoint;
    private int _soldierCount;
    void Start()
    {
        if (!isBuilding)
        {
            _soldierCount = int.Parse(transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text);
        }
    }

    public  void _reduceHealth(GameObject target,int damageTaken)
    {
        SoldierController soldierController = target.GetComponent<SoldierController>();
        healthPoint -= damageTaken;
        if (!isBuilding&&healthPoint>0)
        {
            _soldierCount = ((_soldierCount * healthPoint) - damageTaken) / healthPoint;
            soldierController.ReduceHealth(_soldierCount*damage);
        }

        if (isBuilding&& healthPoint>0)
        {
            soldierController.Attack();
        }

        if (healthPoint<=0)
        {
            Pathfinding.Instance.GetGrid().GetXY(transform.position, out int x, out int y);
            PathNode pathNode = Pathfinding.Instance.GetGrid().GetGridObject(x, y);
            PlacedObject placedObject = gameObject.GetComponent<PlacedObject>();
            if (placedObject!=null)
            {
                placedObject.DestroySelf();

                if (isBuilding)
                {
                    List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        Pathfinding.Instance.GetGrid().GetGridObject(gridPosition.x,gridPosition.y).ClearPlacedObject();
                        Pathfinding.Instance.GetGrid().GetGridObject(gridPosition.x,gridPosition.y).SetIsWalkable(true);
                    }
                }
                else
                {
                    pathNode.ClearPlacedObject();
                    pathNode.SetIsWalkable(true);
                }
                
            }
            soldierController.ClearTarget();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
