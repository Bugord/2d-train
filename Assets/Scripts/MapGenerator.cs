﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                        new Vector3(0, -2, 0),
                        new Vector3(0, 2, 0)
                    }
            },
            {
                RailDirection.Left, new List<Vector3>()
                {
                    Vector3.zero,
                    Vector3.left
                }
            },
            {
                RailDirection.Right, new List<Vector3>()
                {
                    Vector3.zero,
                    Vector3.right
                }
            },
        };

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
            //NewRaiControllers.Clear();
            //RailsIndexes.Clear();

            //foreach (var index in OldRaiControllers.Keys)
            //{
            //    if (index > TrainController.TargetRail.index + 9 || index < TrainController.TargetRail.index - 9)
            //        continue;

            //    if (OldRaiControllers[index].TurnsLeft)
            //        RailsIndexes.Add(index - 1);
            //    if (OldRaiControllers[index].TurnsMiddle)
            //        RailsIndexes.Add(index);
            //    if (OldRaiControllers[index].TurnsRight)
            //        RailsIndexes.Add(index + 1);
            //}

            //RailsIndexes.Sort();
            //RailsIndexes = RailsIndexes.Distinct().ToList();
            //foreach (var railsIndex in RailsIndexes)
            //{
            //    int i = 0;
            //    var valid = false;
            //    GameObject newRailPref = RailPrefabs[Random.Range(0, RailPrefabs.Length)];
            //    while (!valid)
            //    {
            //        i++;
            //        newRailPref = RailPrefabs[Random.Range(0, RailPrefabs.Length)];
            //        if (RailsIndexes[0] == railsIndex)
            //        {
            //            valid = true;
            //            break;
            //        }
            //        if (RailsIndexes.Contains(railsIndex - 1))
            //        {
            //            var newController = newRailPref.GetComponent<RailController>();
            //            if (NewRaiControllers[railsIndex - 1].TurnsRight != newController.TurnsLeft)
            //                valid = true;
            //        }
            //        else
            //        {
            //            valid = true;
            //        }
            //        if (i > 100)
            //        {
            //            Debug.LogError("Generation error");
            //            break;
            //        }
            //    }

            //    var newRail = Instantiate(newRailPref, Map);
            //    var newSmallRail = Instantiate(SmallRail, newRail.transform);
            //    newRail.transform.position = new Vector3(railsIndex * 1.175f, CurrentRow * 4.2f) + Map.position;
            //    newSmallRail.transform.position =
            //        new Vector3(railsIndex * 1.175f, CurrentRow * 4.2f - 2.1f) + Map.position;
            //    var newRailController = newRail.GetComponent<RailController>();
            //    newRailController.TypeRail = Random.Range(0, 3);
            //    newRailController.Row = CurrentRow;
            //    newRailController.index = railsIndex;
            //    newRailController.UpdateRailSprite();
            //    NewRaiControllers.Add(railsIndex, newRailController);
            //}

            //foreach (var oldRailController in OldRaiControllers)
            //{
            //    if (oldRailController.Key > TrainController.TargetRail.index + 8 ||
            //        oldRailController.Key < TrainController.TargetRail.index - 8)
            //        continue;
            //    if (oldRailController.Value.TurnsLeft)
            //        oldRailController.Value.LeftRailControllers = NewRaiControllers[oldRailController.Key - 1];
            //    if (oldRailController.Value.TurnsMiddle)
            //        oldRailController.Value.MiddleRailControllers = NewRaiControllers[oldRailController.Key];
            //    if (oldRailController.Value.TurnsRight)
            //        oldRailController.Value.RightRailControllers = NewRaiControllers[oldRailController.Key + 1];

            //    oldRailController.Value.Enable();
            //}
            //RailRowsList.Add(CurrentRow, OldRaiControllers);
            //OldRaiControllers = new Dictionary<int, RailController>(NewRaiControllers);

            //CurrentRow++;
        }
    }
}