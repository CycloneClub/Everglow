using Everglow.Commons.Enums;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;
using Everglow.Commons.VFX.Scene;

namespace Everglow.CagedDomain.Tiles.Escalator;

/// <summary>
/// An interactive escalator.
/// It connect the NormalEscalator and NormalEscalator_Top tiles by an oblique escalator.
/// Player should control 'Up' to attach to the escalator, and 'Down' to exit the escalator.
/// </summary>
[Pipeline(typeof(WCSPipeline))]
public class NormalEscalator_VFX : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public Rectangle HanddrailStrap_Bottom = new Rectangle(0, 0, 88, 54);
	public Rectangle HanddrailStrap_Medium = new Rectangle(92, 0, 44, 44);
	public Rectangle HanddrailStrap_Top = new Rectangle(140, 0, 70, 44);

	public Rectangle ExteriorPanel_Bottom = new Rectangle(0, 60, 58, 16);
	public Rectangle ExteriorPanel_Medium_Bottom = new Rectangle(136, 44, 30, 32);
	public Rectangle ExteriorPanel_Medium = new Rectangle(80, 48, 26, 28);
	public Rectangle ExteriorPanel_Medium_Top = new Rectangle(108, 50, 24, 24);
	public Rectangle ExteriorPanel_Top = new Rectangle(166, 46, 44, 16);

	public Rectangle SelectedInterfaceHighlight = new Rectangle(166, 64, 18, 10);

	public Rectangle StepBlock = new Rectangle(60, 60, 18, 16);

	public List<Vector2> StepPositions = new List<Vector2>();

	public List<Player> AttachingPlayers = new List<Player>();

	public int TotalLength = 0;

	public float MoveingSpeed = 16f / 30f;

	public float StepValue = 0;

	public int State = 0;

	public enum WorkState
	{
		Upward,
		Downward,
		Cease,
	}

	public override void Update()
	{
		if (TotalLength == 0)
		{
			for (int step = 0; step < 100; step++)
			{
				var tile = WorldGenMisc.SafeGetTile(OriginTilePos + new Point(Direction * step, -step));
				if (tile.TileType == ModContent.TileType<NormalEscalator_Top>())
				{
					TotalLength = step;
					break;
				}
			}
		}
		UpdateSteps();
		UpdatePlayerAttachment();
		UpdateStateControl();
		base.Update();
	}

	public void UpdatePlayerAttachment()
	{
		foreach (var player in Main.player)
		{
			if (player is not null && player.active)
			{
				// TODO: Optimization, check player collide with the exterior rectangle.
				foreach (var blockPos in StepPositions)
				{
					Rectangle stepBox = new Rectangle((int)blockPos.X, (int)blockPos.Y, 16, 16);

					// Control 'Up';Not exist in the list;No mount;Collision;
					if (player.controlUp && !AttachingPlayers.Contains(player) && player.mount.Type == -1 && stepBox.Intersects(player.Hitbox))
					{
						AttachingPlayers.Add(player);
						break;
					}
				}
			}
		}

		foreach (var player in AttachingPlayers)
		{
			if (player is null || !player.active)
			{
				AttachingPlayers.Remove(player);
				break;
			}

			// Control 'Down' to exit the escalator..
			if (player.controlDown)
			{
				AttachingPlayers.Remove(player);
				break;
			}

			bool safe = false;
			for (int i = 0; i < StepPositions.Count; i++)
			{
				Vector2 blockPos = StepPositions[i];
				if (player.Bottom.X >= blockPos.X && player.Bottom.X < blockPos.X + 16)
				{
					// The speed rate constant: depends on the escalator speed (Updated by Main.time / 30f * 16)
					// TODO: Player frame

					int stateDir = 1;
					if (State == (int)WorkState.Downward)
					{
						stateDir = -1;
					}
					if (State == (int)WorkState.Cease)
					{
						stateDir = 0;
					}
					float relativeMoveX = MoveingSpeed * Direction * stateDir;
					if (player.Bottom.Y > blockPos.Y)
					{
						float deltaY = blockPos.Y - player.Bottom.Y;
						if (MathF.Abs(deltaY) > 15)
						{
							player.gfxOffY -= deltaY;
						}
						if (MathF.Abs(player.gfxOffY) > 16)
						{
							player.gfxOffY = 16;
						}
						player.Bottom = new Vector2(player.Bottom.X + relativeMoveX, blockPos.Y);
						if (player.bodyFrame.Y == 280)
						{
							player.bodyFrame.Y = 0;
							player.legFrame.Y = 0;
						}
					}
					safe = true;
					break;
				}
			}
			if (!safe)
			{
				AttachingPlayers.Remove(player);
				break;
			}
		}
	}

	public void UpdateStateControl()
	{
		Rectangle controlBox = new Rectangle((int)Position.X - 1 - 35 * Direction, (int)Position.Y + 6, 18, 6);
		if (controlBox.Contains(Main.MouseWorld.ToPoint()))
		{
			MouseIntersectControlInterface = true;
			if (Main.mouseRight && Main.mouseRightRelease)
			{
				State++;
				if (State > 2)
				{
					State = 0;
				}
				var tile = TileUtils.SafeGetTile(OriginTilePos);
				tile.TileFrameY = (short)(State * 18);
			}
			return;
		}
		MouseIntersectControlInterface = false;
	}

	public bool MouseIntersectControlInterface = false;

	public Rectangle CurrentAnimationPanel()
	{
		int animationValue = (int)(Main.time * 0.1f) % 3;
		return new Rectangle(214, 8 * animationValue + State * 24, 18, 6);
	}

	public Rectangle CurrentAnimationPane_Glow()
	{
		int animationValue = (int)(Main.time * 0.1f) % 3;
		return new Rectangle(216, 8 * animationValue + State * 24, 14, 6);
	}

	public override void Draw()
	{
		if (TotalLength == 0)
		{
			return;
		}
		if (Texture == null)
		{
			Texture = ModAsset.NormalEscalator_Atlas.Value;
		}

		// Handdrail Straps
		Vector2 totalOffset = new Vector2(0);
		Vector2 drawPos = Position + totalOffset;

		DrawAPiece(drawPos + new Vector2(-36 - 26 * Direction, -50), HanddrailStrap_Bottom);
		Vector2 piece_drawPos = drawPos + new Vector2(-14 - 4 * Direction, -50);
		for (int i = 1; i < TotalLength; i++)
		{
			piece_drawPos += new Vector2(Direction * 16, -16);
			DrawAPiece(piece_drawPos, HanddrailStrap_Medium);
		}
		if (Direction == 1)
		{
			DrawAPiece(drawPos + new Vector2(Direction, -1) * 16 * TotalLength + new Vector2(-18 * Direction, -42), HanddrailStrap_Top);
		}
		else
		{
			DrawAPiece(drawPos + new Vector2(Direction, -1) * 16 * TotalLength + new Vector2(-36, -42), HanddrailStrap_Top);
		}

		// Steps
		float stepValue = StepValue % 16;

		if (StepPositions != null && StepPositions.Count > 0)
		{
			for (int i = 0; i < StepPositions.Count; i++)
			{
				Rectangle stepFrame = StepBlock;
				Vector2 offset = Vector2.zeroVector;
				if (i == 0)
				{
					stepFrame = new Rectangle((int)(78 - stepValue), 60, (int)(stepValue + 2), 16);
					if (Direction == -1)
					{
						offset = new Vector2(stepValue - 16, 0);
					}
				}
				DrawAPiece(StepPositions[i] + offset, stepFrame);
			}
		}

		// Exterior Panels
		DrawAPiece(drawPos + new Vector2(-21 - 27 * Direction, 0), ExteriorPanel_Bottom);

		DrawAPiece(drawPos + new Vector2(-1 - 35 * Direction, 6), CurrentAnimationPanel());
		DrawAPieceIgnoreEnvironment(drawPos + new Vector2(1 - 35 * Direction, 6), CurrentAnimationPane_Glow(), new Color(1f, 1f, 1f, 0f));
		if (MouseIntersectControlInterface)
		{
			DrawAPieceIgnoreEnvironment(drawPos + new Vector2(-1 - 35 * Direction, 4), SelectedInterfaceHighlight, new Color(1f, 1f, 1f, 1f));
		}
		piece_drawPos = drawPos + new Vector2(-12, -4);
		if (Direction == -1)
		{
			piece_drawPos = drawPos + new Vector2(2, -4);
		}
		for (int i = 0; i < TotalLength; i++)
		{
			Vector2 offset = Vector2.zeroVector;
			piece_drawPos += new Vector2(Direction * 16, -16);
			Rectangle frame = ExteriorPanel_Medium;
			if (i == 0)
			{
				frame = ExteriorPanel_Medium_Bottom;
				if (Direction == 1)
				{
					offset = new Vector2(-4, 0);
				}
				else
				{
					offset = Vector2.zeroVector;
				}
			}
			if (i == TotalLength - 1)
			{
				frame = ExteriorPanel_Medium_Top;

				if (Direction == 1)
				{
					offset = new Vector2(-2, 4);
				}
				else
				{
					offset = new Vector2(4, 4);
				}
			}
			DrawAPiece(piece_drawPos + offset, frame);
		}

		DrawAPiece(drawPos + new Vector2(Direction, -1) * 16 * TotalLength + new Vector2(-14 + 12 * Direction, -2), ExteriorPanel_Top);
	}

	public void DrawAPiece(Vector2 position, Rectangle frame)
	{
		List<Vertex2D> piece = new List<Vertex2D>();
		if (Direction == 1)
		{
			piece.Add(position, Lighting.GetColor(position.ToTileCoordinates()), new Vector3(frame.TopLeft() / Texture.Size(), 0));
			piece.Add(position + new Vector2(frame.Width, 0), Lighting.GetColor((position + new Vector2(frame.Width, 0)).ToTileCoordinates()), new Vector3(frame.TopRight() / Texture.Size(), 0));

			piece.Add(position + new Vector2(0, frame.Height), Lighting.GetColor((position + new Vector2(0, frame.Height)).ToTileCoordinates()), new Vector3(frame.BottomLeft() / Texture.Size(), 0));
			piece.Add(position + new Vector2(frame.Width, frame.Height), Lighting.GetColor((position + new Vector2(frame.Width, frame.Height)).ToTileCoordinates()), new Vector3(frame.BottomRight() / Texture.Size(), 0));
		}
		else
		{
			piece.Add(position, Lighting.GetColor(position.ToTileCoordinates()), new Vector3(frame.TopRight() / Texture.Size(), 0));
			piece.Add(position + new Vector2(frame.Width, 0), Lighting.GetColor((position + new Vector2(frame.Width, 0)).ToTileCoordinates()), new Vector3(frame.TopLeft() / Texture.Size(), 0));

			piece.Add(position + new Vector2(0, frame.Height), Lighting.GetColor((position + new Vector2(0, frame.Height)).ToTileCoordinates()), new Vector3(frame.BottomRight() / Texture.Size(), 0));
			piece.Add(position + new Vector2(frame.Width, frame.Height), Lighting.GetColor((position + new Vector2(frame.Width, frame.Height)).ToTileCoordinates()), new Vector3(frame.BottomLeft() / Texture.Size(), 0));
		}
		Ins.Batch.Draw(Texture, piece, PrimitiveType.TriangleStrip);
	}

	public void DrawAPieceIgnoreEnvironment(Vector2 position, Rectangle frame, Color color)
	{
		List<Vertex2D> piece = new List<Vertex2D>();
		if (Direction == 1)
		{
			piece.Add(position, color, new Vector3(frame.TopLeft() / Texture.Size(), 0));
			piece.Add(position + new Vector2(frame.Width, 0), color, new Vector3(frame.TopRight() / Texture.Size(), 0));

			piece.Add(position + new Vector2(0, frame.Height), color, new Vector3(frame.BottomLeft() / Texture.Size(), 0));
			piece.Add(position + new Vector2(frame.Width, frame.Height), color, new Vector3(frame.BottomRight() / Texture.Size(), 0));
		}
		else
		{
			piece.Add(position, color, new Vector3(frame.TopRight() / Texture.Size(), 0));
			piece.Add(position + new Vector2(frame.Width, 0), color, new Vector3(frame.TopLeft() / Texture.Size(), 0));

			piece.Add(position + new Vector2(0, frame.Height), color, new Vector3(frame.BottomRight() / Texture.Size(), 0));
			piece.Add(position + new Vector2(frame.Width, frame.Height), color, new Vector3(frame.BottomLeft() / Texture.Size(), 0));
		}
		Ins.Batch.Draw(Texture, piece, PrimitiveType.TriangleStrip);
	}

	public void UpdateSteps()
	{
		StepPositions = new List<Vector2>();
		if (State == (int)WorkState.Upward)
		{
			StepValue += MoveingSpeed;
			if (StepValue > 32000)
			{
				StepValue -= 16000;
			}
		}
		else if (State == (int)WorkState.Downward)
		{
			StepValue -= MoveingSpeed;
			if (StepValue < 0)
			{
				StepValue += 16000;
			}
		}
		float stepValue = StepValue % 16;
		Vector2 piece_drawPos = Position + new Vector2(-76 * Direction, 62) + new Vector2(Direction, -1) * stepValue;
		for (int i = -3; i < TotalLength + 1; i++)
		{
			Vector2 offset = Vector2.zeroVector;
			if (i == TotalLength)
			{
				offset = new Vector2(0, stepValue);
			}
			if (i <= -1)
			{
				offset = new Vector2(0, stepValue + 16 * i);
			}
			if (i == -3)
			{
				offset += new Vector2(16 - stepValue, 0);
			}
			piece_drawPos += new Vector2(Direction * 16, -16);
			StepPositions.Add(piece_drawPos + offset);
		}
	}
}
