using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine.GameInteraction
{
	/// <summary>
	/// 与TR地形进行碰撞检测的函数
	/// </summary>
	public class TileCollisionUtils
	{
		public static List<Vector2> GenerateHalfBrick(int x, int y)
		{
			var slope = Main.tile[x, y].Slope;
			if (slope == Terraria.ID.SlopeType.Solid)
			{
				if (Main.tile[x, y].IsHalfBlock)
				{
					return new List<Vector2>()
					{
					 new Vector2(x * 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 - 8 ),
					 new Vector2(x * 16, -y * 16 - 8),
					};
				}
				else
				{
					return new List<Vector2>()
					{
					 new Vector2(x * 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 ),
					 new Vector2(x * 16, -y * 16),
					};
				}
			}
			else if (slope == Terraria.ID.SlopeType.SlopeDownLeft)
			{
				return new List<Vector2>()
				{
					 new Vector2(x * 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 - 16),
					 new Vector2(x * 16, -y * 16),
				};
			}
			else if (slope == Terraria.ID.SlopeType.SlopeDownRight)
			{
				return new List<Vector2>()
				{
					 new Vector2(x * 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 ),
				};
			}
			else if (slope == Terraria.ID.SlopeType.SlopeUpLeft)
			{
				return new List<Vector2>()
				{
					 new Vector2(x * 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 ),
					 new Vector2(x * 16, -y * 16),
				};
			}
			else if (slope == Terraria.ID.SlopeType.SlopeUpRight)
			{
				return new List<Vector2>()
				{
					 new Vector2(x * 16 + 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 ),
					 new Vector2(x * 16, -y * 16),
				};
			}
			return new List<Vector2>()
				{
					 new Vector2(x * 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 - 16),
					 new Vector2(x * 16 + 16, -y * 16 ),
					 new Vector2(x * 16, -y * 16),
				};
		}

		public static bool GetPolygonTileCollisionInfo(AABB boundingBox, Vector2 polyCenter, List<Edge2D> edges, List<Vector2> corners,
		   out float depth1, out Vector2 normal1)
		{
			bool collided = false;
			float maxDepth = 0;
			Vector2 maxNormal = Vector2.Zero;
			int minX = (int)Math.Floor(boundingBox.MinPoint.X / 16);
			int maxX = (int)Math.Ceiling(boundingBox.MaxPoint.X / 16);
			int minY = (int)Math.Floor((-boundingBox.MaxPoint.Y) / 16);
			int maxY = (int)Math.Ceiling((-boundingBox.MinPoint.Y) / 16);

			for (int x = Math.Max(minX, 0); x <= Math.Min(maxX, Main.maxTilesX - 1); x++)
			{
				for (int y = Math.Max(minY, 0); y <= Math.Min(maxY, Main.maxTilesY - 1); y++)
				{
					if (Main.tile[x, y] != null && Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
					{
						var localPoints = GenerateHalfBrick(x, y);
						List<Edge2D> edgesTile = new List<Edge2D>();
						for (int i = 0; i < localPoints.Count; i++)
						{
							edgesTile.Add(new Edge2D(localPoints[i], localPoints[(i + 1) % localPoints.Count]));
						}
						var centerTile = new Vector2(x * 16 + 8, -y * 16 - 8);
						float depth;
						Vector2 normal;
						if (GeometryUtils.ConvexPolygonPolygonCollisionInfo(edges, corners, edgesTile, localPoints,
							out depth, out normal))
						{
							depth1 = 0;
							normal1 = Vector2.Zero;
							return true;

							if (depth > maxDepth)
							{
								maxDepth = depth;
								maxNormal = Vector2.Dot(maxNormal, polyCenter - centerTile) < 0 ? -normal : normal;
							}
						}
					}
				}
			}
			depth1 = 0;
			normal1 = maxNormal;
			return collided;
		}

		public static void GetPolygonTileContactInfo(AABB boundingBox, Vector2 polyCenter, List<Edge2D> edges,
			List<Vector2> corners,
			PhysicsObject A, PhysicsObject B,
			float deltaTime,
			List<CollisionEvent2D> outputList)
		{
			int minX = (int)Math.Floor(boundingBox.MinPoint.X / 16);
			int maxX = (int)Math.Ceiling(boundingBox.MaxPoint.X / 16);
			int minY = (int)Math.Floor((-boundingBox.MaxPoint.Y) / 16);
			int maxY = (int)Math.Ceiling((-boundingBox.MinPoint.Y) / 16);

			for (int x = Math.Max(minX, 0); x <= Math.Min(maxX, Main.maxTilesX - 1); x++)
			{
				for (int y = Math.Max(minY, 0); y <= Math.Min(maxY, Main.maxTilesY - 1); y++)
				{
					if (Main.tile[x, y] != null && Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
					{
						var localPoints = GenerateHalfBrick(x, y);
						List<Edge2D> edgesTile = new List<Edge2D>();
						for (int i = 0; i < localPoints.Count; i++)
						{
							edgesTile.Add(new Edge2D(localPoints[i], localPoints[(i + 1) % localPoints.Count]));
						}
						var centerTile = new Vector2(x * 16 + 8, -y * 16 - 8);

						var boxCollider = A.Collider as BoxCollider;
						float depth;
						Vector2 normal;
						if (GeometryUtils.ConvexPolygonPolygonCollisionInfo(
							edges,
							corners, edgesTile, localPoints,
							out depth, out normal))
						{
							var e = new CollisionEvent2D()
							{
								Time = 0,
								Source = A,
								Target = B,
								LocalOffsetSrc = Vector2.Zero,
								LocalOffsetTarget = Vector2.Zero,
								Normal = Vector2.Dot(normal, polyCenter - centerTile) < 0 ? -normal : normal,
								Position = Vector2.Zero,
								Depth = depth,
							};
							float weightA = e.Source.RigidBody.InvMass / (e.Source.RigidBody.InvMass + e.Target.RigidBody.InvMass);
							float weightB = e.Target.RigidBody.InvMass / (e.Source.RigidBody.InvMass + e.Target.RigidBody.InvMass);
							e.Source.RigidBody.MoveBody(e.Normal * weightA * e.Depth, deltaTime);
							e.Target.RigidBody.MoveBody(-e.Normal * weightB * e.Depth, deltaTime);

							List<KeyValuePair<Vector2, Vector2>> contacts = new List<KeyValuePair<Vector2, Vector2>>();
							GeometryUtils.ConvexPolygonPolygonContactInfo(
								boxCollider.GetEdges(deltaTime),
								boxCollider.GetCornerPoints(deltaTime), edgesTile, localPoints.ToList(), polyCenter,
								centerTile, contacts);
							foreach (var c in contacts)
							{
								outputList.Add(new CollisionEvent2D(e)
								{
									LocalOffsetSrc = c.Key,
									LocalOffsetTarget = c.Value,
								});
							}
						}
					}
				}
			}
		}

		public static bool GetSphereTileCollisionInfo(Vector2 center, float radius, PhysicsObject A, PhysicsObject B,
			out float depth1, out Vector2 normal1)
		{
			bool collided = false;
			float maxDepth = 0;
			Vector2 maxNormal = Vector2.Zero;
			int minX = (int)Math.Floor((center.X - radius) / 16);
			int maxX = (int)Math.Ceiling((center.X + radius) / 16);
			int minY = (int)Math.Floor((-center.Y - radius) / 16);
			int maxY = (int)Math.Ceiling((-center.Y + radius) / 16);

			for (int x = Math.Max(minX, 0); x <= Math.Min(maxX, Main.maxTilesX - 1); x++)
			{
				for (int y = Math.Max(minY, 0); y <= Math.Min(maxY, Main.maxTilesY - 1); y++)
				{
					if (Main.tile[x, y] != null && Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
					{
						var localPoints = GenerateHalfBrick(x, y);
						List<Edge2D> edges = new List<Edge2D>();
						for (int i = 0; i < localPoints.Count; i++)
						{
							edges.Add(new Edge2D(localPoints[i], localPoints[(i + 1) % localPoints.Count]));
						}
						var centerTile = new Vector2(x * 16 + 8, -y * 16 - 8);
						float depth;
						Vector2 normal;
						if (GeometryUtils.SphereConvexPolygonCollisionInfo(center, radius, edges, localPoints.ToList(), out depth, out normal))
						{
							collided = true;
							if (depth > maxDepth)
							{
								maxDepth = depth;
								maxNormal = Vector2.Dot(maxNormal, center - centerTile) < 0 ? -normal : normal;
							}
						}
					}
				}
			}
			depth1 = 0;
			normal1 = maxNormal;
			return collided;
		}

		public static void GetSphereTileContactInfo(Vector2 center, float radius, PhysicsObject A, PhysicsObject B,
			float deltaTime,
			List<CollisionEvent2D> outputList)
		{
			int minX = (int)Math.Floor((center.X - radius) / 16);
			int maxX = (int)Math.Ceiling((center.X + radius) / 16);
			int minY = (int)Math.Floor((-center.Y - radius) / 16);
			int maxY = (int)Math.Ceiling((-center.Y + radius) / 16);

			for (int x = Math.Max(minX, 0); x <= Math.Min(maxX, Main.maxTilesX - 1); x++)
			{
				for (int y = Math.Max(minY, 0); y <= Math.Min(maxY, Main.maxTilesY - 1); y++)
				{
					if (Main.tile[x, y] != null && Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
					{
						var localPoints = GenerateHalfBrick(x, y);
						List<Edge2D> edges = new List<Edge2D>();
						for (int i = 0; i < localPoints.Count; i++)
						{
							edges.Add(new Edge2D(localPoints[i], localPoints[(i + 1) % localPoints.Count]));
						}
						var centerTile = new Vector2(x * 16 + 8, -y * 16 - 8);
						float depth;
						Vector2 normal;
						if (GeometryUtils.SphereConvexPolygonCollisionInfo(A.RigidBody.CentroidWorldSpace, radius, edges, localPoints.ToList(), out depth, out normal))
						{
							var e = new CollisionEvent2D()
							{
								Time = 0,
								Source = A,
								Target = B,
								LocalOffsetSrc = Vector2.Zero,
								LocalOffsetTarget = Vector2.Zero,
								Normal = Vector2.Dot(normal, center - centerTile) < 0 ? -normal : normal,
								Position = Vector2.Zero,
								Depth = depth,
							};
							float weightA = e.Source.RigidBody.InvMass / (e.Source.RigidBody.InvMass + e.Target.RigidBody.InvMass);
							float weightB = e.Target.RigidBody.InvMass / (e.Source.RigidBody.InvMass + e.Target.RigidBody.InvMass);
							e.Source.RigidBody.MoveBody(e.Normal * weightA * e.Depth, deltaTime);
							e.Target.RigidBody.MoveBody(-e.Normal * weightB * e.Depth, deltaTime);

							List<KeyValuePair<Vector2, Vector2>> contacts = new List<KeyValuePair<Vector2, Vector2>>();
							GeometryUtils.SphereConvexPolygonContactInfo(e.Source.RigidBody.CentroidWorldSpace, radius, edges, localPoints.ToList(), centerTile, contacts);
							foreach (var c in contacts)
							{
								outputList.Add(new CollisionEvent2D(e)
								{
									LocalOffsetSrc = c.Key,
									LocalOffsetTarget = c.Value,
								});
							}
						}
					}
				}
			}
		}

		public static bool GetCapsuleTileCollisionInfo(AABB boundingBox, Vector2 segA, Vector2 segB, float radius,
			out float depth1, out Vector2 normal1)
		{
			bool collided = false;
			float maxDepth = 0;
			Vector2 maxNormal = Vector2.Zero;
			int minX = (int)Math.Floor(boundingBox.MinPoint.X / 16);
			int maxX = (int)Math.Ceiling(boundingBox.MaxPoint.X / 16);
			int minY = (int)Math.Floor((-boundingBox.MaxPoint.Y) / 16);
			int maxY = (int)Math.Ceiling((-boundingBox.MinPoint.Y) / 16);

			for (int x = Math.Max(minX, 0); x <= Math.Min(maxX, Main.maxTilesX - 1); x++)
			{
				for (int y = Math.Max(minY, 0); y <= Math.Min(maxY, Main.maxTilesY - 1); y++)
				{
					if (Main.tile[x, y] != null && Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
					{
						var localPoints = GenerateHalfBrick(x, y);
						List<Edge2D> edges = new List<Edge2D>();
						for (int i = 0; i < localPoints.Count; i++)
						{
							edges.Add(new Edge2D(localPoints[i], localPoints[(i + 1) % localPoints.Count]));
						}
						var centerTile = new Vector2(x * 16 + 8, -y * 16 - 8);
						float depth;
						Vector2 normal;
						if (GeometryUtils.CapsuleConvexPolygonCollisionInfo(segA, segB, radius, localPoints.ToList(), edges,
							out depth, out normal))
						{
							collided = true;

							if (depth > maxDepth)
							{
								maxDepth = depth;
								maxNormal = Vector2.Dot(maxNormal, boundingBox.Center - centerTile) < 0 ? -normal : normal;
							}
						}
					}
				}
			}
			depth1 = 0;
			normal1 = maxNormal;
			return collided;
		}

		public static void GetCapsuleTileContactInfo(AABB boundingBox, Vector2 segA, Vector2 segB, float radius,
			PhysicsObject A, PhysicsObject B,
			float deltaTime,
			List<CollisionEvent2D> outputList)
		{
			int minX = (int)Math.Floor(boundingBox.MinPoint.X / 16);
			int maxX = (int)Math.Ceiling(boundingBox.MaxPoint.X / 16);
			int minY = (int)Math.Floor((-boundingBox.MaxPoint.Y) / 16);
			int maxY = (int)Math.Ceiling((-boundingBox.MinPoint.Y) / 16);

			var capsule = A.Collider as CapsuleCollider;

			for (int x = Math.Max(minX, 0); x <= Math.Min(maxX, Main.maxTilesX - 1); x++)
			{
				for (int y = Math.Max(minY, 0); y <= Math.Min(maxY, Main.maxTilesY - 1); y++)
				{
					if (Main.tile[x, y] != null && Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
					{
						var localPoints = GenerateHalfBrick(x, y);
						List<Edge2D> edges = new List<Edge2D>();
						for (int i = 0; i < localPoints.Count; i++)
						{
							edges.Add(new Edge2D(localPoints[i], localPoints[(i + 1) % localPoints.Count]));
						}
						var centerTile = new Vector2(x * 16 + 8, -y * 16 - 8);
						float depth;
						Vector2 normal;

						if (GeometryUtils.CapsuleConvexPolygonCollisionInfo(segA, segB, radius, localPoints.ToList(), edges,
							out depth, out normal))
						{
							var e = new CollisionEvent2D()
							{
								Time = 0,
								Source = A,
								Target = B,
								LocalOffsetSrc = Vector2.Zero,
								LocalOffsetTarget = Vector2.Zero,
								Normal = Vector2.Dot(normal, boundingBox.Center - centerTile) < 0 ? -normal : normal,
								Position = Vector2.Zero,
								Depth = depth,
							};
							float weightA = e.Source.RigidBody.InvMass / (e.Source.RigidBody.InvMass + e.Target.RigidBody.InvMass);
							float weightB = 1 - weightA;
							e.Source.RigidBody.MoveBody(e.Normal * weightA * e.Depth, deltaTime);
							e.Target.RigidBody.MoveBody(-e.Normal * weightB * e.Depth, deltaTime);

							List<KeyValuePair<Vector2, Vector2>> contacts = new List<KeyValuePair<Vector2, Vector2>>();
							capsule.GetSegment(deltaTime, out Vector2 segAt, out Vector2 segBt);
							GeometryUtils.CapsuleConvexPolygonContactInfo(segAt, segBt, radius, localPoints.ToList(), edges,
								centerTile, contacts);
							foreach (var c in contacts)
							{
								outputList.Add(new CollisionEvent2D(e)
								{
									LocalOffsetSrc = c.Key,
									LocalOffsetTarget = c.Value,
								});
							}
						}
					}
				}
			}
		}
	}
}