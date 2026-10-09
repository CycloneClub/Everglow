using Everglow.Myth.Common;
using Terraria;

namespace Everglow.Myth.Misc.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class SilverWing : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 60;
		Item.height = 26;
		Item.value = 1093;
		Item.accessory = true;
		Item.rare = ItemRarityID.White;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.SilverBar, 14)
			.AddIngredient(ItemID.Ruby, 8)
			.AddTile(TileID.Anvils)
			.Register();
		CreateRecipe()
			.AddIngredient(ItemID.TungstenBar, 14)
			.AddIngredient(ItemID.Ruby, 8)
			.AddTile(TileID.Anvils)
			.Register();
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		MythContentPlayer mplayer = player.GetModPlayer<MythContentPlayer>();
		mplayer.CriticalDamage += 0.08f;
		if (player.controlUseItem)
		{
			noContinueUsingWeaponTime = 0;
		}
		else
		{
			noContinueUsingWeaponTime++;
		}
		if (noContinueUsingWeaponTime >= 180)
		{
			player.GetDamage(DamageClass.Generic) *= 1.4f;
		}
	}
	internal int noContinueUsingWeaponTime = 0;
}
