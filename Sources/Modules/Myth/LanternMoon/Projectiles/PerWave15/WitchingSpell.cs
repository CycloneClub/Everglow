using Everglow.Commons.Utilities.BuffHelpers;
using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class WitchingSpell : ModProjectile
{
	public float Timer = 0;

	public NPC OwnerNPC;

	public int ChasePlayerTime = 180;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCs;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 800;
		Projectile.alpha = 0;
		Projectile.penetrate = 1;
		Projectile.scale = 0.75f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		ChasePlayerTime += Main.rand.Next(-60, 61);
	}

	public override void AI()
	{
		Timer++;
		if (Projectile.velocity.X < -0.5f)
		{
			Projectile.spriteDirection = -1;
		}
		if (Projectile.velocity.X > 0.5f)
		{
			Projectile.spriteDirection = 1;
		}
		if (Timer > 65 && Timer % 5 == Projectile.whoAmI % 5)
		{
			Projectile.frame++;
			if (Projectile.frame >= 23)
			{
				Projectile.frame = 6;
			}
		}
		if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<WizardLantern>())
		{
			if (Timer < 120)
			{
				Projectile.velocity = new Vector2(0, -3).RotatedBy(Projectile.ai[0] / 8f * MathHelper.TwoPi) * MathF.Sin((float)Main.time * 0.06f + Projectile.ai[0] * MathHelper.Pi);
			}
			else if (Timer < ChasePlayerTime)
			{
				Projectile.tileCollide = true;
				Projectile.hostile = true;
				var toTarget = OwnerNPC.Center - Projectile.Center - Projectile.velocity + new Vector2(MathF.Sin(Timer * 0.03f + Projectile.whoAmI) * 120f, MathF.Sin(Timer * 0.015f + Projectile.whoAmI) * 50f);
				Projectile.velocity = toTarget.SafeNormalize(Vector2.Zero) * 6 * 0.1f + Projectile.velocity * 0.9f;
			}
			else if (Timer < ChasePlayerTime + 60)
			{
				Vector2 targetPos = Vector2.zeroVector;
				int targetIndex = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
				Player player = null;
				if (targetIndex != -1)
				{
					player = Main.player[targetIndex];
				}
				if (player != null)
				{
					targetPos = player.Center;
				}
				if (targetPos != Vector2.zeroVector)
				{
					var toTarget = targetPos - Projectile.Center - Projectile.velocity;
					Projectile.velocity = toTarget.SafeNormalize(Vector2.Zero) * 4 * 0.1f + Projectile.velocity * 0.9f;
				}
			}
		}
		else
		{
			KillEffect();
			Projectile.Kill();
		}
		Projectile.rotation = Projectile.velocity.X * 0.3f;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(BuffID.Confused, 180);
		KillEffect();
		Projectile.Kill();
		base.OnHitPlayer(target, info);
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		modifiers.Knockback *= 0;
		base.ModifyHitPlayer(target, ref modifiers);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		KillEffect();
		return base.OnTileCollide(oldVelocity);
	}

	public void KillEffect()
	{
		for (int k = 0; k < 16; k++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(7f, 14f)).RotatedByRandom(MathHelper.TwoPi);
			var spellPaper = new SpellPaperFragment
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				RotateSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.05f, 0.15f) * (k % 2 - 0.5f) * 2,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(45, 60),
				Scale = Main.rand.NextFloat(1.4f, 3.2f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(spellPaper);
		}
		for (int k = 0; k < 16; k++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 5f)).RotatedByRandom(MathHelper.TwoPi);
			var WitchingSpark = new WitchingSpellDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.4f, 0.4f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.02f, 0.05f) * (k % 2 - 0.5f) * 2,
				MaxTime = Main.rand.Next(45, 80),
				Scale = Main.rand.NextFloat(1.5f, 2f),
			};
			Ins.VFXManager.Add(WitchingSpark);
		}
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		if (Timer < 30)
		{
			Projectile.frame = 0;
		}
		if (Timer >= 30 && Timer < 65)
		{
			Projectile.frame = (int)((Timer - 30) / 6f);
		}
		SpriteEffects effects = SpriteEffects.None;
		Projectile.spriteDirection = Projectile.direction;
		if (Projectile.spriteDirection == -1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Texture2D tex_shape = ModAsset.Spell_shape.Value;
		Texture2D tex_glow = ModAsset.WitchingSpell_glow.Value;
		Texture2D bloom = ModAsset.Spell_bloom.Value;
		float fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade = Projectile.timeLeft / 60f;
		}
		Color drawColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()) * fade;
		Color bloomColor = Color.Lerp(new Color(0.3f, 1f, 0.4f, 0), new Color(0.1f, 0.6f, 0.3f, 0), MathF.Sin(Timer * 0.03f + Projectile.whoAmI) * 0.5f + 0.5f);
		Rectangle frame = new Rectangle(0, Projectile.frame * 60, 60, 60);
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, drawColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, effects, 0);
		if (Timer < 12)
		{
			Color shapeColor = new Color(0.3f, 1f, 0.4f, 0) * ((12 - Timer) / 12f);
			Main.EntitySpriteDraw(tex_shape, Projectile.Center - Main.screenPosition, frame, shapeColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, effects, 0);
		}
		Main.EntitySpriteDraw(tex_glow, Projectile.Center - Main.screenPosition, frame, new Color(0.3f, 1f, 0.4f, 0), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, effects, 0);
		Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, frame, bloomColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, effects, 0);
		return false;
	}
}
