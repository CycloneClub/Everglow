using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine.Collision.BroadPhase.Structure
{
    public struct ProfilingDataBVH
    {
        public AABB GridBox;
        public int Layer;
    }
    public class BVHNode
    {
        public AABB Box
        {
            get; set;
        }
        public BVHNode Left
        {
            get; set;
        }
        public BVHNode Right
        {
            get; set;
        }
        public int SplitIndex
        {
            get; set;
        }
        public List<ColliderEntry> Colliders
        {
            get; set;
        }
        public bool Checked
        {
            get; set;
        }

        public BVHNode()
        {
            Colliders = new List<ColliderEntry>();
            Left = null;
            Right = null;
            SplitIndex = -1;
            Checked = false;
        }
    }

    public class BVH
    {
        public BVHNode Root
        {
            get; private set;
        }

        private List<ColliderEntry> _colliders;
        private List<KeyValuePair<int, int>> _pairQueryCache;

        private const int MAX_COLLIDERS_PER_NODE = 1;

        public BVH(List<ColliderEntry> colliders)
        {
            _colliders = colliders;
            _pairQueryCache = new List<KeyValuePair<int, int>>();
            Root = _build(0, _colliders.Count - 1);

            //foreach (ColliderEntry collider in _colliders)
            //{
            //    Insert(collider);
            //}
        }


        public void Insert(in ColliderEntry entry)
        {
            if (Root == null)
            {
                Root = new BVHNode() 
                { 
                    Box = entry.BoundingBox, 
                    Colliders = new List<ColliderEntry> { entry },
                };
                return;
            }
            _insert(Root, entry);
        }

        private void _insert(BVHNode node, in ColliderEntry entry)
        {
            if (node.Colliders.Count == 0)
            {
                var newLeftBox = node.Left.Box.Merge(entry.BoundingBox);
                var newRightBox = node.Right.Box.Merge(entry.BoundingBox);
                if (newLeftBox.GetArea() - node.Left.Box.GetArea()
                    < newRightBox.GetArea() - node.Right.Box.GetArea())
                {
                    _insert(node.Left, entry);
                }
                else
                {
                    _insert(node.Right, entry);
                }
                node.Box = node.Left.Box.Merge(node.Right.Box);
            }
            else
            {
                node.Left = new BVHNode()
                {
                    Box = node.Colliders[0].BoundingBox,
                    Colliders = new List<ColliderEntry>() { node.Colliders[0] }
                };

                node.Right = new BVHNode()
                {
                    Box = entry.BoundingBox,
                    Colliders = new List<ColliderEntry>() { entry }
                };

                node.Colliders.Clear();
                node.Box = node.Left.Box.Merge(node.Right.Box);
            }
        }

        private BVHNode _build(int l, int r)
        {
            BVHNode node = new BVHNode();
            if (r - l + 1 <= MAX_COLLIDERS_PER_NODE)
            {
                node.Colliders = _colliders.GetRange(l, r - l + 1);
                AABB aabb = new AABB();
                for (int i = l; i <= r; i++)
                {
                    aabb.MergeWith(_colliders[i].BoundingBox);
                }
                node.Box = aabb;
                return node;
            }

            AABB fullBox = new AABB();
            for (int i = l; i <= r; i++)
            {
                fullBox.MergeWith(_colliders[i].BoundingBox);
            }

            var sortByX = Comparer<ColliderEntry>.Create((a, b) =>
            {
                return a.BoundingBox.Center.X.CompareTo(b.BoundingBox.Center.X);
            });
            var sortByY = Comparer<ColliderEntry>.Create((a, b) =>
            {
                return a.BoundingBox.Center.Y.CompareTo(b.BoundingBox.Center.Y);
            });
            Comparer<ColliderEntry> compFunc = (fullBox.MaxPoint.Y - fullBox.MinPoint.Y) > (fullBox.MaxPoint.X - fullBox.MinPoint.X)
                ? sortByY : sortByX;
            _colliders.Sort(l, r - l + 1, compFunc);

            int mid = l + (r - l) / 2;

            node.Left = _build(l, mid);
            node.Right = _build(mid + 1, r);
            node.Box = node.Left.Box.Merge(node.Right.Box);
            node.SplitIndex = mid;
            return node;
        }

        private void _clearFlags(BVHNode node)
        {
            if (node == null)
                return;
            node.Checked = false;
            _clearFlags(node.Left);
            _clearFlags(node.Right);
        }

        private void _innerPairQuery(BVHNode node)
        {
            if (node == null)
                return;
            if (node.Colliders.Count != 0)
            {
                int sz = node.Colliders.Count;
                for (int i = 0; i < sz; i++)
                {
                    for (int j = i + 1; j < sz; j++)
                    {
                        if ((node.Colliders[i].IsDynamic || node.Colliders[j].IsDynamic)
                            && node.Colliders[i].BoundingBox.Intersects(node.Colliders[j].BoundingBox))
                        {
                            _pairQueryCache.Add(new KeyValuePair<int, int>(node.Colliders[i].ColliderId,
                                node.Colliders[j].ColliderId));
                        }
                    }
                }
                return;
            }
            _innerPairQuery(node.Left);
            _innerPairQuery(node.Right);
        }
        private void _outerPairQuery(BVHNode node1, BVHNode node2)
        {
            int sz1 = node1.Colliders.Count;
            int sz2 = node2.Colliders.Count;
            for (int i = 0; i < sz1; i++)
            {
                for (int j = 0; j < sz2; j++)
                {
                    if ((node1.Colliders[i].IsDynamic || node2.Colliders[j].IsDynamic)
                        && node1.Colliders[i].BoundingBox.Intersects(node2.Colliders[j].BoundingBox))
                    {
                        _pairQueryCache.Add(new KeyValuePair<int, int>(node1.Colliders[i].ColliderId,
                            node2.Colliders[j].ColliderId));
                    }
                }
            }
        }

        private void _queryPairHelper(BVHNode node1, BVHNode node2)
        {

            if (node1.Colliders.Count > 0 && node2.Colliders.Count > 0)
            {
                _outerPairQuery(node1, node2);
                return;
            }
            if (node1.Colliders.Count > 0 && node2.Colliders.Count == 0)
            {
                if (node1.Box.Intersects(node2.Left.Box))
                {
                    _queryPairHelper(node1, node2.Left);
                }
                if (node1.Box.Intersects(node2.Right.Box))
                {
                    _queryPairHelper(node1, node2.Right);
                }
                if (!node2.Checked)
                {
                    node2.Checked = true;
                    _queryPairHelper(node2.Left, node2.Right);
                }
                return;
            }
            if (node2.Colliders.Count > 0 && node1.Colliders.Count == 0)
            {
                if (node2.Box.Intersects(node1.Left.Box))
                {
                    _queryPairHelper(node2, node1.Left);
                }
                if (node2.Box.Intersects(node1.Right.Box))
                {
                    _queryPairHelper(node2, node1.Right);
                }
                if (!node1.Checked)
                {
                    node1.Checked = true;
                    _queryPairHelper(node1.Left, node1.Right);
                }
                return;
            }

            if (node1.Box.Intersects(node2.Box))
            {
                _queryPairHelper(node1.Left, node2.Left);
                _queryPairHelper(node1.Right, node2.Left);
                _queryPairHelper(node1.Left, node2.Right);
                _queryPairHelper(node1.Right, node2.Right);
            }
            if (!node1.Checked)
            {
                node1.Checked = true;
                _queryPairHelper(node1.Left, node1.Right);
            }
            if (!node2.Checked)
            {
                node2.Checked = true;
                _queryPairHelper(node2.Left, node2.Right);
            }

        }

        private void _queryPair(BVHNode node, int l, int r)
        {
            // If is a leaf node
            if (node.Colliders.Count > 0)
            {
                for (int i = 0; i < node.Colliders.Count; i++)
                {
                    for (int j = i + 1; j < node.Colliders.Count; j++)
                    {
                        if ((node.Colliders[i].IsDynamic || node.Colliders[j].IsDynamic)
                            && node.Colliders[i].BoundingBox.Intersects(node.Colliders[j].BoundingBox))
                        {
                            _pairQueryCache.Add(new KeyValuePair<int, int>(node.Colliders[i].ColliderId,
                                node.Colliders[j].ColliderId));
                        }
                    }
                }
                return;
            }
            _queryPairHelper(node.Left, node.Right);
            //_queryPair(node.Left, l, node.SplitIndex);
            //_queryPair(node.Right, node.SplitIndex + 1, r);

            //if (node.Left.Box.Intersects(node.Right.Box))
            //{
            //    for (int i = l; i <= node.SplitIndex; i++)
            //    {
            //        for (int j = node.SplitIndex + 1; j <= r; j++)
            //        {
            //            if ((_colliders[i].IsDynamic || _colliders[j].IsDynamic) &&
            //                _colliders[i].BoundingBox.Intersects(_colliders[j].BoundingBox))
            //            {
            //                _pairQueryCache.Add(new KeyValuePair<int, int>(_colliders[i].ColliderId,
            //                    _colliders[j].ColliderId));
            //            }
            //        }
            //    }
            //}
        }

        public List<KeyValuePair<int,int>> QueryPairs()
        {
            _pairQueryCache.Clear();
            if (_colliders.Count == 0)
            {
                return _pairQueryCache;
            }
            _clearFlags(Root);
            _innerPairQuery(Root);
            _queryPair(Root, 0, _colliders.Count - 1);
            return _pairQueryCache;
        }

        public List<int> QueryRange(in AABB targetRange)
        {
            List<int> result = new List<int>();
            if (!targetRange.Intersects(Root.Box))
                return result;

            Stack<BVHNode> stack = new Stack<BVHNode>();
            stack.Push(Root);

            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                if (!cur.Box.Intersects(targetRange))
                {
                    continue;
                }
                if (cur.Colliders.Count != 0)
                {
                    foreach (var collider in cur.Colliders)
                    {
                        if (collider.BoundingBox.Intersects(targetRange))
                        {
                            result.Add(collider.ColliderId);
                        }
                    }
                }
                else
                {
                    if (cur.Left.Box.Intersects(targetRange))
                    {
                        stack.Push(cur.Left);
                    }
                    if (cur.Right.Box.Intersects(targetRange))
                    {
                        stack.Push(cur.Right);
                    }
                }
            }
            return result;
        }

        private void _getProfilingData(BVHNode node, int layer, List<ProfilingDataBVH> result)
        {
            if(node == null) return;
            result.Add(new ProfilingDataBVH()
            {
                GridBox = node.Box,
                Layer = layer,
            });

            _getProfilingData(node.Left, layer + 1, result);
            _getProfilingData(node.Right, layer + 1, result);
        }
        public List<ProfilingDataBVH> GetProfilingData()
        {
            List<ProfilingDataBVH> result = new List<ProfilingDataBVH>();
            _getProfilingData(Root, 0, result);
            return result;
        }
    }
}
