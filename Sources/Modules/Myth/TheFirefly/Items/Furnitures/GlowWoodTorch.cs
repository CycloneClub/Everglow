using Terraria.GameContent.Creative;

namespace Everglow.Myth.TheFirefly.Items.Furnitures;

public class GlowWoodTorch : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 100;
		ItemID.Sets.SingleUseInGamepad[Type] = true;
		ItemID.Sets.Torches[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furnitures.GlowWoodTorch>());
		Item.width = 20;
		Item.height = 20;
	}

	public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
	{
		itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torches;
	}

	public override void HoldItem(Player player)
	{
		if (Main.rand.NextBool(player.itemAnimation > 0 ? 10 : 20))
		{
			var d = Dust.NewDustDirect(new Vector2(player.itemLocation.X + 10f * player.direction - 6, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<Dusts.BlueToPurpleSpark>(), 0, 0, 0, default, Main.rand.NextFloat(0.95f, 1.65f));
			d.velocity.Y = -2;
			d.velocity.X *= 0.05f;
		}
		Lighting.AddLight(player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true, true), 0.7f, 0.06f, 1f);
	}

	public override void PostUpdate()
	{
		Lighting.AddLight(Item.Center, 1f, 1f, 1f);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(3);
		recipe.AddIngredient(ModContent.ItemType<GlowWood>(), 1);
		recipe.AddIngredient(ItemID.Gel, 1);
		recipe.Register();
	}
}
