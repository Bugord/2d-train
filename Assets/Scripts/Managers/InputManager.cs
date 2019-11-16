using System;
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
        
        void Update()
        {
            if (Input.touchCount == 0)
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
        }
    }
}
