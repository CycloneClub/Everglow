using System.Reflection;
using Everglow.Yggdrasil.Mechanics.TownCrowd;
using Terraria.ModLoader.Config;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class CrowdConfigTests
{
	[TestMethod]
	public void DensityScalesStreetAndTrainPopulationTogether()
	{
		var levels = new[] { CrowdDensity.Sparse, CrowdDensity.Standard, CrowdDensity.Busy }.Select(CrowdSettings.ForDensity).ToArray();
		for (int i = 1; i < levels.Length; i++)
		{
			Assert.IsTrue(levels[i].PopulationLimit > levels[i - 1].PopulationLimit);
			Assert.IsTrue(levels[i].SpawnIntervalTicks < levels[i - 1].SpawnIntervalTicks);
			Assert.IsTrue(levels[i].InitialFill > levels[i - 1].InitialFill);
			Assert.IsTrue(levels[i].TrainInitialPassengers > levels[i - 1].TrainInitialPassengers);
			Assert.IsTrue(levels[i].TrainPassengers > levels[i - 1].TrainPassengers);
		}
	}

	[TestMethod]
	public void ConfigRoundTripKeepsPreferencesAndIgnoresRemovedTuning()
	{
		var config = Newtonsoft.Json.JsonConvert.DeserializeObject<CrowdConfig>("{\"Enabled\":false,\"Density\":2,\"ChatChancePercent\":0,\"ReturnPercent\":100}")!;
		var loaded = Newtonsoft.Json.JsonConvert.DeserializeObject<CrowdConfig>(Newtonsoft.Json.JsonConvert.SerializeObject(config))!;
		Assert.IsFalse(loaded.Enabled);
		Assert.AreEqual(CrowdDensity.Busy, loaded.Density);
		Assert.AreEqual(CrowdDensity.Standard, new CrowdConfig().Density);
		Assert.AreSame(CrowdSettings.ForDensity(CrowdDensity.Standard), CrowdSettings.ForDensity((CrowdDensity)999));
	}

	[TestMethod]
	public void PlayerSettingsOnlyExposeEnableAndDensityOnTheClient()
	{
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdConfig");
		Assert.IsNotNull(type, "Crowd settings must be available in the normal mod configuration menu.");
		var config = (ModConfig)Activator.CreateInstance(type)!;
		Assert.AreEqual(ConfigScope.ClientSide, config.Mode);
		CollectionAssert.AreEquivalent(new[] { "Enabled", "Density" }, type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(field => field.Name).ToArray());
		Assert.AreEqual(true, type.GetField("Enabled")!.GetValue(config));
		Assert.AreEqual("Standard", type.GetField("Density")!.GetValue(config)!.ToString());
	}
}
