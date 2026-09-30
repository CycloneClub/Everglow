using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.ProjectileEffects;

[Pipeline(typeof(ColorDissolvePipeline))]
public class MiningPowerPickaxe_Proj_TileBound : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public List<Point16> CurrentTiles = [];

	public Dictionary<Point16, Rectangle> InitialTile_Frames = new Dictionary<Point16, Rectangle>();

	public float Value;

	public Player Owner;

	public int OreType;

	public bool ShouldKill = false;

	public override void Update()
	{
		if (Owner is null || Owner.HeldItem is null || Owner.HeldItem.type != ModContent.ItemType<MiningPowerPickaxe>() || !Owner.controlUseItem)
		{
			ShouldKill = true;
		}
		if (!ShouldKill)
		{
			if (Value < 40)
			{
				Value++;
			}
		}
		else
		{
			Value--;
		}
		if (Value <= 0)
		{
			Active = false;
			return;
		}
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.MiningPowerPickaxe_Proj_TileBound.Value;
		List<Vertex2D> bars = new List<Vertex2D>();
		foreach (var pos in CurrentTiles)
		{
			if (InitialTile_Frames.Count < CurrentTiles.Count)
			{
				InitialTile_Frames[pos] = GetFrame(pos);
			}
		}

		foreach (var pos in InitialTile_Frames.Keys)
		{
			Rectangle frame = InitialTile_Frames[pos];
			var color = Color.Lerp(new Color(1f, 1f, 1f, 0.4f), Lighting.GetColor(pos.ToPoint()), 0.5f) * 0.5f;
			bars.Add(pos.ToVector2() * 16, color, new Vector3(frame.X / (float)tex.Width, frame.Y / (float)tex.Height, Value / 30f));
			bars.Add(pos.ToVector2() * 16 + new Vector2(16, 0), color, new Vector3((frame.X + frame.Width) / (float)tex.Width, frame.Y / (float)tex.Height, Value / 30f));
			bars.Add(pos.ToVector2() * 16 + new Vector2(16, 16), color, new Vector3((frame.X + frame.Width) / (float)tex.Width, (frame.Y + frame.Height) / (float)tex.Height, Value / 30f));

			bars.Add(pos.ToVector2() * 16, color, new Vector3(frame.X / (float)tex.Width, frame.Y / (float)tex.Height, Value / 30f));
			bars.Add(pos.ToVector2() * 16 + new Vector2(16, 16), color, new Vector3((frame.X + frame.Width) / (float)tex.Width, (frame.Y + frame.Height) / (float)tex.Height, Value / 30f));
			bars.Add(pos.ToVector2() * 16 + new Vector2(0, 16), color, new Vector3(frame.X / (float)tex.Width, (frame.Y + frame.Height) / (float)tex.Height, Value / 30f));
		}

		foreach (var pos in CurrentTiles)
		{
			var tile = TileUtils.SafeGetTile(pos);
			Rectangle frame = GetFrame(pos);
			var color = Color.Lerp(new Color(1f, 1f, 1f, 0.4f), Lighting.GetColor(pos.ToPoint()), 0.5f);
			bars.Add(pos.ToVector2() * 16, color, new Vector3(frame.X / (float)tex.Width, frame.Y / (float)tex.Height, Value / 30f));
			bars.Add(pos.ToVector2() * 16 + new Vector2(16, 0), color, new Vector3((frame.X + frame.Width) / (float)tex.Width, frame.Y / (float)tex.Height, Value / 30f));
			bars.Add(pos.ToVector2() * 16 + new Vector2(16, 16), color, new Vector3((frame.X + frame.Width) / (float)tex.Width, (frame.Y + frame.Height) / (float)tex.Height, Value / 30f));

			bars.Add(pos.ToVector2() * 16, color, new Vector3(frame.X / (float)tex.Width, frame.Y / (float)tex.Height, Value / 30f));
			bars.Add(pos.ToVector2() * 16 + new Vector2(16, 16), color, new Vector3((frame.X + frame.Width) / (float)tex.Width, (frame.Y + frame.Height) / (float)tex.Height, Value / 30f));
			bars.Add(pos.ToVector2() * 16 + new Vector2(0, 16), color, new Vector3(frame.X / (float)tex.Width, (frame.Y + frame.Height) / (float)tex.Height, Value / 30f));
		}
		Ins.Batch.Draw(tex, bars, PrimitiveType.TriangleList);
	}

	public Rectangle GetFrame(Point16 pos)
	{
		var tile = TileUtils.SafeGetTile(pos);
		Rectangle frame = new Rectangle(18, 18, 16, 16);
		var tileLeft = TileUtils.SafeGetTile(pos + new Point16(-1, 0));
		var tileRight = TileUtils.SafeGetTile(pos + new Point16(1, 0));
		var tileUp = TileUtils.SafeGetTile(pos + new Point16(0, -1));
		var tileDown = TileUtils.SafeGetTile(pos + new Point16(0, 1));

		bool boundLeft = !tileLeft.HasTile || tileLeft.TileType != tile.TileType || !CurrentTiles.Contains(pos + new Point16(-1, 0));
		bool boundRight = !tileRight.HasTile || tileRight.TileType != tile.TileType || !CurrentTiles.Contains(pos + new Point16(1, 0));
		bool boundUp = !tileUp.HasTile || tileUp.TileType != tile.TileType || !CurrentTiles.Contains(pos + new Point16(0, -1));
		bool boundDown = !tileDown.HasTile || tileDown.TileType != tile.TileType || !CurrentTiles.Contains(pos + new Point16(0, 1));
		if (boundLeft)
		{
			frame.X = 0;
		}
		if (boundRight)
		{
			frame.X = 36;
		}
		if (boundUp)
		{
			frame.Y = 0;
		}
		if (boundDown)
		{
			frame.Y = 36;
		}
		if (boundLeft && boundRight)
		{
			frame.X = 54;
		}
		if (boundUp && boundDown)
		{
			frame.Y = 54;
		}
		return frame;
	}
}
