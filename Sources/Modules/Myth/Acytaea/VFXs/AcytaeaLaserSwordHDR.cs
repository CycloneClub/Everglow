using Everglow.Myth.Acytaea.Projectiles;
using Terraria;

namespace Everglow.Myth.Acytaea.VFXs;

public class AcytaeaLaserSwordHDRPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.Acytaea_None;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D dark = Commons.ModAsset.Noise_perlin.Value;
		Ins.Batch.BindTexture<Vertex2D>(dark);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

public class AcytaeaLaserSwordHDRPipeline2 : PostPipeline
{
	private RenderTarget2D acytaeaLaserSwordHDRScreen;
	private RenderTarget2D acytaeaLaserSwordHDRScreenSwap;
	private float UnstableValue = 0.125f;

	private static int ScreenWidth => Main.screenWidth;

	private static int ScreenHeight => Main.screenHeight;

	public override void Load()
	{
		Ins.MainThread.AddTask(() =>
		{
			AllocateRenderTarget();
		});
		Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
		{
			acytaeaLaserSwordHDRScreen?.Dispose();
			acytaeaLaserSwordHDRScreenSwap?.Dispose();
			AllocateRenderTarget();
		}, "Realloc RenderTarget");
		effect = ModAsset.AcytaeaHDR;
	}

	private void AllocateRenderTarget()
	{
		var gd = Main.instance.GraphicsDevice;
		acytaeaLaserSwordHDRScreen = new RenderTarget2D(gd, ScreenWidth, ScreenHeight, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		acytaeaLaserSwordHDRScreenSwap = new RenderTarget2D(gd, ScreenWidth, ScreenHeight, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
	}

	public override void Render(RenderTarget2D rt2D)
	{
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;
		var effect = this.effect.Value;

		sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

		gd.SetRenderTarget(acytaeaLaserSwordHDRScreen);
		effect.Parameters["uTransform"].SetValue(Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.CurrentTechnique.Passes["DarkCover"].Apply();
		sb.Draw(rt2D, Vector2.Zero, new Color(255, 255, 255, 255));

		sb.End();

		var cur = Ins.VFXManager.CurrentRenderTarget;
		Ins.VFXManager.SwapRenderTarget();
		gd.SetRenderTarget(Ins.VFXManager.CurrentRenderTarget);
		sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		effect.Parameters["uShade"].SetValue(rt2D);
		effect.CurrentTechnique.Passes["Vivid"].Apply();
		UnstableValue = (float)Utils.Lerp(UnstableValue, Main.rand.NextFloat(0.125f, 0.25f), 0.05f);
		sb.Draw(cur, Vector2.Zero, new Color(UnstableValue, 1, 1, 1));
		gd.BlendState = BlendState.AlphaBlend;
		sb.Draw(acytaeaLaserSwordHDRScreen, Vector2.Zero, new Color(255, 255, 255, 255));
		sb.End();
	}
}

[Pipeline(typeof(AcytaeaLaserSwordHDRPipeline), typeof(AcytaeaLaserSwordHDRPipeline2))]
public class AcytaeaLaserSwordHDREffect : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PreDrawFilter;

	public float timer;
	public float maxTime;
	public NPC owner;

	public AcytaeaLaserSwordHDREffect() { }

	public AcytaeaLaserSwordHDREffect(int maxTime, NPC owner)
	{
		this.maxTime = maxTime;
		this.owner = owner;
	}

	public override void Update()
	{
		timer++;
		if (owner == null || !owner.active)
		{
			timer = maxTime;
			Active = false;
		}
		if (timer >= maxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		if (Main.netMode == NetmodeID.Server)
		{
			return;
		}
		if (owner == null || !owner.active)
		{
			return;
		}
		Vector2 playerCenter = Main.LocalPlayer.Center;
		Vector2 position = owner.Center;
		float width = Main.screenWidth;
		float height = Main.screenHeight;
		float pocession = timer / maxTime;

		pocession -= 0.5f;
		pocession = MathF.Abs(pocession);

		pocession *= 2;
		pocession = 1 - pocession;

		pocession = Math.Clamp(pocession, 0, 0.5f);

		pocession *= Math.Clamp(2f - (playerCenter - position).Length() / 1000f, 0, 1);

		pocession = MathF.Sin(pocession * MathF.PI);
		Color background = Color.Lerp(new Color(160, 255, 255, 255), new Color(160, 222, 160, 255), pocession);
		Vector3 coord = new Vector3(new Vector2(0.5f), 0);
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Vector2.zeroVector, background, coord),
			new Vertex2D(new Vector2(0, height), background, coord),
			new Vertex2D(new Vector2(width, 0), background, coord),

			new Vertex2D(new Vector2(width, 0), background, coord),
			new Vertex2D(new Vector2(0, height), background, coord),
			new Vertex2D(new Vector2(width, height), background, coord),
		};
		Color lightDot = new Color(255, 255, 255, 0);
		foreach (Projectile p in Main.projectile)
		{
			if (p != null && p.active)
			{
				if (p.type == ModContent.ProjectileType<AcytaeaLaserSword>())
				{
					AcytaeaLaserSword acyLaser = p.ModProjectile as AcytaeaLaserSword;
					if (acyLaser != null)
					{
						Vector2 baseCenter = acyLaser.EndPos - Main.screenPosition;
						float size = 220f;

						for (int x = 0; x < 30; x++)
						{
							float coordX = x / 30f;
							float coordXNext = (x + 1) / 30f;
							float coordY = (float)(Main.timeForVisualEffects * 0.002) % 1f;
							bars.Add(baseCenter, lightDot, new Vector3((coordX + coordXNext) / 2f, coordY, 0));
							bars.Add(baseCenter + new Vector2(size, 0).RotatedBy(coordX * MathHelper.TwoPi), Color.Transparent, new Vector3(coordX, coordY, 0));
							bars.Add(baseCenter + new Vector2(size, 0).RotatedBy(coordXNext * MathHelper.TwoPi), Color.Transparent, new Vector3(coordXNext, coordY, 0));
						}
					}
				}
			}
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleList);
	}
}
