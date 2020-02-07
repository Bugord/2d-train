using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

namespace UI
{
    public class TimerButtonController : MonoBehaviour
    {
        [SerializeField] private Image _circleImage;
        [SerializeField] private Sprite _runningSptite;
        [SerializeField] private Sprite _readySprite;
        public float time;
        private DateTime startSystemTime {
            get
            {
                if (!PlayerPrefs.HasKey("FreeCoinsTimer"))
                {
                    return DateTime.Now;
                }
                
                return DateTime.Parse(PlayerPrefs.GetString("FreeCoinsTimer"));
            }
            set
            {
                PlayerPrefs.SetString("FreeCoinsTimer", value.ToString());
            }
        }
        private float watch;

        public event Action TimerEnded;
        
        protected void SetTimer(Action<float> callBack)
        {
            watch = 0f;
            startSystemTime = DateTime.Now;
            StartCoroutine(RunTimer(callBack));
        }

        protected void UpdateTimer(Action<float> callBack)
        {
            watch = (float)(DateTime.Now - startSystemTime).TotalSeconds;
            StartCoroutine(RunTimer(callBack));
        }

        private IEnumerator RunTimer(Action<float> callBack)
        {
            _circleImage.sprite = _runningSptite;
            while (watch <= time)
            {
                watch += Time.deltaTime;
                _circleImage.fillAmount = watch / time;
                callBack(watch);
                yield return null;
            }
            TimerEnded?.Invoke();
            _circleImage.sprite = _readySprite;
        }
    }
}
