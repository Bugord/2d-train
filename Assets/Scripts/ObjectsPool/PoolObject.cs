using System.Collections;
using UnityEngine;

namespace Assets.Scripts.ObjectsPool
{
    public class PoolObject : MonoBehaviour
    {
        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }

        public void ReturnToPool(float delay)
        {
            StartCoroutine(WaitForSeconds(delay));
        }

        private IEnumerator WaitForSeconds(float time)
        {
            for (float t = 0; t < time; t+= Time.deltaTime)
            {
                yield return null;
            }
            ReturnToPool();
        }
    }
}
