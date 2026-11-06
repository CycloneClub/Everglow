using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Food.Buffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class NativeWingBonusTests
{
	private CooldownTestContext context = null!;
	private FoodBuffModPlayer food = null!;
	private float previousCriticalDamage;
	private float previousAddCritDamage;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		previousCriticalDamage = FoodBuffModPlayer.CriticalDamage;
		previousAddCritDamage = FoodBuffModPlayer.AddCritDamage;
		context = new CooldownTestContext();
		context.Attach<StatBonusPlayer>();
		food = context.Attach<FoodBuffModPlayer>();
	}

	[TestCleanup]
	public void Cleanup()
	{
		FoodBuffModPlayer.CriticalDamage = previousCriticalDamage;
		FoodBuffModPlayer.AddCritDamage = previousAddCritDamage;
		context.Dispose();
	}

	[TestMethod]
	[DataRow(6.25f)]
	[DataRow(8f)]
	[DataRow(16f)]
	public void RoastedBirdScalesTheNativeWingOrHoverSpeedWithoutChangingAcceleration(float nativeSpeed)
	{
		Apply(new RoastedBirdBuff());
		float speed = nativeSpeed;
		float acceleration = 0.32f;
		new FoodBuffGlobalItem().HorizontalWingSpeeds(new Item(), context.Player, ref speed, ref acceleration);
		Assert.AreEqual(nativeSpeed * 1.15f, speed, 0.00001f);
		Assert.AreEqual(0.32f, acceleration);
	}

	[TestMethod]
	[DataRow(0, 1, 2)]
	[DataRow(0, 2, 1)]
	[DataRow(1, 0, 2)]
	[DataRow(1, 2, 0)]
	[DataRow(2, 0, 1)]
	[DataRow(2, 1, 0)]
	public void FoodFlightTimeUsesTheEquippedWingBaselineInEveryBuffOrder(int first, int second, int third)
	{
		ModBuff[] buffs = [new RoastedBirdBuff(), new RoastedDuckBuff(), new EscargotBuff()];
		Apply(buffs[first], buffs[second], buffs[third]);
		context.Player.wingTimeMax = 200; // The equipped wing establishes its base after Buff.Update.
		context.Player.wingTime = 150;
		food.PostUpdateEquips();
		Assert.AreEqual(90, context.Player.wingTimeMax);
		Assert.AreEqual(150f, context.Player.wingTime);
	}

	[TestMethod]
	[DataRow(false, 150f)]
	[DataRow(true, 150f)]
	[DataRow(false, 260f)]
	[DataRow(true, 260f)]
	public void EscargotPreservesRemainingWingTimeIncludingExtraRocketTime(bool insignia, float remainingTime)
	{
		context.Player.empressBrooch = insignia;
		context.Player.wingTime = remainingTime;
		for (int frame = 0; frame < 3; frame++)
		{
			food.ResetEffects();
			context.Player.wingTimeMax = 200;
			float beforeBuff = context.Player.wingTime;
			Apply(new EscargotBuff());
			Assert.AreEqual(beforeBuff, context.Player.wingTime, "Buff collection must not repeatedly divide the resource.");
			food.PostUpdateEquips();
			Assert.AreEqual(20, context.Player.wingTimeMax);
			Assert.AreEqual(beforeBuff, context.Player.wingTime, "Capacity bonuses must preserve remaining time, including extra rocket time.");
			context.Player.wingTime -= 1;
			if (insignia)
			{
				context.Player.wingTime = context.Player.wingTimeMax; // Native Soaring Insignia refill.
			}
		}
	}

	[TestMethod]
	public void FlightTimeBonusesDoNotRefillSpentTimeOrGrantFlightWithoutWings()
	{
		Apply(new RoastedBirdBuff(), new RoastedDuckBuff());
		context.Player.wingTimeMax = 200;
		context.Player.wingTime = 7;
		food.PostUpdateEquips();
		Assert.AreEqual(270, context.Player.wingTimeMax);
		Assert.AreEqual(7f, context.Player.wingTime);
		food.ResetEffects();
		Apply(new RoastedBirdBuff(), new RoastedDuckBuff(), new EscargotBuff());
		context.Player.wingTimeMax = 0;
		context.Player.wingTime = 0;
		food.PostUpdateEquips();
		Assert.AreEqual(0, context.Player.wingTimeMax);
		Assert.AreEqual(0f, context.Player.wingTime);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void WingEffectsClearOnResetAndDeath(bool death)
	{
		Apply(new RoastedBirdBuff(), new RoastedDuckBuff(), new EscargotBuff());
		if (death)
		{
			food.UpdateDead();
		}
		else
		{
			food.ResetEffects();
		}
		context.Player.wingTimeMax = 200;
		context.Player.wingTime = 150;
		food.PostUpdateEquips();
		float speed = 8f;
		float acceleration = 0.16f;
		new FoodBuffGlobalItem().HorizontalWingSpeeds(new Item(), context.Player, ref speed, ref acceleration);
		Assert.AreEqual(200, context.Player.wingTimeMax);
		Assert.AreEqual(150f, context.Player.wingTime);
		Assert.AreEqual(8f, speed);
		Assert.AreEqual(0.16f, acceleration);
	}

	private void Apply(params ModBuff[] buffs)
	{
		int index = 0;
		foreach (var buff in buffs)
		{
			buff.Update(context.Player, ref index);
		}
	}
}
