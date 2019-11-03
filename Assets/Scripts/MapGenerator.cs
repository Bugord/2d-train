using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Row
    {
        public List<RailController> Rails;
        public Dictionary<int, List<RailController>> Outputs;

        public Row()
        {
            Rails = new List<RailController>();
            Outputs = new Dictionary<int, List<RailController>>();
        }

        public Row(Row row)
        {
            Rails = row.Rails;
            Outputs = row.Outputs;
        }
    }

    public class MapGenerator : MonoBehaviour
    {
        public RailController InitialRailController;

        public GameObject[] RailPrefabs;
        public Transform Map;
       
        [SerializeField] private GameObject _point;
        [SerializeField] private GameObject _stop;

        public Row OldRow;
        public Row NewRow;
        public int CurrentRow;
        public TrainController TrainController;

        public static Dictionary<int, Row> _rowsList;
        
        private void Start()
        {
            NewRow = new Row();
            OldRow = new Row();
            OldRow.Rails.Add(InitialRailController);
            OldRow.Outputs.Add(InitialRailController.OutputId, new List<RailController> { InitialRailController });

            _rowsList = new Dictionary<int, Row>();
            _rowsList.Add(0, OldRow);
        }

        private void Update()
        {
            if (CurrentRow <= TrainController.TargetRail.Row + 3)
            {
                GenerateRails();

                if (TrainController.TargetRail.Row - 3 >= 0)
                {
                    foreach (var oldRail in _rowsList[TrainController.TargetRail.Row - 3].Rails)
                    {
                        Destroy(oldRail.gameObject);
                    }
                }
            }
        }

        public void GenerateRails()
        {
            //GenerateObjects();
            NewRow = new Row();
            int maxRailCountFromOutput = 3;

            foreach (var output in OldRow.Outputs)
            {
                if (output.Value.Count == 0) continue;

                var outputId = output.Key;
                var outputRail = output.Value.First();
                var outputPosition = outputRail.transform.position + outputRail.WayPoints.Last();

                List<GameObject> prefabs = RailPrefabs.ToList();
                if (CurrentRow % 2 == 0)
                {
                    if (output.Value.Any(rail => rail.RailDirection != RailDirection.Forward))
                    {
                        prefabs = RailPrefabs.Where(prefab =>
                            prefab.GetComponent<RailController>().RailDirection == RailDirection.Forward).ToList();
                    }

                    if (output.Value.Any(rail => rail.RailDirection == RailDirection.Forward) && output.Value.Count == 1)
                    {
                        prefabs = RailPrefabs.Where(prefab =>
                            prefab.GetComponent<RailController>().RailDirection != RailDirection.Forward).ToList();
                    }
                    
                }

                switch (outputId)
                {
                    case 0:
                        prefabs = prefabs.Where(prefab =>
                            prefab.GetComponent<RailController>().RailDirection != RailDirection.Left && 
                            prefab.GetComponent<RailController>().RailDirection != RailDirection.RightCircle).ToList();
                        break;

                    case 1:
                    case 2:
                        if (OldRow.Outputs.Count > 1)
                        {
                            prefabs = prefabs.Where(prefab =>
                                prefab.GetComponent<RailController>().RailDirection != RailDirection.LeftCircle &&
                                prefab.GetComponent<RailController>().RailDirection != RailDirection.RightCircle).ToList();
                        }

                        maxRailCountFromOutput = 4;
                        break;

                    case 3:
                        prefabs = prefabs.Where(prefab =>
                            prefab.GetComponent<RailController>().RailDirection != RailDirection.Right &&
                            prefab.GetComponent<RailController>().RailDirection != RailDirection.LeftCircle).ToList();
                        break;

                    default:
                        prefabs = prefabs.ToList();
                        break;
                }

                if (CurrentRow % 2 == 0)
                {
                    if (prefabs.Any(prefab => prefab.GetComponent<RailController>().RailDirection == RailDirection.LeftCircle ||
                                              prefab.GetComponent<RailController>().RailDirection == RailDirection.RightCircle))
                    {
                        prefabs.Remove(prefabs.FirstOrDefault(prefab => prefab.GetComponent<RailController>().RailDirection == RailDirection.Forward));
                    }
                }
                else
                {
                    prefabs = prefabs.Where(prefab =>
                        prefab.GetComponent<RailController>().RailDirection != RailDirection.LeftCircle &&
                        prefab.GetComponent<RailController>().RailDirection != RailDirection.RightCircle).ToList();
                }

                int newRailsCount = Random.Range(1, maxRailCountFromOutput);

                List<int> indexes = new List<int>();

                for (int i = 0; i < newRailsCount; i++)
                {
                    int index = Random.Range(0, prefabs.Count);
                    int check = 0;
                    while (check <= 100)
                    {
                        if (!indexes.Contains(index))
                        {
                            indexes.Add(index);
                            break;
                        }
                        index = Random.Range(0, prefabs.Count);
                        check++;
                    }
                    
                    GameObject newRailPref = prefabs[index];

                    var newRailController = Instantiate(newRailPref, Map).GetComponent<RailController>();
                    newRailController.Row = CurrentRow;
                    output.Value.ForEach(rail =>
                    {
                        rail.NextActiveRail = newRailController;
                        rail.NextRails.Add(newRailController);
                    });
                    newRailController.InputId = outputId;
                    switch (newRailController.RailDirection)
                    {
                        case RailDirection.Left:
                            newRailController.OutputId = outputId - 1;
                            break;

                        case RailDirection.Forward:
                        case RailDirection.RightCircle:
                        case RailDirection.LeftCircle:
                            newRailController.OutputId = outputId;
                            break;

                        case RailDirection.Right:
                            newRailController.OutputId = outputId + 1;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    newRailController.transform.position = outputPosition;
                    NewRow.Rails.Add(newRailController);
                    
                    if (NewRow.Outputs.ContainsKey(newRailController.OutputId))
                    {
                        NewRow.Outputs[newRailController.OutputId].Add(newRailController);
                    }
                    else
                    {
                        NewRow.Outputs.Add(newRailController.OutputId, new List<RailController> { newRailController });
                    }
                }
            }
            _rowsList.Add(CurrentRow, NewRow);
            OldRow = new Row(NewRow);

            CurrentRow++;
        }
        bool hasStop = true;
        private void GenerateObjects()
        {
            if (OldRow.Outputs.Count == 1)
            {
                hasStop = true;
            }
            foreach (var rails in OldRow.Outputs.Values)
            {
                var rail = rails.First();
                var point = Instantiate(_point, rail.transform);
                point.transform.localPosition = rail.WayPoints.Last();
                //if (!hasStop)
                //{
                //    var stop = Instantiate(_stop, rail.transform);
                //    stop.transform.localPosition = rail.WayPoints.Last();
                //    stop.transform.localRotation = Quaternion.Euler(0, 0, 90);
                //    hasStop = true;
                //}
                //else
                //{
                //    var point = Instantiate(_point, rail.transform);
                //    point.transform.localPosition = rail.WayPoints.Last();
                //    hasStop = false;
                    
                //}
            }
        }
    }
}