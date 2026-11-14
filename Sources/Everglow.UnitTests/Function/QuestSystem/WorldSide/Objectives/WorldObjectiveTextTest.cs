using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Terraria.ID;
using Terraria.Localization;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldObjectiveTextTest
{
	private LanguageManager oldLanguage = null!;

	[TestInitialize]
	public void Initialize()
	{
		oldLanguage = LanguageManager.Instance;
		LanguageManager.Instance = (LanguageManager)Activator.CreateInstance(typeof(LanguageManager), true)!;
	}

	[TestCleanup]
	public void Cleanup() => LanguageManager.Instance = oldLanguage;

	[TestMethod]
	public void ReachObjective_PreservesLocalizedTextMarkup()
	{
		const string text = "[TextDrawer,Text='到达目标',Color='1,2,3,255']";
		Language.GetOrRegister("Tests.Reach.ObjectiveText", () => text);
		var objective = new WorldReachObjective(_ => true) { LocalizationKey = "Tests.Reach" };

		Assert.AreEqual(text, objective.GetObjectiveText());
	}

	[TestMethod]
	public void ExploreObjective_AppendsSynchronizedProgressToLocalizedText()
	{
		Language.GetOrRegister("Tests.Explore.ObjectiveText", () => "在丛林中探索");
		var objective = new WorldExploreObjective(500, _ => true) { LocalizationKey = "Tests.Explore" };

		Assert.AreEqual("在丛林中探索 (0/500)", objective.GetObjectiveText());
	}

	[TestMethod]
	public void SingleItemObjectivesPutTheRequiredCountOnTheIconAndKeepProgress()
	{
		Language.GetOrRegister("Mods.Everglow.QuestSystem.Objectives.Have", () => "拥有{0} {1}");
		Language.GetOrRegister("Mods.Everglow.QuestSystem.Objectives.Consume", () => "消耗{0} {1}");
		string icon = ItemDrawer.Create(ItemID.Ale, 5);

		Assert.AreEqual($"拥有{icon} (0/5)", new WorldCollectItemObjective(ItemID.Ale, 5).GetObjectiveText());
		Assert.AreEqual($"消耗{icon} (0/5)", new WorldConsumeItemObjective(ItemID.Ale, 5).GetObjectiveText());
	}

	[TestMethod]
	public void MissingObjectiveTextDoesNotExposeLocalizationKey()
	{
		var objective = new WorldReachObjective(_ => true) { LocalizationKey = "Tests.Missing" };
		Assert.AreEqual(string.Empty, objective.GetObjectiveText());
		Assert.IsFalse(objective.CheckCompletion());
	}
}
