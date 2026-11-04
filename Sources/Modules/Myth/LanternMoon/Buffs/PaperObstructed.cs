using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Buffs;

public class PaperObstructed : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		if (!player.creativeGodMode)
		{
			player.headcovered = true;
			player.bleed = true;
			// -10% critical strike chance
			player.GetCritChance(DamageClass.Generic) -= 10;
			// -10% damage
			player.GetDamage(DamageClass.Generic) -= 0.1f;
		}
	}
}

public class PaperObstructedDrawLayer : PlayerDrawLayer
{
	public override bool IsHeadLayer => true;

	public override Position GetDefaultPosition()
	{
		return new AfterParent(PlayerDrawLayers.Head);
	}

	public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
	{
		if (drawInfo.drawPlayer.HasBuff(ModContent.BuffType<PaperObstructed>()))
		{
			return true;
		}
		return false;
	}

	public override void Draw(ref PlayerDrawSet drawInfo)
	{
		Texture2D Texture = ModAsset.LittleRedPaperFigure.Value;
		Vector2 position = drawInfo.Center + new Vector2(0f, -10f) - Main.screenPosition;
		position = new Vector2((int)position.X, (int)position.Y);
		drawInfo.DrawDataCache.Add(new DrawData(
	Texture,
	position,
	new Rectangle(112, 112, 32, 32), Lighting.GetColor((int)position.X / 16, (int)position.Y / 16), 0f, new Vector2(16, 16), 1f, drawInfo.drawPlayer.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0));
	}
}
