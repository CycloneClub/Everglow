using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.TileHelper;
using Terraria.GameContent.Drawing;

namespace Everglow.CagedDomain.Tiles.HangingTiles;

public class HangingFluoriteLamp : HangingTile
{
	public override void PostSetDefaults()
	{
		RopeUnitMass = 0.6f;
		HangingItemMass = 200f;
		MaxWireStyle = 1;
		Elasticity = 70f;
	}

	public override void DrawCable(Rope rope, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Color color = default)
	{
		// 回声涂料
		if (!TileDrawing.IsVisible(Main.tile[pos]))
		{
			return;
		}

		var tile = Main.tile[pos];
		ushort type = tile.TileType;
		int paint = Main.tile[pos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.HangingFluoriteLamp_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.HangingFluoriteLamp.Value;
		var tileSpriteEffect = SpriteEffects.None;

		// 获取发绳端物块信息
		var masses = rope.Masses;
		for (int i = 0; i < masses.Length; i++)
		{
			Mass thisMass = masses[i];
			if (i < MaxCableLength - tile.TileFrameY)
			{
				thisMass.IsStatic = true;
				thisMass.Position = pos.ToWorldCoordinates();
				continue;
			}
			else
			{
				thisMass.IsStatic = false;
			}
			int totalPushTime = 80;
			float pushForcePerFrame = 1.26f;
			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), tileDrawing._sunflowerWindCounter);
			}

			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			if (!Main.gamePaused)
			{
				if (i < masses.Length - 1)
				{
					rope.ApplyForceSpecial(i, new Vector2(windCycle / 4.0f, 0.4f * thisMass.Value));
				}
				else
				{
					rope.ApplyForceSpecial(i, new Vector2(windCycle * 10.0f, 0.4f * thisMass.Value));
				}
			}

			// 支持发光涂料
			Color tileLight;
			if (color != default)
			{
				tileLight = color;
			}
			else
			{
				tileLight = Lighting.GetColor((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f));
			}

			Vector2 toNextMass;
			if (i < masses.Length - 1)
			{
				Mass nextMass = masses[i + 1];
				toNextMass = nextMass.Position - thisMass.Position;
			}
			else
			{
				Mass passedMass = masses[i - 1];
				toNextMass = thisMass.Position - passedMass.Position;
			}
			Vector2 drawPos = thisMass.Position - Main.screenPosition;
			if (i < masses.Length - 1)
			{
				spriteBatch.Draw(tex, drawPos, new Rectangle(8 * (i % 4), 0, 8, 10), tileLight, toNextMass.ToRotation() - MathHelper.PiOver2, new Vector2(4f, 0), 1f, tileSpriteEffect, 0);
			}
			else
			{
				spriteBatch.Draw(tex, drawPos, new Rectangle(0, 12, 32, 40), tileLight, toNextMass.ToRotation() - MathHelper.PiOver2, new Vector2(16f, 0), 1f, tileSpriteEffect, 0);
				Lighting.AddLight(drawPos + Main.screenPosition, new Vector3(0.8f, 0.8f, 0.2f));
			}
		}
	}
}
