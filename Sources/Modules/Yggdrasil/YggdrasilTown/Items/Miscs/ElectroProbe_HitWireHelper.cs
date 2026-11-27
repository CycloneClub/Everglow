namespace Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;

[Pipeline(typeof(WCSPipeline))]
public class ElectroProbe_HitWireHelper : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public Player MyOwner;

	public Item MyProbe;

	public Point FixPoint;

	public float Power = 0;

	public override void OnSpawn()
	{
		base.OnSpawn();
	}

	public override void Update()
	{
		if (MyOwner == null || !MyOwner.active || MyOwner.dead)
		{
			Active = false;
			return;
		}
		if (MyProbe.ModItem is ElectroProbe eProbe)
		{
			FixPoint = Main.MouseWorld.ToTileCoordinates();
			eProbe.OwnHelperCooling++;
			if (eProbe.OwnHelperCooling >= 10)
			{
				eProbe.OwnHelperCooling = 10;
			}
			if (MyOwner.HeldItem.type != ModContent.ItemType<ElectroProbe>())
			{
				Active = false;
				eProbe.OwnHelperCooling = 0;
				return;
			}
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				Power += 1f;
			}
			else
			{
				Power -= 0.1f;
				if (Power <= 0)
				{
					Power = 0;
				}
			}
		}
		else
		{
			Active = false;
			return;
		}
	}

	public override void Draw()
	{
		Color drawColor = Color.White * (Power + 0.25f);
		if ((FixPoint.ToWorldCoordinates() - MyOwner.Center).Length() >= 300)
		{
			drawColor = new Color(0.5f, 0f, 0f, 0.3f);
		}
		DrawBlockBound(FixPoint.X, FixPoint.Y, drawColor);
	}

	public void DrawBlockBound(int i, int j, Color color)
	{
		bool overFar = false;
		if ((FixPoint.ToWorldCoordinates() - MyOwner.Center).Length() >= 300)
		{
			overFar = true;
		}
		Vector2 pos = new Vector2(i, j) * 16;
		Color drawColor = Color.White * 0.3f;
		if (overFar)
		{
			drawColor = new Color(0.5f, 0f, 0f, 0.3f);
		}
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(pos, drawColor, new Vector3(0, 0, 0)),
			new Vertex2D(pos + new Vector2(16, 0), drawColor, new Vector3(1, 0, 0)),
			new Vertex2D(pos + new Vector2(0, 16), drawColor, new Vector3(0, 1, 0)),

			new Vertex2D(pos + new Vector2(0, 16), drawColor, new Vector3(0, 1, 0)),
			new Vertex2D(pos + new Vector2(16, 0), drawColor, new Vector3(1, 0, 0)),
			new Vertex2D(pos + new Vector2(16), drawColor, new Vector3(1, 1, 0)),
		};

		Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, bars, PrimitiveType.TriangleList);
		if (!overFar)
		{
			if (Main.tile[i, j].RedWire)
			{
				float size = 16f + ((float)Main.timeForVisualEffects / 4f) % 10f;
				size *= 0.5f;
				drawColor = new Color(1f, 0f, 0f, 0) * ((13 - size) / 5f);
				Vector2 newPos = pos + new Vector2(8);
				bars = new List<Vertex2D>()
				{
					new Vertex2D(newPos + new Vector2(-size, -size), drawColor, new Vector3(0, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, -size), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(-size, size), drawColor, new Vector3(0, 1, 0)),

					new Vertex2D(newPos + new Vector2(-size, size), drawColor, new Vector3(0, 1, 0)),
					new Vertex2D(newPos + new Vector2(size, -size), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, size), drawColor, new Vector3(1, 1, 0)),
				};
				Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, bars, PrimitiveType.TriangleList);
			}
			if (Main.tile[i, j].GreenWire)
			{
				float size = 16f + ((float)(Main.timeForVisualEffects + 10) / 4f) % 10f;
				size *= 0.5f;
				drawColor = new Color(0f, 1f, 0f, 0) * ((13 - size) / 5f);
				Vector2 newPos = pos + new Vector2(8);
				bars = new List<Vertex2D>()
				{
					new Vertex2D(newPos + new Vector2(-size, -size), drawColor, new Vector3(0, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, -size), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(-size, size), drawColor, new Vector3(0, 1, 0)),

					new Vertex2D(newPos + new Vector2(-size, size), drawColor, new Vector3(0, 1, 0)),
					new Vertex2D(newPos + new Vector2(size, -size), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, size), drawColor, new Vector3(1, 1, 0)),
				};
				Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, bars, PrimitiveType.TriangleList);
			}
			if (Main.tile[i, j].BlueWire)
			{
				float size = 16f + ((float)(Main.timeForVisualEffects + 20) / 4f) % 10f;
				size *= 0.5f;
				drawColor = new Color(0f, 0.3f, 1f, 0.5f) * ((13 - size) / 5f) * 2;
				Vector2 newPos = pos + new Vector2(8);
				bars = new List<Vertex2D>()
				{
					new Vertex2D(newPos + new Vector2(-size, -size), drawColor, new Vector3(0, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, -size), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(-size, size), drawColor, new Vector3(0, 1, 0)),

					new Vertex2D(newPos + new Vector2(-size, size), drawColor, new Vector3(0, 1, 0)),
					new Vertex2D(newPos + new Vector2(size, -size), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, size), drawColor, new Vector3(1, 1, 0)),
				};
				Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, bars, PrimitiveType.TriangleList);
			}
			if (Main.tile[i, j].YellowWire)
			{
				float size = 16f + ((float)(Main.timeForVisualEffects + 30) / 4f) % 10f;
				size *= 0.5f;
				drawColor = Color.Yellow * ((13 - size) / 5f);
				Vector2 newPos = pos + new Vector2(8);
				bars = new List<Vertex2D>()
				{
					new Vertex2D(newPos + new Vector2(-size, -size), drawColor, new Vector3(0, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, -size), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(-size, size), drawColor, new Vector3(0, 1, 0)),

					new Vertex2D(newPos + new Vector2(-size, size), drawColor, new Vector3(0, 1, 0)),
					new Vertex2D(newPos + new Vector2(size, -size), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, size), drawColor, new Vector3(1, 1, 0)),
				};
				Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, bars, PrimitiveType.TriangleList);
			}

			bars = new List<Vertex2D>()
			{
				new Vertex2D(pos, color, new Vector3(0, 0, 0)),
				new Vertex2D(pos + new Vector2(16, 0), color, new Vector3(1, 0, 0)),
				new Vertex2D(pos + new Vector2(0, 16), color, new Vector3(0, 1, 0)),

				new Vertex2D(pos + new Vector2(0, 16), color, new Vector3(0, 1, 0)),
				new Vertex2D(pos + new Vector2(16, 0), color, new Vector3(1, 0, 0)),
				new Vertex2D(pos + new Vector2(16), color, new Vector3(1, 1, 0)),
			};
			Ins.Batch.Draw(ModAsset.ElectroProbe_Mark.Value, bars, PrimitiveType.TriangleList);

			if (Power > 0)
			{
				float size = 16f + (1 - Power) * 24f;
				size *= 0.5f;
				drawColor = new Color(Power, Power, Power, 0);
				Vector2 newPos = pos + new Vector2(8);
				bars = new List<Vertex2D>()
				{
					new Vertex2D(newPos + new Vector2(-size, -size).RotatedBy(MathHelper.PiOver4), drawColor, new Vector3(0, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, -size).RotatedBy(MathHelper.PiOver4), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(-size, size).RotatedBy(MathHelper.PiOver4), drawColor, new Vector3(0, 1, 0)),

					new Vertex2D(newPos + new Vector2(-size, size).RotatedBy(MathHelper.PiOver4), drawColor, new Vector3(0, 1, 0)),
					new Vertex2D(newPos + new Vector2(size, -size).RotatedBy(MathHelper.PiOver4), drawColor, new Vector3(1, 0, 0)),
					new Vertex2D(newPos + new Vector2(size, size).RotatedBy(MathHelper.PiOver4), drawColor, new Vector3(1, 1, 0)),
				};
			}
			Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, bars, PrimitiveType.TriangleList);
		}
	}
}
