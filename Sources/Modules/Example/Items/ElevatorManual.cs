using Everglow.Commons.Utilities;

namespace Everglow.Example.Items;

public class ElevatorManual : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Examples;

	public bool EnableManual = false;

	public Vector2 PlayerPos = Vector2.zeroVector;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.useStyle = ItemUseStyleID.None;
		Item.useAnimation = 15;
		Item.useTime = 15;
		Item.consumable = false;
		Item.maxStack = 1;
		Item.value = 1000;
	}

	public override void UpdateInventory(Player player)
	{
		if (player.HeldItem == Item)
		{
			EnableManual = true;
		}
		else
		{
			EnableManual = false;
		}
		PlayerPos = player.MountedCenter;
		base.UpdateInventory(player);
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (EnableManual)
		{
			Texture2D tex = ModAsset.ElevatorManual_Large.Value;
			spriteBatch.Draw(tex, new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f, null, Lighting.GetColor(PlayerPos.ToTileCoordinates()), 0, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}
		base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
	}
}
