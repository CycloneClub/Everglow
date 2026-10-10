using System.Collections.ObjectModel;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Everglow.AssetReplace;

public class ItemTooltipGlobal : GlobalItem
{
	private static int currentYOffset;
	private static int globalYOffset;

	public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
	{
		currentYOffset = 0;
		globalYOffset = 0;

		if (ModContent.GetInstance<AssetReplaceConfig>().TextureReplace == TextureReplaceMode.Terraria)
		{
			return base.PreDrawTooltip(item, lines, ref x, ref y);
		}

		// 原版代码
		Vector2 zero = Vector2.Zero;
		bool yoyoLogo = lines.Any(l => l.Name == "OneDropLogo");
		List<DrawableTooltipLine> drawableLines = lines.Select((x, i) => new DrawableTooltipLine(x, i, 0, 0, Color.White)).ToList();
		for (int j = 0; j < drawableLines.Count; j++)
		{
			Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, drawableLines[j].Text, Vector2.One);
			if (stringSize.X > zero.X)
			{
				zero.X = stringSize.X;
			}

			zero.Y += stringSize.Y;
		}
		if (yoyoLogo)
		{
			zero.Y += 24f;
		}

		// 原版代码
		int borderWidth = 14;
		int borderHeight = 9;
		var invBG = new Rectangle(x - borderWidth, y - borderHeight, (int)zero.X + borderWidth * 2, (int)zero.Y + borderHeight + borderHeight / 2);
		// 计算物品大小，调节提示框
		Texture2D tex = TextureAssets.Item[item.type].Value;
		Rectangle frame = Utils.Frame(TextureAssets.Item[item.type].Value, 1, 1, 0, 0);
		if (Main.itemAnimations[item.type] is not null)
		{
			frame = Main.itemAnimations[item.type].GetFrame(tex);
		}

		Vector2 itemSize = Utils.Size(frame);
		// 修改位置和框大小
		currentYOffset = (int)itemSize.Y + 34;
		invBG.Height += currentYOffset;

		if (drawableLines.Count > 1)
		{
			float scaleY = Math.Min((float)Main.screenHeight / (drawableLines[^1].Y + 11 - drawableLines[0].Y), 1);
			var vec = new Vector3(drawableLines[^1].X, Main.screenHeight, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
				DepthStencilState.Default, RasterizerState.CullNone,
				null,
				Matrix.CreateTranslation(-vec) *
				Matrix.CreateScale(scaleY, scaleY, 1) *
				Matrix.CreateTranslation(vec) *
				Matrix.CreateTranslation(-Math.Min(drawableLines[0].X - 11, 0), 0, 0) *
				Main.UIScaleMatrix);
		}

		// 框在屏幕里面
		if (x + zero.X + borderWidth * 2 > Main.screenWidth)
		{
			x = (int)(Main.screenWidth - zero.X - borderWidth * 2);
		}

		if (y + zero.Y + currentYOffset + borderHeight * 2 > Main.screenHeight)
		{
			y = (int)(Main.screenHeight - zero.Y - currentYOffset - borderHeight * 2);
		}

		invBG.X = x - borderWidth;
		invBG.Y = y - borderHeight;
		Vector2 drawCenter = new(invBG.Center.X, invBG.Y + frame.Height / 2f + 50f);
		Vector2 drawOrigin = frame.Size() / 2f;
		Color drawColor = Color.White;
		// 原版用的是这个颜色
		// drawColor = new Color(23, 25, 81, 255) * 0.925f;
		if (Main.SettingsEnabled_OpaqueBoxBehindTooltips)
		{
			Utils.DrawInvBG(Main.spriteBatch, invBG, drawColor);
		}
		else
		{
			drawCenter.Y -= 24;
		}

		Vector2 drawPosition = drawCenter - drawOrigin;
		Vector2 endPosition = drawCenter + drawOrigin;

		// 绘制物品
		Main.spriteBatch.Draw(tex, drawPosition, new Rectangle?(frame), Color.White);

		// 绘制物品贴图和名字、简介的分割线
		if (Main.SettingsEnabled_OpaqueBoxBehindTooltips)
		{
			Color lineColor = new(78, 178, 200);
			Vector2 sepLineStart1 = new(invBG.X + 16f, drawPosition.Y - 12f);
			Vector2 sepLineEnd1 = new(invBG.Right - 18f, drawPosition.Y - 12f);
			Vector2 sepLineStart2 = new(invBG.X + 16f, endPosition.Y + 12f);
			Vector2 sepLineEnd2 = new(invBG.Right - 18f, endPosition.Y + 12f);
			DrawLine(Main.spriteBatch, sepLineStart1, sepLineEnd1, lineColor, 2);
			DrawLine(Main.spriteBatch, sepLineStart2, sepLineEnd2, lineColor, 2);
		}

		return base.PreDrawTooltip(item, lines, ref x, ref y);
	}

	public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
	{
		if (ModContent.GetInstance<AssetReplaceConfig>().TextureReplace != TextureReplaceMode.Terraria)
		{
			// 对于除了名字以外每一行，调整每行的y偏移，没有开悬停文本框的话名字也会显示在贴图下方，不然很怪
			if (line.Name != "ItemName" || line.Mod != "Terraria" || !Main.SettingsEnabled_OpaqueBoxBehindTooltips)
			{
				line.Y += currentYOffset;
			}

			line.Y -= globalYOffset;
		}
		return base.PreDrawTooltipLine(item, line, ref yOffset);
	}

	public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width)
	{
		float num = Vector2.Distance(start, end);
		Vector2 vector = (end - start) / num;
		Vector2 value = start;
		float rotation = vector.ToRotation();
		float scale = width / 16f;
		for (float num2 = 0f; num2 <= num; num2 += width)
		{
			spriteBatch.Draw(TextureAssets.BlackTile.Value, value, null, color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
			value = start + num2 * vector;
		}
	}
}
