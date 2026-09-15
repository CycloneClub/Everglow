namespace Everglow.Yggdrasil.KelpCurtain.Items.Consumables;

public class VampireMatTreasureBag : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.TreasureBags;

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
		player.QuickSpawnItem(null, ItemID.GoldCoin, 15);
		base.RightClick(player);
	}

	private int myLightTimer = 0;

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D t = ModAsset.SquamousShellTreasureBag.Value;
		for (int i = 0; i < 4; i++)
		{
			Vector2 v = new Vector2(0, 8 * ((float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 6.28318548f / 4f)) * 0.3f + 0.7f)).RotatedBy((double)(Main.GlobalTimeWrappedHourly * 6.28318548f / 4f) + MathHelper.Pi * i / 2d);
			Main.EntitySpriteDraw(t, Item.Center - Main.screenPosition + v, null, new Color(100, 100, 100, 0), 0, new Vector2(16, 16), 1f, SpriteEffects.None, 0);
		}
		if (!Main.gamePaused && myLightTimer % 20 == 19)
		{
			myLightTimer = 0;
			int num37 = Dust.NewDust(Item.Center + new Vector2(Main.rand.Next(-16, 6), 0), 0, 0, DustID.SilverCoin, 0f, 0f, 254, Color.White, 1f);
			Main.dust[num37].velocity = new Vector2(0, -Main.rand.NextFloat(0.3f, 0.9f));
			Main.dust[num37].rotation = 0;
			Main.dust[num37].noLight = true;
			Main.dust[num37].alpha = 240;
		}
		if (!Main.gamePaused)
		{
			myLightTimer++;
			Lighting.AddLight((int)(Item.Center.X / 16f), (int)(Item.Center.Y / 16f), 0.5f, 0.15f, 0.0f);
		}
		return true;
	}
}
