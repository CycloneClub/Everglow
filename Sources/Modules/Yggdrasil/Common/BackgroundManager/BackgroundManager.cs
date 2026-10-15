using Everglow.Commons.DataStructures;

namespace Everglow.Yggdrasil.Common.BackgroundManager;

public class BackgroundManager
{
	public class WorldBackground
	{
		public float Layer = 2; // 绘制层,景深
		public int Priority = 0; // 同层下绘制优先级
		public Vector2 Center; // 中心坐标
		public Color Color; // 颜色
		public Rectangle DrawRectangle; // 绘制域
		public bool Active; // 是否绘制，优先级最高
		public Texture2D Texture; // 图片
		public float scale = 1; // 大小
	}

	public class BoardBackground : WorldBackground
	{
		public bool xClamp = false;
		public bool yClamp = true;
	}

	public class PointBackground : WorldBackground
	{
		public float Rotation = 0;
		public Vector2 Velocity;
		public float[] ai = new float[8];

		public void Update()
		{
		}

		public void SpecialDraw(SpriteBatch spriteBatch)
		{
		}
	}

	public static void DrawBG_RestrictXY(Texture2D tex, float depth, Vector2 anchorWorldPos, Color baseColor, int xWorldCoordMin, int xWorldCoordMax, int yWorldCoordMin, int yWorldCoordMax, bool xClamp = false, bool yClamp = true)
	{
		DrawBG_RestrictXY(tex, GetDrawFrame(tex, depth, anchorWorldPos), baseColor, xWorldCoordMin, xWorldCoordMax, yWorldCoordMin, yWorldCoordMax, xClamp, yClamp);
	}

	public static void DrawBG_RestrictY(Texture2D tex, float depth, Vector2 anchorWorldPos, Color baseColor, int yWorldCoordMin, int yWorldCoordMax, bool xClamp = false, bool yClamp = true)
	{
		DrawBG_RestrictY(tex, GetDrawFrame(tex, depth, anchorWorldPos), baseColor, yWorldCoordMin, yWorldCoordMax, xClamp, yClamp);
	}

	/// <summary>
	/// Draw a texture in the background layer.
	/// Auto rasterize the area that out of yWorldCoord Clamp (yWorldCoordMin, yWorldCoordMax).
	/// </summary>
	/// <param name="tex"></param>
	/// <param name="drawFrame"></param>
	/// <param name="baseColor"></param>
	/// <param name="yWorldCoordMin"></param>
	/// <param name="yWorldCoordMax"></param>
	/// <param name="xClamp"></param>
	/// <param name="yClamp"></param>
	public static void DrawBG_RestrictY(Texture2D tex, Rectangle drawFrame, Color baseColor, int yWorldCoordMin, int yWorldCoordMax, bool xClamp = false, bool yClamp = true)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect bg_wrapEffect = ModAsset.BackgroundXWarp.Value;
		if (xClamp && yClamp)
		{
			bg_wrapEffect = ModAsset.BackgroundXYClamp.Value;
		}

		if (xClamp && !yClamp)
		{
			bg_wrapEffect = ModAsset.BackgroundYWarp.Value;
		}

		if (!xClamp && !yClamp)
		{
			bg_wrapEffect = ModAsset.BackgroundXYWarp.Value;
		}

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		if (Main.LocalPlayer.gravDir == -1)
		{
			projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, 0, Main.screenHeight, 0, 1);
		}

		bg_wrapEffect.Parameters["uTransform"].SetValue(projection);
		bg_wrapEffect.Parameters["uTime"].SetValue(0.34f);
		bg_wrapEffect.CurrentTechnique.Passes[0].Apply();

		// 处理掉超出地图界限的部分
		int drawMaxY = Main.screenHeight;
		int drawMinY = 0;
		float yClampValueUp = 0f;
		float yClampValueDown = 1f;
		if (Main.screenPosition.Y + Main.screenHeight > yWorldCoordMax)
		{
			drawMaxY = yWorldCoordMax - (int)Main.screenPosition.Y;
			yClampValueDown = (float)drawMaxY / Main.screenHeight;
		}
		if (Main.screenPosition.Y < yWorldCoordMin)
		{
			drawMinY = yWorldCoordMin - (int)Main.screenPosition.Y;
			yClampValueUp = drawMinY / (float)Main.screenHeight;
		}

		var bg_vertices = new List<Vertex2D>
		{
			new Vertex2D(new Vector2(0, drawMinY), baseColor, new Vector3(drawFrame.X / (float)tex.Width, (drawFrame.Y + drawFrame.Height * yClampValueUp) / tex.Height, 0)),
			new Vertex2D(new Vector2(Main.screenWidth, drawMinY), baseColor, new Vector3((drawFrame.X + drawFrame.Width) / (float)tex.Width, (drawFrame.Y + drawFrame.Height * yClampValueUp) / tex.Height, 0)),
			new Vertex2D(new Vector2(0, drawMaxY), baseColor, new Vector3(drawFrame.X / (float)tex.Width, (drawFrame.Y + drawFrame.Height * yClampValueDown) / tex.Height, 0)),

			new Vertex2D(new Vector2(0, drawMaxY), baseColor, new Vector3(drawFrame.X / (float)tex.Width, (drawFrame.Y + drawFrame.Height * yClampValueDown) / tex.Height, 0)),
			new Vertex2D(new Vector2(Main.screenWidth, drawMinY), baseColor, new Vector3((drawFrame.X + drawFrame.Width) / (float)tex.Width, (drawFrame.Y + drawFrame.Height * yClampValueUp) / tex.Height, 0)),
			new Vertex2D(new Vector2(Main.screenWidth, drawMaxY), baseColor, new Vector3((drawFrame.X + drawFrame.Width) / (float)tex.Width, (drawFrame.Y + drawFrame.Height * yClampValueDown) / tex.Height, 0)),
		};
		if (bg_vertices.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bg_vertices.ToArray(), 0, 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public static void DrawBG_RestrictXY(Texture2D tex, Rectangle drawFrame, Color baseColor, int xWorldCoordMin, int xWorldCoordMax, int yWorldCoordMin, int yWorldCoordMax, bool xClamp = false, bool yClamp = true)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect bg_wrapEffect = ModAsset.BackgroundXWarp.Value;
		if (xClamp && yClamp)
		{
			bg_wrapEffect = ModAsset.BackgroundXYClamp.Value;
		}

		if (xClamp && !yClamp)
		{
			bg_wrapEffect = ModAsset.BackgroundYWarp.Value;
		}

		if (!xClamp && !yClamp)
		{
			bg_wrapEffect = ModAsset.BackgroundXYWarp.Value;
		}

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		if (Main.LocalPlayer.gravDir == -1)
		{
			projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, 0, Main.screenHeight, 0, 1);
		}

		bg_wrapEffect.Parameters["uTransform"].SetValue(projection);
		bg_wrapEffect.Parameters["uTime"].SetValue(0.34f);
		bg_wrapEffect.CurrentTechnique.Passes[0].Apply();

		// 处理掉超出地图界限的部分

		int drawMaxX = Main.screenWidth;
		int drawMinX = 0;
		float xClampValueUp = 0f;
		float xClampValueDown = 1f;
		if (Main.screenPosition.X + Main.screenWidth > xWorldCoordMax)
		{
			drawMaxX = xWorldCoordMax - (int)Main.screenPosition.X;
			xClampValueDown = (float)drawMaxX / Main.screenWidth;
		}
		if (Main.screenPosition.X < xWorldCoordMin)
		{
			drawMinX = xWorldCoordMin - (int)Main.screenPosition.X;
			xClampValueUp = drawMinX / (float)Main.screenWidth;
		}

		int drawMaxY = Main.screenHeight;
		int drawMinY = 0;
		float yClampValueUp = 0f;
		float yClampValueDown = 1f;
		if (Main.screenPosition.Y + Main.screenHeight > yWorldCoordMax)
		{
			drawMaxY = yWorldCoordMax - (int)Main.screenPosition.Y;
			yClampValueDown = (float)drawMaxY / Main.screenHeight;
		}
		if (Main.screenPosition.Y < yWorldCoordMin)
		{
			drawMinY = yWorldCoordMin - (int)Main.screenPosition.Y;
			yClampValueUp = drawMinY / (float)Main.screenHeight;
		}
		float xCoord0 = (drawFrame.X + drawFrame.Width * xClampValueUp) / tex.Width;
		float xCoord1 = (drawFrame.X + drawFrame.Width * xClampValueDown) / tex.Width;
		float yCoord0 = (drawFrame.Y + drawFrame.Height * yClampValueUp) / tex.Height;
		float yCoord1 = (drawFrame.Y + drawFrame.Height * yClampValueDown) / tex.Height;

		var bg_vertices = new List<Vertex2D>
		{
			new Vertex2D(new Vector2(drawMinX, drawMinY), baseColor, new Vector3(xCoord0, yCoord0, 0)),
			new Vertex2D(new Vector2(drawMaxX, drawMinY), baseColor,  new Vector3(xCoord1, yCoord0, 0)),
			new Vertex2D(new Vector2(drawMinX, drawMaxY), baseColor,  new Vector3(xCoord0, yCoord1, 0)),

			new Vertex2D(new Vector2(drawMinX, drawMaxY), baseColor, new Vector3(xCoord0, yCoord1, 0)),
			new Vertex2D(new Vector2(drawMaxX, drawMinY), baseColor, new Vector3(xCoord1, yCoord0, 0)),
			new Vertex2D(new Vector2(drawMaxX, drawMaxY), baseColor, new Vector3(xCoord1, yCoord1, 0)),
		};
		if (bg_vertices.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bg_vertices.ToArray(), 0, 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	/// <summary>
	/// Get the draw frame of texture by screenPos, depth(1 , ∞) and anchor world pos.
	/// </summary>
	/// <param name="texSize"></param>
	/// <param name="MoveStep"></param>
	/// <returns></returns>
	public static Rectangle GetDrawFrame(Texture2D texture, float depth, Vector2 anchorWorldPos)
	{
		float depthValue = 1 / depth;
		Vector2 texSize = texture.Size();
		Vector2 sampleTopleft = Vector2.Zero;
		Vector2 sampleCenter = sampleTopleft + texSize / 2;
		var screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
		Vector2 drawCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
		Vector2 deltaPos = drawCenter - anchorWorldPos;
		deltaPos *= depthValue;
		int rectangleX = (int)(sampleCenter.X - screenSize.X / 2f + deltaPos.X);
		int rectangleY = (int)(sampleCenter.Y - screenSize.Y / 2f + deltaPos.Y);

		return new Rectangle(rectangleX, rectangleY, (int)screenSize.X, (int)screenSize.Y);
	}

	public static void DrawWaterfallInBackground(Vector2 biomeCenter, float moveStep, Vector2 positionToTextureCenter, float Width, float Height, Color baseColor, int yWorldCoordMin, int yWorldCoordMax, Vector2 textureSize = default(Vector2), bool xClamp = false, bool yClamp = true)
	{
		Vector2 HalfScreenSize = new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
		Vector2 ScreenCenter = Main.screenPosition + HalfScreenSize;
		Vector2 deltaPos = ScreenCenter - biomeCenter;
		deltaPos *= moveStep;
		Vector2 DrawCenter = HalfScreenSize - deltaPos + positionToTextureCenter;
		if (xClamp)
		{
			if (yClamp)
			{
				DrawWaterfall(DrawCenter, Width, Height, baseColor, yWorldCoordMin, yWorldCoordMax);
			}
			else
			{
				Vector2 dCenter = DrawCenter;
				while (dCenter.Y > Main.screenHeight + 160)
				{
					dCenter.Y -= textureSize.Y;
				}
				while (dCenter.Y >= -Height && dCenter.Y <= Main.screenHeight + 160)
				{
					DrawWaterfall(dCenter, Width, Height, baseColor, yWorldCoordMin, yWorldCoordMax);
					dCenter.Y -= textureSize.Y;
				}

				dCenter = DrawCenter;
				if (dCenter.Y >= -Height && dCenter.Y <= Main.screenHeight + 160)
				{
					dCenter.Y += textureSize.Y;
				}

				while (dCenter.Y < -Height)
				{
					dCenter.Y += textureSize.Y;
				}
				while (dCenter.Y >= -Height && dCenter.Y <= Main.screenHeight + 160)
				{
					DrawWaterfall(dCenter, Width, Height, baseColor, yWorldCoordMin, yWorldCoordMax);
					dCenter.Y += textureSize.Y;
				}
			}
		}
		else if (yClamp)
		{
			Vector2 dCenter = DrawCenter;
			while (dCenter.X > Main.screenWidth + Width)
			{
				dCenter.X -= textureSize.X;
			}
			while (dCenter.X >= -Width && dCenter.X <= Main.screenWidth + Width)
			{
				DrawWaterfall(dCenter, Width, Height, baseColor, yWorldCoordMin, yWorldCoordMax);
				dCenter.X -= textureSize.X;
			}
			dCenter = DrawCenter;
			if (dCenter.X >= -Width && dCenter.X <= Main.screenWidth + Width)
			{
				dCenter.X += textureSize.X;
			}

			while (dCenter.X < -Width)
			{
				dCenter.X += textureSize.X;
			}
			while (dCenter.X >= -Width && dCenter.X <= Main.screenWidth + Width)
			{
				DrawWaterfall(dCenter, Width, Height, baseColor, yWorldCoordMin, yWorldCoordMax);
				dCenter.X += textureSize.X;
			}
		}
		else
		{
			Vector2 dCenter = DrawCenter;
			while (dCenter.X > Main.screenWidth + Width)
			{
				dCenter.X -= textureSize.X;
			}
			while (dCenter.X >= -Width && dCenter.X <= Main.screenWidth + Width)
			{
				Vector2 ddCenter = dCenter;
				while (ddCenter.Y > Main.screenHeight + 160)
				{
					ddCenter.Y -= textureSize.Y;
				}
				while (ddCenter.Y >= -Height && ddCenter.Y <= Main.screenHeight + 160)
				{
					DrawWaterfall(ddCenter, Width, Height, baseColor, yWorldCoordMin, yWorldCoordMax);
					ddCenter.Y -= textureSize.Y;
				}
				ddCenter = dCenter;
				if (ddCenter.Y >= -Height && ddCenter.Y <= Main.screenHeight + 160)
				{
					ddCenter.Y += textureSize.Y;
				}

				while (ddCenter.Y < -Height)
				{
					ddCenter.Y += textureSize.Y;
				}
				while (ddCenter.Y >= -Height && ddCenter.Y <= Main.screenHeight + 160)
				{
					ddCenter.Y += textureSize.Y;
					DrawWaterfall(ddCenter, Width, Height, baseColor, yWorldCoordMin, yWorldCoordMax);
				}
				dCenter.X -= textureSize.X;
			}
			dCenter = DrawCenter;
			if (dCenter.X >= -Width && dCenter.X <= Main.screenWidth + Width)
			{
				dCenter.X += textureSize.X;
			}

			while (dCenter.X < -Width)
			{
				dCenter.X += textureSize.X;
			}
			while (dCenter.X >= -Width && dCenter.X <= Main.screenWidth + Width)
			{
				Vector2 ddCenter = dCenter;
				while (ddCenter.Y > Main.screenHeight + 160)
				{
					ddCenter.Y -= textureSize.Y;
				}
				while (ddCenter.Y >= -Height && ddCenter.Y <= Main.screenHeight + 160)
				{
					DrawWaterfall(ddCenter, Width, Height, baseColor, yWorldCoordMin, yWorldCoordMax);
					ddCenter.Y -= textureSize.Y;
				}
				ddCenter = dCenter;
				if (ddCenter.Y >= -Height && ddCenter.Y <= Main.screenHeight + 160)
				{
					ddCenter.Y += textureSize.Y;
				}

				while (ddCenter.Y < -Height)
				{
					ddCenter.Y += textureSize.Y;
				}
				while (ddCenter.Y >= -Height && ddCenter.Y <= Main.screenHeight + 160)
				{
					ddCenter.Y += textureSize.Y;
					DrawWaterfall(ddCenter, Width, Height, baseColor, yWorldCoordMin, yWorldCoordMax);
				}
				dCenter.X += textureSize.X;
			}
		}
	}

	public static void DrawWaterfall(Vector2 drawCenter, float width, float height, Color baseColor, int yWorldCoordMin, int yWorldCoordMax)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		if (Main.LocalPlayer.gravDir == -1)
		{
			projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, 0, Main.screenHeight, 0, 1);
		}

		Effect bg_wrapEffect = ModAsset.BackgroundYWarp.Value;
		bg_wrapEffect.Parameters["uTransform"].SetValue(projection);
		bg_wrapEffect.Parameters["uTime"].SetValue(0.34f);
		bg_wrapEffect.CurrentTechnique.Passes[0].Apply();

		float Time = (float)-Main.timeForVisualEffects / 40f;
		var WaterFallVertex = new List<Vertex2D>();
		for (float y = 0; y < height; y += 10f)
		{
			float ColorAlpha = Math.Min(y / 100f, 1);
			if (height - y < 100)
			{
				ColorAlpha = Math.Max((height - y) / 100f, 0);
			}

			float WorldYCoordinate = drawCenter.Y + y + Main.screenPosition.Y;
			if (WorldYCoordinate > yWorldCoordMax || WorldYCoordinate < yWorldCoordMin)
			{
				ColorAlpha *= 0;
			}

			WaterFallVertex.Add(new Vertex2D(new Vector2(drawCenter.X + width / 2f, drawCenter.Y + y), baseColor * ColorAlpha, new Vector3(0, (float)Math.Pow(y, 0.6) / 10f + Time, 0)));
			WaterFallVertex.Add(new Vertex2D(new Vector2(drawCenter.X - width / 2f, drawCenter.Y + y), baseColor * ColorAlpha, new Vector3(1, (float)Math.Pow(y, 0.6) / 10f + Time, 0)));
		}
		if (WaterFallVertex.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.WaterFall.Value;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, WaterFallVertex.ToArray(), 0, WaterFallVertex.Count - 2);
		}

		var WaterFallVertexII = new List<Vertex2D>();
		for (float y = 0; y < height; y += 10f)
		{
			float ColorAlpha = Math.Min(y / 100f, 1);
			if (height - y < 100)
			{
				ColorAlpha = Math.Max((height - y) / 100f, 0);
			}

			float WorldYCoordinate = drawCenter.Y + y + Main.screenPosition.Y;
			if (WorldYCoordinate > yWorldCoordMax || WorldYCoordinate < yWorldCoordMin)
			{
				ColorAlpha *= 0;
			}

			WaterFallVertexII.Add(new Vertex2D(new Vector2(drawCenter.X + width / 2f, drawCenter.Y + y), baseColor * ColorAlpha * 1.63f, new Vector3(0, (float)Math.Pow(y, 0.4) / 10f + Time, 0)));
			WaterFallVertexII.Add(new Vertex2D(new Vector2(drawCenter.X - width / 2f, drawCenter.Y + y), baseColor * ColorAlpha * 1.63f, new Vector3(1, (float)Math.Pow(y, 0.4) / 10f + Time, 0)));
		}
		if (WaterFallVertexII.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.WaterFall.Value;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, WaterFallVertexII.ToArray(), 0, WaterFallVertexII.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}
}
