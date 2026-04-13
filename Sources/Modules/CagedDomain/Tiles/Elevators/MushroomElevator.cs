using Everglow.Commons.Templates.Furniture.Elevator;

namespace Everglow.CagedDomain.Tiles.Elevators;

public class MushroomElevator : ElevatorBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		LightSourceColor = new Vector3(0f, 0f, 1f);
	}

	public override void PostDrawElevator(Color lightColor)
	{
		Texture2D glow = ModAsset.MushroomElevator_glow.Value;
		Main.spriteBatch.Draw(glow, new Vector2(Box.Center.X, Box.Bottom) - Main.screenPosition, null, new Color(0f,0f, 1f, 0), 0, new Vector2(glow.Width / 2f, glow.Height), 1, SpriteEffects.None, 0);
	}

	public override void DrawAuxiliaryStructure(Color lightColor)
	{
		Vector2 drawPos = new Vector2(Box.Center.X, Box.Top) - Main.screenPosition;
		int unitFrameWidth = AuxiliaryStructureTexture.Width / 2;
		int unitFrameHeight = AuxiliaryStructureTexture.Height / 3;
		Rectangle auxiliaryFrame = new Rectangle(0, 0, unitFrameWidth, unitFrameHeight);
		if (LightSourceOn)
		{
			auxiliaryFrame.X += AuxiliaryStructureTexture.Width / 2;
		}
		AddLight();
		Vector2 auxiliaryOffset = new Vector2(auxiliaryFrame.Width * 0.5f, auxiliaryFrame.Height - 16);
		Main.spriteBatch.Draw(AuxiliaryStructureTexture, drawPos, auxiliaryFrame, lightColor, 0, auxiliaryOffset, 1, SpriteEffects.None, 0);
		if (LightSourceOn)
		{
			Rectangle auxiliaryFrame_glow = new Rectangle(auxiliaryFrame.X, unitFrameHeight, unitFrameWidth, unitFrameHeight);
			Rectangle auxiliaryFrame_bloom = new Rectangle(auxiliaryFrame.X, unitFrameHeight * 2, unitFrameWidth, unitFrameHeight);
			Main.spriteBatch.Draw(AuxiliaryStructureTexture, drawPos, auxiliaryFrame_glow, new Color(1f, 1f, 1f, 0) * 0.4f, 0, auxiliaryOffset, 1, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(AuxiliaryStructureTexture, drawPos, auxiliaryFrame_bloom, new Color(1f, 1f, 1f, 0) * 0.7f, 0, auxiliaryOffset, 1, SpriteEffects.None, 0);
		}
		if (Highlighted)
		{
			if (Main.SmartCursorIsUsed)
			{
				Vector2 highlight_Offset = new Vector2(AuxiliaryStructureTextureHighlight.Width * 0.5f, AuxiliaryStructureTextureHighlight.Height - 16);
				Main.spriteBatch.Draw(AuxiliaryStructureTextureHighlight, drawPos, null, Color.White, 0, highlight_Offset, 1, SpriteEffects.None, 0);
			}
		}
	}

	public override void AddLight() => base.AddLight();
}
