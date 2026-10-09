using Everglow.Commons.Physics.PBEngine.Collision.BroadPhase.Structure;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace Everglow.Commons.Physics.PBEngine.Collision.BroadPhase
{
    internal class HashGridMethod : BroadPhase
    {
        private List<Collider2D> _colliders;
        private Dictionary<string, MultiLevelHashGrid> _groups;
        private int _curHashGridSize;
        private int _prevHashGridSize;
        private long _prevPairTime;
        private long _prev2PairTime;

        public HashGridMethod(CollisionGraph graph) : base(graph)
        {
            _colliders = new List<Collider2D>();
            _groups = new Dictionary<string, MultiLevelHashGrid>();
            _prevHashGridSize = 256;
            _curHashGridSize = 256;

            _prev2PairTime = long.MaxValue;
            _prevPairTime = long.MinValue;
        }

        public override void DrawDebugInfo(SpriteBatch sb)
        {
            if (!_groups.ContainsKey("Default"))
                return;
            //var data = _groups["Default"].GetProfilingData();
            //sb.Begin();
            //foreach (var entry in data)
            //{
            //    int x = (int)(entry.GridBox.MinPoint.X - Main.screenPosition.X);
            //    int y = (int)(-entry.GridBox.MaxPoint.Y - Main.screenPosition.Y);
            //    int w = (int)(entry.GridBox.MaxPoint.X - entry.GridBox.MinPoint.X);
            //    int h = (int)(entry.GridBox.MaxPoint.Y - entry.GridBox.MinPoint.Y);
            //    sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, y, w, h), Color.White * MathHelper.Lerp(0.1f, 0.5f, entry.NumObjects / 64f));
            //}

            //sb.End();
        }

        public override List<KeyValuePair<Collider2D, Collider2D>> GetCollisionPairs(float deltaTime)
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<KeyValuePair<Collider2D, Collider2D>> finalPairs = new List<KeyValuePair<Collider2D, Collider2D>>();
            foreach (var group in _groups)
            {
                var thisgroup = _groups[group.Key];

                if (_collisionGraph.Graph.ContainsKey(group.Key))
                {
                    foreach (var dual in _collisionGraph.Graph[group.Key])
                    {
                        if (dual == group.Key)
                        {
                            foreach (var pair in thisgroup.QueryPairs())
                            {
                                finalPairs.Add(new KeyValuePair<Collider2D, Collider2D>(_colliders[pair.Key],
                                    _colliders[pair.Value]));
                            }
                            continue;
                        }
                        if (!_groups.ContainsKey(dual))
                            continue;
                        foreach (var pair in thisgroup.QueryPairsWith(_groups[dual]))
                        {
                            finalPairs.Add(new KeyValuePair<Collider2D, Collider2D>(_colliders[pair.Key],
                                _colliders[pair.Value]));
                        }
                    }
                }
            }
            sw.Stop();
            _prev2PairTime = _prevPairTime;
            _prevPairTime = sw.ElapsedTicks;
            return finalPairs;
        }

        public override List<Collider2D> GetSingleCollision(AABB aabb, Vector2 velocity, float deltaTime, List<string> targetTags)
        {
            List<Collider2D> colliders = new List<Collider2D>();
            foreach (var tag in targetTags)
            {
                if (!_groups.ContainsKey(tag))
                {
                    continue;
                }

                foreach (var id in _groups[tag].QueryRange(aabb))
                {
                    colliders.Add(_colliders[id]);
                }
            }
            return colliders;
        }


        public override void Prepare(List<PhysicsObject> objects, float deltaTime)
        {
            _colliders.Clear();
            _colliders.EnsureCapacity(objects.Count);
            _groups.Clear();

            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                _colliders.Add(obj.Collider);
                var entry = new ColliderEntry()
                {
                    ColliderId = i,
                    BoundingBox = obj.Collider.GetAABB(deltaTime),
                    IsDynamic = obj.RigidBody.MovementType == MovementType.Dynamic || obj.RigidBody.MovementType == MovementType.Player
                };

                if (!_groups.ContainsKey(obj.Tag))
                {
                    _groups.Add(obj.Tag, new MultiLevelHashGrid(4, new int[] 
                    {
                        1024,
                        256,
                        64,
                        16
                    }));

                }
                _groups[obj.Tag].Add(entry);
            }

            // _prevHashGridSize = _curHashGridSize;
        }

        public override bool TestSingleCollision(AABB aabb, Vector2 velocity, float deltaTime, List<string> targetTags)
        {
            foreach (var tag in targetTags)
            {
                if (!_groups.ContainsKey(tag))
                {
                    continue;
                }

                if (_groups[tag].QueryRange(aabb).Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
