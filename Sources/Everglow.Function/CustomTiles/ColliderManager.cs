using Everglow.Commons.CustomTiles.Abstracts;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Commons.Enums;
using Everglow.Commons.Physics.DataStructures;

namespace Everglow.Commons.CustomTiles;

public class ColliderManager : ILoadable
{
	private const bool AllowOverflow = true;

	private const int Capacity = 100;

	public static bool EnableHook { get; set; } = true;

	public static bool Enable { get; private set; }

	public static ColliderManager Instance => ModContent.GetInstance<ColliderManager>();

	private List<RigidEntity> rigidbodies;

	public void Load(Mod mod)
	{
		rigidbodies = [];

		On_Collision.LaserScan += Collision_LaserScan;
		On_Collision.TileCollision += Collision_TileCollision;
		On_Collision.SolidCollision_Vector2_int_int += Collision_SolidCollision_Vector2_int_int;
		On_Collision.SolidCollision_Vector2_int_int_bool += Collision_SolidCollision_Vector2_int_int_bool;
		On_Collision.IsWorldPointSolid += On_Collision_IsWorldPointSolid;
		On_Collision.StepUp += Collision_StepUp;
		On_Collision.StepDown += Collision_StepDown;

		Ins.HookManager.AddHook(CodeLayer.PostUpdateEverything, Update);
		Ins.HookManager.AddHook(CodeLayer.PostDrawTiles, Draw);
		Ins.HookManager.AddHook(CodeLayer.PostDrawMapIcons, DrawToMap);
	}

	public void Unload() => rigidbodies = null;

	// Now support automatical unload.// On_Collision.LaserScan -= Collision_LaserScan;// On_Collision.TileCollision -= Collision_TileCollision;// On_Collision.SolidCollision_Vector2_int_int -= Collision_SolidCollision_Vector2_int_int;// On_Collision.SolidCollision_Vector2_int_int_bool -= Collision_SolidCollision_Vector2_int_int_bool;

	/// <summary>
	/// Adds the rigid entity to the collection, enabling the manager if it is not already enabled.
	/// <br/>Not recommended for use, call <see cref="Add{TRigidEntity}(Vector2)"/> instead.
	/// </summary>
	/// <remarks>
	/// If the collection exceeds its capacity, inactive entities may be removed to make room for the new entity.
	/// <br/>If no inactive entities are available and overflow is not allowed, the first entity in the collection will be replaced.
	/// </remarks>
	/// <param name="entity">The rigid entity to add to the collection.</param>
	private void Add(RigidEntity entity)
	{
		Enable = true;
		if (rigidbodies.Count >= Capacity)
		{
			int removedCount = rigidbodies.RemoveAll(rg => !rg.Active);
			if (removedCount == 0 && !AllowOverflow) // TODO: Add more intelligent removal strategy. Allow banning removal for some important entities.
			{
				rigidbodies[0] = entity;
				return;
			}
		}
		rigidbodies.Add(entity);
	}

	/// <summary>
	/// Adds the rigid entity to the collection, enabling the manager if it is not already enabled.
	/// <br/>Automatically creates an instance of the specified rigid entity type using <c>new()</c>.
	/// </summary>
	/// <remarks>
	/// If the collection exceeds its capacity, inactive entities may be removed to make room for the new entity.
	/// <br/>If no inactive entities are available and overflow is not allowed, the first entity in the collection will be replaced.
	/// </remarks>
	/// <param name="position">The initial position of the rigid entity.</param>
	public TRigidEntity Add<TRigidEntity>(Vector2 position)
		where TRigidEntity : RigidEntity, new()
	{
		var entity = new TRigidEntity()
		{
			Position = position,
		};

		Add(entity);

		entity.SetDefaults();
		entity.OnSpawn();

		return entity;
	}

	/// <summary>
	/// Tries to find the first rigid entity that intersects with the specified axis-aligned bounding box.
	/// <br/>If none is found, returns false and sets result to null.
	/// </summary>
	/// <param name="aabb"></param>
	/// <param name="result"></param>
	/// <returns></returns>
	public bool TryFirst(AABB aabb, out RigidEntity result)
	{
		foreach (var entity in rigidbodies)
		{
			if (entity.Intersect(aabb))
			{
				result = entity;
				return true;
			}
		}
		result = null;
		return false;
	}

	/// <summary>
	/// Indicates whether any rigid entity intersects with the specified axis-aligned bounding box.
	/// </summary>
	/// <param name="aabb"></param>
	/// <returns></returns>
	public bool Intersect(AABB aabb)
	{
		foreach (var entity in rigidbodies)
		{
			if (entity.Intersect(aabb))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Attempts to move the specified box by the given stride, detecting and reporting any collisions encountered during movement.
	/// </summary>
	/// <remarks>
	/// If a collision occurs, the stride may be adjusted to prevent overlapping with other entities.
	/// <br/>The box's final position reflects the actual movement after resolving collisions.
	/// </remarks>
	/// <param name="box">The box to move. Must not be null.</param>
	/// <param name="stride">The desired movement vector to apply to the box.</param>
	/// <returns>Details about each collision encountered in the order they occurred.</returns>
	public IEnumerable<CollisionResult> Move(IBox box, Vector2 stride)
	{
		var list = new List<CollisionResult>();
		foreach (var entity in rigidbodies)
		{
			if (box.Ignore(entity))
			{
				continue;
			}

			if (entity.Collision(box, stride, out var result))
			{
				stride = result.Stride;
				list.Add(result);
			}
		}

		box.Position += stride;
		return list;
	}

	/// <summary>
	/// Filters the elements based on a specified type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public IEnumerable<T> OfType<T>()
	{
		return rigidbodies.OfType<T>();
	}

	#region Game Hooks

	private void Update()
	{
		foreach (var rigidbody in rigidbodies.Where(r => r.Active))
		{
			rigidbody.Update();
		}

		rigidbodies.RemoveAll(r => !r.Active);
	}

	private void Clear()
	{
		rigidbodies.Clear();
		Enable = false;
	}

	private void Draw()
	{
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		foreach (var tile in rigidbodies)
		{
			tile.Draw();
		}
		Main.spriteBatch.End();
	}

	private void DrawToMap(Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale)
	{
		foreach (var tile in rigidbodies)
		{
			tile.DrawToMap(mapTopLeft, mapX2Y2AndOff, mapRect, mapScale);
		}
	}

	// TODO: StepHook, when player try to move on the CustomTile, it should be stepped to the top.
	private void Collision_StepUp(On_Collision.orig_StepUp orig, ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir = 1, bool holdsMatching = false, int specialChecksMode = 0)
	{
		// if (!Enable || !EnableHook)
		// {
		// if (gravDir == 1 && gfxOffY != 0)
		// {
		// foreach (var customTile in rigidbodies.OfType<BoxEntity>())
		// {
		// if(Math.Abs(customTile.Box.Top - (position.Y - gfxOffY + height)) <= 1)
		// {
		// gfxOffY = 0;
		// break;
		// }
		// }
		// }
		// }
		orig(ref position, ref velocity, width, height, ref stepSpeed, ref gfxOffY, gravDir, holdsMatching, specialChecksMode);
		if (!Enable || !EnableHook)
		{
			CustomTileStepUp(ref position, ref velocity, width, height, ref stepSpeed, ref gfxOffY, gravDir, holdsMatching, specialChecksMode);
		}
	}

	private void CustomTileStepUp(ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir = 1, bool holdsMatching = false, int specialChecksMode = 0)
	{
		float oldPosY = position.Y;
		float ascendValue = 0;
		if (Instance is null || MathF.Abs(velocity.X) < 1e-5)
		{
			return;
		}
		float maxAscend = 20;
		AABB entityBoxNow = new AABB(position + new Vector2(0, 0), new Vector2(width, height));

		// var playerVFX = new PlayerCollisionIndicator();
		// playerVFX.Visible = true;
		// playerVFX.Active = true;
		// playerVFX.MaxTime = 1;
		// playerVFX.Pos = position + new Vector2(0, gfxOffY - 0.4f);
		// playerVFX.HitBox = new Rectangle((int)entityBoxNow.Left, (int)entityBoxNow.Top, width, height);
		// Ins.VFXManager.Add(playerVFX);
		AABB entityBoxNext = new AABB(position + velocity + new Vector2(0, gfxOffY - 0.4f), new Vector2(width, height));
		float customTileTop = 0;
		if (gravDir == 1)
		{
			foreach (var customTile in rigidbodies.OfType<BoxEntity>())
			{
				if (customTile.Intersect(entityBoxNext) && !customTile.Intersect(entityBoxNow) && customTile.Box.Top > entityBoxNext.Bottom - maxAscend && entityBoxNext.Bottom - customTile.Box.Top > ascendValue)
				{
					customTileTop = customTile.Box.Top;
					ascendValue = entityBoxNext.Bottom - customTile.Box.Top;
				}
			}
			if (customTileTop != 0)
			{
				gfxOffY += position.Y + height - customTileTop;
				position.Y = customTileTop - height - 2;
			}
		}
		else
		{
			// up-side-down
			foreach (var customTile in rigidbodies.OfType<BoxEntity>())
			{
				if (customTile.Intersect(entityBoxNext) && !customTile.Intersect(entityBoxNow) && customTile.Box.Bottom < entityBoxNext.Top + maxAscend && entityBoxNext.Top - customTile.Box.Bottom > ascendValue)
				{
					customTileTop = customTile.Box.Bottom;
					ascendValue = entityBoxNext.Top - customTile.Box.Bottom;
				}
			}
			if (customTileTop != 0)
			{
				gfxOffY += position.Y - customTileTop;
				position.Y = customTileTop + 2;
			}
		}
	}

	private void Collision_StepDown(On_Collision.orig_StepDown orig, ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir, bool waterWalk)
	{
		orig(ref position, ref velocity, width, height, ref stepSpeed, ref gfxOffY, gravDir, waterWalk);
		if (!Enable || !EnableHook)
		{
			CustomTileStepDown(ref position, ref velocity, width, height, ref stepSpeed, ref gfxOffY, gravDir, waterWalk);
		}
	}

	public void CustomTileStepDown(ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir, bool waterWalk)
	{
		if (gfxOffY < 0)
		{
			AABB entityBoxNow = new AABB(position + new Vector2(0, gfxOffY - 0.4f), new Vector2(width, height));
			AABB entityBoxNext = new AABB(position + velocity + new Vector2(0, gfxOffY - 0.4f), new Vector2(width, height));
			foreach (var customTile in rigidbodies.OfType<BoxEntity>())
			{
				if (customTile.Intersect(entityBoxNext) && !customTile.Intersect(entityBoxNow))
				{
					position.Y = customTile.Box.Top - height;
					gfxOffY = 0;
					stepSpeed = 0;
				}
			}
		}
	}
	#endregion

	#region Collision Hooks

	private bool On_Collision_IsWorldPointSolid(On_Collision.orig_IsWorldPointSolid orig, Vector2 pos, bool treatPlatformsAsNonSolid)
	{
		if (!Enable || !EnableHook)
		{
			return orig(pos, treatPlatformsAsNonSolid);
		}
		bool collision_with_custom_tile = false;
		foreach (var entity in rigidbodies)
		{
			if (entity.Intersect(new AABB(pos, Vector2.One)))
			{
				collision_with_custom_tile = true;
				break;
			}
		}
		return orig(pos, treatPlatformsAsNonSolid) || collision_with_custom_tile;
	}

	private void Collision_LaserScan(On_Collision.orig_LaserScan orig, Vector2 samplingPoint, Vector2 directionUnit, float samplingWidth, float maxDistance, float[] samples)
	{
		if (!Enable || !EnableHook)
		{
			orig(samplingPoint, directionUnit, samplingWidth, maxDistance, samples);
			return;
		}
		orig(samplingPoint, directionUnit, samplingWidth, maxDistance, samples);
		for (int i = 0; i < samples.Length; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				float t = MathHelper.Lerp(0, samples[i], j / 20f);
				Vector2 p = samplingPoint + t * directionUnit;
				if (Intersect(new AABB(p - Vector2.One * 6, Vector2.One * 12)))
				{
					samples[i] = t;
					break;
				}
			}
		}
	}

	private bool Collision_SolidCollision_Vector2_int_int(On_Collision.orig_SolidCollision_Vector2_int_int orig, Vector2 Position, int Width, int Height)
	{
		if (!Enable || !EnableHook)
		{
			return orig(Position, Width, Height);
		}
		return orig(Position, Width, Height) || Intersect(new AABB(Position.X, Position.Y, Width, Height));
	}

	private bool Collision_SolidCollision_Vector2_int_int_bool(On_Collision.orig_SolidCollision_Vector2_int_int_bool orig, Vector2 Position, int Width, int Height, bool acceptTopSurfaces)
	{
		if (!Enable || !EnableHook)
		{
			return orig(Position, Width, Height, acceptTopSurfaces);
		}
		return orig(Position, Width, Height, acceptTopSurfaces) || Intersect(new AABB(Position.X, Position.Y, Width, Height));
	}

	private Vector2 Collision_TileCollision(On_Collision.orig_TileCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough, bool fall2, int gravDir)
	{
		Vector2 stride = orig(Position, Velocity, Width, Height, fallThrough, fall2, gravDir);
		if (EnableHook && Enable)
		{
			var box = new BoxImpl()
			{
				Position = Position,
				Size = new Vector2(Width, Height),
				Gravity = gravDir,
			};
			foreach (var entity in rigidbodies)
			{
				if (entity.Collision(box, stride, out var result))
				{
					stride = result.Stride;
				}
			}
			return stride;
		}
		return stride;
	}

	#endregion

	public class ColliderManagerSystem : ModPlayer
	{
		public override void OnEnterWorld()
		{
			if (Player.whoAmI == Main.myPlayer)
			{
				Instance.Clear();
			}
		}
	}
}
