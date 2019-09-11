using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PooledBehaviour : MonoBehaviour
{
    public abstract void SetAquire();
    public abstract void SetRetire();
}

public class ObjectPool : MonoBehaviour
{
    public GameObject source;

    readonly HashSet<GameObject> idles = new HashSet<GameObject>();
    readonly HashSet<GameObject> lives = new HashSet<GameObject>();

    void Start()
    {
        foreach (var c in transform.GetComponentsInChildren<PooledBehaviour>(true))
        {
            if (source != c.gameObject) idles.Add(c.gameObject);
        }
    }

    public GameObject Aquire()
    {
        GameObject res = null;
        if (idles.Count == 0) res = Instantiate(source, this.transform);
        else
        {
            res = idles.First();
            idles.Remove(res);
        }
        
        res.GetComponent<PooledBehaviour>().SetAquire();
        lives.Add(res);
        return res;
    }

    public void Retire(GameObject x)
    {
        if (!lives.Contains(x)) throw new Exception("Object is not owned by this pool.");
        x.transform.SetParent(this.transform);
        x.GetComponent<PooledBehaviour>().SetRetire();
        lives.Remove(x);
        idles.Add(x);
        return;
    }
}
