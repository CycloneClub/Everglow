using System.Reflection;
using Everglow.Commons;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class HandheldProjectileTest
{
	private Player[] originalPlayers = null!;
	private int originalMyPlayer;
	private Vector2 originalScreenPosition;
	private int originalMouseX;
	private int originalMouseY;
	private EverglowPlayer? originalModPlayer;
	private Player owner = null!;
	private EverglowPlayer modPlayer = null!;
	private TestProjectile held = null!;

	private static readonly PropertyInfo InstanceProperty = typeof(ContentInstance<EverglowPlayer>).GetProperty("Instance")!;
	private static readonly FieldInfo MouseWorldField = typeof(EverglowPlayer).GetField("mouseWorld", BindingFlags.Instance | BindingFlags.NonPublic)!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalPlayers = Main.player;
		originalMyPlayer = Main.myPlayer;
		originalScreenPosition = Main.screenPosition;
		originalMouseX = Main.mouseX;
		originalMouseY = Main.mouseY;
		originalModPlayer = ContentInstance<EverglowPlayer>.Instance;

		owner = new Player { whoAmI = 1, active = true, controlUseItem = true, itemTimeMax = 30 };
		owner.Center = new Vector2(100, 100);
		Main.player = new Player[Main.maxPlayers + 1];
		Main.player[0] = new Player { whoAmI = 0 };
		Main.player[1] = owner;
		Main.myPlayer = 0;
		Main.screenPosition = Vector2.Zero;
		Main.mouseX = 300;
		Main.mouseY = 100;

		modPlayer = (EverglowPlayer)new EverglowPlayer().NewInstance(owner);
		InstanceProperty.SetValue(null, modPlayer);
		typeof(Player).GetField("modPlayers", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(owner, new ModPlayer[] { modPlayer });
		held = (TestProjectile)new TestProjectile().NewInstance(new Projectile { owner = 1, active = true });
		held.SetDefaults();
		held.Projectile.rotation = MathHelper.PiOver4;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.player = originalPlayers;
		Main.myPlayer = originalMyPlayer;
		Main.screenPosition = originalScreenPosition;
		Main.mouseX = originalMouseX;
		Main.mouseY = originalMouseY;
		InstanceProperty.SetValue(null, originalModPlayer);
	}

	[TestMethod]
	public void RemoteOwner_AimsAtSynchronizedMouseInsteadOfLocalMouse()
	{
		MouseWorldField.SetValue(modPlayer, new Vector2(96, 298));
		held.HeldProjectileAI();
		Assert.AreEqual(96f, held.Projectile.Center.X, 0.001f);
		Assert.AreEqual(148f, held.Projectile.Center.Y, 0.001f);
	}

	[TestMethod]
	public void InvertedGravity_MirrorsArmRootOffset()
	{
		owner.gravDir = -1;
		MouseWorldField.SetValue(modPlayer, new Vector2(296, 102));
		held.HeldProjectileAI();
		Assert.AreEqual(new Vector2(96, 102), held.ArmRootPos);
	}

	[TestMethod]
	public void AimAtArmRoot_DoesNotProduceNonFinitePosition()
	{
		Main.myPlayer = 1;
		Main.mouseX = 96;
		Main.mouseY = 98;
		MouseWorldField.SetValue(modPlayer, new Vector2(96, 98));
		held.HeldProjectileAI();
		Assert.IsTrue(float.IsFinite(held.Projectile.rotation));
		Assert.IsTrue(float.IsFinite(held.Projectile.Center.X));
		Assert.IsTrue(float.IsFinite(held.Projectile.Center.Y));
	}

	[TestMethod]
	public void SpawnDirection_UsesNetworkedVelocityInsteadOfObserverMouse()
	{
		held.Projectile.velocity = new Vector2(0, -12);
		held.OnSpawn(new Terraria.DataStructures.EntitySource_Misc("Test"));
		Assert.AreEqual(-MathHelper.PiOver4, held.Projectile.rotation, 0.001f);
	}

	[TestMethod]
	public void LocalOwner_RequestsMouseSynchronizationBeforeDerivedAI()
	{
		Main.myPlayer = 1;
		Assert.IsTrue(held.PreAI());
		var listenField = typeof(EverglowPlayer).GetField("listenMouseWorld", BindingFlags.Instance | BindingFlags.NonPublic)!;
		Assert.IsTrue((bool)listenField.GetValue(modPlayer)!);
	}

	[TestMethod]
	public void OppositeAim_TurnsToTheOtherSide()
	{
		MouseWorldField.SetValue(modPlayer, new Vector2(-104, 98));
		held.HeldProjectileAI();
		Assert.AreEqual(46f, held.Projectile.Center.X, 0.001f);
		Assert.AreEqual(98f, held.Projectile.Center.Y, 0.001f);
		Assert.AreEqual(-1, owner.direction);
	}

	[TestMethod]
	public void RemoteOwnerGravity_IsIndependentOfObserverGravity()
	{
		Main.player[0].gravDir = -1;
		MouseWorldField.SetValue(modPlayer, new Vector2(96, 298));
		held.HeldProjectileAI();
		Assert.AreEqual(148f, held.Projectile.Center.Y, 0.001f);
	}

	[TestMethod]
	public void InvertedGravity_MirrorsDrawOffset()
	{
		owner.gravDir = -1;
		held.DrawOffset = new Vector2(-6, -8);
		Assert.AreEqual(new Vector2(-6, 8), held.DrawOffsetInWorld);
	}

	[TestMethod]
	public void CrystalBeforeParentSync_WaitsWithoutDereferencingMissingParent()
	{
		var crystal = (YggdrasilAmberLaser_crystal)new YggdrasilAmberLaser_crystal().NewInstance(new Projectile { owner = 1, timeLeft = 900 });
		crystal.AI();
		Assert.IsFalse(crystal.EndAI);
	}

	[TestMethod]
	public void ReceivedProjectile_InitializesDirectionWithoutOnSpawn()
	{
		held.Projectile.velocity = new Vector2(0, -12);
		held.PreAI();
		Assert.AreEqual(-MathHelper.PiOver4, held.Projectile.rotation, 0.001f);
	}

	[TestMethod]
	public void ReceivedCrystal_InitializesTrackWithoutOnSpawn()
	{
		var projectile = new Projectile { owner = 1, timeLeft = 900, Center = new Vector2(120, 140) };
		var crystal = (YggdrasilAmberLaser_crystal)new YggdrasilAmberLaser_crystal().NewInstance(projectile);
		crystal.AI();
		Assert.AreEqual(new Vector2(120, 140), crystal.CrystalTrack[0]);
	}

	private sealed class TestProjectile : HandheldProjectile
	{
		public Vector2 DrawOffsetInWorld => GravityDrawOffset;
	}
}
