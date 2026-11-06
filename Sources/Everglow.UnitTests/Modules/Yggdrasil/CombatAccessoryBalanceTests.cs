using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class CombatAccessoryBalanceTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	[DataRow(0f)]
	[DataRow(0.2f)]
	[DataRow(0.5f)]
	public void MoltenCorePreservesExistingReductionAndReducesRemainingDamageByFivePercent(float existingReduction)
	{
		var player = new Player { endurance = existingReduction };
		player.buffType[0] = BuffID.OnFire;
		player.buffTime[0] = 60;
		new MoltenCore().UpdateAccessory(player, false);
		Assert.AreEqual((1f - existingReduction) * 0.95f, 1f - player.endurance, 0.00001f);
	}

	[TestMethod]
	public void MoltenCoreDoesNotApplyItsBonusesWithoutAFlameDebuff()
	{
		var player = new Player { endurance = 0.2f, moveSpeed = 1f };
		new MoltenCore().UpdateAccessory(player, false);
		Assert.AreEqual(0.2f, player.endurance);
		Assert.AreEqual(1f, player.moveSpeed);
	}
}
