using Everglow.Commons.CustomTiles.Core;
using Everglow.Commons.DataStructures;
using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.TileHelper;

public class NormalCableCar : BoxEntity
{
	public int RopeDuration;
	public Point AnchorCableTile;
	public Rope Cable;
	public int Direction = 1;
	public bool Initialized = false;
	public List<int> InsidePlayers = new List<int>();

	public override void SetDefaults()
	{
		Size = new Vector2(80, 80);
		Direction = -1;
	}

	public override void AI()
	{
		if (!Initialized)
		{
			ChangeCableCarJoint();
			Initialized = true;
		}
		if (Cable != null && Cable != default)
		{
			float minLength = 2500;
			int minIndex = 0;

			// find the closest rope joint unit.
			for (int i = 0; i < Cable.Masses.Length; i++)
			{
				float thisLength = (Cable.Masses[i].Position - Position).Length();
				if ((Cable.Masses[i].Position - Position).Length() < minLength)
				{
					minLength = thisLength;
					minIndex = i;
				}
			}
			Vector2 closestPos = Cable.Masses[minIndex].Position;
			Vector2 targetMassPos = Cable.Masses[Math.Clamp(minIndex + Direction, 0, Cable.Masses.Length - 1)].Position;

			// If depart from the cable over 3 unit, force postion to the cable.
			Vector2 deltaPos = targetMassPos - closestPos;
			Vector2 normalizedDelta = Utils.SafeNormalize(deltaPos, new Vector2(0, -1));
			Vector2 toClosestPos = Position - closestPos;
			float projectionLength = toClosestPos.Length() * Vector2.Dot(toClosestPos, deltaPos) / (toClosestPos.Length() * deltaPos.Length());
			Vector2 projection = closestPos + projectionLength * normalizedDelta;
			float toVelocityLineLength = (projection - Position).Length();
			if (toVelocityLineLength > 1)
			{
				Position = projection;
			}
			Velocity = Utils.SafeNormalize(targetMassPos + Velocity - closestPos, Vector2.zeroVector) * 3f;
			if (Direction == 1 && RopeDuration >= Cable.Masses.Length)
			{
				ChangeCableCarJoint();
			}
			if (Direction == -1 && RopeDuration <= 1)
			{
				ChangeCableCarJoint();
			}
			RopeDuration = minIndex + 1;
		}
		else
		{
			Kill();
			return;
		}
		CheckMouseClick();
		foreach (Player player in Main.player)
		{
			if (InsidePlayers.Contains(player.whoAmI))
			{
				player.Center = Position + Size * 0.5f + new Vector2(0, 60);
			}
		}
		Lighting.AddLight(Position + Size * 0.5f, new Vector3(1f, 0.9f, 0.6f));
		base.AI();
	}

	public void CheckMouseClick()
	{
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			if (Main.MouseWorld.X > Position.X && Main.MouseWorld.X < Position.X + Size.X)
			{
				if (Main.MouseWorld.Y > Position.Y && Main.MouseWorld.Y < Position.Y + Size.Y)
				{
					if (!InsidePlayers.Contains(Main.myPlayer))
					{
						InsidePlayers.Add(Main.myPlayer);
						CombatText.NewText(new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 0, 0), Color.White, "Now you sit in the cable car.");
					}
					else
					{
						InsidePlayers.Remove(Main.myPlayer);
						CombatText.NewText(new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 0, 0), Color.White, "Now you left the cable car.");
					}
				}
			}
		}
	}

	public void ChangeCableCarJoint()
	{
		Point pointPos = Position.ToTileCoordinates();
		pointPos.X = Math.Clamp(pointPos.X, 20, Main.maxTilesX - 20);
		pointPos.Y = Math.Clamp(pointPos.Y, 20, Main.maxTilesY - 20);

		// find a closest cable tile in a 5*5 area near by cable car entity.
		float minDis = 4;
		Point minDisPoint = new Point(114, 514);
		for (int i = -2; i < 3; i++)
		{
			for (int j = -2; j < 3; j++)
			{
				Point newPointPos = pointPos + new Point(i, j);
				Tile tile = Main.tile[newPointPos];
				if (tile.HasTile && tile.TileType == ModContent.TileType<CableCarJoint>())
				{
					float distance = new Vector2(i, j).Length();
					if (distance < minDis)
					{
						minDis = distance;
						minDisPoint = new Point(i, j);
					}
				}
			}
		}
		if (minDisPoint != new Point(114, 514))
		{
			Point newPointPos = pointPos + minDisPoint;
			Tile tile = Main.tile[newPointPos];
			if (tile.HasTile && tile.TileType == ModContent.TileType<CableCarJoint>())
			{
				AnchorCableTile = newPointPos;

				// the cable pos is the previous result while direction = 1.
				CableTile cableCarJoint = TileLoader.GetTile(tile.TileType) as CableTile;
				if (Direction == -1)
				{
					// else, chose a random rope that end at previous result.
					// inasmuch as the cable tile data structure design, a cable joint tile can spread more than 1 rope but can only receive 1 at most.
					List<Point> points = new List<Point>();
					foreach (Point point in cableCarJoint.RopeHeadAndTail.Keys)
					{
						if (cableCarJoint.RopeHeadAndTail[point] == newPointPos)
						{
							points.Add(point);
						}
					}
					if (points.Count > 0)
					{
						int randIndex = Main.rand.Next(points.Count);
						newPointPos = points[randIndex];
					}
					else
					{
						Velocity *= 0;
						Cable = null;
						return;
					}
				}
				cableCarJoint.RopesOfAllThisTileInTheWorld.TryGetValue(newPointPos, out Cable);
			}
		}
		else
		{
			Velocity *= 0;
		}
	}

	public override Color MapColor => new Color(73, 68, 65);

	public override void Draw()
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
		Effect effect = ModAsset.Shader2D.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Texture2D cableCar = ModAsset.NormalCableCar.Value;
		Vector2 drawPos = Position + new Vector2(Size.X * 0.5f, 8);

		var bars = new List<Vertex2D>();
		AddVertex(bars, drawPos + new Vector2(-2, -8), new Vector3(0.5f, 0.05f, 0));
		AddVertex(bars, drawPos + new Vector2(2, -8), new Vector3(0.5f, 0.05f, 0));
		AddVertex(bars, drawPos + new Vector2(-2, 16), new Vector3(0.5f, 0.05f, 0));

		AddVertex(bars, drawPos + new Vector2(-2, 16), new Vector3(0.5f, 0.05f, 0));
		AddVertex(bars, drawPos + new Vector2(2, -8), new Vector3(0.5f, 0.05f, 0));
		AddVertex(bars, drawPos + new Vector2(2, 16), new Vector3(0.5f, 0.05f, 0));

		AddVertex(bars, drawPos + new Vector2(-43, 16), new Vector3(0f, 18 / 134f, 0));
		AddVertex(bars, drawPos + new Vector2(43, 16), new Vector3(1f, 18 / 134f, 0));
		AddVertex(bars, drawPos + new Vector2(-43, 132), new Vector3(0f, 1, 0));

		AddVertex(bars, drawPos + new Vector2(-43, 132), new Vector3(0f, 1, 0));
		AddVertex(bars, drawPos + new Vector2(43, 16), new Vector3(1f, 18 / 134f, 0));
		AddVertex(bars, drawPos + new Vector2(43, 132), new Vector3(1f, 1, 0));
		Main.graphics.graphicsDevice.Textures[0] = cableCar;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);

		// glow
		Texture2D cableCarGlow = ModAsset.NormalCableCar_glow.Value;
		Color glowColor = new Color(1f, 1f, 1f, 0) * 0.6f;
		bars = new List<Vertex2D>();
		bars.Add(drawPos + new Vector2(-43, 16), glowColor, new Vector3(0f, 18 / 134f, 0));
		bars.Add(drawPos + new Vector2(43, 16), glowColor, new Vector3(1f, 18 / 134f, 0));

		bars.Add(drawPos + new Vector2(-43, 132), glowColor, new Vector3(0f, 1f, 0));
		bars.Add(drawPos + new Vector2(43, 132), glowColor, new Vector3(1f, 1f, 0));

		Main.graphics.graphicsDevice.Textures[0] = cableCarGlow;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 pos, Vector3 coord)
	{
		bars.Add(pos, Lighting.GetColor(pos.ToTileCoordinates()), coord);
	}
}
