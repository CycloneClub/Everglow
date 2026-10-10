using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DecayingWoodCourt;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables.DecayingWoodCourt;

public class WitherWoodColdFlameTorch_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 100;
		ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
		ItemID.Sets.SingleUseInGamepad[Type] = true;
		ItemID.Sets.Torches[Type] = true;
		Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
	}

	public override void Update(WorldItem worldItem, ref float gravity, ref float maxFallSpeed)
	{
		base.Update(worldItem, ref gravity, ref maxFallSpeed);
	}

	public override void SetDefaults()
	{
		// DefaultToTorch sets various properties common to torch placing items. Hover over DefaultToTorch in Visual Studio to see the specific properties set.
		// Of particular note to torches are Item.holdStyle, Item.flame, and Item.noWet.
		Item.DefaultToTorch(ModContent.TileType<WitherWoodColdFlameTorch>(), 0, false);
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
		if (player.itemAnimation > 0)
		{
			if (Main.rand.NextBool(7))
			{
				Vector2 offsetPos = new Vector2(player.direction == -1 ? -16f : 6f, -18f * player.gravDir);
				Vector2 animationRotPos = new Vector2(20 * player.direction, -20).RotatedBy(player.itemRotation);
				var dust = Dust.NewDustDirect(offsetPos + animationRotPos + player.itemLocation, 4, 4, ModContent.DustType<WitherWoodTorchDust>(), 0f, 0f, 100);
				dust.scale = Main.rand.NextFloat(0.75f, 0.85f);
				dust.position = player.RotatedRelativePoint(dust.position);
				dust.velocity += animationRotPos.RotatedBy(0) * 0.1f;
				dust.velocity.Y -= 0.4f;
			}
		}
		else
		{
			if (Main.rand.NextBool(240))
			{
				var dust = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -16f : 6f), player.itemLocation.Y - 18f * player.gravDir), 4, 4, ModContent.DustType<WitherWoodTorchDust>(), 0f, 0f, 100);
				dust.scale = Main.rand.NextFloat(0.75f, 0.85f);
				dust.position = player.RotatedRelativePoint(dust.position);
				dust.velocity.X *= 0.1f;
				dust.velocity.Y = -2.4f;
			}
		}

		// Create a white (1.0, 1.0, 1.0) light at the torch's approximate position, when the item is held.
		Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

		Lighting.AddLight(position, 0.668f, 0.088f, 1f);
	}

	public override void PostUpdate(WorldItem worldItem)
	{
		// Create a white (1.0, 1.0, 1.0) light when the worldItem is in world, and isn't underwater.
		Lighting.AddLight(worldItem.Center, 0.7f, 0.6f, 0.1f);
	}

	public override bool PreDrawInWorld(WorldItem worldItem, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		return base.PreDrawInWorld(worldItem, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
}
