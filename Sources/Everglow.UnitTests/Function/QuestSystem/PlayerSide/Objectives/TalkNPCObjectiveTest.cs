using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class TalkNPCObjectiveTest
{
	[TestInitialize]
	public void Initialize() => Terraria.Program.SavePath = string.Empty;

	[TestMethod]
	public void Constructor_Should_ThrowException_When_NPCTypeIsLessThanOne()
	{
		for (int i = 0; i > -100; i--)
		{
			Assert.ThrowsExactly<InvalidDataException>(() => new TalkNPCObjective(i));
		}
	}

	[TestMethod]
	public void MissingDialogueDoesNotClearOriginalNpcChat()
	{
		string original = Terraria.Main.npcChatText;
		try
		{
			Terraria.Main.npcChatText = "Original dialogue";
			new TalkNPCObjective(1).Complete();
			Assert.AreEqual("Original dialogue", Terraria.Main.npcChatText);
		}
		finally
		{
			Terraria.Main.npcChatText = original;
		}
	}
}
