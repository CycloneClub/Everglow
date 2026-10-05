namespace Everglow.Yggdrasil.KelpCurtain;

public static class RedAlgaeProtection
{
	public static bool PreventsTouchDamage(bool armorSet, int tileType, int bloodVineType) => armorSet && tileType == bloodVineType;
}
