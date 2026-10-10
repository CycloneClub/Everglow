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

	public override void Update(WorldItem item, ref float gravity, ref float maxFallSpeed)
	{
		if (item.velocity.Length() > 0.1f)
		{
			for (float vel = 0f; vel < item.velocity.Length(); vel += 1f)
			{
				if (Main.rand.NextBool(24))
				{
					Dust.NewDustDirect(item.position - Vector2.Normalize(item.velocity) * vel, item.width, item.height, ModContent.DustType<FireButterflyShimmer>());
				}
			}
		}
		if ((item.oldVelocity - item.velocity).Length() > 2f)
		{
			for (float vel = 0f; vel < (item.oldVelocity - item.velocity).Length(); vel += 0.1f)
			{
				if (Main.rand.NextBool(4))
				{
					var d = Dust.NewDustDirect(item.position - Vector2.Normalize(item.velocity) * vel, item.width, item.height, ModContent.DustType<FireButterflyShimmer>());
					d.velocity = new Vector2(0, (item.oldVelocity - item.velocity).Length() * Main.rand.NextFloat(0.85f, 1.15f)).RotatedByRandom(6.283f);
				}
			}
		}
		item.oldVelocity = item.velocity;
		base.Update(item, ref gravity, ref maxFallSpeed);
	}
}
