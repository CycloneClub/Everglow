using Everglow.Commons.Graphics;
using Everglow.Commons.Utilities;
using ReLogic.Graphics;
using Terraria.GameContent;

namespace Everglow.Commons.Mechanics.Cooldown;

public class CooldownSystem : ModSystem
{
	public const float DefaultCooldownAlpha = 0.6f;

	private static Dictionary<string, float> cooldownAlpha;
	private static bool bossNearby = false;

	public override void Load()
	{
		cooldownAlpha = [];
	}

	public override void Unload()
	{
		cooldownAlpha = null;
	}

	public override void PostDrawInterface(SpriteBatch spriteBatch)
	{
		var p = Main.LocalPlayer;
		var mp = Main.LocalPlayer.GetModPlayer<EverglowPlayer>();

		bossNearby = Main.npc.Any(npc => npc.active && npc.boss);
		UpdateAlpha();

		if (Main.playerInventory)
		{
			return;
		}

		// Draw cooldowns
		Vector2 baseDrawPosition = new Vector2(32, 80) + Vector2.UnitY * 50 * MathF.Ceiling(Main.LocalPlayer.CountBuffs() / 11f);
		Vector2 spacing = Vector2.UnitX * 38f;

		string coolDownTextKey = string.Empty;
		Point drawPos = baseDrawPosition.ToPoint();
		foreach (var (cKey, cooldown) in mp.cooldowns)
		{
			if (!cooldown.cooldown.IsVisible)
			{
				continue;
			}

			coolDownTextKey = DrawCooldownIcon(spriteBatch, p, coolDownTextKey, cKey, drawPos, new Point(Main.mouseX, Main.mouseY));
			drawPos += spacing.ToPoint();
		}

		if (coolDownTextKey != string.Empty)
		{
			var cdText = mp.cooldowns[coolDownTextKey].cooldown;
			Main.instance.MouseText(cdText.DisplayName + "\n" + cdText.Discription);
			Main.LocalPlayer.mouseInterface = true;
		}
	}

	public static void UpdateAlpha()
	{
		var toAdd = Main.LocalPlayer.GetModPlayer<EverglowPlayer>().cooldowns.Where(c => !cooldownAlpha.ContainsKey(c.Key)).Select(o => o.Key);
		foreach (var key in toAdd)
		{
			cooldownAlpha[key] = DefaultCooldownAlpha;
		}

		var toDelete = cooldownAlpha.Where(c => !Main.LocalPlayer.GetModPlayer<EverglowPlayer>().cooldowns.ContainsKey(c.Key)).Select(o => o.Key);
		foreach (var key in toDelete)
		{
			cooldownAlpha.Remove(key);
		}
	}

	public static string DrawCooldownIcon(SpriteBatch spriteBatch, Player player, string hoveredCooldownIndex, string cooldownSlotOnPlayer, Point position, Point mousePosition)
	{
		var mp = player.GetModPlayer<EverglowPlayer>();
		if (!player.HasCooldown(cooldownSlotOnPlayer))
		{
			return hoveredCooldownIndex;
		}

		var cooldownInstance = mp.cooldowns[cooldownSlotOnPlayer];
		var cooldownBase = cooldownInstance.cooldown;
		if (!cooldownBase.IsVisible)
		{
			return hoveredCooldownIndex;
		}

		int width = 32, height = 32;
		(int drawPosX, int drawPosY) = (position.X, position.Y);
		Vector2 drawPosition = new Vector2(drawPosX, drawPosY);
		Vector2 textPosition = new Vector2(drawPosX, drawPosY + height);
		Rectangle mouseRectangle = new Rectangle(drawPosX, drawPosY, width, height);
		Color drawColor = bossNearby ? Color.White : Color.White * cooldownAlpha[cooldownSlotOnPlayer];

		// Draw background
		spriteBatch.Draw(ModAsset.Cooldown_Background.Value, drawPosition, drawColor);

		// Draw icon
		var sBS = GraphicsUtils.GetState(spriteBatch).Value;
		if (cooldownBase.EnableCutShader)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
			var effect = ModAsset.Cooldown_IconShader.Value;
			effect.Parameters["uCut"].SetValue(ModAsset.Cooldown_Cut.Value);
			// effect.Parameters["uBg"].SetValue(ModAsset.Cooldown_Background.Value);
			effect.CurrentTechnique.Passes[0].Apply();
		}
		else
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
		}

		var iconTexture = cooldownInstance.cooldown.Texture;
		var iconPosition = drawPosition + new Vector2(width / 2, height / 2);
		var iconScale = width / (float)Math.Max(iconTexture.Width, iconTexture.Height) * cooldownBase.TextureScale;
		var iconOrigin = iconTexture.Size() / 2 + cooldownBase.TextureOffset;
		spriteBatch.Draw(iconTexture, iconPosition, null, drawColor, 0, iconOrigin, iconScale, SpriteEffects.None, 0);

		// Draw time progress circle
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
		spriteBatch.GraphicsDevice.Textures[0] = ModAsset.White.Value;
		var progress = cooldownInstance.Progress;
		var progressPos = drawPosition + new Vector2(width / 2, height / 2);
		var progressAlpha = cooldownAlpha[cooldownSlotOnPlayer];
		progressAlpha += (1 - progressAlpha) * 0.5f;
		var progressColor = Color.Lerp(cooldownBase.StartColor, cooldownBase.EndColor, 1f - progress) * progressAlpha * 0.6f;
		var progressSize = new Vector2(0, 7);
		ValueBarHelper.DrawCircle(spriteBatch, progressPos, progressColor, progressSize, progressSize, progress, clockwise: false);

		// Draw progress cursor
		spriteBatch.Draw(ModAsset.White.Value, progressPos, null, drawColor, MathHelper.Pi - progress * MathHelper.TwoPi, Vector2.Zero, new Vector2(1 / 256f, 13 / 256f), SpriteEffects.None, 0);

		spriteBatch.End();
		spriteBatch.Begin(sBS);

		// Draw icon frame
		spriteBatch.Draw(ModAsset.Cooldown_IconFrame.Value, drawPosition, null, drawColor, 0f, default, 1f, SpriteEffects.None, 0f);

		// Draw time text
		var cooldownTime = cooldownInstance.timeLeft;
		if (cooldownTime > 2)
		{
			string text = Lang.LocalizedDuration(new TimeSpan(0, 0, cooldownTime / 60), abbreviated: true, showAllAvailableUnits: false);
			spriteBatch.DrawString(FontAssets.ItemStack.Value, text, textPosition, drawColor, 0f, default, 0.8f, SpriteEffects.None, 0f);
		}

		// Set mouse text and adjust alpha on mouse hover
		if (mouseRectangle.Contains(mousePosition))
		{
			hoveredCooldownIndex = cooldownSlotOnPlayer;
			cooldownAlpha[cooldownSlotOnPlayer] += 0.1f;
		}
		else
		{
			cooldownAlpha[cooldownSlotOnPlayer] -= 0.05f;
		}

		if (cooldownAlpha[cooldownSlotOnPlayer] > 1f)
		{
			cooldownAlpha[cooldownSlotOnPlayer] = 1f;
		}
		else if (cooldownAlpha[cooldownSlotOnPlayer] < DefaultCooldownAlpha)
		{
			cooldownAlpha[cooldownSlotOnPlayer] = DefaultCooldownAlpha;
		}

		return hoveredCooldownIndex;
	}
}
