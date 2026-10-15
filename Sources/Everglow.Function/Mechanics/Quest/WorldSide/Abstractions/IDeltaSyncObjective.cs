namespace Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

public interface IDeltaSyncObjective
{
	bool NeedDeltaSync { get; }

	void SendDelta(BinaryWriter bw);

	void ReceiveDelta(BinaryReader br);

	void SendMain(BinaryWriter bw);

	void ReceiveMain(BinaryReader br);
}
