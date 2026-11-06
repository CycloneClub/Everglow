namespace Everglow.Myth.Common;

public class MythContent
{
	/// <summary>
	/// 对于神话模块专用的获取图片封装
	/// </summary>
	/// <param name="path"> </param>
	/// <returns> </returns>
	public static Texture2D QuickTexture(string path)
	{
		return ModContent.Request<Texture2D>("Everglow/Myth/" + path, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
	}

	/// <summary>
	/// 获取太阳位置
	/// </summary>
	public static Vector2 GetSunPos()
	{
		float HalfMaxTime = Main.dayTime ? 27000 : 16200;
		float bgTop = -Main.screenPosition.Y / (float)(Main.worldSurface * 16.0 - 600.0) * 200f;
		float value = 1 - (float)Main.time / HalfMaxTime;
		float StarX = (1 - value) * Main.screenWidth / 2f - 100 * value;
		float t = value * value;
		float StarY = bgTop + t * 250f + 180;
		if (Main.LocalPlayer != null)
		{
			if (Main.LocalPlayer.gravDir == -1)
			{
				return new Vector2(StarX, Main.screenHeight - StarY);
			}
		}

		return new Vector2(StarX, StarY);
	}
}
