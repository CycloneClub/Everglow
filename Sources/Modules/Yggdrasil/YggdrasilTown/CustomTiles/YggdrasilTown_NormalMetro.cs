using Everglow.Commons.CustomTiles.Abstracts;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.CustomTiles;

public class YggdrasilTown_NormalMetro : BoxEntity
{
	public enum BehaviorType
	{
		Move,
		Stop,
	}

	public int StopTimer = 0;

	public int Direction = 1;

	public int StopTimeMax = 600;

	public int State = 0;

	public float TrackY = 0;

	public float StationX = 0;

	public float Speed = 0;

	public float ShellAlpha = 1;

	public override void SetDefaults()
	{
		Size = new Vector2(1696, 16);
	}

	public override void OnSpawn()
	{
		SetTrackY();
		Position = new Vector2(Position.X, TrackY);
		if (!SearchStation())
		{
			Direction *= -1;
			SearchStation();
		}
	}

	public override Color MapColor => new Color(128, 131, 142);

	public override void AI()
	{
		switch (State)
		{
			case (int)BehaviorType.Move:

				if (StationX != 0)
				{
					Accelerate();
				}
				Velocity = new Vector2(Direction * Speed, 0);
				break;
			case (int)BehaviorType.Stop:
				Velocity *= 0;
				StopTimer++;
				if (StopTimer >= StopTimeMax)
				{
					if (!SearchStation())
					{
						Direction *= -1;
						SearchStation();
					}
					State = (int)BehaviorType.Move;
				}
				break;
		}

		Position += Velocity;
		if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<MetroHelper>() && Main.mouseMiddle && Main.mouseMiddleRelease)
		{
			Active = false;
		}
		CheckPlayerInTrain();
		AddLight();
	}

	public void AddLight()
	{
		float value = 1 - ShellAlpha;
		for (int m = 0; m < 2; m++)
		{
			for (int k = 0; k < 3; k++)
			{
				Lighting.AddLight(Position + new Vector2(61 + k * 126 + m * 958, -86), new Vector3(1f, 1f, 1f) * value);
			}
			for (int k = 0; k < 3; k++)
			{
				Lighting.AddLight(Position + new Vector2(535 + k * 126 + m * 958, -86), new Vector3(1f, 1f, 1f) * value);
			}
		}
	}

	public void Accelerate()
	{
		float disToStationSign = (StationX - Box.Center.X) * Direction;
		if (disToStationSign > 8000)
		{
			if (Speed < 8)
			{
				Speed += 0.02f;
			}
		}
		else if (disToStationSign > 1600)
		{
			if (Speed >= 5)
			{
				Speed -= 0.02f;
			}
			else
			{
				Speed += 0.02f;
			}
		}
		else if (disToStationSign > 200)
		{
			if (Speed >= 1)
			{
				Speed -= 0.02f;
			}
			else
			{
				Speed += 0.02f;
			}
		}
		else if (disToStationSign > 10)
		{
			if (Speed >= 0.2f)
			{
				Speed -= 0.02f;
			}
			else
			{
				Speed += 0.02f;
			}
		}
		else if (disToStationSign > 1)
		{
			if (Speed >= 0.02)
			{
				Speed -= 0.005f;
			}
			else
			{
				Speed += 0.005f;
			}
		}
		else
		{
			if (disToStationSign < -10)
			{
				Direction *= -1;
			}
			else
			{
				Speed = 0;
				State = (int)BehaviorType.Stop;
				StopTimer = 0;
			}
		}
	}

	public void SetTrackY()
	{
		TrackY = Position.Y - Position.Y % 16;
	}

	public bool SearchStation()
	{
		int y = (int)(TrackY / 16f - 3);
		int box_x = (int)(Box.Center.X / 16);
		for (int dx = 10; dx < 6000; dx += 5)
		{
			int x = box_x + dx * Direction;
			var tile = TileUtils.SafeGetTile(x, y);
			if (tile.TileType == ModContent.TileType<MetroStationSign_YggdrasilTown>() || tile.TileType == ModContent.TileType<MetroStationSign_PylonSquare>())
			{
				StationX = (x - tile.TileFrameX / 18 + 2) * 16;
				return true;
			}
		}
		return false;
	}

	public override void Draw()
	{
		Texture2D metro = ModAsset.YggdrasilTown_NormalMetro.Value;
		var frame = new Rectangle(174, 165, 848, 160);
		var glow_frame = new Rectangle(174, 495, 848, 160);
		var outShellFrame = frame;
		outShellFrame.Y -= 170;
		var pos0 = new Vector2((Box.Center.X + Box.Left) * 0.5f, Box.Bottom);
		TileUtils.VertexDraw_Grid(pos0 - Main.screenPosition, frame, new Vector2(frame.Width / 2f, frame.Height), metro, Main.spriteBatch, 0);
		Main.spriteBatch.Draw(metro, pos0 - Main.screenPosition, glow_frame, new Color(1f, 1f, 1f, 0) * (1 - ShellAlpha), 0, new Vector2(frame.Width / 2f, frame.Height), 1, SpriteEffects.None, 0);
		if (ShellAlpha > 0)
		{
			TileUtils.VertexDraw_Grid(pos0 - Main.screenPosition, outShellFrame, new Vector2(frame.Width / 2f, frame.Height), metro, Main.spriteBatch, 0, ShellAlpha);
		}

		var pos1 = new Vector2((Box.Center.X + Box.Right) * 0.5f, Box.Bottom);
		TileUtils.VertexDraw_Grid(pos1 - Main.screenPosition, frame, new Vector2(frame.Width / 2f, frame.Height), metro, Main.spriteBatch, 0);
		Main.spriteBatch.Draw(metro, pos1 - Main.screenPosition, glow_frame, new Color(1f, 1f, 1f, 0) * (1 - ShellAlpha), 0, new Vector2(frame.Width / 2f, frame.Height), 1, SpriteEffects.None, 0);
		if (ShellAlpha > 0)
		{
			TileUtils.VertexDraw_Grid(pos1 - Main.screenPosition, outShellFrame, new Vector2(frame.Width / 2f, frame.Height), metro, Main.spriteBatch, 0, ShellAlpha);
		}

		var headFrame = new Rectangle(0, 0, 172, 160);
		var tailFrame = new Rectangle(1024, 0, 172, 160);

		var pos2 = new Vector2(Box.Left, Box.Bottom - 75);
		var pos3 = new Vector2(Box.Right, Box.Bottom - 75);
		TileUtils.VertexDraw_Grid(pos2 - Main.screenPosition, headFrame, new Vector2(headFrame.Width, headFrame.Height * 0.5f), metro, Main.spriteBatch, 0);
		TileUtils.VertexDraw_Grid(pos3 - Main.screenPosition, tailFrame, new Vector2(0, tailFrame.Height * 0.5f), metro, Main.spriteBatch, 0);

		headFrame.Y += 330;
		tailFrame.Y += 330;
		Main.spriteBatch.Draw(metro, pos2 - Main.screenPosition, headFrame, new Color(1f, 1f, 1f, 0) * 0.36f, 0, new Vector2(headFrame.Width, headFrame.Height * 0.5f), 1, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(metro, pos3 - Main.screenPosition, tailFrame, new Color(1f, 1f, 1f, 0) * 0.36f, 0, new Vector2(0, tailFrame.Height * 0.5f), 1, SpriteEffects.None, 0);
	}

	public void CheckPlayerInTrain()
	{
		if (PlayerIntersect())
		{
			if (ShellAlpha > 0)
			{
				ShellAlpha -= 0.04f;
			}
			else
			{
				ShellAlpha = 0;
			}
		}
		else
		{
			if (ShellAlpha < 1)
			{
				ShellAlpha += 0.04f;
			}
			else
			{
				ShellAlpha = 1;
			}
		}
	}

	public bool PlayerIntersect()
	{
		Rectangle hitBox = new Rectangle((int)Position.X, (int)Position.Y - 146, (int)Box.Width, 146);
		return hitBox.Contains((int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y);
	}

	public void RightClick()
	{
	}

	public override Vector2 StandAccelerate(IBox obj)
	{
		return base.StandAccelerate(obj) * 2;
	}
}
