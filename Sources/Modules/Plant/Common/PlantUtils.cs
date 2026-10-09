using Terraria.Localization;

namespace Everglow.Plant.Common;

public static class PlantUtils
{
	public static Texture2D GetTexture(string text) =>
		ModContent.Request<Texture2D>(text, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
	public static int LocaizationChineseId => GameCulture.FromName("Chinese").LegacyId;
	public static GameCulture LocaizationChinese => GameCulture.FromName("Chinese");
	public static Vector2 DirectionToSafe(this Vector2 source, Vector2 des)
	{
		des = source.DirectionTo(des);
		if (des.HasNaNs())
		{
			des = Vector2.Zero;
		}

		return des;
	}
	public static Vector2 DirectionFromSafe(this Vector2 source, Vector2 des) => -source.DirectionToSafe(des);
	public static float AngleToSafe(this Vector2 source, Vector2 des) => source.DirectionToSafe(des).ToRotation();
	public static float AngleFromSafe(this Vector2 source, Vector2 des) => source.DirectionFromSafe(des).ToRotation();
	public static Vector2 DirectionToSafe(this Entity source, Vector2 des)
	{
		des = source.Center.DirectionTo(des);
		if (des.HasNaNs())
		{
			des = Vector2.Zero;
		}

		return des;
	}
	public static Vector2 DirectionFromSafe(this Entity source, Vector2 des) => -source.DirectionToSafe(des);
	public static float AngleToSafe(this Entity source, Vector2 des) => source.DirectionToSafe(des).ToRotation();
	public static float AngleFromSafe(this Entity source, Vector2 des) => source.DirectionFromSafe(des).ToRotation();
	public static Vector2 NormalizeSafe(this Vector2 vec, Vector2? defVec = null)
	{
		vec.Normalize();
		if (Utils.HasNaNs(vec))
		{
			vec = defVec ?? Vector2.Zero;
		}

		return vec;
	}
}
