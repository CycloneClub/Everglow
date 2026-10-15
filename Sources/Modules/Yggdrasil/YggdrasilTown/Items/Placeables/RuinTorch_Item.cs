using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class RuinTorch_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 100;

		ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
		ItemID.Sets.SingleUseInGamepad[Type] = true;
		ItemID.Sets.Torches[Type] = true;
	}

	public override void SetDefaults()
	{
		// DefaultToTorch sets various properties common to torch placing items. Hover over DefaultToTorch in Visual Studio to see the specific properties set.
		// Of particular note to torches are Item.holdStyle, Item.flame, and Item.noWet.
		Item.DefaultToTorch(ModContent.TileType<RuinTorch>(), 0, false);
		Item.value = 50;
	}

	public override void HoldItem(Player player)
	{
		// This torch cannot be used in water, so it shouldn't spawn particles or light either
		if (player.wet)
		{
			return;
		}

		// Note that due to biome select torch god's favor, the player may not actually have an ExampleTorch in their inventory when this hook is called, so no modifications should be made to the item instance.

		// Randomly spawn sparkles when the torch is held. Bigger chance to spawn them when swinging the torch.
		if (Main.rand.NextBool(player.itemAnimation > 0 ? 7 : 30))
		{
			Dust dust = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -16f : 6f), player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<RuinTorchDust>(), 0f, 0f, 100);
			if (!Main.rand.NextBool(3))
			{
				dust.noGravity = true;
			}
			dust.scale *= 0.6f;
			dust.velocity *= 0.3f;
			dust.velocity.Y -= 2f;
			dust.position = player.RotatedRelativePoint(dust.position);
		}

		// Create a white (1.0, 1.0, 1.0) light at the torch's approximate position, when the item is held.
		Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

		Lighting.AddLight(position, 0.5f, 1.1f, 1.1f);
	}

	public override void PostUpdate()
	{
		// Create a white (1.0, 1.0, 1.0) light when the item is in world, and isn't underwater.
		if (!Item.wet)
		{
			Lighting.AddLight(Item.Center, 0.5f, 1.1f, 1.1f);
		}
	}
}
