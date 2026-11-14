using Everglow.Commons.Enums;

namespace Everglow.Commons.MEAC;

internal class MEACManager : ILoadable
{
	private RenderTarget2D screen = null;
	private RenderTarget2D bloomTarget1 = null;
	private RenderTarget2D bloomTarget2 = null;

	private Effect screenWarp;

	public void Load(Mod mod)
	{
		if (!Main.dedServ)
		{
			var hookManager = Ins.HookManager;
			hookManager.AddHook(CodeLayer.ResolutionChanged, Main_OnResolutionChanged);
			hookManager.AddHook(CodeLayer.PreDrawFilter, FilterManager_EndCapture);
			screenWarp = ModAsset.ScreenWarp.Value;
			ModIns.OnUnload += () =>
			{
				Ins.MainThread.AddTask(() =>
				{
					bloomTarget1?.Dispose();
					bloomTarget2?.Dispose();
				});
			};
		}
	}

	private void CreateRender(Vector2 v)
	{
		bloomTarget1 = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)v.X / 3, (int)v.Y / 3);
		bloomTarget2 = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)v.X / 3, (int)v.Y / 3);
	}

	private void Main_OnResolutionChanged(Vector2 obj)
	{
		CreateRender(obj);
	}

	private bool HasBloom()
	{
		bool flag = false;
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active)
			{
				if (proj.ModProjectile is IBloomProjectile ModProj)
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	private void UseBloom(GraphicsDevice graphicsDevice)
	{
		if (HasBloom())
		{
			if (bloomTarget1 == null)
			{
				CreateRender(new Vector2(Main.screenWidth, Main.screenHeight));
			}

			Effect Bloom = ModAsset.Bloom1.Value;

			// 保存原图
			graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
			graphicsDevice.Clear(Color.Black);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
			Main.spriteBatch.End();

			// 在screen上绘制发光部分
			graphicsDevice.SetRenderTarget(screen);
			graphicsDevice.Clear(Color.Black);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			DrawBloom(Main.spriteBatch);
			Main.spriteBatch.End();

			// 取样
			graphicsDevice.SetRenderTarget(bloomTarget2);
			graphicsDevice.Clear(Color.Black);
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Bloom.CurrentTechnique.Passes[0].Apply(); // 取亮度超过m值的部分
			Bloom.Parameters["m"].SetValue(0.5f);
			Main.spriteBatch.Draw(screen, new Rectangle(0, 0, Main.screenWidth / 3, Main.screenHeight / 3), Color.White);
			Main.spriteBatch.End();

			// 处理
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Bloom.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / 3f);
			Bloom.Parameters["uRange"].SetValue(1.5f); // 范围
			Bloom.Parameters["uIntensity"].SetValue(0.97f); // 发光强度
			for (int i = 0; i < 2; i++)// 交替使用两个RenderTarget2D，进行多次模糊
			{
				Bloom.CurrentTechnique.Passes["GlurV"].Apply(); // 横向
				graphicsDevice.SetRenderTarget(bloomTarget1);
				graphicsDevice.Clear(Color.Black);
				Main.spriteBatch.Draw(bloomTarget2, Vector2.Zero, Color.White);

				Bloom.CurrentTechnique.Passes["GlurH"].Apply(); // 纵向
				graphicsDevice.SetRenderTarget(bloomTarget2);
				graphicsDevice.Clear(Color.Black);
				Main.spriteBatch.Draw(bloomTarget1, Vector2.Zero, Color.White);
			}
			Main.spriteBatch.End();

			graphicsDevice.SetRenderTarget(Main.screenTarget);
			graphicsDevice.Clear(Color.Black);

			// 叠加
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
			Main.spriteBatch.Draw(bloomTarget2, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
			Main.spriteBatch.End();
		}
	}

	private void FilterManager_EndCapture()
	{
		// 直接从RT池子里取
		var renderTargets = Ins.RenderTargetPool.GetRenderTarget2DArray(1);
		screen = renderTargets.Resource[0];

		GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;

		WarpStyle warpStyle = GetWarpStyle();

		// Debug code
		// Main.NewText(warpStyle);
		if (warpStyle == WarpStyle.Style1 || warpStyle == WarpStyle.Both)
		{
			GetOrig(graphicsDevice);
			graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
			graphicsDevice.Clear(Color.Black);
			DrawWarp();
			graphicsDevice.SetRenderTarget(Main.screenTarget);
			graphicsDevice.Clear(Color.Black);
			graphicsDevice.Textures[1] = Main.screenTargetSwap;
			graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			screenWarp.Parameters["strength"].SetValue(0.25f); // 扭曲程度
			screenWarp.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
			Main.spriteBatch.End();
		}
		if (warpStyle == WarpStyle.Style2 || warpStyle == WarpStyle.Both)
		{
			GetOrig(graphicsDevice);
			graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
			graphicsDevice.Clear(Color.Black);
			DrawWarp_Style2();
			graphicsDevice.SetRenderTarget(Main.screenTarget);
			graphicsDevice.Clear(new Color(0.5f, 0.5f, 0, 1));
			graphicsDevice.Textures[1] = Main.screenTargetSwap;
			graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			screenWarp.Parameters["strength"].SetValue(0.25f); // 扭曲程度
			screenWarp.CurrentTechnique.Passes[1].Apply();
			Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
			Main.spriteBatch.End();
		}
		UseBloom(graphicsDevice);
		screen = null;
		renderTargets.Release();
	}

	private void DrawBloom(SpriteBatch sb)// 发光层
	{
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active)
			{
				if (proj.ModProjectile is IBloomProjectile ModProj)
				{
					ModProj.DrawBloom();
				}
			}
		}
	}

	private enum WarpStyle
	{
		None,
		Style1,
		Style2,
		Both,
	}

	private WarpStyle GetWarpStyle()
	{
		WarpStyle result = WarpStyle.None;
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active)
			{
				if (proj.ModProjectile is IWarpProjectile)
				{
					if (result == WarpStyle.None)
					{
						result = WarpStyle.Style1;
					}
					else
					{
						result = WarpStyle.Both;
						break;
					}
				}
				if (proj.ModProjectile is IWarpProjectile_WarpStyle2)
				{
					if (result == WarpStyle.None)
					{
						result = WarpStyle.Style2;
					}
					else
					{
						result = WarpStyle.Both;
						break;
					}
				}
			}
		}
		return result;
	}

	private void DrawWarp()// 扭曲层
	{
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		Effect KEx = ModAsset.DrawWarp.Value;
		KEx.Parameters["uTransform"].SetValue(Main.Transform * Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		KEx.CurrentTechnique.Passes[0].Apply();
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active)
			{
				if (proj.ModProjectile is IWarpProjectile ModProj2)
				{
					ModProj2.DrawWarp(Ins.Batch);
				}
			}
		}
		Ins.Batch.End();
	}

	private void DrawWarp_Style2()// 扭曲层
	{
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		Effect ef = ModAsset.DrawWarp.Value;
		ef.Parameters["uTransform"].SetValue(Main.Transform * Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		ef.CurrentTechnique.Passes["Trail1"].Apply();
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active)
			{
				if (proj.ModProjectile is IWarpProjectile_WarpStyle2 modProj2)
				{
					modProj2.DrawWarp(Ins.Batch);
				}
			}
		}
		Ins.Batch.End();
	}

	/// <summary>
	/// 往screen上保存原图
	/// </summary>
	/// <param name="graphicsDevice"> </param>
	private void GetOrig(GraphicsDevice graphicsDevice)
	{
		graphicsDevice.SetRenderTarget(screen);
		graphicsDevice.Clear(Color.Black);
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
		Main.spriteBatch.End();
	}

	public void Unload()
	{
	}
}
