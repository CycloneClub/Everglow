using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

namespace Everglow.Yggdrasil.Common;

public class YggdrasilBiomeTileCounter : ModSystem
{
	public int DarkForestGrassCount;
	public int TwilightGrassCount;
	public int JellyBallSecretionCount;

	public int WaterSluiceRoomCount;

	public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
	{
		DarkForestGrassCount = tileCounts[ModContent.TileType<DarkForestGrass>()];
		TwilightGrassCount = tileCounts[ModContent.TileType<TwilightGrassBlock>()];
		JellyBallSecretionCount = tileCounts[ModContent.TileType<JellyBallSecretion>()];

		WaterSluiceRoomCount = tileCounts[ModContent.TileType<WaterSluice_Scene>()];
	}
}
