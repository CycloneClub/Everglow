using Everglow.Commons.Enums;
using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Everglow.Commons.TileHelper;

/// <summary>
/// 缆绳物块,可用于制作彩灯,彩带等
/// </summary>
public abstract class CableTile : ModTile, ITileFluentlyDrawn
{
	public override string Texture => "Everglow/Commons/TileHelper/CableKnot";

	/// <summary>
	/// 最大绳长,默认900
	/// </summary>
	public int MaxCableLength = 900;

	/// <summary>
	/// 单个灯的重量,默认2
	/// </summary>
	public float SingleLampMass = 2;

	/// <summary>
	/// 单节绳重,默认0.05
	/// </summary>
	public float RopeUnitMass = 0.8f;

	/// <summary>
	/// 灯距,默认8
	/// </summary>
	public int LampDistance = 8;

	/// <summary>
	/// 最多电线形态,默认为2,最多16,原版的绝大多数物品都是2,但是这里提供了例外的可能
	/// </summary>
	public int MaxWireStyle = 2;

	/// <summary>
	/// 劲度系数
	/// </summary>
	public float Elasticity = 60;

	/// <summary>
	/// How far should start drawing the rope in an area out of screen .
	/// </summary>
	public float DrawOffScreenRange = 1200f;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		TileID.Sets.BlocksWaterDrawingBehindSelf[Type] = true;

		// MyTileEntity refers to the tile entity mentioned in the previous section
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<CableEneity>().Hook_AfterPlacement, -1, 0, true);

		// This is required so the hook is actually called.
		TileObjectData.newTile.UsesCustomCanPlace = true;

		AddMapEntry(new Color(42, 42, 84));
	}

	/// <summary>
	/// 受绳端位置 绳子
	/// </summary>
	public Dictionary<Point, Rope> RopesOfAllThisTileInTheWorld = new Dictionary<Point, Rope>();

	/// <summary>
	/// 受绳端位置 发绳端位置
	/// </summary>
	public Dictionary<Point, Point> RopeHeadAndTail = new Dictionary<Point, Point>();

	public override void HitWire(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		tile.TileFrameX += 18;
		if (tile.TileFrameX > 54)
		{
			tile.TileFrameX = 0;
			tile.TileFrameY += 18;
		}
		int style = tile.TileFrameX / 18 + (tile.TileFrameY / 18) * 4;
		MaxWireStyle = Math.Min(MaxWireStyle, 16);
		if (style >= MaxWireStyle)
		{
			tile.TileFrameX = 0;
			tile.TileFrameY = 0;
		}
		base.HitWire(i, j);
	}

	/// <summary>
	/// 玩家拉绳点位
	/// </summary>
	public Dictionary<Player, Point> HasHoldRope = new Dictionary<Player, Point>();

	/// <summary>
	/// 右键效果。不拿绳拆绳，拿绳拉绳，拉了绳挂绳
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <returns></returns>
	public override bool RightClick(int i, int j)
	{
		// 挂绳
		if (HasHoldRope.ContainsKey(Main.LocalPlayer))
		{
			Point point = HasHoldRope[Main.LocalPlayer];
			if (i != point.X || j != point.Y)
			{
				if (new Vector2(point.X - i, point.Y - j).Length() * 16 < MaxCableLength)
				{
					if (Main.LocalPlayer.HeldItem.createTile == Main.tile[i, j].TileType)
					{
						AddRope(i, j, point.X, point.Y);
						SoundEngine.PlaySound(SoundID.Item50.WithVolumeScale(0.8f), new Vector2(i, j) * 16);
					}
				}
			}
			HasHoldRope.Remove(Main.LocalPlayer);
			return base.RightClick(i, j);
		}

		// 拉绳
		if (Main.LocalPlayer.HeldItem.ModItem is CableTileItem)
		{
			if (Main.LocalPlayer.HeldItem.createTile == Main.tile[i, j].TileType)
			{
				HasHoldRope.Add(Main.LocalPlayer, new Point(i, j));
				CableTilePlaceHelpingSystem vfx = new CableTilePlaceHelpingSystem { FixPoint = new Point(i, j), Active = true, Visible = true, Style = 1 };
				Ins.VFXManager.Add(vfx);
				SoundEngine.PlaySound(SoundID.Item17, new Vector2(i, j) * 16);
			}
			return base.RightClick(i, j);
		}

		// 拆绳
		RemoveAllRope(i, j);
		SoundEngine.PlaySound(SoundID.Item110, new Vector2(i, j) * 16);
		return base.RightClick(i, j);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		RemoveAllRope(i, j);

		// ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
		// The tile coordinate parameters already refer to the top-left corner of the multitile
		base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
	}

	/// <summary>
	/// 移除挂在这个点上所有绳
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	public virtual void RemoveAllRope(int i, int j)
	{
		// 移除受绳
		RemoveRopeEndAt(i, j);

		// 移除发绳
		foreach (Point point in RopeHeadAndTail.Keys)
		{
			if (RopeHeadAndTail[point] == new Point(i, j))
			{
				RemoveRopeEndAt(point.X, point.Y);
			}
		}
	}

	/// <summary>
	/// Remove all ropes ending at this point.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	public virtual void RemoveRopeEndAt(int i, int j)
	{
		if (RopesOfAllThisTileInTheWorld.ContainsKey(new Point(i, j)))
		{
			Rope rope;
			RopesOfAllThisTileInTheWorld.TryGetValue(new Point(i, j), out rope);
			RemoveRopeEffect(rope);
			RopesOfAllThisTileInTheWorld.Remove(new Point(i, j));
			ModContent.GetInstance<CableEneity>().Kill(i, j);
		}
		if (RopeHeadAndTail.ContainsKey(new Point(i, j)))
		{
			RopeHeadAndTail.Remove(new Point(i, j));
		}
	}

	/// <summary>
	/// Connect rope by TileEntity information.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="closer"></param>
	public override void NearbyEffects(int i, int j, bool closer)
	{
		// connect rope start
		if (!RopesOfAllThisTileInTheWorld.ContainsKey(new Point(i, j)))
		{
			CableEneity cableEneity;
			TryGetCableEntityAs(i, j, out cableEneity);
			if (cableEneity != null)
			{
				AddRope(i, j, i + cableEneity.ToTail.X, j + cableEneity.ToTail.Y);
			}

			// Only use for Cable Car
			// FindAndConnectRopeNearBy(i, j, 3);
		}
		base.NearbyEffects(i, j, closer);
	}

	public void FindAndConnectRopeNearBy(int i, int j, int maxStep = 100)
	{
		if (maxStep <= 0)
		{
			return;
		}
		Tile tile = Main.tile[i, j];
		for (int x = -125; x < 126; x++)
		{
			for (int y = -125; y < 126; y++)
			{
				CableEneity cableEneity;
				int pointX = Math.Clamp(i + x, 20, Main.maxTilesX - 20);
				int pointY = Math.Clamp(j + y, 20, Main.maxTilesY - 20);
				Tile newTile = Main.tile[pointX, pointY];
				if (newTile.TileType == tile.TileType)
				{
					TryGetCableEntityAs(pointX, pointY, out cableEneity);
					if (cableEneity != null)
					{
						AddRope(pointX, pointY, pointX + cableEneity.ToTail.X, pointY + cableEneity.ToTail.Y);
					}
					FindAndConnectRopeNearBy(pointX, pointY, maxStep - 1);
				}
			}
		}
	}

	/// <summary>
	/// Visual effect when cutting the rope.
	/// </summary>
	/// <param name="rope"></param>
	public virtual void RemoveRopeEffect(Rope rope)
	{
		var masses = rope.Masses;
		for (int i = 0; i < masses.Length; i++)
		{
			Mass thisMass = masses[i];
			Dust d0 = Dust.NewDustDirect(thisMass.Position - new Vector2(4), 0, 0, DustID.Asphalt);
			d0.noGravity = true;
			d0.velocity *= 0f;
			d0.scale = 0.8f;
			if (thisMass.Value == 2)
			{
				for (int t = 0; t < 3; t++)
				{
					Dust d = Dust.NewDustDirect(thisMass.Position + new Vector2(0, 5) - new Vector2(4), 0, 0, DustID.Electric);
					d.velocity = new Vector2(0, Main.rand.NextFloat(0f, 3.5f)).RotatedByRandom(MathHelper.TwoPi);
					d.scale = Main.rand.NextFloat(0.1f, 0.3f);
				}
			}
			if (i == 0 || i == masses.Length - 1)
			{
				for (int t = 0; t < 5; t++)
				{
					Dust d = Dust.NewDustDirect(thisMass.Position + new Vector2(0, 5) - new Vector2(4), 0, 0, DustID.Electric);
					d.velocity = new Vector2(0, Main.rand.NextFloat(0.5f, 6.5f)).RotatedByRandom(MathHelper.TwoPi);
					d.scale = Main.rand.NextFloat(0.3f, 0.7f);
				}
			}
		}
	}

	/// <summary>
	/// 挂上绳子
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="rope"></param>
	public virtual void AddRope(int i, int j, int i2, int j2)
	{
		Rope rope = ConnectRope(i, j, i2, j2);
		if (rope == null)
		{
			return;
		}
		if (RopesOfAllThisTileInTheWorld.ContainsKey(new Point(i, j)))
		{
			return;
		}
		var masses = rope.Masses;
		RopesOfAllThisTileInTheWorld.Add(new Point(i, j), rope);
		RopeHeadAndTail.Add(new Point(i, j), new Point(i2, j2));
		TryGetCableEntityAs(i, j, out CableEneity cableEneity);
		if (cableEneity == null)
		{
			TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<CableEneity>());
			TryGetCableEntityAs(i, j, out cableEneity);
		}
		if (cableEneity != null)
		{
			Vector2 tail = masses.Last().Position / 16f;
			Point toTail = tail.ToPoint() - new Point(i, j);
			cableEneity.ToTail = toTail;
		}
	}

	/// <summary>
	/// 两点式挂绳
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="i2"></param>
	/// <param name="j2"></param>
	/// <returns></returns>
	public virtual Rope ConnectRope(int i, int j, int i2, int j2)
	{
		Tile tile = Main.tile[i2, j2];

		// 挂在自己身上报错
		if (i == i2 && j == j2)
		{
			CombatText.NewText(new Rectangle(i * 16, j * 16, 8, 8), Color.Red, "You can't connect rope in a same tile!");
			return null;
		}

		// 只有同一种块之间才能连绳
		if (tile.TileType == Type)
		{
			int counts = (int)new Vector2(i2 - i, j2 - j).Length() * 2 + 2;
			Rope rope = Rope.Create_Fixed_StartAndEnd_WithKnots(new Vector2(i, j) * 16 + new Vector2(8), new Vector2(i2, j2) * 16 + new Vector2(8), counts, Elasticity, RopeUnitMass, LampDistance, SingleLampMass);
			return rope;
		}
		return null;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return base.PreDraw(i, j, spriteBatch);
	}

	public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
	{
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		foreach (Point point in RopeHeadAndTail.Keys)
		{
			if (RopeHeadAndTail[point] == new Point(i, j))
			{
				TileFluentDrawManager.AddFluentPoint(this, point.X, point.Y);
			}
		}
		base.DrawEffects(i, j, spriteBatch, ref drawData);
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		Rope rope;
		RopesOfAllThisTileInTheWorld.TryGetValue(pos, out rope);
		if (rope != null)
		{
			DrawCable(rope, pos, spriteBatch, tileDrawing);
		}

		// foreach (Point point in RopeHeadAndTail.Keys)
		// {
		// if (RopeHeadAndTail[point] == pos)
		// {
		// RopesOfAllThisTileInTheWorld.TryGetValue(point, out rope);
		// DrawCable(rope, point, spriteBatch, tileDrawing);
		// }
		// }
	}

	public string BulbTexturePath;

	/// <summary>
	/// Draw cable.While rope with hanging unit(such as light bulb), this method must be overrided.
	/// </summary>
	/// <param name="rope"></param>
	/// <param name="pos"></param>
	/// <param name="spriteBatch"></param>
	/// <param name="tileDrawing"></param>
	/// <param name="color"></param>
	public virtual void DrawCable(Rope rope, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Color color = default(Color))
	{
		// 回声涂料
		if (!TileDrawing.IsVisible(Main.tile[pos]))
		{
			return;
		}

		var tile = Main.tile[pos];
		ushort type = tile.TileType;
		int paint = Main.tile[pos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(BulbTexturePath, type, 1, paint, tileDrawing);
		var tileSpriteEffect = SpriteEffects.None;
		var masses = rope.Masses;
		for (int i = 0; i < masses.Length - 1; i++)
		{
			Mass thisMass = masses[i];
			Mass nextMass = masses[i + 1];

			int totalPushTime = 80;
			float pushForcePerFrame = 1.26f;
			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), tileDrawing._sunflowerWindCounter);
			}

			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			if (!Main.gamePaused)
			{
				rope.ApplyForceSpecial(i, new Vector2(windCycle / 16f, 0.4f * thisMass.Value));
			}

			// 支持发光涂料
			Color tileLight;
			if (color != default)
			{
				tileLight = color;
			}
			else
			{
				tileLight = Lighting.GetColor((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f)) * 10;
			}
			Vector2 toNextMass = nextMass.Position - thisMass.Position;
			Vector2 drawPos = thisMass.Position - Main.screenPosition;
			spriteBatch.Draw(tex, drawPos, new Rectangle(0, 2, 2, 2), tileLight, toNextMass.ToRotation(), new Vector2(1f), new Vector2(toNextMass.Length() / 2f, 1), tileSpriteEffect, 0);
		}
	}

	/// <summary>
	/// Try to get the cable entity bound at (<paramref name="i"/>, <paramref name="j"/>).
	/// </summary>
	/// <typeparam name="T">The type to get the entity as</typeparam>
	/// <param name="i">The tile X-coordinate</param>
	/// <param name="j">The tile Y-coordinate</param>
	/// <param name="entity">The found <typeparamref name="T"/> instance, if there was one.</param>
	/// <returns><see langword="true"/> if there was a <typeparamref name="T"/> instance, or <see langword="false"/> if there was no entity present OR the entity was not a <typeparamref name="T"/> instance.</returns>
	public static bool TryGetCableEntityAs<T>(int i, int j, out T entity)
		where T : TileEntity
	{
		Point16 origin = new Point16(i, j);

		// TileEntity.ByPosition is a Dictionary<Point16, TileEntity> which contains all placed TileEntity instances in the world
		// TryGetValue is used to both check if the dictionary has the key, origin, and get the value from that key if it's there
		if (TileEntity.ByPosition.TryGetValue(origin, out TileEntity existing) && existing is T existingAsT)
		{
			entity = existingAsT;
			return true;
		}

		entity = null;
		return false;
	}

	/// <summary>
	/// 鼠标划过点位
	/// </summary>
	public Dictionary<Player, Point> MouseOverWinchPlayers = new Dictionary<Player, Point>();

	public override void MouseOver(int i, int j)
	{
		if (!MouseOverWinchPlayers.ContainsKey(Main.LocalPlayer))
		{
			MouseOverWinchPlayers.Add(Main.LocalPlayer, new Point(i, j));
			if (Main.LocalPlayer.HeldItem.ModItem is CableTileItem)
			{
				CableTilePlaceHelpingSystem vfx = new CableTilePlaceHelpingSystem { FixPoint = new Point(i, j), Active = true, Visible = true, Style = 0 };
				Ins.VFXManager.Add(vfx);
			}
		}
		else if (MouseOverWinchPlayers[Main.LocalPlayer] != new Point(i, j))
		{
			MouseOverWinchPlayers.Remove(Main.LocalPlayer);
		}
	}

	public void CreateACableTile(int i0, int j0, int i1, int j1, int style = 0)
	{
		var tile0 = Main.tile[Math.Clamp(i0, 20, Main.maxTilesX - 20), Math.Clamp(j0, 20, Main.maxTilesY - 20)];
		var tile1 = Main.tile[Math.Clamp(i1, 20, Main.maxTilesX - 20), Math.Clamp(j1, 20, Main.maxTilesY - 20)];
		tile1.TileType = tile0.TileType = Type;
		tile1.HasTile = tile0.HasTile = true;
		int styleX = style % 4;
		int styleY = (style - styleX) / 4;
		tile1.TileFrameX = tile0.TileFrameX = (short)(18 * styleX);
		tile1.TileFrameY = tile0.TileFrameY = (short)(18 * styleY);
		AddRope(i0, j0, i1, j1);
	}
}

public class CableEneity : ModTileEntity
{
	public Point ToTail;

	public override bool IsTileValidForEntity(int x, int y)
	{
		Tile tile = Main.tile[x, y];
		if (TileLoader.GetTile(tile.TileType) is CableTile)
		{
			return tile.HasTile;
		}
		return false;
	}

	public override void OnNetPlace()
	{
		if (Main.netMode == NetmodeID.Server)
		{
			NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
		}
	}

	public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			// Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
			int width = 1;
			int height = 1;
			NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

			// Sync the placement of the tile entity with other clients
			// The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
			NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
			return -1;
		}

		// ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
		int placedEntity = Place(i, j);
		return placedEntity;
	}

	public override void SaveData(TagCompound tag)
	{
		tag.Add("CableToTailX", ToTail.X);
		tag.Add("CableToTailY", ToTail.Y);
		base.SaveData(tag);
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		tag.TryGet<int>("CableToTailX", out ToTail.X);
		tag.TryGet<int>("CableToTailY", out ToTail.Y);
	}
}

[Pipeline(typeof(WCSPipeline))]
public class CableTilePlaceHelpingSystem : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PreDrawFilter;

	public Texture2D Texture;
	public Point FixPoint;
	public int Style;

	public override void OnSpawn()
	{
		Texture = ModAsset.TileBlock.Value;
	}

	public override void Update()
	{
		int i = FixPoint.X;
		int j = FixPoint.Y;
		if (i < 20 || i > Main.maxTilesX - 20)
		{
			if (j < 20 || j > Main.maxTilesY - 20)
			{
				Active = false;
				return;
			}
		}
		CableTile cableTile = TileLoader.GetTile(Main.tile[i, j].type) as CableTile;
		if (cableTile == null)
		{
			Active = false;
			return;
		}
		if (Style == 1)
		{
			if (!cableTile.HasHoldRope.ContainsKey(Main.LocalPlayer))
			{
				Active = false;
				return;
			}
		}
		if (Style == 0)
		{
			if (cableTile.HasHoldRope.ContainsKey(Main.LocalPlayer))
			{
				Active = false;
				if (cableTile.MouseOverWinchPlayers.ContainsKey(Main.LocalPlayer))
				{
					cableTile.MouseOverWinchPlayers.Remove(Main.LocalPlayer);
				}
				return;
			}
			int x = (int)(Main.MouseWorld.X / 16f);
			int y = (int)(Main.MouseWorld.Y / 16f);
			if (x != FixPoint.X || y != FixPoint.Y)
			{
				Active = false;
				if (cableTile.MouseOverWinchPlayers.ContainsKey(Main.LocalPlayer))
				{
					cableTile.MouseOverWinchPlayers.Remove(Main.LocalPlayer);
				}
				return;
			}
		}
		base.Update();
	}

	public override void Draw()
	{
		int i = FixPoint.X;
		int j = FixPoint.Y;
		if (i < 20 || i > Main.maxTilesX - 20)
		{
			if (j < 20 || j > Main.maxTilesY - 20)
			{
				Active = false;
				return;
			}
		}
		Player player = Main.LocalPlayer;
		Tile tile = Main.tile[i, j];
		CableTile cableTile = TileLoader.GetTile(tile.type) as CableTile;
		if (cableTile == null)
		{
			Active = false;
			return;
		}
		Color drawColor = Color.Lerp(new Color(0.75f, 0.75f, 1f, 0.5f), new Color(0.85f, 0.85f, 0.75f, 0.5f), MathF.Sin((float)Main.timeForVisualEffects * 0.08f) * 0.5f + 0.5f);

		// 不同种类物块标红
		if (player.HeldItem.createTile != tile.type)
		{
			drawColor = new Color(1f, 0, 0, 0.5f);
			Main.instance.MouseText("Different Type Error", ItemRarityID.Red);
		}
		Ins.Batch.BindTexture<Vertex2D>(Texture);
		Vector2 fixPos = FixPoint.ToVector2() * 16f;

		// 类型1 ： 牵线后
		if (Style == 1)
		{
			if (cableTile.HasHoldRope.ContainsKey(player))
			{
				if (player.HeldItem.ModItem is not CableTileItem)
				{
					cableTile.HasHoldRope.Remove(player);
					Active = false;
					return;
				}
				if (cableTile.HasHoldRope[player] == new Point(i, j))
				{
					// 校正鼠标坐标
					Vector2 toTarget = new Vector2(i, j) * 16 + new Vector2(8) - Main.MouseWorld;

					// 超过cableTile.MaxCableLength + 600距离取消选中
					if (toTarget.Length() > cableTile.MaxCableLength + 600)
					{
						cableTile.HasHoldRope.Remove(player);
						Active = false;
						return;
					}
					if (Main.mouseRight && Main.mouseRightRelease && toTarget.Length() > 8 * MathF.Sqrt(2))
					{
						cableTile.HasHoldRope.Remove(player);
						Active = false;
						return;
					}

					int x = (int)(Main.MouseWorld.X / 16f);
					int y = (int)(Main.MouseWorld.Y / 16f);

					// 超过cableTile.MaxCableLength距离标红
					if (new Vector2(i - x, j - y).Length() * 16f > cableTile.MaxCableLength)
					{
						toTarget = Vector2.Normalize(toTarget) * (cableTile.MaxCableLength + 0.0001f);
						drawColor = new Color(1f, 0, 0, 0.3f);

						Vector2 destination = fixPos + new Vector2(8) - toTarget;
						DrawLine(destination + new Vector2(10, 10), destination - new Vector2(10, 10), 3, drawColor);
						DrawLine(destination + new Vector2(-10, 10), destination - new Vector2(-10, 10), 3, drawColor);
						Main.instance.MouseText("Over Length", ItemRarityID.Red);
					}

					bool canDrawDestination = false;

					// 绘制选中块
					if (player.IsInTileInteractionRange(x, y, TileReachCheckSettings.Simple))
					{
						if (x > 20 && x < Main.maxTilesX - 20)
						{
							if (y > 20 && y < Main.maxTilesY - 20)
							{
								int type = Main.tile[x, y].TileType;
								ModTile modTile0 = TileLoader.GetTile(type);
								if (modTile0 is CableTile)
								{
									if (new Vector2(i - x, j - y).Length() * 16f <= cableTile.MaxCableLength)
									{
										drawColor = new Color(1f, 0.9f, 0, 0.4f);

										// 如果是不同种类物块,标红
										if (player.HeldItem.createTile != Main.tile[x, y].type)
										{
											drawColor = new Color(1f, 0, 0, 0.5f);
										}

										// 试图连接到自己标红
										if (i == x && j == y)
										{
											drawColor = new Color(1f, 0, 0, 0.5f);
											Main.instance.MouseText("Can Not Connect Selfly", ItemRarityID.Red);
										}

										// 已经被占据块标红
										else if (cableTile.RopesOfAllThisTileInTheWorld.ContainsKey(new Point(x, y)))
										{
											drawColor = new Color(1f, 0, 0, 0.5f);
											Vector2 anotherPoint = cableTile.RopesOfAllThisTileInTheWorld[new Point(x, y)].Masses.Last().Position;
											Vector2 start0 = anotherPoint;
											Vector2 end0 = fixPos + new Vector2(8) - toTarget;

											Vector2 direction0 = Utils.SafeNormalize(start0 - end0, Vector2.zeroVector) * 16;
											DrawLine(end0, end0 + direction0.RotatedBy(0.4), 3, drawColor);
											DrawLine(end0, end0 + direction0.RotatedBy(-0.4), 3, drawColor);

											DrawLine(anotherPoint, fixPos + new Vector2(8) - toTarget, 3, drawColor);
											DrawBlockBound((int)((anotherPoint.X - 8) / 16f), (int)((anotherPoint.Y - 8) / 16f), drawColor);
											Main.instance.MouseText("Occupied", ItemRarityID.Red);
										}

										canDrawDestination = true;
									}
								}
							}
						}
					}

					// 正常绘制
					Vector2 start = fixPos + new Vector2(8);
					Vector2 end = start - toTarget;

					Vector2 direction = Utils.SafeNormalize(start - end, Vector2.zeroVector) * 16;
					if (new Vector2(i - x, j - y).Length() * 16f <= cableTile.MaxCableLength)
					{
						DrawLine(end, end + direction.RotatedBy(0.4), 3, drawColor);
						DrawLine(end, end + direction.RotatedBy(-0.4), 3, drawColor);
					}

					DrawLine(start, end, 3, drawColor);
					if (canDrawDestination)
					{
						// 绘制
						DrawBlockBound(x, y, drawColor);
					}
				}
			}
		}
		DrawBlockBound(FixPoint.X, FixPoint.Y, drawColor);
	}

	public void DrawBlockBound(int i, int j, Color color)
	{
		Vector2 pos = new Vector2(i, j) * 16;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(pos, color, new Vector3(0, 0, 0)),
			new Vertex2D(pos + new Vector2(16, 0), color, new Vector3(1, 0, 0)),
			new Vertex2D(pos + new Vector2(0, 16), color, new Vector3(0, 1, 0)),

			new Vertex2D(pos + new Vector2(0, 16), color, new Vector3(0, 1, 0)),
			new Vertex2D(pos + new Vector2(16, 0), color, new Vector3(1, 0, 0)),
			new Vertex2D(pos + new Vector2(16), color, new Vector3(1, 1, 0)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleList);
	}

	public void DrawLine(Vector2 pos1, Vector2 pos2, float width, Color color)
	{
		Vector2 normal = Utils.SafeNormalize(pos1 - pos2, Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * width / 2f;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(pos1 + normal, color, new Vector3(0, 0, 0)),
			new Vertex2D(pos2 + normal, color, new Vector3(0.1f, 0, 0)),
			new Vertex2D(pos1 - normal, color, new Vector3(0, 1, 0)),

			new Vertex2D(pos1 - normal, color, new Vector3(0, 1, 0)),
			new Vertex2D(pos2 + normal, color, new Vector3(0.1f, 0, 0)),
			new Vertex2D(pos2 - normal, color, new Vector3(0.1f, 1, 0)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleList);
	}
}

public class CableTileUpdate : ModSystem
{
	/// <summary>
	/// 物块质点系统
	/// </summary>
	public static MassSpringContainer CableTileMassSpringSystem = new MassSpringContainer();
	public static EulerSolver CableTileEulerSolver = new EulerSolver(8);
	public static PBDSolver CableTilePBDSolver = new PBDSolver(8);

	public override void PostUpdateEverything()
	{
		CableTileMassSpringSystem = new MassSpringContainer();
		foreach (var cableTile in TileLoader.tiles.OfType<CableTile>())
		{
			foreach (var rope in cableTile.RopesOfAllThisTileInTheWorld.Values)
			{
				CableTileMassSpringSystem.AddMassSpringMesh(rope);
			}
		}
		CableTileEulerSolver.Step(CableTileMassSpringSystem, 1);
	}

	public override void OnWorldLoad()
	{
		foreach (var cableTile in TileLoader.tiles.OfType<CableTile>())
		{
			cableTile.RopesOfAllThisTileInTheWorld.Clear();
			cableTile.RopeHeadAndTail.Clear();
		}
		base.OnWorldLoad();
	}

	public override void OnWorldUnload()
	{
		foreach (var cableTile in TileLoader.tiles.OfType<CableTile>())
		{
			cableTile.RopesOfAllThisTileInTheWorld.Clear();
			cableTile.RopeHeadAndTail.Clear();
		}
		base.OnWorldUnload();
	}
}
