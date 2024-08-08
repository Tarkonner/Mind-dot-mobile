using System;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolerBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private T _prefab;

    protected ObjectPool<T> pool;

    protected void InitPool(T prefab, int initial = 10, int max = 20, bool collectionChecks = false)
    {
        _prefab = prefab;
        pool = new ObjectPool<T>(
            CreateSetup,
            GetSetup,
            ReleaseSetup,
            DestroySetup,
            collectionChecks,
            initial,
            max);
    }

    #region Overrides
    protected virtual T CreateSetup()
    {
        T spawn = Instantiate(_prefab);
        spawn.gameObject.SetActive(false);
        return spawn;
    }
    protected virtual void GetSetup(T obj) => obj.gameObject.SetActive(true);
    protected virtual void ReleaseSetup(T obj) => obj.gameObject.SetActive(false);
    protected virtual void DestroySetup(T obj) => Destroy(obj);
    #endregion

    #region Getters
    public T Get() => pool.Get();
    public void Release(T obj)
    {
        if (obj == null)
            Debug.LogError("Empty object was trying to put in" + this.name);
        
        pool.Release(obj);
    }
    #endregion
}
