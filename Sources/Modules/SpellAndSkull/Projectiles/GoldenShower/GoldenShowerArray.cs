using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.SpellAndSkull.Common;

namespace Everglow.SpellAndSkull.Projectiles.GoldenShower;

public class GoldenShowerArray : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.tileCollide = false;
	}

	public override bool? CanCutTiles()
	{
		return false;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = Projectile.Center * 0.7f + (player.Center + new Vector2(player.direction * 22, 12 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d))) * 0.3f;
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.itemTime > 0 && player.HeldItem.type == ItemID.GoldenShower && player.active && !player.dead)
		{
			Projectile.timeLeft = player.itemTime + 60;
			if (projPower < 30)
			{
				projPower++;
			}
		}
		else
		{
			projPower--;
			if (projPower < 0)
			{
				Projectile.Kill();
			}
		}
		Player.CompositeArmStretchAmount PCAS = Player.CompositeArmStretchAmount.Full;

		player.SetCompositeArmFront(true, PCAS, (float)(-Math.Sin(Main.timeForVisualEffects / 18d) * 0.6 + 1.2) * -player.direction);
		Vector2 vTOMouse = Main.MouseWorld - player.Center;
		player.SetCompositeArmBack(true, PCAS, (float)(Math.Atan2(vTOMouse.Y, vTOMouse.X) - Math.PI / 2d));
		Projectile.rotation = player.fullRotation;

		ringPos = ringPos * 0.9f + new Vector2(-72 * player.direction, -24 * player.gravDir) * 0.1f;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		DrawMagicArray();
		return false;
	}

	internal int projPower = 0;
	internal Vector2 ringPos = Vector2.Zero;

	public void DrawMagicArray()
	{
		Player player = Main.player[Projectile.owner];
		float power = projPower / 30f;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect fade = ModAsset.GoldenShowerArray_shader.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		fade.Parameters["uTransform"].SetValue(model * projection);
		fade.Parameters["uTime"].SetValue((float)(Main.timeForVisualEffects * 0.01f));
		fade.CurrentTechnique.Passes[0].Apply();

		// 黑色底
		Color c0 = new Color(0, 0, 0, 55) * power;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int t = 0; t <= 30; t++)
		{
			Vector2 radius = new Vector2(0, -90).RotatedBy(t / 30d * MathHelper.TwoPi);
			bars.Add(new Vertex2D(player.Center + ringPos + radius, c0, new Vector3(t / 30f, 0, 0)));
			bars.Add(new Vertex2D(player.Center + ringPos + radius * 0.5f, Color.Transparent, new Vector3(t / 30f, 1, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Wave_full_black.Value;
		Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_flame_0.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// 黄色波纹外圈
		c0 = new Color(255, 210, 0, 0) * 0.15f * power;
		bars = new List<Vertex2D>();
		for (int t = 0; t <= 30; t++)
		{
			Vector2 radius = new Vector2(0, -90).RotatedBy(t / 30d * MathHelper.TwoPi);
			bars.Add(new Vertex2D(player.Center + ringPos + radius, c0, new Vector3(t / 30f, 0, 0)));
			bars.Add(new Vertex2D(player.Center + ringPos + radius * 0.5f, Color.Transparent, new Vector3(t / 30f, 1, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Wave_full.Value;
		Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_flame_0.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		// 黄色辐条
		bars = new List<Vertex2D>();
		float timeValue = (float)(Main.timeForVisualEffects * 0.03);
		for (int t = 0; t <= 12; t++)
		{
			Vector2 radius = new Vector2(0, -81).RotatedBy((t + 0.5) / 12d * MathHelper.TwoPi);
			Vector2 radiousT = radius.RotatedBy(MathHelper.PiOver2) / 12;

			bars.Add(new Vertex2D(player.Center + ringPos + radiousT, c0, new Vector3(timeValue + t * 0.7f, 0, 0)));
			bars.Add(new Vertex2D(player.Center + ringPos - radiousT, c0, new Vector3(timeValue + t * 0.7f, 1, 0)));
			bars.Add(new Vertex2D(player.Center + ringPos + radius + radiousT, c0 * 0, new Vector3(timeValue + t * 0.7f + 0.7f, 0, 0)));

			bars.Add(new Vertex2D(player.Center + ringPos + radius + radiousT, c0 * 0, new Vector3(timeValue + t * 0.7f + 0.7f, 0, 0)));
			bars.Add(new Vertex2D(player.Center + ringPos + radius - radiousT, c0 * 0, new Vector3(timeValue + t * 0.7f + 0.7f, 1, 0)));
			bars.Add(new Vertex2D(player.Center + ringPos - radiousT, c0, new Vector3(timeValue + t * 0.7f, 1, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);

		// 黑底瞳孔
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect eye = ModAsset.GoldenEye_shader.Value;
		eye.Parameters["uTransform"].SetValue(model * projection);
		eye.Parameters["uTime"].SetValue((float)(Main.timeForVisualEffects * 0.01f));
		eye.CurrentTechnique.Passes[0].Apply();

		bars = new List<Vertex2D>();
		c0 = new Color(0, 0, 0, 150) * power;
		for (int t = 0; t <= 8; t++)
		{
			Vector2 radius = new Vector2(0, -240);
			Vector2 radiousT = new Vector2(0, -projPower).RotatedBy(MathHelper.PiOver2);
			float value = (t - 4f) / 8f;
			float zValue = MathF.Cos(value * MathHelper.TwoPi) + 1;
			zValue /= 3f;
			bars.Add(new Vertex2D(player.Center + ringPos + radius * value + radiousT, c0, new Vector3(t / 7f, 0, zValue)));
			bars.Add(new Vertex2D(player.Center + ringPos + radius * value - radiousT, c0, new Vector3(t / 7f, 1, zValue)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0_black.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// 金色瞳孔
		bars = new List<Vertex2D>();
		c0 = new Color(255, 210, 0, 0) * 0.75f * power;
		for (int t = 0; t <= 8; t++)
		{
			Vector2 radius = new Vector2(0, -240);
			Vector2 radiousT = new Vector2(0, -projPower).RotatedBy(MathHelper.PiOver2);
			float value = (t - 4f) / 8f;
			float zValue = MathF.Cos(value * MathHelper.TwoPi) + 1;
			zValue /= 3f;
			zValue *= projPower / 30f;
			bars.Add(new Vertex2D(player.Center + ringPos + radius * value + radiousT, c0, new Vector3(t / 7f, 0, zValue)));
			bars.Add(new Vertex2D(player.Center + ringPos + radius * value - radiousT, c0, new Vector3(t / 7f, 1, zValue)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// 黑色瞳仁
		c0 = new Color(0, 0, 0, 255) * power;
		bars = new List<Vertex2D>();
		for (int t = 0; t <= 8; t++)
		{
			Vector2 radius = new Vector2(0, -105);
			Vector2 radiousT = new Vector2(0, -30).RotatedBy(MathHelper.PiOver2);
			float value = (t - 4f) / 8f;
			float zValue = MathF.Cos(value * MathHelper.TwoPi) + 1;
			zValue /= 4f;
			zValue *= projPower / 30f;
			bars.Add(new Vertex2D(player.Center + ringPos + radius * value + radiousT, c0, new Vector3(t / 7f, 0, zValue)));
			bars.Add(new Vertex2D(player.Center + ringPos + radius * value - radiousT, c0, new Vector3(t / 7f, 1, zValue)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0_black.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float power = projPower / 30f;
		Player player = Main.player[Projectile.owner];
		SpellAndSkullUtils.DrawTexCircle_Warp(spriteBatch, projPower * 3, projPower * 2.4f, new Color(1, 0.06f * power, 0f, 0f), player.Center + ringPos - Main.screenPosition, Commons.ModAsset.Trail_0.Value, (float)(Main.timeForVisualEffects * 0.02f));

		spriteBatch.End();
		spriteBatch.Begin(BlendState.AlphaBlend);
		Effect eye = ModAsset.GoldenEye_shader.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		eye.Parameters["uTransform"].SetValue(model * projection);
		eye.Parameters["uTime"].SetValue((float)(Main.timeForVisualEffects * 0.01f));
		eye.CurrentTechnique.Passes[0].Apply();

		List<Vertex2D> bars = new List<Vertex2D>();
		Color c0 = new Color(0, 0, 0, 255);
		for (int t = 0; t <= 8; t++)
		{
			Vector2 radius = new Vector2(0, -projPower * 8);
			Vector2 radiousT = new Vector2(0, -projPower).RotatedBy(MathHelper.PiOver2);
			float value = (t - 4f) / 8f;
			float zValue = MathF.Cos(value * MathHelper.TwoPi) + 1;
			zValue /= 3f;
			bars.Add(new Vertex2D(player.Center + ringPos + radius * value + radiousT, c0, new Vector3(t / 7f, 0, zValue)));
			bars.Add(new Vertex2D(player.Center + ringPos + radius * value - radiousT, c0, new Vector3(t / 7f, 1, zValue)));
		}

		spriteBatch.Draw(Commons.ModAsset.Trail_7_black.Value, bars, PrimitiveType.TriangleStrip);
	}
}
