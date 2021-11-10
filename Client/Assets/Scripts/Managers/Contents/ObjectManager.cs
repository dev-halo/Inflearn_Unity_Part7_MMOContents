using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    //Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>();

    List<GameObject> objects = new List<GameObject>();

    public void Add(GameObject go)
    {
        objects.Add(go);
    }

    public void Remove(GameObject go)
    {
        objects.Remove(go);
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach (GameObject obj in objects)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (cc.CellPos == cellPos)
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        objects.Clear();
    }
}
