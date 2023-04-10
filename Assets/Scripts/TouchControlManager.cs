using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class TouchControlManager: MonoBehaviour
    {
        private Pathfinding _pathfinding;
        
        public static TouchControlManager Instance { get; set; }
        public bool isSoldierSelect,isSoldierMoving,isBuilding,isVisualPlaced;
        public SoldierController soldierController;
        [SerializeField] private Sprite whiteSprite;
        public LayerMask ignoreLayer;

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Start()
        {
            Instance=this;
            _pathfinding= new Pathfinding(12, 15);
            EnemyBuildingsInstantiater.Instance.BuildEnemyBuildings();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)&&  !EventSystem.current.IsPointerOverGameObject())
            {
               

                if (isBuilding)
                {
                    BuildManager.Instance.VisualPlaced();
                }
                
                else
                {
                    _clearInfoPanel();
                    _castRay();
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (isVisualPlaced)
                {
                    BuildManager.Instance.VisualFollowMouse();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isBuilding && isVisualPlaced)
                {
                    BuildManager.Instance.Build();
                }
            }

            if (Input.GetMouseButtonUp(1) )
            {
                if (isSoldierSelect&&!isSoldierMoving)
                {
                    _castRayAttack();
                }

                else
                {
                    _clearInfoPanel();
                }
            }

            
        }

        void _clearInfoPanel()
        {
            UIManager.Instance.OpenInformationParent(" ",whiteSprite,false);
        }

        private void _castRay()
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero,Mathf.Infinity,~ignoreLayer);
            if (hit&& hit.transform.gameObject.CompareTag("Soldier"))
            { 
                print("select");
                _selectSoldier(hit.transform.gameObject);
                UIManager.Instance.OpenInformationParent(hit.transform.tag,hit.transform.GetComponent<SpriteRenderer>().sprite,false);
            }

            if (hit&&hit.transform.gameObject.CompareTag("Barracks"))
            {
                print("barrack");
                UIManager.Instance.barracksController = hit.transform.gameObject.GetComponent<BarracksController>();
                UIManager.Instance.OpenInformationParent(hit.transform.tag,hit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite,true);
            }
            
            if (hit&&hit.transform.gameObject.CompareTag("PowerPlant"))
            {
                UIManager.Instance.OpenInformationParent(hit.transform.tag,hit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite,false);
            }
        }
        private void _castRayAttack()
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero,Mathf.Infinity);
            if (hit&& hit.transform.gameObject.CompareTag("Enemy"))
            { 
                print("attack");
                _moveSoldierForAttack(hit.transform.gameObject);
            }

            if (hit&& hit.transform.gameObject.CompareTag("Tile")&& isSoldierSelect)
            {
                print("move");
               _moveSoldier();
            }
        }

        private void _selectSoldier(GameObject soldier)
        {
            soldierController = soldier.GetComponent<SoldierController>();
            isSoldierSelect = true;
            isSoldierMoving = false;
        }

        private void _moveSoldier()
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
            _pathfinding.GetGrid().GetXY(mouseWorldPosition,out int x,out int y);
            soldierController.SetTargetPosition(mouseWorldPosition);
            Vector3 soldierPosition = soldierController._getPosition();
            _pathfinding.GetGrid().GetXY(soldierPosition,out int w,out int k);
            List<PathNode> pathNodes = _pathfinding.FindPath(w, k, x, y);
            if (pathNodes!=null)
            {
                for (int i = 0; i < pathNodes.Count-1; i++)
                {
                    Debug.DrawLine(new Vector3(pathNodes[i].X,pathNodes[i].Y)*10+Vector3.one*5f,new Vector3(pathNodes[i+1].X,pathNodes[i+1].Y)*10f+Vector3.one*5,Color.yellow,10);
                }
            }

            isSoldierMoving = true;
            isSoldierSelect = false;
           
        }
        private void _moveSoldierForAttack(GameObject target)
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
            _pathfinding.GetGrid().GetXY(mouseWorldPosition,out int x,out int y);
            
            soldierController.SetTargetPositionForAttack(mouseWorldPosition,target);
            Vector3 soldierPosition = soldierController._getPosition();
            _pathfinding.GetGrid().GetXY(soldierPosition,out int w,out int k);
            List<PathNode> pathNodes = _pathfinding.FindPath(w, k, x, y);
            if (pathNodes!=null)
            {
                for (int i = 0; i < pathNodes.Count-1; i++)
                {
                    Debug.DrawLine(new Vector3(pathNodes[i].X,pathNodes[i].Y)*10+Vector3.one*5f,new Vector3(pathNodes[i+1].X,pathNodes[i+1].Y)*10f+Vector3.one*5,Color.yellow,10);
                }
            }

            isSoldierMoving = true;
            isSoldierSelect = false;
        }

        private void _findAvailableGridObject(Vector3 mousePosition)
        {
            _pathfinding.GetGrid().GetXY(mousePosition,out int x,out int y);
            Vector3 soldierPosition = soldierController._getPosition();
            _pathfinding.GetGrid().GetXY(soldierPosition,out int w,out int k);
            
            
        }
    }

   
}