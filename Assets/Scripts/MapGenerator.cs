﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Assets.Scripts.Controllers;
using Assets.Scripts.Enums;
using Assets.Scripts.ObjectsPool;
using Assets.Scripts.Services;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Quaternion = UnityEngine.Quaternion;
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

    public class MapGenerator : Singleton<MapGenerator>
    {
        public RailController InitialRailController;

        public List<RailController> RailPrefabs;
        private Dictionary<RailDirection, RailController> _railPrefabsDictionary;
        
        [SerializeField] private GameObject _point;
        [SerializeField] private GameObject _stop;
        [SerializeField] private GameObject _boost;

        private bool canBoost = false;

        public Row OldRow;
        public Row NewRow;
        public int CurrentRow;
        public int DeltaRow;
        public HeadTrainController TrainController;

        public static Dictionary<int, Row> _rowsList;

        public int RowsBefore;
        public int RowsAfter;

        public static event Action LevelUp;

        private PoolService _poolService;
        private LevelService _levelService;
        private UIService _uiService;

        private bool _hasFirstStop;
        public List<Row> TutorialRows = new List<Row>();
        
        private void Awake()
        {
            _poolService = ServiceLocator.GetService<PoolService>();
            _levelService = ServiceLocator.GetService<LevelService>();
            _uiService = ServiceLocator.GetService<UIService>();
            _railPrefabsDictionary = new Dictionary<RailDirection, RailController>();
            foreach (var prefab in RailPrefabs)
            {
                _railPrefabsDictionary.Add(prefab.RailDirection, prefab);
            }

            DeltaRow = 0;
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
                        if (oldRail != null)
                        {
                            oldRail.ReturnToPool();
                        }
                    }
                }
            }

            if (CurrentRow == RowsAfter && !_uiService.IsFirstTime)
            {
                InitialRailController.SwitchRail();
            }
        }

        public void ResetGenerator()
        {
            DeltaRow = TrainController.TargetRail.Row;
        }

        public void GenerateRails()
        {
            NewRow = new Row();
            int maxRailCountFromOutput = CurrentRow <= 4 ? 1 : 3;

            var enabledPrefabs = GetRailPrefabs(OldRow.Outputs);

            foreach (var output in OldRow.Outputs)
            {
                if (output.Value.OutputRails.Count == 0) continue;

                var outputId = output.Key;
                var outputRail = output.Value.OutputRails.First();
                var outputPosition = outputRail.EndPoint.position;

                List<RailController> prefabs = enabledPrefabs[outputId];

                int newRailsCount = 1;
                if (_uiService.IsFirstTime)
                {
                    if (_hasFirstStop)
                    {
                        newRailsCount = Random.Range(1, maxRailCountFromOutput + 1);
                    }
                    else
                    {
                        switch (CurrentRow)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 6:
                            case 9:
                                newRailsCount = 1;
                                break;
                            case 4:
                            case 5:
                            case 7:
                            case 8:
                                newRailsCount = 2;
                                break;
                            case 10:
                                newRailsCount = 3;
                                break;
                        }
                    }
                }
                else
                {
                    if (CurrentRow >= 4)
                        newRailsCount = Random.Range(1, maxRailCountFromOutput + 1);
                }

                List<int> indexes = GetPrefabsIndexes(newRailsCount, prefabs.Count);

                foreach (var newRailController in indexes.Select(index => 
                    _poolService.GetObject<RailController>(prefabs[index].name, outputPosition + Vector3.up * 0.01f, Quaternion.identity)))
                {
                    newRailController.NextRails.Clear();
                    newRailController.NextActiveRail = null;
                    newRailController.Row = CurrentRow;

                    output.Value.OutputRails.ForEach(rail =>
                    {
                        rail.NextActiveRail = newRailController;
                        rail.NextRails.Add(newRailController);
                    });
                    newRailController.InputId = outputId;
                    
                    if (indexes.Count > 1)
                    {
                        newRailController._spriteMask.sprite = newRailController._splitMask;
                    }

                    switch (newRailController.RailDirection)
                    {
                        case RailDirection.Left:
                            newRailController.OutputId = outputId - 1;
                            break;

                        case RailDirection.Strait:
                        case RailDirection.CircleRight:
                        case RailDirection.CircleLeft:
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

            if (CurrentRow < 11 && _uiService.IsFirstTime)
            {
                for (int i = 0; i < prefabsCount; i++)
                {
                    indexes.Add(i);
                }

                return indexes;
            }

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

                if (_uiService.IsFirstTime)
                {
                    switch (CurrentRow)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 6:
                        case 9:
                            newRails.Add(_railPrefabsDictionary[RailDirection.Strait]);
                            prefabs.Add(1, newRails);
                            return prefabs;
                        case 4:
                            newRails.Add(_railPrefabsDictionary[RailDirection.Right]);
                            newRails.Add(_railPrefabsDictionary[RailDirection.Strait]);
                            prefabs.Add(1, newRails);
                            return prefabs;
                        case 5:
                            newRails.Add(_railPrefabsDictionary[RailDirection.Strait]);
                            prefabs.Add(1, newRails);
                            newRails = new List<RailController>();
                            newRails.Add(_railPrefabsDictionary[RailDirection.Left]);
                            prefabs.Add(2, newRails);
                            return prefabs;
                        case 7:
                            newRails.Add(_railPrefabsDictionary[RailDirection.Strait]);
                            newRails.Add(_railPrefabsDictionary[RailDirection.Left]);
                            prefabs.Add(1, newRails);
                            return prefabs;
                        case 8:
                            newRails.Add(_railPrefabsDictionary[RailDirection.Strait]);
                            prefabs.Add(1, newRails);
                            newRails = new List<RailController>();
                            newRails.Add(_railPrefabsDictionary[RailDirection.Right]);
                            prefabs.Add(0, newRails);
                            return prefabs;
                        case 10:
                            newRails.Add(_railPrefabsDictionary[RailDirection.Strait]);
                            newRails.Add(_railPrefabsDictionary[RailDirection.Left]);
                            newRails.Add(_railPrefabsDictionary[RailDirection.Right]);
                            prefabs.Add(1, newRails);
                            return prefabs;
                    }
                }

                switch (outputId)
                {
                    case 0:
                        if (outputRails.Count == 1)
                        {
                            newRails.Add(CurrentRow % 5 != 0
                                ? _railPrefabsDictionary[RailDirection.Strait]
                                : _railPrefabsDictionary[RailDirection.CircleLeft]);

                            newRails.Add(_railPrefabsDictionary[RailDirection.Right]);
                        }
                        if (outputRails.Count > 1)
                        {
                            newRails.Add(CurrentRow % 3 != 0
                                ? _railPrefabsDictionary[RailDirection.Strait]
                                : _railPrefabsDictionary[RailDirection.CircleLeft]);
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
                            newRails.Add(_railPrefabsDictionary[RailDirection.Strait]);
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
                            newRails.Add(_railPrefabsDictionary[RailDirection.Strait]);
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
                                ? _railPrefabsDictionary[RailDirection.Strait]
                                : _railPrefabsDictionary[RailDirection.CircleRight]);

                            newRails.Add(_railPrefabsDictionary[RailDirection.Left]);
                        }
                        if (outputRails.Count > 1)
                        {
                            newRails.Add(CurrentRow % 6 != 0
                                ? _railPrefabsDictionary[RailDirection.Strait]
                                : _railPrefabsDictionary[RailDirection.CircleRight]);

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
            if (!_hasFirstStop && _uiService.IsFirstTime)
            {
                RailController rail;
                float stopOffset = 0;
                switch (CurrentRow)
                {
                    case 1:
                    case 2:
                    case 3:
                        rail = NewRow.Rails.FirstOrDefault();
                        if (rail != null)
                            foreach (var pos in rail.PointPositions)
                            {
                                _poolService.GetObject<PoolObject>(_point.name, pos.position, Quaternion.identity);
                            }

                        NewRow.Outputs[rail.OutputId].HasObject = true;
                        return;
                    case 4:
                        rail = NewRow.Rails.LastOrDefault();
                        if (rail != null)
                            for (var j = 0; j < 2; j++)
                            {
                                _poolService.GetObject<PoolObject>(_stop.name, rail.EndPoint.position + Vector3.down * stopOffset, _stop.transform.rotation);
                                stopOffset += 0.25f;
                            }
                        NewRow.Outputs[rail.OutputId].HasObject = true;
                        TutorialRows.Add(NewRow);
                        return;
                    case 5:
                    case 6:
                    case 8:
                    case 9:
                        return;
                    case 7:
                        rail = NewRow.Rails.FirstOrDefault();
                        if (rail != null)
                            for (var j = 0; j < 2; j++)
                            {
                                _poolService.GetObject<PoolObject>(_stop.name, rail.EndPoint.position + Vector3.down * stopOffset, _stop.transform.rotation);
                                stopOffset += 0.25f;
                            }
                        NewRow.Outputs[rail.OutputId].HasObject = true;
                        TutorialRows.Add(NewRow);
                        return;
                    case 10:
                        rail = NewRow.Rails.FirstOrDefault();
                        if (rail != null)
                            for (var j = 0; j < 2; j++)
                            {
                                _poolService.GetObject<PoolObject>(_stop.name, rail.EndPoint.position + Vector3.down * stopOffset, _stop.transform.rotation);
                                stopOffset += 0.25f;
                            }
                        NewRow.Outputs[rail.OutputId].HasObject = true;
                        stopOffset = 0;
                        rail = NewRow.Rails[1];
                        if (rail != null)
                            for (var j = 0; j < 2; j++)
                            {
                                _poolService.GetObject<PoolObject>(_stop.name, rail.EndPoint.position + Vector3.down * stopOffset, _stop.transform.rotation);
                                stopOffset += 0.25f;
                            }
                        NewRow.Outputs[rail.OutputId].HasObject = true;
                        TutorialRows.Add(NewRow);
                        _hasFirstStop = true;
                        return;
                }
            }
            
            if (CurrentRow - DeltaRow <= 3)
            {
                var rail = NewRow.Rails.FirstOrDefault();
                if (rail != null)
                    foreach (var pos in rail.PointPositions)
                    {
                        _poolService.GetObject<PoolObject>(_point.name, pos.position, Quaternion.identity);
                    }
                return;
            }

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

                        if (stopRail.RailDirection == RailDirection.CircleLeft || stopRail.RailDirection == RailDirection.CircleRight)
                        {
                            if (circleConfig == CircleRailConfig.Stop || circleConfig == CircleRailConfig.StopWithPoints)
                            {
                                CircleRailsConfig[circleConfig] = true;
                                print(circleConfig);
                                for (int j = 0; j < Random.Range(_levelService.MinStopsCount, _levelService.MaxStopsCount+1); j++)
                                {
                                    _poolService.GetObject<PoolObject>(_stop.name, stopRail.EndPoint.position + Vector3.down * stopOffset, _stop.transform.rotation);
                                    stopOffset += 0.25f;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < Random.Range(_levelService.MinStopsCount, _levelService.MaxStopsCount+1); j++)
                            {
                                _poolService.GetObject<PoolObject>(_stop.name, stopRail.EndPoint.position + Vector3.down * stopOffset, _stop.transform.rotation);
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
                        _poolService.GetObject<PoolObject>(_boost.name, rail.PointPositions.First().position, Quaternion.identity);
                        output.HasObject = true;
                    }
                    else
                    {
                        if (CurrentRow % 2 == 0 && rail.OutputId % 2 == 0)
                        {
                            foreach (var pos in rail.PointPositions)
                            {
                                _poolService.GetObject<PoolObject>(_point.name, pos.position, Quaternion.identity);
                            }
                        }

                        if (CurrentRow % 2 != 0 && rail.OutputId % 2 != 0)
                        {
                            foreach (var pos in rail.PointPositions)
                            {
                                _poolService.GetObject<PoolObject>(_point.name, pos.position, Quaternion.identity);
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
                        if (r.RailDirection == RailDirection.CircleLeft || r.RailDirection == RailDirection.CircleRight)
                        {
                            print(circleConfig);
                            CircleRailsConfig[circleConfig] = true;
                            foreach (var pos in r.PointPositions)
                            {
                                _poolService.GetObject<PoolObject>(_point.name, pos.position, Quaternion.identity);
                            }
                        }
                    }
                }

                if (circleConfig == CircleRailConfig.Clear)
                {
                    foreach (var r in outputRails)
                    {
                        if (r.RailDirection == RailDirection.CircleLeft || r.RailDirection == RailDirection.CircleRight)
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