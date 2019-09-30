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

        public List<RailController> OldRaiControllers;
        public List<RailController> NewRaiControllers;
        public int CurrentRow;
        public TrainController TrainController;

        private Dictionary<int, List<RailController>> RailRowsList;

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

        private void Start()
        {
            NewRaiControllers = new List<RailController>();
            OldRaiControllers = new List<RailController> { InitialRailController };
            RailRowsList = new Dictionary<int, List<RailController>>();
            RailRowsList.Add(0, OldRaiControllers);
        }

        private void Update()
        {
            if (CurrentRow <= TrainController.TargetRail.Row + 3)
            {
                Generate();

                if (TrainController.TargetRail.Row - 3 >= 0)
                {
                    foreach (var oldRail in RailRowsList[TrainController.TargetRail.Row - 3])
                    {
                        Destroy(oldRail.gameObject);
                    }
                }
            }
        }

        public void Generate()
        {
            NewRaiControllers.Clear();
            Debug.LogError(OldRaiControllers.Count);
            foreach (var oldRail in OldRaiControllers)
            {
                int newRailsCount = Random.Range(1, RailPrefabs.Length);
                newRailsCount = 1;
                List<int> indexes = new List<int>();

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
                    Debug.LogError(index);
                    GameObject newRailPref = RailPrefabs[index];
                    var newRailController = Instantiate(newRailPref, Map).GetComponent<RailController>();
                    newRailController.Row = CurrentRow;
                    oldRail.NextActiveRail = newRailController;
                    oldRail.NextRails.Add(newRailController);
                    newRailController.transform.position = oldRail.transform.position + oldRail.WayPoints.Last();
                    NewRaiControllers.Add(newRailController);
                }
            }

            RailRowsList.Add(CurrentRow, NewRaiControllers);
            OldRaiControllers = new List<RailController>(NewRaiControllers);

            CurrentRow++;
        }
    }
}