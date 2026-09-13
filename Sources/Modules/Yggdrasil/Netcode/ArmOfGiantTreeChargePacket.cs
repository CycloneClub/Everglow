using Everglow.Commons.Netcode.Abstracts;
using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;

namespace Everglow.Yggdrasil.Netcode;

/// <summary>
/// Carries the owning client's <see cref="ArmOfGiantTree"/> charge to the authoritative
/// side (whoAmI's own player), plus a release flag that signals a full-charge shockwave.
/// </summary>
public class ArmOfGiantTreeChargePacket : IPacket
{
	public int Charge = 0;

	public bool ReleaseSmash = false;

	public void Receive(BinaryReader reader, int whoAmI)
	{
		Charge = reader.ReadInt32();
		ReleaseSmash = reader.ReadBoolean();
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(Charge);
		writer.Write(ReleaseSmash);
	}

	[HandlePacket(typeof(ArmOfGiantTreeChargePacket))]
	public class ArmOfGiantTreeChargePacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			var packetData = (ArmOfGiantTreeChargePacket)packet;
			Player player = Main.player[whoAmI];
			if (player is null || !player.active)
			{
				return;
			}

			var mp = player.GetModPlayer<KelpCurtainPlayer>();

			// Untrusted input: clamp to the legal charge window.
			mp.ArmOfGiantTreeCharge = System.Math.Clamp(packetData.Charge, 0, ArmOfGiantTree.MaxChargeFrames);

			if (packetData.ReleaseSmash && ArmOfGiantTree.IsHeldBy(player))
			{
				// Authoritative shockwave: the sender must actually hold the item (spoof rejection).
				ArmOfGiantTree.ApplyShockwave(player);
				mp.ArmOfGiantTreeCharge = 0;
			}
		}
	}
}
