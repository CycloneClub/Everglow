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
	public void OnSpawn_OnDedicatedServer_StartsLandingWithoutLoadingSkeleton()
	{
		var boss = (Boss)new Boss().NewInstance(new NPC());

		boss.OnSpawn(new Terraria.DataStructures.EntitySource_Misc("Test"));
		boss._coroutineManager.Update();

		Assert.AreEqual(2.2f, boss.NPC.velocity.Y, 0.001f);
		Assert.IsNull(boss.SquamousShellSkeleton);
		Assert.IsNull(boss.skeletonRenderer);
		Assert.IsNull(boss.skeletonDebugRenderer);
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
		var server = CreateBossDuringTakeoff();
		server.Flying = true;
		server.SmashValue = 0.75f;
		server.NPC.rotation = 1.25f;
		var client = (Boss)new Boss().NewInstance(new NPC());
		client.SquamousShellSkeleton = LoadBossSkeleton();
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		server.SendExtraAI(writer);
		Assert.IsNull(server.SquamousShellSkeleton);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		Main.dedServ = false;
		Main.netMode = Terraria.ID.NetmodeID.MultiplayerClient;
		client.ReceiveExtraAI(reader);

		Assert.IsTrue(client.Flying);
		Assert.AreEqual(0.75f, client.SmashValue);
		Assert.AreEqual(1.25f, client.NPC.rotation);
		Assert.AreEqual("takeOff", client.SquamousShellSkeleton.AnimationState.GetCurrent(0).Animation.Name);
		Assert.AreEqual(1f / 3f, client.SquamousShellSkeleton.AnimationState.GetCurrent(0).TrackTime, 0.001f);
		Assert.IsFalse(client.SquamousShellSkeleton.AnimationState.GetCurrent(0).Loop);
	}

	[TestMethod]
	public void ExtraAI_BeforeClientSkeletonIsLoaded_PreservesPendingAnimation()
	{
		var server = CreateBossDuringTakeoff();
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		server.SendExtraAI(writer);
		byte[] expected = stream.ToArray();
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		var client = (Boss)new Boss().NewInstance(new NPC());
		Main.dedServ = false;
		Main.netMode = Terraria.ID.NetmodeID.MultiplayerClient;
		client.ReceiveExtraAI(reader);
		Assert.IsNull(client.SquamousShellSkeleton);

		using var copy = new MemoryStream();
		using var copyWriter = new BinaryWriter(copy);
		client.SendExtraAI(copyWriter);
		CollectionAssert.AreEqual(expected, copy.ToArray());
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
	[DataRow(false, 0)]
	[DataRow(true, 1)]
	public void SummonPacket_PreservesClickedTileAndServerAcknowledgement(bool spawned, int expectedFlag)
	{
		var request = new SquamousShellSummonPacket { TileX = 123, TileY = 20000, Spawned = spawned };
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		request.Send(writer);
		CollectionAssert.AreEqual(new byte[] { 123, 0, 0, 0, 32, 78, 0, 0, (byte)expectedFlag }, stream.ToArray());
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		var received = new SquamousShellSummonPacket();
		received.Receive(reader, 0);
		Assert.AreEqual(123, received.TileX);
		Assert.AreEqual(20000, received.TileY);
		Assert.AreEqual(spawned, received.Spawned);
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

	[TestMethod]
	public void Slash_ReceivedWithoutOnSpawn_ReconstructsTrajectoryAndCollisionWithoutDrawing()
	{
		var server = (Squamous_Slash)new Squamous_Slash().NewInstance(new Projectile { timeLeft = 114, damage = 40 });
		server.SpacePos = new Vector3(100, 20, 30);
		server.RotatedAxis = new Vector3(1, -8, 2);
		server.Omega = 0.7f;
		for (int i = 0; i < 8; i++)
		{
			server.AI();
		}
		var client = (Squamous_Slash)new Squamous_Slash().NewInstance(new Projectile { timeLeft = 120, damage = 40 });
		TransferExtraAI(server, client);
		Assert.AreEqual(server.SpacePos, client.SpacePos);
		Assert.AreEqual(server.RotatedAxis, client.RotatedAxis);
		Assert.AreEqual(server.Omega, client.Omega);
		Assert.AreEqual(server.Projectile.timeLeft, client.Projectile.timeLeft);
		CollectionAssert.AreEqual(server.SmoothTrail, client.SmoothTrail);
		server.AI();
		client.AI();
		Assert.AreEqual(server.SpacePos, client.SpacePos);
		Assert.IsTrue(server.SmoothTrail.Count > 4);
		Vector2 hitPoint = server.Projectile.Center + server.SmoothTrail[0];
		Assert.AreEqual(true, server.Colliding(default, new Rectangle((int)hitPoint.X - 5, (int)hitPoint.Y - 5, 10, 10)));
	}

	[TestMethod]
	[DataRow(typeof(SquamousMoonBladePredict))]
	[DataRow(typeof(SquamousRockSpike_Falling_Predict))]
	public void PredictionExtraAI_PreservesHeadingAndLifetime(Type type)
	{
		var template = (Terraria.ModLoader.ModProjectile)Activator.CreateInstance(type)!;
		var server = template.NewInstance(new Projectile { rotation = 1.4f, timeLeft = 137 });
		var client = template.NewInstance(new Projectile { timeLeft = 95 });
		TransferExtraAI(server, client);
		Assert.AreEqual(1.4f, client.Projectile.rotation);
		Assert.AreEqual(137, client.Projectile.timeLeft);
	}

	[TestMethod]
	public void MoonBlade_ReceivedWithoutOnSpawn_InitializesDirectionWithoutGraphics()
	{
		var blade = (YggdrasilMoonBlade)new YggdrasilMoonBlade().NewInstance(new Projectile { velocity = new Vector2(3, 4), timeLeft = 300 });
		blade.AI();
		Assert.AreEqual(new Vector2(0.6f, 0.8f), blade.startVelocity);
	}

	[TestMethod]
	public void Shade_ReceivedWithoutOnSpawn_DerivesHeadingFromVelocity()
	{
		var shade = (SquamousShell_shade)new SquamousShell_shade().NewInstance(new Projectile { velocity = new Vector2(0, -4) });
		shade.AI();
		Assert.AreEqual(-MathHelper.PiOver2, shade.Projectile.rotation, 0.001f);
	}

	[TestMethod]
	public void RockSpike_ReceivedWithoutOnSpawn_UsesSyncedTarget()
	{
		Player originalPlayer = Main.player[0];
		try
		{
			Main.player[0] = new Player { active = true, position = new Vector2(500, 500) };
			var projectile = new Projectile { active = true, timeLeft = 3599, velocity = Vector2.UnitY };
			projectile.ai[0] = 0;
			var spike = (SquamousRockSpike)new SquamousRockSpike().NewInstance(projectile);
			spike.AI();
			Assert.IsTrue(projectile.active);
			Assert.IsTrue(projectile.velocity.X > 0);
		}
		finally
		{
			Main.player[0] = originalPlayer;
		}
	}

	[TestMethod]
	public void EffectPacket_PreservesEventAndWorldPosition()
	{
		foreach (var effect in Enum.GetValues<SquamousShellEffect>())
		{
			var sent = new SquamousShellEffectPacket { NPCIndex = 47, Effect = effect, Position = new Vector2(1234, 234567), Scale = 1.25f };
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
			sent.Send(writer);
			stream.Position = 0;
			using var reader = new BinaryReader(stream);
			var received = new SquamousShellEffectPacket();
			received.Receive(reader, 0);
			Assert.AreEqual(sent.NPCIndex, received.NPCIndex);
			Assert.AreEqual(sent.Effect, received.Effect);
			Assert.AreEqual(sent.Position, received.Position);
			Assert.AreEqual(sent.Scale, received.Scale);
			Assert.AreEqual(stream.Length, stream.Position);
		}
	}

	[TestMethod]
	public void EffectPacket_OnDedicatedServer_IgnoresClientRequests()
	{
		new SquamousShellEffectPacket.Handler().Handle(new SquamousShellEffectPacket { NPCIndex = -1 }, 0);
		var impact = (YggdrasilMoonBladeHit)new YggdrasilMoonBladeHit().NewInstance(new Projectile { timeLeft = 200 });
		impact.AI();
	}

	private static void TransferExtraAI(Terraria.ModLoader.ModProjectile source, Terraria.ModLoader.ModProjectile target)
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		source.SendExtraAI(writer);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		target.ReceiveExtraAI(reader);
	}

	private static Boss CreateBossDuringTakeoff()
	{
		var boss = (Boss)new Boss().NewInstance(new NPC());
		using var attack = boss.JumpAndShake(1);
		for (int tick = 0; tick < 10; tick++)
		{
			Assert.IsTrue(attack.MoveNext());
		}
		return boss;
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
