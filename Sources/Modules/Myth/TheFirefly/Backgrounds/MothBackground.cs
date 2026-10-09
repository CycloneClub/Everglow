using Everglow.Myth.TheFirefly.NPCs.Bosses;
using Everglow.Myth.TheFirefly.WorldGeneration;
using SubworldLibrary;

namespace Everglow.Myth.TheFirefly.Backgrounds;

public class MothBackground : ModSystem
{
	// 加了个环境光，但还要调整下不然看上去很怪
	public readonly Vector3 ambient = new Vector3(0.001f, 0.001f, 0.05f);

	public List<GHang> GPos = new List<GHang>();

	public List<GHang> GPosSec = new List<GHang>();

	private float backgroundAlpha = 0f;

	private float luminance = 1f; // 发光物亮度，boss战时变暗

	/// <summary>
	/// 初始化
	/// </summary>
	public override void OnModLoad()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			Ins.HookManager.AddHook(CodeLayer.PostDrawBG, DrawBackground);
			Terraria.Graphics.Light.On_TileLightScanner.GetTileLight += TileLightScanner_GetTileLight;
		}
	}

	/// <summary>
	/// 荧光悬挂物的数据结构
	/// </summary>
	public struct GHang
	{
		public Vector2 Pos;

		public float Length;

		public float Size;

		public int Type;

		public GHang(Vector2 pos, float length, float size, int type)
		{
			Pos = pos;
			Length = length;
			Size = size;
			Type = type;
		}
	}

	/// <summary>
	/// 环境光的钩子
	/// </summary>
	/// <param name="orig"> </param>
	/// <param name="self"> </param>
	/// <param name="x"> </param>
	/// <param name="y"> </param>
	/// <param name="outputColor"> </param>
	private void TileLightScanner_GetTileLight(Terraria.Graphics.Light.On_TileLightScanner.orig_GetTileLight orig, Terraria.Graphics.Light.TileLightScanner self, int x, int y, out Vector3 outputColor)
	{
		orig(self, x, y, out outputColor);
		outputColor += ambient;
	}

	public override void PostUpdateEverything()// 开启地下背景
	{
		const float increase = 0.02f;
		if (GlowingFlowerLandActive() && Main.BackgroundEnabled)
		{
			if (backgroundAlpha < 1)
			{
				backgroundAlpha += increase;
			}
			else
			{
				backgroundAlpha = 1;
				Ins.HookManager.Disable(TerrariaFunction.DrawBackground);
			}
		}
		else
		{
			if (backgroundAlpha > 0)
			{
				backgroundAlpha -= increase;
			}
			else
			{
				backgroundAlpha = 0;
			}
			Ins.HookManager.Enable(TerrariaFunction.DrawBackground);
		}
		if (NPC.CountNPCS(ModContent.NPCType<CorruptMoth>()) > 0)// 发光物体在boss战时变暗
		{
			luminance = MathHelper.Lerp(luminance, 0.1f, 0.02f);
		}
		else
		{
			luminance = MathHelper.Lerp(luminance, 1, 0.02f);
			if (luminance == 1)
			{
				CorruptMoth.CorruptMothNPC = null;
			}
		}
	}

	/// <summary>
	/// 判定是否开启地形
	/// </summary>
	/// <returns> </returns>
	public static bool BiomeActive()
	{
		return SubworldSystem.IsActive<MothWorld>();
	}

	/// <summary>
	/// 判定是否开启高级背景
	/// </summary>
	/// <returns> </returns>
	public static bool GlowingFlowerLandActive()
	{
		MothLand mothLand = ModContent.GetInstance<MothLand>();
		var BiomeCenter = new Vector2(mothLand.fireflyCenterX * 16, (mothLand.fireflyCenterY - 20) * 16); // 读取地形信息
		Vector2 v0 = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f - BiomeCenter; // 距离中心Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f
		v0.Y *= 1.35f;
		v0.X *= 0.9f; // 近似于椭圆形，所以xy坐标变换

		// TODO World
		return v0.Length() < 2200 && SubworldSystem.IsActive<MothWorld>();
	}

	/// <summary>
	/// 获取荧光悬挂物点位
	/// </summary>
	/// <param name="Shapepath"> </param>
	/// <exception cref="Exception"> </exception>
	public void GetGlowPos(string Shapepath)
	{
		var imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>("Everglow/Myth/TheFirefly/Backgrounds/" + Shapepath);
		imageData.ProcessPixelRows(accessor =>
		{
			for (int y = 0; y < accessor.Height; y++)
			{
				var pixelRow = accessor.GetRowSpan(y);
				for (int x = 0; x < pixelRow.Length; x++)
				{
					ref var pixel = ref pixelRow[x];
					if (pixel.R == 255)
					{
						GPos.Add(new GHang(new Vector2(x * 10, y * 10), pixel.G / 4f + 2, pixel.B / 255f + 0.5f, Main.rand.Next(5)));
					}
				}
			}
		});
	}

	/// <summary>
	/// 获取第二层荧光悬挂物点位
	/// </summary>
	/// <param name="Shapepath"> </param>
	/// <exception cref="Exception"> </exception>
	public void GetGlowPosSec(string Shapepath)
	{
		var imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>("Everglow/Myth/TheFirefly/Backgrounds/" + Shapepath);
		imageData.ProcessPixelRows(accessor =>
		{
			for (int y = 0; y < accessor.Height; y++)
			{
				var pixelRow = accessor.GetRowSpan(y);
				for (int x = 0; x < pixelRow.Length; x++)
				{
					ref var pixel = ref pixelRow[x];
					if (pixel.R == 255)
					{
						GPosSec.Add(new GHang(new Vector2(x * 10, y * 4.2f), pixel.G / 4f + 2, pixel.B / 255f + 0.5f, Main.rand.Next(5)));
					}
				}
			}
		});
	}

	/// <summary>
	/// 绘制荧光
	/// </summary>
	private void DrawGlow(Vector2 texSize, float MoveStep)
	{
		if (GPos.Count <= 1)
		{
			GetGlowPos("GlosPos");
		}

		MothLand mothLand = ModContent.GetInstance<MothLand>();
		Vector2 sampleTopleft = Vector2.Zero;
		Vector2 sampleCenter = sampleTopleft + texSize / 2;
		var screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
		Vector2 DCen = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
		Vector2 deltaPos = DCen - new Vector2((mothLand.fireflyCenterX + 34) * 16f, mothLand.fireflyCenterY * 16f);
		deltaPos *= MoveStep;
		Vector2 TexLT = sampleCenter - screenSize / 2f + deltaPos;
		float deltaY = DCen.Y - (mothLand.fireflyCenterY - 90) * 16f;
		deltaY *= MoveStep * 0.4f;
		if (GPos.Count > 0)
		{
			var texGlow = ModAsset.GlowHanging.Value;
			for (int x = 0; x < GPos.Count; x++)
			{
				Vector2 GP = GPos[x].Pos;
				GP.Y += deltaY / (GPos[x].Size + 1);
				Vector2 dPos = GP - TexLT + new Vector2(0, -194);
				var sRtTop = new Rectangle(GPos[x].Type * 20, 0, 20, 10);
				var sRtLine = new Rectangle(GPos[x].Type * 20, 10, 20, 20);
				var sRtDrop = new Rectangle(GPos[x].Type * 20, 65, 20, 35);
				float Dlength = (float)(GPos[x].Length + Math.Sin(Main.timeForVisualEffects / 128d + GPos[x].Pos.X / 70d + GPos[x].Pos.Y / 120d) * GPos[x].Length * 0.2f);
				Color color = GetLuminace(Color.White * backgroundAlpha);
				Main.spriteBatch.Draw(texGlow, dPos, sRtTop, color, 0, new Vector2(10, 0), GPos[x].Size, SpriteEffects.None, 0);
				Main.spriteBatch.Draw(texGlow, dPos + new Vector2(0, 10 + Dlength) * GPos[x].Size, sRtLine, color, 0, new Vector2(10, 10), new Vector2(1f, Dlength / 10f) * GPos[x].Size, SpriteEffects.None, 0);
				Main.spriteBatch.Draw(texGlow, dPos + new Vector2(0, 27.5f + Dlength * 2) * GPos[x].Size, sRtDrop, color, 0, new Vector2(10, 17.5f), GPos[x].Size, SpriteEffects.None, 0);
			}
		}
	}

	/// <summary>
	/// 绘制第二层荧光
	/// </summary>
	private void DrawGlowSec(Vector2 texSize, float MoveStep)
	{
		if (GPosSec.Count <= 1)
		{
			GetGlowPosSec("GlowHangingMiddlePosition");
		}

		MothLand mothLand = ModContent.GetInstance<MothLand>();
		Vector2 sampleTopleft = Vector2.Zero;
		Vector2 sampleCenter = sampleTopleft + texSize / 2;
		var screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
		Vector2 DCen = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
		Vector2 deltaPos = DCen - new Vector2((mothLand.fireflyCenterX + 34) * 16f, mothLand.fireflyCenterY * 16f);
		deltaPos *= MoveStep;
		Vector2 TexLT = sampleCenter - screenSize / 2f + deltaPos;
		float deltaY = DCen.Y - (mothLand.fireflyCenterY - 90) * 16f;
		deltaY *= MoveStep * 0.14f;
		if (GPosSec.Count > 0)
		{
			var texGlow = ModAsset.GlowHangingMiddle.Value;
			for (int x = 0; x < GPosSec.Count; x++)
			{
				Vector2 GP = GPosSec[x].Pos;
				GP.Y += deltaY / (GPosSec[x].Size + 1);
				Vector2 dPos = GP - TexLT + new Vector2(0, -180);
				var sRtTop = new Rectangle(GPosSec[x].Type * 10, 0, 10, 3);
				var sRtLine = new Rectangle(GPosSec[x].Type * 10, 3, 10, 5);
				var sRtDrop = new Rectangle(GPosSec[x].Type * 10, 10, 10, 15);
				Color color = GetLuminace(Color.White * backgroundAlpha);
				float Dlength = (float)(GPosSec[x].Length + Math.Sin(Main.timeForVisualEffects / 128d + GPosSec[x].Pos.X / 70d + GPosSec[x].Pos.Y / 120d) * GPosSec[x].Length * 0.2f);
				Main.spriteBatch.Draw(texGlow, dPos, sRtTop, color, 0, new Vector2(5, 0), GPosSec[x].Size, SpriteEffects.None, 0);
				Main.spriteBatch.Draw(texGlow, dPos + new Vector2(0, 3 + Dlength) * GPosSec[x].Size, sRtLine, color, 0, new Vector2(5, 2.5f), new Vector2(1f, Dlength / 2.5f) * GPosSec[x].Size, SpriteEffects.None, 0);
				Main.spriteBatch.Draw(texGlow, dPos + new Vector2(0, 10.5f + Dlength * 2) * GPosSec[x].Size, sRtDrop, color, 0, new Vector2(5, 7.5f), GPosSec[x].Size, SpriteEffects.None, 0);
			}
		}
	}

	/// <summary>
	/// 获取绘制矩形
	/// </summary>
	/// <param name="texSize"> </param>
	/// <param name="MoveStep"> </param>
	/// <param name="Correction">是否执行坐标校正分支；当前分支重复相同的计算，不改变返回矩形。</param>
	/// <returns> </returns>
	public Rectangle GetDrawRect(Vector2 texSize, float MoveStep, bool Correction)
	{
		MothLand mothLand = ModContent.GetInstance<MothLand>();
		Vector2 sampleTopleft = Vector2.Zero;
		Vector2 sampleCenter = sampleTopleft + texSize / 2;
		var screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
		Vector2 dCen = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
		Vector2 deltaPos = dCen - new Vector2((mothLand.fireflyCenterX + 34) * 16f, mothLand.fireflyCenterY * 16f);
		deltaPos *= MoveStep;

		int RX = (int)(sampleCenter.X - screenSize.X / 2f + deltaPos.X);
		int RY = (int)(sampleCenter.Y - screenSize.Y / 2f + deltaPos.Y);
		if (Correction)
		{
			RX = (int)(sampleCenter.X - screenSize.X / 2f + deltaPos.X);
			RY = (int)(sampleCenter.Y - screenSize.Y / 2f + deltaPos.Y);
		}
		return new Rectangle(RX, RY, (int)screenSize.X, (int)screenSize.Y);
	}

	private void DrawFarBG(Color baseColor)
	{
		var texSky = ModAsset.FireflySky.Value;
		var texFar = ModAsset.FireflyFar.Value;
		var texMiddle = ModAsset.FireflyMiddle.Value;
		var texMiddleGlow = ModAsset.FireflyMiddleGlow.Value;
		var texMidClose = ModAsset.FireflyMidClose.Value;
		var texClose = ModAsset.FireflyClose.Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Vector2 ScreenCen = new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
		Vector2 DrawPos = ScreenCen;
		Main.spriteBatch.Draw(texSky, DrawPos, GetDrawRect(texSky.Size(), 0, true), baseColor, 0,
			 ScreenCen, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texFar, DrawPos, GetDrawRect(texSky.Size(), 0.03f, true), baseColor, 0,
			 ScreenCen, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texMiddle, DrawPos, GetDrawRect(texSky.Size(), 0.17f, true), baseColor, 0,
			 ScreenCen, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texMiddleGlow, DrawPos, GetDrawRect(texSky.Size(), 0.17f, true), GetLuminace(baseColor), 0,
			 ScreenCen, 1f, SpriteEffects.None, 0);
		DrawGlowSec(texClose.Size(), 0.17f);
		Main.spriteBatch.Draw(texMidClose, DrawPos, GetDrawRect(texSky.Size(), 0.25f, false), baseColor, 0,
			 ScreenCen, 1f, SpriteEffects.None, 0);
		DrawGlow(texClose.Size(), 0.25f);
	}

	private void DrawCloseBG(Color baseColor)
	{
		var texClose = ModAsset.FireflyClose.Value;
		Rectangle targetSourceRect = GetDrawRect(texClose.Size(), 0.33f, false);
		targetSourceRect.Y -= 120;
		targetSourceRect.X += 150;
		Main.spriteBatch.Draw(texClose, Vector2.Zero, targetSourceRect, baseColor);
		Main.spriteBatch.End();

		// 便于合批，顶点绘制分开处置
		var texCloseII = ModAsset.FireflyClose2.Value;
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Rectangle rvcII = GetDrawRect(texCloseII.Size(), 0.57f, false);
		rvcII.Y += -300;
		rvcII.X += 300;
		float UpY = rvcII.Y / (float)texCloseII.Height;
		float DownY = (rvcII.Y + rvcII.Height) / (float)texCloseII.Height;
		var CloseII = new List<Vertex2D>
		{
			new Vertex2D(new Vector2(0, 0), baseColor, new Vector3(rvcII.X / (float)texCloseII.Width, UpY, 0)),
			new Vertex2D(new Vector2(Main.screenWidth, 0), baseColor, new Vector3((rvcII.X + rvcII.Width) / (float)texCloseII.Width, UpY, 0)),
			new Vertex2D(new Vector2(0, Main.screenHeight), baseColor, new Vector3(rvcII.X / (float)texCloseII.Width, DownY, 0)),

			new Vertex2D(new Vector2(0, Main.screenHeight), baseColor, new Vector3(rvcII.X / (float)texCloseII.Width, DownY, 0)),
			new Vertex2D(new Vector2(Main.screenWidth, 0), baseColor, new Vector3((rvcII.X + rvcII.Width) / (float)texCloseII.Width, UpY, 0)),
			new Vertex2D(new Vector2(Main.screenWidth, Main.screenHeight), baseColor, new Vector3((rvcII.X + rvcII.Width) / (float)texCloseII.Width, DownY, 0)),
		};
		Effect bgW = ModAsset.BackgroundWrap.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		bgW.Parameters["uTransform"].SetValue(projection);
		bgW.CurrentTechnique.Passes[0].Apply();

		if (CloseII.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = texCloseII;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, CloseII.ToArray(), 0, 2);
		}
		if (DownY > 1)
		{
			float DrawY = (1 - UpY) / (DownY - UpY) * Main.screenHeight - 5;
			Main.spriteBatch.Draw(texCloseII, new Rectangle(0, (int)DrawY, Main.screenWidth, Main.screenHeight - (int)DrawY), new Rectangle(1000, 1100, 1, 1), baseColor);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	/// <summary>
	/// 当然是绘制主体啦
	/// </summary>
	private void DrawBackground()
	{
		if (backgroundAlpha > 0)
		{
			Color baseColor = Color.White * backgroundAlpha;
			DrawFarBG(baseColor);
			DrawCloseBG(baseColor);
		}
		if (Main.spriteBatch.beginCalled)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
	}

	private Color GetLuminace(Color color)
	{
		if (luminance != 1)
		{
			byte a = color.A;
			color *= luminance;
			color.A = a;
		}
		return color;
	}
}
