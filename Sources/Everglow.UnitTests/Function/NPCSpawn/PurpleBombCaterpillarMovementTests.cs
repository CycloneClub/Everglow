using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Function.NPCSpawn;

[TestClass]
[DoNotParallelize]
public class PurpleBombCaterpillarMovementTests
{
	private readonly List<Action> restore = new();

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		int width = Main.maxTilesX;
		int height = Main.maxTilesY;
		bool dedicated = Main.dedServ;
		int netMode = Main.netMode;
		bool solid = Main.tileSolid[TileID.Stone];
		bool noHardmodeScaling = NPCID.Sets.DontDoHardmodeScaling[NPCID.None];
		var random = Main.rand;
		restore.Add(() =>
		{
			Main.maxTilesX = width;
			Main.maxTilesY = height;
			Main.dedServ = dedicated;
			Main.netMode = netMode;
			Main.tileSolid[TileID.Stone] = solid;
			Main.rand = random;
			NPCID.Sets.DontDoHardmodeScaling[NPCID.None] = noHardmodeScaling;
		});
		Main.maxTilesX = 1000;
		Main.maxTilesY = 1000;
		Main.dedServ = true;
		Main.netMode = NetmodeID.Server;
		Main.tileSolid[TileID.Stone] = true;
		Main.rand = new UnifiedRandom(7);
		for (int x = 80; x < 120; x++)
		{
			for (int y = 80; y < 120; y++)
			{
				Tile tile = Main.tile[x, y];
				var old = (tile.TileType, tile.HasTile, tile.IsActuated, tile.Slope, tile.IsHalfBlock);
				restore.Add(() =>
				{
					tile.TileType = old.TileType;
					tile.HasTile = old.HasTile;
					tile.IsActuated = old.IsActuated;
					tile.Slope = old.Slope;
					tile.IsHalfBlock = old.IsHalfBlock;
				});
				tile.HasTile = false;
				tile.IsActuated = false;
				tile.Slope = SlopeType.Solid;
				tile.IsHalfBlock = false;
				tile.TileType = TileID.Stone;
			}
		}
	}

	[TestCleanup]
	public void Cleanup()
	{
		for (int i = restore.Count - 1; i >= 0; i--)
		{
			restore[i]();
		}
		restore.Clear();
	}

	private static T Create<T>()
		where T : ModNPC, new()
	{
		var content = new T();
		var npc = (NPC)RuntimeHelpers.GetUninitializedObject(typeof(NPC));
		npc.scale = 1;
		typeof(ModNPC).GetProperty("Entity")!.SetValue(content, npc);
		content.SetDefaults();
		return content;
	}

	private static void SetTile(int x, int y, bool active)
	{
		Tile tile = Main.tile[x, y];
		tile.HasTile = active;
	}

	private static void Floor(int top = 100)
	{
		for (int x = 80; x < 120; x++)
		{
			for (int y = top; y < 120; y++)
			{
				SetTile(x, y, true);
			}
		}
	}

	private static PurpleBombCaterpillar CreateBomb(Vector2 feet, int direction = 1)
	{
		var bomb = Create<PurpleBombCaterpillar>();
		bomb.NPC.Bottom = feet;
		bomb.NPC.direction = bomb.NPC.spriteDirection = direction;
		// Keep the existing idle decisions deterministic without running targeting.
		bomb.NPC.target = 0;
		return bomb;
	}

	private static void TickBomb(PurpleBombCaterpillar bomb)
	{
		bomb.AI();
		if (!bomb.NPC.noGravity)
		{
			bomb.NPC.velocity.Y += 0.3f;
		}
		Vector2 requested = bomb.NPC.velocity;
		Vector2 actual = bomb.NPC.noTileCollide ? requested : Collision.TileCollision(bomb.NPC.position, requested, bomb.NPC.width, bomb.NPC.height);
		bomb.NPC.oldVelocity = requested;
		bomb.NPC.velocity = actual;
		bomb.NPC.collideX = actual.X != requested.X;
		bomb.NPC.collideY = actual.Y != requested.Y;
		bomb.NPC.position += actual;
	}

	[DataTestMethod]
	[DataRow(1, 0.85f)]
	[DataRow(-1, 0.85f)]
	[DataRow(1, 1f)]
	[DataRow(-1, 1f)]
	[DataRow(1, 1.15f)]
	[DataRow(-1, 1.15f)]
	public void BombCaterpillarStopsAtTallWallThenTurnsAndMovesAway(int direction, float scale)
	{
		Floor();
		for (int x = 80; x < 120; x++)
		{
			if (direction > 0 ? x >= 103 : x <= 96)
			{
				for (int y = 94; y < 100; y++)
				{
					SetTile(x, y, true);
				}
			}
		}
		var bomb = CreateBomb(new Vector2(1600, 1600), direction);
		bomb.NPC.scale = scale;
		bool turned = false;
		int stoppedFrame = -1;
		int turnedFrame = -1;
		float stoppedX = 0;
		var speed = typeof(PurpleBombCaterpillar).GetField("crawlSpeed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
		float closestGap = float.MaxValue;
		var getContact = typeof(PurpleBombCaterpillar).GetField("surfacePosition", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
		for (int frame = 0; frame < 700; frame++)
		{
			TickBomb(bomb);
			Vector2 contact = (Vector2)getContact.GetValue(bomb)!;
			Assert.IsTrue(contact.Y > 1560, $"Must not climb the wall: {contact}.");
			var getTop = typeof(PurpleBombCaterpillar).GetField("bodyTops", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
			float front = bomb.BodyFeet.Select((p, i) => direction > 0 ? Math.Max(p.X, ((Vector2[])getTop.GetValue(bomb)!)[i].X) : Math.Min(p.X, ((Vector2[])getTop.GetValue(bomb)!)[i].X)).Aggregate((a, b) => direction > 0 ? Math.Max(a, b) : Math.Min(a, b));
			closestGap = Math.Min(closestGap, direction > 0 ? 1648 - front : front - 1552);
			if (!turned && bomb.NPC.spriteDirection == -direction)
			{
				turnedFrame = frame;
			}
			turned |= bomb.NPC.spriteDirection == -direction;
			if (!turned && stoppedFrame >= 0)
			{
				Assert.AreEqual(stoppedX, contact.X, 0.01f, "Stay stopped until the turn phase.");
			}
			if (!turned && closestGap <= -1.8f && stoppedFrame < 0 && (float)speed.GetValue(bomb)! == 0)
			{
				stoppedFrame = frame;
				stoppedX = contact.X;
			}
			if (turned && (contact.X - 1600) * direction < -24)
			{
				break;
			}
		}
		Assert.IsTrue(closestGap <= -1.8f && closestGap >= -2.15f, $"Expected two-pixel inset at wall: gap={closestGap}.");
		Assert.IsTrue(stoppedFrame >= 0 && turnedFrame > stoppedFrame, "Stop at contact before turning on a later frame.");
		Assert.IsTrue(turned);
		Assert.IsTrue((((Vector2)getContact.GetValue(bomb)!).X - 1600) * direction < -24, "Move away after turning.");
		Assert.AreEqual(-direction, bomb.NPC.direction);
		Assert.IsFalse(bomb.Angry);
	}

	[TestMethod]
	public void BombCaterpillarConformsToConsecutiveSingleTileStairs()
	{
		for (int x = 80; x < 120; x++)
		{
			int rise = Math.Clamp(x - 101, 0, 5);
			for (int y = 100 - rise; y < 120; y++)
			{
				SetTile(x, y, true);
			}
		}
		var bomb = CreateBomb(new Vector2(1600, 1600));
		bool bent = false;
		for (int frame = 0; frame < 1400 && bomb.NPC.Center.X < 1730; frame++)
		{
			TickBomb(bomb);
			var feet = bomb.BodyFeet;
			if (feet.Count > 2 && feet.Max(p => p.Y) - feet.Min(p => p.Y) > 24)
			{
				bent = true;
				foreach (Vector2 foot in feet)
				{
					Assert.IsTrue(SurfaceDistance(foot) >= -2.15f, $"Foot exceeded visual inset at {foot}.");
				}
			}
		}
		Assert.IsTrue(bent, "The body should span several stair heights.");
		Assert.IsTrue(bomb.NPC.Center.X > 1720, $"Stopped at {bomb.NPC.Center}.");
	}

	[DataTestMethod]
	[DataRow(0.85f, 1)]
	[DataRow(1f, 1)]
	[DataRow(1.15f, 1)]
	[DataRow(0.85f, -1)]
	[DataRow(1f, -1)]
	[DataRow(1.15f, -1)]
	public void BombCaterpillarBridgesSingleTileHole(float scale, int direction)
	{
		Floor();
		for (int y = 100; y < 120; y++)
		{
			SetTile(102, y, false);
		}
		var bomb = CreateBomb(new Vector2(direction > 0 ? 1600 : 1696, 1600), direction);
		bomb.NPC.scale = scale;
		var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		var getContact = typeof(PurpleBombCaterpillar).GetField("surfacePosition", flags)!;
		var getRadius = typeof(PurpleBombCaterpillar).GetField("surfaceRadius", flags)!;
		bool passed = false;
		for (int frame = 0; frame < 800; frame++)
		{
			TickBomb(bomb);
			Vector2 contact = (Vector2)getContact.GetValue(bomb)!;
			float radius = (float)getRadius.GetValue(bomb)!;
			Assert.IsTrue(bomb.NPC.noGravity, "A one-tile gap should retain support.");
			Assert.AreEqual(1600f, contact.Y + radius, 0.1f, "Cross the gap at the rim height instead of following its interior.");
			passed = direction > 0 ? bomb.NPC.Center.X >= 1680 : bomb.NPC.Center.X <= 1600;
			if (passed)
			{
				break;
			}
		}
		Assert.IsTrue(passed, $"Stopped at {bomb.NPC.Center}.");
	}

	[DataTestMethod]
	[DataRow(0.85f, false, 2)]
	[DataRow(0.85f, true, 2)]
	[DataRow(1f, false, 2)]
	[DataRow(1f, true, 2)]
	[DataRow(1.15f, false, 2)]
	[DataRow(1.15f, true, 2)]
	[DataRow(0.85f, false, 6)]
	[DataRow(0.85f, true, 6)]
	[DataRow(1f, false, 6)]
	[DataRow(1f, true, 6)]
	[DataRow(1.15f, false, 6)]
	[DataRow(1.15f, true, 6)]
	public void BombCaterpillarFallsOffCliffInsteadOfClingingToWall(float scale, bool mirror, int drop)
	{
		int lowerTop = 100 + drop;
		Floor(lowerTop);
		for (int x = 80; x < 120; x++)
		{
			if ((mirror ? 200 - x : x) >= 103)
			{
				continue;
			}
			for (int y = 100; y < lowerTop; y++)
			{
				SetTile(x, y, true);
			}
		}
		var bomb = CreateBomb(new Vector2(mirror ? 1616 : 1600, 1600), mirror ? -1 : 1);
		bomb.NPC.scale = scale;
		bool fell = false;
		bool wasAttached = false;
		int falls = 0;
		int stableGroundFrames = 0;
		for (int frame = 0; frame < 1200; frame++)
		{
			// 保留完整 IdleAI 和伸缩节奏，验证真实掉落、落地与继续爬行。
			TickBomb(bomb);
			if (wasAttached && !bomb.NPC.noGravity)
			{
				falls++;
			}
			fell |= !bomb.NPC.noGravity;
			wasAttached = bomb.NPC.noGravity;
			if (fell && bomb.NPC.noGravity && bomb.NPC.Center.Y > lowerTop * 16 - 30)
			{
				stableGroundFrames++;
				if (stableGroundFrames >= 60)
				{
					break;
				}
			}
			else
			{
				stableGroundFrames = 0;
			}
		}
		Assert.IsTrue(fell, "A cliff must release ground attachment.");
		Assert.AreEqual(1, falls, "Do not repeatedly drop and reattach on the same ledge.");
		Assert.AreEqual(60, stableGroundFrames, $"Must land and stay grounded on the lower floor: {bomb.NPC.Center}.");
	}

	[TestMethod]
	public void BombCaterpillarGroundHitDoesNotDetach()
	{
		Floor();
		var bomb = CreateBomb(new Vector2(1600, 1600));
		TickBomb(bomb);
		bomb.HitEffect(new NPC.HitInfo { Damage = 1 });
		Assert.IsTrue(bomb.NPC.noGravity);
	}

	[TestMethod]
	public void BombCaterpillarStillWalksAcrossPlatforms()
	{
		bool solid = Main.tileSolid[TileID.Platforms];
		bool top = Main.tileSolidTop[TileID.Platforms];
		restore.Add(() =>
		{
			Main.tileSolid[TileID.Platforms] = solid;
			Main.tileSolidTop[TileID.Platforms] = top;
		});
		Main.tileSolid[TileID.Platforms] = Main.tileSolidTop[TileID.Platforms] = true;
		for (int x = 80; x < 120; x++)
		{
			Tile tile = Main.tile[x, 100];
			short frameY = tile.TileFrameY;
			restore.Add(() => tile.TileFrameY = frameY);
			tile.TileType = TileID.Platforms;
			tile.HasTile = true;
			tile.TileFrameY = 0;
		}
		var bomb = CreateBomb(new Vector2(1600, 1600));
		for (int frame = 0; frame < 400; frame++)
		{
			TickBomb(bomb);
		}
		Assert.IsTrue(bomb.NPC.Center.X > 1660, $"Stopped on platform: {bomb.NPC.Center}.");
		Assert.IsTrue(bomb.NPC.Center.Y < 1600);
	}

	[TestMethod]
	public void BombCaterpillarNetworkFallRestoresCompactCollisionBox()
	{
		Floor();
		for (int y = 90; y < 100; y++)
		{
			SetTile(103, y, true);
		}
		var source = CreateBomb(new Vector2(1600, 1600));
		TickBomb(source);
		var remote = CreateBomb(Vector2.Zero);
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true);
		using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true);
		source.SendExtraAI(writer);
		stream.Position = 0;
		remote.ReceiveExtraAI(reader);
		remote.UpdateRotation();
		Assert.IsTrue(remote.NPC.width > remote.NPC.height);
		typeof(PurpleBombCaterpillar).GetMethod("DetachFromSurface", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(source, null);
		stream.SetLength(0);
		stream.Position = 0;
		source.SendExtraAI(writer);
		stream.Position = 0;
		remote.NPC.position = source.NPC.position; // SyncNPC transmits position, but not width/height.
		remote.ReceiveExtraAI(reader);
		Assert.AreEqual(36, remote.NPC.width);
		Assert.AreEqual(36, remote.NPC.height);
		Assert.AreEqual(source.NPC.position, remote.NPC.position);
		Assert.AreEqual(source.NPC.spriteDirection, remote.NPC.spriteDirection);
	}

	[DataTestMethod]
	[DataRow(SlopeType.Solid, false)]
	[DataRow(SlopeType.Solid, true)]
	[DataRow(SlopeType.SlopeDownRight, false)]
	[DataRow(SlopeType.SlopeDownLeft, false)]
	[DataRow(SlopeType.SlopeUpRight, false)]
	[DataRow(SlopeType.SlopeUpLeft, false)]
	public void BombCaterpillarSurfaceMatchesNativeTileShape(SlopeType slope, bool halfBlock)
	{
		Tile tile = Main.tile[100, 100];
		tile.HasTile = true;
		tile.Slope = slope;
		tile.IsHalfBlock = halfBlock;
		var sample = typeof(PurpleBombCaterpillar).GetMethod("SampleSurface", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
		for (int x = 1; x < 16; x += 2)
		{
			for (int y = 1; y < 16; y += 2)
			{
				Vector2 position = new Vector2(1600 + x, 1600 + y);
				Vector3 result = (Vector3)sample.Invoke(null, new object[] { position, 1 })!;
				if (MathF.Abs(result.X) < 0.01f)
				{
					continue;
				}
				Assert.AreEqual(Collision.IsWorldPointSolid(position), result.X < 0, $"Mismatch at {position}, slope={slope}, halfBlock={halfBlock}.");
			}
		}
	}

	private static float Cross(Vector2 a, Vector2 b) => a.X * b.Y - a.Y * b.X;

	[TestMethod]
	public void BombCaterpillarStairMeshDoesNotFoldOverItself()
	{
		for (int x = 80; x < 120; x++)
		{
			int rise = Math.Clamp(x - 101, 0, 5);
			for (int y = 100 - rise; y < 120; y++)
			{
				SetTile(x, y, true);
			}
		}
		var bomb = CreateBomb(new Vector2(1600, 1600));
		var getTop = typeof(PurpleBombCaterpillar).GetField("bodyTops", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
		for (int frame = 0; frame < 700; frame++)
		{
			TickBomb(bomb);
			for (int i = 0; i < bomb.BodyFeet.Count - 1; i++)
			{
				Vector2 bottom = bomb.BodyFeet[i];
				Vector2 nextBottom = bomb.BodyFeet[i + 1];
				Vector2 top = ((Vector2[])getTop.GetValue(bomb)!)[i];
				Vector2 nextTop = ((Vector2[])getTop.GetValue(bomb)!)[i + 1];
				Assert.IsTrue(Cross(top - bottom, nextBottom - bottom) > 0, $"Lower triangle flipped: frame={frame}, segment={i}.");
				Assert.IsTrue(Cross(nextTop - top, nextBottom - top) > 0, $"Upper triangle flipped: frame={frame}, segment={i}.");
			}
		}
	}

	[DataTestMethod]
	[DataRow(-1, 0f)]
	[DataRow(1, 0f)]
	[DataRow(-1, 1.5707964f)]
	[DataRow(1, 3.1415927f)]
	public void BombCaterpillarEyeRootRemainsOnBentHead(int direction, float rotation)
	{
		// A curved head translated and rotated into floor/wall/ceiling orientations.
		var feet = Enumerable.Range(0, 11).Select(i => new Vector2(i * 9, -MathF.Sin(i * 0.3f) * 20)).ToArray();
		for (int i = 0; i < feet.Length; i++)
		{
			float x = feet[i].X;
			float y = feet[i].Y;
			feet[i] = new Vector2(x * MathF.Cos(rotation) - y * MathF.Sin(rotation), x * MathF.Sin(rotation) + y * MathF.Cos(rotation)) + new Vector2(1600, 1500);
		}
		var flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic;
		var buildTop = typeof(PurpleBombCaterpillar).GetMethod("BuildBodyEdges", flags)!;
		var getPose = typeof(PurpleBombCaterpillar).GetMethod("GetEyePose", flags)!;
		var edges = ((Vector2[] Feet, Vector2[] Tops))buildTop.Invoke(null, new object[] { feet, 36f, false })!;
		feet = edges.Feet;
		Vector2[] tops = edges.Tops;
		var pose = ((Vector2 Position, float Rotation, Vector2 Origin))getPose.Invoke(null, new object[] { feet, tops, direction, 134 })!;
		// This is the visible root pixel in the existing 134x54 animation frame.
		Vector2 sourceRoot = new Vector2(direction == 1 ? 134 - 21 : 21, 41);
		Vector2 offset = sourceRoot - pose.Origin;
		Vector2 root = pose.Position + new Vector2(offset.X * MathF.Cos(pose.Rotation) - offset.Y * MathF.Sin(pose.Rotation), offset.X * MathF.Sin(pose.Rotation) + offset.Y * MathF.Cos(pose.Rotation));
		int segment = direction == 1 ? feet.Length - 2 : 0;
		bool ContainsRoot(Vector2 a, Vector2 b, Vector2 c)
		{
			float area = Cross(b - a, c - a);
			float u = Cross(b - root, c - root) / area;
			float v = Cross(c - root, a - root) / area;
			return u >= 0 && v >= 0 && u + v <= 1;
		}
		Assert.IsTrue(
			ContainsRoot(tops[segment], feet[segment], feet[segment + 1]) ||
			ContainsRoot(tops[segment], tops[segment + 1], feet[segment + 1]),
			"The rendered eye root must remain inside the head mesh when its attachment is adjusted.");
		Vector2 outward = new Vector2(MathF.Sin(pose.Rotation), -MathF.Cos(pose.Rotation));
		Vector2 bodyOutward = Vector2.Normalize(tops[segment] - feet[segment]);
		Assert.IsTrue(Vector2.Dot(outward, bodyOutward) > 0.9f, "The stalk must rotate with the local head, including on walls and ceilings.");
	}

	[DataTestMethod]
	[DataRow(-1, 1f, 0f)]
	[DataRow(1, 1f, 0f)]
	[DataRow(-1, 0.85f, 0.6f)]
	[DataRow(1, 0.85f, 0.6f)]
	[DataRow(-1, 1.15f, -1.1f)]
	[DataRow(1, 1.15f, -1.1f)]
	public void BombCaterpillarEyeOffsetMatchesOriginalFlatPose(int direction, float scale, float rotation)
	{
		Vector2 bottom = new Vector2(1600, 1500);
		var feet = Enumerable.Range(0, 41)
			.Select(i => bottom + new Vector2((i / 40f - 0.5f) * 98, 0).RotatedBy(rotation) * scale)
			.ToArray();
		var tops = feet.Select(foot => foot + new Vector2(0, -36).RotatedBy(rotation) * scale).ToArray();
		var getPose = typeof(PurpleBombCaterpillar).GetMethod("GetEyePose", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
		var pose = ((Vector2 Position, float Rotation, Vector2 Origin))getPose.Invoke(null, new object[] { feet, tops, direction, 134 })!;

		// Original draw: NPC.Bottom, frame.Size() * (0.5, 0.95), then scale and rotate.
		Vector2 sourceRoot = new Vector2(direction == 1 ? 134 - 21 : 21, 41);
		Vector2 originalOrigin = new Vector2(134 * 0.5f, 54 * 0.95f);
		Vector2 expected = bottom + (sourceRoot - originalOrigin).RotatedBy(rotation) * scale;
		Assert.AreEqual(expected.X, pose.Position.X, 0.001f, "Preserve the original horizontal eye offset.");
		Assert.AreEqual(expected.Y, pose.Position.Y, 0.001f, "Preserve the original vertical eye offset.");
		Assert.AreEqual(sourceRoot, pose.Origin, "Keep the source eye root attached to the deforming body.");
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void BombCaterpillarBendingAllowsLimitedCompression(bool bentTerrain)
	{
		for (int x = 80; x < 120; x++)
		{
			int rise = bentTerrain ? Math.Clamp(x - 101, 0, 5) : 0;
			for (int y = 100 - rise; y < 120; y++)
			{
				SetTile(x, y, true);
			}
		}
		var bomb = CreateBomb(new Vector2(1600, 1600));
		var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		var getTop = typeof(PurpleBombCaterpillar).GetField("bodyTops", flags)!;
		var getSqueeze = typeof(PurpleBombCaterpillar).GetMethod("GetSqueeze", flags)!;
		bool compressed = false;
		// 平地检查在抵达测试平台边缘前结束，避免将悬崖转弯误算为平地收缩。
		for (int frame = 0; frame < (bentTerrain ? 700 : 400); frame++)
		{
			TickBomb(bomb);
			float squeeze = (float)getSqueeze.Invoke(bomb, null)!;
			// 长度约束作用于材质主中心线；腹部下垂属于局部厚度形变，另行验证。
			var buildSpine = typeof(PurpleBombCaterpillar).GetMethod("BuildBodySpine", flags)!;
			var spine = (Vector2[])buildSpine.Invoke(bomb, new object[] { 98 * squeeze * bomb.NPC.scale, 36 * (2 - squeeze) * bomb.NPC.scale })!;
			float expected = 98 * squeeze * bomb.NPC.scale;
			float length = 0;
			for (int i = 1; i < spine.Length; i++)
			{
				length += Vector2.Distance(spine[i - 1], spine[i]);
			}
			Assert.IsTrue(length >= expected * 0.9f - 0.02f && length <= expected + 0.02f, $"Compression exceeded 10% at frame={frame}: {length}/{expected}.");
			if (!bentTerrain)
			{
				Assert.AreEqual(expected, length, 0.02f, $"Flat ground should retain the original crawling length: frame={frame}, center={bomb.NPC.Center}.");
			}
			compressed |= length < expected * 0.995f;
			float spacing = length / (spine.Length - 1);
			for (int i = 1; i < spine.Length; i++)
			{
				Assert.AreEqual(spacing, Vector2.Distance(spine[i - 1], spine[i]), spacing * 0.04f, "UV columns need equal arc-length spacing.");
				if (i < 2)
				{
					continue;
				}
				Vector2 before = Vector2.Normalize(spine[i - 1] - spine[i - 2]);
				Vector2 after = Vector2.Normalize(spine[i] - spine[i - 1]);
				float bend = MathF.Acos(Math.Clamp(Vector2.Dot(before, after), -1, 1));
				Assert.IsTrue(bend < 0.35f, $"Sharp texture kink at frame={frame}, column={i}, angle={bend}.");
			}
		}
		if (bentTerrain)
		{
			Assert.IsTrue(compressed, "Bending should allow a little relaxation instead of enforcing a rigid length.");
		}
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void BombCaterpillarMeshStaysContinuousAroundSingleCorner(bool isolatedTile)
	{
		if (isolatedTile)
		{
			SetTile(100, 100, true);
		}
		else
		{
			Floor();
			for (int x = 101; x < 120; x++)
			{
				SetTile(x, 99, true);
			}
		}
		var bomb = CreateBomb(new Vector2(isolatedTile ? 1608 : 1600, 1600));
		TickBomb(bomb);
		var getTop = typeof(PurpleBombCaterpillar).GetField("bodyTops", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
		Vector2[] feet = bomb.BodyFeet.ToArray();
		Vector2[] tops = feet.Select((_, i) => ((Vector2[])getTop.GetValue(bomb)!)[i]).ToArray();
		var materialSpine = (Vector2[])typeof(PurpleBombCaterpillar).GetMethod("BuildBodySpine", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(bomb, new object[] { 98f, 36f })!;
		float length = 0;
		for (int i = 0; i < feet.Length - 1; i++)
		{
			Assert.IsTrue(Cross(tops[i] - feet[i], feet[i + 1] - feet[i]) > 0, $"Lower triangle flipped at column={i}.");
			Assert.IsTrue(Cross(tops[i + 1] - tops[i], feet[i + 1] - tops[i]) > 0, $"Upper triangle flipped at column={i}.");
			length += Vector2.Distance(materialSpine[i], materialSpine[i + 1]);
		}
		Assert.IsTrue(length >= 98 * 0.9f - 0.02f && length <= 98.02f, "Corner compression must stay within 10%.");
	}

	[DataTestMethod]
	[DataRow(1, 0.85f)]
	[DataRow(1, 1.15f)]
	[DataRow(-1, 0.85f)]
	[DataRow(-1, 1.15f)]
	public void BombCaterpillarStopsBeforeLowOverhang(int direction, float scale)
	{
		// 00111 / 00111 / 00000 / 11111, plus its horizontal mirror.
		Floor(103);
		int left = direction == 1 ? 102 : 95;
		int right = left + 3;
		for (int x = left; x < right; x++)
		{
			SetTile(x, 100, true);
			SetTile(x, 101, true);
		}
		var bomb = CreateBomb(new Vector2(1600, 1648), direction);
		bomb.NPC.scale = scale;
		var getContact = typeof(PurpleBombCaterpillar).GetField("surfacePosition", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;

		bool turned = false;
		for (int frame = 0; frame < 700; frame++)
		{
			TickBomb(bomb);
			Vector2 contact = (Vector2)getContact.GetValue(bomb)!;
			bool insideColumns = contact.X >= left * 16 && contact.X < right * 16;
			Assert.IsFalse(insideColumns, $"Must not enter the low passage or climb over it: {contact}.");
			turned |= bomb.NPC.spriteDirection == -direction;
			if (turned && (contact.X - 1600) * direction < -24)
			{
				break;
			}
		}
		Assert.IsTrue(turned);
		Assert.AreEqual(-direction, bomb.NPC.direction);
		Assert.IsFalse(bomb.Angry);
	}

	[TestMethod]
	public void LargeBombCaterpillarCanAttachAfterLandingBesideWall()
	{
		Floor();
		for (int x = 103; x < 120; x++)
		{
			for (int y = 90; y < 100; y++)
			{
				SetTile(x, y, true);
			}
		}
		var bomb = CreateBomb(new Vector2(1630, 1600));
		bomb.NPC.scale = 1.15f;
		for (int frame = 0; frame < 400; frame++)
		{
			TickBomb(bomb);
		}
		Assert.IsTrue(bomb.NPC.noGravity, "Landing next to a wall must still attach to the supported surface.");
		Assert.IsTrue(bomb.NPC.Center.Y > 1550, $"Must stay on the ground: {bomb.NPC.Center}.");
		Assert.AreEqual(-1, bomb.NPC.spriteDirection);
	}

	[DataTestMethod]
	[DataRow(0.85f, false)]
	[DataRow(0.85f, true)]
	[DataRow(1f, false)]
	[DataRow(1f, true)]
	[DataRow(1.15f, false)]
	[DataRow(1.15f, true)]
	public void BombCaterpillarBasinMeshMovesContinuously(float scale, bool turnBack)
	{
		// 1000001 / 1000011 / 1000111 / 1111111.
		Floor(103);
		for (int y = 100; y < 103; y++)
		{
			for (int x = 80; x < 120; x++)
			{
				if (x <= 100 || x >= 206 - y)
				{
					SetTile(x, y, true);
				}
			}
		}
		var bomb = CreateBomb(new Vector2(1728, 1600), -1);
		bomb.NPC.scale = scale;
		var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		var getTop = typeof(PurpleBombCaterpillar).GetField("bodyTops", flags)!;
		var getContact = typeof(PurpleBombCaterpillar).GetField("surfacePosition", flags)!;
		var speed = typeof(PurpleBombCaterpillar).GetField("crawlSpeed", flags)!;
		Vector2[] previous = null;
		Vector2[] previousMaterial = null;
		var buildSpine = typeof(PurpleBombCaterpillar).GetMethod("BuildBodySpine", flags)!;
		Vector2 lastContact = Vector2.Zero;
		float worst = 0;
		float worstCenter = 0;
		string detail = string.Empty;
		bool enteredBasin = false;
		for (int frame = 0; frame < 450; frame++)
		{
			// 固定蠕动相位，单独检查连续运动引起的几何变化，避免原有伸缩脉冲干扰。
			if (turnBack && frame == 165)
			{
				bomb.NPC.direction = bomb.NPC.spriteDirection = 1;
			}
			speed.SetValue(bomb, bomb.NPC.spriteDirection * 0.75f);
			bomb.MoveTimer = 0;
			bomb.MoveScale = 0.3f;
			bomb.UpdateMoveAndLight();
			var tops = bomb.BodyFeet.Select((_, i) => ((Vector2[])getTop.GetValue(bomb)!)[i]).ToArray();
			var points = bomb.BodyFeet.Concat(tops).ToArray();
			var material = (Vector2[])buildSpine.Invoke(bomb, new object[] { 98 * scale, 36 * scale })!;
			Vector2 contact = (Vector2)getContact.GetValue(bomb)!;
			enteredBasin |= contact.Y > 1610;
			for (int i = 0; i < tops.Length; i++)
			{
				Vector2 foot = bomb.BodyFeet[i];
				Vector2 top = tops[i];
				Assert.IsTrue(Math.Min(SurfaceDistance(foot), SurfaceDistance(top)) >= -2.15f, $"Mesh exceeded visual inset: frame={frame}, column={i}, foot={foot}, top={top}.");
			}
			if (previous != null)
			{
				float jump = points.Select((p, i) => Vector2.Distance(p, previous[i])).Max();
				for (int i = 0; i < tops.Length; i++)
				{
					Vector2 center = material[i];
					Vector2 oldCenter = previousMaterial[i];
					worstCenter = Math.Max(worstCenter, Vector2.Distance(center, oldCenter));
				}

				if (jump > worst)
				{
					worst = jump;
					detail = $"frame={frame}, contact={lastContact}->{contact}, vertex={previous[20]}->{points[20]}, attached={bomb.NPC.noGravity}";
				}
			}
			previous = points;
			previousMaterial = material;
			lastContact = contact;
		}
		Assert.IsTrue(enteredBasin, "Must descend into the basin before evaluating the return path.");
		Assert.IsTrue(turnBack ? lastContact.X > 1700 : bomb.NPC.spriteDirection == -1, $"Unexpected basin direction: {lastContact}.");
		// 主中心线保留原连续性要求；腹部另有局部厚度变化，边缘允许额外一像素位移。
		Assert.IsTrue(worstCenter < 4, $"Centerline jumped {worstCenter} pixels; vertex={worst}; {detail}.");
		Assert.IsTrue(worst < 7, $"Body jumped {worst} pixels: {detail}");
	}

	private static float SurfaceDistance(Vector2 point)
	{
		var sample = typeof(PurpleBombCaterpillar).GetMethod("SampleSurface", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
		return ((Vector3)sample.Invoke(null, new object[] { point, 2 })!).X;
	}

	[DataTestMethod]
	[DataRow(0.85f)]
	[DataRow(1f)]
	[DataRow(1.15f)]
	public void BombCaterpillarVisualInsetDoesNotLowerMovementClearance(float scale)
	{
		Floor();
		var bomb = CreateBomb(new Vector2(1600, 1600));
		bomb.NPC.scale = scale;
		TickBomb(bomb);
		foreach (Vector2 foot in bomb.BodyFeet)
		{
			Assert.AreEqual(1602f, foot.Y, 0.05f, "The flat-ground outline should sit slightly inside the surface.");
		}
		var contact = (Vector2)typeof(PurpleBombCaterpillar).GetField("surfacePosition", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(bomb)!;
		Assert.AreEqual(18 * scale + 1, SurfaceDistance(contact), 0.05f, "The movement path must retain its full clearance.");
	}

	[DataTestMethod]
	[DataRow(0.85f, 1)]
	[DataRow(1f, 1)]
	[DataRow(1.15f, 1)]
	[DataRow(0.85f, -1)]
	[DataRow(1f, -1)]
	[DataRow(1.15f, -1)]
	public void BombCaterpillarNarrowNotchDoesNotTrapOrTeleport(float scale, int direction)
	{
		Floor(104);
		for (int x = 101; x <= 102; x++)
		{
			for (int y = 104; y < 106; y++)
			{
				SetTile(x, y, false);
			}
		}
		var bomb = CreateBomb(new Vector2(direction > 0 ? 1568 : 1696, 1664), direction);
		bomb.NPC.scale = scale;
		var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		var getTop = typeof(PurpleBombCaterpillar).GetField("bodyTops", flags)!;
		var getContact = typeof(PurpleBombCaterpillar).GetField("surfacePosition", flags)!;
		var speed = typeof(PurpleBombCaterpillar).GetField("crawlSpeed", flags)!;
		Vector2[] previous = null;
		for (int frame = 0; frame < 240; frame++)
		{
			speed.SetValue(bomb, direction * 0.75f);
			bomb.MoveTimer = 0;
			bomb.UpdateMoveAndLight();
			Assert.IsTrue(bomb.NPC.noGravity, "The body-sized gap should retain support at its rims.");
			var centers = bomb.BodyFeet.Select((p, i) => (p + ((Vector2[])getTop.GetValue(bomb)!)[i]) * 0.5f).ToArray();
			if (previous != null)
			{
				Assert.IsTrue(centers.Select((p, i) => Vector2.Distance(p, previous[i])).Max() < 6, $"Mesh jumped at frame {frame}.");
			}
			foreach (Vector2 foot in bomb.BodyFeet)
			{
				Assert.IsTrue(SurfaceDistance(foot) >= -2.15f, $"Foot deeply embedded at {foot}.");
			}
			previous = centers;
		}
		Vector2 contact = (Vector2)getContact.GetValue(bomb)!;
		Assert.IsTrue(direction > 0 ? contact.X > 1680 : contact.X < 1584, $"Trapped in notch: {contact}.");
	}

	[DataTestMethod]
	[DataRow(1, 1)]
	[DataRow(1, -1)]
	[DataRow(-1, 1)]
	[DataRow(-1, -1)]
	public void BombCaterpillarTraversesNativeSlopes(int slopeDirection, int direction)
	{
		int Top(int x) => 101 - Math.Clamp((slopeDirection > 0 ? x : 200 - x) - 99, 0, 4);
		for (int x = 80; x < 120; x++)
		{
			for (int y = Top(x); y < 120; y++)
			{
				SetTile(x, y, true);
			}
			int localX = slopeDirection > 0 ? x : 200 - x;
			if (localX >= 100 && localX <= 103)
			{
				Tile tile = Main.tile[x, Top(x)];
				tile.Slope = slopeDirection > 0 ? SlopeType.SlopeDownRight : SlopeType.SlopeDownLeft;
			}
		}
		int start = direction > 0 ? 94 : 110;
		var bomb = CreateBomb(new Vector2(start * 16, Top(start) * 16), direction);
		for (int frame = 0; frame < 700; frame++)
		{
			TickBomb(bomb);
			if (direction > 0 ? bomb.NPC.Center.X > 1720 : bomb.NPC.Center.X < 1500)
			{
				break;
			}
		}
		Assert.IsTrue(direction > 0 ? bomb.NPC.Center.X > 1720 : bomb.NPC.Center.X < 1500, $"Stopped at slope: {bomb.NPC.Center}.");
	}

	[TestMethod]
	public void BombCaterpillarWalksUnderCeilingWithEnoughHeadroom()
	{
		Floor(104);
		for (int x = 102; x <= 105; x++)
		{
			SetTile(x, 100, true);
		}
		var bomb = CreateBomb(new Vector2(1600, 1664));
		for (int frame = 0; frame < 800 && bomb.NPC.Center.X < 1730; frame++)
		{
			TickBomb(bomb);
			Assert.IsTrue(bomb.NPC.Center.Y > 1620, "Do not attach to the ceiling.");
		}
		Assert.IsTrue(bomb.NPC.Center.X >= 1730, $"Stopped under a passable ceiling: {bomb.NPC.Center}.");
	}

	[TestMethod]
	public void BombCaterpillarStopsAtHeadContactButCanLeaveWhenDirectionChanges()
	{
		Floor();
		for (int x = 103; x < 120; x++)
		{
			for (int y = 94; y < 100; y++)
			{
				SetTile(x, y, true);
			}
		}
		var bomb = CreateBomb(new Vector2(1504, 1600));
		var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		var speed = typeof(PurpleBombCaterpillar).GetField("crawlSpeed", flags)!;
		var getContact = typeof(PurpleBombCaterpillar).GetField("surfacePosition", flags)!;
		Vector2? stoppedAt = null;
		for (int frame = 0; frame < 240; frame++)
		{
			speed.SetValue(bomb, 0.75f);
			bomb.MoveTimer = 0;
			bomb.UpdateMoveAndLight();
			Vector2 contact = (Vector2)getContact.GetValue(bomb)!;
			Assert.AreEqual(1, bomb.NPC.spriteDirection);
			if ((float)speed.GetValue(bomb)! == 0 && bomb.BodyFeet[^1].X >= 1649.5f)
			{
				if (stoppedAt != null)
				{
					Assert.AreEqual(stoppedAt.Value.X, contact.X, 0.01f, "Stop the motion point as well as the rendered head.");
				}
				stoppedAt = contact;
			}
		}
		Assert.IsNotNull(stoppedAt);
		bomb.NPC.direction = bomb.NPC.spriteDirection = -1;
		for (int i = 0; i < 30; i++)
		{
			speed.SetValue(bomb, -0.75f);
			bomb.UpdateMoveAndLight();
		}
		Assert.IsTrue(((Vector2)getContact.GetValue(bomb)!).X < stoppedAt.Value.X - 16, "A later direction decision must release the blocked movement.");
	}

	[TestMethod]
	public void BombCaterpillarBellyRelaxesLocallyIntoStairs()
	{
		for (int x = 80; x < 120; x++)
		{
			for (int y = 100 - Math.Clamp(x - 101, 0, 5); y < 120; y++)
			{
				SetTile(x, y, true);
			}
		}
		var bomb = CreateBomb(new Vector2(1600, 1600));
		var sagField = typeof(PurpleBombCaterpillar).GetField("bellySag", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
		float deepest = 0;
		for (int frame = 0; frame < 400; frame++)
		{
			TickBomb(bomb);
			var sag = (float[])sagField.GetValue(bomb)!;
			deepest = Math.Max(deepest, sag.Max());
			Assert.IsTrue(sag.All(value => value >= 0 && value <= 8.01f), "Local thickness relaxation must remain bounded.");
			foreach (Vector2 foot in bomb.BodyFeet)
			{
				Assert.IsTrue(SurfaceDistance(foot) >= -2.15f, "The relaxed belly must preserve the two-pixel penetration limit.");
			}
		}
		Assert.IsTrue(deepest > 2, $"The belly must visibly relax between steps, measured {deepest}px.");
	}

	[TestMethod]
	public void BombCaterpillarBellyRetractsGraduallyAboveUnsupportedGround()
	{
		Floor();
		var bomb = CreateBomb(new Vector2(1600, 1600));
		((List<Vector2>)typeof(PurpleBombCaterpillar).GetField("bodyFeet", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(bomb)!).Add(new Vector2(1600, 1500));
		var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		var sag = new[] { 4f };
		typeof(PurpleBombCaterpillar).GetField("bellySag", flags)!.SetValue(bomb, sag);
		typeof(PurpleBombCaterpillar).GetMethod("RelaxBelly", flags)!.Invoke(bomb, null);
		Assert.AreEqual(3.5f, sag[0], 0.01f, "Missing ground must not instantly reset belly deformation.");
		Assert.AreEqual(1503.5f, bomb.BodyFeet[0].Y, 0.01f);
	}

	[DataTestMethod]
	[DataRow(1, 0.85f, false)]
	[DataRow(-1, 0.85f, false)]
	[DataRow(1, 1f, false)]
	[DataRow(-1, 1f, false)]
	[DataRow(1, 1.15f, false)]
	[DataRow(-1, 1.15f, false)]
	[DataRow(1, 1f, true)]
	[DataRow(-1, 1f, true)]
	public void BombCaterpillarKeepsStrideAcrossSlopeThenStep(int direction, float scale, bool halfBlock)
	{
		for (int x = 80; x < 120; x++)
		{
			int localX = direction > 0 ? x : 200 - x;
			int top = localX < 100 ? 100 : localX == 100 ? 99 : halfBlock && localX == 101 ? 99 : 98;
			for (int y = top; y < 120; y++)
			{
				SetTile(x, y, true);
			}
			if (localX == 100)
			{
				Tile tile = Main.tile[x, top];
				tile.IsHalfBlock = halfBlock;
				if (!halfBlock)
				{
					tile.Slope = direction > 0 ? SlopeType.SlopeDownRight : SlopeType.SlopeDownLeft;
				}
			}
		}
		float startX = direction > 0 ? 1536 : 1680;
		var bomb = CreateBomb(new Vector2(startX, 1600), direction);
		bomb.NPC.scale = scale;
		var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		var contactField = typeof(PurpleBombCaterpillar).GetField("surfacePosition", flags)!;
		var pendingField = typeof(PurpleBombCaterpillar).GetField("obstacleTurnPending", flags)!;
		float expectedSpeed = 0;
		float expectedTravel = 0;
		for (int frame = 0; frame < 400; frame++)
		{
			TickBomb(bomb);
			if (bomb.MoveTimer % 60 == 30)
			{
				expectedSpeed += 1.8f * scale;
			}
			else
			{
				expectedSpeed *= 0.95f;
			}
			expectedTravel += expectedSpeed;
			Vector2 contact = (Vector2)contactField.GetValue(bomb)!;
			Assert.IsTrue(bomb.NPC.noGravity, $"Lost ground on climb: frame={frame}, contact={contact}.");
			Assert.IsFalse((bool)pendingField.GetValue(bomb)!, $"False wall: frame={frame}, contact={contact}, phase={bomb.MoveTimer}.");
			Assert.AreEqual(direction, bomb.NPC.spriteDirection, "Traversable terrain must not reverse travel.");
			Assert.AreEqual(expectedTravel, (contact.X - startX) * direction, 0.1f, $"Terrain shortened stride at frame {frame}.");
		}
	}

	[DataTestMethod]
	[DataRow(1, 0.85f)]
	[DataRow(-1, 0.85f)]
	[DataRow(1, 1f)]
	[DataRow(-1, 1f)]
	[DataRow(1, 1.15f)]
	[DataRow(-1, 1.15f)]
	public void BombCaterpillarKeepsStrideAcrossUnevenGround(int direction, float scale)
	{
		int[] rises = { 0, 1, 2, 1, 0, 0, 1, 2, 3, 2, 1, 0 };
		for (int x = 80; x < 120; x++)
		{
			int localX = direction > 0 ? x : 200 - x;
			int index = localX - 96;
			int top = 100 - (index >= 0 && index < rises.Length ? rises[index] : 0);
			for (int y = top; y < 120; y++)
			{
				SetTile(x, y, true);
			}
			Tile tile = Main.tile[x, top];
			if (localX == 97)
			{
				tile.IsHalfBlock = true;
			}
			if (localX == 98 || localX == 103)
			{
				tile.Slope = direction > 0 ? SlopeType.SlopeDownRight : SlopeType.SlopeDownLeft;
			}
		}
		float startX = direction > 0 ? 1504 : 1712;
		var bomb = CreateBomb(new Vector2(startX, 1600), direction);
		bomb.NPC.scale = scale;
		var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		var contactField = typeof(PurpleBombCaterpillar).GetField("surfacePosition", flags)!;
		var pendingField = typeof(PurpleBombCaterpillar).GetField("obstacleTurnPending", flags)!;
		float expectedSpeed = 0;
		float expectedTravel = 0;
		for (int frame = 0; frame < 900; frame++)
		{
			TickBomb(bomb);
			if (bomb.MoveTimer % 60 == 30)
			{
				expectedSpeed += 1.8f * scale;
			}
			else
			{
				expectedSpeed *= 0.95f;
			}
			expectedTravel += expectedSpeed;
			Vector2 contact = (Vector2)contactField.GetValue(bomb)!;
			Assert.IsTrue(bomb.NPC.noGravity, $"Lost ground: frame={frame}, contact={contact}.");
			Assert.IsFalse((bool)pendingField.GetValue(bomb)!, $"False wall: frame={frame}, contact={contact}.");
			Assert.AreEqual(direction, bomb.NPC.spriteDirection, "Uneven ground must not reverse travel.");
			Assert.AreEqual(expectedTravel, (contact.X - startX) * direction, 0.15f, $"Terrain shortened stride at frame {frame}.");
			if (expectedTravel >= 320)
			{
				break;
			}
		}
		Assert.IsTrue(expectedTravel >= 320, "Must traverse the entire uneven section.");
	}

	[DataTestMethod]
	[DataRow(0.85f, false, 0)]
	[DataRow(0.85f, true, 0)]
	[DataRow(1f, false, 0)]
	[DataRow(1f, true, 0)]
	[DataRow(1.15f, false, 0)]
	[DataRow(1.15f, true, 0)]
	[DataRow(0.85f, false, 1)]
	[DataRow(0.85f, true, 1)]
	[DataRow(1f, false, 1)]
	[DataRow(1f, true, 1)]
	[DataRow(1.15f, false, 1)]
	[DataRow(1.15f, true, 1)]
	[DataRow(0.85f, false, 2)]
	[DataRow(0.85f, true, 2)]
	[DataRow(1f, false, 2)]
	[DataRow(1f, true, 2)]
	[DataRow(1.15f, false, 2)]
	[DataRow(1.15f, true, 2)]
	public void BombCaterpillarDescendingSteepTerracesDoesNotFlicker(float scale, bool mirror, int profile)
	{
		// 截图中的缓坡接陡坎、两格直落台阶、窄落脚面的连续三格陡坎。
		int[] tops = profile switch
		{
			0 => new[] { 105, 105, 104, 103, 102, 101, 98, 97, 96, 96, 96, 96, 95, 91, 90, 90 },
			1 => new[] { 92, 92, 92, 92, 92, 92, 92, 92, 92, 90, 90, 90, 90, 90, 90, 90 },
			_ => new[] { 102, 102, 102, 102, 99, 99, 96, 96, 93, 93, 90, 90, 90, 90, 90, 90 },
		};
		for (int x = 80; x < 120; x++)
		{
			int localX = mirror ? 200 - x : x;
			int top = tops[Math.Clamp(localX - 92, 0, tops.Length - 1)];
			for (int y = top; y < 120; y++)
			{
				SetTile(x, y, true);
			}
			if (profile == 0 && localX is 94 or 95 or 96 or 99 or 100 or 104 or 106)
			{
				Tile tile = Main.tile[x, top];
				tile.Slope = mirror ? SlopeType.SlopeDownLeft : SlopeType.SlopeDownRight;
			}
		}
		int direction = mirror ? 1 : -1;
		float endX = mirror ? 1726 : 1490;
		var bomb = CreateBomb(new Vector2(mirror ? 1472 : 1744, 1440), direction);
		bomb.NPC.scale = scale;
		var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		var topsField = typeof(PurpleBombCaterpillar).GetField("bodyTops", flags)!;
		var contactField = typeof(PurpleBombCaterpillar).GetField("surfacePosition", flags)!;
		var speedField = typeof(PurpleBombCaterpillar).GetField("crawlSpeed", flags)!;
		Vector2[] previous = null;
		Vector2 lastContact = Vector2.Zero;
		bool lastAttached = false;
		float worst = 0;
		string detail = string.Empty;
		for (int frame = 0; frame < 1500; frame++)
		{
			// 固定蠕动相位，单独检查地形变化；腾空时仍运行真实重力和碰撞。
			if (frame == 0 || bomb.NPC.noGravity)
			{
				speedField.SetValue(bomb, direction * 0.75f);
				bomb.MoveTimer = 0;
				bomb.MoveScale = 0.3f;
				bomb.UpdateMoveAndLight();
			}
			else
			{
				bomb.MoveTimer = -0.25f;
				TickBomb(bomb);
			}
			Vector2 contact = (Vector2)contactField.GetValue(bomb)!;
			Vector2[] points = bomb.BodyFeet.Concat((Vector2[])topsField.GetValue(bomb)!).ToArray();
			if (previous != null && bomb.NPC.noGravity && lastAttached)
			{
				float jump = points.Select((p, j) => MathF.Abs(p.Y - previous[j].Y)).Max();
				float excess = jump - MathF.Abs(contact.Y - lastContact.Y);
				if (excess > worst)
				{
					worst = excess;
					detail = $"frame={frame}, jump={jump}, contact={lastContact}->{contact}, phase={bomb.MoveTimer}";
				}
			}
			previous = points;
			lastContact = contact;
			lastAttached = bomb.NPC.noGravity;
			if ((bomb.NPC.Center.X - endX) * direction > 0 && bomb.NPC.noGravity)
			{
				break;
			}
		}
		Assert.IsTrue(worst < 7, $"Body flashed vertically by {worst}px beyond ground movement: {detail}.");
		Assert.IsTrue((bomb.NPC.Center.X - endX) * direction > 0 && bomb.NPC.noGravity, $"Did not descend terraces: {bomb.NPC.Center}.");
	}

	private Player SetChaseTarget(bool active = true)
	{
		Player[] oldPlayers = Main.player;
		restore.Add(() => Main.player = oldPlayers);
		Main.player = Enumerable.Range(0, Main.maxPlayers + 1)
			.Select(_ => (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player))).ToArray();
		var player = new Player { whoAmI = 0, active = active, width = 20, height = 42 };
		player.Bottom = new Vector2(1800, 1600);
		Main.player[0] = player;
		return player;
	}

	[DataTestMethod]
	[DataRow(0f)]
	[DataRow(0.25f)]
	[DataRow(0.5f)]
	[DataRow(0.99f)]
	public void BombCaterpillarAwakeStrideKeepsSixtyTickCycle(float fraction)
	{
		Floor();
		var bomb = CreateBomb(new Vector2(1504, 1600));
		bomb.MoveTimer = 28 + fraction;
		var speedField = typeof(PurpleBombCaterpillar).GetField("crawlSpeed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
		var impulses = new List<int>();
		float previousSpeed = 0;
		for (int frame = 0; frame < 123; frame++)
		{
			TickBomb(bomb);
			float speed = (float)speedField.GetValue(bomb)!;
			if (speed > previousSpeed + 0.1f)
			{
				impulses.Add(frame);
			}
			Assert.AreEqual((29 + fraction + frame) % 60, bomb.MoveTimer, 0.001f, "Awake movement must advance once per tick without random pauses.");
			previousSpeed = speed;
		}
		CollectionAssert.AreEqual(new[] { 1, 61, 121 }, impulses.ToArray(), "Fractional phases must not skip or duplicate a push.");
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void BombCaterpillarAngerDoesNotSlowDownBehindTerrain(bool hasPlayer)
	{
		Floor();
		for (int y = 90; y < 100; y++)
		{
			SetTile(105, y, true);
		}
		Player player = SetChaseTarget(hasPlayer);
		var bomb = CreateBomb(new Vector2(1504, 1600));
		Assert.IsFalse(Collision.CanHit(bomb.NPC, player), "The target must be occluded in this regression.");
		bomb.MoveTimer = 8.25f;
		bomb.HitEffect(new NPC.HitInfo { Damage = 1 });
		for (int frame = 0; frame < 80; frame++)
		{
			TickBomb(bomb);
			Assert.IsTrue(bomb.Angry);
			Assert.AreEqual(1f, bomb.MoveScale, "Occlusion or a missing target must not downgrade an angry NPC to idle speed.");
			Assert.AreEqual((9.25f + frame) % 60, bomb.MoveTimer, 0.001f);
		}
	}

	[TestMethod]
	public void BombCaterpillarRepeatedHitsPreserveStridePhase()
	{
		Floor();
		SetChaseTarget();
		var bomb = CreateBomb(new Vector2(1504, 1600));
		bomb.MoveTimer = 5.75f;
		var speedField = typeof(PurpleBombCaterpillar).GetField("crawlSpeed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
		int impulses = 0;
		float previousSpeed = 0;
		for (int frame = 0; frame < 90; frame++)
		{
			bomb.HitEffect(new NPC.HitInfo { Damage = 1 });
			TickBomb(bomb);
			float speed = (float)speedField.GetValue(bomb)!;
			if (speed > previousSpeed + 0.1f)
			{
				impulses++;
			}
			Assert.AreEqual((6.75f + frame) % 60, bomb.MoveTimer, 0.001f, "Repeated damage must not restart the action.");
			previousSpeed = speed;
		}
		Assert.AreEqual(2, impulses, "Both scheduled pushes must survive repeated hits.");
	}
}
