using Everglow.Commons.Skeleton2D;
using Everglow.Commons.Skeleton2D.Reader;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Boss = Everglow.Yggdrasil.YggdrasilTown.NPCs.SquamousShell.SquamousShell;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class SquamousShellServerTests
{
	private bool originalDedServ;
	private int originalNetMode;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalDedServ = Main.dedServ;
		originalNetMode = Main.netMode;
		Main.dedServ = true;
		Main.netMode = Terraria.ID.NetmodeID.Server;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.dedServ = originalDedServ;
		Main.netMode = originalNetMode;
	}

	[TestMethod]
	public void Skeleton_WithoutGraphics_UpdatesEveryBossAnimation()
	{
		var skeleton = LoadBossSkeleton();
		foreach (var animation in skeleton.Skeleton.Data.Animations)
		{
			skeleton.AnimationState.SetAnimation(0, animation.Name, false);
			skeleton.AnimationState.Update(animation.Duration * 0.5f);
			skeleton.AnimationState.Apply(skeleton.Skeleton);
			skeleton.Position = new Vector2(100, 200);
			foreach (var bone in skeleton.Skeleton.Bones)
			{
				Assert.IsTrue(float.IsFinite(bone.WorldX) && float.IsFinite(bone.WorldY), $"{animation.Name}: {bone.Data.Name}");
			}
		}
	}

	[TestMethod]
	public void OnSpawn_WithHeadlessSkeleton_StartsLandingWithoutReadingTextures()
	{
		var boss = (Boss)new Boss().NewInstance(new NPC());
		boss.SquamousShellSkeleton = LoadBossSkeleton();

		boss.OnSpawn(new Terraria.DataStructures.EntitySource_Misc("Test"));
		boss._coroutineManager.Update();

		Assert.AreEqual(2.2f, boss.NPC.velocity.Y, 0.001f);
	}

	[TestMethod]
	public void VisualEffects_OnDedicatedServer_DoNotRequireGoreOrVFXRegistration()
	{
		var boss = (Boss)new Boss().NewInstance(new NPC());
		boss.DiswingEffect();
		boss.GenerateSmogAtBottom(120);
		boss.OnKill();
	}

	[TestMethod]
	public void ExtraAI_TransfersFlightAndAnimationState()
	{
		var server = (Boss)new Boss().NewInstance(new NPC());
		server.SquamousShellSkeleton = LoadBossSkeleton();
		server.Flying = true;
		server.SmashValue = 0.75f;
		server.NPC.rotation = 1.25f;
		server.SquamousShellSkeleton.AnimationState.SetAnimation(0, "fly", true).TrackTime = 0.5f;
		var client = (Boss)new Boss().NewInstance(new NPC());
		client.SquamousShellSkeleton = LoadBossSkeleton();
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		server.SendExtraAI(writer);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		client.ReceiveExtraAI(reader);

		Assert.IsTrue(client.Flying);
		Assert.AreEqual(0.75f, client.SmashValue);
		Assert.AreEqual(1.25f, client.NPC.rotation);
		Assert.AreEqual("fly", client.SquamousShellSkeleton.AnimationState.GetCurrent(0).Animation.Name);
		Assert.AreEqual(0.5f, client.SquamousShellSkeleton.AnimationState.GetCurrent(0).TrackTime);
	}

	[TestMethod]
	public void OnSpawn_OnMultiplayerClient_DoesNotStartAuthoritativeMovement()
	{
		Main.netMode = Terraria.ID.NetmodeID.MultiplayerClient;
		var boss = (Boss)new Boss().NewInstance(new NPC());
		boss.SquamousShellSkeleton = LoadBossSkeleton();
		boss.OnSpawn(new Terraria.DataStructures.EntitySource_Misc("Test"));
		boss._coroutineManager.Update();

		Assert.AreEqual(Vector2.Zero, boss.NPC.velocity);
	}

	[TestMethod]
	public void TrySummon_OnMultiplayerClient_RejectsLocalNPCSpawn()
	{
		Main.netMode = Terraria.ID.NetmodeID.MultiplayerClient;
		Assert.IsFalse(new SquamousShellSeal().TrySummon(100, 200, 0));
	}

	[TestMethod]
	public void SummonPacket_PreservesClickedTileAndServerAcknowledgement()
	{
		var request = new SquamousShellSummonPacket { TileX = 123, TileY = 20000, Spawned = true };
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		request.Send(writer);
		CollectionAssert.AreEqual(new byte[] { 123, 0, 0, 0, 32, 78, 0, 0, 1 }, stream.ToArray());
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		var received = new SquamousShellSummonPacket();
		received.Receive(reader, 0);
		Assert.AreEqual(123, received.TileX);
		Assert.AreEqual(20000, received.TileY);
		Assert.IsTrue(received.Spawned);
	}

	[TestMethod]
	[DataRow(typeof(SquamousRollingStone))]
	[DataRow(typeof(SquamousRollingStone_fly))]
	public void RollingStoneExtraAI_TransfersGrowingHitbox(Type projectileType)
	{
		var template = (Terraria.ModLoader.ModProjectile)Activator.CreateInstance(projectileType)!;
		var server = template.NewInstance(new Projectile { scale = 0.35f, width = 35, height = 35 });
		var client = template.NewInstance(new Projectile { scale = 1, width = 100, height = 100 });
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		server.SendExtraAI(writer);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		client.ReceiveExtraAI(reader);

		Assert.AreEqual(0.35f, client.Projectile.scale);
		Assert.AreEqual(35, client.Projectile.width);
		Assert.AreEqual(35, client.Projectile.height);
	}

	[TestMethod]
	public void AirProjectile_ReceivedWithoutOnSpawn_UsesSyncedTarget()
	{
		Player originalPlayer = Main.player[0];
		try
		{
			Main.player[0] = new Player { active = true, position = new Vector2(500, 500) };
			var projectile = new Projectile { active = true, timeLeft = 3599, width = 20, height = 20 };
			projectile.ai[0] = 0;
			var air = (SquamousAirProj)new SquamousAirProj().NewInstance(projectile);
			air.AI();
			Assert.IsTrue(projectile.active);
			Assert.IsTrue(projectile.velocity.X > 0);
		}
		finally
		{
			Main.player[0] = originalPlayer;
		}
	}

	private static Skeleton2D LoadBossSkeleton()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Everglow.sln")))
		{
			directory = directory.Parent;
		}
		Assert.IsNotNull(directory, "The repository containing the boss skeleton fixture was not found.");
		string skeletonDirectory = Path.Combine(directory.FullName, "Sources", "Modules", "Yggdrasil", "YggdrasilTown", "NPCs", "SquamousShell", "Skeletons");
		return Skeleton2DReader.ReadSkeleton(File.ReadAllBytes(Path.Combine(skeletonDirectory, "monstera.atlas")), File.ReadAllBytes(Path.Combine(skeletonDirectory, "monsterj.json")));
	}
}
