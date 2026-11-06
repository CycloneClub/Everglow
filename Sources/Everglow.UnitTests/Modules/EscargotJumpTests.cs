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
	[DataRow(0f, false, false, 1.2525f)]
	[DataRow(1.6f, false, false, 1.6525f)]
	[DataRow(-1f, false, false, 1.0025f)]
	[DataRow(1.6f, true, false, 2.0275f)]
	[DataRow(1.6f, true, true, 2.4775f)]
	public void EscargotScalesFinalNativeJumpSpeedInsteadOfItsAdditiveBonus(float bonus, bool balloon, bool insignia, float expected)
	{
		context.Player.jumpSpeedBoost = bonus;
		context.Player.jumpBoost = balloon;
		context.Player.empressBrooch = insignia;
		int index = 0;
		new EscargotBuff().Update(context.Player, ref index);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(bonus, context.Player.jumpSpeedBoost, "Ordinary jump additions must not be scaled.");

		typeof(Player).GetMethod("UpdateJumpHeight", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.Invoke(context.Player, null);
		foodBuffs.PostUpdateRunSpeeds();
		Assert.AreEqual(expected, Player.jumpSpeed, 0.00001f);
		Assert.AreEqual(balloon ? 20 : 15, Player.jumpHeight);
		Assert.AreEqual(bonus + (insignia ? 1.8f : 0), context.Player.jumpSpeedBoost, 0.00001f);
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
		int index = 0;
		var buff = new EscargotBuff();
		buff.Update(context.Player, ref index);
		buff.Update(context.Player, ref index);
		Player.jumpSpeed = 8f;
		foodBuffs.PostUpdateRunSpeeds();
		Assert.AreEqual(2f, Player.jumpSpeed);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void EscargotScalesCombinedBunnyStewJumpInEitherBuffOrder(bool reverse)
	{
		context.Player.jumpSpeedBoost = 2f;
		int index = 0;
		if (reverse)
		{
			new EscargotBuff().Update(context.Player, ref index);
			new BunnyStewBuff().Update(context.Player, ref index);
		}
		else
		{
			new BunnyStewBuff().Update(context.Player, ref index);
			new EscargotBuff().Update(context.Player, ref index);
		}
		bonuses.PostUpdateEquips();
		Assert.AreEqual(3.5f, context.Player.jumpSpeedBoost);
		typeof(Player).GetMethod("UpdateJumpHeight", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.Invoke(context.Player, null);
		foodBuffs.PostUpdateRunSpeeds();
		Assert.AreEqual(2.5025f, Player.jumpSpeed, 0.00001f);
		Assert.AreEqual(20, Player.jumpHeight);
	}
}
