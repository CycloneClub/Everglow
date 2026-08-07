namespace Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

public interface IMissionNetcode
{
	public void NetSend(BinaryWriter writer);

	public void NetReceive(BinaryReader reader);

	public void OnMPSync();
}
