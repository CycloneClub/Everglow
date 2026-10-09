using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Walls;

public class UnionGoldAcanthusWall : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = ModContent.DustType<FurnacePlatingWallDust>();
		AddMapEntry(new Color(158, 144, 133));
	}

	public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber)
	{
		return base.WallFrame(i, j, randomizeFrame, ref style, ref frameNumber);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var offsetScreen = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			offsetScreen = Vector2.Zero;
		}
		if (!Ins.VisualQuality.High)
		{
			Vector2 drawPos = new Point(i, j).ToWorldCoordinates() - Main.screenPosition + offsetScreen;
			Rectangle frame = new Rectangle(4 + (i % 16) * 16 - 4, 178 + (j % 16) * 16 - 4, 24, 24);
			spriteBatch.Draw(texture, drawPos, frame, Lighting.GetColor(i, j), 0, frame.Size() * 0.5f, 1, SpriteEffects.None, 0);
		}
		else
		{
			Vector2 drawPos = new Point(i, j).ToWorldCoordinates() - Main.screenPosition + offsetScreen;
			Color lightColor = Lighting.GetColor(i, j);
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					Rectangle frame = new Rectangle(4 + (i % 16) * 16 + x * 8 - 2, 178 + (j % 16) * 16 + y * 8 - 2, 8, 8);
					Vector2 offset = new Vector2(x, y) * 8 + new Vector2(-4);

					Color offsetColor = Lighting.GetColor(i + x - 1, j + y - 1);
					Color drawColor = Color.Lerp(offsetColor, lightColor, 0.5f);
					Vector4 color4 = drawColor.ToVector4();
					float addColor = 1.4f;
					float subsColor = -0.4f;
					color4.X = (float)Utils.Lerp(subsColor, addColor, color4.X);
					color4.Y = (float)Utils.Lerp(subsColor, addColor, color4.Y);
					color4.Z = (float)Utils.Lerp(subsColor, addColor, color4.Z);

					Color drawColorShine = new Color(color4.X, color4.Y, color4.Z, color4.W);

					frame = new Rectangle(264 + (i % 16) * 16 + x * 8 - 2, 178 + (j % 16) * 16 + y * 8 - 2, 8, 8);

					spriteBatch.Draw(texture, drawPos + offset - new Vector2(8), frame, drawColorShine, 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
				}
			}
		}
	}
}
