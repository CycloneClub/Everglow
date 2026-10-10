using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.LampWood;

public class LampWoodHammer : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 64;
		Item.height = 62;
		Item.useAnimation = 30;
		Item.useTime = 30;
		Item.knockBack = 5.5f;
		Item.damage = 13;
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
		Item.value = 100;
		Item.hammer = 45;
	}

	public override void MeleeEffects(Player player, Rectangle hitbox)
	{
		var d = Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, ModContent.DustType<LampWood_Dust_fluorescent_appear>());
		d.velocity = new Vector2(0, Main.rand.NextFloat(4f)).RotatedByRandom(6.283);
		d.rotation = Main.rand.NextFloat(0.4f, 1.2f);
		base.MeleeEffects(player, hitbox);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<LampWood_Wood>(), 8)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
