using Everglow.Commons.CustomTiles.Abstracts;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Commons.Physics.DataStructures;

namespace Everglow.Commons.CustomTiles.GameInteraction;

public class ProjCollider : GlobalProjectile, IEntityCollider<Projectile>
{
	public const int HookAIStyle = 7;

	public static readonly HashSet<Projectile> canHook = new();

	private bool collidedWithTile;

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

	private static void On_Projectile_HandleMovement(On_Projectile.orig_HandleMovement orig, Projectile self, Vector2 wetVelocity, out int overrideWidth, out int overrideHeight)
	{
		if (!ColliderManager.Enable || !self.tileCollide || self.aiStyle == HookAIStyle)
		{
			orig(self, wetVelocity, out overrideWidth, out overrideHeight);
			return;
		}

		ColliderManager.EnableHook = false;
		ProjCollider collider = self.GetGlobalProjectile<ProjCollider>();
		IEntityCollider<Projectile> proj = collider;
		proj.Prepare();
		orig(self, wetVelocity, out overrideWidth, out overrideHeight);

		Vector2 oldVelocity = self.velocity;
		collider.collidedWithTile = false;
		if (self.active && self.tileCollide)
		{
			proj.Update();
		}
		ColliderManager.EnableHook = true;

		// 临时补齐普通弹幕的碰撞响应；保持原有移动和平台承载流程。
		if (collider.collidedWithTile && self.active && self.tileCollide && self.velocity != oldVelocity
			&& UsesDefaultTileCollision(self) && ProjectileLoader.OnTileCollide(self, oldVelocity))
		{
			self.Kill();
		}
	}

	private static bool UsesDefaultTileCollision(Projectile projectile)
	{
		if (projectile.minion || projectile.sentry || Main.projPet[projectile.type] || ProjectileID.Sets.Explosive[projectile.type])
		{
			return false;
		}
		if (projectile.ModProjectile is not null)
		{
			// 借用原版 AI 的弹幕可能还依赖原版的特殊碰撞分支。
			return projectile.aiStyle <= 0;
		}

		// tML 1.4.4 中这些常用弹药默认撞墙销毁；不包含陨星弹、纳米弹、叶绿箭等反弹弹药。
		return projectile.aiStyle == ProjAIStyleID.Arrow && projectile.type is
			ProjectileID.Bullet or ProjectileID.SilverBullet or ProjectileID.BulletHighVelocity
			or ProjectileID.CrystalBullet or ProjectileID.CursedBullet or ProjectileID.ChlorophyteBullet
			or ProjectileID.IchorBullet or ProjectileID.VenomBullet or ProjectileID.PartyBullet
			or ProjectileID.ExplosiveBullet or ProjectileID.GoldenBullet or ProjectileID.MoonlordBullet
			or ProjectileID.WoodenArrowFriendly or ProjectileID.WoodenArrowHostile
			or ProjectileID.FireArrow or ProjectileID.UnholyArrow or ProjectileID.JestersArrow
			or ProjectileID.HellfireArrow or ProjectileID.HolyArrow or ProjectileID.CursedArrow
			or ProjectileID.FrostburnArrow or ProjectileID.IchorArrow or ProjectileID.VenomArrow
			or ProjectileID.BeeArrow or ProjectileID.BoneArrowFromMerchant or ProjectileID.ShadowFlameArrow
			or ProjectileID.MoonlordArrow;
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
		collidedWithTile |= result.Normal != Vector2.Zero;
	}

	public void OnLeave()
	{
	}

	public bool Ignore(RigidEntity entity)
	{
		return false;
	}
}
