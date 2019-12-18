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
        public Dictionary<int, Output> Outputs;

        public Row()
        {
            Rails = new List<RailController>();
            Outputs = new Dictionary<int, Output>();
        }

        public Row(Row row)
        {
            Rails = row.Rails;
            Outputs = row.Outputs;
        }
    }

    public class Output
    {
        public List<RailController> OutputRails;
        public bool HasObject;
    }

    public class MapGenerator : MonoBehaviour
    {
        public RailController InitialRailController;

        public List<RailController> RailPrefabs;
        private Dictionary<RailDirection, RailController> _railPrefabsDictionary;

        public Transform Map;

        [SerializeField] private GameObject _point;
        [SerializeField] private GameObject _stop;
        [SerializeField] private GameObject _boost;

        private bool canBoost = false;

        public Row OldRow;
        public Row NewRow;
        public int CurrentRow;
        public TrainController TrainController;

        public static Dictionary<int, Row> _rowsList;

        public int RowsBefore;
        public int RowsAfter;

        public static event Action LevelUp;

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
            OldRow.Outputs.Add(InitialRailController.OutputId, new Output { OutputRails = new List<RailController> { InitialRailController } });

            _rowsList = new Dictionary<int, Row>();
            _rowsList.Add(0, OldRow);
        }

        private void LateUpdate()
        {
            if (TrainController.Trains.Count % 4 == 0)
            {
                RowsBefore = (int)(TrainController.Trains.Count * 0.25f) + 3;
            }

            if (CurrentRow <= TrainController.TargetRail.Row + RowsAfter)
            {
                if (CurrentRow % 10 == 0)
                {
                    LevelUp?.Invoke();
                }
                GenerateRails();

                if (TrainController.TargetRail.Row - RowsBefore >= 0)
                {
                    foreach (var oldRail in _rowsList[TrainController.TargetRail.Row - RowsBefore].Rails)
                    {
                        if (oldRail != null) Destroy(oldRail.gameObject);
                    }
                }
            }

            if (CurrentRow == RowsAfter)
            {
                InitialRailController.SwitchRail(false);
            }
        }

        public void GenerateRails()
        {
            NewRow = new Row();
            int maxRailCountFromOutput = CurrentRow <= 3 ? 1 : 3;

            var enabledPrefabs = GetRailPrefabs(OldRow.Outputs);

            foreach (var output in OldRow.Outputs)
            {
                if (output.Value.OutputRails.Count == 0) continue;

                var outputId = output.Key;
                var outputRail = output.Value.OutputRails.First();
                var outputPosition = outputRail.EndPoint.position;

                List<RailController> prefabs = enabledPrefabs[outputId];

                int newRailsCount = Random.Range(1, maxRailCountFromOutput + 1);

                List<int> indexes = GetPrefabsIndexes(newRailsCount, prefabs.Count);

                foreach (var newRailController in indexes.Select(index => Instantiate(prefabs[index], Map)))
                {
                    newRailController.Row = CurrentRow;
                    output.Value.OutputRails.ForEach(rail =>
                    {
                        rail.NextActiveRail = newRailController;
                        rail.NextRails.Add(newRailController);
                    });
                    newRailController.InputId = outputId;
                    newRailController.transform.position = outputPosition + Vector3.up * 0.01f;

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
                        NewRow.Outputs[newRailController.OutputId].OutputRails.Add(newRailController);
                    }
                    else
                    {
                        NewRow.Outputs.Add(newRailController.OutputId, new Output { OutputRails = new List<RailController> { newRailController } });
                    }
                }
            }

            CircleRailConfig circleConfig;
            if (CircleRailsConfig.ToList().Any(kv => !kv.Value))
            {
                circleConfig = GetRandomCircleConfig();
            }
            else
            {
                UpdateCircleRailsConfigDictionary();
                circleConfig = GetRandomCircleConfig();
            }

            GenerateItems(circleConfig);
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

        private Dictionary<int, List<RailController>> GetRailPrefabs(Dictionary<int, Output> outputs)
        {
            Dictionary<int, List<RailController>> prefabs = new Dictionary<int, List<RailController>>();

            foreach (var output in outputs)
            {
                var outputId = output.Key;
                var outputRails = output.Value.OutputRails;

                List<RailController> newRails = new List<RailController>();

                switch (outputId)
                {
                    case 0:
                        if (outputRails.Count == 1)
                        {
                            newRails.Add(CurrentRow % 5 != 0
                                ? _railPrefabsDictionary[RailDirection.Forward]
                                : _railPrefabsDictionary[RailDirection.LeftCircle]);

                            newRails.Add(_railPrefabsDictionary[RailDirection.Right]);
                        }
                        if (outputRails.Count > 1)
                        {
                            newRails.Add(CurrentRow % 3 != 0
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
                            newRails.Add(CurrentRow % 4 != 0
                                ? _railPrefabsDictionary[RailDirection.Forward]
                                : _railPrefabsDictionary[RailDirection.RightCircle]);

                            newRails.Add(_railPrefabsDictionary[RailDirection.Left]);
                        }
                        if (outputRails.Count > 1)
                        {
                            newRails.Add(CurrentRow % 6 != 0
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

        private Dictionary<CircleRailConfig, bool> CircleRailsConfig = new Dictionary<CircleRailConfig, bool>();

        private void UpdateCircleRailsConfigDictionary()
        {
            CircleRailsConfig.Clear();
            CircleRailsConfig.Add(CircleRailConfig.Points, false);
            CircleRailsConfig.Add(CircleRailConfig.Stop, false);
            CircleRailsConfig.Add(CircleRailConfig.StopWithPoints, false);
            CircleRailsConfig.Add(CircleRailConfig.Clear, false);
        }

        private CircleRailConfig GetRandomCircleConfig()
        {
            System.Random rand = new System.Random();
            return CircleRailsConfig.ToList().Where(kv => !kv.Value).OrderBy(kv => rand.Next()).First().Key;
        }

        private void GenerateItems(CircleRailConfig circleConfig)
        {
            if (CurrentRow <= 3)
            {
                var rail = NewRow.Rails.FirstOrDefault();
                if (rail != null)
                    foreach (var pos in rail.PointPositions)
                    {
                        var point = Instantiate(_point, rail.transform);
                        point.transform.localPosition = pos.localPosition;
                    }
            }

            if (CurrentRow < 5) return;

            var outputs = NewRow.Outputs.OrderBy(o => o.Key).ToList();

            if (CurrentRow % 2 == 0)
            {
                outputs.RemoveAt(0);
                outputs.Reverse();
            }

            if (CurrentRow % 40 == 0)
            {
                canBoost = true;
            }

            for (int i = 0; i < outputs.Count; i++)
            {
                var keyValuePair = outputs[i];
                var previousOutput = i == 0 ? new Output { HasObject = false } : outputs[i - 1].Value;
                var outputId = keyValuePair.Key;
                var output = keyValuePair.Value;
                var outputRails = keyValuePair.Value.OutputRails;

                var ok = outputRails.TrueForAll(rail => NewRow.Rails.Count(r => r.InputId == rail.InputId) > 1 && !previousOutput.HasObject);

                if (NewRow.Rails.Count(rail => rail.InputId == outputId) > 1 && ok)
                {
                    var check = outputRails.TrueForAll(rail =>
                        NewRow.Rails.Where(r => r.InputId == rail.InputId).ToList()
                            .TrueForAll(r => Mathf.Abs(r.OutputId - rail.OutputId) <= 1));

                    var stopRail = outputRails.FirstOrDefault();
                    if (stopRail != null && check && !TrainController.IsBoosted)
                    {
                        output.HasObject = true;
                        float stopOffset = 0;

                        if (stopRail.RailDirection == RailDirection.LeftCircle || stopRail.RailDirection == RailDirection.RightCircle)
                        {
                            if (circleConfig == CircleRailConfig.Stop || circleConfig == CircleRailConfig.StopWithPoints)
                            {
                                CircleRailsConfig[circleConfig] = true;
                                print(circleConfig);
                                for (int j = 0; j < Random.Range(1, LevelManager.Instance.StopsCount); j++)
                                {
                                    var stop = Instantiate(_stop, stopRail.transform);
                                    stop.transform.localPosition = stopRail.EndPoint.localPosition + Vector3.down * stopOffset;
                                    stopOffset += 0.25f;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < Random.Range(1, LevelManager.Instance.StopsCount); j++)
                            {
                                var stop = Instantiate(_stop, stopRail.transform);
                                stop.transform.localPosition = stopRail.EndPoint.localPosition + Vector3.down * stopOffset;
                                stopOffset += 0.25f;
                            }
                        }
                    }
                }
                else
                {
                    var rail = outputRails.FirstOrDefault();
                    if (rail == null) continue;
                    if (canBoost)
                    {
                        canBoost = false;
                        var point = Instantiate(_boost, rail.transform);
                        point.transform.localPosition = rail.PointPositions.First().localPosition;
                    }
                    else
                    {
                        if (CurrentRow % 2 == 0 && rail.OutputId % 2 == 0)
                        {
                            foreach (var pos in rail.PointPositions)
                            {
                                var point = Instantiate(_point, rail.transform);
                                point.transform.localPosition = pos.localPosition;
                            }
                        }

                        if (CurrentRow % 2 != 0 && rail.OutputId % 2 != 0)
                        {
                            foreach (var pos in rail.PointPositions)
                            {
                                var point = Instantiate(_point, rail.transform);
                                point.transform.localPosition = pos.localPosition;
                            }
                        }
                    }
                }
            }

            outputs = NewRow.Outputs.OrderBy(o => o.Key).ToList();
            foreach (var keyValuePair in outputs)
            {
                var output = keyValuePair.Value;
                var outputRails = output.OutputRails;

                if ((!output.HasObject && circleConfig == CircleRailConfig.Points) || circleConfig == CircleRailConfig.StopWithPoints)
                {
                    foreach (var r in outputRails)
                    {
                        if (r.RailDirection == RailDirection.LeftCircle || r.RailDirection == RailDirection.RightCircle)
                        {
                            print(circleConfig);
                            CircleRailsConfig[circleConfig] = true;
                            foreach (var pos in r.PointPositions)
                            {
                                var point = Instantiate(_point, r.transform);
                                point.transform.localPosition = pos.localPosition;
                            }
                        }
                    }
                }

                if (circleConfig == CircleRailConfig.Clear)
                {
                    foreach (var r in outputRails)
                    {
                        if (r.RailDirection == RailDirection.LeftCircle || r.RailDirection == RailDirection.RightCircle)
                        {
                            print(circleConfig);
                            CircleRailsConfig[circleConfig] = true;
                        }
                    }
                }
            }
        }
    }
}