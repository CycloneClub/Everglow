using Everglow.Yggdrasil.KelpCurtain.Items.Armors.Molluscs;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;
using Everglow.Yggdrasil.Netcode;
using static Terraria.Player;

namespace Everglow.Yggdrasil.KelpCurtain;

public class KelpCurtainPlayer : ModPlayer
{
	/// <summary>
	/// <see cref="Items.Armors.Molluscs.MolluscsLeggings"/>
	/// </summary>
	public bool MolluscsLeggings { get; set; }

	/// <summary>
	/// Set buff symbol of <see cref="Items.Armors.Molluscs.MolluscsLeggings"/>.
	/// </summary>
	public bool MolluscsSetBuff { get; set; }

	/// <summary>
	/// <see cref="Items.Accessories.RadialCarapace"/>
	/// </summary>
	public bool RadialCarapace { get; set; }

	/// <summary>
	/// <see cref="Items.Accessories.CorrodedPearl"/>
	/// </summary>
	public bool CorrodedPearl { get; set; }

	/// <summary>
	/// Charged-smash accumulator for <see cref="ArmOfGiantTree"/>, bounded 0 to
	/// <see cref="ArmOfGiantTree.MaxChargeFrames"/>. Transient combat state: it lives
	/// on the per-player ModPlayer (never on the shared ModItem) and is not persisted.
	/// </summary>
	public int ArmOfGiantTreeCharge { get; set; }

	/// <summary>
	/// The <see cref="Player.selectedItem"/> slot index of the stack that accumulated the
	/// current <see cref="ArmOfGiantTreeCharge"/>; the -1 sentinel forces adoption of the
	/// held stack on first touch. Two same-type stacks share <c>Item.type</c> but occupy
	/// different slots, so this discriminator is what isolates their charge.
	/// </summary>
	public int ArmOfGiantTreeChargedSlot { get; set; } = -1;

	public override void ResetEffects()
	{
		MolluscsLeggings = false;
		MolluscsSetBuff = false;
		RadialCarapace = false;
		CorrodedPearl = false;
	}

	public override void FrameEffects()
	{
		if (Player.head == EquipLoader.GetEquipSlot(Mod, nameof(MossyMolluscsHelmet), EquipType.Head)
			&& Player.body == EquipLoader.GetEquipSlot(Mod, nameof(ShellMolluscsBreastPlate), EquipType.Body))
		{
			Player.body = EquipLoader.GetEquipSlot(Mod, ShellMolluscsBreastPlate.AltTextureKey, EquipType.Body);
		}
	}

	public override void UpdateEquips()
	{
		if (Player.wet)
		{
			float multiplier = 1f
				+ (MolluscsSetBuff ? 0.3f : 0f)
				+ (MolluscsLeggings ? 0.35f : 0f)
				+ (RadialCarapace ? 0.35f : 0f)
				+ (CorrodedPearl ? 0.2f : 0f);

			Player.runAcceleration *= multiplier;
			Player.maxRunSpeed *= multiplier;
		}
	}

	public override void CopyClientState(ModPlayer targetCopy)
	{
		var clone = (KelpCurtainPlayer)targetCopy;
		clone.ArmOfGiantTreeCharge = ArmOfGiantTreeCharge;
	}

	public override void SendClientChanges(ModPlayer clientPlayer)
	{
		var clone = (KelpCurtainPlayer)clientPlayer;
		if (ArmOfGiantTreeCharge != clone.ArmOfGiantTreeCharge)
		{
			// Mirror Everglow.Yggdrasil.Common.YggdrasilPlayer: send the change to the
			// world (server and other clients), ignoring the local client.
			ModIns.PacketResolver.Send(
				new ArmOfGiantTreeChargePacket()
				{
					Charge = ArmOfGiantTreeCharge,
					ReleaseSmash = false,
				}, toClient: -1, ignoreClient: Main.myPlayer);
		}
	}
}
