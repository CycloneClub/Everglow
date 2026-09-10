using Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class ArcI_proj : ModProjectile
{
	public Player Owner;

	public int useCount = 0;

	public int overridedamage;

	public IEntitySource shootSource = null;

	private Vector2 OwnerMouseWorld => Main.player[Projectile.owner].MouseWorld();

	public override void SetDefaults()
	{
		Projectile.timeLeft = 6000000;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.friendly = false;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.tileCollide = false;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Owner = Main.player[Projectile.owner];
		shootSource = source;
		if (source is EntitySource_ItemUse_WithAmmo withammo && withammo.Entity is Player owner)
		{
			var modifer = owner.GetTotalDamage(withammo.Item.DamageType);
			modifer.CombineWith(owner.bulletDamage);
			CombinedHooks.ModifyWeaponDamage(owner, withammo.Item, ref modifer);
			overridedamage = Math.Max(
				1,
				(int)modifer.ApplyTo(withammo.Item.damage + ContentSamples.ItemsByType[withammo.AmmoItemIdUsed].damage));
		}
		else
		{
			overridedamage = -1;
		}
		Projectile.hide = true;
		base.OnSpawn(source);
	}

	public override void AI()
	{
		if (Owner is null || !Owner.active || Owner.dead)
		{
			Projectile.Kill();
			return;
		}
		Projectile.Center = Owner.MountedCenter;
		Projectile.velocity *= 0;

		var arcI = Owner.HeldItem.ModItem as ArcI;
		if (arcI == null)
		{
			Projectile.Kill();
			return;
		}
		int controlCount = 0;
		Vector2 toMouse = OwnerMouseWorld - Owner.MountedCenter;
		toMouse = Vector2.Normalize(toMouse);
		if (toMouse.X > 0)
		{
			Projectile.spriteDirection = 1;
		}
		if (toMouse.X < 0)
		{
			Projectile.spriteDirection = -1;
		}
		Projectile.rotation = toMouse.ToRotationSafe();
		Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
		if (Owner.controlUseItem)
		{
			if (Projectile.timeLeft % Owner.HeldItem.useTime == 0)
			{
				if (Owner.HasAmmo(Owner.HeldItem))
				{
					Shoot();
				}
				else
				{
					Projectile.Kill();
				}
			}
			controlCount++;
		}
		if (controlCount == 0)
		{
			Projectile.Kill();
		}
	}

	private void Shoot()
	{
		var toMuzzle = new Vector2(12, -4 * Projectile.spriteDirection);
		toMuzzle = toMuzzle.RotatedBy(Projectile.rotation);
		Player player = Main.player[Projectile.owner];
		Vector2 toMouse = OwnerMouseWorld - Projectile.Center;
		toMouse = Vector2.Normalize(toMouse);
		Vector2 velocity = Vector2.Normalize(toMouse) * 27;
		Item item = player.HeldItem;
		if (item.ModItem is ArcI)
		{
			ScreenShaker Gsplayer = player.GetModPlayer<ScreenShaker>();
			Gsplayer.FlyCamPosition = new Vector2(0, 2).RotatedByRandom(6.283);
			var arc = item.ModItem as ArcI;
			arc.Power++;
			if (arc.Power >= 5)
			{
				arc.Power = 0;
				arc.CurrencyCount++;
				if (arc.CurrencyCount > 3)
				{
					arc.CurrencyCount = 3;
				}
			}
			var p = Projectile.NewProjectileDirect(
				shootSource,
				Projectile.Center + toMuzzle,
				velocity,
				arc.ShootType,
				overridedamage == -1 ? item.damage : overridedamage,
				item.knockBack,
				player.whoAmI);
			p.CritChance = (int)(item.crit + player.GetCritChance(DamageClass.Generic));
			if (arc.CurrencyCount > 0)
			{
				int target = FindTarget();
				if (target >= 0)
				{
					Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + toMuzzle, Vector2.zeroVector, ModContent.ProjectileType<ArcI_Current>(), 120, 1, Projectile.owner);
					ArcI_Current aIC = p0.ModProjectile as ArcI_Current;
					if (aIC != null)
					{
						aIC.ParentProj = Projectile;
						aIC.Target = Main.npc[target];
					}
					arc.CurrencyCount--;
				}
			}
		}
		useCount++;
	}

	public int FindTarget()
	{
		float closest = 360;
		float nearestToMouse = 240;
		int index = -1;
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active && !npc.dontTakeDamage)
			{
				float distance = (npc.Center - Projectile.Center).Length();
				if (distance < closest)
				{
					Vector2 toMouse = Owner.MouseWorld() - npc.Center;
					if (toMouse.Length() < nearestToMouse)
					{
						nearestToMouse = toMouse.Length();
						index = npc.whoAmI;
					}
				}
			}
		}
		return index;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModAsset.ArcI_proj.Value;
		Texture2D tex_glow = ModAsset.ArcI_glow.Value;
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spriteEffects = SpriteEffects.FlipVertically;
		}
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Color glowColor = new Color(1f, 1f, 1f, 0);
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		Vector2 toMouse = OwnerMouseWorld - Projectile.Center;
		toMouse = Vector2.Normalize(toMouse);
		drawPos += toMouse * 24;
		Main.EntitySpriteDraw(tex, drawPos, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, 1f, spriteEffects, 0);
		Main.EntitySpriteDraw(tex_glow, drawPos, null, glowColor, Projectile.rotation, tex.Size() * 0.5f, 1f, spriteEffects, 0);
		return false;
	}
}
