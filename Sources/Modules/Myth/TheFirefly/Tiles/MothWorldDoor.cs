using Everglow.Myth.TheFirefly.WorldGeneration;
using SubworldLibrary;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class MothWorldDoor : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileLighted[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 7;
		TileObjectData.newTile.Width = 5;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			16,
			16,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(148, 0, 255), modTranslation);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 0f;
		g = 0f;
		b = 0f;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D tex = ModAsset.MothWorldDoorGlow.Value;

		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(1f, 1f, 1f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
		if (tile.TileFrameX == 72 && tile.TileFrameY == 108)
		{
			DrawMagicArraySystem.ArrayPosition = new Vector2(i, j);
		}
		base.PostDraw(i, j, spriteBatch);
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (DrawMagicArraySystem.ArrayPosition != Vector2.zeroVector)
		{
			float playerDistance = 2000;
			foreach (Player player in Main.player)
			{
				if (player.active && !player.dead)
				{
					float distance = (player.Center - (new Vector2(i, j) * 16 + new Vector2(-24, -27))).Length();
					if (distance < playerDistance)
					{
						playerDistance = distance;
					}
				}
			}
			if (playerDistance > 1800)
			{
				DrawMagicArraySystem.ArrayPosition = Vector2.zeroVector;
			}
		}
		base.RandomUpdate(i, j);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Player player = Main.LocalPlayer;
		if ((player.Center - new Vector2(i * 16, j * 16)).Length() < 12)
		{
		}
		base.NearbyEffects(i, j, closer);
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<Items.FireflyImpression>();
	}

	public override bool RightClick(int i, int j)
	{
		if (SubworldSystem.IsActive<MothWorld>())
		{
			SubworldSystem.Exit();
		}
		else
		{
			if (!SubworldSystem.Enter<MothWorld>())
			{
				Main.NewText("Fail!");
			}
		}
		return base.RightClick(i, j);
	}
}

public class DrawMagicArraySystem : ModSystem
{
	public override void OnModLoad()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			Ins.HookManager.AddHook(CodeLayer.PostDrawTiles, DrawMagicArray);
		}
	}

	public static Vector2 ArrayPosition = Vector2.zeroVector;

	public static void DrawMagicArray()
	{
		if (ArrayPosition == Vector2.zeroVector)
		{
			return;
		}
		Color c0 = new Color(0, 70, 155, 0);
		int playerWhoAmI = -1;
		float playerDistance = 150;
		foreach (Player player in Main.player)
		{
			if (player.active && !player.dead)
			{
				float distance = (player.Center - (ArrayPosition * 16 + new Vector2(-24, -27))).Length();
				if (distance < playerDistance)
				{
					playerDistance = distance;
					playerWhoAmI = player.whoAmI;
				}
			}
		}
		float mulColor = Math.Max((150 - playerDistance) / 150f, 0.25f);
		c0 *= mulColor;
		float timer = (float)(Main.time * 0.003f);
		Vector2 pos = ArrayPosition * 16 - Main.screenPosition + new Vector2(-24, -27);

		Lighting.AddLight(pos + Main.screenPosition, 0, 1.2f * mulColor, 2.6f * mulColor);
		Texture2D magicSeal = ModAsset.HiveCyberNoiseThicker.Value;
		DrawTexCircle(26, 12, c0, pos, magicSeal, -timer);
		DrawTexCircle(22, 12, c0, pos, magicSeal, timer);

		Color c1 = c0 * 0.8f;
		float timeRot = timer;
		float size = 24f;
		Vector2 Point1 = pos + new Vector2(0, size).RotatedBy(Math.PI * 0 + timeRot);
		Vector2 Point2 = pos + new Vector2(0, size).RotatedBy(Math.PI * 2 / 3d + timeRot);
		Vector2 Point3 = pos + new Vector2(0, size).RotatedBy(Math.PI * 4 / 3d + timeRot);

		Vector2 Point4 = pos + new Vector2(0, size).RotatedBy(Math.PI * 1 / 3d + timeRot);
		Vector2 Point5 = pos + new Vector2(0, size).RotatedBy(Math.PI * 3 / 3d + timeRot);
		Vector2 Point6 = pos + new Vector2(0, size).RotatedBy(Math.PI * 5 / 3d + timeRot);
		magicSeal = ModAsset.CorruptDustLine.Value;

		DrawTexLine(Point1, Point2, c1, c1, magicSeal);
		DrawTexLine(Point2, Point3, c1, c1, magicSeal);
		DrawTexLine(Point3, Point1, c1, c1, magicSeal);

		DrawTexLine(Point4, Point5, c1, c1, magicSeal);
		DrawTexLine(Point5, Point6, c1, c1, magicSeal);
		DrawTexLine(Point6, Point4, c1, c1, magicSeal);
	}

	public static void DrawTexLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2, Texture2D tex)
	{
		float Wid = 4f;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		float texcoordHeight = 1f;
		float value0 = (float)(Main.time / 391d + 20) % 1f;
		float value1 = (float)(Main.time / 391d + 20.4) % 1f;

		if (value1 < value0)
		{
			float valueMiddle = (1 - value0) / (0.4f);
			Vector2 Delta = EndPos - StartPos;
			vertex2Ds.Add(new Vertex2D(StartPos + Width, color1, new Vector3(value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos + Delta * valueMiddle + Width, color2, new Vector3(1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width, color1, new Vector3(value0, texcoordHeight, 0)));

			vertex2Ds.Add(new Vertex2D(StartPos + Delta * valueMiddle + Width, color2, new Vector3(1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos + Delta * valueMiddle - Width, color2, new Vector3(1, texcoordHeight, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width, color1, new Vector3(value0, texcoordHeight, 0)));

			vertex2Ds.Add(new Vertex2D(StartPos + Delta * valueMiddle + Width, color1, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width, color2, new Vector3(value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos + Delta * valueMiddle - Width, color1, new Vector3(0, texcoordHeight, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width, color2, new Vector3(value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width, color2, new Vector3(value1, texcoordHeight, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos + Delta * valueMiddle - Width, color1, new Vector3(0, texcoordHeight, 0)));
		}
		else
		{
			vertex2Ds.Add(new Vertex2D(StartPos + Width, color1, new Vector3(value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width, color2, new Vector3(value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width, color1, new Vector3(value0, texcoordHeight, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width, color2, new Vector3(value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width, color2, new Vector3(value1, texcoordHeight, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width, color1, new Vector3(value0, texcoordHeight, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}

	public static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		float timer = (float)(Main.time * 0.003f);
		float sinValue = 0.8f * (MathF.Sin(timer + radius / 8 * MathF.PI) * 0.5f + 0.5f);
		var circle = new List<Vertex2D>();
		for (int h = 0; h < 60; h += 1)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / 60f * Math.PI * 2 + addRot), color, new Vector3(h / 60f, 0.2f + sinValue, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / 60f * Math.PI * 2 + addRot), color, new Vector3(h / 60f, sinValue, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0.2f + sinValue, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(1, sinValue, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0.2f + sinValue, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0, sinValue, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}
}
