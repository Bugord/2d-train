using Assets.Scripts.ObjectsPool;
using Assets.Scripts.Services;
using UnityEngine;

namespace Controllers
{
    public class BoostStarController : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Stop"))
            {
                PoolObject poolObject = col.GetComponent<PoolObject>();
                poolObject.ReturnToPool();
            }
        }
    }
}
