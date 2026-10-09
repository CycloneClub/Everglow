using Everglow.Myth.TheFirefly.Dusts;
using Terraria.ID;

namespace Everglow.Myth.TheFirefly.Buffs;

public class ShadowPotionBuff : ModBuff
{
	internal int LightTime = 0;
	private Player player = Main.LocalPlayer;
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}
	public override void Update(Player player, ref int buffIndex)
	{
		Color playerLight = Lighting.GetColor((int)(player.Center.X / 16), (int)(player.Center.Y / 16));
		if (Math.Max(Math.Max(playerLight.R, playerLight.G), playerLight.B) <= 190)
		{
			player.statDefense += 15;
			player.allDamage += 0.2f;
			LightTime++;

			if (Main.rand.NextBool(30))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<FluorescentTreeDust>(), newColor: new(30, 0, 60), Alpha: 155);
				dust.noGravity = true;
				dust.velocity *= 0.5f;
				dust.velocity.Y -= 1f;
			}
		}
		if (Math.Max(Math.Max(playerLight.R, playerLight.G), playerLight.B) >= 230)
		{
			player.lifeRegen -= (int)(player.statLifeMax2 * 0.01f);
			player.lifeRegenTime = 0;
			LightTime--;

			if (Main.rand.NextBool(10))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<FluorescentTreeDust>(), newColor: new(30, 0, 60), Alpha: 155);
				dust.noGravity = true;
				dust.velocity *= 0.5f;
				dust.velocity.Y -= 1f;
			}
		}
		if (LightTime > 4)
		{
			player.nightVision = true;
		}
		if (LightTime > 20)
		{ LightTime = 20; }
	}
}
