using Everglow.Yggdrasil.Common;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.Molluscs;

[AutoloadEquip(EquipType.Body)]
public class ShellMolluscsBreastPlate : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public const int BuffDuration = 25 * 60;
	public const int CooldownDuration = 40 * 60;

	// The equip texture name for the alternate equip texture.
	private const string AltTextureName = "Blue";
	public const string AltTextureKey = nameof(ShellMolluscsBreastPlate) + AltTextureName;

	public override void Load()
	{
		if (Main.dedServ)
		{
			return;
		}

		// Add a special equip texture by providing a custom name reference instead of an item reference
		EquipLoader.AddEquipTexture(Mod, $"{Texture}_{AltTextureName}_{EquipType.Body}", EquipType.Body, this, AltTextureKey);
	}

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(silver: 60);
		Item.rare = ItemRarityID.Green;
		Item.defense = 5;
	}

	override public void UpdateEquip(Player player)
	{
		player.GetCritChance<GenericDamageClass>() += 6; // Increase critical chance by 6%.
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return (head.type == ModContent.ItemType<MossyMolluscsHelmet>() || head.type == ModContent.ItemType<PearlMolluscsHelmet>())
			&& legs.type == ModContent.ItemType<MolluscsLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.breathEffectiveness += 1f;
		player.GetModPlayer<YggdrasilPlayer>().molluscsSet = true;
	}
}
