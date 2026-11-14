using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.TileHelper;
using Terraria.GameContent.Drawing;

namespace Everglow.CagedDomain.Tiles.CableTiles;

public class GreenGlassbulbBand_Bulb : CableTile
{
	public override void PostSetDefaults()
	{
		LampDistance = 3;
		RopeUnitMass = 0.4f;
		SingleLampMass = 0.2f;
		MaxWireStyle = 4;
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
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.GreenGlassbulbBand_Bulb_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.GreenGlassbulbBand_Bulb.Value;
		var tileSpriteEffect = SpriteEffects.None;

		// 获取发绳端物块信息
		Tile endTile = Main.tile[RopeHeadAndTail[pos]];
		int style = endTile.TileFrameX / 18 + (endTile.TileFrameY / 18) * 4;
		var masses = rope.Masses;
		for (int i = 0; i < masses.Length - 1; i++)
		{
			Mass thisMass = masses[i];
			Mass nextMass = masses[i + 1];

			int totalPushTime = 80;
			float pushForcePerFrame = 1.26f;
			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), tileDrawing._sunflowerWindCounter);
			}

			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			float rotation = -windCycle * 0.4f;
			if (!Main.gamePaused)
			{
				rope.ApplyForceSpecial(i, new Vector2(windCycle / 4.0f, 0.4f * thisMass.Value));
			}

			// 支持发光涂料
			Color tileLight;
			if (color != default(Color))
			{
				tileLight = color;
			}
			else
			{
				tileLight = Lighting.GetColor((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f));
			}
			Vector2 toNextMass = nextMass.Position - thisMass.Position;
			Vector2 drawPos = thisMass.Position - Main.screenPosition;
			spriteBatch.Draw(tex, drawPos, new Rectangle(10, 2, 2, 2), tileLight, toNextMass.ToRotation(), new Vector2(1f), new Vector2(toNextMass.Length() / 2f, 1), tileSpriteEffect, 0);
			float timeValue;
			float light;
			if (thisMass.Value == 0.2f)
			{
				switch (style)
				{
					case 0:
						spriteBatch.Draw(tex, drawPos, new Rectangle(12, 6, 10, 18), tileLight, rotation, new Vector2(3f, 0), 0.7f, tileSpriteEffect, 0);
						break;
					case 1:
						spriteBatch.Draw(tex, drawPos, new Rectangle(2, 6, 6, 18), tileLight, rotation, new Vector2(3f, 0), 0.7f, tileSpriteEffect, 0);
						Lighting.AddLight(thisMass.Position, new Vector3(0f, 0.5f, 0f));
						spriteBatch.Draw(tex, drawPos, new Rectangle(0, 30, 30, 30), new Color(1f, 1f, 1f, 0), rotation, new Vector2(15f, 7f), 0.7f, tileSpriteEffect, 0);
						break;
					case 2:
						timeValue = (float)Main.time * 0.09f + 2;
						light = MathF.Sin(timeValue + i / 3f * MathHelper.Pi);
						if (light > 0)
						{
							spriteBatch.Draw(tex, drawPos, new Rectangle(2, 6, 6, 18), tileLight, rotation, new Vector2(3f, 0), 0.7f, tileSpriteEffect, 0);
						}
						else
						{
							spriteBatch.Draw(tex, drawPos, new Rectangle(12, 6, 10, 18), tileLight, rotation, new Vector2(3f, 0), 0.7f, tileSpriteEffect, 0);
						}
						Lighting.AddLight(thisMass.Position, new Vector3(0f, 0.5f, 0f) * light); // blue light is dimmer than red and green, so it's 0.7f.
						spriteBatch.Draw(tex, drawPos, new Rectangle(0, 30, 30, 30), new Color(light, light, light, 0), rotation, new Vector2(15f, 7f), 0.7f, tileSpriteEffect, 0);
						break;
					case 3:
						timeValue = (float)Main.time * 0.02f + 2;
						light = MathF.Sin(timeValue);
						if (light > 0)
						{
							spriteBatch.Draw(tex, drawPos, new Rectangle(2, 6, 6, 18), tileLight, rotation, new Vector2(3f, 0), 0.7f, tileSpriteEffect, 0);
						}
						else
						{
							spriteBatch.Draw(tex, drawPos, new Rectangle(12, 6, 10, 18), tileLight, rotation, new Vector2(3f, 0), 0.7f, tileSpriteEffect, 0);
						}
						Lighting.AddLight(thisMass.Position, new Vector3(0f, 0.5f, 0f) * light); // blue light is dimmer than red and green, so it's 0.7f.
						spriteBatch.Draw(tex, drawPos, new Rectangle(0, 30, 30, 30), new Color(light, light, light, 0), rotation, new Vector2(15f, 7f), 0.7f, tileSpriteEffect, 0);
						break;
				}
			}
		}
	}
}
