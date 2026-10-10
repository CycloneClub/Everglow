using Everglow.Commons.DataStructures;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Guard_Attack_Fist : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => ModAsset.Guard_Attack_Spear_Mod;

	public Vector2 NPCCenter = Vector2.Zero;
	public float Progress = 0f;
	public int[] DrawOffset1;
	public int[] DrawOffset2;
	public int[] DrawVertex = [42, 35, 34, 39, 46, 54];

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.Bullet);
		Projectile.friendly = true;
		Projectile.ownerHitCheck = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.tileCollide = false;
		Projectile.penetrate = 100;

		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPCCenter = new Vector2(Projectile.Center.X, Projectile.Center.Y);
		DrawOffset1 = [0, 0, 0, 0, 0, 0, 0];
		DrawOffset2 = [0, 0, 0, 0, 0, 0, 0];
		for (int i = 0; i < 7; i++)
		{
			int offset1 = Main.rand.Next(10) + 10;
			int offset2 = Main.rand.Next(20) + 10;
			DrawOffset1[i] = offset1;
			DrawOffset2[i] = offset2;
		}
		base.OnSpawn(source);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override bool PreAI()
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathF.PI / 2;
		int duration = 50;

		if (Projectile.timeLeft > duration)
		{
			Projectile.timeLeft = duration;
		}

		float progress = 1f - (float)Projectile.timeLeft / duration;
		Projectile.velocity.Y = 0; // 无竖直速度
		Projectile.velocity = Vector2.Normalize(Projectile.velocity);
		Projectile.Center = NPCCenter + 10 * Projectile.velocity * (1 - MathF.Pow(1f - progress, 4));
		Progress = progress;

		return false;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Vector2 vel = Projectile.velocity;
		// float moveX1 = Progress * 5;
		float moveX2 = Progress * 20f - 10f;

		List<Vertex2D> bars1 = new();
		Vector2 fx1Center = Projectile.Center + new Vector2(-20, 0) * vel; // 扇形圆心
		var fx1Color = new Color(241, 113, 19, 255); // 暗金色
		for (int i = 0; i < 7; i++)
		{
			int offset1 = DrawOffset1[i];
			int offset2 = DrawOffset2[i];
			float fixedAngle = (i - 3) * (float)Math.PI / 18;
			Vector2 vec = new Vector2(30, 0).RotatedBy(fixedAngle);
			Vector2 pos1 = new Vector2(offset1, 0f).RotatedBy(fixedAngle);
			Vector2 pos2 = new Vector2(offset2, 0f).RotatedBy(fixedAngle);
			vec.X *= vel.X;
			pos1.X *= vel.X;
			pos2.X *= vel.X;
			float alpha = Progress < 0.6f ? 255 : (1f - Progress) * 2.5f * 255;
			if (i < 2)
			{
				alpha *= i / 3f;
			}
			else if (i > 7 - 3)
			{
				alpha *= -(i - 7 + 1) / 3f;
			}
			fx1Color.A = (byte)alpha;
			bars1.Add(fx1Center + vec - pos1, fx1Color, new Vector3(i / 6f, 0.1f, 1));
			bars1.Add(fx1Center + vec + pos2, fx1Color, new Vector3(i / 6f, 0.9f, 1));
		}

		List<Vertex2D> bars2 = new();
		Vector2 fx2Center = Projectile.Center + new Vector2(moveX2 + 50f, 0) * vel;
		var fx2Color = new Color(255, 255, 255, 127); // 白色
		float fx2Length = -(1.3f - Progress) * 50f;
		int length = DrawVertex.Length;
		int count = length - 1;
		for (int i = 0; i < length; i++)
		{
			int offset = DrawVertex[i];
			float alpha = Progress < 0.6f ? 150 : (1f - Progress) * 2.5f * 150;
			float ratio = (float)i / count;
			if (i < 2)
			{
				alpha *= i / 3f;
			}
			else if (i > length - 3)
			{
				alpha *= -(i - length + 1) / 3f;
			}
			fx2Color.A = (byte)alpha;
			float len = fx2Length * ratio;
			bars2.Add(fx2Center + new Vector2(len, offset) * vel.X, fx2Color, new Vector3(ratio, 1, 1));
			bars2.Add(fx2Center + new Vector2(len, -offset) * vel.X, fx2Color, new Vector3(ratio, 0, 1));
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes["Test"].Apply();
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars1.ToArray(), 0, bars1.Count - 2);
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		return false;
	}
}
