using Everglow.Commons.Mechanics.Mission.Core;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.Presentation;
using Everglow.Commons.UI.UIElements;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements;

public class UIMissionBackground : UIBlock
{
	private static readonly Color InitialColor = new Color(1f, 1f, 1f, 0f) * 0.8f;

	private PlayerMissionState? poolType = null;

	private MissionType? missionType = null;

	private float chainMovement = 0;

	private Color PoolTypeColor => ColorDefinition.GetMissionStateColor(poolType);

	private Color MissionTypeColor => ColorDefinition.GetMissionTypeColor(missionType);

	public void SetSpectrumColor(PlayerMissionState? poolType, MissionType? missionType)
	{
		this.poolType = poolType;
		this.missionType = missionType;
	}

	public void SetChainValue(float value)
	{
		chainMovement = value;
	}

	public override void Draw(SpriteBatch sb)
	{
		base.Draw(sb);
		Rectangle sourceRectangle = new Rectangle(0, 0, HitBox.Width, HitBox.Height);
		sb.Draw(ModAsset.Marble_Texture.Value, HitBox, sourceRectangle, new Color(1f, 1f, 1f, 1));
	}

	public void OldDraw()
	{
		SpriteBatch sb = Main.spriteBatch;

		// Remove the upper code.
		sb.Draw(ModAsset.Mission_MarbleBoard.Value, HitBox, Color.White);
		sb.Draw(ModAsset.Mission_MarbleBoard_background.Value, HitBox, Color.White);

		var scale = MissionContainer.Scale;
		var basePos = HitBox.TopLeft();

		// Laser Prism
		var laserPrism = ModAsset.LaserPrism.Value;
		var laserPrismPos = basePos + scale * (new Vector2(45, 203) + laserPrism.Size() / 2);

		// Crystal
		var crystal = ModAsset.TetragonalCrystal.Value;
		var crystalPos1 = basePos + scale * new Vector2(239, 413);
		var crystalPos2 = basePos + scale * new Vector2(618, 52);

		// Glass Brick
		var glassBrick = ModAsset.GlassBrick.Value;
		var glassBrickPos = basePos + scale * (new Vector2(233, 203) + glassBrick.Size() / 2);

		var glassBrick2 = ModAsset.GlassBrick225.Value;
		var glassBrickPos2 = basePos + scale * (new Vector2(420, 47) + glassBrick.Size() / 2);

		// Reflect Chain
		var chainCenter = basePos + scale * new Vector2(274, 210);

		var sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin();

		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

		const float laserColorScale = 0.35f;

		bool blockedAtOuter = MissionContainer.Filter.SpectrumBlockedAtOuter;
		bool blockedAtInner = MissionContainer.Filter.SpectrumBlockedAtInner;
		if (blockedAtOuter)
		{
			blockedAtInner = true;
		}

		// dark reflect effect.
		if (!blockedAtInner)
		{
			Texture2D reflectBeam2250_dark = ModAsset.Laser_Reflect22_50_black.Value;
			{
				Color drawColor = Color.Lerp(PoolTypeColor, MissionTypeColor, MathF.Sin((float)Main.timeForVisualEffects * 0.1f) * 0.2f + 0.8f);
				drawColor.R = drawColor.G = drawColor.B = drawColor.A;
				sb.Draw(reflectBeam2250_dark, chainCenter + new Vector2(-5, 0) * scale, null, drawColor, MathHelper.PiOver4, reflectBeam2250_dark.Size() / 2, scale * 0.4f, SpriteEffects.None, 0);
				sb.Draw(reflectBeam2250_dark, chainCenter + new Vector2(-5, 0.5f) * scale, null, drawColor, -MathHelper.PiOver2, reflectBeam2250_dark.Size() / 2, scale * 0.4f, SpriteEffects.FlipHorizontally, 0);

				drawColor = Color.Lerp(MissionTypeColor, PoolTypeColor, MathF.Sin((float)Main.timeForVisualEffects * 0.1f) * 0.2f + 0.8f);
				drawColor.R = drawColor.G = drawColor.B = drawColor.A;
				sb.Draw(reflectBeam2250_dark, chainCenter + new Vector2(-6, 0) * scale + new Vector2(224, 0).RotatedBy(-MathHelper.PiOver4) * scale, null, drawColor, -MathHelper.PiOver4 * 3, reflectBeam2250_dark.Size() / 2, scale * 0.3f, SpriteEffects.None, 0);
				sb.Draw(reflectBeam2250_dark, chainCenter + new Vector2(-6, 0) * scale + new Vector2(224, 0).RotatedBy(-MathHelper.PiOver4) * scale, null, drawColor, MathHelper.PiOver2, reflectBeam2250_dark.Size() / 2, scale * 0.3f, SpriteEffects.FlipHorizontally, 0);
			}
			Texture2D reflectBeam4500_dark = ModAsset.Laser_Reflect45_00_black.Value;
			{
				Color drawColor = MissionTypeColor;
				drawColor.R = drawColor.G = drawColor.B = drawColor.A;
				sb.Draw(reflectBeam4500_dark, glassBrickPos + new Vector2(-2, -1) * scale, null, drawColor, MathHelper.Pi, reflectBeam4500_dark.Size() / 2, scale * 0.2f, SpriteEffects.FlipHorizontally, 0);
				sb.Draw(reflectBeam4500_dark, glassBrickPos + new Vector2(-2, -0.5f) * scale, null, drawColor, -MathHelper.PiOver2, reflectBeam4500_dark.Size() / 2, scale * 0.2f, SpriteEffects.None, 0);
			}
		}

		// Prism to first layer of turntable(marble ring).
		var texCoordOffset = (float)Main.timeForVisualEffects * -0.02f;
		{
			var spectrumColor = InitialColor * laserColorScale;
			var spectrumPos = laserPrismPos;
			var radius = 8;
			var length = 60;

			var texCoordX = 0;

			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1.Value;
			var vertices = new List<Vertex2D>();
			vertices.Add(spectrumPos + new Vector2(0, radius) * scale, spectrumColor, new(0 + texCoordOffset, 0, 0));
			vertices.Add(spectrumPos + new Vector2(0, -radius) * scale, spectrumColor, new(0 + texCoordOffset, 1, 0));
			vertices.Add(spectrumPos + new Vector2(length, radius) * scale, spectrumColor, new(texCoordX + texCoordOffset, 0, 0));
			vertices.Add(spectrumPos + new Vector2(length, -radius) * scale, spectrumColor, new(texCoordX + texCoordOffset, 1, 0));
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
		}

		if (!blockedAtOuter)
		{
			// First layer of turntable to 2nd layer of turntable.The laser duplicated in this stage.
			{
				var spectrumColor_m_dark = MissionTypeColor * laserColorScale;
				var spectrumColor_i_dark = InitialColor * laserColorScale;
				spectrumColor_m_dark.R = spectrumColor_m_dark.G = spectrumColor_m_dark.B = spectrumColor_m_dark.A;
				spectrumColor_i_dark.R = spectrumColor_i_dark.G = spectrumColor_i_dark.B = spectrumColor_i_dark.A;

				var spectrumColor_m = MissionTypeColor * laserColorScale;
				var spectrumColor_i = InitialColor * laserColorScale;
				spectrumColor_m.A = 0;
				spectrumColor_i.A = 0;

				var spectrumPos = laserPrismPos + new Vector2(60, 0) * scale;
				var radius = 8;
				var length = 60;

				var texCoordX = 0;

				Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1_black.Value;
				var vertices = new List<Vertex2D>();
				vertices.Add(spectrumPos + new Vector2(0, radius) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 0, 0));
				vertices.Add(spectrumPos + new Vector2(0, -0) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 0.5f, 0));
				vertices.Add(spectrumPos + new Vector2(length, radius) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 0, 0));
				vertices.Add(spectrumPos + new Vector2(length, -0) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 0.5f, 0));
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

				vertices = new List<Vertex2D>();
				vertices.Add(spectrumPos + new Vector2(0, 0) * scale, spectrumColor_i_dark, new(0 + texCoordOffset, 0.5f, 0));
				vertices.Add(spectrumPos + new Vector2(0, -radius) * scale, spectrumColor_i_dark, new(0 + texCoordOffset, 1, 0));
				vertices.Add(spectrumPos + new Vector2(length, 0) * scale, spectrumColor_i_dark, new(texCoordX + texCoordOffset, 0.5f, 0));
				vertices.Add(spectrumPos + new Vector2(length, -radius) * scale, spectrumColor_i_dark, new(texCoordX + texCoordOffset, 1, 0));
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

				Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1.Value;
				vertices.Add(spectrumPos + new Vector2(0, radius) * scale, spectrumColor_m, new(0 + texCoordOffset, 0, 0));
				vertices.Add(spectrumPos + new Vector2(0, -0) * scale, spectrumColor_m, new(0 + texCoordOffset, 0.5f, 0));
				vertices.Add(spectrumPos + new Vector2(length, radius) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 0, 0));
				vertices.Add(spectrumPos + new Vector2(length, -0) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 0.5f, 0));
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

				vertices = new List<Vertex2D>();
				vertices.Add(spectrumPos + new Vector2(0, 0) * scale, spectrumColor_i, new(0 + texCoordOffset, 0.5f, 0));
				vertices.Add(spectrumPos + new Vector2(0, -radius) * scale, spectrumColor_i, new(0 + texCoordOffset, 1, 0));
				vertices.Add(spectrumPos + new Vector2(length, 0) * scale, spectrumColor_i, new(texCoordX + texCoordOffset, 0.5f, 0));
				vertices.Add(spectrumPos + new Vector2(length, -radius) * scale, spectrumColor_i, new(texCoordX + texCoordOffset, 1, 0));
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
			}

			if (!blockedAtInner)
			{
				// 2nd layer of turntable to center of turntable.
				{
					var spectrumColor_m_dark = MissionTypeColor * laserColorScale;
					var spectrumColor_p_dark = PoolTypeColor * laserColorScale;
					spectrumColor_m_dark.R = spectrumColor_m_dark.G = spectrumColor_m_dark.B = spectrumColor_m_dark.A;
					spectrumColor_p_dark.R = spectrumColor_p_dark.G = spectrumColor_p_dark.B = spectrumColor_p_dark.A;

					var spectrumColor_m = MissionTypeColor * laserColorScale;
					var spectrumColor_p = PoolTypeColor * laserColorScale;
					spectrumColor_m.A = 0;
					spectrumColor_p.A = 0;

					var spectrumPos = laserPrismPos + new Vector2(120, 0) * scale;
					var radius = 8;
					var length = 100;

					var texCoordX = 0;

					Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1_black.Value;
					var vertices = new List<Vertex2D>();
					vertices.Add(spectrumPos + new Vector2(0, radius) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 0, 0));
					vertices.Add(spectrumPos + new Vector2(0, -0) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 0.5f, 0));
					vertices.Add(spectrumPos + new Vector2(length, radius) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 0, 0));
					vertices.Add(spectrumPos + new Vector2(length, -0) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 0.5f, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					vertices = new List<Vertex2D>();
					vertices.Add(spectrumPos + new Vector2(0, 0) * scale, spectrumColor_p_dark, new(0 + texCoordOffset, 0.5f, 0));
					vertices.Add(spectrumPos + new Vector2(0, -radius) * scale, spectrumColor_p_dark, new(0 + texCoordOffset, 1, 0));
					vertices.Add(spectrumPos + new Vector2(length, 0) * scale, spectrumColor_p_dark, new(texCoordX + texCoordOffset, 0.5f, 0));
					vertices.Add(spectrumPos + new Vector2(length, -radius) * scale, spectrumColor_p_dark, new(texCoordX + texCoordOffset, 1, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1.Value;
					vertices = new List<Vertex2D>();
					vertices.Add(spectrumPos + new Vector2(0, radius) * scale, spectrumColor_m, new(0 + texCoordOffset, 0, 0));
					vertices.Add(spectrumPos + new Vector2(0, -0) * scale, spectrumColor_m, new(0 + texCoordOffset, 0.5f, 0));
					vertices.Add(spectrumPos + new Vector2(length, radius) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 0, 0));
					vertices.Add(spectrumPos + new Vector2(length, -0) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 0.5f, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					vertices = new List<Vertex2D>();
					vertices.Add(spectrumPos + new Vector2(0, 0) * scale, spectrumColor_p, new(0 + texCoordOffset, 0.5f, 0));
					vertices.Add(spectrumPos + new Vector2(0, -radius) * scale, spectrumColor_p, new(0 + texCoordOffset, 1, 0));
					vertices.Add(spectrumPos + new Vector2(length, 0) * scale, spectrumColor_p, new(texCoordX + texCoordOffset, 0.5f, 0));
					vertices.Add(spectrumPos + new Vector2(length, -radius) * scale, spectrumColor_p, new(texCoordX + texCoordOffset, 1, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
				}

				// Center of turntable to topright reflector.
				{
					var spectrumColor_m_dark = MissionTypeColor * laserColorScale;
					var spectrumColor_p_dark = PoolTypeColor * laserColorScale;
					spectrumColor_m_dark.R = spectrumColor_m_dark.G = spectrumColor_m_dark.B = spectrumColor_m_dark.A;
					spectrumColor_p_dark.R = spectrumColor_p_dark.G = spectrumColor_p_dark.B = spectrumColor_p_dark.A;

					var spectrumColor_m = MissionTypeColor * laserColorScale;
					var spectrumColor_p = PoolTypeColor * laserColorScale;
					spectrumColor_m.A = 0;
					spectrumColor_p.A = 0;

					var spectrumPos = chainCenter + new Vector2(-5, 0) * scale;
					var radius = 8;
					var length = 224;

					var texCoordX = 0;

					Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1_black.Value;
					var vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, radius).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(0, -0).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 0.5f, 0))
						 .Add(spectrumPos + new Vector2(length, radius).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(length, -0).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 0.5f, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, 0).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_p_dark, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(0, -radius).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_p_dark, new(0 + texCoordOffset, 1, 0))
						.Add(spectrumPos + new Vector2(length, 0).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_p_dark, new(texCoordX + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(length, -radius).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_p_dark, new(texCoordX + texCoordOffset, 1, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1.Value;
					vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, radius).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_m, new(0 + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(0, -0).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_m, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(length, radius).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(length, -0).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 0.5f, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, 0).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_p, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(0, -radius).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_p, new(0 + texCoordOffset, 1, 0))
						.Add(spectrumPos + new Vector2(length, 0).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_p, new(texCoordX + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(length, -radius).RotatedBy(-MathHelper.PiOver4) * scale, spectrumColor_p, new(texCoordX + texCoordOffset, 1, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
				}

				// Topright reflector to mission message board.
				{
					var spectrumColor_m_dark = MissionTypeColor * laserColorScale;
					var spectrumColor_p_dark = PoolTypeColor * laserColorScale;
					spectrumColor_m_dark.R = spectrumColor_m_dark.G = spectrumColor_m_dark.B = spectrumColor_m_dark.A;
					spectrumColor_p_dark.R = spectrumColor_p_dark.G = spectrumColor_p_dark.B = spectrumColor_p_dark.A;

					var spectrumColor_m = MissionTypeColor * laserColorScale;
					var spectrumColor_p = PoolTypeColor * laserColorScale;
					spectrumColor_m.A = 0;
					spectrumColor_p.A = 0;

					var spectrumPos = chainCenter + new Vector2(-6, 0) * scale + new Vector2(224, 0).RotatedBy(-MathHelper.PiOver4) * scale;
					var radius = 8;
					var length = 188;

					var texCoordX = 0;

					Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1_black.Value;
					var vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, radius) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(0, -0) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(length, radius) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(length, -0) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 0.5f, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, 0) * scale, spectrumColor_p_dark, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(0, -radius) * scale, spectrumColor_p_dark, new(0 + texCoordOffset, 1, 0))
						.Add(spectrumPos + new Vector2(length, 0) * scale, spectrumColor_p_dark, new(texCoordX + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(length, -radius) * scale, spectrumColor_p_dark, new(texCoordX + texCoordOffset, 1, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail.Value;
					vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, radius) * scale, spectrumColor_m, new(0 + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(0, -0) * scale, spectrumColor_m, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(length, radius) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(length, -0) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 0.5f, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, 0) * scale, spectrumColor_p, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(0, -radius) * scale, spectrumColor_p, new(0 + texCoordOffset, 1, 0))
						.Add(spectrumPos + new Vector2(length, 0) * scale, spectrumColor_p, new(texCoordX + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(length, -radius) * scale, spectrumColor_p, new(texCoordX + texCoordOffset, 1, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
				}

				// Refractor to mission stack.
				{
					var spectrumColor_m_dark = MissionTypeColor * laserColorScale;
					var spectrumColor_p_dark = PoolTypeColor * laserColorScale;
					spectrumColor_m_dark.R = spectrumColor_m_dark.G = spectrumColor_m_dark.B = spectrumColor_m_dark.A;
					spectrumColor_p_dark.R = spectrumColor_p_dark.G = spectrumColor_p_dark.B = spectrumColor_p_dark.A;

					var spectrumColor_m = MissionTypeColor * laserColorScale;
					var spectrumColor_p = PoolTypeColor * laserColorScale;
					spectrumColor_m.A = 0;
					spectrumColor_p.A = 0;

					var spectrumPos = glassBrickPos + new Vector2(-2, -1) * scale;
					var radius = 8;
					var length = 198;

					var texCoordX = 0;

					Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1_black.Value;
					var vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, 0) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 0.5f, 0))
						 .Add(spectrumPos + new Vector2(-radius, 0) * scale, spectrumColor_m_dark, new(0 + texCoordOffset, 1, 0))
						   .Add(spectrumPos + new Vector2(0, length) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 0.5f, 0))
						  .Add(spectrumPos + new Vector2(-radius, length) * scale, spectrumColor_m_dark, new(texCoordX + texCoordOffset, 1, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(radius, 0) * scale, spectrumColor_p_dark, new(0 + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(-0, 0) * scale, spectrumColor_p_dark, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(radius, length) * scale, spectrumColor_p_dark, new(texCoordX + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(-0, length) * scale, spectrumColor_p_dark, new(texCoordX + texCoordOffset, 0.5f, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Trail_1.Value;
					vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(0, 0) * scale, spectrumColor_m, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(-radius, 0) * scale, spectrumColor_m, new(0 + texCoordOffset, 1, 0))
						.Add(spectrumPos + new Vector2(0, length) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(-radius, length) * scale, spectrumColor_m, new(texCoordX + texCoordOffset, 1, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

					vertices = new List<Vertex2D>()
						.Add(spectrumPos + new Vector2(radius, 0) * scale, spectrumColor_p, new(0 + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(-0, 0) * scale, spectrumColor_p, new(0 + texCoordOffset, 0.5f, 0))
						.Add(spectrumPos + new Vector2(radius, length) * scale, spectrumColor_p, new(texCoordX + texCoordOffset, 0, 0))
						.Add(spectrumPos + new Vector2(-0, length) * scale, spectrumColor_p, new(texCoordX + texCoordOffset, 0.5f, 0));
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
				}
			}
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		// Draw outer ring blocking vfx
		if (blockedAtOuter)
		{
			Texture2D star = ModAsset.StarSlash.Value;
			Color drawColor = InitialColor;
			sb.Draw(star, laserPrismPos + new Vector2(50, 0) * scale, new Rectangle(0, 0, 256, 128), drawColor, -MathHelper.Pi / 2f, star.Size() / 2, scale * 0.6f, SpriteEffects.FlipHorizontally, 0);
		}

		// Draw inner ring blocking vfx
		if (blockedAtInner && !blockedAtOuter)
		{
			Texture2D star_black = ModAsset.StarSlash_black.Value;
			Color drawColor = MissionTypeColor;
			drawColor.R = drawColor.G = drawColor.B = drawColor.A;
			sb.Draw(star_black, laserPrismPos + new Vector2(105, 0) * scale, new Rectangle(0, 0, 256, 128), drawColor, -MathHelper.Pi / 2f, star_black.Size() / 2, scale * 0.6f, SpriteEffects.FlipHorizontally, 0);
			Texture2D star = ModAsset.StarSlash.Value;
			drawColor = MissionTypeColor;
			drawColor.A = 0;
			sb.Draw(star, laserPrismPos + new Vector2(105, 0) * scale, new Rectangle(0, 0, 256, 128), drawColor, -MathHelper.Pi / 2f, star.Size() / 2, scale * 0.6f, SpriteEffects.FlipHorizontally, 0);
		}

		if (!blockedAtInner)
		{
			Texture2D reflectBeam2250 = ModAsset.Laser_Reflect22_50.Value;
			{
				Color drawColor = Color.Lerp(PoolTypeColor, MissionTypeColor, MathF.Sin((float)Main.timeForVisualEffects * 0.1f) * 0.2f + 0.8f);
				drawColor.A = 0;
				sb.Draw(reflectBeam2250, chainCenter + new Vector2(-5, 0) * scale, null, drawColor, MathHelper.PiOver4, reflectBeam2250.Size() / 2, scale * 0.4f, SpriteEffects.None, 0);
				sb.Draw(reflectBeam2250, chainCenter + new Vector2(-5, 0.5f) * scale, null, drawColor, -MathHelper.PiOver2, reflectBeam2250.Size() / 2, scale * 0.4f, SpriteEffects.FlipHorizontally, 0);

				drawColor = Color.Lerp(MissionTypeColor, PoolTypeColor, MathF.Sin((float)Main.timeForVisualEffects * 0.1f) * 0.2f + 0.8f);
				drawColor.A = 0;
				sb.Draw(reflectBeam2250, chainCenter + new Vector2(-6, 0) * scale + new Vector2(224, 0).RotatedBy(-MathHelper.PiOver4) * scale, null, drawColor, -MathHelper.PiOver4 * 3, reflectBeam2250.Size() / 2, scale * 0.3f, SpriteEffects.None, 0);
				sb.Draw(reflectBeam2250, chainCenter + new Vector2(-6, 0) * scale + new Vector2(224, 0).RotatedBy(-MathHelper.PiOver4) * scale, null, drawColor, MathHelper.PiOver2, reflectBeam2250.Size() / 2, scale * 0.3f, SpriteEffects.FlipHorizontally, 0);
			}
			Texture2D reflectBeam4500 = ModAsset.Laser_Reflect45_00.Value;
			{
				Color drawColor = MissionTypeColor;
				drawColor.A = 0;
				sb.Draw(reflectBeam4500, glassBrickPos + new Vector2(-2, -1) * scale, null, drawColor, MathHelper.Pi, reflectBeam4500.Size() / 2, scale * 0.2f, SpriteEffects.FlipHorizontally, 0);
				sb.Draw(reflectBeam4500, glassBrickPos + new Vector2(-2, -0.5f) * scale, null, drawColor, -MathHelper.PiOver2, reflectBeam4500.Size() / 2, scale * 0.2f, SpriteEffects.None, 0);

				drawColor = InitialColor;
				drawColor.A = 0;
				sb.Draw(reflectBeam4500, laserPrismPos, null, drawColor, MathHelper.PiOver2, reflectBeam4500.Size() / 2, scale * 0.2f, SpriteEffects.FlipHorizontally, 0);
				sb.Draw(reflectBeam4500, laserPrismPos, null, drawColor, MathHelper.PiOver2, reflectBeam4500.Size() / 2, scale * 0.2f, SpriteEffects.None, 0);
			}
		}

		sb.Draw(laserPrism, laserPrismPos, null, Color.White, 0, laserPrism.Size() / 2, scale, SpriteEffects.None, 0);
		sb.Draw(crystal, crystalPos1, null, Color.White, 0, crystal.Size() / 2, scale, SpriteEffects.None, 0);
		sb.Draw(crystal, crystalPos2, null, Color.White, 0, crystal.Size() / 2, scale, SpriteEffects.None, 0);
		sb.Draw(glassBrick, glassBrickPos, null, Color.White, 0, glassBrick.Size() / 2, scale, SpriteEffects.None, 0);
		sb.Draw(glassBrick2, glassBrickPos2, null, Color.White, 0, glassBrick.Size() / 2, scale * 1.18f, SpriteEffects.None, 0);

		DrawChain(sb);
	}

	private void DrawChain(SpriteBatch sb)
	{
		var sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		Rectangle listHitBox = MissionContainer.List.HitBox;
		Texture2D texture = ModAsset.MirrorChain.Value;
		float scale = MissionContainer.Scale;
		float width = texture.Width * scale;
		float height = texture.Height * scale;

		float globalTexCoordOffset = chainMovement / height - 0.26f; // + 0.07f;

		// Draw mirrior chains (Left. Move with mission items synchronously)
		var vertices = new List<Vertex2D>();
		{
			float startX = listHitBox.X + 5 * scale;
			float endX = startX + width;

			float startY = listHitBox.Y - 2 * scale - 120 * scale;
			float endY = startY + height * 7f;

			float startTexCoordY = 0 - 0.66f;
			float endTexCoordY = startTexCoordY + (endY - startY) / height;

			float resourceOffset = 0.23f;
			startTexCoordY -= resourceOffset;
			endTexCoordY -= resourceOffset;

			startTexCoordY -= globalTexCoordOffset;
			endTexCoordY -= globalTexCoordOffset;

			vertices.Add(new Vector2(startX, startY), Color.White, new(0, startTexCoordY, 0));
			vertices.Add(new Vector2(endX, startY), Color.White, new(1, startTexCoordY, 0));
			vertices.Add(new Vector2(startX, endY), Color.White, new(0, endTexCoordY, 0));
			vertices.Add(new Vector2(endX, endY), Color.White, new(1, endTexCoordY, 0));
		}

		Main.graphics.GraphicsDevice.Textures[0] = texture;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

		// Draw mirrior chains (Right. Used as scrollbar)
		vertices = [];
		{
			float startX = listHitBox.X + listHitBox.Width - 18 * scale;
			float endX = startX + width;

			float startY = listHitBox.Y - 272 * scale;
			float endY = startY + height * 9f;

			float startTexCoordY = 0 - 0.42f;
			float endTexCoordY = startTexCoordY + (endY - startY) / height;

			startTexCoordY += globalTexCoordOffset;
			endTexCoordY += globalTexCoordOffset;

			vertices.Add(new Vector2(startX, startY), Color.White, new(0, startTexCoordY, 0));
			vertices.Add(new Vector2(endX, startY), Color.White, new(1, startTexCoordY, 0));
			vertices.Add(new Vector2(startX, endY), Color.White, new(0, endTexCoordY, 0));
			vertices.Add(new Vector2(endX, endY), Color.White, new(1, endTexCoordY, 0));
		}
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

		// Draw mirror chains (Above. Used to reflect the spectrum）
		vertices = [];
		{
			var baseCoord = new Vector2(HitBox.X, HitBox.Y);
			var vLeft = baseCoord + new Vector2(86, 284f) * scale;
			var vRight = baseCoord + new Vector2(448, 136f) * scale;
			var xOffset = width * MathF.Sin(MathHelper.PiOver4 / 2);
			var yOffset = width * MathF.Cos(MathHelper.PiOver4 / 2);

			var distance = Vector2.Distance(vLeft, vRight);
			var texCoordXDiff = distance / height;
			var startTexCoordY = globalTexCoordOffset;
			var endTexCoordY = texCoordXDiff + globalTexCoordOffset;

			vertices.Add(new Vector2(vLeft.X, vLeft.Y), Color.White, new(0, startTexCoordY, 0));
			vertices.Add(new Vector2(vLeft.X + xOffset, vLeft.Y + yOffset), Color.White, new(1, startTexCoordY, 0));
			vertices.Add(new Vector2(vRight.X, vRight.Y), Color.White, new(0, endTexCoordY, 0));
			vertices.Add(new Vector2(vRight.X + xOffset, vRight.Y + yOffset), Color.White, new(1, endTexCoordY, 0));
		}
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
	}
}
