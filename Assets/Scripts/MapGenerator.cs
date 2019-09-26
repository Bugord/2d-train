using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MapGenerator : MonoBehaviour
    {
        public RailController InitialRailController;

        public GameObject[] RailPrefabs;
        public Transform Map;
        public GameObject SmallRail;

        public Dictionary<int, RailController> OldRaiControllers;
        public Dictionary<int, RailController> NewRaiControllers;
        public List<int> RailsIndexes;
        public int CurrentRow;
        public TrainController TrainController;

        private Dictionary<int, Dictionary<int, RailController>> RailRowsList;

        public static Dictionary<RailDirection, List<Vector3>> WayPoints = new Dictionary<RailDirection, List<Vector3>>()
        {
            {
                RailDirection.Forward, new List<Vector3>()
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(0, 4, 0)
                    }
            },
            {
                RailDirection.Left, new List<Vector3>()
                {
                    new Vector3(0, 0, 0),
                    new Vector3(-0.08f, 1.15f, 0),
                    new Vector3(-0.5f, 2.09f, 0),
                    new Vector3(-1.09f, 3.12f, 0),
                    new Vector3(-1.18f, 4.22f, 0),
                }
            },
            {
                RailDirection.Right, new List<Vector3>()
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0.08f, 1.15f, 0),
                    new Vector3(0.5f, 2.09f, 0),
                    new Vector3(1.09f, 3.12f, 0),
                    new Vector3(1.18f, 4.22f, 0),
                }
            },
        };

        private int lastRailIndex = 0;

        private void Start()
        {
            //OldRaiControllers = new Dictionary<int, RailController>();
            NewRaiControllers = new Dictionary<int, RailController>();
            OldRaiControllers = new Dictionary<int, RailController> { { 0, InitialRailController } };
            RailRowsList = new Dictionary<int, Dictionary<int, RailController>>();
            RailRowsList.Add(0, OldRaiControllers);
        }

        private void Update()
        {
            if (CurrentRow <= TrainController.TargetRail.Row + 3)
            {
                Generate();
                if (TrainController.TargetRail.Row - 3 >= 0)
                    foreach (var oldRail in RailRowsList[TrainController.TargetRail.Row - 3].Values)
                    {
                        Destroy(oldRail.gameObject);
                    }
            }
        }

        public void Generate()
        {
            NewRaiControllers.Clear();
            RailsIndexes.Clear();

            foreach (var index in OldRaiControllers.Keys)
            {
                if (index > TrainController.TargetRail.index + 9 || index < TrainController.TargetRail.index - 9)
                    continue;
                RailsIndexes.Add(index);
            }

            RailsIndexes.Sort();
            RailsIndexes = RailsIndexes.Distinct().ToList();

            foreach (var railsIndex in RailsIndexes)
            {
                int newRailsCount = Random.Range(1, RailPrefabs.Length);
                newRailsCount = 1;
                Debug.LogError("railIndex: " + railsIndex);
                List<int> indexes = new List<int>();
                var oldRail = OldRaiControllers[railsIndex];
                for (int i = 0; i < newRailsCount; i++)
                {
                    int index = Random.Range(0, RailPrefabs.Length);
                    while (true)
                    {
                        if (!indexes.Contains(index))
                        {
                            indexes.Add(index);
                            break;
                        }
                        index = Random.Range(0, RailPrefabs.Length);
                    }

                    GameObject newRailPref = RailPrefabs[index];
                    lastRailIndex++;
                    var newRailController = Instantiate(newRailPref, Map).GetComponent<RailController>();
                    newRailController.Row = CurrentRow;
                    newRailController.index = railsIndex;
                    //newRailController.UpdateRailSprite();
                    NewRaiControllers.Add(railsIndex, newRailController);
                    oldRail.NextActiveRail = newRailController;
                    newRailController.transform.position = oldRail.transform.position + oldRail.WayPoints.Last();
                    lastRailIndex++;
                }
            }

            RailRowsList.Add(CurrentRow, OldRaiControllers);
            OldRaiControllers = new Dictionary<int, RailController>(NewRaiControllers);

            CurrentRow++;
        }
    }
}