using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    internal List<GameObject> objectList;

    internal GameObject baseObject;  // reference for initial BASE object state
    private Transform parentRef;    // reference for TRANSFORM of PARENT

    public void CreateObjectPool(GameObject prefab, int poolInitialAmount, Transform parent)  // Initialize Pool With Parent
    {
        if (objectList == null)
            objectList = new List<GameObject>();
        if (baseObject == null)
            baseObject = prefab;
        if (parentRef == null)
            parentRef = parent;

        for (int x = 0; x < poolInitialAmount; x++)
        {
            GameObject tempO = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
            tempO.SetActive(false);
            objectList.Add(tempO);
        }
    }
    public void CreateObjectPool(GameObject prefab, int poolInitialAmount)  // Initialize Pool Without Parent
    {
        CreateObjectPool(prefab, poolInitialAmount, null);
    }
    public void CreateObjectPool(GameObject prefab, int poolInitialAmount, bool createClumpObject, string clumpName)  // Initialize Pool With CLUMP
    {
        GameObject clump = new GameObject();
        clump.transform.position = Vector3.zero;
        clump.name = clumpName;

        CreateObjectPool(prefab, poolInitialAmount, clump.transform);
    }
    internal GameObject DoSpawn(Vector3 spawnPosition)
    {
        foreach (GameObject obj in objectList)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = spawnPosition;
                obj.SetActive(true);
                return obj;
            }
        }

        return CreateNewObjectInPool(spawnPosition);
    }
    private GameObject CreateNewObjectInPool(Vector3 spawnPos)
    {
        GameObject tempO = Object.Instantiate(baseObject, spawnPos, Quaternion.identity, parentRef);
        objectList.Add(tempO);
        return tempO;
    }
}
