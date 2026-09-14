using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

public class VampireMat_Attack_Proj_Ball_In_AbsorbVortex2 : ModProjectile
{
	public int TargetPlayerIndex => (int)Projectile.ai[0];

	public int ParentNPCIndex => (int)Projectile.ai[1];

	public int Timer = 0;

	public float RotationValue = 0;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => ModAsset.VampireMat_Attack_Proj_Ball_In_AbsorbVortex_Mod;

	public struct SubProj
	{
		public Vector2 Position;
		public Vector2 Velocity;
		public bool Active;
		public float Scale;
		public float MaxTime;
		public float Timer;
	}

	public List<SubProj> SubProjs = new List<SubProj>();

	public List<SubProj> SubProjs_Style2 = new List<SubProj>();

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 1200;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.friendly = false;
		Projectile.hostile = true;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4096;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
	}

	private bool initialized;

	public override bool PreAI()
	{
		if (initialized)
		{
			return true;
		}
		initialized = true;
		for (int k = 1; k < 3; k++)
		{
			for (int j = 0; j < k * 10; j++)
			{
				var sproj = default(SubProj);
				sproj.Position = Projectile.Center;
				sproj.Velocity = new Vector2(0, -k * 320).RotatedBy(j / (k * 10f) * MathHelper.TwoPi);
				sproj.Active = true;
				sproj.Scale = 0f;
				sproj.MaxTime = 540;
				sproj.Timer = 0;
				SubProjs_Style2.Add(sproj);
			}
		}
		return true;
	}

	public override void AI()
	{
		if (Timer % 24 == 0 && Timer < 450)
		{
			var sproj = default(SubProj);
			sproj.Position = Projectile.Center;
			sproj.Velocity = new Vector2(0, -11).RotatedBy(-Timer * 0.02f);
			sproj.Active = true;
			sproj.Scale = 0.4f;
			sproj.MaxTime = 360;
			sproj.Timer = 0;
			SubProjs.Add(sproj);

			var sproj2 = default(SubProj);
			sproj2.Position = Projectile.Center;
			sproj2.Velocity = new Vector2(0, 11).RotatedBy(-Timer * 0.02f);
			sproj2.Active = true;
			sproj2.Scale = 0.4f;
			sproj2.MaxTime = 360;
			sproj2.Timer = 0;
			SubProjs.Add(sproj2);
		}
		Projectile.velocity *= 0;
		for (int i = SubProjs.Count - 1; i >= 0; i--)
		{
			var sp = SubProjs[i];
			Lighting.AddLight(sp.Position, new Vector3(1f, 0.1f, 0.2f) * sp.Scale);
			sp.Position += sp.Velocity;
			sp.Velocity = sp.Velocity.NormalizeSafe() * (4f + MathF.Sin(i / 3f * MathHelper.TwoPi + (float)Main.time * 0.03f + Projectile.whoAmI) * 0.3f * MathF.Cos(Projectile.timeLeft / 60f));
			sp.Timer++;
			if (sp.MaxTime - sp.Timer < 60)
			{
				sp.Scale *= 0.96f;
			}
			SubProjs[i] = sp;
			if (sp.Timer >= sp.MaxTime)
			{
				sp.Active = false;
				SubProjs.RemoveAt(i);
			}
		}

		for (int i = SubProjs_Style2.Count - 1; i >= 0; i--)
		{
			var sp = SubProjs_Style2[i];
			Lighting.AddLight(sp.Position, new Vector3(1f, 0.1f, 0.2f) * sp.Scale);
			int dir = -1;
			sp.Position = Projectile.Center + sp.Velocity.RotatedBy(RotationValue / MathF.Pow(sp.Velocity.Length(), 2) * dir * 2400f);
			sp.Timer++;
			if (sp.MaxTime - sp.Timer < 60)
			{
				sp.Scale *= 0.96f;
			}
			else
			{
				if (sp.Scale < 0.4f)
				{
					sp.Scale += 0.01f;
				}
			}
			SubProjs_Style2[i] = sp;
			if (sp.Timer >= sp.MaxTime)
			{
				sp.Active = false;
				SubProjs_Style2.RemoveAt(i);
			}
		}
		if (SubProjs.Count + SubProjs_Style2.Count <= 0)
		{
			Projectile.Kill();
		}
		Timer++;
		float rotSpeed = 1f;
		if (Timer < 60)
		{
			rotSpeed *= Timer / 60f;
		}
		if (540 - Timer < 60)
		{
			rotSpeed *= (540 - Timer) / 60f;
		}
		RotationValue += rotSpeed;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (var sp in SubProjs)
		{
			int x = (int)sp.Position.X;
			int y = (int)sp.Position.Y;
			Rectangle subHitbox = new Rectangle(x - 10, y - 10, 20, 20);
			if (subHitbox.Intersects(targetHitbox))
			{
				return true;
			}
		}
		foreach (var sp in SubProjs_Style2)
		{
			int x = (int)sp.Position.X;
			int y = (int)sp.Position.Y;
			Rectangle subHitbox = new Rectangle(x - 10, y - 10, 20, 20);
			if (subHitbox.Intersects(targetHitbox))
			{
				return true;
			}
		}
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		VampireMat.OnVampireMatHitPlayer(target, info.Damage);
		base.OnHitPlayer(target, info);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Reverse, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		foreach (var sp in SubProjs)
		{
			DrawSubProj(sp);
		}

		foreach (var sp in SubProjs_Style2)
		{
			DrawSubProj(sp);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawSubProj(SubProj sp)
	{
		Texture2D star = Commons.ModAsset.CrossStar.Value;
		Main.EntitySpriteDraw(star, sp.Position - Main.screenPosition, null, new Color(1f, 0.2f, 0.3f, 0f) * MathF.Min(sp.Scale, 0.5f), 0, star.Size() * 0.5f, sp.Scale * 2f, SpriteEffects.None, 0);
		Texture2D tex = ModAsset.VampireMat_Attack_Proj_Ball.Value;
		int frameNumber = (int)(TileUtils.GetFixedRandomNumber_SingleSeed(sp.GetHashCode(), 10) + Projectile.timeLeft / 12f) % 10;
		Rectangle frame = new Rectangle(0, 90 * frameNumber, 90, 90);
		Main.EntitySpriteDraw(tex, sp.Position - Main.screenPosition, frame, new Color(1f, 0.2f, 0.3f, 1f), 0, frame.Size() * 0.5f, sp.Scale, SpriteEffects.None, 0);
		frame.X += 90;
		Main.EntitySpriteDraw(tex, sp.Position - Main.screenPosition, frame, new Color(1f, 1f, 1f, 1f), 0, frame.Size() * 0.5f, sp.Scale, SpriteEffects.None, 0);
	}
}
