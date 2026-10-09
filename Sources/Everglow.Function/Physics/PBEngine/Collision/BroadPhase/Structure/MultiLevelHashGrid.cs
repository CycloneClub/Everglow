using Everglow.Commons.Physics.PBEngine.Collision;
using log4net.Core;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Physics.PBEngine.Collision.BroadPhase.Structure
{
    public struct ProfilingData
    {
        public AABB GridBox;
        public int NumObjects;
    }
    public class MultiLevelHashGrid
    {
        private class HGrid
        {
            public List<ColliderEntry> Entries;


            public HGrid()
            {
                Entries = new List<ColliderEntry>();
            }

            public List<KeyValuePair<int, int>> GetPairs()
            {
                List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();

                for (int i = 0; i < Entries.Count; i++)
                {
                    for (int j = i + 1; j < Entries.Count; j++)
                    {
                        if (Entries[i].BoundingBox.Intersects(Entries[j].BoundingBox)
                            && (Entries[i].IsDynamic || Entries[j].IsDynamic))
                        {
                            // 序号小的在前面，这样判重方便一点
                            if (Entries[i].ColliderId < Entries[j].ColliderId)
                            {
                                pairs.Add(new KeyValuePair<int, int>(Entries[i].ColliderId, Entries[j].ColliderId));
                            }
                            else
                            {
                                pairs.Add(new KeyValuePair<int, int>(Entries[j].ColliderId, Entries[i].ColliderId));
                            }
                        }
                    }
                }
                return pairs;
            }

            public List<KeyValuePair<int, int>> GetPairsWith(HGrid grid)
            {
                List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();

                for (int i = 0; i < Entries.Count; i++)
                {
                    for (int j = 0; j < grid.Entries.Count; j++)
                    {
                        if (Entries[i].BoundingBox.Intersects(grid.Entries[j].BoundingBox)
                            && (Entries[i].IsDynamic || grid.Entries[j].IsDynamic))
                        {
                            pairs.Add(new KeyValuePair<int, int>(Entries[i].ColliderId, grid.Entries[j].ColliderId));
                        }
                    }
                }
                return pairs;
            }

            public List<int> QueryRange(in AABB aabb)
            {
                List<int> result = new List<int>();
                for (int i = 0; i < Entries.Count; i++)
                {
                    if (Entries[i].BoundingBox.Intersects(aabb))
                    {
                        result.Add(Entries[i].ColliderId);
                    }
                }
                return result;
            }
        }

        private int _maxLevels;
        private int _maxOccupiedGridsPerLevel;
        private int[] _sizeThisLevel;
        private Dictionary<Point, HGrid>[] _multiGrids;
        public MultiLevelHashGrid(int maxGridsPerObjectPerLevel, int[] sizesEachLevel)
        {
            _maxLevels = sizesEachLevel.Length;
            _maxOccupiedGridsPerLevel = maxGridsPerObjectPerLevel;
            _sizeThisLevel = new int[_maxLevels + 1];
            _sizeThisLevel[0] = 0;
            for(int i = 1; i <= _maxLevels; i++)
            {
                _sizeThisLevel[i] = sizesEachLevel[i - 1];
            }
            _multiGrids = new Dictionary<Point, HGrid>[_maxLevels + 1];
            for (int i = 0; i <= _maxLevels; i++)
            {
                _multiGrids[i] = new Dictionary<Point, HGrid>();
            }
        }

        public void AddRange(List<ColliderEntry> entries)
        {
            foreach (var entry in entries)
            {
                int level = FindMatchingLevel(entry.BoundingBox);
                foreach (var grid in OccupiedRange(entry.BoundingBox, level))
                {
                    Insert(level, grid, entry);
                }
            }
        }

        public void Add(ColliderEntry entry)
        {
            int level = FindMatchingLevel(entry.BoundingBox);
            foreach (var grid in OccupiedRange(entry.BoundingBox, level))
            {
                Insert(level, grid, entry);
            }
        }

        public List<int> QueryRange(in AABB aabb)
        {
            List<int> results = new List<int>(1024);
            for (int i = 0; i <= _maxLevels; i++)
            {
                results.AddRange(QueryRange_Level(aabb, i));
            }
            return results;
        }


        private List<int> QueryRange_Level(in AABB aabb, int level)
        {
            List<int> results = new List<int>();
            if (level == 0)
            {
                if (_multiGrids[0].ContainsKey(new Point(0, 0)))
                {
                    // 和大物体的碰撞
                    foreach (var largeEntry in _multiGrids[0][new Point(0, 0)].Entries)
                    {
                        if (largeEntry.BoundingBox.Intersects(aabb))
                        {
                            results.Add(largeEntry.ColliderId);
                        }
                    }
                }
                return results;
            }

            if (aabb.GetArea() < (double)_sizeThisLevel[level] * _sizeThisLevel[level] * _multiGrids[level].Count)
            {
                foreach (var grid in OccupiedRange(aabb, level))
                {
                    if (_multiGrids[level].ContainsKey(grid))
                    {
                        var selfAABB = GetGridAABB(grid, level);
                        if (selfAABB.CompletelyInside(aabb))
                        {
                            results.AddRange(_multiGrids[level][grid].Entries.Select(x => x.ColliderId).ToList());
                        }
                        else
                        {
                            results.AddRange(_multiGrids[level][grid].QueryRange(aabb));
                        }
                    }
                }
            }
            else
            {
                foreach (var gridpair in _multiGrids[level])
                {
                    var selfAABB = GetGridAABB(gridpair.Key, level);
                    if (selfAABB.Intersects(aabb))
                    {
                        if (selfAABB.CompletelyInside(aabb))
                        {
                            results.AddRange(gridpair.Value.Entries.Select(x => x.ColliderId).ToList());
                        }
                        else
                        {
                            results.AddRange(gridpair.Value.QueryRange(aabb));
                        }
                    }
                }
            }
            return results;
        }

        public List<KeyValuePair<int, int>> QueryPairs()
        {
            List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>(1024);

            for (int i = 0; i <= _maxLevels; i++)
            {
                foreach (var pair in _multiGrids[i])
                {
                    pairs.AddRange(pair.Value.GetPairs());
                }
            }


            for (int i = 1; i <= _maxLevels; i++)
            {
                // 和上层进行相互检测
                for (int j = 0; j < i; j++)
                {
                    if (_multiGrids[j].Count == 0)
                    {
                        continue;
                    }
                    foreach (var pair in _multiGrids[i])
                    {
                        var rp = new Point(0, 0);
                        var pos = new Point(pair.Key.X * _sizeThisLevel[i], pair.Key.Y * _sizeThisLevel[i]);
                        // 获取上层中当前格子对应位置 
                        if (j != 0)
                        {
                            int X = (int)Math.Floor((float)pos.X / _sizeThisLevel[j]);
                            int Y = (int)Math.Floor((float)pos.Y / _sizeThisLevel[j]);
                            rp = new Point(X, Y);
                        }
                        if (_multiGrids[j].ContainsKey(rp))
                        {
                            pairs.AddRange(pair.Value.GetPairsWith(_multiGrids[j][rp]));
                        }
                    }
                }
            }
            return pairs.Distinct().ToList();
        }


        public List<KeyValuePair<int, int>> QueryPairsWith(MultiLevelHashGrid other)
        {

            List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>(1024);
            // 尺寸相等格子
            for (int i = 0; i <= _maxLevels; i++)
            {
                if (_multiGrids[i].Count < other._multiGrids[i].Count)
                {
                    foreach (var pair in _multiGrids[i])
                    {
                        if (other._multiGrids[i].ContainsKey(pair.Key))
                        {
                            pairs.AddRange(pair.Value.GetPairsWith(other._multiGrids[i][pair.Key]));
                        }
                    }
                }
                else
                {
                    foreach (var pair in other._multiGrids[i])
                    {
                        if (_multiGrids[i].ContainsKey(pair.Key))
                        {
                            pairs.AddRange(_multiGrids[i][pair.Key].GetPairsWith(pair.Value));
                        }
                    }
                }
            }

            // 自己尺寸比对方尺寸大
            for (int i = 0; i <= _maxLevels; i++)
            {
                for (int j = i + 1; j <= _maxLevels; j++)
                {
                    foreach (var pair_other in other._multiGrids[j])
                    {
                        Point rp = new Point(0, 0);
                        if (i != 0)
                        {
                            var pos = new Point(pair_other.Key.X * _sizeThisLevel[j], pair_other.Key.Y * _sizeThisLevel[j]);
                            // 获取上层中当前格子对应位置 
                            int X = (int)Math.Floor((float)pos.X / _sizeThisLevel[i]);
                            int Y = (int)Math.Floor((float)pos.Y / _sizeThisLevel[i]);
                            rp = new Point(X, Y);
                        }
                        if (_multiGrids[i].ContainsKey(rp))
                        {
                            pairs.AddRange(_multiGrids[i][rp].GetPairsWith(pair_other.Value));
                        }
                    }
                }
            }

            // 自己尺寸比对方尺寸小
            for (int i = 0; i <= _maxLevels; i++)
            {
                for (int j = i + 1; j <= _maxLevels; j++)
                {
                    foreach (var pair_self in _multiGrids[j])
                    {
                        Point rp = new Point(0, 0);
                        if (i != 0)
                        {
                            var pos = new Point(pair_self.Key.X * _sizeThisLevel[j], pair_self.Key.Y * _sizeThisLevel[j]);
                            // 获取上层中当前格子对应位置 
                            int X = (int)Math.Floor((float)pos.X / _sizeThisLevel[i]);
                            int Y = (int)Math.Floor((float)pos.Y / _sizeThisLevel[i]);
                            rp = new Point(X, Y);
                        }
                        if (other._multiGrids[i].ContainsKey(rp))
                        {
                            pairs.AddRange(pair_self.Value.GetPairsWith(other._multiGrids[i][rp]));
                        }
                    }
                }
            }
            return pairs.Distinct().ToList();
        }

        private int FindMatchingLevel(in AABB aabb)
        {
            int ans = _maxLevels;
            int l = 0, r = _maxLevels;
            while (l <= r)
            {
                int mid = (l + r) >> 1;
                if (CanFitInThisLevel(aabb, mid))
                {
                    l = mid + 1;
                    ans = mid;
                }
                else
                {
                    r = mid - 1;
                }
            }
            return ans;
        }

        private void Insert(int level, Point pos, ColliderEntry entry)
        {
            if (!_multiGrids[level].ContainsKey(pos))
            {
                _multiGrids[level].Add(pos, new HGrid());
            }

            _multiGrids[level][pos].Entries.Add(entry);
        }

        private List<Point> OccupiedRange(in AABB aabb, int level)
        {
            if (level == 0)
            {
                return new List<Point>()
                {
                    new Point(0, 0),
                };
            }
            int minX = (int)Math.Floor(aabb.MinPoint.X / _sizeThisLevel[level]);
            int maxX = (int)Math.Floor(aabb.MaxPoint.X / _sizeThisLevel[level]);
            int minY = (int)Math.Floor(aabb.MinPoint.Y / _sizeThisLevel[level]);
            int maxY = (int)Math.Floor(aabb.MaxPoint.Y / _sizeThisLevel[level]);

            List<Point> points = new List<Point>();
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    points.Add(new Point(x, y));
                }
            }
            return points;
        }

        private bool CanFitInThisLevel(in AABB aabb, int level)
        {
            if (level == 0)
            {
                return true;
            }
            int minX = (int)Math.Floor(aabb.MinPoint.X / _sizeThisLevel[level]);
            int maxX = (int)Math.Floor(aabb.MaxPoint.X / _sizeThisLevel[level]);
            int minY = (int)Math.Floor(aabb.MinPoint.Y / _sizeThisLevel[level]);
            int maxY = (int)Math.Floor(aabb.MaxPoint.Y / _sizeThisLevel[level]);
            return Math.Floor((aabb.MaxPoint.X - aabb.MinPoint.X) / _sizeThisLevel[level]) + 1 <= _maxOccupiedGridsPerLevel 
                && Math.Floor((aabb.MaxPoint.Y - aabb.MinPoint.Y) / _sizeThisLevel[level]) + 1 <= _maxOccupiedGridsPerLevel 
                && (maxX - minX + 1) * (maxY - minY + 1) <= _maxOccupiedGridsPerLevel;
        }

        private AABB GetGridAABB(Point p, int level)
        {
            if (level == 0)
            {
                // 0层无限大的AABB
                return new AABB()
                {
                    MinPoint = new Vector2(float.NegativeInfinity, float.NegativeInfinity),
                    MaxPoint = new Vector2(float.PositiveInfinity, float.PositiveInfinity)
                };
            }
            int sz = _sizeThisLevel[level];
            return new AABB()
            {
                MinPoint = new Vector2(p.X * sz, p.Y * sz),
                MaxPoint = new Vector2(p.X * sz + sz, p.Y * sz + sz),
            };
        }

        public List<ProfilingData> GetProfilingData()
        {
            List<ProfilingData> result = new List<ProfilingData>();
            for (int i = 1; i <= _maxLevels; i++)
            {
                foreach (var pair in _multiGrids[i])
                {
                    result.Add(new ProfilingData()
                    {
                        GridBox = GetGridAABB(pair.Key, i),
                        NumObjects = pair.Value.Entries.Count
                    });
                }
            }
            return result;
        }
    }
}
