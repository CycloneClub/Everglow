using Everglow.Commons.Templates.Pylon;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader.Default;

namespace Everglow.Example.Pylon;

/// <summary>
/// Example of infinite placement pylon that uses the EverglowPylon template.
/// </summary>
public class ExamplePylon : EverglowPylonBase<ExamplePylonTileEntity>
{
	public override void SetStaticDefaults()
	{
		AddMapEntry(new Color(148, 148, 148));
		base.SetStaticDefaults();
	}

	public override bool CanPlacePylon() => true;

	public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) => true;

	public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => true;

	public override void ValidTeleportCheck_DestinationPostCheck(TeleportPylonInfo destinationPylonInfo, ref bool destinationPylonValid, ref string errorKey) => base.ValidTeleportCheck_DestinationPostCheck(destinationPylonInfo, ref destinationPylonValid, ref errorKey);

	public override void ValidTeleportCheck_NearbyPostCheck(TeleportPylonInfo nearbyPylonInfo, ref bool destinationPylonValid, ref bool anyNearbyValidPylon, ref string errorKey) => base.ValidTeleportCheck_NearbyPostCheck(nearbyPylonInfo, ref destinationPylonValid, ref anyNearbyValidPylon, ref errorKey);

	public override void ModifyTeleportationPosition(TeleportPylonInfo destinationPylonInfo, ref Vector2 teleportationPosition) => base.ModifyTeleportationPosition(destinationPylonInfo, ref teleportationPosition);
}
