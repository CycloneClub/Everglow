namespace Everglow.Yggdrasil.YggdrasilTown.Buffs;

public class GuideBuff : ModBuff
{
	public override string Texture => Commons.ModAsset.BuffTemplate_Mod;

	public override void SetStaticDefaults()
	{
		Main.pvpBuff[Type] = true;
		Main.buffNoSave[Type] = false;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// 1. Provides a small amount of light at the mouse position
		// =========================================================
		player.GetModPlayer<GuideBuffPlayer>().GuideBuffEnable = true;
	}
}

public class GuideBuffPlayer : ModPlayer
{
	public bool GuideBuffEnable { get; set; } = false;

	public override void PreUpdateBuffs()
	{
		if (GuideBuffEnable)
		{
			if (Main.myPlayer == Player.whoAmI)
			{
				Lighting.AddLight(Main.MouseWorld, 0.8f, 0.6f, 0.4f);
			}

			GuideBuffEnable = false;
		}
	}
}
