using System.Runtime.CompilerServices;
using System.Reflection;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.YggdrasilTown.Items.SummonItems;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.BloodCultist;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.JellyBall;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Mounts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Items.PermanentBoosters;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.RockElemental;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Rock;
using Everglow.Yggdrasil.YggdrasilTown.Items.BossDrops;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.KingJellyBall;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.SquamousShell;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TownNpcLootTests
{
	private readonly List<Action> restoreContent = [];
	private int nextItemType = 40000;
	private bool originalDownedKing;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalDownedKing = YggdrasilWorldSystem.DownedKingJellyBall;
		BindNpc<JellyBall>(50001);
		BindNpc<GiantJellyBall>(50002);
		BindItem<JellyBallDaifuku>();
		BindItem<BloodDrenchedRobe>();
		BindItem<BloodStainedVeil>();
		BindItem<JellyBallInhibitor>();
		BindItem<MysteriousTablet>();
		BindItem<JellyBallBathingCap>();
		BindItem<CrystalNucleusOfJellyKing>();
		BindItem<JellyBallCube>();
		BindItem<JellyBubbleItem>();
		BindItem<ActivatedJellyGland>();
		BindItem<LampBorerHoney>();
		BindItem<BrittleRockSlingshot>();
		BindItem<HyperockSpear>();
		BindItem<VitalizedRocks>();
		BindItem<DragonScaleHammer>();
		BindItem<BlueyWings>();
		BindItem<DeadBeetleEgg>();
		BindItem<EyeOfAnabiosis>();
		BindItem<FlurryingBlades>();
		BindItem<RockSpikeBallista>();
		BindItem<RockGreaves>();
		BindItem<RockHelmet>();
		BindItem<RockPlateMail>();
		BindItem<SquamousShellRelic>();
		BindItem<SquamousShellTrophy>();
		BindItem<SquamousShellTreasureBag>();
		BindItem<SquamousCore>();
		BindItem<GiantBeetleAntennae>();
	}

	[TestCleanup]
	public void Cleanup()
	{
		YggdrasilWorldSystem.DownedKingJellyBall = originalDownedKing;
		foreach (var restore in restoreContent.AsEnumerable().Reverse())
		{
			restore();
		}
	}

	[TestMethod]
	public void CultistAlwaysDropsBothArmorPieces()
	{
		var rates = Rates<CrimsonSpell>();
		AssertRate<BloodDrenchedRobe>(rates, 1f);
		AssertRate<BloodStainedVeil>(rates, 1f);
	}

	[TestMethod]
	public void JellyKingEquipmentPoolIsGuaranteedAndUniform()
	{
		var rates = Rates<KingJellyBall>();
		AssertRate<JellyBallInhibitor>(rates, 1f / 3);
		AssertRate<MysteriousTablet>(rates, 1f / 3);
		AssertRate<JellyBallBathingCap>(rates, 1f / 3);
		AssertRate<CrystalNucleusOfJellyKing>(rates, 1f, 4, 6);
		AssertRate<JellyBallCube>(rates, 1f, 24, 40);
	}

	[TestMethod]
	public void JellyKingMountHasSeparateNormalAndExpertProbabilities()
	{
		var rates = Rates<KingJellyBall>().Where(x => x.itemId == ModContent.ItemType<JellyBubbleItem>()).ToArray();
		Assert.AreEqual(2, rates.Length);
		Assert.AreEqual(0.25f, rates.Single(x => x.conditions.Any(c => c is Conditions.NotExpert)).dropRate);
		Assert.AreEqual(0.5f, rates.Single(x => x.conditions.Any(c => c is Conditions.IsExpert)).dropRate);
	}

	[TestMethod]
	public void BothJellySizesKeepCubeAndGlandDropsWithoutBossDerivatives()
	{
		foreach (var (type, rates, min, max) in new[]
		{
			(ModContent.NPCType<JellyBall>(), Rates<JellyBall>(), 1, 2),
			(ModContent.NPCType<GiantJellyBall>(), Rates<GiantJellyBall>(), 3, 5),
		})
		{
			AssertRate<JellyBallCube>(rates, 1f, min, max);
			AssertRate<ActivatedJellyGland>(rates, 0.01f);
			var npc = (NPC)RuntimeHelpers.GetUninitializedObject(typeof(NPC));
			npc.ai = new float[4];
			npc.type = type;
			foreach (var drop in rates.Where(rate => rate.itemId == ModContent.ItemType<JellyBallCube>() || rate.itemId == ModContent.ItemType<ActivatedJellyGland>()))
			{
				npc.ai[1] = 0;
				Assert.IsTrue(drop.conditions.All(c => c.CanDrop(new DropAttemptInfo { npc = npc })));
				npc.ai[1] = 127;
				Assert.IsFalse(drop.conditions.All(c => c.CanDrop(new DropAttemptInfo { npc = npc })));
			}
		}
	}

	[TestMethod]
	public void BorerHoneyAndRockEquipmentAreRareDrops()
	{
		AssertRate<LampBorerHoney>(Rates<LampFruitBorer>(), 0.1f);
		var rates = Rates<RockElemental>();
		AssertRate<BrittleRockSlingshot>(rates, 1f / 30);
		AssertRate<HyperockSpear>(rates, 1f / 30);
		AssertRate<VitalizedRocks>(rates, 1f / 30);
	}

	[TestMethod]
	public void SquamousTrophyHasTenPercentChanceAndRelicRemainsMasterOnly()
	{
		var rates = Rates<SquamousShell>();
		AssertRate<SquamousShellTrophy>(rates, 0.1f);
		AssertRate<SquamousShellRelic>(rates, 1f);
		Assert.IsTrue(rates.Single(x => x.itemId == ModContent.ItemType<SquamousShellRelic>()).conditions.Any(c => c is Conditions.IsMasterMode));
	}

	[TestMethod]
	public void SquamousBagGivesCoreUntilPlayerHasConsumedPermanentUpgrade()
	{
		var item = (SquamousShellTreasureBag)RuntimeHelpers.GetUninitializedObject(typeof(SquamousShellTreasureBag));
		var loot = new ItemLoot(1, new ItemDropDatabase());
		item.ModifyItemLoot(loot);
		List<DropRateInfo> rates = [];
		foreach (var rule in loot.Get())
		{
			rule.ReportDroprates(rates, new DropRateInfoChainFeed(1));
		}
		AssertRate<SquamousCore>(rates, 1f);
		AssertRate<DragonScaleHammer>(rates, 1f / 6);
		AssertRate<BlueyWings>(rates, 1f / 6);
		AssertRate<DeadBeetleEgg>(rates, 1f / 6);
		AssertRate<EyeOfAnabiosis>(rates, 1f / 6);
		AssertRate<FlurryingBlades>(rates, 1f / 6);
		AssertRate<RockSpikeBallista>(rates, 1f / 6);

		var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		var modPlayer = new YggdrasilPlayer();
		var instance = typeof(ContentInstance<YggdrasilPlayer>).GetProperty("Instance")!;
		var previous = instance.GetValue(null);
		instance.SetValue(null, modPlayer);
		restoreContent.Add(() => instance.SetValue(null, previous));
		typeof(Player).GetField("modPlayers", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(player, new ModPlayer[] { modPlayer });
		var conditions = rates.Single(x => x.itemId == ModContent.ItemType<SquamousCore>()).conditions;
		Assert.IsTrue(conditions.All(c => c.CanDrop(new DropAttemptInfo { player = player })));
		modPlayer.ConsumedSquamousCore = 1;
		Assert.IsFalse(conditions.All(c => c.CanDrop(new DropAttemptInfo { player = player })));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void JellyBallsNeverDropDaifukuBeforeOrAfterKingDefeat(bool downedKing)
	{
		YggdrasilWorldSystem.DownedKingJellyBall = downedKing;
		foreach (var rates in new[] { Rates<JellyBall>(), Rates<GiantJellyBall>() })
		{
			Assert.IsFalse(rates.Any(rate => rate.itemId == ModContent.ItemType<JellyBallDaifuku>()));
		}
	}

	[TestMethod]
	public void SquamousGivesCraftingMaterialInsteadOfFinishedArmor()
	{
		var normal = Rates<SquamousShell>();
		AssertRate<GiantBeetleAntennae>(normal, 1f, 12, 18);
		Assert.IsTrue(normal.Single(x => x.itemId == ModContent.ItemType<GiantBeetleAntennae>()).conditions.Any(c => c is Conditions.NotExpert));
		var bag = (SquamousShellTreasureBag)RuntimeHelpers.GetUninitializedObject(typeof(SquamousShellTreasureBag));
		var loot = new ItemLoot(1, new ItemDropDatabase());
		bag.ModifyItemLoot(loot);
		List<DropRateInfo> expert = [];
		foreach (var rule in loot.Get())
		{
			rule.ReportDroprates(expert, new DropRateInfoChainFeed(1));
		}
		AssertRate<GiantBeetleAntennae>(expert, 1f, 18, 24);
		foreach (int armor in new[] { ModContent.ItemType<RockHelmet>(), ModContent.ItemType<RockPlateMail>(), ModContent.ItemType<RockGreaves>() })
		{
			Assert.IsFalse(normal.Any(rate => rate.itemId == armor));
			Assert.IsFalse(expert.Any(rate => rate.itemId == armor));
		}
	}

	private void BindNpc<T>(int type) where T : ModNPC
	{
		var content = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
		var npc = (NPC)RuntimeHelpers.GetUninitializedObject(typeof(NPC));
		npc.type = type;
		typeof(ModType<NPC>).GetProperty("Entity")!.SetValue(content, npc);
		var instance = typeof(ContentInstance<T>).GetProperty("Instance")!;
		var previous = instance.GetValue(null);
		instance.SetValue(null, content);
		restoreContent.Add(() => instance.SetValue(null, previous));
	}

	private void BindItem<T>() where T : ModItem
	{
		// Only substitute the content registry IDs. Execute the real NPC hook and tML drop rules.
		var content = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
		var item = (Item)RuntimeHelpers.GetUninitializedObject(typeof(Item));
		item.type = nextItemType++;
		typeof(ModType<Item>).GetProperty("Entity")!.SetValue(content, item);
		var instance = typeof(ContentInstance<T>).GetProperty("Instance")!;
		var previous = instance.GetValue(null);
		instance.SetValue(null, content);
		restoreContent.Add(() => instance.SetValue(null, previous));
	}

	private static List<DropRateInfo> Rates<T>() where T : ModNPC
	{
		var npc = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
		var loot = new NPCLoot(1, new ItemDropDatabase());
		npc.ModifyNPCLoot(loot);
		List<DropRateInfo> rates = [];
		foreach (var rule in loot.Get())
		{
			rule.ReportDroprates(rates, new DropRateInfoChainFeed(1));
		}
		return rates;
	}

	private static void AssertRate<T>(List<DropRateInfo> rates, float chance, int min = 1, int max = 1) where T : ModItem
	{
		var matches = rates.Where(x => x.itemId == ModContent.ItemType<T>()).ToArray();
		Assert.AreEqual(1, matches.Length, typeof(T).Name);
		Assert.AreEqual(chance, matches[0].dropRate, 0.00001f, typeof(T).Name);
		Assert.AreEqual(min, matches[0].stackMin);
		Assert.AreEqual(max, matches[0].stackMax);
	}
}
