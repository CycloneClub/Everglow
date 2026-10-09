using Everglow.Commons.Enums;
using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Localization;

namespace Everglow.MEAC.NonTrueMeleeProj;

public class GoldShield : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 1200;
		Projectile.penetrate = -1;
	}
	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}
	public static Texture2D ShieldTexture;
	private RenderTarget2D BlackAreaSwap;
	private RenderTarget2D BlackAreaOrig;
	public Vector2 DrawSize => new Vector2(240, 200);
	public override void Load()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			Ins.HookManager.AddHook(CodeLayer.PostDrawDusts, RenderCanvasOfShield);
			Ins.MainThread.AddTask(() =>
			{
				AllocateRenderTarget(DrawSize);
			});
			Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
			{
				BlackAreaSwap?.Dispose();
				BlackAreaOrig?.Dispose();
				AllocateRenderTarget(size);
			}, "Realloc RenderTarget");
		}
	}
	private void AllocateRenderTarget(Vector2 size)
	{
		if (Ins.VisualQuality.High)
		{
			var gd = Main.instance.GraphicsDevice;
			BlackAreaSwap = new RenderTarget2D(gd, (int)size.X, (int)size.Y);
			BlackAreaOrig = new RenderTarget2D(gd, Main.screenWidth, Main.screenHeight);
		}
	}

	public void RenderCanvasOfShield()
	{
		if (ProjectileCount <= 0 || !Ins.VisualQuality.High)
		{
			return;
		}
		var cur = Main.screenTarget;
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;

		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		gd.SetRenderTarget(BlackAreaSwap);
		gd.Clear(Color.Transparent);

		var projection = Matrix.CreateOrthographicOffCenter(0, DrawSize.X, DrawSize.Y, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Main.GameViewMatrix.EffectMatrix;
		Effect effect = ModAsset.Null.Value;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		// 花纹
		float timeValue = (float)Main.time * 0.006f;
		Texture2D texPiece = ModAsset.GoldShieldScale_dark.Value;
		Vector2 drawPos = DrawSize / 2f;
		float count = 16;
		for (int i = -(int)count; i < count; i++)
		{
			float time2 = timeValue * 20;
			float phi = (time2 + i) / count;
			float phi2 = Math.Abs(MathF.Sin(phi * MathF.PI * 2 + MathHelper.PiOver2)) - 0.3f;
			phi = Math.Abs(MathF.Sin(phi * MathF.PI * 2)) - 0.3f;
			phi = Math.Max(0, phi);
			phi2 = Math.Max(0, phi2);
			sb.Draw(texPiece, drawPos + new Vector2((i + 0.5f) * 120 / count, MathF.Sin((i / count - 0.25f) * MathF.PI) * 80), null, Color.White, 0, texPiece.Size() / 2f, new Vector2(phi2, phi2 * 3f), SpriteEffects.None, 0);
			sb.Draw(texPiece, drawPos + new Vector2((i + 0.5f) * 120 / count, MathF.Sin((i / count + 0.75f) * MathF.PI) * 80), null, Color.White, 0, texPiece.Size() / 2f, new Vector2(phi, phi * 3f), SpriteEffects.None, 0);
		}
		ShieldTexture = BlackAreaSwap;
		sb.End();

		gd.SetRenderTarget(BlackAreaOrig);
		gd.Clear(Color.Transparent);
		sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Invert(Main.GameViewMatrix.TransformationMatrix)/*我不知道为什么,乘一个矩阵的逆就好了*/);

		sb.Draw(cur, Vector2.Zero, Color.White);
		sb.End();

		gd.SetRenderTarget(Main.screenTarget);
		gd.Clear(Color.Transparent);
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		sb.Draw(BlackAreaOrig, Vector2.Zero, Color.White);
		sb.End();
	}
	public void DrawWarp(VFXBatch spriteBatch)
	{
		// float WaveRange = 0.7f;
		////Texture2D BackG = ModContent.Request<Texture2D>("Everglow/MEAC/NonTrueMeleeProj/Black").Value;

		// float k0 = (float)Math.Sqrt(1200 - Projectile.timeLeft) / 6f;//画方波
		// if (k0 is < 1 and > 0)
		// {
		// k0 = Math.Max(k0 - 0.025f, 0);
		// float k1 = 1 - k0;
		// float k2 = k1 * k1;
		// float k3 = (float)Math.Sqrt(k1);
		// Vector2 DrawCen = Projectile.Center - Main.screenPosition;

		// DrawDoubleLine(spriteBatch, DrawCen + new Vector2(0, -k0 * 150) * WaveRange, DrawCen + new Vector2(k0 * 75, -k0 * 75) * WaveRange, new Color(1f * k3, 0.6f * k3, 0f, 0f), new Color(1f * k2, 0.7f * k2, 0f, 0f));
		// DrawDoubleLine(spriteBatch, DrawCen + new Vector2(k0 * 75, -k0 * 75) * WaveRange, DrawCen + new Vector2(k0 * 150, 0) * WaveRange, new Color(1f * k2, 0.7f * k2, 0f, 0f), new Color(1f * k3, 0.6f * k3, 0f, 0f));
		// DrawDoubleLine(spriteBatch, DrawCen + new Vector2(0, -k0 * 150) * WaveRange, DrawCen + new Vector2(-k0 * 75, -k0 * 75) * WaveRange, new Color(1f * k3, 0.6f * k3, 0f, 0f), new Color(1f * k2, 0.7f * k2, 0f, 0f));
		// DrawDoubleLine(spriteBatch, DrawCen + new Vector2(-k0 * 75, -k0 * 75) * WaveRange, DrawCen + new Vector2(-k0 * 150, 0) * WaveRange, new Color(1f * k2, 0.7f * k2, 0f, 0f), new Color(1f * k3, 0.6f * k3, 0f, 0f));

		// DrawDoubleLine(spriteBatch, DrawCen + new Vector2(0, k0 * 150) * WaveRange, DrawCen + new Vector2(k0 * 75, k0 * 75) * WaveRange, new Color(1f * k3, 0.6f * k3, 0f, 0f), new Color(1f * k2, 0.7f * k2, 0f, 0f));
		// DrawDoubleLine(spriteBatch, DrawCen + new Vector2(k0 * 75, k0 * 75) * WaveRange, DrawCen + new Vector2(k0 * 150, 0) * WaveRange, new Color(1f * k2, 0.7f * k2, 0f, 0f), new Color(1f * k3, 0.6f * k3, 0f, 0f));
		// DrawDoubleLine(spriteBatch, DrawCen + new Vector2(0, k0 * 150) * WaveRange, DrawCen + new Vector2(-k0 * 75, k0 * 75) * WaveRange, new Color(1f * k3, 0.6f * k3, 0f, 0f), new Color(1f * k2, 0.7f * k2, 0f, 0f));
		// DrawDoubleLine(spriteBatch, DrawCen + new Vector2(-k0 * 75, k0 * 75) * WaveRange, DrawCen + new Vector2(-k0 * 150, 0) * WaveRange, new Color(1f * k2, 0.7f * k2, 0f, 0f), new Color(1f * k3, 0.6f * k3, 0f, 0f));
		// }
	}
	public void DrawDoubleLine(VFXBatch spriteBatch, Vector2 StartPos, Vector2 EndPos, Color color1, Color color2)
	{
		Vector2 DrawCen = Projectile.Center - Main.screenPosition;
		float Wid = (Projectile.timeLeft - 1170) / 2f;
		Vector2 WidthS = Vector2.Normalize(StartPos - DrawCen).RotatedBy(Math.PI / 2d) * Wid;
		Vector2 WidthE = Vector2.Normalize(EndPos - DrawCen).RotatedBy(Math.PI / 2d) * Wid;
		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			vertex2Ds.Add(new Vertex2D(StartPos + WidthS, color1, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + WidthE, color2, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - WidthS, color1, new Vector3(0, 0, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + WidthE, color2, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - WidthE, color2, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - WidthS, color1, new Vector3(0, 0, 0)));
		}

		spriteBatch.Draw(TextureAssets.MagicPixel.Value, vertex2Ds, PrimitiveType.TriangleList);
	}
	public void DrawDoubleLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2)
	{
		float timeValue = (Projectile.timeLeft - 1170) / 2f;
		Vector2 width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * timeValue;
		var vertex2Ds = new List<Vertex2D>();

		vertex2Ds.Add(new Vertex2D(StartPos + width, color1, new Vector3(0, 0, 0)));
		vertex2Ds.Add(new Vertex2D(StartPos - width, color1, new Vector3(0, 0, 0)));

		vertex2Ds.Add(new Vertex2D(EndPos + width, color2, new Vector3(0, 0, 0)));
		vertex2Ds.Add(new Vertex2D(EndPos - width, color2, new Vector3(0, 0, 0)));


		Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);
	}
	public void DrawPost(Color color, int widthCount, float halfHeight, float initialPhase, Texture2D texture, Texture2D texture1 = null)
	{
		var vertex2Ds = new List<Vertex2D>();
		Vector2 DrawCen = Main.player[Projectile.owner].Center - Main.screenPosition;
		DrawCen += new Vector2(0, -10);
		float SPos = initialPhase;
		float R = widthCount;
		for (int x = -widthCount; x < widthCount; x++)
		{
			float y = (float)Math.Sqrt(R * R - x * x);
			float newy = (float)Math.Sqrt(R * R - (x + 1) * (x + 1));

			float r1 = (float)(Math.Acos(Math.Clamp(x / R, -1, 1)) / Math.PI);
			float r2 = (float)(Math.Acos(Math.Clamp((x + 1) / R, -1, 1)) / Math.PI);

			float deltaY = newy - y;
			float length = (float)Math.Sqrt(deltaY * deltaY + 1);
			const float scale = 0.19f;
			const float frequency = 0.3f;


			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale - 0.5f, halfHeight), color, new Vector3(r1 * frequency, 1, 0)));
			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale + 0.5f, halfHeight), color, new Vector3(r2 * frequency, 1, 0)));
			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale + 0.5f, -halfHeight), color, new Vector3(r2 * frequency, 0, 0)));

			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale + 0.5f, -halfHeight), color, new Vector3(r2 * frequency, 0, 0)));
			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale - 0.5f, halfHeight), color, new Vector3(r1 * frequency, 1, 0)));
			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale - 0.5f, -halfHeight), color, new Vector3(r1 * frequency, 0, 0)));
			SPos += length;
		}

		Main.graphics.GraphicsDevice.Textures[0] = texture;
		if (texture1 != null)
		{
			Main.graphics.GraphicsDevice.Textures[1] = texture1;
		}
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}
	public static int ProjectileCount = 0;
	public override bool PreAI()
	{
		ProjectileCount = 0;
		return base.PreAI();
	}
	public override void AI()
	{
		ProjectileCount++;
		if (Projectile.ai[0] > 0)
		{
			Projectile.ai[0]--;
		}
		else
		{
			Projectile.ai[0] = 0;
		}
		Projectile.Center = Main.player[Projectile.owner].Center;
		Projectile.hide = true;
		Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 0.8f, 0.6f, 0);
	}
	public override void OnKill(int timeLeft)
	{
		ProjectileCount--;
		base.OnKill(timeLeft);
	}
	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}
	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
		int LeftTime = 1200 - Projectile.timeLeft;
		float glowStrength = 0;
		float glowStrength2 = 0;

		if (Projectile.ai[0] > 0)
		{
			if (Projectile.ai[0] < 10)
			{
				glowStrength2 = (float)(-Math.Cos(Projectile.ai[0] / 5d * Math.PI) + 1) * 120f;
			}
		}


		if (LeftTime < 10)
		{
			glowStrength = (float)(-Math.Cos(LeftTime / 10d * Math.PI) + 1) * 120f;
		}
		else if (LeftTime < 40)
		{
			glowStrength = (float)(-Math.Cos((LeftTime + 75) / 30d * Math.PI) + 1) * 120f;
		}

		if (Projectile.timeLeft < 10)
		{
			glowStrength = (float)(-Math.Cos(Projectile.timeLeft / 10d * Math.PI) + 1) * 120f;
		}
		else if (Projectile.timeLeft < 40)
		{
			glowStrength = (float)(-Math.Cos((Projectile.timeLeft + 75) / 30d * Math.PI) + 1) * 120f;
		}
		if (glowStrength + glowStrength2 > 0)// 光效
		{
			for (int x = 0; x < glowStrength + glowStrength2; x++)
			{
				Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0, -12) + new Vector2(x / 20f).RotatedBy(x), null, new Color(2, 2, 2, 0), Projectile.rotation, new Vector2(tex.Width / 2f, tex.Height / 2f), 1, SpriteEffects.None, 0);
			}
		}
		if (Projectile.timeLeft <= 9 && Projectile.timeLeft >= 0 && Projectile.timeLeft % 3 == 0 && !Main.gamePaused)
		{
			for (int x = 0; x < 12; x++)
			{
				float X = (float)Math.Sqrt(Main.rand.NextFloat(0, 0.5f));
				float Y = (float)Math.Sqrt(Main.rand.NextFloat(0, 0.5f));
				var v0 = new Vector2(X * Math.Sign(Main.rand.NextFloat(-1, 1)) * 38, Y * Math.Sign(Main.rand.NextFloat(-1, 1)) * 38);
				Dust dust = Dust.NewDustDirect(Projectile.Center + v0 - new Vector2(4) + new Vector2(0, -12), 0, 0, DustID.GoldFlame, 0, 0, 0, default, Main.rand.NextFloat(0.8f, 2f));
				dust.noGravity = true;
			}
			for (int x = 0; x < 6; x++)
			{
				float X = (float)Math.Sqrt(Main.rand.NextFloat(0, 0.5f));
				float Y = (float)Math.Sqrt(Main.rand.NextFloat(0, 0.5f));
				var v0 = new Vector2(X * Math.Sign(Main.rand.NextFloat(-1, 1)) * 38, Y * Math.Sign(Main.rand.NextFloat(-1, 1)) * 38);
				Dust dust = Dust.NewDustDirect(Projectile.Center + v0 - new Vector2(4) + new Vector2(0, -12), 0, 0, ModContent.DustType<GoldShieldScale_dust>(), 0, 0, 0, default, Main.rand.NextFloat(0.8f, 1.3f));
				dust.noGravity = true;
				dust.velocity += new Vector2(v0.X * 0.1f, 0);
			}
		}
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0, -12), null, new Color(255, 255, 255, 0), Projectile.rotation, new Vector2(tex.Width / 2f, tex.Height / 2f), 1, SpriteEffects.None, 0);
		if (Ins.VisualQuality.High)
		{
			Effect Post = ModAsset.Post_xnb.Value;
			Texture2D shieldTexture = ShieldTexture;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Post.Parameters["uTime"].SetValue((float)Main.time * 0.0005f);
			Post.CurrentTechnique.Passes["Test3"].Apply();
			DrawPost(new Color(255, 175, 25, 0) * 0.25f, 200, 50, 1, ModAsset.GoldShieldMap.Value, Commons.ModAsset.Noise_flame_0.Value);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Post.Parameters["uTime"].SetValue((float)Main.time * 0.002f);
			Post.CurrentTechnique.Passes["Test"].Apply();
			if (shieldTexture != null)
			{
				DrawPost(new Color(255, 255, 255, 0), 200, 40, 1, shieldTexture);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
		else
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect Post = ModAsset.Post_xnb.Value;
			Post.Parameters["uTime"].SetValue((float)(Main.timeForVisualEffects * 0.003));
			Post.CurrentTechnique.Passes[0].Apply();

			Texture2D StoneSquire = ModAsset.GoldShieldGlowMap.Value;
			DrawPost(new Color(255, 255, 255, 0), 200, 50, 1, StoneSquire);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Post.CurrentTechnique.Passes[0].Apply();
			Texture2D StoneSquireD = ModAsset.GoldShieldDarkMap.Value;
			DrawPost(new Color(255, 255, 255, 155), 200, 50, 1, StoneSquireD);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}

		float WaveRange = 0.7f;

		float k0 = (float)Math.Sqrt(1200 - Projectile.timeLeft) / 6f; // 画方波
		if (k0 is < 1 and > 0)
		{
			k0 = Math.Max(k0 - 0.025f, 0);
			float k1 = 1 - k0;
			float k2 = k1 * k1;
			float k3 = (float)Math.Sqrt(k1);
			Vector2 DrawCen = Projectile.Center - Main.screenPosition;


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			float timeValueOfWidth = (Projectile.timeLeft - 1170) / MathF.Sqrt(8);
			Color publicC1 = new Color(0.3f * k3, 0.1f * k3, 0f, 0.8f);
			Color publicC2 = new Color(0.3f * k2, 0.1f * k2, 0f, 0.2f);

			Color publicC3 = new Color(1f * k3, 0.6f * k3, 0f, 0f);
			Color publicC4 = new Color(1f * k2, 0.7f * k2, 0f, 0f);

			DrawDoubleLine(DrawCen + new Vector2(0, -k0 * 150) * WaveRange + new Vector2(timeValueOfWidth), DrawCen + new Vector2(k0 * 75, -k0 * 75) * WaveRange, publicC1, publicC2);
			DrawDoubleLine(DrawCen + new Vector2(k0 * 75, -k0 * 75) * WaveRange, DrawCen + new Vector2(k0 * 150, 0) * WaveRange + new Vector2(timeValueOfWidth), publicC2, publicC1);
			DrawDoubleLine(DrawCen + new Vector2(0, -k0 * 150) * WaveRange + new Vector2(timeValueOfWidth, -timeValueOfWidth), DrawCen + new Vector2(-k0 * 75, -k0 * 75) * WaveRange, publicC1, publicC2);
			DrawDoubleLine(DrawCen + new Vector2(-k0 * 75, -k0 * 75) * WaveRange, DrawCen + new Vector2(-k0 * 150, 0) * WaveRange - new Vector2(timeValueOfWidth, -timeValueOfWidth), publicC2, publicC1);

			DrawDoubleLine(DrawCen + new Vector2(0, k0 * 150) * WaveRange + new Vector2(timeValueOfWidth, -timeValueOfWidth), DrawCen + new Vector2(k0 * 75, k0 * 75) * WaveRange, publicC1, publicC2);
			DrawDoubleLine(DrawCen + new Vector2(k0 * 75, k0 * 75) * WaveRange, DrawCen + new Vector2(k0 * 150, 0) * WaveRange - new Vector2(timeValueOfWidth, -timeValueOfWidth), publicC2, publicC1);
			DrawDoubleLine(DrawCen + new Vector2(0, k0 * 150) * WaveRange + new Vector2(timeValueOfWidth), DrawCen + new Vector2(-k0 * 75, k0 * 75) * WaveRange, publicC1, publicC2);
			DrawDoubleLine(DrawCen + new Vector2(-k0 * 75, k0 * 75) * WaveRange, DrawCen + new Vector2(-k0 * 150, 0) * WaveRange + new Vector2(timeValueOfWidth), publicC2, publicC1);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


			DrawDoubleLine(DrawCen + new Vector2(0, -k0 * 120) * WaveRange + new Vector2(timeValueOfWidth), DrawCen + new Vector2(k0 * 60, -k0 * 60) * WaveRange, publicC3, publicC4);
			DrawDoubleLine(DrawCen + new Vector2(k0 * 60, -k0 * 60) * WaveRange, DrawCen + new Vector2(k0 * 120, 0) * WaveRange + new Vector2(timeValueOfWidth), publicC4, publicC3);
			DrawDoubleLine(DrawCen + new Vector2(0, -k0 * 120) * WaveRange + new Vector2(timeValueOfWidth, -timeValueOfWidth), DrawCen + new Vector2(-k0 * 60, -k0 * 60) * WaveRange, publicC3, publicC4);
			DrawDoubleLine(DrawCen + new Vector2(-k0 * 60, -k0 * 60) * WaveRange, DrawCen + new Vector2(-k0 * 120, 0) * WaveRange - new Vector2(timeValueOfWidth, -timeValueOfWidth), publicC4, publicC3);

			DrawDoubleLine(DrawCen + new Vector2(0, k0 * 120) * WaveRange + new Vector2(timeValueOfWidth, -timeValueOfWidth), DrawCen + new Vector2(k0 * 60, k0 * 60) * WaveRange, publicC3, publicC4);
			DrawDoubleLine(DrawCen + new Vector2(k0 * 60, k0 * 60) * WaveRange, DrawCen + new Vector2(k0 * 120, 0) * WaveRange - new Vector2(timeValueOfWidth, -timeValueOfWidth), publicC4, publicC3);
			DrawDoubleLine(DrawCen + new Vector2(0, k0 * 120) * WaveRange + new Vector2(timeValueOfWidth), DrawCen + new Vector2(-k0 * 60, k0 * 60) * WaveRange, publicC3, publicC4);
			DrawDoubleLine(DrawCen + new Vector2(-k0 * 60, k0 * 60) * WaveRange, DrawCen + new Vector2(-k0 * 120, 0) * WaveRange + new Vector2(timeValueOfWidth), publicC4, publicC3);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
		return false;
	}
}

public class GoldShieldUIDrawer : ModSystem
{
	/*public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int ShieldBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
		if (ShieldBarIndex != -1)
		{
			
			layers.Insert(ShieldBarIndex, new LegacyGameInterfaceLayer(
				"EverglowMod: Shield Bar",
				delegate
				{
					
					ShieldBarDraw(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.UI)
			);
		}

	}*/

	public override void PostDrawInterface(SpriteBatch spriteBatch)
	{
		ShieldBarDraw(Main.spriteBatch);
	}
	public void ShieldBarDraw(SpriteBatch spriteBatch)
	{

		Player p = Main.LocalPlayer;

		if (p.GetModPlayer<GoldShieldPlayer>().HasShield)
		{


			if (Main.ResourceSetsManager.ActiveSet.DisplayedName == Language.GetTextValue("UI.HealthManaStyle_Default"))
			{
				p.GetModPlayer<GoldShieldPlayer>().ClassicDraw();
			}
			else if (Main.ResourceSetsManager.ActiveSet.DisplayedName == Language.GetTextValue("UI.HealthManaStyle_New"))
			{
				p.GetModPlayer<GoldShieldPlayer>().FancyDraw(new FancyClassicPlayerResourcesDisplaySet("New", "New", "FancyClassic", AssetRequestMode.ImmediateLoad));
			}
			else if (Main.ResourceSetsManager.ActiveSet.DisplayedName == Language.GetTextValue("UI.HealthManaStyle_HorizontalBars"))
			{
				p.GetModPlayer<GoldShieldPlayer>().HorizontalDraw(new HorizontalBarsPlayerResourcesDisplaySet("HorizontalBars", "HorizontalBars", "HorizontalBars", AssetRequestMode.ImmediateLoad));
			}
			else if (Main.ResourceSetsManager.ActiveSet.DisplayedName == Language.GetTextValue("UI.HealthManaStyle_NewWithText"))
			{
				p.GetModPlayer<GoldShieldPlayer>().FancyDraw(new FancyClassicPlayerResourcesDisplaySet("NewWithText", "NewWithText", "FancyClassic", AssetRequestMode.ImmediateLoad));
			}
			else if (Main.ResourceSetsManager.ActiveSet.DisplayedName == Language.GetTextValue("UI.HealthManaStyle_HorizontalBarsWithText"))
			{
				p.GetModPlayer<GoldShieldPlayer>().HorizontalDraw(new HorizontalBarsPlayerResourcesDisplaySet("HorizontalBarsWithText", "HorizontalBarsWithText", "HorizontalBars", AssetRequestMode.ImmediateLoad));
			}
			else if (Main.ResourceSetsManager.ActiveSet.DisplayedName == Language.GetTextValue("UI.HealthManaStyle_HorizontalBarsWithFullText"))
			{
				p.GetModPlayer<GoldShieldPlayer>().HorizontalDraw(new HorizontalBarsPlayerResourcesDisplaySet("HorizontalBarsWithFullText", "HorizontalBarsWithFullText", "HorizontalBars", AssetRequestMode.ImmediateLoad));
			}
		}
	}

	public class GoldShieldPlayer : ModPlayer
	{

		public float GoldShieldDurability;
		public bool Dodge;
		public bool HasShield;

		public override bool FreeDodge(Player.HurtInfo info)
		{
			if (Dodge)
			{
				Dodge = false;
				return true;
			}
			return base.FreeDodge(info);
		}

		public void ShieldDuration()
		{
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.active & proj.owner == Player.whoAmI & proj.type == ModContent.ProjectileType<GoldShield>())
				{
					HasShield = true;
					GoldShieldDurability = proj.ai[1];
					// GoldShieldDurability -= 0.1f;
					if (GoldShieldDurability <= 0)
					{
						GoldShieldDurability = 0;
					}
					proj.ai[1] = GoldShieldDurability;
					break;
				}
				else
				{
					HasShield = false;
					GoldShieldDurability = 0;
				}
			}
		}
		public override void PreUpdate()
		{
			ShieldDuration();
		}
		public override void PostUpdate()
		{
			ShieldDuration();
		}
		public void PreHurt(ref Player.HurtInfo info)
		{
			bool shieldsTookHit = false;
			foreach (Projectile proj in Main.projectile)
			{
				if (HasShield)
				{

					GoldShieldDurability = (int)proj.ai[1];
					if (GoldShieldDurability >= info.Damage)
					{
						Dodge = true;
					}
					int DamageBlocked = Math.Min(info.Damage, (int)GoldShieldDurability);
					GoldShieldDurability -= DamageBlocked;
					shieldsTookHit = true;
					info.Damage -= DamageBlocked;
					if (GoldShieldDurability <= 0)
					{
						GoldShieldDurability = 0;
					}
					if (shieldsTookHit && DamageBlocked != 0)
					{
						CombatText.NewText(new Rectangle((int)Player.Center.X - 10, (int)Player.Center.Y - 10, 20, 20), Color.Gold, DamageBlocked);
						proj.ai[0] = 10;
					}

					Main.player[proj.owner].immune = true;
					Main.player[proj.owner].immuneTime = 30;
					Main.player[proj.owner].noKnockback = true;
					proj.ai[1] = GoldShieldDurability;
					if (proj.ai[1] <= 0)
					{
						proj.ai[1] = 0;

						proj.timeLeft = 15;
					}
				}
			}
		}
		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			modifiers.ModifyHurtInfo += new Player.HurtModifiers.HurtInfoModifier(this.PreHurt);
			if (GoldShieldDurability > 0)
			{
				modifiers.DisableDust();
				modifiers.DisableSound();
			}
		}


		private int _lastHeartPanelIndex;
		private float _currentPlayerLife;
		private float _lifePerHeart;
		private int _playerLifeFruitCount;
		private int _lastHeartFillingIndex;
		private int _heartCountRow1;
		private int _heartCountRow2;
		private bool _drawText;

		#region ClassicDraw

		public void ClassicDraw()
		{


			SpriteBatch spriteBatch = Main.spriteBatch;
			Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);
			PlayerStatsSnapshot snapshot = new PlayerStatsSnapshot(Player);
			ClassicPlayerResourcesDisplaySet ing = new ClassicPlayerResourcesDisplaySet("Default", "Default");

			ing.UI_ScreenAnchorX = Main.screenWidth - 800;
			if (Player.ghost || Player.statLifeMax2 <= 0 || snapshot.AmountOfLifeHearts <= 0)
			{
				return;
			}

			int MaxGoldShieldDurability;
			MaxGoldShieldDurability = (int)(Player.statLifeMax * 0.6f);
			float UIDisplay_ShieldOnHeart;

			int HeartsNum = snapshot.AmountOfLifeHearts;
			UIDisplay_ShieldOnHeart = MaxGoldShieldDurability / (float)HeartsNum;

			/*	int num4 = (int)((float)Player.statLifeMax2 / UIDisplay_ShieldOnHeart);
				if (num4 >= 10)
					num4 = 10;

				string text = Lang.inter[0].Value + " " + (int)GoldShieldDurability + "/" + MaxGoldShieldDurability;
				Vector2 vector = FontAssets.MouseText.Value.MeasureString(text);
				if (!Player.ghost)
				{
					spriteBatch.DrawString(FontAssets.MouseText.Value, "盾量", new Vector2((float)(500 + 13 * num4) - vector.X * 0.5f + Main.screenWidth - 800, 6f), color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
					spriteBatch.DrawString(FontAssets.MouseText.Value, ((int)GoldShieldDurability+1) + "/" + MaxGoldShieldDurability, new Vector2((float)(500 + 13 * num4) + vector.X * 0.5f + Main.screenWidth - 800, 6f), color, 0f, new Vector2(FontAssets.MouseText.Value.MeasureString(Player.statLife + "/" + Player.statLifeMax2).X, 0f), 1f, SpriteEffects.None, 0f);
				}*/



			for (int i = 1; i < (int)((float)MaxGoldShieldDurability / UIDisplay_ShieldOnHeart) + 1; i++)
			{
				int num5 = 255;
				float num6 = 1f;
				bool flag = false;
				if ((float)GoldShieldDurability >= (float)i * UIDisplay_ShieldOnHeart)
				{
					num5 = 255;
					if ((float)GoldShieldDurability == (float)i * UIDisplay_ShieldOnHeart)
					{
						flag = true;
					}
				}
				else
				{
					float num7 = ((float)GoldShieldDurability - (float)(i - 1) * UIDisplay_ShieldOnHeart) / UIDisplay_ShieldOnHeart;
					num5 = (int)(30f + 225f * num7);
					if (num5 < 30)
					{
						num5 = 30;
					}

					num6 = num7 / 4f + 0.75f;
					if ((double)num6 < 0.75)
					{
						num6 = 0.75f;
					}

					if (num7 > 0f)
					{
						flag = true;
					}
				}

				if (flag)
				{
					num6 += Main.cursorScale - 1f;
				}

				int num8 = 0;
				int num9 = 0;
				if (i > 10)
				{
					num8 -= 260;
					num9 += 26;
				}



				int a = (int)((double)num5 * 0.9);
				if (!Player.ghost)
				{
					var heartTexture = ModAsset.ShieldHeart;


					float num2 = 1f / (float)HeartsNum;
					float _hpPercent = (float)GoldShieldDurability / MaxGoldShieldDurability;
					/*
					float t = 1f - fillPercent;
					*/
					float t = _hpPercent;
					float lerpValue = Utils.GetLerpValue(num2 * (float)i, num2 * (float)(i + 1), t, clamped: true);

					Rectangle value = heartTexture.Frame();
					int num3 = (int)((float)value.Width * (1f - lerpValue));

					value.Width -= num3;


					Vector2 position = new Vector2(Main.screenWidth - 312 + 26 * (i - 1) + num8 + UIDisplay_ShieldOnHeart + heartTexture.Width() / 2, 32f + heartTexture.Height() * (1 - num6) / 2f + num9 + heartTexture.Height() / 2);

					ResourceOverlayDrawContext drawContext = new ResourceOverlayDrawContext(snapshot, ing, i - 1, heartTexture)
					{
						position = position,
						source = value,
						color = new Color(num5, num5, num5, a),
						origin = heartTexture.Size() / 2f,
						scale = new Vector2(num6),
					};
					ResourceOverlayLoader.DrawResource(drawContext);
				}
			}
		}




		#endregion

		#region FancyDraw



		public void FancyDraw(FancyClassicPlayerResourcesDisplaySet Displayset)
		{

			SpriteBatch spriteBatch = Main.spriteBatch;
			PlayerStatsSnapshot playerStatsSnapshot = new PlayerStatsSnapshot(Player);
			int MaxGoldShieldDurability;
			MaxGoldShieldDurability = (int)(Player.statLifeMax * 0.6f);

			int HeartsNum = playerStatsSnapshot.AmountOfLifeHearts;
			_playerLifeFruitCount = playerStatsSnapshot.LifeFruitCount;
			_lifePerHeart = MaxGoldShieldDurability / (float)HeartsNum;
			_currentPlayerLife = playerStatsSnapshot.Life;

			/*
			_heartCountRow1 = Utils.Clamp((int)((float)playerStatsSnapshot.LifeMax / _lifePerHeart), 0, 10);
			_heartCountRow2 = Utils.Clamp((int)((float)(playerStatsSnapshot.LifeMax - 200) / _lifePerHeart), 0, 10);
			*/
			_heartCountRow1 = Utils.Clamp(playerStatsSnapshot.AmountOfLifeHearts, 0, 10);
			_heartCountRow2 = Utils.Clamp(playerStatsSnapshot.AmountOfLifeHearts - 10, 0, 10);
			int lastHeartFillingIndex = (int)((float)playerStatsSnapshot.Life / _lifePerHeart);
			_lastHeartFillingIndex = lastHeartFillingIndex;
			_lastHeartPanelIndex = _heartCountRow1 + _heartCountRow2 - 1;

			/*
			_starCount = (int)((float)playerStatsSnapshot.ManaMax / _manaPerStar);
			*/

			PlayerStatsSnapshot preparedSnapshot = playerStatsSnapshot;

			// Create a cached override for usages of 'default' to avoid copypasting.
			ResourceDrawSettings defaultResourceDrawSettings = default;
			defaultResourceDrawSettings.StatsSnapshot = preparedSnapshot;
			defaultResourceDrawSettings.DisplaySet = Displayset;
			;


			Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);

			if (Displayset.ConfigKey == "NewWithText")
			{
				_drawText = true;
			}
			else
			{
				_drawText = false;
			}

			Vector2 vector = new Vector2(Main.screenWidth - 300 + 4, 15f);
			if (_drawText)
			{
				vector.Y += 6f;
			}

			bool isHovered = false;

			ResourceDrawSettings resourceDrawSettings = defaultResourceDrawSettings;
			resourceDrawSettings.ElementCount = _heartCountRow1;
			resourceDrawSettings.ElementIndexOffset = 0;
			resourceDrawSettings.TopLeftAnchor = vector + new Vector2(15f, 15f);
			resourceDrawSettings.GetTextureMethod = HeartFillingDrawer;
			resourceDrawSettings.OffsetPerDraw = Vector2.UnitX * 2f;
			resourceDrawSettings.OffsetPerDrawByTexturePercentile = Vector2.UnitX;
			resourceDrawSettings.OffsetSpriteAnchor = new Vector2(11f, 11f);
			resourceDrawSettings.OffsetSpriteAnchorByTexturePercentile = Vector2.Zero;
			resourceDrawSettings.Draw(spriteBatch, ref isHovered);
			resourceDrawSettings = defaultResourceDrawSettings;
			resourceDrawSettings.ElementCount = _heartCountRow2;
			resourceDrawSettings.ElementIndexOffset = 10;
			resourceDrawSettings.TopLeftAnchor = vector + new Vector2(15f, 15f) + new Vector2(0f, 28f);
			resourceDrawSettings.GetTextureMethod = HeartFillingDrawer;
			resourceDrawSettings.OffsetPerDraw = Vector2.UnitX * 2f;
			resourceDrawSettings.OffsetPerDrawByTexturePercentile = Vector2.UnitX;
			resourceDrawSettings.OffsetSpriteAnchor = new Vector2(11f, 11f);
			resourceDrawSettings.OffsetSpriteAnchorByTexturePercentile = Vector2.Zero;
			resourceDrawSettings.Draw(spriteBatch, ref isHovered);
		}

		private void HeartFillingDrawer(int elementIndex, int firstElementIndex, int lastElementIndex, out Asset<Texture2D> sprite, out Vector2 offset, out float drawScale, out Rectangle? sourceRect)
		{


			sourceRect = null;
			offset = Vector2.Zero;
			sprite = ModAsset.ShieldHeart;
			PlayerStatsSnapshot playerStatsSnapshot = new PlayerStatsSnapshot(Player);
			int HeartsNum = playerStatsSnapshot.AmountOfLifeHearts;
			_playerLifeFruitCount = playerStatsSnapshot.LifeFruitCount;
			int MaxGoldShieldDurability;
			MaxGoldShieldDurability = (int)(Player.statLifeMax * 0.6f);

			float _hpPercent = (float)GoldShieldDurability / MaxGoldShieldDurability;
			offset = Vector2.Zero;
			float num = 1f;
			float num2 = 1f / (float)HeartsNum;

			/*
			float t = 1f - fillPercent;
			*/
			float t = _hpPercent;
			float lerpValue = Utils.GetLerpValue(num2 * (float)elementIndex, num2 * (float)(elementIndex + 1), t, clamped: true);

			/*
			num = 1f - lerpValue;
			*/
			num = lerpValue;
			drawScale = 1f;
			Rectangle value = sprite.Frame();
			int num3 = (int)((float)value.Width * (1f - num));

			value.Width -= num3;
			sourceRect = value;
		}

		#endregion FancyDraw

		#region HorizontalDraw
		public void HorizontalDraw(HorizontalBarsPlayerResourcesDisplaySet Displayset)
		{


			Asset<Texture2D> _panelMiddleHP = Main.Assets.Request<Texture2D>("Images\\UI\\PlayerResourceSets\\HorizontalBars\\HP_Panel_Middle", AssetRequestMode.ImmediateLoad);


			PlayerStatsSnapshot playerStatsSnapshot = new PlayerStatsSnapshot(Player);

			// Make drawing use the automatically-clamped AmountOf properties (#HealthManaAPI)
			/*
			_hpSegmentsCount = (int)((float)playerStatsSnapshot.LifeMax / playerStatsSnapshot.LifePerSegment);
			_mpSegmentsCount = (int)((float)playerStatsSnapshot.ManaMax / playerStatsSnapshot.ManaPerSegment);
			*/
			int _drawTextStyle;
			int MaxGoldShieldDurability;
			MaxGoldShieldDurability = (int)(Player.statLifeMax * 0.6f);

			if (Displayset.ConfigKey == "HorizontalBarsWithFullText")
			{
				_drawTextStyle = 2;
			}
			else if (Displayset.ConfigKey == "HorizontalBarsWithText")
			{
				_drawTextStyle = 1;
			}
			else
			{
				_drawTextStyle = 0;
			}

			int HeartsNum = playerStatsSnapshot.AmountOfLifeHearts;

			int _maxSegmentCount = 20;
			_playerLifeFruitCount = playerStatsSnapshot.LifeFruitCount;
			float _hpPercent = (float)GoldShieldDurability / MaxGoldShieldDurability;

			PlayerStatsSnapshot preparedSnapshot = playerStatsSnapshot;
			SpriteBatch spriteBatch = Main.spriteBatch;
			int num = 16;
			int num2 = 18;
			int num3 = Main.screenWidth - 300 - 22 + num;
			if (_drawTextStyle == 2)
			{
				num2 += 2;
			}
			else if (_drawTextStyle == 1)
			{
				num2 += 4;
			}

			Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);


			Vector2 vector = new Vector2(num3, num2);
			vector.X += (_maxSegmentCount - HeartsNum) * _panelMiddleHP.Width();
			bool isHovered = false;
			ResourceDrawSettings resourceDrawSettings = default(ResourceDrawSettings);


			resourceDrawSettings = default(ResourceDrawSettings);
			resourceDrawSettings.ElementCount = HeartsNum;
			resourceDrawSettings.ElementIndexOffset = 0;
			// Make the filling draw from right to left (#HealthManaAPI)
			resourceDrawSettings.TopLeftAnchor = vector + new Vector2(6f, 6f);
			resourceDrawSettings.GetTextureMethod = LifeFillingDrawer;
			resourceDrawSettings.OffsetPerDraw = new Vector2(ModAsset.Shield_Fill.Width(), 0f);
			resourceDrawSettings.OffsetPerDrawByTexturePercentile = Vector2.Zero;
			resourceDrawSettings.OffsetSpriteAnchor = Vector2.Zero;
			resourceDrawSettings.OffsetSpriteAnchorByTexturePercentile = Vector2.Zero;
			resourceDrawSettings.StatsSnapshot = preparedSnapshot;
			resourceDrawSettings.DisplaySet = Displayset;
			resourceDrawSettings.Draw(spriteBatch, ref isHovered);
		}


		private void LifeFillingDrawer(int elementIndex, int firstElementIndex, int lastElementIndex, out Asset<Texture2D> sprite, out Vector2 offset, out float drawScale, out Rectangle? sourceRect)
		{
			sprite = ModAsset.Shield_Fill;
			PlayerStatsSnapshot playerStatsSnapshot = new PlayerStatsSnapshot(Player);
			int HeartsNum = playerStatsSnapshot.AmountOfLifeHearts;
			_playerLifeFruitCount = playerStatsSnapshot.LifeFruitCount;
			int MaxGoldShieldDurability;
			MaxGoldShieldDurability = (int)(Player.statLifeMax * 0.6f);
			float _hpPercent = (float)GoldShieldDurability / MaxGoldShieldDurability;


			FillBarByValues(elementIndex, sprite, HeartsNum, _hpPercent, out offset, out drawScale, out sourceRect);

			// Make the bar fillings draw from right to left (#HealthManaAPI)
			int opposite = lastElementIndex - (elementIndex - firstElementIndex);
			int drawIndexOffset = opposite - elementIndex;
			offset.X += drawIndexOffset * sprite.Width();
		}

		public static void FillBarByValues(int elementIndex, Asset<Texture2D> sprite, int segmentsCount, float fillPercent, out Vector2 offset, out float drawScale, out Rectangle? sourceRect)
		{
			sourceRect = null;
			offset = Vector2.Zero;
			float num = 1f;
			float num2 = 1f / (float)segmentsCount;

			/*
			float t = 1f - fillPercent;
			*/
			float t = fillPercent;
			float lerpValue = Utils.GetLerpValue(num2 * (float)elementIndex, num2 * (float)(elementIndex + 1), t, clamped: true);

			/*
			num = 1f - lerpValue;
			*/
			num = lerpValue;
			drawScale = 1f;
			Rectangle value = sprite.Frame();
			int num3 = (int)((float)value.Width * (1f - num));
			offset.X += num3;
			value.X += num3;
			value.Width -= num3;
			sourceRect = value;
		}

		#endregion HorizontalDraw

	}
}
