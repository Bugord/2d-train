using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimerButtonController : MonoBehaviour
    {
        
        [SerializeField] private Image _circleImage;
        [SerializeField] private Sprite _runningSptite;
        [SerializeField] private Sprite _readySprite;
        public float time { get; set; }

        private float _watch;

        public event Action TimerEnded;

        private IEnumerator _coroutine;
        
        public void StartTimer(bool isClockwise, Action<float> callBack, float watch = 0)
        {
            _watch = watch;

            if (_coroutine == null)
            {
                _coroutine = RunTimer(isClockwise, callBack);
                StartCoroutine(_coroutine);
                return;
            }
            
            StopCoroutine(_coroutine);
            _coroutine = RunTimer(isClockwise, callBack);
            StartCoroutine(_coroutine);
        }

        private IEnumerator RunTimer(bool isClockwise, Action<float> callBack = null)
        {
            _circleImage.sprite = _runningSptite;
            while (_watch <= time)
            {
                _watch += Time.deltaTime;
                _circleImage.fillAmount = isClockwise ? _watch / time : 1 - _watch / time;
                callBack?.Invoke(_watch);
                yield return null;
            }
            TimerEnded?.Invoke();
            _circleImage.fillAmount =isClockwise ? 1 : 0;
            _circleImage.sprite = _readySprite;
        }
    }
}
