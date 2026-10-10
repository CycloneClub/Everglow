using Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae;
using Everglow.Yggdrasil.KelpCurtain.Items.Armors.DevilHeart;
using Everglow.Yggdrasil.KelpCurtain.Items.Armors.Molluscs;
using Everglow.Yggdrasil.KelpCurtain.Items.Armors.Ruin;
using Everglow.Yggdrasil.KelpCurtain.Items.Armors.Witherbark;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Auburn;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.BloodCultist;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.CyanVine;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.LampWood;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.LightSeeker;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Rock;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Trainee;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Twilight;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Valiant;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Everglow.Yggdrasil.Common;

public class YggdrasilArmorSetSystem : ModSystem
{
	private readonly List<ArmorSetBonus> registeredSets = [];

	public override void PostSetupContent()
	{
		Register<WitherbarkHelmet, WitherbarkBreastPlate, WitherbarkLeggings>();
		Register<RuinMask, RuinMagicRobe, RuinLeggings>();
		Register<MossyMolluscsHelmet, ShellMolluscsBreastPlate, MolluscsLeggings>();
		Register<PearlMolluscsHelmet, ShellMolluscsBreastPlate, MolluscsLeggings>();
		Register<DevilHeartHairpin, DevilHeartLightBreastPlate, DevilHeartLeggings>(hasDescription: true);
		Register<DevilHeartHelmet, DevilHeartLightBreastPlate, DevilHeartLeggings>(hasDescription: true);
		Register<CrimsonMoonAlgaeMask, CrimsonMoonAlgaeBreastPlate, CrimsonMoonAlgaeGreaves>();
		Register<CrimsonMoonAlgaeHeaddress, CrimsonMoonAlgaeBreastPlate, CrimsonMoonAlgaeGreaves>();
		Register(ModContent.GetInstance<BloodStainedVeil>(), ModContent.GetInstance<BloodDrenchedRobe>());
		Register<AuburnHoodie, AuburnBreastplate, AuburnBoots>();
		Register<LampWoodHelmet, LampWoodBreastplate, LampWoodLeggings>();
		Register<CyanWarhelm, CyanBreastplate, CyanLeggings>();
		Register<CyanHeavylet, CyanBreastplate, CyanLeggings>();
		Register<RockHelmet, RockPlateMail, RockGreaves>();
		Register<ValiantHelmet, ValiantBreastplate, ValiantLeggings>();
		Register<SafetyHelmets, ConventionalEquipment, StandardLeggings>();
		Register<TwilightWoodHelmet, TwilightWoodBreastplate, TwilightWoodLeggings>();
		Register<OvineHairpin, ConcentratingJacket, ShadowlessBoots>();
		Register<ExplorationHelmet, ConcentratingJacket, ShadowlessBoots>();
		ArmorSetBonuses.BuildLookup();
	}

	public override void Unload()
	{
		foreach (var bonus in registeredSets)
		{
			ArmorSetBonuses.All.Remove(bonus);
		}
		registeredSets.Clear();
	}

	private void Register<THead, TBody, TLegs>(bool hasDescription = false)
		where THead : ModItem
		where TBody : ModItem
		where TLegs : ModItem
	{
		Register(ModContent.GetInstance<THead>(), ModContent.GetInstance<TBody>(), ModContent.GetInstance<TLegs>(), hasDescription);
	}

	private void Register(ModItem head, ModItem body, ModItem legs = null, bool hasDescription = false)
	{
		var description = hasDescription
			? head.GetLocalization(LocalizationUtils.LocalizationKeys.SetBonus)
			: LocalizedText.Empty;
		var bonus = CreateSet(head, body, legs, description);
		ArmorSetBonuses.All.Add(bonus);
		registeredSets.Add(bonus);
	}

	internal static ArmorSetBonus CreateSet(ModItem head, ModItem body, ModItem legs, LocalizedText description)
	{
		return new ArmorSetBonus
		{
			Head = head.Type,
			Body = body.Type,
			Legs = legs?.Type ?? 0,
			Description = description,
			PrimaryPart = ArmorSetBonus.PartType.Head,
			Effect = player =>
			{
				// 1.4.5 selects one registered set; retain the effects of every piece
				// that the legacy ItemLoader armor-set loop applied.
				head.UpdateArmorSet(player);
				body.UpdateArmorSet(player);
				legs?.UpdateArmorSet(player);
			},
		};
	}
}
