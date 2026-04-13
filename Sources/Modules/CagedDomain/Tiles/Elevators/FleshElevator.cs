using Everglow.Commons.Templates.Furniture.Elevator;

namespace Everglow.CagedDomain.Tiles.Elevators;

public class FleshElevator : ElevatorBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		LightSourceColor = new Vector3(0.3f, 0.7f, 1f);
	}
}
