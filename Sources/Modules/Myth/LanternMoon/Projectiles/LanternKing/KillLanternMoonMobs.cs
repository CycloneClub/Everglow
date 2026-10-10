using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;
using Everglow.Myth.LanternMoon.Projectiles.PerWave15;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class KillLanternMoonMobs : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public int Timer = 0;

	public float Range = 0;

	public static List<int> KillProjectileType = new List<int>();

	public override void SetDefaults()
	{
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.aiStyle = -1;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 120;
		Projectile.penetrate = -1;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	public override void AI()
	{
		EliminateProj();
		Timer++;
		Range = MathF.Pow(Timer / 90f, 8) * 3600f;
		if (Timer > 60 && Timer < 80)
		{
			LanternGhostKingSkyEffect skyEffect = ModContent.GetInstance<LanternGhostKingSkyEffect>();
			skyEffect.Active = true;
			skyEffect.SkyPower = (Timer - 60) / 20f;
		}
		base.AI();
	}

	public override void OnSpawn(IEntitySource source)
	{
		Wave(Projectile.Center);
		KillProjectileType.Add(ModContent.ProjectileType<BloodLanternGhost_PowerBall>());

		// KillProjectileType.Add(ModContent.ProjectileType<BloodLanternGhost_PowerBall_Explosion>());
		KillProjectileType.Add(ModContent.ProjectileType<CurseSpell>());

		// KillProjectileType.Add(ModContent.ProjectileType<CylindricalLantern_explosion>());
		KillProjectileType.Add(ModContent.ProjectileType<GreenFlameProj>());
		KillProjectileType.Add(ModContent.ProjectileType<CylindricalLantern_flame>());
		KillProjectileType.Add(ModContent.ProjectileType<GreenFlameSharpCrystal>());
		KillProjectileType.Add(ModContent.ProjectileType<LargeBloodLanternGhost_Matrix_Summon>());
		KillProjectileType.Add(ModContent.ProjectileType<LargeBloodLanternGhost_Minion>());
		KillProjectileType.Add(ModContent.ProjectileType<LargeBloodLanternGhost_Tentacles>());
		KillProjectileType.Add(ModContent.ProjectileType<RedpaperGiantAttackProj>());
		KillProjectileType.Add(ModContent.ProjectileType<ThunderSpell>());
		KillProjectileType.Add(ModContent.ProjectileType<ThunderSpell_AttachPlayer>());
		KillProjectileType.Add(ModContent.ProjectileType<ThunderSpell_Thunder>());
		KillProjectileType.Add(ModContent.ProjectileType<WitchingSpell>());
		KillProjectileType.Add(ModContent.ProjectileType<WizardLantern_Matrix_Curse>());
		KillProjectileType.Add(ModContent.ProjectileType<WizardLantern_Matrix_Thunder>());
		KillProjectileType.Add(ModContent.ProjectileType<WizardLantern_Matrix_Witching>());
	}

	public void Wave(Vector2 pos)
	{
		var redWave = new KillLanternMoonMobsWave
		{
			Position = pos,
			Speed = 60,
			Range = 0,
			Timer = 0,
			MaxTime = 120,
			SpeedDecay = 1f,
			Active = true,
			Visible = true,
		};
		Ins.VFXManager.Add(redWave);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	private void DrawWarpTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Vector2 center, Texture2D tex, float warpStrength, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		Color color = new Color(0f, 0f, 0f, 0f);
		int count = (int)(radius / 5f);
		count = Math.Min(count, 1000);
		for (int h = 0; h < count; h += 1)
		{
			float colorR = (h / radius * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);
			float color2R = ((h + 1) / radius * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);
			color = new Color(colorR, warpStrength, 0f, 0f);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
			if (Math.Abs(color2R - colorR) > 0.8f)
			{
				float midValue = (1f - colorR) / (float)(color2R + (1f - colorR));
				color.R = 255;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 1, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 0, 0)));
				color.R = 0;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 1, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 0, 0)));
			}
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch sb)
	{
		Texture2D t = Commons.ModAsset.NoiseWave.Value;
		float width = 15;
		if (Projectile.timeLeft < 15)
		{
			width = Projectile.timeLeft;
		}
		DrawWarpTexCircle_VFXBatch(sb, Range, width * 10, Projectile.Center - Main.screenPosition, t, Projectile.timeLeft / 12000f);
	}

	public void EliminateProj()
	{
		float maxDistance = Range;
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active)
			{
				if (KillProjectileType.Contains(proj.type))
				{
					if ((proj.Center - Projectile.Center).Length() < maxDistance)
					{
						proj.Kill();
					}
				}
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return CheckCenter(targetHitbox.TopLeft()) || CheckCenter(targetHitbox.TopRight()) || CheckCenter(targetHitbox.BottomLeft()) || CheckCenter(targetHitbox.BottomRight());
	}

	public bool CheckCenter(Vector2 pos)
	{
		return (pos - Projectile.Center).Length() < Range / 0.9f;
	}

	public override bool? CanHitNPC(NPC target)
	{
		bool flag = target.ModNPC is not null && target.ModNPC is LanternMoonNPC && target.type != ModContent.NPCType<LanternGhostKing>();
		if (flag)
		{
			target.value = 0;
		}
		return flag;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (target.life > 0)
		{
			target.active = false;
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<SphereLanternProj>(), 20, 1, Projectile.owner);
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		base.ModifyHitNPC(target, ref modifiers);
	}
}
