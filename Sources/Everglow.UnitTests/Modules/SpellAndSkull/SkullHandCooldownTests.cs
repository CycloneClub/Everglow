using Everglow.Commons;
using Everglow.Commons.Mechanics.Cooldown;
using Everglow.Commons.Utilities;
using Everglow.SpellAndSkull.GlobalItems;
using Everglow.UnitTests.Function;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Modules.SpellAndSkull;

[TestClass]
[DoNotParallelize]
public class SkullHandCooldownTests
{
	private const string CooldownID = "Everglow_SkullHand";
	private CooldownTestContext context = null!;
	private MagicBookPlayer magicPlayer = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
		context.Register(CooldownID, typeof(MagicBookPlayer).Assembly.GetType("Everglow.SpellAndSkull.GlobalItems.SkullHandCooldown", throwOnError: true)!);
		magicPlayer = context.Attach<MagicBookPlayer>();
	}

	[TestCleanup]
	public void Cleanup() => context.Dispose();

	[TestMethod]
	public void HandRemainsUnavailableUntilEighteenthPreUpdateEvenAfterSwitchingItems()
	{
		StartCooldown();
		context.Player.HeldItem.type = ItemID.Wood;
		for (int tick = 0; tick < 17; tick++)
		{
			magicPlayer.PreUpdate();
			context.Cooldowns.PostUpdateMiscEffects();
			Assert.IsTrue(context.Player.HasCooldown(CooldownID));
		}
		Assert.AreEqual(1, context.Cooldowns.cooldowns[CooldownID].timeLeft);
		magicPlayer.PreUpdate();
		Assert.IsFalse(context.Player.HasCooldown(CooldownID));
	}

	[TestMethod]
	public void HandCooldownIsRetainedAcrossDeathButExcludedFromPlayerSave()
	{
		StartCooldown();
		magicPlayer.PreUpdate();
		context.Cooldowns.UpdateDead();
		Assert.AreEqual(17, context.Cooldowns.cooldowns[CooldownID].timeLeft);
		var data = new TagCompound();
		context.Cooldowns.SaveData(data);
		Assert.IsFalse(data.GetCompound(EverglowPlayer.PlayerDataCooldownsKey).ContainsKey(CooldownID));
	}

	private void StartCooldown()
	{
		Assert.IsNotNull(CooldownRegistry.GetNet(CooldownID), "SkullHand must use the shared cooldown system.");
		context.Player.AddCooldown(CooldownID, 18);
	}
}
