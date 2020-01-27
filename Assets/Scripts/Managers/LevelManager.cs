using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class LevelManager : Singleton<LevelManager>
    {
        public int Level;
        public int StopsCount;
        public float Speed;
        public float BoostedSpeed;
        public float MaxSpeed;
        public float DefaultSpeed;
        public float Step;
        [SerializeField] private float SpeedStep;

        private void Awake()
        {
            MapGenerator.LevelUp += OnLevelUp;
            UpdateManager();
        }

        public void FixedUpdate()
        {
            if (Speed < MaxSpeed && UIManager.IsInGame)
            {
                Speed += (MaxSpeed - DefaultSpeed) * Mathf.Atan(Mathf.Lerp(0, Mathf.PI * 0.5f, Step));
                Step += SpeedStep;
            }
        }

        public void UpdateManager()
        {
            ResetSpeed();
            Level = 0;
            StopsCount = 0;
            MaxSpeed = 6;
            UpdateMaxSpeed();
        }

        public void ResetSpeed()
        {
            Speed = DefaultSpeed;
            Step = 0;
        }

        private void OnLevelUp()
        {
            Level++;

            UpdateMaxSpeed();

            if (Level == 1)
            {
                StopsCount = 1;
            }

            if (Level % 5 == 0 && StopsCount < 5)
            {
                StopsCount++;
            }
        }

        private void UpdateMaxSpeed()
        {
            MaxSpeed = Mathf.Atan(Level / 12f + 0.63f) * 5.3f + 3 + GetPlayerSkillFactor();
        }

        private float GetPlayerSkillFactor()
        {
            return Mathf.Pow(th(Level * 0.1f), 2) * (GameData.AverageLevel + CloudVariables.ImportantValues[0]/(Level+1) + Level) * Mathf.Exp(-Level * 0.05f) * 0.02f;
        }

        private float th(float x)
        {
            return (Mathf.Exp(x) - Mathf.Exp(-x)) / (Mathf.Exp(x) + Mathf.Exp(-x));
        }
    }
}