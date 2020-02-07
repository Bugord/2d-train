using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

namespace UI
{
    public class TimerButtonController : MonoBehaviour
    {
        [SerializeField] private string _playerPrefKey;
        [SerializeField] private Image _circleImage;
        [SerializeField] private Sprite _runningSptite;
        [SerializeField] private Sprite _readySprite;
        public float time;
        private DateTime startSystemTime {
            get
            {
                if (!PlayerPrefs.HasKey(_playerPrefKey))
                {
                    return DateTime.Now;
                }
                
                return DateTime.Parse(PlayerPrefs.GetString(_playerPrefKey));
            }
            set
            {
                PlayerPrefs.SetString(_playerPrefKey, value.ToString());
            }
        }
        private float _watch;

        public event Action TimerEnded;
        
        protected void SetTimer(Action<float> callBack)
        {
            _watch = 0f;
            startSystemTime = DateTime.Now;
            StartCoroutine(RunTimer(callBack));
        }

        protected void UpdateTimer(Action<float> callBack)
        {
            _watch = (float)(DateTime.Now - startSystemTime).TotalSeconds;
            StartCoroutine(RunTimer(callBack));
        }

        private IEnumerator RunTimer(Action<float> callBack)
        {
            _circleImage.sprite = _runningSptite;
            while (_watch <= time)
            {
                _watch += Time.deltaTime;
                _circleImage.fillAmount = _watch / time;
                callBack(_watch);
                yield return null;
            }
            TimerEnded?.Invoke();
            _circleImage.fillAmount = 1;
            _circleImage.sprite = _readySprite;
        }
    }
}
