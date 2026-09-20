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

	public bool CrimsonMoonAlgaeBreastPlate { get; set; }

	public bool CrimsonMoonAlgaeGreaves { get; set; }

	public bool CrimsonMoonAlgaeSetBuff { get; set; }

	public int ArmOfGiantTreeCharge { get; set; }

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
				+ (CrimsonMoonAlgaeGreaves ? 0.24f : 0f);

			Player.runAcceleration *= multiplier;
			Player.maxRunSpeed *= multiplier;
		}
	}

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
			ModIns.PacketResolver.Send(
				new ArmOfGiantTreeChargePacket()
				{
					Charge = ArmOfGiantTreeCharge,
					ReleaseSmash = false,
				}, toClient: -1, ignoreClient: Main.myPlayer);
		}
	}
}
