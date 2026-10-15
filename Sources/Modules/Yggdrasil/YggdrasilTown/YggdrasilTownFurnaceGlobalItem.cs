using System.Collections.ObjectModel;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;
using Everglow.Yggdrasil.YggdrasilTown.UI;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Everglow.Yggdrasil.YggdrasilTown;

public class YggdrasilTownFurnaceGlobalItem : GlobalItem
{
	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (!FurnaceScoreShopUI.Instance.IsVisible || item.type == ItemID.None)
		{
			base.ModifyTooltips(item, tooltips);
			return;
		}

		// TODO: Need a modded itemTooltip allocating feature. Now use 36 for Furnace Score Shop Itemslot.
		if (FurnaceScoreShop.SellPricesInFurnaceScore.ContainsKey(item.type) && item.tooltipContext == 36)
		{
			tooltips.Add(new TooltipLine(Mod, "FurnaceScoreShopItem", $"Sell Price:   {FurnaceScoreShop.SellPricesInFurnaceScore[item.type]}") { OverrideColor = new Color(1f, 0.4f, 0.05f, 1f) });
		}
	}

	public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
	{
		//if (!FurnaceScoreShopUI.Instance.IsVisible || item.type == ItemID.None)
		//{
		//	return base.PreDrawTooltip(item, lines, ref x, ref y);
		//}
		//if (FurnaceScoreShop.SellPricesInFurnaceScore.ContainsKey(item.type) && item.tooltipContext == 36)
		//{
		//	Vector2 size = ChatManager.GetStringSize(FontAssets.MouseText.Value, lines[^1].Text, Vector2.One);
		//	Texture2D tex = ModAsset.FurnaceScoreIcon.Value;
		//	Main.spriteBatch.Draw(tex, new Vector2(x + size.X, y), null, Color.White, 0, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		//}
		return base.PreDrawTooltip(item, lines, ref x, ref y);
	}

	public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
	{
		if (!FurnaceScoreShopUI.Instance.IsVisible || item.type == ItemID.None)
		{
			return base.PreDrawTooltipLine(item, line, ref yOffset);
		}
		if (FurnaceScoreShop.SellPricesInFurnaceScore.ContainsKey(item.type) && item.tooltipContext == 36 && line.Text.Contains("Price"))
		{
			Vector2 size = ChatManager.GetStringSize(line.Font, "Sell Price:", Vector2.One);
			Texture2D tex = ModAsset.FurnaceScoreIcon.Value;
			Main.spriteBatch.Draw(tex, new Vector2(line.X + size.X + 10, line.Y + 8), null, Color.White, 0, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}
		return base.PreDrawTooltipLine(item, line, ref yOffset);
	}
}
