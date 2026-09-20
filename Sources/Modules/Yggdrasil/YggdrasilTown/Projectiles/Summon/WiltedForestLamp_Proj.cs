using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class WiltedForestLamp_Proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 550;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.penetrate = -1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.width = 30;
		Projectile.height = 30;
	}

	public Vector2 MousePos
	{
		get => new Vector2(Projectile.ai[0], Projectile.ai[1]);

		set
		{
			Projectile.ai[0] = value.X;
			Projectile.ai[1] = value.Y;
		}
	}

	public bool Arrived = false;

	public int AmmoCount
	{
		get => (int)Projectile.ai[2];
		set => Projectile.ai[2] = value;
	}

	public int AmmoCooling = 0;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.velocity *= 0;
		Projectile.tileCollide = false;
		Arrived = true;
		return false;
	}

	public override void OnSpawn(IEntitySource source)
	{
		MousePos = Main.MouseWorld;
		Player player = Main.player[Projectile.owner];
		AmmoCount = player.maxMinions * 3;
		Projectile.netUpdate = true;
	}

	public override void AI()
	{
		if (!Arrived)
		{
			Projectile.timeLeft = 550;
			Projectile.velocity = Vector2.Normalize(MousePos - Projectile.Center) * 15f;
			if ((MousePos - Projectile.Center).Length() < 75)
			{
				Arrived = true;
			}
		}
		else
		{
			Player player = Main.player[Projectile.owner];
			Projectile.velocity.X *= 0.85f;
			Projectile.velocity.Y *= 0.85f;
			Projectile.velocity.Y += MathF.Sin((float)(Main.time * 0.03f)) * 0.015f;
			if (AmmoCount > 0)
			{
				Projectile.timeLeft = 120;
				AmmoCooling--;
			}
			if (AmmoCooling <= 0 && Projectile.velocity.Length() < 1)
			{
				NPC npc = Projectile.FindTargetWithinRange(800);
				int playerTarget = player.MinionAttackTargetNPC;
				if (playerTarget is >= 0 and < 200)
				{
					npc = Main.npc[playerTarget];
				}
				if (npc != null)
				{
					AmmoCooling = 20;
					AmmoCount--;
					if (Collision.CanHit(Projectile, npc))
					{
						Vector2 vel = Vector2.Normalize(npc.Center - Projectile.Center) * 12;
						Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<WiltedForestLamp_Proj_shoot>(), Projectile.damage, 1, Projectile.owner);
					}
				}
			}
			Vector3 colorLight;
			var leafColor = new Color(0.1f, 1f, 0.4f, 1);
			float mulRadius = 1f;
			if (Projectile.timeLeft > 110)
			{
				colorLight = new Vector3(1f, 1f, 0.2f);
			}
			else if (Projectile.timeLeft is > 40 and < 110)
			{
				float lerpValue = (Projectile.timeLeft - 40) / 70f;
				colorLight = Vector3.Lerp(new Vector3(1f, 1f, 0.2f), new Vector3(0.2f, 0.01f, 0.2f), lerpValue);
				leafColor = new Color(0.8f, 0.6f, 0.45f, 1);
				mulRadius = lerpValue * 0.9f + 0.1f;
			}
			else
			{
				colorLight = new Vector3(0.2f, 0.01f, 0.2f);
				leafColor = new Color(0.8f, 0.6f, 0.45f, 1);
				mulRadius = 0.1f;
			}
			Lighting.AddLight(Projectile.Center, colorLight);
			var dustVFX = new Leaf_VFX_Spin
			{
				omega = 0.04f,
				rotatedCenter = Projectile.Center,
				radius = Main.rand.NextFloat(60, 78) * mulRadius,
				rotPos = Main.rand.NextFloat(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(70, 120),
				maxScale = Main.rand.Next(7, 12),
				scale = Main.rand.Next(8, 10),
				color = leafColor,
				ai = new float[] { Main.rand.NextFloat(1f, 8f) },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D lamp = ModAsset.WiltedForestLamp_Proj.Value;
		Texture2D lamp2 = ModAsset.WiltedForestLamp_Proj_Wilted.Value;
		Texture2D lampGlow = ModAsset.WiltedForestLamp_Proj_glow.Value;
		Texture2D lampGlow2 = ModAsset.WiltedForestLamp_Proj_Wilted_glow.Value;
		Texture2D outLamp = ModAsset.WiltedForestLamp_Proj_OutOfFlame.Value;

		if (Projectile.timeLeft > 110)
		{
			Main.EntitySpriteDraw(lamp, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, lamp.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(lampGlow, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * 0.5f, Projectile.rotation, lampGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		else if (Projectile.timeLeft is > 40 and < 110)
		{
			Main.EntitySpriteDraw(outLamp, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, lamp.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(lampGlow, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * (MathF.Sin(Projectile.timeLeft * 0.6f) + Projectile.timeLeft / 30f - 2), Projectile.rotation, lampGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(lampGlow2, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * (MathF.Sin(Projectile.timeLeft * 0.6f + MathF.PI) + Projectile.timeLeft / 30f - 2), Projectile.rotation, lampGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		else
		{
			Main.EntitySpriteDraw(lamp2, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, lamp.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(lampGlow2, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * 0.15f, Projectile.rotation, lampGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		Player player = Main.player[Projectile.owner];
		for (int i = 0; i < 50; i++)
		{
			var dustVFX = new Leaf_VFX
			{
				velocity = new Vector2(0, Main.rand.NextFloat(2f, 16f)).RotatedByRandom(Math.PI * 2) + Projectile.velocity,
				omega = Main.rand.NextFloat(-0.1f, 0.1f),
				beta = Main.rand.NextFloat(-0.01f, 0.01f),
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(70, 120),
				scale = Main.rand.Next(8, 10),
				color = new Color(0.8f, 0.6f, 0.45f, 1),
				ai = new float[] { Main.rand.NextFloat(1f, 8f) },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		for (int x = 0; x < 24; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.4f, 1.6f));
			d.velocity = new Vector2(0, Main.rand.NextFloat(0.7f, 14.1f)).RotatedByRandom(6.283);
		}
		foreach (var npc in Main.npc)
		{
			if (npc != null && npc.active)
			{
				if (!npc.dontTakeDamage && !npc.friendly)
				{
					if ((npc.Center - Projectile.Center).Length() < 220)
					{
						npc.AddElementalDebuffBuildUp(Main.player[Projectile.owner], BurnDebuff.ID, 5000);
					}
				}
			}
		}
	}
}
