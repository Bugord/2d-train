using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.ObjectsPool;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    [ExecuteInEditMode]
    public class RailController : PoolObject
    {
        public RailDirection RailDirection;
        public List<Transform> WayPoints;

        public int Row;
        public int InputId;
        public int OutputId;

        public SpriteRenderer _spriteRenderer;
        public SpriteMask _spriteMask;

        [SerializeField] private Sprite _fatSprite;
        [SerializeField] private Sprite _thinSprite;
        public Sprite _splitMask;

        public List<RailController> NextRails;
        public RailController NextActiveRail;

        public Transform EndPoint;

        public List<Transform> PointPositions;

        public bool IsActive;

        private UIService _uiService;
        
        private void OnEnable()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteMask = GetComponent<SpriteMask>();
        }

        private void Awake()
        {
            _uiService = ServiceLocator.GetService<UIService>();
            NextRails = new List<RailController>();
        }
        
        public void SwitchRail()
        {
            if (NextRails.Count == 0) return;
            NextRails = NextRails.OrderBy(rail => rail.OutputId).ToList();

            var currentWayIndex = NextRails.FindIndex(way => way == NextActiveRail);
            currentWayIndex++;
            if (currentWayIndex >= NextRails.Count)
                currentWayIndex = 0;
            NextActiveRail = NextRails[currentWayIndex];

            UpdateRailSprite();
            NextActiveRail.SwitchRail();
        }

        public void SwitchRail(SwipeDirection direction)
        {
            if (NextRails.Count == 0) return;
            NextRails = NextRails.OrderBy(rail => rail.OutputId).ToList();

            var currentWayIndex = NextRails.FindIndex(way => way == NextActiveRail);
            if (direction == SwipeDirection.Left)
            {
                currentWayIndex--;
            }

            if (direction == SwipeDirection.Right)
            {
                currentWayIndex++;
            }

            if (currentWayIndex >= NextRails.Count)
                currentWayIndex = NextRails.Count - 1;

            if (currentWayIndex < 0)
                currentWayIndex = 0;

            NextActiveRail = NextRails[currentWayIndex];

            UpdateRailSprite();
            if (_uiService.IsFirstTime)
            {
                NextActiveRail.SwitchRail();
            }
            else
            {
                NextActiveRail.SwitchRail(direction);
            }
        }

        public void UpdateRailSprite()
        {
            if (NextActiveRail == null) return;

            var rails = MapGenerator._rowsList[NextActiveRail.Row].Rails;
            foreach (var rail in rails)
            {
                if (rail == NextActiveRail)
                {
                    rail._spriteRenderer.sprite = rail._fatSprite;
                    rail._spriteRenderer.color = Color.white;
                    rail._spriteRenderer.sortingOrder = 2;
                    rail._spriteMask.enabled = true;
                    rail.IsActive = true;
                }
                else
                {
                    rail._spriteRenderer.sprite = rail._thinSprite;
                    float h, s, v;
                    Color.RGBToHSV(Camera.main.backgroundColor, out h, out s, out v);
                    rail._spriteRenderer.color = Color.HSVToRGB(h, s +0.1f, v - 0.45f);
                    rail._spriteRenderer.sortingOrder = 0;
                    rail._spriteMask.enabled = false;
                    rail.IsActive = false;
                }
            }
        }
    }
}