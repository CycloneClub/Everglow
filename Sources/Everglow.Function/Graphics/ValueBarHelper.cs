using Everglow.Commons.Vertex;

namespace Everglow.Commons.Graphics;

/// <summary>
/// UI Bar(like npc health) painter.
/// </summary>
public class ValueBarHelper
{
	/// <summary>
	/// Draw a bar progress as value (0~1)
	/// </summary>
	/// <param name="spriteBatch"></param>
	/// <param name="center"></param>
	/// <param name="scale"></param>
	/// <param name="value"></param>
	/// <param name="color0"></param>
	/// <param name="color1"></param>
	public static void DrawValueBar(SpriteBatch spriteBatch, Vector2 center, float value, Color color0, Color color1, float scale = 1)
	{
		float powerProgress = value;
		var progressTexture = ModAsset.White.Value;
		var progressPosition = center;

		var frameColor = new Color(0.05f, 0.05f, 0.08f, 0.9f);
		var frameColor2 = new Color(0.15f, 0.25f, 0.38f, 0.4f);
		Vector2 frameScale = new Vector2(2f, 0.4f) * 0.05f * scale;
		Vector2 frameScale2 = new Vector2(1.8f, 0.2f) * 0.05f * scale;

		Color lineColor = Color.Lerp(color0, color1, value);
		Color lineColorInner = lineColor * 0.8f;
		lineColorInner.A = 255;
		Vector2 lineScaleOuter = new Vector2(2.2f * powerProgress + 0.2f, 0.7f) * 0.05f * scale;
		Vector2 lineScale = new Vector2(2.2f * powerProgress, 0.5f) * 0.05f * scale;
		Vector2 lineScale2 = new Vector2(2.2f * powerProgress - 0.2f, 0.3f) * 0.05f * scale;
		var linePositionOffset = new Vector2(-2.2f * (1 - powerProgress) * progressTexture.Width * 0.025f, 0);

		spriteBatch.Draw(progressTexture, progressPosition, null, frameColor, 0, progressTexture.Size() / 2, frameScale, SpriteEffects.None, 0);
		spriteBatch.Draw(progressTexture, progressPosition, null, frameColor2, 0, progressTexture.Size() / 2f, frameScale2, SpriteEffects.None, 0);

		spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, frameColor, 0, progressTexture.Size() / 2, lineScaleOuter, SpriteEffects.None, 0);
		spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColor, 0, progressTexture.Size() / 2, lineScale, SpriteEffects.None, 0);
		spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColorInner, 0, progressTexture.Size() / 2, lineScale2, SpriteEffects.None, 0);
	}

	/// <summary>
	/// Draw a visual effect in the breaking sudden.
	/// </summary>
	/// <param name="spriteBatch"></param>
	/// <param name="position"></param>
	/// <param name="valueTime"></param>
	/// <param name="scale"></param>
	public static void DrawBreakOutEffect(SpriteBatch spriteBatch, Vector2 position, float valueTime, float scale = 1)
	{
		var drawScale = scale * (3 + valueTime * 6);
		var bloom = ModAsset.PieChartBloom_ring.Value;
		spriteBatch.Draw(bloom, position, null, new Color(1f, 1f, 1f, 0) * (1 - valueTime), 0, bloom.Size() / 2, drawScale * 1.5f, SpriteEffects.None, 0);
	}

	/// <summary>
	/// Draw a circle bar progress as value (0~1)
	/// </summary>
	/// <param name="spriteBatch"></param>
	/// <param name="position"></param>
	/// <param name="value"></param>
	/// <param name="color0"></param>
	/// <param name="color1"></param>
	/// <param name="scale"></param>
	/// <param name="centerIcon">Optional texture drawn at the center of the progress bar.</param>
	public static void DrawCircleValueBar(SpriteBatch spriteBatch, Vector2 position, float value, Color color0, Color color1, float scale = 1, Texture2D centerIcon = null)
	{
		var bloom_shadow = ModAsset.PieChartBloom_black.Value;
		var bloom = ModAsset.PieChartBloom.Value;
		Color bloomColor = color1;
		bloomColor.A = 0;
		spriteBatch.Draw(bloom_shadow, position, null, Color.White * 0.8f, 0, bloom_shadow.Size() / 2, scale * 4.5f, SpriteEffects.None, 0);
		spriteBatch.Draw(bloom, position, null, bloomColor, 0, bloom.Size() / 2, scale * 4.5f, SpriteEffects.None, 0);
		spriteBatch.Draw(ModAsset.White.Value, position, null, Color.White, 0, Vector2.Zero, 0, SpriteEffects.None, 0);

		var frameColor = new Color(0.05f, 0.05f, 0.08f, 0.6f);
		var frameInnerColor = new Color(0.15f, 0.25f, 0.38f, 0.4f);
		if (centerIcon is not null)
		{
			frameInnerColor = color1;
		}

		Color progressColor = color0;
		Color progressInnerColor = progressColor * 1.9f;
		progressInnerColor.A = 255;

		Vector2 radius = new Vector2(0, 100) * scale;
		Vector2 frameRadiusOffset = new Vector2(0, 40) * scale;
		Vector2 frameInnerRadiusOffset = frameRadiusOffset * 0.3f;
		Vector2 progressInnerRadiusOffset = frameRadiusOffset * 0.5f;

		// Draw frame
		if (centerIcon is null)
		{
			DrawCircle(spriteBatch, position, frameColor, radius, frameRadiusOffset, 1);
			DrawCircle(spriteBatch, position, frameInnerColor, radius, frameInnerRadiusOffset, 1);
		}
		else
		{
			var fullFrame = (radius + frameRadiusOffset) / 2f;
			DrawCircle(spriteBatch, position, frameColor * 1.5f, fullFrame, fullFrame, 1);
		}

		// Draw progress
		float progressBarSize = 0.7f;
		DrawCircle(spriteBatch, position, progressInnerColor, radius * progressBarSize, progressInnerRadiusOffset, value);

		if (centerIcon is not null)
		{
			var circleRadius = (radius - progressInnerRadiusOffset).Length();
			var iconScale = circleRadius * 2 * 0.95f / centerIcon.Width;
			var iconColor = new Color(0.25f, 0.25f, 0.25f, 0.5f) * 0.25f;
			spriteBatch.Draw(centerIcon, position, null, iconColor, 0, centerIcon.Size() / 2, iconScale, SpriteEffects.None, 0);
		}
	}

	/// <summary>
	/// Draw a circle
	/// </summary>
	/// <param name="spriteBatch"></param>
	/// <param name="position"></param>
	/// <param name="color"></param>
	/// <param name="radius"></param>
	/// <param name="radiusOffset"></param>
	/// <param name="progress"></param>
	/// <param name="clockwise">Whether the arc advances clockwise from its starting direction.</param>
	/// <param name="totalVertexCount"></param>
	public static void DrawCircle(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 radius, Vector2 radiusOffset, float progress, bool clockwise = true, int totalVertexCount = 180)
	{
		int circleVertexCount = (int)(totalVertexCount * progress);
		List<Vertex2D> lineShadowVertices = [];
		float clockwiseFactor = clockwise ? 1 : -1;
		for (int i = 0; i <= circleVertexCount; i++)
		{
			var rotation = MathF.PI * 2 * i / totalVertexCount;
			lineShadowVertices.Add(position - (radius - radiusOffset).RotatedBy(clockwiseFactor * rotation), color, new Vector3(1, 0, 0));
			lineShadowVertices.Add(position - (radius + radiusOffset).RotatedBy(clockwiseFactor * rotation), color, new Vector3(1, 0, 0));
		}

		spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, lineShadowVertices.ToArray(), 0, lineShadowVertices.Count - 2);
	}
}
