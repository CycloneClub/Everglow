using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
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
	public void MissingObjectiveTextDoesNotExposeLocalizationKey()
	{
		var objective = new WorldReachObjective(_ => true) { LocalizationKey = "Tests.Missing" };
		Assert.AreEqual(string.Empty, objective.GetObjectiveText());
		Assert.IsFalse(objective.CheckCompletion());
	}
}
