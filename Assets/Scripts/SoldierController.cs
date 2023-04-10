using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class SoldierController : MonoBehaviour
{
    public List<Vector3> pathVectorList=null;
    private int _currentPathIndex;
    public float speed;
    public bool startMoving;
    private int _soldierCount;
    private GameObject _target;
    [SerializeField] private TextMeshProUGUI soldierCountText;
    [SerializeField] private int damage, healthPoint;

    public void SetTargetPosition(Vector3 targetPosition)
    {
        
        pathVectorList = Pathfinding.Instance.FindPath(_getPosition(), targetPosition);
        if (pathVectorList!=null&& pathVectorList.Count>1)
        {
            pathVectorList.RemoveAt(0);
            
           
        }

        startMoving = true;
        TouchControlManager.Instance.isSoldierMoving = true;
    }
    
    public void SetTargetPositionForAttack(Vector3 targetPosition,GameObject target)
    {
        _target = target;
        pathVectorList= Pathfinding.Instance.FindPathForAttack(_getPosition(),targetPosition);
        

       
        if (pathVectorList!=null&& pathVectorList.Count>1)
        {
            pathVectorList.RemoveAt(0);
           
           
        }
        startMoving = true;
      
    }

    void _handleMovement()
    {
        if (pathVectorList!=null)
        {
            Vector3 targetPosition = pathVectorList[_currentPathIndex];
            if (Vector3.Distance(transform.position,targetPosition)>.5f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }
            else
            {
                _currentPathIndex++;
                if (_currentPathIndex>=pathVectorList.Count)
                {
                    _stopMoving();
                }
            }
        }
    }

    void _stopMoving()
    {
        pathVectorList = null;
        _currentPathIndex = 0;
        startMoving = false;
        TouchControlManager.Instance.isSoldierMoving = false;
        transform.position = transform.position - new Vector3(0, 0, 5);
        if (_target!=null)
        {
            print(_target.transform);
            _attack();
        }
    }

    void _attack()
    {
        _target.GetComponent<EnemyController>()._reduceHealth(this.gameObject,damage*_soldierCount);
    }

    public void ReduceHealth(int damageTaken)
    {
        healthPoint -= damageTaken;
        if (healthPoint>0)
        {
            _attack();
        }

        if (healthPoint<=0)
        {
            Pathfinding.Instance.GetGrid().GetXY(transform.position, out int x, out int y);
            PathNode pathNode = Pathfinding.Instance.GetGrid().GetGridObject(x, y);
            PlacedObject placedObject = pathNode.GetPlacedObject();
            if (placedObject!=null)
            {
                placedObject.DestroySelf();

                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    Pathfinding.Instance.GetGrid().GetGridObject(gridPosition.x,gridPosition.y).ClearPlacedObject();
                }
            }
        }
    }

    public void Attack()
    {
        _attack();
    }


    public void ClearTarget()
    {
        _target = null;
    }
    public Vector3 _getPosition()
    {
        return transform.position;
    }

    private void Update()
    {
        if (startMoving)
        {
            _handleMovement();
        }
        
    }

    public void IncreaseSoldierCount(int value)
    {
        _soldierCount+=value;
        soldierCountText.text = _soldierCount.ToString();
    }
}
