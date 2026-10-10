using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Rock;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.BossDrops;

public class SquamousShellTreasureBag : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TreasureBags;

	public override void SetDefaults()
	{
		Item.maxStack = 999;
		Item.consumable = true;
		Item.width = 32;
		Item.height = 32;
		Item.rare = ItemRarityID.Cyan;
		Item.expert = true;
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void RightClick(Player player)
	{
		switch (Main.rand.Next(6))
		{
			case 0:
				player.QuickSpawnItem(null, ModContent.ItemType<EyeOfAnabiosis>(), 1);
				break;
			case 1:
				player.QuickSpawnItem(null, ModContent.ItemType<FlurryingBlades>(), 1);
				break;
			case 2:
				player.QuickSpawnItem(null, ModContent.ItemType<RockSpikeBallista>(), 1);
				break;
			case 3:
				player.QuickSpawnItem(null, ModContent.ItemType<DeadBeetleEgg>(), 1);
				break;
			case 4:
				player.QuickSpawnItem(null, ModContent.ItemType<DragonScaleHammer>(), 1);
				break;
			case 5:
				player.QuickSpawnItem(null, ModContent.ItemType<BlueyWings>(), 1);
				break;
		}
		switch (Main.rand.Next(3))
		{
			case 0:
				player.QuickSpawnItem(null, ModContent.ItemType<RockGreaves>(), 1);
				break;
			case 1:
				player.QuickSpawnItem(null, ModContent.ItemType<RockHelmet>(), 1);
				break;
			case 2:
				player.QuickSpawnItem(null, ModContent.ItemType<RockPlateMail>(), 1);
				break;
		}
		player.QuickSpawnItem(null, ItemID.GoldCoin, 15);
		base.RightClick(player);
	}

	private int myLightTimer = 0;

	public override bool PreDrawInWorld(WorldItem worldItem, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D t = ModAsset.SquamousShellTreasureBag.Value;
		for (int i = 0; i < 4; i++)
		{
			Vector2 v = new Vector2(0, 8 * ((float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 6.28318548f / 4f)) * 0.3f + 0.7f)).RotatedBy((double)(Main.GlobalTimeWrappedHourly * 6.28318548f / 4f) + MathHelper.Pi * i / 2d);

			// spriteBatch.Draw(t, worldItem.Center, new Rectangle(0,0,32,32), new Color(100, 100, 100, 0), 0, new Vector2(16, 16), 3f, SpriteEffects.None, 1);
			Main.EntitySpriteDraw(t, worldItem.Center - Main.screenPosition + v, null, new Color(100, 100, 100, 0), 0, new Vector2(16, 16), 1f, SpriteEffects.None, 0);
		}
		if (!Main.gamePaused && myLightTimer % 20 == 19)
		{
			myLightTimer = 0;
			int num37 = Dust.NewDust(worldItem.Center + new Vector2(Main.rand.Next(-16, 6), 0), 0, 0, DustID.SilverCoin, 0f, 0f, 254, Color.White, 1f);
			Main.dust[num37].velocity = new Vector2(0, -Main.rand.NextFloat(0.3f, 0.9f));
			Main.dust[num37].rotation = 0;
			Main.dust[num37].noLight = true;
			Main.dust[num37].alpha = 240;
		}
		if (!Main.gamePaused)
		{
			myLightTimer++;
			Lighting.AddLight((int)(worldItem.Center.X / 16f), (int)(worldItem.Center.Y / 16f), 0.5f, 0.15f, 0.0f);
		}

		return true;
	}
}
