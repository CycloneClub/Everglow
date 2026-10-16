using System.Reflection;
using Everglow.Commons.Mechanics.MirrorReflection;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Everglow.Commons.Mechanics.MirrorReflection.Core;

namespace Everglow.UnitTests.Function;

[TestClass]
public class MirrorWallRegistryTest
{
	[TestMethod]
	public void AddingDemo_PreservesBuiltInWalls()
	{
		var defaults = VanillaMirrorWallProfiles.Create();
		var system = CreateSystem(defaults);
		int demoType = WallID.Count;
		system.RegisterWall(demoType, new MirrorReflectionDefinition(0.85f));
		var profiles = ReadProfiles(system);
		Assert.AreEqual(defaults[WallID.Glass], profiles[WallID.Glass]);
		Assert.AreEqual(defaults.Count + 1, profiles.Count);
	}

	[TestMethod]
	public void DynamicRegistration_ReadsCurrentPanelValuesWithoutReimportingDefaults()
	{
		var system = CreateSystem(VanillaMirrorWallProfiles.Create());
		float opacity = 0.85f;
		int demoType = WallID.Count;
		system.RegisterWall(demoType, () => new MirrorReflectionDefinition(opacity));
		Assert.AreEqual(0.85f, ReadProfiles(system)[demoType].Opacity);
		opacity = 0.4f;
		var profiles = ReadProfiles(system);
		Assert.AreEqual(0.4f, profiles[demoType].Opacity);
		Assert.AreEqual(0.35f, profiles[WallID.Glass].Opacity);
	}

	[TestMethod]
	public void ReplacingRegistration_ProducesOneProfilePerWallType()
	{
		var system = CreateSystem(VanillaMirrorWallProfiles.Create());
		system.RegisterWall(WallID.Glass, () => new MirrorReflectionDefinition(0.6f));
		system.RegisterWall(WallID.Glass, new MirrorReflectionDefinition(0.2f));
		var profiles = ReadProfiles(system);
		Assert.AreEqual(0.2f, profiles[WallID.Glass].Opacity);
		Assert.AreEqual(VanillaMirrorWallProfiles.Create().Count, profiles.Count);
	}

	[TestMethod]
	public void ChangingWorlds_PreservesRegisteredMaterials()
	{
		var defaults = VanillaMirrorWallProfiles.Create();
		var system = CreateSystem(defaults);
		int demoType = WallID.Count;
		system.RegisterWall(demoType, new MirrorReflectionDefinition(0.7f));
		system.OnWorldUnload();
		system.OnWorldLoad();
		var profiles = ReadProfiles(system);
		Assert.AreEqual(0.7f, profiles[demoType].Opacity);
		Assert.AreEqual(defaults[WallID.Glass], profiles[WallID.Glass]);
		Assert.AreEqual(defaults.Count + 1, profiles.Count);
	}

	[TestMethod]
	public void ResolveMaterials_UsesSameSamplingKeyForDifferentOpacitiesAndStrengths()
	{
		var system = CreateSystem(new Dictionary<int, MirrorReflectionDefinition>
		{
			[WallID.Glass] = new(0.35f) { Blur = 0 },
			[WallID.MarbleBlock] = new(0.2f, 0.9f) { Blur = 0 },
			[WallID.GoldBrick] = new(0.18f) { Blur = 1 },
		});
		var materials = ResolveMaterials(system);
		Assert.AreEqual(materials[WallID.Glass].Definition, materials[WallID.MarbleBlock].Definition);
		Assert.AreNotEqual(materials[WallID.Glass].Tint, materials[WallID.MarbleBlock].Tint);
		var groups = materials.Values.Select(material => material.Definition).Distinct().ToArray();
		Assert.AreEqual(2, groups.Length);
		CollectionAssert.AreEquivalent(new[] { 0f, 1f }, groups.Select(group => group.Blur).ToArray());
	}

	[TestMethod]
	public void ResolveMaterials_ReadsDynamicDefinitionOnceAndReturnsIndependentSnapshots()
	{
		var system = CreateSystem(new Dictionary<int, MirrorReflectionDefinition>());
		int calls = 0;
		float opacity = 0.5f;
		system.RegisterWall(WallID.Glass, () =>
		{
			calls++;
			return new MirrorReflectionDefinition(opacity);
		});
		var first = ResolveMaterials(system);
		Assert.AreEqual(1, first.Count);
		Assert.AreEqual(1, calls);

		opacity = 0;
		Assert.AreEqual(0, ResolveMaterials(system).Count);
		Assert.AreEqual(1, first.Count);
		Assert.AreEqual(new Color(0.5f, 0.5f, 0.5f, 0.5f), first[WallID.Glass].Tint);
		Assert.AreEqual(2, calls);
	}

	[TestMethod]
	[DataRow(0.4f, 1f, 0.4f, 0.4f)]
	[DataRow(1f, 1f, 1f, 1f)]
	[DataRow(0.8f, 0.5f, 0.4f, 0.8f)]
	[DataRow(0.8f, 0f, 0f, 0.8f)]
	[DataRow(2f, 2f, 1f, 1f)]
	[DataRow(0.8f, -1f, 0f, 0.8f)]
	public void ResolveMaterials_PremultipliesBrightnessWithoutChangingCoverage(float opacity, float strength, float rgb, float alpha)
	{
		var materials = ResolveMaterials(new Dictionary<int, MirrorReflectionDefinition>
		{
			[WallID.Glass] = new(opacity, strength),
		});
		Assert.AreEqual(new Color(rgb, rgb, rgb, alpha), materials[WallID.Glass].Tint);
	}

	[TestMethod]
	public void ResolveMaterials_SkipsInvisibleAndDefaultDefinitions()
	{
		var materials = ResolveMaterials(new Dictionary<int, MirrorReflectionDefinition>
		{
			[WallID.Glass] = new(0),
			[WallID.MarbleBlock] = new(-1),
			[WallID.GoldBrick] = default,
		});
		Assert.AreEqual(0, materials.Count);
	}

	[TestMethod]
	public void ResolveMaterials_ClampsSamplingWithoutMutatingRegisteredDefinition()
	{
		var definition = new MirrorReflectionDefinition(0.5f)
		{
			Offset = new(-200, 50),
			ReflectionScale = 0,
			Blur = 10,
			Sigma = 0,
			NormalStrength = 50,
			NormalPeriod = 0,
			Distance = 0,
			Pitch = 90,
			Yaw = -90,
			Roll = 180,
		};
		var system = CreateSystem(new Dictionary<int, MirrorReflectionDefinition> { [WallID.Glass] = definition });
		var resolved = ResolveMaterials(system)[WallID.Glass].Definition;
		Assert.AreEqual(0.5f, resolved.ReflectionScale);
		Assert.AreEqual(0.25f, resolved.Sigma);
		Assert.AreEqual(8f, resolved.NormalPeriod);
		Assert.AreEqual(240f, resolved.Distance);
		Assert.AreEqual(60f, resolved.Pitch);
		Assert.AreEqual(new Vector2(-100, 30), resolved.Offset);
		Assert.AreEqual(4f, resolved.Blur);
		Assert.AreEqual(24f, resolved.NormalStrength);
		Assert.AreEqual(-60f, resolved.Yaw);
		Assert.AreEqual(90f, resolved.Roll);
		Assert.AreEqual(definition, ReadProfiles(system)[WallID.Glass]);
	}

	[TestMethod]
	[DataRow(8f)]
	[DataRow(12f)]
	[DataRow(16f)]
	public void ResolveMaterials_PreservesFineNormalPeriods(float period)
	{
		var materials = ResolveMaterials(new Dictionary<int, MirrorReflectionDefinition>
		{
			[WallID.Glass] = new(0.35f) { NormalPeriod = period, NormalStrength = 24 },
		});
		Assert.AreEqual(period, materials[WallID.Glass].Definition.NormalPeriod);
		Assert.AreEqual(24f, materials[WallID.Glass].Definition.NormalStrength);
	}

	[TestMethod]
	public void ResolveMaterials_DistinguishesEverySamplingSetting()
	{
		var original = new MirrorReflectionDefinition(1);
		MirrorReflectionDefinition[] definitions =
		[
			original,
			original with { Offset = new(20, 0) },
			original with { ReflectionScale = 1.2f },
			original with { FlipHorizontally = true },
			original with { Blur = 2 },
			original with { Sigma = 2 },
			original with { NormalPattern = MirrorNormalPattern.Twist },
			original with { NormalStrength = 20 },
			original with { NormalPeriod = 128 },
			original with { Pitch = 20 },
			original with { Yaw = 20 },
			original with { Roll = 20 },
			original with { Distance = 600 },
		];
		var materials = ResolveMaterials(definitions.Select((definition, index) => (definition, index))
			.ToDictionary(entry => entry.index + 1, entry => entry.definition));
		Assert.AreEqual(definitions.Length, materials.Values.Select(material => material.Definition).Distinct().Count());
	}

	internal static Dictionary<int, (MirrorReflectionDefinition Definition, Color Tint)> ResolveMaterials(IReadOnlyDictionary<int, MirrorReflectionDefinition> definitions)
		=> ResolveMaterials(CreateSystem(definitions));

	private static Dictionary<int, (MirrorReflectionDefinition Definition, Color Tint)> ResolveMaterials(MirrorReflectionSystem system)
	{
		var collector = typeof(MirrorReflectionSystem).GetField("wallCollector", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(system)!;
		return (Dictionary<int, (MirrorReflectionDefinition, Color)>)collector.GetType().GetMethod("ResolveMaterials")!.Invoke(collector, null)!;
	}

	// 只初始化材质存储，避免调用 Load 安装图形钩子或启动 Terraria.Main。
	private static MirrorReflectionSystem CreateSystem(IReadOnlyDictionary<int, MirrorReflectionDefinition> defaults)
	{
		var system = new MirrorReflectionSystem();
		var field = typeof(MirrorReflectionSystem).GetField("wallCollector", BindingFlags.Instance | BindingFlags.NonPublic)!;
		field.SetValue(system, Activator.CreateInstance(field.FieldType, new object[] { defaults }));
		return system;
	}

	private static Dictionary<int, MirrorReflectionDefinition> ReadProfiles(MirrorReflectionSystem system)
	{
		var collector = typeof(MirrorReflectionSystem).GetField("wallCollector", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(system)!;
		var providers = (Dictionary<int, Func<MirrorReflectionDefinition>>)collector.GetType()
			.GetField("profiles", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(collector)!;
		return providers.ToDictionary(pair => pair.Key, pair => pair.Value());
	}
}
