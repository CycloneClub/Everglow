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
//        }

//        private List<IntervalPoint> _pointsX;
//        private List<IntervalPoint> _pointsY;
//        private List<Collider2D> _colliders;   

//        public SweepAndPrune()
//        {
//            _pointsX = new List<IntervalPoint>();
//            _pointsY = new List<IntervalPoint>();
//            _colliders = new List<Collider2D>();
//        }
//        public List<KeyValuePair<Collider2D, Collider2D>> GetCollisionPairs(float deltaTime)
//        {
//            List<KeyValuePair<Collider2D, Collider2D>> finalPairs = new List<KeyValuePair<Collider2D, Collider2D>>();
//            HashSet<KeyValuePair<int, int>> spottedPairs = new HashSet<KeyValuePair<int, int>>();
//            HashSet<int> current = new HashSet<int>();
//            for (int i = 0; i < _pointsX.Count; i++)
//            {
//                var pt = _pointsX[i];
//                if (pt.IsEnd)
//                {
//                    foreach (var c in current)
//                    {
//                        if (c == pt.ColliderId)
//                            continue;
//                        var p = new KeyValuePair<int, int>(pt.ColliderId, c);

//                        if (_colliders[p.Key].ParentObject.RigidBody.MovementType == MovementType.Static
//                            && _colliders[p.Value].ParentObject.RigidBody.MovementType == MovementType.Static)
//                        {
//                            continue;
//                        }
//                        spottedPairs.Add(p);
//                    }
//                    current.Remove(pt.ColliderId);
//                }
//                else
//                {
//                    current.Add(pt.ColliderId);
//                }
//            }

//            current.Clear();

//            for (int i = 0; i < _pointsY.Count; i++)
//            {
//                var pt = _pointsY[i];
//                if (pt.IsEnd)
//                {
//                    foreach (var c in current)
//                    {
//                        if (c == pt.ColliderId)
//                            continue;
//                        var p = new KeyValuePair<int, int>(pt.ColliderId, c);
//                        if (spottedPairs.Contains(p)
//                            || spottedPairs.Contains(new KeyValuePair<int, int>(c, pt.ColliderId)))
//                        {
//                            if (_colliders[p.Key].ParentObject.RigidBody.MovementType == MovementType.Static
//                                && _colliders[p.Value].ParentObject.RigidBody.MovementType == MovementType.Static)
//                            {
//                                continue;
//                            }
//                            finalPairs.Add(new KeyValuePair<Collider2D, Collider2D>(_colliders[p.Key], _colliders[p.Value]));
//                        }
//                    }
//                    current.Remove(pt.ColliderId);
//                }
//                else
//                {
//                    current.Add(pt.ColliderId);
//                }
//            }
//            return finalPairs;
//        }

//        public void Prepare(List<PhysicsObject> dynamicObjects, List<PhysicsObject> staticObjects, float deltaTime)
//        {
//            _pointsX.Clear();
//            _pointsY.Clear();
//            _colliders.Clear();
//            _colliders.EnsureCapacity(dynamicObjects.Count + staticObjects.Count);
//            for (int i = 0; i < dynamicObjects.Count; i++)
//            {
//                var obj = dynamicObjects[i];
//                var aabb = obj.Collider.GetAABB(deltaTime);
//                _pointsX.Add(new IntervalPoint() { V = aabb.MinPoint.X, IsEnd = false, ColliderId = i });
//                _pointsX.Add(new IntervalPoint() { V = aabb.MaxPoint.X, IsEnd = true, ColliderId = i });
//                _pointsY.Add(new IntervalPoint() { V = aabb.MinPoint.Y, IsEnd = false, ColliderId = i });
//                _pointsY.Add(new IntervalPoint() { V = aabb.MaxPoint.Y, IsEnd = true, ColliderId = i });
//                _colliders.Add(obj.Collider);
//            }

//            for (int i = 0; i < staticObjects.Count; i++)
//            {
//                var obj = staticObjects[i];
//                var aabb = obj.Collider.GetAABB(deltaTime);
//                int id = staticObjects.Count + i;
//                _pointsX.Add(new IntervalPoint() { V = aabb.MinPoint.X, IsEnd = false, ColliderId = id });
//                _pointsX.Add(new IntervalPoint() { V = aabb.MaxPoint.X, IsEnd = true, ColliderId = id });
//                _pointsY.Add(new IntervalPoint() { V = aabb.MinPoint.Y, IsEnd = false, ColliderId = id });
//                _pointsY.Add(new IntervalPoint() { V = aabb.MaxPoint.Y, IsEnd = true, ColliderId = id });
//                _colliders.Add(obj.Collider);
//            }

//            Comparison<IntervalPoint> cmp = (a, b) =>
//            {
//                int c = a.V.CompareTo(b.V);
//                if (c == 0)
//                {
//                    if (a.IsEnd && !b.IsEnd)
//                    {
//                        return -1;
//                    }
//                    if (!a.IsEnd && b.IsEnd)
//                    {
//                        return 1;
//                    }
//                    return 0;
//                }
//                return c;
//            };
//            _pointsX.Sort(cmp);
//            _pointsY.Sort(cmp);
//        }
//    }
//}
