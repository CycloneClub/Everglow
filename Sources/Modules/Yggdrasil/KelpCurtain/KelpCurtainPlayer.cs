using Everglow.Yggdrasil.KelpCurtain.Items.Armors.Molluscs;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;

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
	/// <see cref="Items.Accessories.CorrodedPearl"/>
	/// </summary>
	public bool CorrodedPearl { get; set; }

	public bool CrimsonMoonAlgaeBreastPlate { get; set; }

	public bool CrimsonMoonAlgaeGreaves { get; set; }

	public bool CrimsonMoonAlgaeSetBuff { get; set; }

	public int ArmOfGiantTreeRequestWait { get; set; }

	public override void ResetEffects()
	{
		MolluscsLeggings = false;
		MolluscsSetBuff = false;
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
		if (!Player.wet)
		{
			return;
		}

		// While wet:
		// +30% movement speed while Aquatic Possession is active
		// +35% movement speed with Molluscs Leggings
		// +20% movement speed with Corroded Pearl
		// +24% movement speed with Crimson Moon Algae Greaves
		float waterMovementBonus = (MolluscsSetBuff ? 0.3f : 0f)
			+ (MolluscsLeggings ? 0.35f : 0f)
			+ (CorrodedPearl ? 0.2f : 0f)
			+ (CrimsonMoonAlgaeGreaves ? 0.24f : 0f);
		if (waterMovementBonus != 0)
		{
			Player.moveSpeed += waterMovementBonus;
		}
	}

	public override void PostHurt(Player.HurtInfo info)
	{
		if (CrimsonMoonAlgaeBreastPlate && info.Damage >= 10)
		{
			Player.Heal((int)(info.Damage * 0.15f));
		}
	}

	public override void PostUpdate()
	{
		if (Player.dead || Player.HeldItem.type != ModContent.ItemType<ArmOfGiantTree>())
		{
			ArmOfGiantTreeRequestWait = 0;
		}
		else if (ArmOfGiantTreeRequestWait > 0)
		{
			ArmOfGiantTreeRequestWait--;
		}
	}
}
