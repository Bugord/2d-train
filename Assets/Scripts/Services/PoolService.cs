using Assets.Scripts.ObjectsPool;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class PoolService
    {
        private PoolPart[] pools;
        private GameObject objectsParent;

        [System.Serializable]
        public struct PoolPart
        {
            public string name;
            public PoolObject prefab;
            public int count;
            public ObjectPooling ferula;
        }

        public PoolService(PoolPart[] newPools)
        {
            pools = newPools;
            objectsParent = new GameObject();
            objectsParent.name = "Pool";
            for (int i = 0; i < pools.Length; i++)
            {
                if (pools[i].prefab != null)
                {
                    pools[i].ferula = new ObjectPooling();
                    pools[i].ferula.Initialize(pools[i].count, pools[i].prefab, objectsParent.transform);
                }
            }
        }


        public T GetObject<T>(string name, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion()) where T : PoolObject
        {
            T result = null;
            if (pools != null)
            {
                for (int i = 0; i < pools.Length; i++)
                {
                    if (string.Compare(pools[i].name, name) == 0)
                    {
                        result = (T)pools[i].ferula.GetObject();
                        result.transform.position = position;
                        result.transform.rotation = rotation;
                        result.gameObject.SetActive(true);
                        return result;
                    }
                }
            }
            return result;
        }
    }
}
