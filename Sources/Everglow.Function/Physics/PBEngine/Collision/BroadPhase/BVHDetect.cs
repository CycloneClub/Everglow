using Everglow.Commons.Physics.PBEngine;
using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.BroadPhase;
using Everglow.Commons.Physics.PBEngine.Collision.BroadPhase.Structure;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine.Collision.BroadPhase
{
    /// <summary>
    /// 使用BVH辅助加速碰撞检测
    /// </summary>
    public class BVHDetect : BroadPhase
    {
        private List<Collider2D> _colliders;
        private Dictionary<string, List<ColliderEntry>> _groups;
        private Dictionary<string, BVH> _bvhForGroup;
        public BVHDetect(CollisionGraph graph) : base(graph)
        {
            _colliders = new List<Collider2D>();
            _groups = new Dictionary<string, List<ColliderEntry>>();
            _bvhForGroup = new Dictionary<string, BVH>();
        }

        private void GroupInnerDetection(string name,
            List<KeyValuePair<Collider2D, Collider2D>> collisionResults)
        {
            var pairs = _bvhForGroup[name].QueryPairs();
            foreach (var pair in pairs)
            {
                collisionResults.Add(new KeyValuePair<Collider2D, Collider2D>(_colliders[pair.Key],
                    _colliders[pair.Value]));
            }
        }

        private void GroupOuterDetection(string groupA, string groupB,
            List<KeyValuePair<Collider2D, Collider2D>> collisionResults)
        {
            var groupAEntries = _groups[groupA];
            var groupBBVH = _bvhForGroup[groupB];
            for (int i = 0; i < groupAEntries.Count; i++)
            {
                foreach (var id in groupBBVH.QueryRange(groupAEntries[i].BoundingBox))
                {
                    if (groupAEntries[i].IsDynamic || _colliders[id].ParentObject.RigidBody.MovementType == MovementType.Player ||
                        _colliders[id].ParentObject.RigidBody.MovementType == MovementType.Dynamic)
                    {
                        collisionResults.Add(new KeyValuePair<Collider2D, Collider2D>(_colliders[groupAEntries[i].ColliderId],
                            _colliders[id]));
                    }
                }
            }
        }

        public override List<KeyValuePair<Collider2D, Collider2D>> GetCollisionPairs(float deltaTime)
        {
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
                            GroupInnerDetection(group.Key, finalPairs);
                            continue;
                        }
                        if (!_groups.ContainsKey(dual))
                            continue;
                        GroupOuterDetection(group.Key, dual, finalPairs);
                    }
                }
            }
            return finalPairs;
        }

        public override void Prepare(List<PhysicsObject> objects, float deltaTime)
        {
            _colliders.Clear();
            _colliders.EnsureCapacity(objects.Count);
            _groups.Clear();
            _bvhForGroup.Clear();
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

                if (_groups.ContainsKey(obj.Tag))
                {
                    _groups[obj.Tag].Add(entry);
                }
                else
                {
                    _groups.Add(obj.Tag, new List<ColliderEntry>()
                    {
                        entry
                    });
                }
            }
            foreach (var group in _groups)
            {
                _bvhForGroup.Add(group.Key, new BVH(group.Value));
            }
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
                var bvhThisGroup = _bvhForGroup[tag];

                foreach (var id in bvhThisGroup.QueryRange(aabb))
                {
                    colliders.Add(_colliders[id]);
                }
            }
            return colliders;
        }

        public override void DrawDebugInfo(SpriteBatch sb)
        {
            if (!_groups.ContainsKey("Default"))
                return;
            //var data = _bvhForGroup["Default"].GetProfilingData();
            //sb.Begin();
            //foreach (var entry in data)
            //{
            //    int x = (int)(entry.GridBox.MinPoint.X - Main.screenPosition.X);
            //    int y = (int)(-entry.GridBox.MaxPoint.Y - Main.screenPosition.Y);
            //    int w = (int)(entry.GridBox.MaxPoint.X - entry.GridBox.MinPoint.X);
            //    int h = (int)(entry.GridBox.MaxPoint.Y - entry.GridBox.MinPoint.Y);
            //    sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, y, w, h), Color.White * MathHelper.Lerp(0.1f, 0.1f, entry.Layer / 32f));
            //}

            //sb.End();
        }

        public override bool TestSingleCollision(AABB aabb, Vector2 velocity, float deltaTime, List<string> targetTags)
        {
            foreach (var tag in targetTags)
            {
                if (!_groups.ContainsKey(tag))
                {
                    continue;
                }
                var bvhThisGroup = _bvhForGroup[tag];

                if (bvhThisGroup.QueryRange(aabb).Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
