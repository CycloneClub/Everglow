using Everglow.Myth.Misc.Projectiles.Weapon.Magic.BoneFeatherMagic;
using Terraria.Audio;
using Terraria.DataStructures;
using static Everglow.Commons.Utilities.ProjectileUtils;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic;

public class BoneFeather : StickNPCProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 3600;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 2;
	}

	internal int timeTokill = -1;
	private ModProjectile magicArray = null;

	public override void OnSpawn(IEntitySource source)
	{
		foreach (Projectile projectile in Main.projectile)
		{
			if (projectile.active)
			{
				if (projectile.type == ModContent.ProjectileType<BoneFeatherMagicArray>())
				{
					if (projectile.owner == Projectile.owner)
					{
						magicArray = projectile.ModProjectile;
						break;
					}
				}
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (timeTokill >= 4 && timeTokill <= 6)
		{
			bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 60;
			bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 60;
			bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 60;
			bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 60;
			return bool0 || bool1 || bool2 || bool3;
		}
		if (timeTokill > 0)
		{
			return false;
		}
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override void AI()
	{
		if (timeTokill >= 0 && timeTokill <= 2)
		{
			AmmoHit();
		}
		timeTokill--;
		if (StuckNPC == -1)
		{
			if (timeTokill < 0)
			{
				Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
				if (Projectile.timeLeft < 320)
				{
					if (Projectile.wet)
					{
						Projectile.velocity.Y -= 0.3f;
						Projectile.velocity *= 0.96f;
					}
				}
			}
			if (Main.rand.NextBool(6))
			{
				Vector2 v = new Vector2(0, Main.rand.NextFloat(0, 1)).RotatedByRandom(MathHelper.TwoPi);
				Dust.NewDust(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<Dusts.Bones>(), v.X, v.Y, 0, default, Main.rand.NextFloat(0.8f, 1.2f));
			}
			if (Projectile.timeLeft <= 100 && timeTokill < 0)
			{
				AmmoHit();
			}
		}
		else
		{
			if (StuckNPC == -2)
			{
				Projectile.Kill();
			}
		}
		if (Projectile.position.X <= 320 || Projectile.position.X >= Main.maxTilesX * 16 - 320)
		{
			Projectile.Kill();
		}
		if (Projectile.position.Y <= 320 || Projectile.position.Y >= Main.maxTilesY * 16 - 320)
		{
			Projectile.Kill();
		}
		base.AI();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spriteEffects = SpriteEffects.FlipHorizontally;
		}

		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, spriteEffects, 0);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (magicArray != null)
		{
			var arrayProj = magicArray as BoneFeatherMagicArray;
			arrayProj.WingPower += 0.1f;
		}
		AmmoHit();
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (magicArray != null)
		{
			var arrayProj = magicArray as BoneFeatherMagicArray;
			arrayProj.WingPower += 2.6f;
		}
		timeTokill = 600 * (1 + Projectile.extraUpdates);
	}

	public void AmmoHit()
	{
		SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath.WithVolume(0.3f).WithPitchOffset(0.8f), Projectile.Center);
		for (int j = 0; j < 4; j++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(7, 20)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.BoneFeather>(), v.X, v.Y, 0, default, Main.rand.NextFloat(1.8f, 3.7f));
		}
		Projectile.Kill();
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		base.ModifyHitNPC(target, ref modifiers);
	}
}

public class BoneNPCModifier : GlobalNPC
{
	public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
	{
		foreach (Projectile p in Main.projectile)
		{
			if (p.type == ModContent.ProjectileType<BoneFeather>() && p.active && p != projectile)
			{
				BoneFeather bf = p.ModProjectile as BoneFeather;
				if (bf.StuckNPC == npc.whoAmI)
				{
					if (bf.timeTokill > 0)
					{
						if ((p.type == projectile.type && bf.timeTokill < 540 * (p.extraUpdates + 1)) || p.type != projectile.type)
						{
							modifiers.ScalingBonusDamage += 0.07f;
							bf.AmmoHit();
						}
					}
				}
			}
		}
		base.ModifyHitByProjectile(npc, projectile, ref modifiers);
	}

	public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
	{
		foreach (Projectile p in Main.projectile)
		{
			if (p.type == ModContent.ProjectileType<BoneFeather>() && p.active)
			{
				BoneFeather bf = p.ModProjectile as BoneFeather;
				if (bf.StuckNPC == npc.whoAmI)
				{
					if (bf.timeTokill > 0)
					{
						modifiers.ScalingBonusDamage += 0.07f;
						bf.AmmoHit();
					}
				}
			}
		}
		base.ModifyHitByItem(npc, player, item, ref modifiers);
	}
}
