using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.TileHelper;
using Terraria.GameContent.Drawing;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class JellyBallNerveCable : CableTile
{
	public override string Texture => ModAsset.JellyBallNerveCable_Mod;

	public override void DrawCable(Rope rope, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Color color = default)
	{
		if (!TileDrawing.IsVisible(Main.tile[pos]))
		{
			return;
		}

		Texture2D texture = ModAsset.JellyBallNerveCable_Joint.Value;
		var masses = rope.Masses;
		for (int i = 0; i < masses.Length - 1; i++)
		{
			Mass thisMass = masses[i];
			Mass nextMass = masses[i + 1];
			int tileX = (int)((thisMass.Position.X - 8) / 16f);
			int tileY = (int)((thisMass.Position.Y - 8) / 16f);
			float windCycle = tileDrawing.InAPlaceWithWind(tileX, tileY, 1, 1)
				? tileDrawing.GetWindCycle(tileX, tileY, tileDrawing._sunflowerWindCounter)
				: 0f;
			windCycle += tileDrawing.GetHighestWindGridPushComplex(tileX, tileY, 1, 1, 80, 1.26f, 3, swapLoopDir: true);
			if (!Main.gamePaused)
			{
				rope.ApplyForceSpecial(i, new Vector2(windCycle / 16f, 0.4f * thisMass.Value));
			}

			Vector2 segment = nextMass.Position - thisMass.Position;
			spriteBatch.Draw(
				texture,
				thisMass.Position - Main.screenPosition,
				null,
				Color.White,
				segment.ToRotation(),
				new Vector2(texture.Width, texture.Height) * 0.5f,
				new Vector2(segment.Length() / texture.Width, 1f),
				SpriteEffects.None,
				0f);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return true;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Texture2D tex = ModAsset.JellyBallNerveCable.Value;
		Vector2 pos = new Point(i, j).ToWorldCoordinates();
		Vector2 zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.zeroVector;
		}
		pos += zero - Main.screenPosition;
		spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
	}
}
