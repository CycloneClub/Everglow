using Everglow.Myth.TheTusk.Backgrounds;
using Everglow.Myth.TheTusk.Water;
using Terraria.Graphics.Capture;
using Terraria.Localization;

namespace Everglow.Myth.TheTusk.WorldGeneration;

// Shows setting up two basic biomes. For a more complicated example, please request.
public class TuskSurfaceBiome : ModBiome
{
	// public override bool IsPrimaryBiome => true; // Allows this biome to impact NPC prices

	// Select all the scenery
	public override ModWaterStyle WaterStyle => ModContent.GetInstance<TuskWaterStyle>(); // ModContent.Find<ModWaterStyle>("Everglow/Myth/TheTusk/WorldGeneration/TuskWaterStyle"); // Sets a water style for when inside this biome
	public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<TuskSurfaceBackgroundStyle>(); // ModContent.Find<ModSurfaceBackgroundStyle>("Everglow/Myth/TheTusk/Background/TuskSurfaceBackgroundStyle");
	public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

	public override string BestiaryIcon => base.BestiaryIcon;
	public override string BackgroundPath => base.BackgroundPath;
	public override Color? BackgroundColor => base.BackgroundColor;

	// Use SetStaticDefaults to assign the display name
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Cursed Jaw");
	}
	public override void Load()
	{
		// On.Terraria.Main.DrawWaters += Main_DrawWaters;
		base.Load();
	}
	// Calculate when the biome is active.
	public override bool IsBiomeActive(Player player)
	{

		bool b1 = TuskBiomeSky.Open;
		/*if(b1)
            {
			MythMod.MiscItems.Projectiles.Weapon.Fragrans.Fragrans.ZoneTusk = 2;
		}*/
		return b1;
	}
	public override void OnInBiome(Player player)
	{
		base.OnInBiome(player);
	}
}
