using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PooledObjectControlTest : MonoBehaviour
{
    public ObjectPool pool;
    public float dt;
    public float maxCount;

    [Header("Debug Info")]
    [SerializeField] float t = 0;
    [SerializeField] int dir = 1;
    [SerializeField] int cc;
    List<GameObject> lst = new List<GameObject>();
    
    void Update()
    {
        cc = lst.Count;
        
        t += Time.deltaTime;
        if (lst.Count >= maxCount) dir = -1;
        if (lst.Count == 0) dir = 1;

        if (dir == 1)
        {
            while (t >= dt)
            {
                t -= dt;
                var x = pool.Aquire();
                lst.Add(x);
                x.transform.position = new Vector2(UnityEngine.Random.Range(0, 1000), UnityEngine.Random.Range(0, 500));
            }
        }
        else if (dir == -1)
        {
            while (t >= dt && lst.Count != 0)
            {
                t -= dt;
                pool.Retire(lst[lst.Count - 1]);
                lst.RemoveAt(lst.Count - 1);
            }
        }
    }

}
