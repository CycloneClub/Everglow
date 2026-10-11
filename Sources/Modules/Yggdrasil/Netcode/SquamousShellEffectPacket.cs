using Everglow.Commons.Netcode.Abstracts;
using Boss = Everglow.Yggdrasil.YggdrasilTown.NPCs.SquamousShell.SquamousShell;

namespace Everglow.Yggdrasil.Netcode;

public enum SquamousShellEffect : byte
{
	WingBurst,
	SmashImpact,
	GroundImpact,
	Landing,
	DashImpact,
	SmallRockThrow,
	RockThrow,
	RockLift,
	RockRelease,
	FlyingRockRelease,
}

public class SquamousShellEffectPacket : IPacket
{
	public int NPCIndex;
	public SquamousShellEffect Effect;
	public Vector2 Position;
	public float Scale = 1f;

	public void Send(BinaryWriter writer)
	{
		writer.Write(NPCIndex);
		writer.Write((byte)Effect);
		writer.Write(Position.X);
		writer.Write(Position.Y);
		writer.Write(Scale);
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		NPCIndex = reader.ReadInt32();
		Effect = (SquamousShellEffect)reader.ReadByte();
		Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		Scale = reader.ReadSingle();
	}

	[HandlePacket(typeof(SquamousShellEffectPacket))]
	public class Handler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient || Main.dedServ)
			{
				return;
			}
			var effect = (SquamousShellEffectPacket)packet;
			if (effect.NPCIndex >= 0 && effect.NPCIndex < Main.maxNPCs
				&& Main.npc[effect.NPCIndex] is { active: true, ModNPC: Boss boss })
			{
				boss.QueueEffect(effect);
			}
		}
	}
}
