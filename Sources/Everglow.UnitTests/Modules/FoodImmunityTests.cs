using System.Reflection;
using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Food.Buffs.ModDrinkBuffs;
using Everglow.Food.Buffs.ModFoodBuffs;
using Everglow.UnitTests.Function;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class FoodImmunityTests
{
	private static readonly MethodInfo UpdateImmunity = typeof(Player).GetMethod("UpdateImmunity", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	[DataRow(0)]
	[DataRow(40)]
	public void BuffUpdatesDoNotCreateOrExtendRemainingImmunity(int remaining)
	{
		using var context = new CooldownTestContext();
		context.Attach<StatBonusPlayer>();
		context.Player.immuneTime = remaining;
		Apply(context.Player, true, true, false);
		Assert.AreEqual(remaining, context.Player.immuneTime);
	}

	[TestMethod]
	[DataRow(20, true, false, 50)]
	[DataRow(40, false, true, 80)]
	[DataRow(40, true, true, 140)]
	[DataRow(80, true, true, 220)]
	[DataRow(8, true, true, 76)]
	public void FoodBonusesExtendNativeHitDurationInEitherBuffOrder(int nativeTime, bool icecream, bool juice, int expected)
	{
		foreach (bool reverse in new[] { false, true })
		{
			using var context = new CooldownTestContext();
			var bonuses = context.Attach<StatBonusPlayer>();
			Apply(context.Player, icecream, juice, reverse);
			// Player.Hurt assigns this before PlayerLoader.PostHurt.
			context.Player.immuneTime = nativeTime;
			bonuses.PostHurt(new Player.HurtInfo { CooldownCounter = ImmunityCooldownID.General });
			Assert.AreEqual(expected, context.Player.immuneTime);
		}
	}

	[TestMethod]
	[DataRow(true, false, 70)]
	[DataRow(false, true, 80)]
	[DataRow(true, true, 140)]
	public void NativeCountdownExpiresEvenWhileFoodRemainsActive(bool icecream, bool juice, int duration)
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<StatBonusPlayer>();
		var player = context.Player;
		Apply(player, icecream, juice, false);
		player.immune = true;
		player.immuneTime = 40;
		bonuses.PostHurt(new Player.HurtInfo { CooldownCounter = ImmunityCooldownID.General });
		Assert.AreEqual(duration, player.immuneTime);
		for (int frame = 0; frame < duration; frame++)
		{
			UpdateImmunity.Invoke(player, null);
			bonuses.ResetEffects();
			Apply(player, icecream, juice, frame % 2 == 0);
			Assert.AreEqual(duration - frame - 1, player.immuneTime);
		}
		Assert.IsFalse(player.immune);
	}

	[TestMethod]
	public void ExpiredFoodPreservesCurrentCountdownButDoesNotExtendNextHit()
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<StatBonusPlayer>();
		Apply(context.Player, true, true, false);
		context.Player.immuneTime = 40;
		bonuses.PostHurt(new Player.HurtInfo { CooldownCounter = ImmunityCooldownID.General });
		bonuses.ResetEffects();
		Assert.AreEqual(140, context.Player.immuneTime);
		context.Player.immuneTime = 40;
		bonuses.PostHurt(new Player.HurtInfo { CooldownCounter = ImmunityCooldownID.General });
		Assert.AreEqual(40, context.Player.immuneTime);
	}

	[TestMethod]
	public void SeparateDamageCooldownDoesNotExtendAnUnrelatedGeneralCountdown()
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<StatBonusPlayer>();
		Apply(context.Player, true, true, false);
		context.Player.immuneTime = 15;
		context.Player.hurtCooldowns[ImmunityCooldownID.Bosses] = 40;
		bonuses.PostHurt(new Player.HurtInfo { CooldownCounter = ImmunityCooldownID.Bosses });
		Assert.AreEqual(15, context.Player.immuneTime);
		Assert.AreEqual(40, context.Player.hurtCooldowns[ImmunityCooldownID.Bosses]);
	}

	private static void Apply(Player player, bool icecream, bool juice, bool reverse)
	{
		int index = 0;
		if (juice && reverse)
		{
			new OrangeJuiceBuff().Update(player, ref index);
		}
		if (icecream)
		{
			new OrangeIcecreamBuff().Update(player, ref index);
		}
		if (juice && !reverse)
		{
			new OrangeJuiceBuff().Update(player, ref index);
		}
	}
}
