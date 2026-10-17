using System.Reflection;
using Everglow.Yggdrasil.YggdrasilTown.Travelers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TravelerNPCCollisionTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	[DataRow(32, false, false, false)]
	[DataRow(20700, false, false, false)]
	[DataRow(32, true, false, false)]
	[DataRow(20700, true, false, false)]
	[DataRow(32, true, true, false)]
	[DataRow(20700, true, true, false)]
	[DataRow(32, true, false, true)]
	[DataRow(20700, true, false, true)]
	public void NativeNPCCompletesRouteUsingActualTerrainCollision(int floorY, bool stackedPlatforms, bool dropThrough, bool ascendPlatform)
	{
		var oldTiles = Main.tile;
		int oldWidth = Main.maxTilesX;
		int oldHeight = Main.maxTilesY;
		bool oldSolid = Main.tileSolid[TileID.Stone];
		bool oldPlatformSolid = Main.tileSolid[TileID.Platforms];
		bool oldPlatformTop = Main.tileSolidTop[TileID.Platforms];
		try
		{
			Main.maxTilesX = 64;
			Main.maxTilesY = floorY + 32;
			var constructor = typeof(Tilemap).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, [typeof(ushort), typeof(ushort)], null)!;
			Main.tile = (Tilemap)constructor.Invoke([(ushort)Main.maxTilesX, (ushort)Main.maxTilesY]);
			Main.tileSolid[TileID.Stone] = true;
			Main.tileSolid[TileID.Platforms] = true;
			Main.tileSolidTop[TileID.Platforms] = true;
			for (int x = 0; x < Main.maxTilesX; x++)
			{
				var tile = Main.tile[x, floorY];
				tile.HasTile = true;
				tile.TileType = TileID.Stone;
			}
			if (stackedPlatforms)
			{
				for (int x = 30; x <= 34; x++)
				{
					var tile = Main.tile[x, floorY - (ascendPlatform ? 1 : 2)];
					tile.HasTile = true;
					tile.TileType = TileID.Platforms;
				}
			}
			bool Clear(Point point)
			{
				if (point.X < 8 || point.X > 56 || point.Y < floorY - 8 || point.Y >= floorY)
				{
					return false;
				}
				for (int y = point.Y - 2; y <= point.Y; y++)
				{
					var tile = Main.tile[point.X, y];
					if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
					{
						return false;
					}
				}
				return true;
			}
			bool Stand(Point point) => Clear(point) && Main.tile[point.X, point.Y + 1].HasTile;
			Vector2 Feet(Point point) => new(point.X * 16 + 8, point.Y * 16 + 16);
			bool Platform(Point point) => Main.tile[point.X, point.Y + 1].TileType == TileID.Platforms;
			Point start = ascendPlatform ? new(28, floorY - 1) : new(32, floorY - (stackedPlatforms ? 3 : 1));
			Point goal = ascendPlatform ? new(32, floorY - 2) : new(dropThrough ? 32 : stackedPlatforms ? 31 : 28, floorY - 1);
			var route = TravelerNavigation.FindPath(start, goal, Stand, Clear,
				canDropThrough: dropThrough || ascendPlatform ? Platform : null);
			Assert.IsTrue(route.Count > 1);
			var npc = new NPC { width = 16, height = 48, noGravity = true, aiStyle = -1 };
			npc.Bottom = Feet(route[0]);
			int waypoint = 1;
			bool jumping = false;
			float stepSpeed = 1;
			float stepOffset = 0;
			for (int tick = 0; tick < 240 && waypoint < route.Count; tick++)
			{
				bool grounded = Math.Abs(npc.velocity.Y) < 0.05f && Collision.TileCollision(npc.position, new Vector2(0, 2), npc.width, npc.height).Y < 1.9f;
				if (TravelerRules.ReachedWaypoint(npc.Bottom, Feet(route[waypoint]), grounded))
				{
					waypoint++;
					jumping = false;
					continue;
				}
				bool dropping = TravelerNavigation.CanDropBetween(route[waypoint - 1], route[waypoint], Clear, Stand, dropThrough ? Platform : null);
				bool fall = dropping && TravelerRules.FallThroughSourcePlatform(npc.Bottom.Y, Feet(route[waypoint - 1]).Y);
				if (grounded && !dropping && !TravelerNavigation.CanWalkBetween(route[waypoint - 1], route[waypoint], Clear, Stand, ascendPlatform ? Platform : null))
				{
					jumping = true;
					npc.velocity.Y = TravelerNavigation.JumpVelocity;
				}
				float speed = jumping ? TravelerNavigation.JumpSpeed : 1.5f;
				npc.velocity.X = Math.Clamp(Feet(route[waypoint]).X - npc.Bottom.X, -speed, speed);
				npc.velocity.Y += TravelerNavigation.Gravity;
				if (!dropping)
				{
					Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref stepSpeed, ref stepOffset);
				}
				Invoke(npc, "Collision_WalkDownSlopes");
				npc.oldVelocity = npc.velocity;
				npc.velocity = Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height, fall);
				Invoke(npc, "Collision_MoveWhileDry");
				Invoke(npc, "Collision_MoveSlopesAndStairFall", fall);
			}
			Assert.AreEqual(route.Count, waypoint, $"NPC stalled at {npc.Bottom}, target {Feet(route[Math.Min(waypoint, route.Count - 1)])}.");
		}
		finally
		{
			Main.tile = oldTiles;
			Main.maxTilesX = oldWidth;
			Main.maxTilesY = oldHeight;
			Main.tileSolid[TileID.Stone] = oldSolid;
			Main.tileSolid[TileID.Platforms] = oldPlatformSolid;
			Main.tileSolidTop[TileID.Platforms] = oldPlatformTop;
		}
	}

	private static void Invoke(NPC npc, string name, params object[] args) =>
		typeof(NPC).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(npc, args);
}
