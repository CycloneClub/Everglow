using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Commons.TileHelper;

/// <summary>
/// HangingTile, can be used to make special chandelier with adjustable cable length.
/// TileFrameY is  cable length.
/// </summary>
public abstract class HangingTile : ModTile, ITileFluentlyDrawn
{
	/// <summary>
	/// Max cable length : default 60
	/// It means the max counts of rope units, not the max length in pixels.
	/// </summary>
	public int MaxCableLength = 60;

	/// <summary>
	/// The mass of the hanging item(usually the light fixture at the bottom) : default 8
	/// </summary>
	public float HangingItemMass = 8;

	/// <summary>
	/// Unit mass : default 0.5f
	/// </summary>
	public float RopeUnitMass = 0.5f;

	/// <summary>
	/// Max style counts when hit wire.
	/// </summary>
	public int MaxWireStyle = 2;

	/// <summary>
	/// The distance between joints when there is no external force.
	/// </summary>
	public float UnitLength = 6f;

	/// <summary>
	/// Elasticity of cable : default 150
	/// </summary>
	public float Elasticity = 150;

	/// <summary>
	/// The maximum time (in frames) for the adjusting visual effect to last. default 60f.
	/// </summary>
	public float AdjustingVisualTimeMax = 60f;

	/// <summary>
	/// Allow right-click the top to adjust length : default true
	/// </summary>
	public bool LengthAdjustable = true;

	/// <summary>
	/// If true, player can press "up" when passing the rope to grasp.
	/// Then, player will absorb to the bottom of the rope and can swing on it.
	/// </summary>
	public bool CanGrasp = false;

	/// <summary>
	///  Be valid only when CanGrasp is true. default 48f. The range of detecting the rope tip when player press "up" to grasp.
	/// </summary>
	public float GraspDetectRange = 48f;

	/// <summary>
	/// How far should start drawing the rope in an area out of screen.
	/// </summary>
	public float DrawOffScreenRange = 1200f;

	public string BulbTexturePath;

	/// <summary>
	/// <summary>
	/// Winch position and rope.<br></br>
	/// 1 point contains 1 rope at most.
	/// </summary>
	public Dictionary<Point, Rope> RopesOfAllThisTileInTheWorld = new Dictionary<Point, Rope>();

	/// <summary>
	/// Current Player who is handling with the HangingTile winched at a certain Point.<br></br>
	/// 1 winch can be modified by only 1 player simultanrously.
	/// </summary>
	public Dictionary<Point, Player> WinchAdjustingPlayers = new Dictionary<Point, Player>();

	/// <summary>
	/// A mousePos-player Dictionary to prevent generating multiple knob VFX.<br></br>
	/// 1 winch can display more than 1 knob VFXs in multi-player mode.
	/// </summary>
	public Dictionary<Player, Point> MouseOverWinchPlayers = new Dictionary<Player, Point>();

	/// <summary>
	/// Current Player who grasping the rope of HangingTile winched at a certain Point.<br></br>
	/// Be valid only when CanGrasp is true.<br></br>
	/// 1 rope can be grasped by only 1 player.
	/// </summary>
	public static Dictionary<Point, Player> RopeGraspingPlayer = new Dictionary<Point, Player>();

	public struct DrawStack
	{
		public Texture2D Texture;

		public IEnumerable<Vertex2D> Vertices;

		public PrimitiveType PType;

		public DrawStack(Texture2D texture, IEnumerable<Vertex2D> vertices, PrimitiveType pType)
		{
			Texture = texture;
			Vertices = vertices;
			PType = pType;
		}
	}

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		TileID.Sets.BlocksWaterDrawingBehindSelf[Type] = true;

		// MyTileEntity refers to the tile entity mentioned in the previous section
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<CableEneity>().Hook_AfterPlacement, -1, 0, true);

		// This is required so the hook is actually called.
		TileObjectData.newTile.UsesCustomCanPlace = true;

		AddMapEntry(new Color(59, 67, 67));
		InitHanging();
	}

	/// <summary>
	/// Override to customize the values.<br></br>
	/// defaults:<br></br>
	/// MaxCableLength = 60;<br></br>
	/// HangingItemMass = 8;<br></br>
	/// RopeUnitMass = 0.5f;<br></br>
	/// MaxWireStyle = 2;<br></br>
	/// Elasticity = 150;<br></br>
	/// LengthAdjustable = true;<br></br>
	/// CanGrasp = false;
	/// </summary>
	public virtual void InitHanging()
	{
	}

	/// <summary>
	/// 在刚打开按钮时的行为
	/// </summary>
	/// <param name="fixPoint">HangingTile实例位置</param>
	/// <param name="player">正在操作的玩家</param>
	public virtual void OnAdjustmentStart(Point fixPoint, Player player)
	{
	}

	/// <summary>
	/// 调整长度时的行为
	/// </summary>
	/// <param name="fixPoint">位置坐标</param>
	/// <param name="player">操作玩家</param>
	/// <param name="deltaLength">长度增量,值为0代表这帧没变化,如果不需要处理0请在值为0时直接return</param>
	public virtual void OnAdjustmentUpdate(Point fixPoint, Player player, int deltaLength)
	{
		Rope rope;
		RopesOfAllThisTileInTheWorld.TryGetValue(fixPoint, out rope);
		if (rope != null)
		{
			UpdateRopeAdjustment(rope, fixPoint);
		}
	}

	/// <summary>
	/// 结束调整时的行为
	/// </summary>
	/// <param name="fixPoint">位置坐标</param>
	/// <param name="player">操作玩家</param>
	public virtual void OnAdjustmentEnd(Point fixPoint, Player player)
	{
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		resetFrame = false;
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		Tile tile = Main.tile[i, j];
		tile.TileFrameY = 18;
	}

	public override void HitWire(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		tile.TileFrameX += 18;
		if (tile.TileFrameX > 54)
		{
			tile.TileFrameX = 0;
			tile.TileFrameY += 18;
		}
		int style = tile.TileFrameX / 18 + tile.TileFrameY / 18 * 4;
		MaxWireStyle = Math.Min(MaxWireStyle, 16);
		if (style >= MaxWireStyle)
		{
			tile.TileFrameX = 0;
			tile.TileFrameY = 0;
		}
		base.HitWire(i, j);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (RopesOfAllThisTileInTheWorld.ContainsKey(new Point(i, j)))
		{
			RopesOfAllThisTileInTheWorld.Remove(new Point(i, j));
		}
		base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
	}

	/// <summary>
	/// Add rope by TileEntity when approaching.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="closer"></param>
	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (!RopesOfAllThisTileInTheWorld.ContainsKey(new Point(i, j)))
		{
			AddRope(i, j);
		}
		base.NearbyEffects(i, j, closer);
	}

	/// <summary>
	/// It should not check the grasping logic of players in any drawing function.
	/// </summary>
	public virtual void CheckPlayerGrasp(Point pos)
	{
		Rope rope;
		RopesOfAllThisTileInTheWorld.TryGetValue(pos, out rope);
		if (rope is null)
		{
			return;
		}
		if (!RopeGraspingPlayer.ContainsKey(pos))
		{
			return;
		}
		Player graspPlayer = RopeGraspingPlayer[pos];
		if (graspPlayer is null)
		{
			return;
		}
		UpdateGraspingPlayer(graspPlayer, rope, pos);

		if (!RopeGraspingPlayer.ContainsKey(pos) || RopeGraspingPlayer[pos] != graspPlayer)
		{
			return;
		}

		// Check Remove Player
		if (graspPlayer is null || !graspPlayer.active || graspPlayer.controlJump || graspPlayer.mount.Active)
		{
			RemovePlayerFromRope(graspPlayer, rope, pos);
		}
	}

	/// <summary>
	/// Move the player.<br/>
	/// Animate the player.<br/>
	/// Swing the rope.<br/>
	/// Check switch rope.
	/// </summary>
	/// <param name="player"></param>
	/// <param name="rope"></param>
	/// <param name="pos"></param>
	public void UpdateGraspingPlayer(Player player, Rope rope, Point pos)
	{
		var masses = rope.Masses;
		var vineEndPosition = masses[^1].Position;

		// Player Kinematics
		player.Center = vineEndPosition;
		player.velocity *= 0;
		player.gravity *= 0;

		// Player Animation
		float rot = 0;
		if (masses.Length > 2)
		{
			rot = (masses[^2].Position - masses[^1].Position).ToRotation() + MathHelper.PiOver2;
		}
		player.fullRotation = rot;
		player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
		if (!player.controlUseItem && !player.controlUseTile)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot);
		}

		// Swing
		Vector2 ropeDir = (masses[0].Position - masses[^1].Position).NormalizeSafe();
		Vector2 pushDir = ropeDir.RotatedBy(MathHelper.PiOver2);
		if (player.controlLeft && ropeDir.X < 0.6f)
		{
			PushRope(ref rope, -pushDir * 4);
		}
		if (player.controlRight && ropeDir.X > -0.6f)
		{
			PushRope(ref rope, pushDir * 4);
		}

		// Switch Ropes
		if (player.controlDown)
		{
			CheckSwitchRope(player, rope, pos);
		}

		// Collision
		if (Collision.SolidCollision(player.position, player.width, player.height))
		{
			RemovePlayerFromRope(player, rope, pos);
		}
		if (player.controlHook)
		{
			RemovePlayerFromRope(player, rope, pos);
		}
	}

	public void CheckSwitchRope(Player player, Rope rope, Point pos)
	{
		float minDistance = GraspDetectRange + 10;
		Rope targetRope = null;
		Point targetPoint = new Point(0, 0);
		foreach (var hangingTile in TileLoader.tiles.OfType<HangingTile>())
		{
			// Exclude the current type.
			if (hangingTile == this)
			{
				continue;
			}
			bool findCloser = false;
			foreach (var rope_new in hangingTile.RopesOfAllThisTileInTheWorld.Values)
			{
				Vector2 ropeTipPos = rope_new.Masses.Last().Position;
				Vector2 playerToTip = ropeTipPos - player.Center;
				if (playerToTip.Length() < minDistance)
				{
					minDistance = playerToTip.Length();
					targetRope = rope_new;
					findCloser = true;
				}
			}
			if (findCloser)
			{
				targetPoint = hangingTile.RopesOfAllThisTileInTheWorld.FirstOrDefault(kv => kv.Value == targetRope).Key;
			}
		}
		foreach (var rope_new in RopesOfAllThisTileInTheWorld.Values)
		{
			// Exclude the current rope.
			if (RopesOfAllThisTileInTheWorld.ContainsKey(pos) && RopesOfAllThisTileInTheWorld[pos] == rope_new)
			{
				continue;
			}
			bool findCloser = false;
			Vector2 ropeTipPos = rope_new.Masses.Last().Position;
			Vector2 playerToTip = ropeTipPos - player.Center;
			if (playerToTip.Length() < minDistance)
			{
				minDistance = playerToTip.Length();
				targetRope = rope_new;
				findCloser = true;
			}
			if (findCloser)
			{
				targetPoint = RopesOfAllThisTileInTheWorld.FirstOrDefault(kv => kv.Value == targetRope).Key;
			}
		}
		if (targetRope is null)
		{
			return;
		}
		RemovePlayerFromRope(player, rope, pos);
		AddPlayerToRope(player, targetRope, targetPoint);
	}

	public void AddPlayerToRope(Player player, Rope rope, Point tilePos)
	{
		HangingTile_Player hTP = player.GetModPlayer<HangingTile_Player>();
		if (hTP is null)
		{
			return;
		}
		if (hTP.SwitchVineCoolTimer > 0)
		{
			return;
		}
		hTP.SwitchVineCoolTimer = 30;
		if (RopeGraspingPlayer.TryAdd(tilePos, player))
		{
			hTP.Grasping = true;
			PushRope(ref rope, player.velocity * 12f);
			player.RemoveAllGrapplingHooks();
		}
	}

	public void RemovePlayerFromRope(Player player, Rope rope, Point tilePos)
	{
		HangingTile_Player hTP = player.GetModPlayer<HangingTile_Player>();
		if (hTP is null)
		{
			return;
		}
		if (hTP.SwitchVineCoolTimer > 0)
		{
			return;
		}
		hTP.Grasping = false;
		var masses = rope.Masses;
		RopeGraspingPlayer.Remove(tilePos);
		player.gravity = Player.defaultGravity;
		Vector2 finalVel = masses[^1].Velocity * 3;
		player.velocity = finalVel;
		PushRope(ref rope, -finalVel * 16f);
		player.fullRotation = 0f;
	}

	/// <summary>
	/// Apply a force to the whole rope.
	/// </summary>
	/// <param name="rope"></param>
	/// <param name="force"></param>
	public void PushRope(ref Rope rope, Vector2 force)
	{
		var masses = rope.Masses;
		for (int i = 0; i < masses.Length; i++)
		{
			float value = i / (float)masses.Length;
			rope.ApplyForceSpecial(i, value * force);
		}
	}

	/// <summary>
	/// Add the rope.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="rope"></param>
	public virtual void AddRope(int i, int j)
	{
		Point pos = new Point(i, j);
		Tile tile = Main.tile[pos];
		int counts = MaxCableLength;
		int restCount = tile.TileFrameY;
		Rope rope = Rope.Create_Fixed_Start_Heavy_End(pos.ToWorldCoordinates(), counts, Elasticity, RopeUnitMass, HangingItemMass, MaxCableLength - restCount, UnitLength);
		if (rope == null)
		{
			return;
		}
		if (RopesOfAllThisTileInTheWorld.ContainsKey(pos))
		{
			return;
		}
		RopesOfAllThisTileInTheWorld.Add(pos, rope);
		TryGetCableEntityAs(i, j, out CableEneity cableEneity);
		if (cableEneity == null)
		{
			TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<CableEneity>());
			TryGetCableEntityAs(i, j, out cableEneity);
		}
		UpdateRopeAdjustment(rope, pos);

		// Initialize the positions of masses.
		var masses = rope.Masses;
		Vector2 currentPos = Vector2.zeroVector;
		for (int k = 0; k < masses.Length; k++)
		{
			Mass thisMass = masses[k];
			if (!thisMass.IsStatic)
			{
				currentPos += new Vector2(0, UnitLength);
				masses[k].Position = pos.ToWorldCoordinates() + currentPos;
			}
		}
	}

	/// <summary>
	/// Highlight a knob which can adjust the rope.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	public override void MouseOver(int i, int j)
	{
		base.MouseOver(i, j);
	}

	public override void MouseOverFar(int i, int j)
	{
		Player player = Main.LocalPlayer;
		Point point = new Point(i, j);
		if (!IsCanRightClick(i, j))
		{
			return;
		}
		if (LengthAdjustable && !RopeGraspingPlayer.ContainsKey(point))
		{
			if (!MouseOverWinchPlayers.ContainsKey(player) && TileUtils.CanPlayerInteractWithTile(i, j, player))
			{
				MouseOverWinchPlayers.Add(player, point);
				HangingTileAdjustingHelper vfx = new HangingTileAdjustingHelper { FixPoint = point, Active = true, Visible = true, Style = 0 };
				Ins.VFXManager.Add(vfx);
			}
			else if (MouseOverWinchPlayers[player] != point)
			{
				MouseOverWinchPlayers.Remove(player);
			}
			if (MouseOverWinchPlayers.ContainsKey(player))
			{
				if (Main.mouseRight && Main.mouseRightRelease)
				{
					if (LengthAdjustable && !RopeGraspingPlayer.ContainsKey(point))
					{
						Tile tile = Main.tile[point];
						if (!WinchAdjustingPlayers.ContainsKey(point))
						{
							OnAdjustmentStart(point, Main.LocalPlayer);
							HangingTileUpdateSystem.WinchAdjustingPlayerTimers[point] = AdjustingVisualTimeMax;
							HangingTileAdjustingHelper vfx = new HangingTileAdjustingHelper { FixPoint = point, Active = true, Visible = true, Style = 1, StartFrameY60 = tile.TileFrameY * 60 };
							vfx.RegisterCustomPanelDrawing(DrawDefaultPanel);
							vfx.HandleRotation = (point.ToWorldCoordinates() - player.MouseWorld()).ToRotationSafe();
							vfx.InitialHandleRotation = vfx.HandleRotation;
							vfx.Timer = 0;
							Ins.VFXManager.Add(vfx);
							SoundEngine.PlaySound(SoundID.Item17, point.ToWorldCoordinates());
							WinchAdjustingPlayers.Add(point, Main.LocalPlayer);
							AdjustingVisualTimeMax = 30;
						}
						else
						{
							WinchAdjustingPlayers.Remove(point);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Right click to adjust cable length.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <returns></returns>
	public override bool RightClick(int i, int j)
	{
		return base.RightClick(i, j);
	}

	/// <summary>
	/// 基于右键实现调整绳子长度的功能，你可以自定义开启调整旋钮的条件，默认当手持物品和右键物块类型相同时返回true。
	/// </summary>
	/// <param name="i">右键位置的横坐标</param>
	/// <param name="j">右键位置的纵坐标</param>
	/// <returns>值为true表示允许开启旋钮</returns>
	public virtual bool IsCanRightClick(int i, int j)
	{
		return Main.LocalPlayer.HeldItem.createTile == Main.tile[i, j].TileType;
	}

	/// <summary>
	/// The drawing part of the rope-adjusting-knob. 重写我来重绘按钮效果
	/// </summary>
	/// <param name="hangingSystem"></param>
	/// <param name="player"></param>
	/// <param name="color"></param>
	/// <param name="drawStacks"></param>
	public virtual void DrawDefaultPanel(HangingTileAdjustingHelper hangingSystem, Player player, Color color, ref Queue<DrawStack> drawStacks)
	{
		drawStacks = new Queue<DrawStack>();
		DrawBackgroundPanel(hangingSystem, Color.White * 0.5f, ref drawStacks);
		float maxCos = 0;
		int maxK = 0;
		Vector2 rotCenter = hangingSystem.FixPoint.ToWorldCoordinates();
		Vector2 cut = new Vector2(1, 0).RotatedBy(hangingSystem.HandleRotation + MathHelper.Pi);

		// Calculate the bloom effect of calibration tails.
		for (int k = -10; k < 10; k++)
		{
			Vector2 cut2 = new Vector2(0, -1).RotatedBy(k / 20f * MathHelper.TwoPi);
			float cosValue = Vector2.Dot(cut, cut2);
			if (cosValue > maxCos)
			{
				maxCos = cosValue;
				maxK = k;
			}
		}

		// Draw calibration tails.
		Color newDrawColor = color;
		float nowFrameY = hangingSystem.StartFrameY60 / 60f + hangingSystem.HandleRotation * 2;
		if (nowFrameY < 5)
		{
			newDrawColor = Color.Lerp(color, new Color(1f, 0f, 0f, 0.8f), (5 - nowFrameY) / 4f);
		}
		if (nowFrameY > MaxCableLength - 5)
		{
			newDrawColor = Color.Lerp(color, new Color(1f, 0f, 0f, 0.8f), (nowFrameY - (MaxCableLength - 5)) / 4f);
		}
		float tK = 12f;
		if (hangingSystem.TimeToKill > 0)
		{
			tK = hangingSystem.TimeToKill;
		}
		float fade = Math.Min(hangingSystem.Timer, tK) / 12f;
		for (int k = -10; k < 10; k++)
		{
			Vector2 cut2 = new Vector2(0, -1).RotatedBy(k / 20f * MathHelper.TwoPi);
			float cosValue = Vector2.Dot(cut, cut2);

			if (k == maxK)
			{
				DrawLine_Black(hangingSystem, rotCenter + cut2 * (4 * hangingSystem.PanelRange - 12), rotCenter + cut2 * (4 * hangingSystem.PanelRange + 36), 16, ref drawStacks);
				DrawLine(rotCenter + cut2 * (4 * hangingSystem.PanelRange - 12), rotCenter + cut2 * (4 * hangingSystem.PanelRange + 36), 16, newDrawColor * fade, 1f, ref drawStacks);
			}
			else
			{
				DrawLine(rotCenter + cut2 * 4 * hangingSystem.PanelRange, rotCenter + cut2 * (4 * hangingSystem.PanelRange + 24), 12, newDrawColor * fade, cosValue, ref drawStacks);
			}
		}

		// Draw bound.
		DrawBound(hangingSystem, color, player, ref drawStacks);
		DrawDirectionRing(hangingSystem, newDrawColor, hangingSystem.HandleRotation - MathHelper.PiOver2, ref drawStacks);
	}

	public virtual void DrawBound(HangingTileAdjustingHelper hangingSystem, Color color, Player player, ref Queue<DrawStack> drawStacks)
	{
		if (drawStacks == null)
		{
			drawStacks = new Queue<DrawStack>();
		}
		Vector2 rotCenter = hangingSystem.FixPoint.ToWorldCoordinates();
		if ((player.MouseWorld() - rotCenter).Length() > 240)
		{
			Vector2 cut3 = (player.MouseWorld() - rotCenter).NormalizeSafe();
			float colorValue = ((player.MouseWorld() - rotCenter).Length() - 240f) / 160f;
			colorValue = Math.Min(colorValue, 1);
			Color newDrawColor = Color.Lerp(color, new Color(1f, 0f, 0f, 0f), colorValue);
			newDrawColor.A = 0;
			float colorFade = 1f;
			if (hangingSystem.TimeToKill > 0)
			{
				colorFade = hangingSystem.TimeToKill / 12f;
			}
			Color backgroundColor = Color.White * 0.6f * colorFade;
			newDrawColor *= colorFade;
			Main.instance.MouseText("Drag out of circle to cancel", ItemRarityID.White);

			List<Vertex2D> bars_left = new List<Vertex2D>();
			List<Vertex2D> bars_right = new List<Vertex2D>();

			List<Vertex2D> bars_left_black = new List<Vertex2D>();
			List<Vertex2D> bars_right_black = new List<Vertex2D>();
			for (int k = 0; k < 30; k++)
			{
				float xCoord = (colorValue - k / 30f) * 0.8f;
				xCoord = Math.Clamp(xCoord, 0, 1);
				bars_left_black.Add(rotCenter + cut3.RotatedBy(k / 30f) * 400f, backgroundColor, new Vector3(xCoord, 0.5f, 0));
				bars_left_black.Add(rotCenter + cut3.RotatedBy(k / 30f) * 350f, backgroundColor, new Vector3(xCoord, 0.8f, 0));

				bars_right_black.Add(rotCenter + cut3.RotatedBy(-k / 30f) * 400f, backgroundColor, new Vector3(xCoord, 0.5f, 0));
				bars_right_black.Add(rotCenter + cut3.RotatedBy(-k / 30f) * 350f, backgroundColor, new Vector3(xCoord, 0.8f, 0));

				bars_left.Add(rotCenter + cut3.RotatedBy(k / 30f) * 400f, newDrawColor, new Vector3(xCoord, 0.5f, 0));
				bars_left.Add(rotCenter + cut3.RotatedBy(k / 30f) * 350f, newDrawColor, new Vector3(xCoord, 0.8f, 0));

				bars_right.Add(rotCenter + cut3.RotatedBy(-k / 30f) * 400f, newDrawColor, new Vector3(xCoord, 0.5f, 0));
				bars_right.Add(rotCenter + cut3.RotatedBy(-k / 30f) * 350f, newDrawColor, new Vector3(xCoord, 0.8f, 0));
			}

			drawStacks.Enqueue(new DrawStack(ModAsset.SparkDark.Value, bars_left_black, PrimitiveType.TriangleStrip));
			drawStacks.Enqueue(new DrawStack(ModAsset.SparkDark.Value, bars_right_black, PrimitiveType.TriangleStrip));

			drawStacks.Enqueue(new DrawStack(ModAsset.SparkLight.Value, bars_left, PrimitiveType.TriangleStrip));
			drawStacks.Enqueue(new DrawStack(ModAsset.SparkLight.Value, bars_right, PrimitiveType.TriangleStrip));

			List<Vertex2D> text_bars = new List<Vertex2D>();
			Color textColor = Color.White;
			for (int k = -30; k < 30; k++)
			{
				float xCoord = -k / 15f;
				float fade = (30 * colorValue - Math.Abs(k)) / 15f;
				fade = Math.Clamp(fade, 0f, 1f);
				fade = MathF.Sin(fade);
				text_bars.Add(rotCenter + cut3.RotatedBy(k / 30f) * 420f, textColor * fade * colorFade, new Vector3(xCoord, 1f, 0));
				text_bars.Add(rotCenter + cut3.RotatedBy(k / 30f) * 400f, textColor * fade * colorFade, new Vector3(xCoord, 0f, 0));
			}
			drawStacks.Enqueue(new DrawStack(ModAsset.HangingExit.Value, text_bars, PrimitiveType.TriangleStrip));
		}
	}

	public virtual void DrawDirectionRing(HangingTileAdjustingHelper hangingSystem, Color color, float rotation, ref Queue<DrawStack> drawStacks)
	{
		if (drawStacks == null)
		{
			drawStacks = new Queue<DrawStack>();
		}
		color.A = 0;
		Vector2 pos = hangingSystem.FixPoint.ToWorldCoordinates();
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int k = 0; k <= 60; k++)
		{
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 24).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(0.5f, k / 15f, 0));
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 0).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(0.3f, k / 15f, 0));
		}
		drawStacks.Enqueue(new DrawStack(ModAsset.StarSlash.Value, bars, PrimitiveType.TriangleStrip));
		bars = new List<Vertex2D>();
		for (int k = 0; k <= 60; k++)
		{
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 12).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(0.5f, k / 30f, 0));
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 0).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(0.3f, k / 30f, 0));
		}
		drawStacks.Enqueue(new DrawStack(ModAsset.StarSlash.Value, bars, PrimitiveType.TriangleStrip));
		bars = new List<Vertex2D>();
		for (int k = 0; k <= 60; k++)
		{
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 22).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(0.5f, k / 60f, 0));
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 12).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(0.3f, k / 60f, 0));
		}
		drawStacks.Enqueue(new DrawStack(ModAsset.StarSlash.Value, bars, PrimitiveType.TriangleStrip));
		bars = new List<Vertex2D>();
		for (int k = 0; k <= 60; k++)
		{
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 26).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(0.5f, 0.5f, 0));
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 24).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(0.5f, 0.5f, 0));
		}
		drawStacks.Enqueue(new DrawStack(ModAsset.StarSlash.Value, bars, PrimitiveType.TriangleStrip));
	}

	public virtual void DrawBackgroundPanel(HangingTileAdjustingHelper hangingSystem, Color color, ref Queue<DrawStack> drawStacks)
	{
		if (drawStacks == null)
		{
			drawStacks = new Queue<DrawStack>();
		}
		Vector2 pos = hangingSystem.FixPoint.ToWorldCoordinates();
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int k = 0; k <= 60; k++)
		{
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 70).RotatedBy(k / 60f * MathHelper.TwoPi), color, new Vector3(k / 60f, 0, 0));
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 0).RotatedBy(k / 60f * MathHelper.TwoPi), color, new Vector3(k / 60f, 0.5f, 0));
		}
		drawStacks.Enqueue(new DrawStack(ModAsset.Trail_7_black.Value, bars, PrimitiveType.TriangleStrip));
	}

	public virtual void DrawBlockBound(HangingTileAdjustingHelper hangingSystem, Color color, float rotation, ref Queue<DrawStack> drawStacks)
	{
		if (drawStacks == null)
		{
			drawStacks = new Queue<DrawStack>();
		}
		Vector2 pos = hangingSystem.FixPoint.ToWorldCoordinates();
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(pos + new Vector2(-8, -8).RotatedBy(rotation), color, new Vector3(0, 0, 0)),
			new Vertex2D(pos + new Vector2(8, -8).RotatedBy(rotation), color, new Vector3(1, 0, 0)),
			new Vertex2D(pos + new Vector2(-8, 8).RotatedBy(rotation), color, new Vector3(0, 1, 0)),

			new Vertex2D(pos + new Vector2(-8, 8).RotatedBy(rotation), color, new Vector3(0, 1, 0)),
			new Vertex2D(pos + new Vector2(8, -8).RotatedBy(rotation), color, new Vector3(1, 0, 0)),
			new Vertex2D(pos + new Vector2(8, 8).RotatedBy(rotation), color, new Vector3(1, 1, 0)),
		};
		drawStacks.Enqueue(new DrawStack(ModAsset.TileBlock.Value, bars, PrimitiveType.TriangleList));
	}

	public virtual void DrawLine(Vector2 pos1, Vector2 pos2, float width, Color color, float highlight, ref Queue<DrawStack> drawStacks)
	{
		if (drawStacks == null)
		{
			drawStacks = new Queue<DrawStack>();
		}
		Vector2 normal = Utils.SafeNormalize(pos1 - pos2, Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * width / 2f;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(pos1 + normal, color, new Vector3(0, 0, 0)),
			new Vertex2D(pos2 + normal, color, new Vector3(1, 0, 0)),
			new Vertex2D(pos1 - normal, color, new Vector3(0, 1, 0)),

			new Vertex2D(pos1 - normal, color, new Vector3(0, 1, 0)),
			new Vertex2D(pos2 + normal, color, new Vector3(1, 0, 0)),
			new Vertex2D(pos2 - normal, color, new Vector3(1, 1, 0)),
		};
		drawStacks.Enqueue(new DrawStack(ModAsset.SquarePiece.Value, bars, PrimitiveType.TriangleList));
		if (highlight > 0)
		{
			Color bloomColor = color * highlight;
			bloomColor.A = 0;
			bars = new List<Vertex2D>()
			{
				 new Vertex2D(pos1 + normal, bloomColor, new Vector3(0, 0, 0)),
				 new Vertex2D(pos2 + normal, bloomColor, new Vector3(1, 0, 0)),
				 new Vertex2D(pos1 - normal, bloomColor, new Vector3(0, 1, 0)),

				 new Vertex2D(pos1 - normal, bloomColor, new Vector3(0, 1, 0)),
				 new Vertex2D(pos2 + normal, bloomColor, new Vector3(1, 0, 0)),
				 new Vertex2D(pos2 - normal, bloomColor, new Vector3(1, 1, 0)),
			};
			drawStacks.Enqueue(new DrawStack(ModAsset.SquareBloom.Value, bars, PrimitiveType.TriangleList));
		}
	}

	public virtual void DrawLine_Black(HangingTileAdjustingHelper hangingSystem, Vector2 pos1, Vector2 pos2, float width, ref Queue<DrawStack> drawStacks)
	{
		if (drawStacks == null)
		{
			drawStacks = new Queue<DrawStack>();
		}
		Vector2 normal = Utils.SafeNormalize(pos1 - pos2, Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * width / 2f;
		Color bloomColor = Color.White;
		float tK = 12f;
		if (hangingSystem.TimeToKill > 0)
		{
			tK = hangingSystem.TimeToKill;
		}
		float fade = Math.Min(hangingSystem.Timer, tK) / 12f;
		bloomColor *= fade;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(pos1 + normal, bloomColor, new Vector3(0, 0, 0)),
			new Vertex2D(pos2 + normal, bloomColor, new Vector3(1, 0, 0)),
			new Vertex2D(pos1 - normal, bloomColor, new Vector3(0, 1, 0)),

			new Vertex2D(pos1 - normal, bloomColor, new Vector3(0, 1, 0)),
			new Vertex2D(pos2 + normal, bloomColor, new Vector3(1, 0, 0)),
			new Vertex2D(pos2 - normal, bloomColor, new Vector3(1, 1, 0)),
		};
		drawStacks.Enqueue(new DrawStack(ModAsset.SquareBloom_black.Value, bars, PrimitiveType.TriangleList));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		DrawWinch(i, j, spriteBatch);
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return false;
	}

	public virtual void DrawWinch(int i, int j, SpriteBatch spriteBatch)
	{
		Color lightColor = Lighting.GetColor(i, j);
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		spriteBatch.Draw(ModAsset.HangingWinch.Value, new Vector2(i, j) * 16 - Main.screenPosition + zero, new Rectangle(0, 0, 16, 16), lightColor, 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		Rope rope;
		RopesOfAllThisTileInTheWorld.TryGetValue(pos, out rope);
		if (rope != null)
		{
			DrawCable(rope, pos, spriteBatch, tileDrawing);
			if (CanGrasp && !Main.gamePaused)
			{
				CheckPlayerGrasp(pos);
			}
		}
	}

	/// <summary>
	/// Paint the hanging chains.
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
		string originalPath = @Texture;
		string[] pathSegments = originalPath.Split("/");
		string trimmedTexturePath = Path.Combine(pathSegments.Skip(1).ToArray());
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(trimmedTexturePath, type, 1, paint, tileDrawing);
		tex ??= (Texture2D)ModContent.Request<Texture2D>(Texture);

		var masses = rope.Masses;
		for (int i = 0; i < masses.Length; i++)
		{
			Mass thisMass = masses[i];
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
				if (i < masses.Length - 1)
				{
					rope.ApplyForceSpecial(i, new Vector2(windCycle / 4.0f, 0.4f * thisMass.Value));
				}
				else
				{
					rope.ApplyForceSpecial(i, new Vector2(windCycle * 10.0f, 0.4f * thisMass.Value));
				}
			}

			// 支持发光涂料
			Color tileLight;
			if (color != default)
			{
				tileLight = color;
			}
			else
			{
				tileLight = Lighting.GetColor((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f));
			}

			Vector2 toNextMass;
			if (i < masses.Length - 1)
			{
				Mass nextMass = masses[i + 1];
				toNextMass = nextMass.Position - thisMass.Position;
			}
			else
			{
				Mass passedMass = masses[i - 1];
				toNextMass = thisMass.Position - passedMass.Position;
			}
			Vector2 drawPos = thisMass.Position - Main.screenPosition;
			DrawRopeUnit(spriteBatch, tex, drawPos, pos, rope, i, toNextMass.ToRotation() - MathHelper.PiOver2, tileLight);
		}
	}

	public void UpdateRopeAdjustment(Rope rope, Point pos)
	{
		var masses = rope.Masses;
		var tile = TileUtils.SafeGetTile(pos);
		int currentHideUnitCount = MaxCableLength - tile.TileFrameY;
		if (currentHideUnitCount >= MaxCableLength - 2)
		{
			currentHideUnitCount = MaxCableLength - 2;
		}
		int oldHideUnitCount = 0;
		for (int i = 0; i < masses.Length; i++)
		{
			Mass thisMass = masses[i];
			if (thisMass.IsStatic)
			{
				oldHideUnitCount++;
			}
			else
			{
				break;
			}
		}
		for (int i = 0; i < masses.Length; i++)
		{
			Mass thisMass = masses[i];
			if (i <= currentHideUnitCount)
			{
				if (i == currentHideUnitCount)
				{
					Vector2 stride = pos.ToWorldCoordinates() - thisMass.Position;
					for (int i2 = i; i2 < masses.Length; i2++)
					{
						masses[i2].Position += stride;
					}
				}
				thisMass.IsStatic = true;
				thisMass.Position = pos.ToWorldCoordinates();
				continue;
			}
			else
			{
				thisMass.IsStatic = false;
				if (i < oldHideUnitCount)
				{
					thisMass.Position = pos.ToWorldCoordinates() + new Vector2(0, i - currentHideUnitCount) * UnitLength;
				}
				else if (i == oldHideUnitCount)
				{
					int deltaUnits = oldHideUnitCount - currentHideUnitCount - 1;
					Vector2 stride = new Vector2(0, UnitLength * deltaUnits);
					for (int i2 = i; i2 < masses.Length; i2++)
					{
						masses[i2].Position += stride;
					}
				}
			}
		}
	}

	/// <summary>
	/// Custom draw.
	/// </summary>
	/// <param name="spriteBatch"></param>
	/// <param name="texture"></param>
	/// <param name="drawPos">Screen Position</param>
	/// <param name="rope"></param>
	/// <param name="index"></param>
	/// <param name="rotation"></param>
	public virtual void DrawRopeUnit(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPos, Point tilePos, Rope rope, int index, float rotation, Color tileLight)
	{
		var masses = rope.Masses;
		Rectangle frame = new Rectangle(8 * (index % 4), 0, 8, 10);
		if (index < masses.Length - 1)
		{
			spriteBatch.Draw(texture, drawPos, frame, tileLight, rotation, new Vector2(4f, 0), 1f, SpriteEffects.None, 0);
		}
		else
		{
			spriteBatch.Draw(texture, drawPos, new Rectangle(0, 12, 32, 40), tileLight, rotation, new Vector2(16f, 0), 1f, SpriteEffects.None, 0);
			Lighting.AddLight(drawPos + Main.screenPosition, new Vector3(0.8f, 0.8f, 0.2f));
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
}
