using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Row
    {
        public List<RailController> Rails;
        public int MaxInput = 3;
        public int MaxOutput = 4;
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
        public GameObject SmallRail;

        public Row OldRow;
        public Row NewRow;
        public int CurrentRow;
        public TrainController TrainController;

        public static Dictionary<int, Row> _rowsList;

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
                Generate();

                if (TrainController.TargetRail.Row - 3 >= 0)
                {
                    foreach (var oldRail in _rowsList[TrainController.TargetRail.Row - 3].Rails)
                    {
                        Destroy(oldRail.gameObject);
                    }
                }
            }
        }

        public void Generate()
        {
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
                            prefab.GetComponent<RailController>().RailDirection != RailDirection.Left).ToList();
                        break;

                    case 2:
                        maxRailCountFromOutput = 4;
                        break;

                    case 3:
                        prefabs = prefabs.Where(prefab =>
                            prefab.GetComponent<RailController>().RailDirection != RailDirection.Right).ToList();
                        break;

                    default:
                        prefabs = prefabs.ToList();
                        break;
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

                    var smallRail = Instantiate(SmallRail, newRailController.transform);
                    smallRail.transform.localPosition = Vector3.zero;
                    newRailController.smallRail = smallRail.GetComponent<RailController>();

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
    }
}