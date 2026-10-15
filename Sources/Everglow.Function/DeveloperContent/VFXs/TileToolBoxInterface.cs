using Everglow.Commons.DeveloperContent.Items;
using Everglow.Commons.Enums;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;
using Terraria.GameContent;
using static Everglow.Commons.Utilities.MathUtils;
using static Everglow.Commons.Utilities.TileUtils;
using static Everglow.Commons.VFX.CommonDusts.LightDust;

namespace Everglow.Commons.DeveloperContent.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class TileToolBoxInterface : Visual
{
	public const int TotalTool = 12;

	public Player Owner = null;
	public Vector2 Position;
	public int CurrentTileType;
	public int CurrentWallType = 0;
	public int SelectedTool = -1;
	public int MouseOverTool = -1;
	public int Timer;
	public int PlayerHeldItemTile = -1;
	public int PlayerHeldItemWall = -1;

	/// <summary>
	/// 0: Tile, 1: Wall
	/// </summary>
	public int State = 0;
	public bool Open = true;
	public bool ShouldDrawMouseTlie = false;
	public bool ShouldDrawBrushCoveredTlies = false;
	public bool ShouldDrawSelectedTlies = false;
	public bool RectangleSelecting = false;
	public bool Brushing = false;
	public bool ClickAnyButtonInThisFrame = false;
	public bool CircleSelecting = false;
	public Point RectangleSelectStart;
	public Point RectangleSelectEnd;
	public Point CircleSelectCenter;
	public float CircleSelectRadius;
	public float BrushSize = 16;
	public string MouseText;

	public enum ToolID
	{
		TilePicker,
		TileColorBoard,
		PaintBucket,
		MagicWand,
		Undo,
		Redo,
		RectangleSelect,
		PolygonSelect,
		CircleSelect,
		Unselect,
		PaintBrush,
		SwitchTileAndWall,
		None,
		History,
		Smoothe,
	}

	public class ToolButton
	{
		public float Scale;
		public Vector2 Position;
		public int Type;
	}

	public List<ToolButton> Tools = [];
	public List<Point> SelectedTiles = [];
	public List<Point> CurrentSelectedTiles = [];
	public List<Point> BrushCoveredTiles = [];
	public List<Point> BrushPaintedTiles = [];
	public List<Point> PolygonSelectPoints = [];
	public static Stack<(MapIO Mp, string Pt)> UndoMapIOs = [];
	public static Stack<(MapIO Mp, string Pt)> RedoMapIOs = [];

	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public override void Update()
	{
		if (Tools.Count <= 0)
		{
			Initialization();
		}
		UpdateActive();
		if (ClickAnyButtonInThisFrame && Main.mouseLeftRelease)
		{
			ClickAnyButtonInThisFrame = false;
		}

		if (Main.mouseRight && Main.mouseRightRelease)
		{
			UpdateRightClick();
		}
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			UpdateLeftClick();
		}
		UpdateTools();
	}

	/// <summary>
	/// Initialize the tool buttons. This method is called once when the visual is first updated, and it populates the Tools list with the necessary buttons for the interface.
	/// </summary>
	public void Initialization()
	{
		for (int k = 0; k < TotalTool; k++)
		{
			Tools.Add(new ToolButton()
			{
				Position = Vector2.zeroVector,
				Scale = 1f,
				Type = k,
			});
		}
	}

	public void UpdateActive()
	{
		if (Owner.HeldItem is not null)
		{
			if (State == 0)
			{
				if (Owner.HeldItem.createTile != PlayerHeldItemTile && Owner.HeldItem.createTile >= 0)
				{
					CurrentTileType = Owner.HeldItem.createTile;
				}
			}
			PlayerHeldItemTile = Owner.HeldItem.createTile;
			if (State == 1)
			{
				if (Owner.HeldItem.createWall != PlayerHeldItemWall && Owner.HeldItem.createWall >= 1)
				{
					CurrentWallType = Owner.HeldItem.createWall;
				}
			}
			PlayerHeldItemWall = Owner.HeldItem.createWall;
		}
		else
		{
			PlayerHeldItemTile = -1;
		}
		if (Open)
		{
			if (Timer < 60)
			{
				Timer += 6;
			}
		}
		else
		{
			if (Timer > 0)
			{
				Timer -= 6;
			}
			else
			{
				Active = false;
				return;
			}
		}
	}

	public void UpdateTools()
	{
		for (int t = Tools.Count - 1; t >= 0; t--)
		{
			var ui = Tools[t];
			float value = Timer / 60f;
			value = MathF.Pow(value, 0.5f);
			ui.Position = new Vector2(0, value * TotalTool * 6).RotatedBy(ui.Type / (float)Tools.Count * MathHelper.TwoPi - 1 + value);
			Tools[t] = ui;
		}
		ShouldDrawMouseTlie = false;
		ShouldDrawBrushCoveredTlies = false;
		ShouldDrawSelectedTlies = false;
		switch (SelectedTool)
		{
			case (int)ToolID.TilePicker:
				ShouldDrawMouseTlie = true;
				break;
			case (int)ToolID.RectangleSelect:
				ShouldDrawSelectedTlies = true;
				ShouldDrawMouseTlie = true;
				if (!ClickAnyButtonInThisFrame)
				{
					if (Main.mouseLeft)
					{
						if (!RectangleSelecting)
						{
							RectangleSelecting = true;
							RectangleSelectStart = Main.MouseWorld.ToTileCoordinates();
						}
						RectangleSelectEnd = Main.MouseWorld.ToTileCoordinates();
						CurrentSelectedTiles = GetAABBAreaOfTile(RectangleSelectStart, RectangleSelectEnd);
					}
					else
					{
						RectangleSelecting = false;
						SelectedTiles.AddRangeDistinct(CurrentSelectedTiles);
						CurrentSelectedTiles = [];
					}
				}
				break;
			case (int)ToolID.PolygonSelect:
				ShouldDrawSelectedTlies = true;
				ShouldDrawMouseTlie = true;
				break;
			case (int)ToolID.CircleSelect:
				ShouldDrawSelectedTlies = true;
				ShouldDrawMouseTlie = true;
				if (!ClickAnyButtonInThisFrame)
				{
					if (Main.mouseLeft)
					{
						if (!CircleSelecting)
						{
							CircleSelecting = true;
							var point = Main.MouseWorld.ToTileCoordinates();
							CircleSelectCenter = point;
						}
						CircleSelectRadius = (CircleSelectCenter.ToWorldCoordinates() - Main.MouseWorld).Length() / 16f;
						CurrentSelectedTiles = GetCircleAreaOfTilePos(CircleSelectCenter, CircleSelectRadius);
					}
					else
					{
						CircleSelecting = false;
						SelectedTiles.AddRangeDistinct(CurrentSelectedTiles);
						CurrentSelectedTiles = [];
					}
				}
				break;
			case (int)ToolID.MagicWand:
				ShouldDrawSelectedTlies = true;
				break;
			case (int)ToolID.PaintBucket:
				ShouldDrawSelectedTlies = true;
				break;
			case (int)ToolID.PaintBrush:
				ShouldDrawBrushCoveredTlies = true;
				if (!ClickAnyButtonInThisFrame)
				{
					Point brushStart = (Main.MouseWorld - new Vector2(BrushSize)).ToTileCoordinates();
					Point brushEnd = (Main.MouseWorld + new Vector2(BrushSize)).ToTileCoordinates();
					BrushCoveredTiles = GetAABBAreaOfTile(brushStart, brushEnd);
					if (Main.mouseLeft)
					{
						Brushing = true;

						// HashSet<Point> set = new HashSet<Point>(BrushPaintedTiles);
						// foreach (var pos in BrushCoveredTiles)
						// {
						// set.Add(pos);
						// }
						// BrushPaintedTiles = set.ToList();
						BrushPaintedTiles.AddRangeDistinct(BrushCoveredTiles);
					}
					if (!Main.mouseLeft && Brushing)
					{
						if (BrushPaintedTiles.Count > 0)
						{
							SaveStepToUndoable(BrushPaintedTiles);
						}
						foreach (var pos in BrushPaintedTiles)
						{
							var tile = SafeGetTile(pos);
							if (State == 0)
							{
								ChangeTile(tile, CurrentTileType, 0);
							}
							if (State == 1)
							{
								ChangeWall(tile, CurrentWallType, 0);
							}
							WorldGen.TileFrame(pos.X, pos.Y, true, true);
							WorldGen.SquareWallFrame(pos.X, pos.Y);
						}
						BrushPaintedTiles = [];
					}
				}
				break;
		}
	}

	public void UpdateLeftClick()
	{
		var mouseTile = SafeGetTile(Main.MouseWorld.ToTileCoordinates());
		if (MouseOverTool >= 0)
		{
			if (SelectedTool == (int)ToolID.PolygonSelect)
			{
				SelectedTiles.AddRangeDistinct(CurrentSelectedTiles);
				CurrentSelectedTiles = [];
			}
			if (MouseOverTool == SelectedTool)
			{
				SelectedTool = -1;
				ClickAnyButtonInThisFrame = true;
			}
			else
			{
				switch (MouseOverTool)
				{
					case -1:
						SelectedTool = -1;
						break;
					case (int)ToolID.Undo:
						SelectedTool = -1;
						if (UndoMapIOs.Count > 0)
						{
							var mapIOandStream_undo = UndoMapIOs.Pop();
							SaveStepToRedoable(mapIOandStream_undo.Mp);
							ReadMapIO(mapIOandStream_undo);
							File.Delete(mapIOandStream_undo.Pt);
						}
						break;
					case (int)ToolID.Redo:
						SelectedTool = -1;
						if (RedoMapIOs.Count > 0)
						{
							var mapIOandStream_redo = RedoMapIOs.Pop();
							SaveStepToUndoable(mapIOandStream_redo.Mp);
							ReadMapIO(mapIOandStream_redo);
							File.Delete(mapIOandStream_redo.Pt);
						}
						break;
					case (int)ToolID.Unselect:
						SelectedTiles = [];
						CurrentSelectedTiles = [];
						PolygonSelectPoints = [];
						break;
					case (int)ToolID.SwitchTileAndWall:
						State += 1;
						State %= 2;
						break;
					default:
						SelectedTool = MouseOverTool;
						break;
				}
				if (MouseOverTool != -1)
				{
					ClickAnyButtonInThisFrame = true;
				}
				PolygonSelectPoints = [];
			}
		}
		else
		{
			RectangleSelecting = false;
			CircleSelecting = false;
			switch (SelectedTool)
			{
				case -1:
					if (Owner.HeldItem.type == ModContent.ItemType<TileToolBox>())
					{
						Position = Main.MouseWorld;
						Timer = 0;
					}
					break;
				case (int)ToolID.PaintBucket:
					SaveStepToUndoable(SelectedTiles);
					foreach (var pos in SelectedTiles)
					{
						if (State == 0)
						{
							ChangeTile(SafeGetTile(pos), CurrentTileType, (int)TileChangeState.Forceful);
						}
						else if (State == 1)
						{
							ChangeWall(SafeGetTile(pos), CurrentWallType, (int)TileChangeState.Forceful);
						}
						WorldGen.TileFrame(pos.X, pos.Y, true, true);
						WorldGen.SquareWallFrame(pos.X, pos.Y);
					}
					break;
				case (int)ToolID.TilePicker:
					if (State == 0)
					{
						if (mouseTile.HasTile)
						{
							CurrentTileType = mouseTile.TileType;
						}
						else
						{
							CurrentTileType = -1;
						}
					}
					else if (State == 1)
					{
						CurrentWallType = mouseTile.WallType;
					}
					break;
				case (int)ToolID.RectangleSelect:
					break;
				case (int)ToolID.PolygonSelect:
					if (!ClickAnyButtonInThisFrame)
					{
						var point = Main.MouseWorld.ToTileCoordinates();
						if (!PolygonSelectPoints.Contains(point))
						{
							PolygonSelectPoints.Add(point);
						}
						else
						{
							PolygonSelectPoints.Remove(point);
						}
						if (PolygonSelectPoints.Count >= 3)
						{
							CurrentSelectedTiles = GetPolygonAreaOfTilePos(PolygonSelectPoints);
						}
						else
						{
							CurrentSelectedTiles.Clear();
						}
					}
					break;
				case (int)ToolID.MagicWand:
					if (!ClickAnyButtonInThisFrame)
					{
						var point = Main.MouseWorld.ToTileCoordinates();
						if (State == 0)
						{
							if (mouseTile.HasTile)
							{
								SelectedTiles.AddRangeDistinct(BFSContinueTile(point, false, 2048, [mouseTile.TileType]));
							}
							else
							{
								SelectedTiles.AddRangeDistinct(BFSContinueEmpty(point, false, 2048));
							}
						}
						if (State == 1)
						{
							SelectedTiles.AddRangeDistinct(BFSContinueWall(point, 2048, [mouseTile.WallType]));
						}
					}
					break;
				case (int)ToolID.CircleSelect:
					break;
			}
		}
	}

	public void UpdateRightClick()
	{
		switch (SelectedTool)
		{
			case -1:
				Open = false;
				break;
			default:
				if (SelectedTool == (int)ToolID.PolygonSelect)
				{
					SelectedTiles.AddRangeDistinct(CurrentSelectedTiles);
					CurrentSelectedTiles = [];
				}
				SelectedTool = -1;
				break;
		}
	}

	public override void Draw()
	{
		MouseText = "  ";
		if (Main.mapFullscreen)
		{
			return;
		}
		MouseOverTool = -1;
		foreach (var ui in Tools)
		{
			DrawToolUI(ui);
		}
		if (MouseOverTool != -1)
		{
			MouseText += Enum.GetName(typeof(ToolID), MouseOverTool);
		}
		if (SelectedTool == (int)ToolID.TilePicker && MouseOverTool == -1)
		{
			int type = -1;
			var checkTile = SafeGetTile(Main.MouseWorld.ToTileCoordinates());
			if (State == 0)
			{
				if (checkTile.HasTile)
				{
					type = checkTile.TileType;
				}
			}
			else if (State == 1)
			{
				type = checkTile.WallType;
			}
			if (MouseText.Length > 2)
			{
				MouseText += "\n";
			}
			MouseText += "[c/00FFFF:";
			if (State == 0)
			{
				AddTileData(type);
			}
			else if (State == 1)
			{
				AddWallData(type);
			}
			MouseText += "]";
		}
		if (MouseOverTool == (int)ToolID.TileColorBoard)
		{
			MouseText += "\n";
			if (State == 0)
			{
				AddTileData(CurrentTileType);
			}
			else if (State == 1)
			{
				AddWallData(CurrentWallType);
			}
		}
		if (SelectedTool != -1)
		{
			Texture2D tex = ModAsset.TileToolBoxUI.Value;
			Rectangle frame = ToolFrame(SelectedTool);
			Ins.Batch.Draw(tex, Main.MouseWorld + new Vector2(16), frame, Color.White, 0, frame.Size() / 2f, 0.75f, SpriteEffects.None);
			if (SelectedTool == (int)ToolID.PolygonSelect)
			{
				DrawSelectPolygon();
			}
			if (SelectedTool == (int)ToolID.CircleSelect && CircleSelecting)
			{
				Texture2D point = ModAsset.LightPoint2.Value;
				Ins.Batch.Draw(point, CircleSelectCenter.ToWorldCoordinates(), null, new Color(0.4f, 0.4f, 1f, 0f), 0, point.Size() * 0.5f, 1f, SpriteEffects.None);
			}
			if (MouseOverTool == -1)
			{
				if (MouseText.Length > 2)
				{
					MouseText += "\n";
				}
				MouseText += "[c/666666:Right Click to Cancel the Tool.]";
			}
		}
		if (ShouldDrawMouseTlie)
		{
			DrawTile(Main.MouseWorld.ToTileCoordinates(), new Color(1f, 0.75f, 0.15f, 0.4f));
		}
		if (ShouldDrawBrushCoveredTlies)
		{
			var drawColor = new Color(1f, 0.0f, 0.2f, 0.7f);
			if (State == 1)
			{
				drawColor = new Color(0f, 0.6f, 0.1f, 0.7f);
			}
			DrawListOfTiles(BrushPaintedTiles, drawColor);
			DrawListOfTiles(BrushCoveredTiles, new Color(0.7f, 0.4f, 0.4f, 0.4f));
		}
		if (ShouldDrawSelectedTlies)
		{
			DrawListOfTiles(CurrentSelectedTiles, new Color(0.7f, 0.4f, 0.4f, 0.4f));
			DrawListOfTiles(SelectedTiles, new Color(0.7f, 0.4f, 0.4f, 0.4f) * 0.5f);
		}
		Main.instance.MouseText(MouseText, ItemRarityID.White);
		if (State == 0 && CurrentTileType >= 0)
		{
			Main.instance.LoadTiles(CurrentTileType);
			Texture2D tileTex = TextureAssets.Tile[CurrentTileType].Value;
			var frame = new Rectangle(162, 54, 16, 16);
			Ins.Batch.Draw(tileTex, Position, frame, Color.White, 0, frame.Size() * 0.5f, 1f, SpriteEffects.None);
		}
		if (State == 1 && CurrentWallType >= 1)
		{
			Main.instance.LoadWall(CurrentWallType);
			Texture2D wallTex = TextureAssets.Wall[CurrentWallType].Value;
			var frame = new Rectangle(324, 108, 32, 32);
			Ins.Batch.Draw(wallTex, Position, frame, Color.White, 0, frame.Size() * 0.5f, 1f, SpriteEffects.None);
		}
	}

	public void DrawSelectPolygon()
	{
		Texture2D point = ModAsset.LightPoint2.Value;
		for (int i = 0; i < PolygonSelectPoints.Count; i++)
		{
			float drawScale = 0.75f;
			var drawColor = new Color(0.4f, 0.4f, 1f, 0f) * 0.5f;
			if (Main.MouseWorld.ToTileCoordinates() == PolygonSelectPoints[i])
			{
				drawColor = new Color(1f, 0.7f, 0f, 0f);
				drawScale = 1f;
			}
			Ins.Batch.Draw(point, PolygonSelectPoints[i].ToWorldCoordinates(), null, drawColor, 0, point.Size() * 0.5f, drawScale, SpriteEffects.None);
			if (PolygonSelectPoints.Count >= 2)
			{
				Vector2 next;
				if (i < PolygonSelectPoints.Count - 1)
				{
					next = PolygonSelectPoints[i + 1].ToWorldCoordinates();
				}
				else
				{
					next = PolygonSelectPoints[0].ToWorldCoordinates();
				}
				DrawLine(PolygonSelectPoints[i].ToWorldCoordinates(), next);
			}
		}
	}

	public void DrawLine(Vector2 pos0, Vector2 pos1)
	{
		var drawColor = new Color(0, 0.2f, 0.4f, 0);
		Texture2D tex = ModAsset.White.Value;
		List<Vertex2D> bars = new List<Vertex2D>();
		Vector2 dir = (pos0 - pos1).RotatedBy(MathHelper.PiOver2).NormalizeSafe();
		bars.Add(pos0 + dir, drawColor, new Vector3(0, 0, 0));
		bars.Add(pos0 - dir, drawColor, new Vector3(0, 0, 0));
		bars.Add(pos1 + dir, drawColor, new Vector3(0, 0, 0));
		bars.Add(pos1 - dir, drawColor, new Vector3(0, 0, 0));
		Ins.Batch.Draw(tex, bars, PrimitiveType.TriangleStrip);
	}

	public void DrawTile(Point pos, Color color)
	{
		Texture2D tile = ModAsset.TileBlock.Value;
		Ins.Batch.Draw(tile, pos.ToWorldCoordinates(), null, color, 0, tile.Size() / 2f, 1f, SpriteEffects.None);
	}

	public void DrawListOfTiles(List<Point> tiles, Color drawColor)
	{
		Texture2D tex = ModAsset.TileBlock4x4.Value;
		foreach (var pos in tiles)
		{
			if (!VFXManager.InScreen(pos.ToWorldCoordinates(), 32))
			{
				continue;
			}
			Rectangle drawFrame = new Rectangle(16, 16, 16, 16);
			if (!tiles.Contains(pos + new Point(-1, 0)))
			{
				drawFrame.X = 0;
			}
			if (!tiles.Contains(pos + new Point(0, -1)))
			{
				drawFrame.Y = 0;
			}
			if (!tiles.Contains(pos + new Point(1, 0)))
			{
				drawFrame.X = 32;
			}
			if (!tiles.Contains(pos + new Point(0, 1)))
			{
				drawFrame.Y = 32;
			}
			if (!tiles.Contains(pos + new Point(-1, 0)) && !tiles.Contains(pos + new Point(1, 0)))
			{
				drawFrame.X = 48;
			}
			if (!tiles.Contains(pos + new Point(0, -1)) && !tiles.Contains(pos + new Point(0, 1)))
			{
				drawFrame.Y = 48;
			}
			Ins.Batch.Draw(tex, pos.ToWorldCoordinates(), drawFrame, drawColor, 0, drawFrame.Size() / 2f, 1f, SpriteEffects.None);
		}
	}

	public void DrawToolUI(ToolButton tool)
	{
		Texture2D tex = ModAsset.TileToolBoxUI.Value;
		Texture2D panel = ModAsset.Wires_0.Value;
		Rectangle frame = ToolFrame(tool.Type);
		Vector2 pos = Position + tool.Position;
		tool.Scale = 1f;
		if ((Main.MouseWorld - pos).Length() < 20)
		{
			MouseOverTool = tool.Type;
			panel = ModAsset.Wires_1.Value;
			tool.Scale = 1.2f;
		}

		Ins.Batch.Draw(panel, pos, null, Color.White, 0, panel.Size() / 2f, tool.Scale, SpriteEffects.None);
		Ins.Batch.Draw(tex, pos, frame, Color.White, 0, frame.Size() / 2f, tool.Scale, SpriteEffects.None);
	}

	public void AddTileData(int type)
	{
		string datas = "Clear Tile(-1)";
		if (type >= 0)
		{
			datas = TileID.Search.GetName(type) + "(" + type + ")";
		}
		MouseText += datas;
	}

	public void AddWallData(int type)
	{
		string datas = "Clear Wall(-1)";
		if (type >= 0)
		{
			datas = WallID.Search.GetName(type) + "(" + type + ")";
		}
		MouseText += datas;
	}

	public Rectangle ToolFrame(int id)
	{
		Rectangle frame = new Rectangle(0, 0, 32, 32);
		switch (id)
		{
			case (int)ToolID.TilePicker:
				frame = new Rectangle(2, 6, 26, 26);
				break;
			case (int)ToolID.TileColorBoard:
				frame = new Rectangle(40, 22, 28, 20);
				if (State == 0)
				{
					if (CurrentTileType >= 0)
					{
						frame = new Rectangle(40, 0, 28, 20);
					}
				}
				else if (State == 1)
				{
					if (CurrentWallType >= 1)
					{
						frame = new Rectangle(40, 0, 28, 20);
					}
				}
				break;
			case (int)ToolID.PaintBucket:
				frame = new Rectangle(76, 10, 22, 20);
				break;
			case (int)ToolID.History:
				frame = new Rectangle(106, 4, 30, 30);
				break;
			case (int)ToolID.Undo:
				frame = new Rectangle(142, 12, 18, 18);
				if (UndoMapIOs.Count <= 0)
				{
					frame.X += 48;
				}
				break;
			case (int)ToolID.Redo:
				frame = new Rectangle(166, 12, 18, 18);
				if (RedoMapIOs.Count <= 0)
				{
					frame.X += 48;
				}
				break;
			case (int)ToolID.RectangleSelect:
				frame = new Rectangle(8, 42, 18, 18);
				break;
			case (int)ToolID.PolygonSelect:
				frame = new Rectangle(36, 42, 18, 18);
				break;
			case (int)ToolID.CircleSelect:
				frame = new Rectangle(62, 42, 18, 18);
				break;
			case (int)ToolID.Unselect:
				frame = new Rectangle(88, 42, 18, 18);
				break;
			case (int)ToolID.PaintBrush:
				frame = new Rectangle(120, 44, 22, 20);
				break;
			case (int)ToolID.None:
				frame = new Rectangle(12, 68, 16, 22);
				break;
			case (int)ToolID.MagicWand:
				frame = new Rectangle(36, 68, 20, 20);
				break;
			case (int)ToolID.Smoothe:
				frame = new Rectangle(64, 72, 18, 14);
				break;
			case (int)ToolID.SwitchTileAndWall:
				frame = new Rectangle(88, 72, 20, 20);
				break;
		}
		if (State == 1)
		{
			frame.Y += 100;
		}
		return frame;
	}

	public Rectangle GetBoundOfTiles(List<Point> tiles)
	{
		int x = int.MaxValue;
		int y = int.MaxValue;
		int x_max = 0;
		int y_max = 0;
		foreach (var pos in tiles)
		{
			if (pos.X < x)
			{
				x = pos.X;
			}
			if (pos.Y < y)
			{
				y = pos.Y;
			}
			if (pos.X > x_max)
			{
				x_max = pos.X;
			}
			if (pos.Y > y_max)
			{
				y_max = pos.Y;
			}
		}
		int width = x_max - x + 1;
		int height = y_max - y + 1;
		return new Rectangle(x, y, width, height);
	}

	public void ReadMapIO((MapIO Mp, string Pt) item)
	{
		var mapIO = item.Mp;
		var path = item.Pt;
		mapIO.Read(path);
		var it = mapIO.GetEnumerator();
		while (it.MoveNext())
		{
			WorldGen.SquareTileFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
			WorldGen.SquareWallFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
		}
	}

	public void SaveStepToUndoable(List<Point> tiles)
	{
		if (tiles.Count <= 0)
		{
			return;
		}
		Rectangle area = GetBoundOfTiles(tiles);
		var mapIO = new MapIO(area);
		string path = Path.Combine(Main.SavePath, "Mods", "ModDatas", "Everglow");
		path = Path.Combine(path, "TileToolBox");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		string writePath = path + "\\Undo" + UndoMapIOs.Count.ToString() + ".mapio";
		mapIO.Write(writePath);
		UndoMapIOs.Push((mapIO, writePath));
	}

	public void SaveStepToRedoable(MapIO mapIO)
	{
		string path = Path.Combine(Main.SavePath, "Mods", "ModDatas", "Everglow");
		path = Path.Combine(path, "TileToolBox");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		string writePath = path + "\\Redo" + RedoMapIOs.Count.ToString() + ".mapio";
		mapIO.Write(writePath);
		RedoMapIOs.Push((mapIO, writePath));
	}

	public void SaveStepToUndoable(MapIO mapIO)
	{
		string path = Path.Combine(Main.SavePath, "Mods", "ModDatas", "Everglow");
		path = Path.Combine(path, "TileToolBox");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		string writePath = path + "\\Undo" + UndoMapIOs.Count.ToString() + ".mapio";
		mapIO.Write(writePath);
		UndoMapIOs.Push((mapIO, writePath));
	}

	public static void ClearHistory()
	{
		string path = Path.Combine(Main.SavePath, "Mods", "ModDatas", "Everglow");
		path = Path.Combine(path, "TileToolBox");
		if (!Directory.Exists(path))
		{
			return;
		}
		DeleteFilesByPattern(path, "*.mapio");
		UndoMapIOs = [];
		RedoMapIOs = [];
	}

	public static void DeleteFilesByPattern(string directoryPath, string searchPattern = "*.*")
	{
		if (!Directory.Exists(directoryPath))
		{
			Console.WriteLine($" Path does not exist.: {directoryPath}");
			return;
		}

		int deletedCount = 0;
		try
		{
			string[] files = Directory.GetFiles(directoryPath, searchPattern);

			foreach (string file in files)
			{
				File.Delete(file);
				deletedCount++;
			}

			Console.WriteLine($" {deletedCount} Have been deleted.");
			return;
		}
		catch (Exception ex)
		{
			Console.WriteLine($" Fail: {ex.Message}");
			return;
		}
	}
}
