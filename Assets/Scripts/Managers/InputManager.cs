using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class InputManager : MonoBehaviour
    {
        public static event Action<SwipeDirection> Swipe;
        private bool swiping = false;
        private bool eventSent = false;
        private Vector2 lastPosition;

        public static event Action BackButton;

        private UIService _uiService;

        private void Awake()
        {
            _uiService = ServiceLocator.GetService<UIService>();
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                BackButton?.Invoke();
            }
#if !UNITY_EDITOR
            if (Input.touchCount == 0 && !_uiService.IsInGame)
                return;

            var touch = Input.GetTouch(0);

            if (Mathf.Abs(touch.deltaPosition.sqrMagnitude) > 0)
            {
                if (swiping == false)
                {
                    swiping = true;
                    lastPosition = touch.position;
                    return;
                }

                if (!eventSent)
                {
                    if (Swipe != null && touch.phase == TouchPhase.Moved)
                    {
                        Vector2 direction = touch.position - lastPosition;

                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                        {
                            if (direction.x > 0)
                                Swipe(SwipeDirection.Right);
                            else
                                Swipe(SwipeDirection.Left);
                        }
                        else
                        {
                            if (direction.y > 0)
                                Swipe(SwipeDirection.Up);
                            else
                                Swipe(SwipeDirection.Down);
                        }

                        eventSent = true;
                    }
                }
            }
            else
            {
                swiping = false;
                eventSent = false;
            }
#endif
        }
    }
}
