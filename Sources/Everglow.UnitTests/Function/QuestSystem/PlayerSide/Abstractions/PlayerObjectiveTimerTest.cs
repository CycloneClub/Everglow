using System.Reflection;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class PlayerObjectiveTimerTest
{
	private sealed class StubObjective : PlayerObjectiveBase
	{
		public override bool CheckCompletion() => false;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}

		public override string GetObjectiveText() => string.Empty;
	}

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
	}

	[TestMethod]
	public void WithTimeLimit_ConfiguresTimerAndReturnsObjective()
	{
		var objective = new StubObjective();

		PlayerObjectiveBase configured = objective.WithTimeLimit(100);

		Assert.AreSame(objective, configured);
		Assert.IsNotNull(objective.Timer);
		Assert.AreEqual(100, objective.Timer.TimeLimit);
		Assert.IsFalse(objective.IsTimedOut);
	}

	[TestMethod]
	public void RuntimeGate_DoesNotAddAlternativeObjectiveLifecycleMethods()
	{
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		Assert.IsNull(typeof(PlayerObjectiveBase).GetMethod("UpdateRuntime", flags));
		Assert.IsNull(typeof(PlayerObjectiveBase).GetMethod("CheckRuntimeCompletion", flags));
		Assert.IsNull(typeof(PlayerObjectiveBase).GetMethod("UpdateTimer", flags));
	}

	[TestMethod]
	public void ResetProgress_ReopensTimedOutObjective()
	{
		var objective = new StubObjective();
		objective.WithTimeLimit(10);
		objective.Timer.Update(10);

		objective.ResetProgress();

		Assert.AreEqual(0, objective.Timer.ElapsedTime);
		Assert.IsFalse(objective.IsTimedOut);
	}

	[TestMethod]
	public void SaveData_LoadData_RestoresAndClampsElapsedTimeToCurrentLimit()
	{
		var saved = new StubObjective();
		saved.WithTimeLimit(100);
		saved.Timer.Update(80);
		var tag = new TagCompound();

		try
		{
			saved.SaveData(tag);
		}
		catch (IOException)
		{
			// The headless TagCompound rejects the existing bool reward payload.
		}

		var loaded = new StubObjective();
		loaded.WithTimeLimit(50);
		loaded.LoadData(tag);

		Assert.AreEqual(50, loaded.Timer.ElapsedTime);
		Assert.IsTrue(loaded.IsTimedOut);
	}

	[TestMethod]
	public void LoadData_MissingTimerData_StartsAtZero()
	{
		var objective = new StubObjective();
		objective.WithTimeLimit(100);
		objective.Timer.Update(40);

		objective.LoadData(new TagCompound());

		Assert.AreEqual(0, objective.Timer.ElapsedTime);
		Assert.IsFalse(objective.IsTimedOut);
	}
}
