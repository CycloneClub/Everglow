using System.Timers;
using Everglow.Commons.Utilities.BackgroundHelper;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class GiantFurnace_Construct : BackgroundSlideBase
{
	public Point TileAnchor;

	public List<Point> BgTiles = new List<Point>();

	public List<Rectangle> GlowFrames = new List<Rectangle>();

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.GiantFurnace_Construct.Value;
		Distance = 1;
		UseColorStyle = 1;
		LayerPriority = 1;
		Shader = Effects.XWrap_YWrap_Shader;

		Rectangle currentFrame = new Rectangle(0, 158, 6, 6);
		Point step = new Point(0, 14);
		for (int k = 0; k < 78; k++)
		{
			switch (k)
			{
				case 1:
					step = new Point(4, 10);
					break;
				case 2:
					step = new Point(18, 2);
					break;
				case 3:
					step = new Point(16, 0);
					break;
				case 45:
					step = new Point(8, -4);
					break;
				case 46:
					step = new Point(2, -14);
					break;
				case 47:
					step = new Point(0, -16);
					break;
				case 51:
					step = new Point(-4, -10);
					break;
				case 52:
					step = new Point(-14, -2);
					break;
				case 53:
					step = new Point(-16, 0);
					break;
				case 67:
					step = new Point(-18, 0);
					break;
				case 68:
					step = new Point(0, -10);
					break;
				case 69:
					step = new Point(0, -16);
					break;
				default:
					break;
			}
			GlowFrames.Add(currentFrame);
			currentFrame.X += step.X;
			currentFrame.Y += step.Y;
		}
	}

	public override void Update()
	{
		base.Update();
		for (int i = 0; i < 76; i++)
		{
			Vector2 worldPos = new Vector2(i * 16, 0) + WorldAnchor - (new Vector2(47, 63 - 92) * 16 + new Vector2(4, 24));
			Lighting.AddLight(worldPos, new Vector3(3f, 1.8f, 0.6f));
		}
	}

	public override void Draw()
	{
		var bars = new List<Vertex2D>();
		BackgroundHigherPerformanceHelper.Add_TileBgVertice(this, BgTiles, bars, 3);
		DrawVertexBackground(this, PrimitiveType.TriangleStrip, bars);

		Texture2D glow = ModAsset.GiantFurnace_Construct_glow.Value;
		Vector2 pos = WorldAnchor - Main.screenPosition + new Vector2(-412, 288);
		foreach (var timer in YggdrasilTownFurnaceSystem.MeltingAnimationTimer)
		{
			if (timer > 0)
			{
				float value = (120 - timer) / 120f * 1.2f;
				value -= 0.1f;
				for (int i = 0; i < GlowFrames.Count; i++)
				{
					float index = i / (float)GlowFrames.Count * 1.05f;
					float valueLerp = index - value;
					valueLerp *= 10;
					if (valueLerp is >= 0 and <= 1f)
					{
						var frame = GlowFrames[i];
						var color = Color.Lerp(new Color(1f, 0.75f, 0.1f, 1), new Color(0.4f, 0f, 0, 0), 1 - valueLerp);
						color = Color.Lerp(color, new Color(0, 0, 0, 0), 1 - valueLerp);
						Main.spriteBatch.Draw(glow, new Rectangle((int)pos.X + frame.X, (int)pos.Y + frame.Y, frame.Width, frame.Height), GlowFrames[i], color);
						if (i <= 73)
						{
							Lighting.AddLight(WorldAnchor + new Vector2(-412 + frame.X, 288 + frame.Y), color.ToVector3() * 2);
						}
					}
				}
			}
		}
		Texture2D screen = ModAsset.FurnaceScreen_Animation.Value;
		if (YggdrasilTownFurnaceSystem.CurrentPlayer is not null || YggdrasilTownFurnaceSystem.PlayerLeaveFloorScaleTimer > 0)
		{
			var frame = new Rectangle(0, 0, 80, 48);
			if (YggdrasilTownFurnaceSystem.PlayerDropInFloorScaleTimer > 0)
			{
				int value = (int)((30 - YggdrasilTownFurnaceSystem.PlayerDropInFloorScaleTimer) / 30f * 7f);
				frame = new Rectangle(0, 336 + 48 * value, 80, 48);
			}
			if (YggdrasilTownFurnaceSystem.PlayerLeaveFloorScaleTimer > 0)
			{
				int value = (int)((30 - YggdrasilTownFurnaceSystem.PlayerLeaveFloorScaleTimer) / 30f * 7f);
				frame = new Rectangle(0, 48 * value, 80, 48);
			}
			Main.spriteBatch.Draw(screen, new Rectangle((int)pos.X - 166, (int)pos.Y + 96, frame.Width, frame.Height), frame, Color.White);
		}
	}

	public override bool CanActive()
	{
		return TileUtils.SafeGetTile(TileAnchor).TileType == ModContent.TileType<YggdrasilCommandBlock>();
	}
}
