using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpawnPool : MonoBehaviour {

    Dictionary<GameObject, List<GameObject>> objs;
    Dictionary<GameObject, GameObject> spawnedObjs;//go -> prefab dict.
    public void Awake()
    {
        objs = new Dictionary<GameObject, List<GameObject>>();
        spawnedObjs = new Dictionary<GameObject, GameObject>();
    }

    GameObject InstantiateObj(GameObject prefab)
    {
        GameObject go = Instantiate(prefab) as GameObject;
        go.name = prefab.name;
        go.transform.parent = this.transform;
        go.SetActive(false);
        return go;
    }

    public void Preload(GameObject prefab, int num)
    {
        if (!objs.ContainsKey(prefab))
        {
            objs.Add(prefab, new List<GameObject>());
        }

        int addNum = num - objs[prefab].Count;
        for (int i = 0; i < num; i++)
        {
            GameObject go = InstantiateObj(prefab);
            objs[prefab].Add(go);
        }
    }
    public void Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if(!objs.ContainsKey(prefab))
        {
            objs.Add(prefab, new List<GameObject>());
        }
        List<GameObject> list = objs[prefab];
        GameObject go = null;
        if (list.Count > 0 && !list[list.Count - 1].activeSelf)
        {
            go = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }
        else
        {
            go = InstantiateObj(prefab);
        }
        go.transform.position = position;
        go.transform.rotation = rotation;
        list.Insert(0, go);

        spawnedObjs.Add(go, prefab);
        go.SetActive(true);

        go.SendMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);

    }
    public void Despawn(GameObject go)
    {
        if(!spawnedObjs.ContainsKey(go))
        {
            Debug.LogError("Despawn Error. can't find object : " + go.name);
            return;
        }

        GameObject prefab = spawnedObjs[go];
        List<GameObject> list = objs[prefab];

        spawnedObjs.Remove(go);
        list.Remove(go);
        list.Add(go);


        go.SendMessage("OnDespawned",SendMessageOptions.DontRequireReceiver);
        go.SetActive(false);
        
    }

    





}
