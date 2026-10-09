using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;

namespace Everglow.Commons.TileHelper;

public class PaintedTextureSystem : ModSystem
{
	/// <summary>
	/// TileVariationkey -> Texture Path
	/// 通过TileVariationkey获取贴图路径，懒得写自己的Key就借用原版的了
	/// </summary>
	internal static Dictionary<TilePaintSystemV2.TileVariationkey, string> TexturePathLookup { get; private set; }
	public TilePaintSystemV2.TileVariationkey Key;

	public static Texture2D TryGetPaintedTexture(string path, int tileType, int tileStyle, int paintColor, TileDrawing tileDrawing)
	{
		var paintSystem = tileDrawing._paintSystem;

		TilePaintSystemV2.TileVariationkey tileVariationkey = default;
		tileVariationkey.TileType = tileType;
		tileVariationkey.TileStyle = tileStyle;
		tileVariationkey.PaintColor = paintColor;
		if (paintSystem._tilesRenders.TryGetValue(tileVariationkey, out var value) && value.IsReady)
			return value.Target;

		TexturePathLookup[tileVariationkey] = path;

		if (!paintSystem._tilesRenders.TryGetValue(tileVariationkey, out value))
		{
			value = new PaintedTextureHolder
			{
				Key = tileVariationkey,
			};
			paintSystem._tilesRenders.Add(tileVariationkey, value);
		}

		if (!value.IsReady)
		{
			paintSystem._requests.Add(value);
		}

		return null;
	}

	public override void Load()
	{
		TexturePathLookup = new Dictionary<TilePaintSystemV2.TileVariationkey, string>();
	}

	public override void Unload()
	{
		TexturePathLookup = null;
	}
}

public class PaintedTextureHolder : TilePaintSystemV2.TileRenderTargetHolder
{
	public TilePaintSystemV2.TileVariationkey Key;

	public override void Prepare()
	{
		var modTile = TileLoader.GetTile(Key.TileType);
		Asset<Texture2D> asset = ModContent.Request<Texture2D>($"Everglow/{PaintedTextureSystem.TexturePathLookup[Key]}");

		asset.Wait?.Invoke();
		PrepareTextureIfNecessary(asset.Value);
	}

	public override void PrepareShader()
	{
		PrepareShader(Key.PaintColor, TreePaintSystemData.DefaultNoSpecialGroups);
	}
}
