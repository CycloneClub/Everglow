using Everglow.Commons.Coroutines;
using Everglow.Commons.Templates.Weapons;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon.FaelanternProj;

public class Fae : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 7;
	}

	public enum State
	{
		Idle,
		Homing,
		Charm,
	}

	public int state = (int)State.Idle;

	public override void SetCustomDefaults()
	{
		Projectile.width = 26;
		Projectile.height = 26;
		Projectile.netImportant = true;
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 45;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.gfxOffY = 7;

		TrailLength = 30;
		TrailColor = new Color(0.13f, 0.976f, 0.977f, 0f);
		TrailWidth = 20f;
		SelfLuminous = true;
		TrailTexture = Commons.ModAsset.Trail_2_thick.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_2_black.Value;
	}

	public override bool? CanDamage()
	{
		return false;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[1] = 0;
		state = (int)State.Idle;
	}

	private Projectile owner;
	private NPC target;

	public CoroutineManager _coroutineManager = new CoroutineManager();

	public override void Behaviors()
	{
		Lighting.AddLight(Projectile.Center, 0.13f, 0.976f, 0.977f);
		owner = Main.projectile[(int)Projectile.ai[0]];
		if (owner == null || !owner.active || owner.type != ModContent.ProjectileType<FaelanternProj>())
		{
			Projectile.Kill();
		}
		if (Timer % 3 == 0)
		{
			Projectile.frame++;
		}

		Projectile.timeLeft++;
		Projectile.spriteDirection = Projectile.velocity.X >= 0 ? -1 : 1;
		switch (state)
		{
			case (int)State.Idle:
				{
					Idle();
					break;
				}
			case (int)State.Homing:
				{
					Homing();
					break;
				}
			case (int)State.Charm:
				{
					Charm();
					break;
				}
		}
	}

	public void Idle()
	{
		var FaelanternProj = owner.ModProjectile as FaelanternProj;
		if (FaelanternProj is null)
		{
			return;
		}
		var pos = new Vector2(FaelanternProj.FaelanternSkeleton.Skeleton.FindBone("bone6").WorldX, FaelanternProj.FaelanternSkeleton.Skeleton.FindBone("bone6").WorldY);
		Vector2 ToPos = (pos - Projectile.Center).NormalizeSafe() * 0.5f;
		if (Projectile.Hitbox.Intersects(owner.Hitbox))
		{
			Projectile.velocity += new Vector2(-(float)Math.Sin(0.05f * Timer) / 2f, -(float)Math.Cos(0.04f * Timer) / 3f) * 0.5f;
		}
		else
		{
			Projectile.velocity += new Vector2(0, -(float)Math.Cos(0.04f * Timer)) * 0.25f;
			ToPos = (pos - Projectile.Center).NormalizeSafe() * 5f;
		}

		Projectile.velocity = Vector2.Lerp(Projectile.velocity, ToPos, 0.2f);

		if (Projectile.ai[1] != -1)
		{
			state = (int)State.Homing;
		}
	}

	public void Homing()
	{
		if (Projectile.ai[1] != -1)
		{
			target = Main.npc[(int)Projectile.ai[1]];
			if (!target.active || target == null)
			{
				Projectile.ai[1] = -1;
				state = (int)State.Idle;
			}
			else
			{
				Vector2 pos = target.Center + new Vector2(0, -(float)Math.Cos(0.04f * Timer));
				Vector2 ToPos = (pos - Projectile.Center).NormalizeSafe() * 5f;
				Projectile.velocity += new Vector2(0, -(float)Math.Cos(0.04f * Timer)) * 0.5f;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, ToPos, 0.2f);
				if (Projectile.Distance(target.Center) <= 50)
				{
					i = 60;
					state = (int)State.Charm;
				}
			}
		}
	}

	private int i = 60;

	public void Charm()
	{
		i--;
		Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(-(float)Math.Sin(0.05f * Timer), -(float)Math.Cos(0.04f * Timer) / 3f), 0.1f);
		if (i == 30)
		{
			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<FaeCharmRing>(), Projectile.damage / 2, 0f, Projectile.owner, 10);
			foreach (NPC npc in Main.npc)
			{
				if (Projectile.Distance(npc.Center) <= 150)
				{
					npc.AddBuff(BuffID.Confused, 300);
				}
			}
		}
		Projectile.ai[1] = -1;
		if (i == 0)
		{
			state = (int)State.Idle;
		}
	}

	public override void DrawSelf()
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		var origin = new Vector2(tex.Width / 2, tex.Height / 2);
		Rectangle sourceRec = tex.Frame(1, 7, 0, Projectile.frame % 7);
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, 70f), sourceRec, Color.White, 0f, origin, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			float value = index / (float)SmoothedOldPos.Count;
			Color color = Color.Lerp(new Color(0, 126, 255, 0), Color.Transparent, value);
			return color;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.5f;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		var size = new Vector2(Projectile.width, Projectile.height);
		float point = 0;
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.position + size, Projectile.position, 50f, ref point))
		{
			return true;
		}
		return false;
	}
}
