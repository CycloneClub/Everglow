using Everglow.Myth.TheFirefly.Dusts;

namespace Everglow.Myth.TheFirefly.Items.Materials;

public class MothScaleDust : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.maxStack = Item.CommonMaxStack;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<GlowingFirefly>(), 1);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}

	public override void Update(ref float gravity, ref float maxFallSpeed)
	{
		if (Item.velocity.Length() > 0.1f)
		{
			for (float vel = 0f; vel < Item.velocity.Length(); vel += 1f)
			{
				if (Main.rand.NextBool(24))
				{
					Dust.NewDustDirect(Item.position - Vector2.Normalize(Item.velocity) * vel, Item.width, Item.height, ModContent.DustType<FireButterflyShimmer>());
				}
			}
		}
		if ((Item.oldVelocity - Item.velocity).Length() > 2f)
		{
			for (float vel = 0f; vel < (Item.oldVelocity - Item.velocity).Length(); vel += 0.1f)
			{
				if (Main.rand.NextBool(4))
				{
					var d = Dust.NewDustDirect(Item.position - Vector2.Normalize(Item.velocity) * vel, Item.width, Item.height, ModContent.DustType<FireButterflyShimmer>());
					d.velocity = new Vector2(0, (Item.oldVelocity - Item.velocity).Length() * Main.rand.NextFloat(0.85f, 1.15f)).RotatedByRandom(6.283f);
				}
			}
		}
		Item.oldVelocity = Item.velocity;
		base.Update(ref gravity, ref maxFallSpeed);
	}
}
