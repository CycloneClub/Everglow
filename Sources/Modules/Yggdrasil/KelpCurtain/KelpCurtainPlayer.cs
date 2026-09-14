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
	/// <see cref="Items.Armors.CrimsonMoonAlgae.CrimsonMoonAlgaeBreastPlate"/>
	/// </summary>
	public bool CrimsonMoonAlgaeBreastPlate { get; set; }

	/// <summary>
	/// <see cref="Items.Armors.CrimsonMoonAlgae.CrimsonMoonAlgaeGreaves"/>
	/// </summary>
	public bool CrimsonMoonAlgaeGreaves { get; set; }

	/// <summary>
	/// Set-buff flag of the 红月水藻 armor set, set by
	/// <see cref="Items.Armors.CrimsonMoonAlgae.CrimsonMoonAlgaeHeaddress"/> and
	/// <see cref="Items.Armors.CrimsonMoonAlgae.CrimsonMoonAlgaeMask"/> when the full set is worn.
	/// </summary>
	public bool CrimsonMoonAlgaeSetBuff { get; set; }

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
		CrimsonMoonAlgaeBreastPlate = false;
		CrimsonMoonAlgaeGreaves = false;
		CrimsonMoonAlgaeSetBuff = false;
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
				+ (CorrodedPearl ? 0.2f : 0f)
				+ (CrimsonMoonAlgaeGreaves ? 0.24f : 0f); // Design: 护胫 水下额外增加24%

			Player.runAcceleration *= multiplier;
			Player.maxRunSpeed *= multiplier;
		}
	}

	/// <summary>
	/// The 红月水藻 breastplate heals 15% of any single hit of 10 or more damage
	/// (design: 受到大于等于10的伤害时治疗该伤害的15%). PostHurt runs after health is
	/// reduced, so the heal is not swallowed by the max-life clamp, and Player.Heal applies
	/// the clamp and synchronizes the life value; no extra client-side gating is required.
	/// </summary>
	public override void PostHurt(Player.HurtInfo info)
	{
		if (CrimsonMoonAlgaeBreastPlate && info.Damage >= 10)
		{
			Player.Heal((int)(info.Damage * 0.15f));
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
