using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.ProjectileEffects;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.ProjNPC;

public class AncientPhantom_Magic_Orb : ModNPC
{
	public int Timer = 0;

	public override void SetStaticDefaults()
	{
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		NPC.width = 18;
		NPC.height = 18;
		NPC.lifeMax = 1;
		NPC.damage = 36;
		NPC.defense = 0;
		NPC.friendly = false;
		NPC.aiStyle = NPCAIStyleID.Fighter;
		NPC.knockBackResist = 0.75f;
		NPC.value = 0;
		NPC.noGravity = true;
		NPC.HitSound = SoundID.Item53;
		NPC.DeathSound = SoundID.Shatter;
		NPCID.Sets.ProjectileNPC[Type] = true;
	}

	public override void AI()
	{
		Timer++;
		NPC.rotation += 0.6f;
		if (Timer % 2 == 0)
		{
			var dustVFX = new TwilightCrystalVFXDust
			{
				Velocity = NPC.velocity * 0.1f,
				Active = true,
				Visible = true,
				Position = NPC.Center + NPC.velocity.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * (Timer % 4 - 1) * 2,
				MaxTime = Main.rand.Next(6, 15),
				Scale = Main.rand.NextFloat(9, 11),
				Rotation = NPC.velocity.ToRotation() - MathHelper.PiOver4,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}

		var dustVFX_s = new TwilightCrystalVFXDust
		{
			Velocity = NPC.velocity * 0.1f,
			Active = true,
			Visible = true,
			Position = NPC.Center + NPC.velocity.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * MathF.Pow(MathF.Sin(Timer * 1f), 3) * 6,
			MaxTime = Main.rand.Next(30, 45),
			Scale = Main.rand.NextFloat(3, 4),
			Rotation = NPC.velocity.ToRotation() - MathHelper.PiOver4,
			ai = new float[] { 0, 0, 0 },
		};
		Ins.VFXManager.Add(dustVFX_s);
		if (NPC.collideX || NPC.collideY)
		{
			NPC.life = 0;
			OnKill();
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		NPC.life = 0;
		OnKill();
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit)
	{
		NPC.life = 0;
		OnKill();
	}

	public override void OnKill()
	{
		float waveRot = Main.rand.NextFloat(6.283f);
		for (int i = 0; i < 2; i++)
		{
			var wave = new AncientPhantom_Magic_ExplosionWave
			{
				Velocity = Vector2.zeroVector,
				Active = true,
				Visible = true,
				Position = NPC.Center,
				MaxTime = 45 - i * 15,
				Scale = i * 0.6f,
				Rotation = waveRot + i * MathHelper.Pi / 6f,
				ai = new float[] { 0.07f + i * 0.002f },
			};
			Ins.VFXManager.Add(wave);
		}
		for (int i = 0; i < 20; i++)
		{
			Vector2 vel = new Vector2(10 + (i % 2) * 2, 0).RotatedBy(MathHelper.TwoPi / 20f * i) * 0.25f;
			var dustVFX = new TwilightCrystalVFXDust_slow
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = NPC.Center,
				MaxTime = Main.rand.Next(37, 55),
				Scale = Main.rand.NextFloat(9, 15),
				Rotation = vel.ToRotationSafe() - MathHelper.PiOver4,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}

		for (int j = 0; j < 6; j++)
		{
			for (int i = 0; i < 12; i++)
			{
				Vector2 vel = new Vector2(i - 6, 6 * MathF.Sqrt(3)).RotatedBy(j * MathHelper.TwoPi / 6f) * 0.2f;
				vel = vel.RotatedBy(MathF.Sin(i * MathHelper.PiOver2) * 0.15f);
				vel *= 1f + MathF.Sin(i * MathHelper.PiOver4) * 0.5f;
				var dustVFX = new TwilightCrystalVFXDust_slow
				{
					Velocity = vel,
					Active = true,
					Visible = true,
					Position = NPC.Center,
					MaxTime = Main.rand.Next(37, 55),
					Scale = Main.rand.NextFloat(4, 5),
					Rotation = vel.ToRotationSafe() - MathHelper.PiOver4,
					ai = new float[] { 0, 0, 0 },
				};
				Ins.VFXManager.Add(dustVFX);
			}
		}
		Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<AncientPhantom_Magic_Explosion>(), 24, 1, Main.myPlayer);
		proj.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = ModAsset.AncientPhantom_Magic_Orb.Value;
		spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, drawColor * 0.75f, NPC.rotation, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * 0.2f, NPC.rotation, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		if (Timer % 6 == 0)
		{
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			float size = Main.rand.NextFloat(0.2f, 0.5f);
			for (int t = 0; t < 3; t++)
			{
				Main.EntitySpriteDraw(star, NPC.Center + new Vector2(10, 0).RotatedBy(NPC.rotation) - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * size, t * MathHelper.TwoPi / 3f, star.Size() * 0.5f, size, SpriteEffects.None, 0);
			}
		}
		return false;
	}
}
