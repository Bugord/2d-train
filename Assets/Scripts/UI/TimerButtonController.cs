using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimerButtonController : MonoBehaviour
    {
        [SerializeField] private Image _circleImage;

        public void SetTimer(float time)
        {
            StartCoroutine(RunTimer(time));
        }

        private IEnumerator RunTimer(float time)
        {
            var watch = 0f;
            
            while (watch <= time)
            {
                _circleImage.fillAmount = watch / time;
                watch += Time.deltaTime;
                yield return null;
            }

            _circleImage.fillAmount = 1;
        }
    }
}
