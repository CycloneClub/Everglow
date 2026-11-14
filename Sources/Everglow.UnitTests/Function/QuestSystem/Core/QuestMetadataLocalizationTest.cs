using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Terraria.Localization;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class QuestMetadataLocalizationTest
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
	public void Metadata_UsesCustomLocalizationPathForBothQuestSides()
	{
		Language.GetOrRegister("Tests.Metadata.DisplayName", () => "Title");
		Language.GetOrRegister("Tests.Metadata.Description", () => "Description");
		Language.GetOrRegister("Tests.Metadata.Hint", () => "Hint");
		Language.GetOrRegister("Tests.Metadata.Greeting", () => "Hello {0}");
		var world = new WorldQuest();
		var player = new PlayerQuest();

		Assert.AreEqual("Title", world.DisplayName);
		Assert.AreEqual("Title", player.DisplayName);
		Assert.AreEqual("Description", world.Description);
		Assert.AreEqual("Description", player.Description);
		Assert.AreEqual("Hint", world.Hint);
		Assert.AreEqual("Hint", player.Hint);
		Assert.AreEqual("Hello Herm", world.Greeting);
		Assert.AreEqual("Hello Herm", player.Greeting);
	}

	[TestMethod]
	public void MissingMetadata_ReturnsEmptyTextForBothQuestSides()
	{
		var world = new WorldQuest();
		var player = new PlayerQuest();

		Assert.AreEqual(string.Empty, world.DisplayName);
		Assert.AreEqual(string.Empty, player.DisplayName);
		Assert.AreEqual(string.Empty, world.Description);
		Assert.AreEqual(string.Empty, player.Description);
		Assert.AreEqual(string.Empty, world.Hint);
		Assert.AreEqual(string.Empty, player.Hint);
		Assert.AreEqual(string.Empty, world.Greeting);
		Assert.AreEqual(string.Empty, player.Greeting);
	}

	private sealed class WorldQuest : WorldQuestBase
	{
		public override string LocalizationKey => "Tests.Metadata";

		public string Greeting => Text("Greeting", "Herm");
	}

	private sealed class PlayerQuest : PlayerQuestBase
	{
		public override string LocalizationKey => "Tests.Metadata";

		public string Greeting => Text("Greeting", "Herm");
	}
}
