using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.IIID;
using Everglow.MEAC.PlanetBeFall.Projectiles.NonIIIDProj.GoldenCrack;
using Everglow.MEAC.PlanetBeFall.Projectiles.NonIIIDProj.PlanetBefallArray;
using Everglow.MEAC.PlanetBeFall.Projectiles.NonIIIDProj.PlanetBefallExplosion;
using Terraria.Graphics.Effects;

namespace Everglow.MEAC.PlanetBeFall;

internal class DrawIIID : ModSystem
{
	public float BloomIntensity = 1;
	private RenderTarget2D render;
	private RenderTarget2D screen;
	private RenderTarget2D bloom1;
	private RenderTarget2D bloom2;
	private RenderTarget2D goldenCrack;

	private Effect bloom;
	private Effect goldenCrackVFX;
	private Effect radial;

	public override void Load()
	{
		bloom = ModAsset.Bloom.Value;
		goldenCrackVFX = ModAsset.GoldenCrackEffect.Value;
		radial = ModAsset.Radial.Value;
		On_FilterManager.EndCapture += FilterManager_EndCapture; // 原版绘制场景的最后部分——滤镜。在这里运用render保证不会与原版冲突
		Main.OnResolutionChanged += Main_OnResolutionChanged;
	}

	public override void Unload()
	{
		On_FilterManager.EndCapture -= FilterManager_EndCapture;
		Main.OnResolutionChanged -= Main_OnResolutionChanged;
	}

	private void Main_OnResolutionChanged(Vector2 obj)// 在分辨率更改时，重建render防止某些bug
	{
		if (render != null)
		{
			CreateRender();
		}
	}

	private ViewProjectionParams viewProjectionParams = new ViewProjectionParams
	{
		ViewTransform = Matrix.Identity,
		FieldOfView = MathF.PI / 3f,
		AspectRatio = 1.0f,
		ZNear = 1f,
		ZFar = 2000,
	};

	private void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
	{
		GraphicsDevice gd = Main.instance.GraphicsDevice;
		SpriteBatch sb = Main.spriteBatch;
		if (render == null)
		{
			CreateRender();
		}
		if (gd == null)
		{
			return;
		}
		bool flag = false;
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active && (proj.type == ModContent.ProjectileType<GoldenCrack>() || proj.type == ModContent.ProjectileType<PlanetBefallArray>() || proj.type == ModContent.ProjectileType<IIIDProj>()))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			#region drawcrack
			bloom = ModAsset.Bloom.Value;
			gd.SetRenderTarget(Main.screenTargetSwap);
			gd.Clear(Color.Black);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
			Main.spriteBatch.End();

			// 在screen上绘制发光部分
			gd.SetRenderTarget(screen);
			gd.Clear(Color.Black);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.active && proj.type == ModContent.ProjectileType<GoldenCrack>())
				{
					Color c3 = Color.Gold;
					(proj.ModProjectile as GoldenCrack).PreDraw(ref c3);
				}
				if (proj.active && proj.type == ModContent.ProjectileType<PlanetBefallArray>())
				{
					(proj.ModProjectile as PlanetBefallArray).DrawBloom();
					if (BloomIntensity <= (proj.ModProjectile as PlanetBefallArray).BloomIntensity)
					{
						BloomIntensity = (proj.ModProjectile as PlanetBefallArray).BloomIntensity;
					}
					else
					{
						BloomIntensity = 1;
					}
				}
			}
			Main.spriteBatch.End();

			// 取样
			gd.SetRenderTarget(bloom2);
			gd.Clear(Color.Black);
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			bloom.CurrentTechnique.Passes[0].Apply(); // 取亮度超过m值的部分
			bloom.Parameters["m"].SetValue(0.5f);
			Main.spriteBatch.Draw(screen, new Rectangle(0, 0, Main.screenWidth / 3, Main.screenHeight / 3), Color.White);
			Main.spriteBatch.End();

			// 处理
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			bloom.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / 3f);
			bloom.Parameters["uRange"].SetValue(1.5f); // 范围
			bloom.Parameters["uIntensity"].SetValue(BloomIntensity); // 发光强度
			for (int i = 0; i < 2; i++)// 交替使用两个RenderTarget2D，进行多次模糊
			{
				bloom.CurrentTechnique.Passes["GlurV"].Apply(); // 横向
				gd.SetRenderTarget(bloom1);
				gd.Clear(Color.Black);
				Main.spriteBatch.Draw(bloom2, Vector2.Zero, Color.White);

				bloom.CurrentTechnique.Passes["GlurH"].Apply(); // 纵向
				gd.SetRenderTarget(bloom2);
				gd.Clear(Color.Black);
				Main.spriteBatch.Draw(bloom1, Vector2.Zero, Color.White);
			}
			Main.spriteBatch.End();

			gd.SetRenderTarget(Main.screenTarget);
			gd.Clear(Color.Black);

			// 叠加
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
			Main.spriteBatch.Draw(bloom2, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
			Main.spriteBatch.End();

			goldenCrackVFX = ModAsset.GoldenCrackEffect.Value;
			gd.SetRenderTarget(Main.screenTargetSwap);
			gd.Clear(Color.Black);
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
			sb.End();

			gd.SetRenderTarget(render);
			gd.Clear(Color.Black);
			sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.active && proj.type == ModContent.ProjectileType<GoldenCrack>())
				{
					Color c3 = Color.Gold;
					(proj.ModProjectile as GoldenCrack).PreDraw(ref c3);
				}
			}

			sb.End();

			gd.SetRenderTarget(Main.screenTarget);
			gd.Clear(Color.Black);
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
			sb.End();

			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			gd.Textures[1] = ModAsset.GoldenCrack.Value;
			goldenCrackVFX.CurrentTechnique.Passes["Tentacle"].Apply();
			goldenCrackVFX.Parameters["m"].SetValue(0.01f);
			goldenCrackVFX.Parameters["n"].SetValue(0.01f);
			sb.Draw(render, Vector2.Zero, Color.White);
			sb.End();
			#endregion

			Main.spriteBatch.Begin(0, BlendState.Additive);
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.active && proj.ModProjectile is IIIDProj)
				{
					(proj.ModProjectile as IIIDProj).DrawIIIDProj(viewProjectionParams);
				}
			}
			Main.spriteBatch.End();
		}

		#region drawexplosion
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active && proj.type == ModContent.ProjectileType<PlanetBefallExplosion>())
			{
				float BlurOffset = (proj.ModProjectile as PlanetBefallExplosion).BlurOffset;
				gd.SetRenderTarget(Main.screenTargetSwap);
				gd.Clear(Color.Black);
				sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
				sb.End();

				gd.SetRenderTarget(Main.screenTarget);
				radial = ModAsset.Radial.Value;
				sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				radial.CurrentTechnique.Passes["Blend"].Apply();
				radial.Parameters["_BlurOffset"].SetValue(BlurOffset / Math.Max(10, MathF.Pow((proj.Center - Main.LocalPlayer.Center).Length() / 100, 2)));
				radial.Parameters["_Center"].SetValue((proj.Center - Main.screenPosition) / new Vector2(Main.screenWidth, Main.screenHeight));
				sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
				sb.End();
			}
		}
		#endregion

		orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
	}

	public override void OnModLoad()
	{
		if (render != null)
		{
			CreateRender();
		}
		base.OnModLoad();
	}

	public void CreateRender()
	{
		render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
		screen = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
		goldenCrack = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
		bloom1 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 3, Main.screenHeight / 3);
		bloom2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 3, Main.screenHeight / 3);
	}
}
