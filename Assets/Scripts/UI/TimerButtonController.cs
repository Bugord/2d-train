using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimerButtonController : MonoBehaviour
    {
        [SerializeField] private Image _circleImage;

        public void SetTimer(float time, Action callBack)
        {
            StartCoroutine(RunTimer(time, callBack));
        }

        private IEnumerator RunTimer(float time, Action callBack)
        {
            var watch = 0f;
            
            while (watch <= time)
            {
                _circleImage.fillAmount = watch / time;
                watch += Time.deltaTime;
                yield return null;
            }

            _circleImage.fillAmount = 1;
            callBack();
        }
    }
}
