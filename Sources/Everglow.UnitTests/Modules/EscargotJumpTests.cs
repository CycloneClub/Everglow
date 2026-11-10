using System.Reflection;
using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Food.Buffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Terraria;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class EscargotJumpTests
{
	private CooldownTestContext context = null!;
	private StatBonusPlayer bonuses = null!;
	private FoodBuffModPlayer foodBuffs = null!;
	private float originalJumpSpeed;
	private int originalJumpHeight;
	private float originalCriticalDamage;
	private float originalAddCritDamage;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalJumpSpeed = Player.jumpSpeed;
		originalJumpHeight = Player.jumpHeight;
		originalCriticalDamage = FoodBuffModPlayer.CriticalDamage;
		originalAddCritDamage = FoodBuffModPlayer.AddCritDamage;
		context = new CooldownTestContext();
		bonuses = context.Attach<StatBonusPlayer>();
		foodBuffs = context.Attach<FoodBuffModPlayer>();
		Player.jumpSpeed = 5.01f;
		Player.jumpHeight = 15;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Player.jumpSpeed = originalJumpSpeed;
		Player.jumpHeight = originalJumpHeight;
		FoodBuffModPlayer.CriticalDamage = originalCriticalDamage;
		FoodBuffModPlayer.AddCritDamage = originalAddCritDamage;
		context.Dispose();
	}

	[TestMethod]
	public void EscargotScalesFinalNativeJumpSpeedInsteadOfItsAdditiveBonus()
	{
		(float Speed, int Height, float Boost) Measure(float bonus, bool balloon, bool insignia, bool escargot)
		{
			foodBuffs.ResetEffects();
			bonuses.ResetEffects();
			Player.jumpSpeed = 5.01f;
			Player.jumpHeight = 15;
			context.Player.jumpSpeedBoost = bonus;
			context.Player.jumpBoost = balloon;
			context.Player.empressBrooch = insignia;
			if (escargot)
			{
				int index = 0;
				new EscargotBuff().Update(context.Player, ref index);
				bonuses.PostUpdateEquips();
				Assert.AreEqual(bonus, context.Player.jumpSpeedBoost, "Ordinary jump additions must not be scaled.");
			}

			typeof(Player).GetMethod("UpdateJumpHeight", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
				.Invoke(context.Player, null);
			if (escargot)
			{
				foodBuffs.PostUpdateRunSpeeds();
			}

			return (Player.jumpSpeed, Player.jumpHeight, context.Player.jumpSpeedBoost);
		}

		float? ratio = null;
		foreach ((float Bonus, bool Balloon, bool Insignia) row in new[] { (0f, false, false), (1.6f, false, false), (-1f, false, false), (1.6f, true, false), (1.6f, true, true) })
		{
			var with = Measure(row.Bonus, row.Balloon, row.Insignia, true);
			var without = Measure(row.Bonus, row.Balloon, row.Insignia, false);
			Assert.IsTrue(with.Speed < without.Speed);
			Assert.AreEqual(without.Height, with.Height);
			Assert.AreEqual(without.Boost, with.Boost, 0.00001f);
			float scaled = with.Speed / without.Speed;
			ratio ??= scaled;
			Assert.AreEqual(ratio.Value, scaled, 0.0001f);
		}
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void EscargotJumpPenaltyClearsWithBuffEffects(bool death)
	{
		int index = 0;
		new EscargotBuff().Update(context.Player, ref index);
		if (death)
		{
			foodBuffs.UpdateDead();
		}
		else
		{
			foodBuffs.ResetEffects();
		}
		Player.jumpSpeed = 8f;
		foodBuffs.PostUpdateRunSpeeds();
		Assert.AreEqual(8f, Player.jumpSpeed);
	}

	[TestMethod]
	public void EscargotDoesNotChangeMountedPlayerJumpSpeed()
	{
		int index = 0;
		new EscargotBuff().Update(context.Player, ref index);
		typeof(Mount).GetField("_active", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.SetValue(context.Player.mount, true);
		Player.jumpSpeed = 8f;
		foodBuffs.PostUpdateRunSpeeds();
		Assert.AreEqual(8f, Player.jumpSpeed);
	}

	[TestMethod]
	public void RepeatedBuffUpdatesDoNotMultiplyTheJumpPenalty()
	{
		float Scaled(int updates)
		{
			foodBuffs.ResetEffects();
			int index = 0;
			var buff = new EscargotBuff();
			for (int i = 0; i < updates; i++)
			{
				buff.Update(context.Player, ref index);
			}

			Player.jumpSpeed = 8f;
			foodBuffs.PostUpdateRunSpeeds();
			return Player.jumpSpeed;
		}

		float once = Scaled(1);
		Assert.AreEqual(once, Scaled(2), 0.00001f);
		Assert.IsTrue(once < 8f);
	}

	[TestMethod]
	public void EscargotScalesCombinedBunnyStewJumpInEitherBuffOrder()
	{
		(float Boost, float Speed, int Height) Measure(bool reverse, bool escargot)
		{
			foodBuffs.ResetEffects();
			bonuses.ResetEffects();
			Player.jumpSpeed = 5.01f;
			Player.jumpHeight = 15;
			context.Player.jumpSpeedBoost = 2f;
			int index = 0;
			if (!escargot || !reverse)
			{
				new BunnyStewBuff().Update(context.Player, ref index);
			}

			if (escargot)
			{
				new EscargotBuff().Update(context.Player, ref index);
			}

			if (escargot && reverse)
			{
				new BunnyStewBuff().Update(context.Player, ref index);
			}

			bonuses.PostUpdateEquips();
			typeof(Player).GetMethod("UpdateJumpHeight", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
				.Invoke(context.Player, null);
			foodBuffs.PostUpdateRunSpeeds();
			return (context.Player.jumpSpeedBoost, Player.jumpSpeed, Player.jumpHeight);
		}

		var forward = Measure(false, true);
		var backward = Measure(true, true);
		var stewOnly = Measure(false, false);
		Assert.AreEqual(forward.Boost, backward.Boost, 0.00001f);
		Assert.AreEqual(forward.Speed, backward.Speed, 0.00001f);
		Assert.AreEqual(stewOnly.Boost, forward.Boost, 0.00001f);
		Assert.AreEqual(stewOnly.Height, forward.Height);
		Assert.IsTrue(forward.Speed < stewOnly.Speed);
	}
}
