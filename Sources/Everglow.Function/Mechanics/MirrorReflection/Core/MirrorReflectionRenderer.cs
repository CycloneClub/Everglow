using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.MirrorReflection.Core;

/// <summary>
/// 客户端倒影绘制器：处理投影、法线扭曲和模糊，并将结果裁剪到墙格。
/// 接收组织者提供的法线纹理和渲染目标；只缓存高斯权重，不管理捕获或纹理生命周期。
/// </summary>
internal sealed class MirrorReflectionRenderer
{
	private float cachedSigma = float.NaN;
	private int cachedSampleCount;
	private float[] blurWeights;

	public void ProcessForeground(Texture2D foreground, RenderTarget2D projected, RenderTarget2D processed, MirrorReflectionDefinition definition, Texture2D normalMap, int sampleCount)
	{
		Player viewer = Main.LocalPlayer;
		// 整张前景使用同一个观察基准，不按对象或镜墙拼接范围分别翻转。
		Vector2 anchor = viewer.Bottom + new Vector2(0, viewer.gfxOffY) - Main.screenPosition;
		Matrix projection = CreateScreenProjection(anchor, definition.Offset, definition.ReflectionScale,
			definition.FlipHorizontally, definition.Angles, definition.Distance, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.GraphicsDevice.SetRenderTarget(projected);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp,
			DepthStencilState.None, RasterizerState.CullNone, null, projection);
		try
		{
			Main.spriteBatch.Draw(foreground, Vector2.Zero, Color.White);
		}
		finally
		{
			Main.spriteBatch.End();
		}
		ProcessReflection(projected, processed, definition, normalMap, sampleCount);
	}

	public void DrawReflection(RenderTarget2D[] reflections, IReadOnlyList<MirrorWallReflectionCollector.ReflectionGroup> groups)
	{
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
			DepthStencilState.None, RasterizerState.CullNone);
		try
		{
			for (int index = 0; index < groups.Count; index++)
			{
				foreach (var wall in groups[index].Walls)
				{
					DrawWallReflection(wall.Position, reflections[index], wall.Tint);
				}
			}
		}
		finally
		{
			Main.spriteBatch.End();
		}
	}

	private void ProcessReflection(RenderTarget2D captured, RenderTarget2D processed, MirrorReflectionDefinition definition, Texture2D normalMap, int sampleCount)
	{
		Effect effect = ModAsset.MirrorReflectionShader.Value;
		Matrix view = Main.GameViewMatrix.TransformationMatrix;
		Vector3 angles = definition.Angles;
		effect.Parameters["uTransform"].SetValue(Matrix.CreateOrthographicOffCenter(0, captured.Width, captured.Height, 0, 0, 1));
		effect.Parameters["uSize"].SetValue(captured.Size());
		effect.Parameters["uScreenToWorld"].SetValue(Matrix.Invert(view) * Matrix.CreateTranslation(Main.screenPosition.X, Main.screenPosition.Y, 0));
		effect.Parameters["uWorldToScreen"].SetValue(view);
		effect.Parameters["uSurfaceRotation"].SetValue(Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y) * Matrix.CreateRotationZ(angles.Z));
		effect.Parameters["uNormalStrength"].SetValue(definition.NormalStrength);
		effect.Parameters["uNormalPeriod"].SetValue(definition.NormalPeriod);
		RenderPass(captured, processed, effect, "NormalDistortion", normalMap);
		if (definition.Blur > 0)
		{
			if (cachedSigma != definition.Sigma || cachedSampleCount != sampleCount)
			{
				cachedSigma = definition.Sigma;
				cachedSampleCount = sampleCount;
				blurWeights = CreateGaussianKernel(cachedSigma, sampleCount);
			}
			effect.Parameters["uWeights"].SetValue(blurWeights);
			// 各档位保持相同采样范围；减少采样点只降低滤波精度，不覆盖材质参数。
			float spacing = definition.Blur * 8f / (sampleCount - 1);
			string technique = sampleCount switch
			{
				3 => "GaussianBlurLow",
				5 => "GaussianBlurMedium",
				_ => "GaussianBlur",
			};
			Vector2 horizontal = Vector2.TransformNormal(new Vector2(spacing, 0), view);
			Vector2 vertical = Vector2.TransformNormal(new Vector2(0, spacing), view);
			effect.Parameters["uBlurStep"].SetValue(horizontal / captured.Size());
			RenderPass(processed, captured, effect, technique, normalMap);
			effect.Parameters["uBlurStep"].SetValue(vertical / captured.Size());
			RenderPass(captured, processed, effect, technique, normalMap);
		}
	}

	private static void RenderPass(Texture2D source, RenderTarget2D target, Effect effect, string technique, Texture2D normalMap)
	{
		GraphicsDevice device = Main.graphics.GraphicsDevice;
		device.SetRenderTarget(target);
		device.Clear(Color.Transparent);
		effect.CurrentTechnique = effect.Techniques[technique];
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp,
			DepthStencilState.None, RasterizerState.CullNone, effect);
		try
		{
			device.Textures[1] = normalMap;
			device.SamplerStates[1] = SamplerState.LinearWrap;
			Main.spriteBatch.Draw(source, target.Bounds, Color.White);
		}
		finally
		{
			Main.spriteBatch.End();
		}
	}

	private static void DrawWallReflection(Point point, Texture2D reflection, Color tint)
	{
		Rectangle cell = GetScreenRectangle(point.ToVector2() * 16, new Vector2(16), Main.screenPosition, Main.GameViewMatrix.TransformationMatrix);
		Rectangle viewport = new(0, 0, reflection.Width, reflection.Height);
		Rectangle slice = Rectangle.Intersect(cell, viewport);
		if (slice.Width > 0 && slice.Height > 0)
		{
			Main.spriteBatch.Draw(reflection, slice, slice, tint);
		}
	}

	/// <summary>
	/// 先还原前景已有的相机变换，再围绕共同基准投影倒影；墙网格保持原位。
	/// angles 为俯仰、偏航、滚转弧度。
	/// </summary>
	private static Matrix CreateScreenProjection(Vector2 anchor, Vector2 offset, float scale, bool flip, Vector3 angles, float distance, Matrix view)
	{
		if (!float.IsFinite(distance) || distance <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(distance));
		}
		Matrix perspective = Matrix.Identity;
		perspective.M34 = -1 / distance;
		// 保留透视 W，但将输出深度归零，避免 SpriteBatch 的二维裁剪面截掉倾斜图像。
		perspective.M33 = 0;
		Matrix projection = Matrix.CreateTranslation(-anchor.X, -anchor.Y, 0)
			* Matrix.CreateScale(flip ? -scale : scale, scale, 1)
			* Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y) * Matrix.CreateRotationZ(angles.Z)
			* perspective * Matrix.CreateTranslation(anchor.X + offset.X, anchor.Y + offset.Y, 0) * view;
		return Matrix.Invert(view) * projection;
	}

	/// <summary>
	/// 创建归一化高斯权重，各画质档位均覆盖原九点核的 [-4, 4] 采样范围。
	/// </summary>
	/// <param name="sigma">以原采样间距为单位的正标准差。</param>
	/// <param name="sampleCount">每个方向的采样数量，支持 3、5、9。</param>
	/// <returns>供 Shader 使用的九元素数组；前 sampleCount 项有效，其余为零。</returns>
	private static float[] CreateGaussianKernel(float sigma, int sampleCount)
	{
		if (!float.IsFinite(sigma) || sigma <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(sigma));
		}
		if (sampleCount is not (3 or 5 or 9))
		{
			throw new ArgumentOutOfRangeException(nameof(sampleCount));
		}
		float sampleSpacing = 8f / (sampleCount - 1);
		float[] weights = new float[9];
		float sum = 0;
		for (int i = 0; i < sampleCount; i++)
		{
			float distance = (i - sampleCount / 2) * sampleSpacing;
			weights[i] = MathF.Exp(-distance * distance / (2 * sigma * sigma));
			sum += weights[i];
		}
		for (int i = 0; i < sampleCount; i++)
		{
			weights[i] /= sum;
		}
		return weights;
	}

	/// <summary>
	/// 将世界矩形映射到屏幕并向下取整共用边界，使相邻墙格在相机缩放时保持连续。
	/// </summary>
	private static Rectangle GetScreenRectangle(Vector2 worldPosition, Vector2 size, Vector2 screenPosition, Matrix view)
	{
		Vector2 first = Vector2.Transform(worldPosition - screenPosition, view);
		Vector2 second = Vector2.Transform(worldPosition + size - screenPosition, view);
		Vector2 topLeft = Vector2.Min(first, second);
		Vector2 bottomRight = Vector2.Max(first, second);
		// 共用边界取整，缩放时相邻墙格既不留缝也不重复混合。
		return new Rectangle((int)MathF.Floor(topLeft.X), (int)MathF.Floor(topLeft.Y),
			(int)MathF.Floor(bottomRight.X) - (int)MathF.Floor(topLeft.X),
			(int)MathF.Floor(bottomRight.Y) - (int)MathF.Floor(topLeft.Y));
	}
}
