//using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Everglow.Commons.Physics.PBEngine.Collision.BroadPhase
//{
//    internal class SweepAndPrune : IBroadPhase
//    {
//        private struct IntervalPoint
//        {
//            public float V;
//            public bool IsEnd;
//            public int ColliderId;
//            public bool IsDynamic;
//        }

//        private class GroupInfo
//        {
//            public List<IntervalPoint> IntervalPointsX;
//            public List<IntervalPoint> IntervalPointsY;

//            public GroupInfo()
//            {
//                IntervalPointsX = new List<IntervalPoint>();
//                IntervalPointsY = new List<IntervalPoint>();
//            }
//        }

//        private List<Collider2D> _colliders;
//        private Dictionary<string, GroupInfo> _groups;
//        public SweepAndPrune(CollisionGraph graph) : base(graph)
//        {
//            _colliders = new List<Collider2D>();
//            _groups = new Dictionary<string, GroupInfo>();
//        }
//        private bool IsDynamic(int id)
//        {
//            return _colliders[id].ParentObject.RigidBody.MovementType == MovementType.Dynamic
//                        || _colliders[id].ParentObject.RigidBody.MovementType == MovementType.Player;
//        }

//        public override void Prepare(List<PhysicsObject> objects, float deltaTime)
//        {
//            _colliders.Clear();
//            _colliders.EnsureCapacity(objects.Count);
//            _groups.Clear();
//            for (int i = 0; i < objects.Count; i++)
//            {
//                var obj = objects[i];
//                _colliders.Add(obj.Collider);
//                var box = obj.Collider.GetAABB(deltaTime);
//                bool isDynamic = obj.RigidBody.MovementType == MovementType.Dynamic
//                    || obj.RigidBody.MovementType == MovementType.Player;

//                if (!_groups.ContainsKey(obj.Tag))
//                {
//                    _groups.Add(obj.Tag, new GroupInfo());
//                }
//                _groups[obj.Tag].IntervalPointsX.Add(new IntervalPoint() { ColliderId = i, IsEnd = false, V = box.MinPoint.X, IsDynamic = isDynamic });
//                _groups[obj.Tag].IntervalPointsX.Add(new IntervalPoint() { ColliderId = i, IsEnd = true, V = box.MaxPoint.X, IsDynamic = isDynamic });
//                _groups[obj.Tag].IntervalPointsY.Add(new IntervalPoint() { ColliderId = i, IsEnd = false, V = box.MinPoint.Y, IsDynamic = isDynamic });
//                _groups[obj.Tag].IntervalPointsY.Add(new IntervalPoint() { ColliderId = i, IsEnd = true, V = box.MaxPoint.Y, IsDynamic = isDynamic });
//            }

//            Comparer<IntervalPoint> compFunc = Comparer<IntervalPoint>.Create((a, b) =>
//            {
//                int cmp1 = a.V.CompareTo(b.V);
//                if (cmp1 == 0)
//                {
//                    if (a.IsEnd && !b.IsEnd)
//                    {
//                        return 1;
//                    }
//                    else if (!a.IsEnd && b.IsEnd)
//                    {
//                        return -1;
//                    }
//                    return 0;
//                }
//                return cmp1;
//            });
//            foreach (var group in _groups)
//            {
//                group.Value.IntervalPointsX.Sort(compFunc);
//                group.Value.IntervalPointsY.Sort(compFunc);
//            }
//        }

//        public override List<KeyValuePair<Collider2D, Collider2D>> GetCollisionPairs(float deltaTime)
//        {
//            List<KeyValuePair<Collider2D, Collider2D>> finalPairs = new List<KeyValuePair<Collider2D, Collider2D>>();
//            foreach (var group in _groups)
//            {
//                GroupInnerDetection(group.Key, finalPairs);
//                if (_collisionGraph.Graph.ContainsKey(group.Key))
//                {
//                    foreach (var dual in _collisionGraph.Graph[group.Key])
//                    {
//                        if (!_groups.ContainsKey(dual))
//                            continue;
//                        GroupOuterDetection(group.Key, dual, finalPairs);
//                    }
//                }
//            }
//            return finalPairs;
//        }

//        private void GroupInnerDetection(string key, List<KeyValuePair<Collider2D, Collider2D>> pairs)
//        {
//            var xpoints = _groups[key].IntervalPointsX;
//            var ypoints = _groups[key].IntervalPointsY;
            
//            HashSet<int> xSet = new HashSet<int>();
//            HashSet<int> ySet = new HashSet<int>();
//            List<KeyValuePair<int, int>> xPairs = new List<KeyValuePair<int, int>>();
//            List<KeyValuePair<int, int>> yPairs = new List<KeyValuePair<int, int>>();
//            foreach (var pt in xpoints)
//            {
//                if (pt.IsEnd)
//                {
//                    xSet.Remove(pt.ColliderId);
//                    foreach (var c in xSet)
//                    {
//                        if (pt.IsDynamic || IsDynamic(c))
//                        {
//                            // 序号小的在前面，这样判重方便一点
//                            if (pt.ColliderId < c)
//                            {
//                                xPairs.Add(new KeyValuePair<int, int>(pt.ColliderId, c));
//                            }
//                            else
//                            {
//                                xPairs.Add(new KeyValuePair<int, int>(c, pt.ColliderId));
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    xSet.Add(pt.ColliderId);
//                }
//            }

//            foreach (var pt in ypoints)
//            {
//                if (pt.IsEnd)
//                {
//                    ySet.Remove(pt.ColliderId);
//                    foreach (var c in ySet)
//                    {
//                        if (pt.IsDynamic || IsDynamic(c))
//                        {
//                            // 序号小的在前面，这样判重方便一点
//                            if (pt.ColliderId < c)
//                            {
//                                yPairs.Add(new KeyValuePair<int, int>(pt.ColliderId, c));
//                            }
//                            else
//                            {
//                                yPairs.Add(new KeyValuePair<int, int>(c, pt.ColliderId));
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    ySet.Add(pt.ColliderId);
//                }
//            }

//            xPairs.AddRange(yPairs);

//            xPairs.Sort((a, b) =>
//            {
//                int cmp1 = a.Key.CompareTo(b.Key);
//                if (cmp1 == 0)
//                {
//                    return a.Value.CompareTo(b.Value);
//                }
//                return cmp1;
//            });

//            for(int i = 0; i < xPairs.Count; i++)
//            {
//                var pt = xPairs[i];
//                if (i == xPairs.Count - 1 || xPairs[i + 1].Key != pt.Key || xPairs[i + 1].Value != pt.Value)
//                {
//                    continue;
//                }
//                pairs.Add(new KeyValuePair<Collider2D, Collider2D>(_colliders[pt.Key], _colliders[pt.Value]));
//                i++;
//            }
//        }

//        public override List<Collider2D> GetSingleCollision(AABB aabb, Vector2 velocity, float deltaTime, List<string> targetTags)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
