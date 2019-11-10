using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

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

        public List<RailController> RailPrefabs;
        private Dictionary<RailDirection, RailController> _railPrefabsDictionary;

        public Transform Map;
       
        [SerializeField] private GameObject _point;
        [SerializeField] private GameObject _stop;

        public Row OldRow;
        public Row NewRow;
        public int CurrentRow;
        public TrainController TrainController;

        public static Dictionary<int, Row> _rowsList;

        public int RowsBefore;
        public int RowsAfter;

        private void Awake()
        {
            _railPrefabsDictionary = new Dictionary<RailDirection, RailController>();
            foreach (var prefab in RailPrefabs)
            {
                _railPrefabsDictionary.Add(prefab.RailDirection, prefab);
            }
        }

        private void Start()
        {
            NewRow = new Row();
            OldRow = new Row();
            OldRow.Rails.Add(InitialRailController);
            OldRow.Outputs.Add(InitialRailController.OutputId, new List<RailController> { InitialRailController });

            _rowsList = new Dictionary<int, Row>();
            _rowsList.Add(0, OldRow);
        }

        private void LateUpdate()
        {
            if (CurrentRow <= TrainController.TargetRail.Row + RowsAfter)
            {
                GenerateRails();

                if (TrainController.TargetRail.Row - RowsBefore >= 0)
                {
                    foreach (var oldRail in _rowsList[TrainController.TargetRail.Row - RowsBefore].Rails)
                    {
                        Destroy(oldRail.gameObject);
                    }
                }
            }

            if (CurrentRow == RowsAfter - 1)
            {
                InitialRailController.SwitchRail();
            }
        }

        public void GenerateRails()
        {
            NewRow = new Row();
            int maxRailCountFromOutput = 3;

            var enabledPrefabs = GetRailPrefabs(OldRow.Outputs);

            foreach (var output in OldRow.Outputs)
            {
                if (output.Value.Count == 0) continue;

                var outputId = output.Key;
                var outputRail = output.Value.First();
                var outputPosition = outputRail.WayPoints.Last().position;
                
                List<RailController> prefabs = enabledPrefabs[outputId];

                int newRailsCount = Random.Range(1, maxRailCountFromOutput+1);

                List<int> indexes = GetPrefabsIndexes(newRailsCount, prefabs.Count);

                foreach (var newRailController in indexes.Select(index => Instantiate(prefabs[index], Map)))
                {
                    newRailController.Row = CurrentRow;
                    output.Value.ForEach(rail =>
                    {
                        rail.NextActiveRail = newRailController;
                        rail.NextRails.Add(newRailController);
                    });
                    newRailController.InputId = outputId;
                    newRailController.transform.position = outputPosition;

                    if (indexes.Count > 1)
                    {
                        newRailController._spriteMask.sprite = newRailController._splitMask;
                    }

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
            GenerateItems();
            _rowsList.Add(CurrentRow, NewRow);
            OldRow = new Row(NewRow);

            CurrentRow++;
        }
        
        private List<int> GetPrefabsIndexes(int newRailsCount, int prefabsCount)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < newRailsCount; i++)
            {
                int index = Random.Range(0, prefabsCount);
                int check = 0;
                while (check <= 100)
                {
                    if (!indexes.Contains(index))
                    {
                        indexes.Add(index);
                        break;
                    }
                    index = Random.Range(0, prefabsCount);
                    check++;
                }
            }

            return indexes;
        }

        private Dictionary<int, List<RailController>> GetRailPrefabs(Dictionary<int, List<RailController>> outputs)
        {
            Dictionary<int, List<RailController>> prefabs = new Dictionary<int, List<RailController>>();

            foreach (var output in outputs)
            {
                var outputId = output.Key;
                var outputRails = output.Value;

                List<RailController> newRails = new List<RailController>();
                
                switch (outputId)
                {
                    case 0:
                        if (outputRails.Count == 1)
                        {
                            newRails.Add(CurrentRow % 2 == 0
                                ? _railPrefabsDictionary[RailDirection.Forward]
                                : _railPrefabsDictionary[RailDirection.LeftCircle]);

                            newRails.Add(_railPrefabsDictionary[RailDirection.Right]);
                        }
                        if (outputRails.Count > 1)
                        {
                            newRails.Add(CurrentRow % 2 == 0
                                ? _railPrefabsDictionary[RailDirection.Forward]
                                : _railPrefabsDictionary[RailDirection.LeftCircle]);
                            if (outputRails.Any(rail => rail.RailDirection == RailDirection.Left))
                            {
                                newRails.Add(_railPrefabsDictionary[RailDirection.Right]);
                            }
                        }
                        break;
                    case 1:
                    case 2:
                        if (outputRails.Count == 1)
                        {
                            newRails.Add(_railPrefabsDictionary[RailDirection.Forward]);
                            if (prefabs.ContainsKey(outputId - 1))
                            {
                                newRails.Add(
                                    prefabs[outputId - 1].Any(rail => rail.RailDirection == RailDirection.Right)
                                        ? _railPrefabsDictionary[RailDirection.Right]
                                        : _railPrefabsDictionary[RailDirection.Left]);
                            }
                            else
                            {
                                newRails.Add(_railPrefabsDictionary[RailDirection.Left]);
                                newRails.Add(_railPrefabsDictionary[RailDirection.Right]);
                            }
                        }
                        if (outputRails.Count > 1)
                        {
                            newRails.Add(_railPrefabsDictionary[RailDirection.Forward]);
                            if (outputRails.Any(rail => rail.RailDirection == RailDirection.Left))
                            {
                                newRails.Add(CurrentRow % 2 == 0
                                    ? _railPrefabsDictionary[RailDirection.Left]
                                    : _railPrefabsDictionary[RailDirection.Right]);
                            }
                            if (outputRails.Any(rail => rail.RailDirection == RailDirection.Right))
                            {
                                newRails.Add(CurrentRow % 2 == 0
                                    ? _railPrefabsDictionary[RailDirection.Right]
                                    : _railPrefabsDictionary[RailDirection.Left]);
                            }
                        }
                        break;
                    case 3:
                        if (outputRails.Count == 1)
                        {
                            newRails.Add(CurrentRow % 2 != 0
                                ? _railPrefabsDictionary[RailDirection.Forward]
                                : _railPrefabsDictionary[RailDirection.RightCircle]);

                            newRails.Add(_railPrefabsDictionary[RailDirection.Left]);
                        }
                        if (outputRails.Count > 1)
                        {
                            newRails.Add(CurrentRow % 2 != 0
                                ? _railPrefabsDictionary[RailDirection.Forward]
                                : _railPrefabsDictionary[RailDirection.RightCircle]);

                            if (outputRails.Any(rail => rail.RailDirection == RailDirection.Right))
                            {
                                newRails.Add(_railPrefabsDictionary[RailDirection.Left]);
                            }
                        }
                        break;
                }

                prefabs.Add(outputId, newRails);
            }

            return prefabs;
        }

        private void GenerateItems()
        {
            foreach (var output in OldRow.Outputs)
            {
                var outputRails = output.Value;
                if (outputRails.Any(rail => rail.IsActive))
                {
                    outputRails.ForEach(rail =>
                    {
                        if (!rail.IsActive)
                        {
                            var point = Instantiate(_point, rail.transform);
                            point.transform.localPosition = Vector3.zero;
                        }
                        else
                        {
                            if (CurrentRow > 5 && CurrentRow % 2 == 0)
                            {
                                var previousRow = _rowsList[rail.Row-1];

                                var center = previousRow.Rails.Where(rowRail => rowRail.InputId == rail.InputId).Any(rowRail => rowRail.InputId == rowRail.OutputId + 1 ||
                                                                                                                                rowRail.InputId == rowRail.OutputId - 1);

                                var left = previousRow.Rails.Where(rowRail => rowRail.InputId == rail.InputId - 1).Any(rowRail => rowRail.InputId == rowRail.OutputId ||
                                                                                                                                  rowRail.InputId == rowRail.OutputId + 1);

                                var right = previousRow.Rails.Where(rowRail => rowRail.InputId == rail.InputId + 1).Any(rowRail => rowRail.InputId == rowRail.OutputId ||
                                                                                                                                   rowRail.InputId == rowRail.OutputId - 1);

                                if (center && (left || right))
                                {
                                    var stop = Instantiate(_stop, rail.transform);
                                    stop.transform.localPosition = Vector3.zero;
                                }
                            }
                        }
                    });
                }
            }
        }
    }
}