namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class LanternSword_Slash : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public struct SlashEffect
	{
		public bool Active;

		public int Timer;

		public int MaxTime;

		public float Length;

		public float Rotation;

		public Vector2 Position;
	}

	public List<SlashEffect> SlashEffects = new List<SlashEffect>();

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 30;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;

		// Projectile.extraUpdates = 4;
	}

	public override void AI()
	{
		if (Timer % 5 == 0 && Timer <= 20)
		{
			SlashEffect sEffect = default(SlashEffect);
			sEffect.Active = true;
			sEffect.Timer = 0;
			sEffect.MaxTime = 30;
			sEffect.Position = Projectile.Center;
			sEffect.Rotation = MathF.Sin(Timer / 20f * MathHelper.TwoPi * 2 + MathHelper.PiOver2) * 0.7f + Main.rand.NextFloat(-0.2f, 0.2f);
			sEffect.Length = Main.rand.NextFloat(1.2f, 1.8f);
			SlashEffects.Add(sEffect);
		}
		for (int k = SlashEffects.Count - 1; k >= 0; k--)
		{
			var sEffect = SlashEffects[k];
			sEffect.Timer++;
			if (sEffect.Timer > sEffect.MaxTime)
			{
				sEffect.Active = false;
			}

			if (!sEffect.Active)
			{
				SlashEffects.RemoveAt(k);
			}
			else
			{
				SlashEffects[k] = sEffect;
			}
		}
		Timer++;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (var slash in SlashEffects)
		{
			if (slash.Active)
			{
				if (CollisionUtils.Intersect(slash.Position + new Vector2(0, -slash.Length).RotatedBy(slash.Rotation) * 50, slash.Position + new Vector2(0, slash.Length).RotatedBy(slash.Rotation) * 50, 16, targetHitbox.Top(), targetHitbox.Bottom(), targetHitbox.Width))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.Defense *= 2f;
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (target.life <= 0)
		{
			Player player = Main.player[Projectile.owner];
			float maxDis = 800;
			NPC nextTarget = null;
			foreach (var npc in Main.npc)
			{
				if (npc is not null && npc.active && npc != target && !npc.boss)
				{
					if (npc.type == target.type)
					{
						Vector2 toNPC = player.Center - npc.Center;
						if (toNPC.Length() < maxDis)
						{
							maxDis = toNPC.Length();
							nextTarget = npc;
						}
					}
				}
			}
			if (nextTarget is not null)
			{
				foreach (var proj in Main.projectile)
				{
					if (proj is not null && proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<LanternSword_Proj>())
					{
						LanternSword_Proj lSP = proj.ModProjectile as LanternSword_Proj;
						if (lSP is not null)
						{
							lSP.NextTarget = nextTarget;
							lSP.NextTargetAvailableTimer = 180;
						}
					}
				}
			}
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		foreach (var slash in SlashEffects)
		{
			if (slash.Active)
			{
				Texture2D star = Commons.ModAsset.StarSlash.Value;
				Texture2D star_dark = Commons.ModAsset.StarSlash_black.Value;
				var drawPos = slash.Position - Main.screenPosition;
				float timeLeft = (slash.MaxTime - slash.Timer) / (float)slash.MaxTime;
				float sizeMul = 2f;
				var drawColor = new Color(1f, 0.8f, 0.3f, 0);
				if (slash.Timer >= 3)
				{
					drawColor = new Color(1f, 0, 0, 0);
					sizeMul = 1f;
				}
				Main.EntitySpriteDraw(star_dark, drawPos, null, new Color(1f, 1f, 1f, 1f), slash.Rotation + MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(timeLeft, slash.Length) * sizeMul, SpriteEffects.None, 0);
				Main.EntitySpriteDraw(star, drawPos, null, drawColor, slash.Rotation + MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(timeLeft, slash.Length) * sizeMul, SpriteEffects.None, 0);
			}
		}
		return false;
	}
}
