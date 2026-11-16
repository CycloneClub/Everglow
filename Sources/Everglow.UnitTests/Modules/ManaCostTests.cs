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

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
	}

	[TestCleanup]
	public void Cleanup() => context.Dispose();

	[TestMethod]
	public void OrdinaryReductionsAddToExistingReductionInEitherOrder()
	{
		float Cost(bool reverse)
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

			return context.Player.manaCost;
		}

		float forward = Cost(false);
		Assert.AreEqual(forward, Cost(true), 0.000001f);
		Assert.IsTrue(forward < 0.8f);
	}

	private sealed class TestMod : Mod
	{
		public override string Name => "Everglow";
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ArmorSetReductionsAreAdditive(bool ovine)
	{
		var oldLanguage = LanguageManager.Instance;
		bool oldMenu = Main.gameMenu;
		try
		{
			LanguageManager.Instance = (LanguageManager)Activator.CreateInstance(typeof(LanguageManager), true)!;
			Main.gameMenu = true; // Lighting.GetColor returns white without loading a graphics engine.
			ModItem armor = ovine ? new OvineHairpin() : new EldamberHairpin();
			typeof(ModType).GetProperty("Mod")!.SetValue(armor, RuntimeHelpers.GetUninitializedObject(typeof(TestMod)));
			float Reduction(float initial)
			{
				context.Player.manaCost = initial;
				armor.UpdateArmorSet(context.Player);
				return initial - context.Player.manaCost;
			}

			float fromPartial = Reduction(0.8f);
			Assert.AreEqual(fromPartial, Reduction(1f), 0.000001f);
			Assert.IsTrue(fromPartial > 0f);
		}
		finally
		{
			Main.gameMenu = oldMenu;
			LanguageManager.Instance = oldLanguage;
		}
	}

	[TestMethod]
	public void SmoothieAndSteakReduceManaCostByAConstantInEitherOrder()
	{
		float Cost(float initial, bool reverse)
		{
			context.Player.manaCost = initial;
			int index = 0;
			ModBuff[] buffs = [new TropicalSmoothieBuff(), new SteakBuff()];
			foreach (var buff in reverse ? buffs.Reverse() : buffs)
			{
				buff.Update(context.Player, ref index);
			}

			return context.Player.manaCost;
		}

		float fromFull = Cost(1f, false);
		Assert.AreEqual(fromFull, Cost(1f, true), 0.000001f);
		float reduction = 1f - fromFull;
		Assert.IsTrue(reduction > 0f);
		Assert.AreEqual(reduction, 0.25f - Cost(0.25f, false), 0.000001f);
		float drained = Cost(0.05f, false);
		if (drained <= 0f)
		{
			Assert.AreEqual(0, context.Player.GetManaCost(new Item { mana = 20 }));
		}
	}
}
