using Everglow.Commons.Utilities;
using Terraria.Graphics;
using Terraria.Localization;

namespace Everglow.Example.AdvancedMirror;

public class AdvancedMirrorWall : ModWall
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = DustID.Glass;
		AddMapEntry(new Color(140, 180, 205), Language.GetText(AdvancedMirrorItem.TextKey + "Name"));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (!Main.dedServ)
		{
			AdvancedMirrorConfigUI settings = ModContent.GetInstance<AdvancedMirrorConfigUI>();
			DrawSurface(i, j, settings.SurfaceTint,
				Math.Clamp(settings.SurfaceOpacity, 0, 1));
		}
		return false;
	}

	/// <summary>
	/// 在墙的 PreDraw 中调用；TileBatch 的 Begin/End 由原版墙绘制阶段管理。
	/// </summary>
	private static void DrawSurface(int i, int j, Color tint, float opacity)
	{
		if (Main.dedServ || opacity <= 0)
		{
			return;
		}
		// 每个角平均周围四格光照，相邻墙格因此使用同一组边界采样。
		Lighting.GetCornerColors(i, j, out VertexColors light);
		Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + offset;
		Texture2D pixel = Commons.ModAsset.White.Value;
		Main.tileBatch.Draw(pixel, new Vector4(position.X, position.Y, 16, 16), Tint(light, tint, opacity));
	}

	/// <summary>
	/// 将光照与表面颜色相乘，并为 AlphaBlend 预乘透明度。
	/// </summary>
	/// <param name="light">顶点光照颜色，仅使用 RGB 分量。</param>
	/// <param name="tint">表面颜色，仅使用 RGB 分量。</param>
	/// <param name="opacity">表面不透明度，会限制到 0 到 1。</param>
	/// <returns>已预乘透明度的顶点颜色。</returns>
	private static Color ApplyLight(Color light, Color tint, float opacity)
	{
		opacity = Math.Clamp(opacity, 0, 1);
		return new Color(new Vector4(light.ToVector3() * tint.ToVector3() * opacity, opacity));
	}

	private static VertexColors Tint(VertexColors light, Color tint, float opacity) => new(
		ApplyLight(light.TopLeftColor, tint, opacity), ApplyLight(light.TopRightColor, tint, opacity),
		ApplyLight(light.BottomRightColor, tint, opacity), ApplyLight(light.BottomLeftColor, tint, opacity));
}
