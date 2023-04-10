using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Mathematics;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{

    public List<GameObject> gridObjectList = new List<GameObject>();
    [SerializeField]private List<GameObject> gridObjects = new List<GameObject>();
    public static GridManager Instance { get; set; }
    void Start()
    {
        Instance = this;
    }

    public void CreateTile(int value)
    {
        GameObject tiles = new GameObject();
        tiles.transform.name = "Tiles";
        for (int i = 0; i < value; i++)
        {
            
            GameObject tile = Instantiate(gridObjects[i%2], Vector2.zero, quaternion.identity);
            gridObjectList.Add(tile);
            tile.SetActive(false);
            tile.transform.parent=tiles.transform;
        }
    }
   
    


    void Update()
    {
       
    }
}
