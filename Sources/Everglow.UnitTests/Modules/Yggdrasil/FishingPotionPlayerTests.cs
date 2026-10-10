using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class FishingPotionPlayerTests
{
	private Player player = null!;
	private FishingPotionPlayer potionPlayer = null!;
	private float originalJumpSpeed;
	private int originalJumpHeight;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		player = new Player();
		potionPlayer = (FishingPotionPlayer)new FishingPotionPlayer().NewInstance(player);
		originalJumpSpeed = Player.jumpSpeed;
		originalJumpHeight = Player.jumpHeight;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Player.jumpSpeed = originalJumpSpeed;
		Player.jumpHeight = originalJumpHeight;
	}

	[TestMethod]
	public void ResetEffects_RemovesBothPotionEffects()
	{
		potionPlayer.JumpPotionActive = true;
		potionPlayer.ScalePotionActive = true;
		potionPlayer.ResetEffects();
		Assert.IsFalse(potionPlayer.JumpPotionActive);
		Assert.IsFalse(potionPlayer.ScalePotionActive);
	}

	[TestMethod]
	public void ScalePotion_ReducesContactDamageOnly()
	{
		potionPlayer.ScalePotionActive = true;
		potionPlayer.JumpPotionActive = true;
		var contact = new Player.HurtModifiers { DamageSource = PlayerDeathReason.ByNPC(0) };
		potionPlayer.ModifyHitByNPC(new NPC(), ref contact);
		potionPlayer.ModifyHurt(ref contact);
		Assert.AreEqual(94f, contact.FinalDamage.ApplyTo(100f), 0.001f);

		var projectileSource = new PlayerDeathReason();
		projectileSource.SourceProjectileType = Terraria.ID.ProjectileID.WoodenArrowFriendly;
		var projectile = new Player.HurtModifiers { DamageSource = projectileSource };
		potionPlayer.ModifyHitByProjectile(new Projectile(), ref projectile);
		potionPlayer.ModifyHurt(ref projectile);
		Assert.AreEqual(100f, projectile.FinalDamage.ApplyTo(100f));
	}

	[TestMethod]
	[DataRow(0)]
	[DataRow(5)]
	public void JumpPotion_ReducesFallDamageWithoutReducingOtherEnvironmentalDamage(int fallReason)
	{
		potionPlayer.JumpPotionActive = true;
		var fall = new Player.HurtModifiers { DamageSource = PlayerDeathReason.ByOther(fallReason) };
		potionPlayer.ModifyHurt(ref fall);
		Assert.AreEqual(70f, fall.FinalDamage.ApplyTo(100f), 0.001f);

		var drowning = new Player.HurtModifiers { DamageSource = PlayerDeathReason.ByOther(1) };
		potionPlayer.ModifyHurt(ref drowning);
		Assert.AreEqual(100f, drowning.FinalDamage.ApplyTo(100f));
	}

	[TestMethod]
	public void InactivePotions_LeaveDamageAndJumpUnchanged()
	{
		Player.jumpSpeed = 5.01f;
		Player.jumpHeight = 15;
		var contact = new Player.HurtModifiers { DamageSource = PlayerDeathReason.ByNPC(0) };
		potionPlayer.ModifyHitByNPC(new NPC(), ref contact);
		potionPlayer.ModifyHurt(ref contact);
		potionPlayer.PostUpdateRunSpeeds();
		Assert.AreEqual(100f, contact.FinalDamage.ApplyTo(100f));
		Assert.AreEqual(5.01f, Player.jumpSpeed);
		Assert.AreEqual(15, Player.jumpHeight);
	}

	[TestMethod]
	[DataRow(5.01f, 15, 0.4f)]
	[DataRow(7.41f, 15, 0.4f)]
	[DataRow(6.51f, 20, 0.4f)]
	public void JumpPotion_IncreasesFullJumpApexByFifteenPercent(float speed, int holdFrames, float gravity)
	{
		Player.jumpSpeed = speed;
		Player.jumpHeight = holdFrames;
		player.gravity = gravity;
		potionPlayer.JumpPotionActive = true;
		float originalApex = SimulateJumpApex(speed, holdFrames, gravity);
		potionPlayer.PostUpdateRunSpeeds();
		float boostedApex = SimulateJumpApex(Player.jumpSpeed, Player.jumpHeight, gravity);
		Assert.AreEqual(1.15f, boostedApex / originalApex, 0.005f);
		Assert.AreEqual(holdFrames, Player.jumpHeight, "The potion must not add extra jumps or alter jump hold time.");
	}

	private static float SimulateJumpApex(float speed, int holdFrames, float gravity)
	{
		float height = 0f;
		float velocity = -speed;
		for (int tick = 0; tick < 1000; tick++)
		{
			if (tick <= holdFrames)
			{
				velocity = -speed;
			}
			velocity += gravity;
			if (velocity >= 0f)
			{
				return height;
			}
			height -= velocity;
		}
		Assert.Fail("Jump did not reach its apex.");
		return height;
	}
}
