namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

public class WitheredMask : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Vanity;

	public override string Texture => $"Terraria/Images/Item_{ItemID.TikiMask}";

	public override void Load()
	{
		if (Main.dedServ)
		{
			return;
		}

		EquipLoader.AddEquipTexture(Mod, $"Terraria/Images/Armor_Head_{ArmorIDs.Head.TikiMask}", EquipType.Head, this, nameof(WitheredMask));
	}

	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 24;
		Item.value = Item.buyPrice(silver: 15);
		Item.rare = ItemRarityID.Blue;
		Item.vanity = true;
		Item.headSlot = EquipLoader.GetEquipSlot(Mod, nameof(WitheredMask), EquipType.Head);
	}
}
