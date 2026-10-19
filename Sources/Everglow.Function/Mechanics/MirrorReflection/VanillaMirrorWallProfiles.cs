using Everglow.Commons.Mechanics.MirrorReflection.Core;
using Terraria.ID;

namespace Everglow.Commons.Mechanics.MirrorReflection;

/// <summary>原版墙的初始反射材质。覆盖率优先保留墙面纹理，不改变其原版绘制。</summary>
public static class VanillaMirrorWallProfiles
{
	public static Dictionary<int, MirrorReflectionDefinition> Create()
	{
		Dictionary<int, MirrorReflectionDefinition> profiles = [];
		// 普通玻璃使用清晰倒影；花窗玻璃、冰雪和镒墙使用细密强扰动。
		Add(0.5f, 1, 0, WallID.Glass);
		var stainedGlass = profiles[WallID.Glass] with { Blur = 1, NormalPeriod = 12, NormalStrength = 24 };
		int[] stainedGlassWalls =
		[
			WallID.PurpleStainedGlass, WallID.YellowStainedGlass, WallID.BlueStainedGlass,
			WallID.GreenStainedGlass, WallID.RedStainedGlass, WallID.OrangeStainedGlass, WallID.RainbowStainedGlass,
			WallID.IceEcho, WallID.IceUnsafe, WallID.ShimmerBlockWall, WallID.ShimmerBrickWall,
		];
		foreach (int wall in stainedGlassWalls)
		{
			profiles.Add(wall, stainedGlass);
		}
		// 外层玻璃负责反光，内部的瀑布、沙尘、雪花、彩纸和星光不改变玻璃材质。
		Add(0.35f, 1, 0, WallID.SandFall, WallID.SnowFall, WallID.Confetti, WallID.ConfettiBlack,
			WallID.GoldStarryGlassWall, WallID.BlueStarryGlassWall, WallID.Waterfall, WallID.Lavafall, WallID.Honeyfall);
		Add(0.32f, 1, 0, WallID.Crystal);
		Add(0.24f, 0.95f, 0, WallID.IceBrick);

		// 自发光宝石保留较多底色；未通电版本略增强倒影。
		Add(0.22f, 1, 0, WallID.AmberGemspark, WallID.AmethystGemspark, WallID.DiamondGemspark,
			WallID.EmeraldGemspark, WallID.RubyGemspark, WallID.SapphireGemspark, WallID.TopazGemspark);
		Add(0.28f, 0.95f, 0, WallID.AmberGemsparkOff, WallID.AmethystGemsparkOff, WallID.DiamondGemsparkOff,
			WallID.EmeraldGemsparkOff, WallID.RubyGemsparkOff, WallID.SapphireGemsparkOff, WallID.TopazGemsparkOff);

		// 光面大理石使用清晰倒影；花岗岩与金属砖保留柔和反射。
		Add(0.24f, 0.95f, 1, WallID.GraniteBlock);
		Add(0.20f, 0.9f, 0, WallID.MarbleBlock);
		Add(0.26f, 1, 1, WallID.CopperPlating, WallID.TinPlating, WallID.ShroomitePlating, WallID.MartianConduit);
		Add(0.18f, 0.95f, 1, WallID.GoldBrick, WallID.SilverBrick, WallID.PlatinumBrick);
		Add(0.14f, 0.9f, 1, WallID.CopperBrick, WallID.TinBrick, WallID.TungstenBrick,
			WallID.IronBrick, WallID.LeadBrick, WallID.CobaltBrick, WallID.MythrilBrick);
		Add(0.16f, 0.9f, 1, WallID.ObsidianBrick, WallID.IridescentBrick);

		// 糖衣与气球使用轻微模糊，凝胶保留清晰倒影。
		Add(0.18f, 0.9f, 0.5f, WallID.CandyCane, WallID.GreenCandyCane,
			WallID.SillyBalloonPinkWall, WallID.SillyBalloonGreenWall, WallID.SillyBalloonPurpleWall);
		Add(0.22f, 0.9f, 0, WallID.Slime);

		// 柱梁、夜明砖及其月相变体使用普通模糊。
		Add(0.24f, 0.95f, 1, WallID.PalladiumColumn, WallID.AdamantiteBeam);
		Add(0.20f, 0.95f, 1, WallID.LunarBrickWall);
		Add(0.18f, 0.95f, 1, WallID.LunarRustBrickWall, WallID.DarkCelestialBrickWall,
			WallID.AstraBrickWall, WallID.CosmicEmberBrickWall, WallID.CryocoreBrickWall,
			WallID.MercuryBrickWall, WallID.StarRoyaleBrickWall, WallID.HeavenforgeBrickWall);

		// 远古金属砖沿用对应现代砖的反射强度。
		Add(0.18f, 0.95f, 1, WallID.AncientGoldBrickWall, WallID.AncientSilverBrickWall);
		Add(0.14f, 0.9f, 1, WallID.AncientCopperBrickWall, WallID.AncientCobaltBrickWall, WallID.AncientMythrilBrickWall);

		return profiles;

		void Add(float opacity, float strength, float blur, params int[] walls)
		{
			var profile = new MirrorReflectionDefinition(opacity, strength)
			{
				Blur = blur,
			};
			foreach (int wall in walls)
			{
				profiles.Add(wall, profile);
			}
		}
	}
}
