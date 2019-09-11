using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;


public static class ObjectPoolUtil
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Sync<T>(this ObjectPool pool, List<T> v, int cnt)
        where T : PooledBehaviour
    {
        while(v.Count < cnt) v.Add(pool.Aquire().GetComponent<T>());
        while(v.Count > cnt)
        {
            pool.Retire(v[v.Count - 1].gameObject);
            v.RemoveAt(v.Count - 1);
        }
    }
}
