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

        private void LateUpdate()
        {
            if (CurrentRow <= TrainController.TargetRail.Row + 4)
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
            NewRow = new Row();
            int maxRailCountFromOutput = 3;

            foreach (var output in OldRow.Outputs)
            {
                if (output.Value.Count == 0) continue;

                var outputId = output.Key;
                var outputRail = output.Value.First();
                var outputPosition = outputRail.WayPoints.Last().position;
                
                List<RailController> prefabs = GetPrefabsList(outputId, output.Value);

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

        private List<RailController> GetPrefabsList(int outputId, List<RailController> outputRails)
        {
            List<RailController> prefabs = RailPrefabs;
            if (CurrentRow % 2 == 0)
            {
                if (outputRails.Any(rail => rail.RailDirection != RailDirection.Forward))
                {
                    prefabs = prefabs.Where(prefab =>
                        prefab.RailDirection == RailDirection.Forward).ToList();
                }

                if (outputRails.Any(rail => rail.RailDirection == RailDirection.Forward) && outputRails.Count == 1)
                {
                    prefabs = prefabs.Where(prefab =>
                        prefab.RailDirection != RailDirection.Forward).ToList();
                }

            }

            switch (outputId)
            {
                case 0:
                    prefabs = prefabs.Where(prefab =>
                        prefab.RailDirection != RailDirection.Left &&
                        prefab.RailDirection != RailDirection.RightCircle).ToList();
                    break;

                case 1:
                case 2:
                    if (OldRow.Outputs.Count > 1)
                    {
                        prefabs = prefabs.Where(prefab =>
                            prefab.RailDirection != RailDirection.LeftCircle &&
                            prefab.RailDirection != RailDirection.RightCircle).ToList();
                    }

                    break;

                case 3:
                    prefabs = prefabs.Where(prefab =>
                        prefab.RailDirection != RailDirection.Right &&
                        prefab.RailDirection != RailDirection.LeftCircle).ToList();
                    break;

                default:
                    prefabs = prefabs.ToList();
                    break;
            }

            if (CurrentRow % 2 == 0)
            {
                if (prefabs.Any(prefab => prefab.RailDirection == RailDirection.LeftCircle ||
                                          prefab.RailDirection == RailDirection.RightCircle))
                {
                    prefabs.Remove(prefabs.FirstOrDefault(prefab => prefab.RailDirection == RailDirection.Forward));
                }
            }
            else
            {
                prefabs = prefabs.Where(prefab =>
                    prefab.RailDirection != RailDirection.LeftCircle &&
                    prefab.RailDirection != RailDirection.RightCircle).ToList();
            }

            return prefabs;
        }
    }
}