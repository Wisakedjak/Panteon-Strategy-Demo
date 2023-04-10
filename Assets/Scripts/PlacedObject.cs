using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour {

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO) {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.Setup(placedObjectTypeSO, origin, dir,placedObjectTransform);

        return placedObject;
    }




    private PlacedObjectTypeSO _placedObjectTypeSO;
    private Vector2Int _origin;
    private PlacedObjectTypeSO.Dir _dir;
    private Transform _placedObjectTransform;

    private void Setup(PlacedObjectTypeSO placedObjectTypeSO, Vector2Int origin, PlacedObjectTypeSO.Dir dir, Transform placedObjectTransform) {
        _placedObjectTypeSO = placedObjectTypeSO;
        _origin = origin;
        _dir = dir;
        _placedObjectTransform = placedObjectTransform;
    }

    public List<Vector2Int> GetGridPositionList() {
        return _placedObjectTypeSO.GetGridPositionList(_origin, _dir);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public GameObject GetPlacedObjectTransform()
    {
        return _placedObjectTransform.gameObject;
    }

    

}