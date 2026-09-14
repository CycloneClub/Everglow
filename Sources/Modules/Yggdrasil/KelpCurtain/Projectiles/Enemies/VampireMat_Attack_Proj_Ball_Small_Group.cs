using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

public class VampireMat_Attack_Proj_Ball_Small_Group : ModProjectile
{
	public int TargetPlayerIndex => (int)Projectile.ai[0];

	public int ParentNPCIndex => (int)Projectile.ai[1];

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

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
	}

	private bool initialized;

	public override bool PreAI()
	{
		if (initialized)
		{
			return true;
		}
		initialized = true;
		for (int k = 0; k < 24; k++)
		{
			var sproj = default(SubProj);
			sproj.Position = Projectile.Center;
			sproj.Velocity = new Vector2(0, -4).RotatedBy(k * MathHelper.TwoPi / 24f);
			sproj.Active = true;
			sproj.Scale = 0.4f;
			sproj.MaxTime = 360;
			sproj.Timer = 0;
			SubProjs.Add(sproj);
		}
		return true;
	}

	public override void AI()
	{
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
		if (SubProjs.Count <= 0)
		{
			Projectile.Kill();
		}
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
		Texture2D tex = ModAsset.VampireMat_Attack_Proj_Ball.Value;
		foreach (var sp in SubProjs)
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
		Main.EntitySpriteDraw(star, sp.Position - Main.screenPosition, null, new Color(1f, 0.2f, 0.3f, 0f) * MathF.Min(sp.Scale, 0.5f), 0, star.Size() * 0.5f, sp.Scale, SpriteEffects.None, 0);
		Texture2D tex = ModAsset.VampireMat_Attack_Proj_Ball.Value;
		int frameNumber = (int)(TileUtils.GetFixedRandomNumber_SingleSeed(sp.GetHashCode(), 10) + Projectile.timeLeft / 12f) % 10;
		Rectangle frame = new Rectangle(0, 90 * frameNumber, 90, 90);
		Main.EntitySpriteDraw(tex, sp.Position - Main.screenPosition, frame, new Color(1f, 0.2f, 0.3f, 1f), 0, frame.Size() * 0.5f, sp.Scale, SpriteEffects.None, 0);
		frame.X += 90;
		Main.EntitySpriteDraw(tex, sp.Position - Main.screenPosition, frame, new Color(1f, 1f, 1f, 1f), 0, frame.Size() * 0.5f, sp.Scale, SpriteEffects.None, 0);
	}
}
