using Everglow.Commons.Templates.Furniture.Elevator;

namespace Everglow.CagedDomain.Tiles.Elevators;

public class PearlwoodElevator : ElevatorBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		LightSourceColor = new Vector3(0.2f, 0.7f, 0.4f);
	}
}
