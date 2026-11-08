using Everglow.Commons.Mechanics.NPCSpawn;

namespace Everglow.UnitTests.Function.NPCSpawn;

[TestClass]
public class SpawnContentTests
{
	[TestMethod]
	public void PublicRuleNeedsNeitherNpcInterfaceNorHabitatClassification()
	{
		var assembly = typeof(SubworldSpawnRule).Assembly;
		Assert.IsNull(assembly.GetType("Everglow.Commons.Mechanics.NPCSpawn.IManagedSpawnProvider"));
		Assert.IsNull(assembly.GetType("Everglow.Commons.Mechanics.NPCSpawn.SpawnHabitat"));
		Assert.IsNull(typeof(SubworldSpawnRule).GetProperty("Id"));
		Assert.IsNull(typeof(SubworldSpawnRule).GetProperty("FollowerAi1"));
	}
}
