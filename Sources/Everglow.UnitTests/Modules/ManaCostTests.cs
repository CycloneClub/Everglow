using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Food.Buffs.VanillaDrinkBuffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.KelpCurtain.Items.Armors.Ruin;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.LightSeeker;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class ManaCostTests
{
	private CooldownTestContext context = null!;
	private TropicalSmoothieBuff.TropicalSmoothiePlayer smoothie = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
		smoothie = context.Attach<TropicalSmoothieBuff.TropicalSmoothiePlayer>();
	}

	[TestCleanup]
	public void Cleanup() => context.Dispose();

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void OrdinaryReductionsAddToExistingReductionInEitherOrder(bool reverse)
	{
		context.Player.manaCost = 0.8f;
		int index = 0;
		var steak = new SteakBuff();
		var robe = new RuinMagicRobe();
		if (reverse)
		{
			robe.UpdateEquip(context.Player);
			steak.Update(context.Player, ref index);
		}
		else
		{
			steak.Update(context.Player, ref index);
			robe.UpdateEquip(context.Player);
		}
		Assert.AreEqual(0.5f, context.Player.manaCost, 0.000001f);
	}

	private sealed class TestMod : Mod
	{
		public override string Name => "Everglow";
	}

	[TestMethod]
	[DataRow(false, 0.65f)]
	[DataRow(true, 0.68f)]
	public void ArmorSetReductionsAreAdditive(bool ovine, float expected)
	{
		var oldLanguage = LanguageManager.Instance;
		bool oldMenu = Main.gameMenu;
		try
		{
			LanguageManager.Instance = (LanguageManager)Activator.CreateInstance(typeof(LanguageManager), true)!;
			Main.gameMenu = true; // Lighting.GetColor returns white without loading a graphics engine.
			ModItem armor = ovine ? new OvineHairpin() : new EldamberHairpin();
			typeof(ModType).GetProperty("Mod")!.SetValue(armor, RuntimeHelpers.GetUninitializedObject(typeof(TestMod)));
			context.Player.manaCost = 0.8f;
			armor.UpdateArmorSet(context.Player);
			Assert.AreEqual(expected, context.Player.manaCost, 0.000001f);
		}
		finally
		{
			Main.gameMenu = oldMenu;
			LanguageManager.Instance = oldLanguage;
		}
	}

	[TestMethod]
	[DataRow(1f, 0.1f, false)]
	[DataRow(1f, 0.1f, true)]
	[DataRow(0.25f, 0.05f, false)]
	[DataRow(0.25f, 0.05f, true)]
	[DataRow(0.05f, -0.15f, false)]
	[DataRow(0.05f, -0.15f, true)]
	public void SmoothieCapsAfterAdditiveReductionsWithoutRaisingLowerCosts(float initial, float expected, bool reverse)
	{
		context.Player.manaCost = initial;
		int index = 0;
		ModBuff[] buffs = [new TropicalSmoothieBuff(), new SteakBuff()];
		foreach (var buff in reverse ? buffs.Reverse() : buffs)
		{
			buff.Update(context.Player, ref index);
		}
		Assert.AreEqual(initial - 0.2f, context.Player.manaCost, 0.000001f);
		smoothie.PostUpdateEquips();
		Assert.AreEqual(expected, context.Player.manaCost, 0.000001f);
		if (expected <= 0)
		{
			Assert.AreEqual(0, context.Player.GetManaCost(new Item { mana = 20 }));
		}
	}

	[TestMethod]
	public void SmoothieClearsOnReset()
	{
		int index = 0;
		new TropicalSmoothieBuff().Update(context.Player, ref index);
		smoothie.ResetEffects();
		context.Player.manaCost = 0.7f;
		smoothie.PostUpdateEquips();
		Assert.AreEqual(0.7f, context.Player.manaCost);
	}

	[TestMethod]
	public void SmoothieDoesNotShareActiveStateWithCloneAndSkipsUnloadedPlayer()
	{
		int index = 0;
		new TropicalSmoothieBuff().Update(context.Player, ref index);
		var other = new Player { active = true, manaCost = 0.7f };
		var clone = (TropicalSmoothieBuff.TropicalSmoothiePlayer)smoothie.Clone(other);
		clone.PostUpdateEquips();
		Assert.AreEqual(0.7f, other.manaCost);
		context.Player.manaCost = 0.7f;
		context.Player.outOfRange = true;
		smoothie.PostUpdateEquips();
		context.Player.outOfRange = false;
		smoothie.PostUpdateEquips();
		Assert.AreEqual(0.7f, context.Player.manaCost);
	}
}
