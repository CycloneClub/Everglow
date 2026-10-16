using Everglow.Commons.Mechanics.MirrorReflection;
using Terraria.ID;
using Everglow.Commons.Mechanics.MirrorReflection.Core;

namespace Everglow.UnitTests.Function;

[TestClass]
public class MirrorReflectionConfigTest
{
	[TestMethod]
	[DataRow(MirrorReflectionQuality.Low, 3)]
	[DataRow(MirrorReflectionQuality.Medium, 5)]
	[DataRow(MirrorReflectionQuality.High, 9)]
	public void Quality_SelectsBlurSampleBudget(MirrorReflectionQuality quality, int samples)
	{
		var config = new MirrorReflectionConfig { Quality = quality };
		Assert.AreEqual(samples, config.GetBlurSampleCount());
	}

	[TestMethod]
	public void InvalidQuality_FallsBackToCurrentHighQuality()
	{
		var config = new MirrorReflectionConfig { Quality = (MirrorReflectionQuality)99 };
		Assert.AreEqual(9, config.GetBlurSampleCount());
	}

	[TestMethod]
	public void OldConfig_RetainsSwitchAndIgnoresRemovedMaterialOverrides()
	{
		var config = Newtonsoft.Json.JsonConvert.DeserializeObject<MirrorReflectionConfig>(
			"{\"Enabled\":false,\"OffsetX\":100,\"OpacityMultiplier\":0,\"FlipHorizontally\":true}")!;
		Assert.IsFalse(config.Enabled);
		Assert.AreEqual(MirrorReflectionQuality.High, config.Quality);
	}

	[TestMethod]
	public void VanillaMaterials_KeepIndependentDefinitions()
	{
		var demo = new MirrorReflectionDefinition(1) { NormalPattern = MirrorNormalPattern.Twist, NormalStrength = 20, Yaw = 30, ReflectionScale = 1.4f };
		var profiles = VanillaMirrorWallProfiles.Create();
		foreach (var profile in profiles.Values)
		{
			var parameters = profile;
			Assert.AreNotEqual(demo, parameters);
			Assert.AreEqual(0f, parameters.Yaw);
			Assert.AreEqual(1f, parameters.ReflectionScale);
		}
		Assert.AreEqual(profiles[WallID.Glass], profiles[WallID.Lavafall]);
		Assert.AreEqual(profiles[WallID.Glass], profiles[WallID.SnowFall]);
	}

	[TestMethod]
	public void VanillaMaterials_CoverRequestedWallsWithAtMostThreeSamplingGroups()
	{
		var profiles = VanillaMirrorWallProfiles.Create();
		int[] required =
		{
			WallID.RainbowStainedGlass, WallID.Waterfall, WallID.Lavafall, WallID.Honeyfall,
			WallID.AmberGemspark, WallID.AmethystGemspark, WallID.DiamondGemspark, WallID.EmeraldGemspark,
			WallID.RubyGemspark, WallID.SapphireGemspark, WallID.TopazGemspark,
			WallID.GraniteBlock, WallID.MarbleBlock, WallID.Crystal, WallID.SandFall, WallID.SnowFall,
			WallID.Confetti, WallID.ConfettiBlack, WallID.GoldStarryGlassWall, WallID.BlueStarryGlassWall,
			WallID.PalladiumColumn, WallID.AdamantiteBeam, WallID.CandyCane, WallID.GreenCandyCane,
			WallID.Slime, WallID.LunarBrickWall, WallID.SillyBalloonPinkWall, WallID.SillyBalloonGreenWall,
			WallID.SillyBalloonPurpleWall, WallID.IceBrick, WallID.IceEcho, WallID.ShimmerBlockWall, WallID.ShimmerBrickWall,
			WallID.LunarRustBrickWall, WallID.DarkCelestialBrickWall, WallID.AstraBrickWall,
			WallID.CosmicEmberBrickWall, WallID.CryocoreBrickWall, WallID.MercuryBrickWall,
			WallID.StarRoyaleBrickWall, WallID.HeavenforgeBrickWall, WallID.AncientGoldBrickWall,
			WallID.AncientSilverBrickWall, WallID.AncientCopperBrickWall, WallID.AncientCobaltBrickWall,
			WallID.AncientMythrilBrickWall,
		};
		foreach (int wall in required)
		{
			Assert.IsTrue(profiles.ContainsKey(wall), $"Missing WallID {wall}");
		}
		Assert.IsTrue(MirrorWallRegistryTest.ResolveMaterials(profiles).Values.Select(material => material.Definition).Distinct().Count() <= 3);
		Assert.IsFalse(profiles.ContainsKey(WallID.DirtUnsafe));
	}

	[TestMethod]
	public void ClearMaterials_SkipGaussianBlur()
	{
		var profiles = VanillaMirrorWallProfiles.Create();
		int[] walls =
		[
			WallID.Glass, WallID.Waterfall, WallID.Lavafall, WallID.Honeyfall, WallID.SandFall,
			WallID.SnowFall, WallID.Confetti, WallID.ConfettiBlack, WallID.GoldStarryGlassWall,
			WallID.BlueStarryGlassWall, WallID.Crystal, WallID.IceBrick, WallID.IceEcho, WallID.MarbleBlock,
			WallID.CandyCane, WallID.GreenCandyCane, WallID.Slime, WallID.ShimmerBlockWall,
			WallID.SillyBalloonPinkWall, WallID.SillyBalloonGreenWall, WallID.SillyBalloonPurpleWall,
			WallID.AmberGemspark, WallID.AmethystGemspark, WallID.DiamondGemspark,
			WallID.EmeraldGemspark, WallID.RubyGemspark, WallID.SapphireGemspark, WallID.TopazGemspark,
			WallID.AmberGemsparkOff, WallID.AmethystGemsparkOff, WallID.DiamondGemsparkOff,
			WallID.EmeraldGemsparkOff, WallID.RubyGemsparkOff, WallID.SapphireGemsparkOff, WallID.TopazGemsparkOff,
		];
		foreach (int wall in walls)
		{
			Assert.AreEqual(0f, profiles[wall].Blur, $"Unexpected blur for WallID {wall}");
		}
	}

	[TestMethod]
	public void StainedGlass_UsesDenseStrongDistortionWithSoftenedReflection()
	{
		var profiles = VanillaMirrorWallProfiles.Create();
		int[] walls =
		[
			WallID.PurpleStainedGlass, WallID.YellowStainedGlass, WallID.BlueStainedGlass,
			WallID.GreenStainedGlass, WallID.RedStainedGlass, WallID.OrangeStainedGlass, WallID.RainbowStainedGlass,
		];
		foreach (int wall in walls)
		{
			var parameters = profiles[wall];
			Assert.AreEqual(12f, parameters.NormalPeriod);
			Assert.AreEqual(24f, parameters.NormalStrength);
			Assert.AreEqual(1f, parameters.Blur);
		}
		Assert.AreEqual(0f, profiles[WallID.Glass].NormalStrength);
	}
}
