using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

namespace Everglow.Yggdrasil.KelpCurtain;

public class CrimsonMoonAlgaeTileImmunity : ModSystem
{
	public override void Load() => On_Player.ApplyTouchDamage += ApplyTouchDamage;

	public override void Unload() => On_Player.ApplyTouchDamage -= ApplyTouchDamage;

	private static void ApplyTouchDamage(On_Player.orig_ApplyTouchDamage orig, Player player, int tileType, int x, int y)
	{
		if (!RedAlgaeProtection.PreventsTouchDamage(
			player.GetModPlayer<KelpCurtainPlayer>().CrimsonMoonAlgaeSetBuff,
			tileType, ModContent.TileType<JadeLakeBloodVineAlgea>()))
		{
			orig(player, tileType, x, y);
		}
	}
}
