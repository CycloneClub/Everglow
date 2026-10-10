using Everglow.Commons.CustomTiles.Abstracts;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Commons.Physics.DataStructures;

namespace Everglow.Commons.CustomTiles.GameInteraction;

public class ProjCollider : GlobalProjectile, IEntityCollider<Projectile>
{
	public const int HookAIStyle = 7;

	public static readonly HashSet<Projectile> canHook = new();

	public override bool CloneNewInstances => true;

	public override bool InstancePerEntity => true;

	public override bool IsCloneable => true;

	public Vector2 Position { get => Entity.position; set => Entity.position = value; }

	public Projectile Entity { get; set; }

	public RigidEntity Ground { get; set; }

	public float OffsetY { get => Entity.gfxOffY; set => Entity.gfxOffY = value; }

	public Vector2 OldPosition { get; set; }

	public AABB Box => new AABB(Entity.position, Entity.width, Entity.height);

	public float Gravity => 1;

	public Vector2 Size => new Vector2(Entity.width, Entity.height);

	public Vector2 Velocity { get => Entity.velocity; set => Entity.velocity = value; }

	public override GlobalProjectile Clone(Projectile from, Projectile to)
	{
		var clone = base.Clone(from, to) as ProjCollider;
		clone.Entity = to;
		clone.Ground = null;
		clone.OldPosition = to.position;
		return clone;
	}

	public override void Load()
	{
		On_Projectile.AI_007_GrapplingHooks += On_Projectile_AI_007_GrapplingHooks;
		On_Projectile.AI_007_GrapplingHooks_CanTileBeLatchedOnTo += On_Projectile_AI_007_GrapplingHooks_CanTileBeLatchedOnTo;
		On_Projectile.HandleMovement += On_Projectile_HandleMovement;
	}

	private bool On_Projectile_AI_007_GrapplingHooks_CanTileBeLatchedOnTo(On_Projectile.orig_AI_007_GrapplingHooks_CanTileBeLatchedOnTo orig, Projectile self, int x, int y)
	{
		if (canHook.Contains(self))
		{
			return true;
		}
		return orig(self, x, y);
	}

	private static void On_Projectile_AI_007_GrapplingHooks(On_Projectile.orig_AI_007_GrapplingHooks orig, Projectile self)
	{
		if (!ColliderManager.Enable)
		{
			orig(self);
			return;
		}
		if (self.ai[0] != 1)
		{
			ProjCollider collider = self.GetGlobalProjectile<ProjCollider>();
			collider.UpdateHook();
		}
		orig(self);
		canHook.Remove(self);
	}

	private static void On_Projectile_HandleMovement(On_Projectile.orig_HandleMovement orig, Projectile self, Vector2 wetVelocity)
	{
		if (!ColliderManager.Enable || !self.tileCollide || self.aiStyle == HookAIStyle)
		{
			orig(self, wetVelocity);
			return;
		}

		ColliderManager.EnableHook = false;
		IEntityCollider<Projectile> proj = self.GetGlobalProjectile<ProjCollider>();

		// 记录位置，否则会把传送当成位移
		proj.Prepare();
		orig(self, wetVelocity);
		proj.Update();
		ColliderManager.EnableHook = true;
	}

	private void UpdateHook()
	{
		if (Ground is not null)
		{
			if (Ground.Active)
			{
				canHook.Add(Entity);
				((IHookable)Ground).SetHookPosition(Entity);
			}
			else
			{
				Ground = null;
			}
			return;
		}

		foreach (var hookable in ColliderManager.Instance.OfType<IHookable>())
		{
			var rigitbody = (RigidEntity)hookable;
			if (rigitbody.Intersect(new AABB(Entity.position, Entity.Size)))
			{
				hookable.SetHookPosition(Entity);
				canHook.Add(Entity);
				Ground = rigitbody;
				break;
			}
		}
	}

	public void OnCollision(CollisionResult result)
	{
	}

	public void OnLeave()
	{
	}

	public bool Ignore(RigidEntity entity)
	{
		return false;
	}
}
