using ReLogic.Content;

namespace Everglow.Commons.IIID
{
	public struct BloomParams
	{
		public float BlurRadius;
		public float BlurIntensity;
	}

	public struct ViewProjectionParams
	{
		public Matrix ViewTransform;
		public float FieldOfView;
		public float AspectRatio;
		public float ZNear;
		public float ZFar;
	}

	public struct ArtParameters
	{
		public bool EnableOuterEdge;
		public bool EnablePixelArt;
	}

	public class ModelRenderingPipeline : ModSystem
	{
		private RenderTarget2D fakeScreenTarget;
		private RenderTarget2D fakeScreenTargetSwap;
		private RenderTarget2D emissionTarget;
		private RenderTarget2D depthTarget;

		private RenderTarget2D bloomTargetSwap;
		private RenderTarget2D bloomTargetSwap1;
		private RenderTarget2D[] blurRenderTargets;

		private const int MAX_BLUR_LEVELS = 5;
		private int renderTargetSize = Main.dedServ
			? 0
			: Math.Max(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) / 2;

		// private RenderTarget2D m_albedoTarget;
		// private RenderTarget2D m_normalTarget;
		// private RenderTarget2D m_surfaceTarget;
		// private RenderTarget2D m_worldPosTarget;
		private List<ModelEntity> models;

		private ArtParameters artParams;
		private BloomParams bloomParams;
		private Matrix viewProjectionMatrix;
		private Vector4 zbufferParams;
		private Asset<Effect> gbufferPassEffect;
		private Asset<Effect> filtersEffect;
		private Asset<Effect> toneMapping;
		private Asset<Effect> concaveEdge;
		private Asset<Effect> pixelArt;
		private Asset<Effect> edge;

		public RenderTarget2D ModelTarget
		{
			get
			{
				return fakeScreenTarget;
			}
		}

		public override void OnModLoad()
		{
			gbufferPassEffect = ModAsset.GBufferPass;
			filtersEffect = ModAsset.Filters;
			toneMapping = ModAsset.ToneMapping;
			concaveEdge = ModAsset.ConcaveEdge;
			pixelArt = ModAsset.PixelArt;
			edge = ModAsset.Edge;

			if (!Main.dedServ)
			{
				Main.OnResolutionChanged += Main_OnResolutionChanged;
				Ins.MainThread.AddTask(() =>
				{
					fakeScreenTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, renderTargetSize, renderTargetSize, false,
						SurfaceFormat.HdrBlendable, DepthFormat.Depth24Stencil8, 1, RenderTargetUsage.PreserveContents);
					fakeScreenTargetSwap = new RenderTarget2D(Main.graphics.GraphicsDevice, renderTargetSize, renderTargetSize, false,
						SurfaceFormat.HdrBlendable, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
					emissionTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, renderTargetSize, renderTargetSize, false,
						SurfaceFormat.HdrBlendable, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
					depthTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, renderTargetSize, renderTargetSize, false,
						SurfaceFormat.Vector4, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);

					blurRenderTargets = new RenderTarget2D[MAX_BLUR_LEVELS];
					for (int i = 0; i < MAX_BLUR_LEVELS; i++)
					{
						blurRenderTargets[i] = new RenderTarget2D(Main.graphics.GraphicsDevice, renderTargetSize >> i,
							renderTargetSize >> i, false, SurfaceFormat.HdrBlendable, DepthFormat.None,
							1, RenderTargetUsage.PreserveContents);
					}

					bloomTargetSwap1 = new RenderTarget2D(Main.graphics.GraphicsDevice, renderTargetSize >> 1,
						renderTargetSize >> 1, false,
						SurfaceFormat.HdrBlendable, DepthFormat.None,
						1, RenderTargetUsage.PreserveContents);

					bloomTargetSwap = new RenderTarget2D(Main.graphics.GraphicsDevice, renderTargetSize >> (MAX_BLUR_LEVELS - 1),
						renderTargetSize >> (MAX_BLUR_LEVELS - 1), false,
						SurfaceFormat.HdrBlendable, DepthFormat.None,
						1, RenderTargetUsage.PreserveContents);
				});
			}

			models = new List<ModelEntity>();
			base.OnModLoad();
		}

		public override void OnModUnload()
		{
			base.OnModUnload();
		}

		private void Main_OnResolutionChanged(Vector2 obj)
		{
			// CreateRender(obj);
		}

		public void PushModelEntity(ModelEntity entity)
		{
			models.Add(entity);
		}

		public void BeginCapture(ViewProjectionParams viewProjectionParams, BloomParams bloomParams, ArtParameters artParams)
		{
			var projectionTransform = Matrix.CreatePerspectiveFieldOfView(
				viewProjectionParams.FieldOfView,
				viewProjectionParams.AspectRatio, viewProjectionParams.ZNear, viewProjectionParams.ZFar);
			this.viewProjectionMatrix = viewProjectionParams.ViewTransform * projectionTransform;
			this.bloomParams = bloomParams;

			float zc0 = 1.0f - viewProjectionParams.ZFar / viewProjectionParams.ZNear;
			float zc1 = viewProjectionParams.ZFar / viewProjectionParams.ZNear;
			this.zbufferParams = new Vector4(zc0, zc1, zc0 / viewProjectionParams.ZFar, zc1 / viewProjectionParams.ZFar);
			this.artParams = artParams;
			models.Clear();
		}

		public void EndCapture()
		{
			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;
			spriteBatch.End();

			if (!gbufferPassEffect.IsLoaded)
			{
				gbufferPassEffect.Wait();
			}
			if (!filtersEffect.IsLoaded)
			{
				filtersEffect.Wait();
			}
			if (!concaveEdge.IsLoaded)
			{
				concaveEdge.Wait();
			}
			if (!pixelArt.IsLoaded)
			{
				pixelArt.Wait();
			}
			if (!edge.IsLoaded)
			{
				edge.Wait();
			}

			Blit(Main.screenTarget, Main.screenTargetSwap, null, string.Empty);

			ShadingPass();

			BloomPass();
			ToneMappingPass();
			// ConcaveEdgePass();  //这两个调试用，别动
			// FinalBlend();      //这两个调试用，别动
			if (artParams.EnablePixelArt)
			{
				PixelArt();
			}
			if (artParams.EnableOuterEdge)
			{
				Edge();
			}

			graphicsDevice.SetRenderTarget(Main.screenTarget);
			spriteBatch.Begin();
			spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
		}

		private void ShadingPass()
		{
			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;

			// 绘制在具有深度缓冲的一个RT上，而不是原屏幕
			DepthStencilState dState = DepthStencilState.Default;
			RasterizerState rState = new RasterizerState();
			rState.CullMode = CullMode.CullClockwiseFace;
			rState.FillMode = FillMode.Solid;
			Main.graphics.GraphicsDevice.DepthStencilState = dState;
			Main.graphics.GraphicsDevice.RasterizerState = rState;

			// 切换到GBuffer Targets上
			var renderTargets = new RenderTargetBinding[3]
			{
				fakeScreenTarget,
				emissionTarget,
				depthTarget,
			};
			graphicsDevice.SetRenderTargets(renderTargets);
			graphicsDevice.Clear(
				ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil,
				Color.Transparent, 1.0f, 0);

			for (int i = 0; i < models.Count; i++)
			{
				DrawOneModel(i);
			}
		}

		private void DrawOneModel(int index)
		{
			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;

			var model = models[index];
			var vertices = model.Vertices;
			var gBufferShader = gbufferPassEffect.Value;

			gBufferShader.Parameters["uModel"].SetValue(model.ModelTransform);
			gBufferShader.Parameters["uViewProjection"].SetValue(viewProjectionMatrix);

			// 如果Model有非均匀缩放，就要用法线变换矩阵而不是Model矩阵
			gBufferShader.Parameters["uModelNormal"].SetValue(Matrix.Transpose(Matrix.Invert(model.ModelTransform)));
			gBufferShader.Parameters["uCameraPosition"].SetValue(new Vector3(0, 0, 1000));
			gBufferShader.Parameters["uLightDirection"].SetValue(Vector3.Normalize(new Vector3(0f, 1f, 1f)));
			gBufferShader.Parameters["uLightIntensity"].SetValue(new Vector3(1f, 1f, 1f) * 150);
			gBufferShader.Parameters["uNormalIntensity"].SetValue(-1f);

			graphicsDevice.Textures[0] = model.Texture;
			graphicsDevice.Textures[1] = model.NormalTexture;
			graphicsDevice.Textures[2] = model.MaterialTexture;
			graphicsDevice.Textures[3] = model.EmissionTexture;

			gBufferShader.CurrentTechnique.Passes["Forward_Lit"].Apply();
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(),
				0, vertices.Count / 3);
		}

		private void BloomPass()
		{
			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;

			// Blit into m_blurRenderTargets[0]
			graphicsDevice.SetRenderTarget(blurRenderTargets[0]);
			graphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.None,
				RasterizerState.CullNone);
			spriteBatch.Draw(emissionTarget, emissionTarget.Bounds, Color.White);
			spriteBatch.End();

			var filterEffect = filtersEffect.Value;
			for (int i = 0; i < MAX_BLUR_LEVELS - 1; i++)
			{
				filterEffect.Parameters["uInvImageSize"].SetValue(new Vector2(
					1.0f / blurRenderTargets[i].Width,
					1.0f / blurRenderTargets[i].Height));
				filterEffect.Parameters["uDelta"].SetValue(1.0f);
				Blit(blurRenderTargets[i], blurRenderTargets[i + 1], filterEffect, "Box", BlendState.Opaque,
					SamplerState.AnisotropicClamp);
			}

			for (int i = 0; i < 1; i++)
			{
				filterEffect.Parameters["uDelta"].SetValue(1.0f);
				filterEffect.Parameters["uHorizontal"].SetValue(true);
				Blit(blurRenderTargets[MAX_BLUR_LEVELS - 1], bloomTargetSwap, filterEffect, "GBlur", BlendState.Opaque,
					SamplerState.AnisotropicClamp);

				filterEffect.Parameters["uDelta"].SetValue(1.0f);
				filterEffect.Parameters["uHorizontal"].SetValue(false);
				Blit(bloomTargetSwap, blurRenderTargets[MAX_BLUR_LEVELS - 1], filterEffect, "GBlur", BlendState.Opaque,
					SamplerState.AnisotropicClamp);
			}

			for (int i = MAX_BLUR_LEVELS - 1; i > 0; i--)
			{
				filterEffect.Parameters["uInvImageSize"].SetValue(new Vector2(
					1.0f / blurRenderTargets[i].Width,
					1.0f / blurRenderTargets[i].Height));
				filterEffect.Parameters["uDelta"].SetValue(0.5f);
				Blit(blurRenderTargets[i], blurRenderTargets[i - 1], filterEffect, "Box", BlendState.Opaque,
					SamplerState.AnisotropicClamp);
			}

			// Draw to bloom renderTarget
		}

		private void ToneMappingPass()
		{
			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;

			// Save content of m_fakeScreenTarget
			var toneMappingShader = toneMapping.Value;
			graphicsDevice.SetRenderTarget(fakeScreenTargetSwap);
			graphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp,
				DepthStencilState.None,
				RasterizerState.CullNone);
			toneMappingShader.CurrentTechnique.Passes[1].Apply();
			spriteBatch.Draw(fakeScreenTarget, fakeScreenTarget.Bounds, Color.White);
			spriteBatch.Draw(emissionTarget, emissionTarget.Bounds, Color.White);
			spriteBatch.Draw(blurRenderTargets[0], blurRenderTargets[0].Bounds, Color.White);
			spriteBatch.End();
		}

		private void ConcaveEdgePass()
		{
			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;
			//// Save content of m_fakeScreenTarget
			// graphicsDevice.SetRenderTarget(m_blurRenderTargets[1]);
			// graphicsDevice.Clear(Color.Transparent);
			// spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
			//    DepthStencilState.None,
			//    RasterizerState.CullNone);
			// ConcaveEdgeEffect.CurrentTechnique.Passes["DownSample_Naive"].Apply();
			// int width = m_fakeScreenTargetSwap.Width;
			// int height = m_fakeScreenTargetSwap.Height;
			// spriteBatch.Draw(m_fakeScreenTargetSwap, m_blurRenderTargets[1].Bounds, Color.White);
			//// spriteBatch.Draw(m_emissionTarget, m_emissionTarget.Bounds, Color.White);
			// spriteBatch.End();
			var ConcaveEdgeEffect = concaveEdge.Value;
			graphicsDevice.Textures[1] = depthTarget;
			graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
			ConcaveEdgeEffect.Parameters["uBias"].SetValue(0.01f);
			ConcaveEdgeEffect.Parameters["_ZBufferParams"].SetValue(zbufferParams);
			if (artParams.EnableOuterEdge)
			{
				ConcaveEdgeEffect.Parameters["_EdgeColor"].SetValue(new Vector4(1f, 0f, 0f, 1f));
				Blit(fakeScreenTargetSwap, fakeScreenTarget, ConcaveEdgeEffect, "Edge_HighLight_Outer");
			}
			else
			{
				Blit(fakeScreenTargetSwap, fakeScreenTarget, ConcaveEdgeEffect, "Edge_HighLight_Inner");
			}
			Blit(fakeScreenTarget, fakeScreenTargetSwap, null, string.Empty);
		}

		private void FinalBlend()
		{
			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;

			var filterEffect = filtersEffect.Value;

			// Draw to m_fakeScreenTarget
			graphicsDevice.SetRenderTarget(fakeScreenTarget);
			graphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
				DepthStencilState.None,
				RasterizerState.CullNone);
			graphicsDevice.Textures[1] = blurRenderTargets[0];
			filterEffect.Parameters["uIntensity"].SetValue(0.1f);
			filterEffect.CurrentTechnique.Passes["Blend"].Apply();
			spriteBatch.Draw(fakeScreenTargetSwap, fakeScreenTarget.Bounds, Color.White);
			spriteBatch.End();
		}

		private void PixelArt()
		{
			Blit(fakeScreenTarget, fakeScreenTargetSwap, null, string.Empty);

			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;

			var PixelEffect = pixelArt.Value;

			// Draw to m_fakeScreenTarget
			graphicsDevice.SetRenderTarget(fakeScreenTarget);
			graphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
				DepthStencilState.None,
				RasterizerState.CullNone);
			PixelEffect.Parameters["RenderTargetSize"].SetValue(renderTargetSize);
			PixelEffect.Parameters["_PixelSize"].SetValue(1);
			PixelEffect.Parameters["_PixelRatio"].SetValue(1);
			PixelEffect.CurrentTechnique.Passes["Blend"].Apply();
			spriteBatch.Draw(fakeScreenTargetSwap, fakeScreenTarget.Bounds, Color.White);
			spriteBatch.End();
		}

		private void Edge()
		{
			Blit(fakeScreenTarget, fakeScreenTargetSwap, null, string.Empty);

			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;

			var EdgeEffect = edge.Value;

			// Draw to m_fakeScreenTarget
			graphicsDevice.SetRenderTarget(fakeScreenTarget);
			graphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
				DepthStencilState.None,
				RasterizerState.CullNone);
			EdgeEffect.Parameters["RenderTargetSize"].SetValue(renderTargetSize);
			EdgeEffect.CurrentTechnique.Passes["Edge"].Apply();
			spriteBatch.Draw(fakeScreenTargetSwap, fakeScreenTarget.Bounds, Color.White);
			spriteBatch.End();
		}

		private void Blit(RenderTarget2D rt1, RenderTarget2D rt2, Effect effect, string pass)
		{
			Blit(rt1, rt2, effect, pass,
				BlendState.Opaque);
		}

		private void Blit(RenderTarget2D rt1, RenderTarget2D rt2, Effect effect, string pass, BlendState blendState)
		{
			Blit(rt1, rt2, effect, pass,
				blendState, SamplerState.PointClamp);
		}

		private void Blit(RenderTarget2D rt1, RenderTarget2D rt2, Effect effect, string pass, BlendState blendState,
			SamplerState samplerState)
		{
			Blit(rt1, rt2, effect, pass,
				blendState, samplerState, DepthStencilState.None);
		}

		private void Blit(RenderTarget2D rt1, RenderTarget2D rt2, Effect effect, string pass, BlendState blendState,
			SamplerState samplerState, DepthStencilState depthStencilState)
		{
			Blit(rt1, rt2, effect, pass,
				blendState, samplerState, depthStencilState, RasterizerState.CullNone);
		}

		private void Blit(RenderTarget2D rt1, RenderTarget2D rt2, Effect effect, string pass,
			BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState,
			RasterizerState rasterizerState)
		{
			var graphicsDevice = Main.graphics.GraphicsDevice;
			var spriteBatch = Main.spriteBatch;
			graphicsDevice.SetRenderTarget(rt2);
			graphicsDevice.Clear(Color.Transparent);

			SpriteSortMode spriteSortMode = SpriteSortMode.Deferred;
			if (effect != null)
			{
				spriteSortMode = SpriteSortMode.Immediate;
			}
			spriteBatch.Begin(spriteSortMode, blendState, samplerState,
				depthStencilState,
				rasterizerState);
			if (effect != null)
			{
				effect.Parameters["uInvImageSize"].SetValue(new Vector2(
					1.0f / rt1.Width,
					1.0f / rt1.Height));
				effect.CurrentTechnique.Passes[pass].Apply();
			}
			spriteBatch.Draw(rt1, rt2.Bounds, Color.White);
			spriteBatch.End();
		}

		public override void PostUpdateProjectiles()
		{
			if (Main.mouseRight && Main.mouseRightRelease)
			{
				Player player = Main.LocalPlayer;
			}
			base.PostUpdateProjectiles();
		}
	}
}
