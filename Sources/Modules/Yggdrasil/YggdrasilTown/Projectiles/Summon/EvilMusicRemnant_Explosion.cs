using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class EvilMusicRemnant_Explosion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = Projectile.height = 96;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 3;
		Projectile.tileCollide = false;
		Projectile.alpha = 255;
		Projectile.knockBack = 10f;
		Projectile.DamageType = DamageClass.Summon;
	}

	public override string Texture => ModAsset.EvilMusicRemnant_Minion_Mod;

	public override void AI()
	{
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (Main.expertMode)
		{
			if (target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail)
			{
				modifiers.FinalDamage /= 5;
			}
		}
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
		for (int i = 0; i < 120; i++)
		{
			float size = Main.rand.NextFloat(0.1f, 0.96f);
			var noteFlame = new EvilMusicRemnant_FlameDust_Explosion
			{
				Velocity = new Vector2(0, 10f).RotatedBy(i / 120f * MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(56, 80),
				Scale = 14f * size,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Frame = Main.rand.Next(3),
				ai = [Main.rand.NextFloat(-0.8f, 0.8f)],
			};
			Ins.VFXManager.Add(noteFlame);
		}
		for (int i = 0; i < 6; i++)
		{
			float size = Main.rand.NextFloat(0.1f, 0.96f);
			var gore = new EvilMusicRemnant_Minion_Gore
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(12f)).RotatedByRandom(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(576, 666),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Style = Main.rand.Next(6),
				ai = [Main.rand.NextFloat(-0.8f, 0.8f)],
			};
			Ins.VFXManager.Add(gore);
		}
	}
}
