using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(YggdrasilTownAccessGate_CrackPipeline))]
public class YggdrasilTownAccessGate_VFX : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public float DecayTimer;

	public float DecayTimeMax = 30;

	public float CoolingTimer;

	public Dictionary<int, Rectangle> PlayersHitboxOld = new Dictionary<int, Rectangle>();

	public int GateHeight = 1;

	public override void Update()
	{
		base.Update();
		Position = OriginTilePos.ToWorldCoordinates();
		var tile = TileUtils.SafeGetTile(OriginTilePos);
		for (int h = 1; h < 1000; h++)
		{
			var barrierTile = TileUtils.SafeGetTile(OriginTilePos + new Point(0, h));
			if (barrierTile.HasTile && (!barrierTile.HasTile || barrierTile.TileType != ModContent.TileType<YggdrasilTownAccessGate_Barrier>()))
			{
				GateHeight = h;
				break;
			}
		}

		// Magic number
		if (tile.TileFrameY == 768)
		{
			CoolingTimer = 300;
			tile.TileFrameY = 0;
		}
		if (CoolingTimer > 0)
		{
			if (DecayTimer > 0)
			{
				DecayTimer--;
			}
			else
			{
				CoolingTimer--;
			}

			Rectangle frame = new Rectangle((int)Position.X - 8, (int)Position.Y, 16, GateHeight * 16);
			foreach (var k in PlayersHitboxOld.Keys)
			{
				if (Main.player[k] is not null && frame.Intersects(PlayersHitboxOld[k]) && !frame.Intersects(Main.player[k].Hitbox))
				{
					CoolingTimer = 0;
				}
			}
			PlayersHitboxOld = new Dictionary<int, Rectangle>();
			foreach (var player in Main.player)
			{
				if (frame.Intersects(player.Hitbox))
				{
					PlayersHitboxOld.Add(player.whoAmI, player.Hitbox);
				}
			}
		}
		else
		{
			if (DecayTimer < DecayTimeMax)
			{
				DecayTimer++;
			}
		}
		if (DecayTimer >= DecayTimeMax * 0.3f)
		{
			for (int h = 1; h < GateHeight; h++)
			{
				var barrierTile = TileUtils.SafeGetTile(OriginTilePos + new Point(0, h));
				barrierTile.TileType = (ushort)ModContent.TileType<YggdrasilTownAccessGate_Barrier>();
				barrierTile.HasTile = true;
			}
		}
		else
		{
			for (int h = 1; h < GateHeight; h++)
			{
				var barrierTile = TileUtils.SafeGetTile(OriginTilePos + new Point(0, h));
				if (barrierTile.HasTile && barrierTile.TileType == ModContent.TileType<YggdrasilTownAccessGate_Barrier>())
				{
					barrierTile.HasTile = false;
				}
			}
		}
	}

	public override void Draw()
	{
		List<Vertex2D> bars = new List<Vertex2D>();
		int posY = 0;
		Color drawColor = new Color(0.2f, 0.6f, 1f, 0f);
		for (int h = 0; h <= GateHeight; h++)
		{
			bars.Add(Position + new Vector2(-8, h * 16 - 4), drawColor, new Vector3(0, h * 16 / 60f, DecayTimer / DecayTimeMax));
			bars.Add(Position + new Vector2(8, h * 16 - 4), drawColor, new Vector3(1, h * 16 / 60f, DecayTimer / DecayTimeMax));
			var tile = TileUtils.SafeGetTile(OriginTilePos + new Point(0, h));
			posY = h;
			Lighting.AddLight(Position + new Vector2(0, h * 16 - 4), new Vector3(0.3f, 0.75f, 1f) * DecayTimer / DecayTimeMax);
		}
		bars.Add(Position + new Vector2(-8, posY * 16 - 4), drawColor, new Vector3(0, posY * 16 / 60f, DecayTimer / DecayTimeMax));
		bars.Add(Position + new Vector2(8, posY * 16 - 4), drawColor, new Vector3(1, posY * 16 / 60f, DecayTimer / DecayTimeMax));
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
