using Everglow.Yggdrasil.YggdrasilTown.Travelers;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class TravelerOptionsTests
{
	[TestMethod]
	public void TrainSettingsPersistSeparatelyThroughPlayerSaveData()
	{
		var player = new TravelerPreferences();
		player.Options[TravelerOption.TrainInitialPassengers] = 10;
		player.Options[TravelerOption.TrainPassengers] = 4;
		var tag = new TagCompound();
		player.SaveData(tag);
		var loaded = new TravelerPreferences();
		loaded.LoadData(tag);
		Assert.AreEqual(10f, loaded.Options[TravelerOption.TrainInitialPassengers]);
		Assert.AreEqual(4f, loaded.Options[TravelerOption.TrainPassengers]);
	}

	[TestMethod]
	public void LiveOptionsControlIntervalAndCapacity()
	{
		var options = new TravelerOptions();
		Assert.IsTrue(options.SpawnIntervalTicks < 240);
		Assert.IsTrue(options.PopulationLimit(TravelerMode.NPC) > 12);
		Assert.IsTrue(options.PopulationLimit(TravelerMode.Actors) > 24);
		options[TravelerOption.SpawnSeconds] = 0.5f;
		options[TravelerOption.NpcLimit] = 40;
		Assert.AreEqual(30, options.SpawnIntervalTicks);
		Assert.AreEqual(40, options.PopulationLimit(TravelerMode.NPC));
	}

	[TestMethod]
	public void SourceWeightsCanDisableAnyOrAllArrivals()
	{
		var options = new TravelerOptions();
		options[TravelerOption.StreetWeight] = 0;
		options[TravelerOption.PylonWeight] = 0;
		options[TravelerOption.BuildingWeight] = 3;
		for (int roll = 0; roll < 3; roll++)
		{
			Assert.AreEqual(1, options.ChooseSource(roll));
		}
		options[TravelerOption.BuildingWeight] = 0;
		Assert.AreEqual(-1, options.ChooseSource(0));
	}

	[TestMethod]
	public void InvalidAndCrossedValuesCannotBreakSimulation()
	{
		var options = new TravelerOptions();
		options[TravelerOption.SpawnSeconds] = float.NaN;
		Assert.IsTrue(options.SpawnIntervalTicks > 0);
		options[TravelerOption.NpcLimit] = 10000;
		Assert.IsTrue(options.PopulationLimit(TravelerMode.NPC) <= 80);
		options[TravelerOption.ActivityMinSeconds] = 20;
		Assert.IsTrue(options.ActivityMaxTicks >= options.ActivityMinTicks);
		options[TravelerOption.ActivityMaxSeconds] = 2;
		Assert.IsTrue(options.ActivityMaxTicks >= options.ActivityMinTicks);
	}

	[TestMethod]
	public void TwoTrainPassengersCanUseSeparatePlanningFrames()
	{
		var batch = new TravelerStopBatch();
		Assert.IsTrue(batch.TryBegin(true, 30, 600));
		Assert.IsTrue(batch.TryNextSide(2, 3, out int first));
		Assert.AreEqual(-1, first);
		batch.RecordArrival();
		Assert.IsFalse(batch.TryNextSide(2, 1, out _));
		Assert.IsFalse(batch.TryNextSide(1, 3, out _));
		Assert.IsTrue(batch.TryNextSide(2, 3, out int second));
		Assert.AreEqual(1, second);
		batch.RecordArrival();
		Assert.IsFalse(batch.TryNextSide(2, 3, out _));
		batch.TryBegin(false, 600, 600);
		Assert.IsTrue(batch.TryBegin(true, 30, 600));
		Assert.IsTrue(batch.TryNextSide(1, 3, out _));
	}

	[TestMethod]
	public void SettingsRoundTripWithoutSavingEnabledMode()
	{
		var options = new TravelerOptions();
		options[TravelerOption.SpawnSeconds] = 0.75f;
		options[TravelerOption.ActorLimit] = 88;
		var tag = options.Save();
		var loaded = new TravelerOptions();
		loaded.Load(tag);
		Assert.AreEqual(45, loaded.SpawnIntervalTicks);
		Assert.AreEqual(88, loaded.PopulationLimit(TravelerMode.Actors));
		Assert.IsFalse(tag.ContainsKey("Mode"));
		loaded.Load(new TagCompound());
		Assert.AreEqual(new TravelerOptions().SpawnIntervalTicks, loaded.SpawnIntervalTicks);
	}
}
