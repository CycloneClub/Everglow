using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spine;
using Terraria;

namespace Everglow.Commons.Skeleton2D.Reader;

public class XnaTextureLoader : TextureLoader
{
	private Texture2D targetAtlas = null;
	private string[] textureLayerSuffixes = null;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="device">The graphics device to be used.</param>
	/// <param name="loadMultipleTextureLayers">If <c>true</c> multiple textures layers
	/// (e.g. a diffuse/albedo texture and a normal map) are loaded instead of a single texture.
	/// Names are constructed based on suffixes added according to the <c>textureSuffixes</c> parameter.</param>
	/// <param name="textureSuffixes">If <c>loadMultipleTextureLayers</c> is <c>true</c>, the strings of this array
	/// define the path name suffix of each layer to be loaded. Array size must be equal to the number of layers to be loaded.
	/// The first array entry is the suffix to be <c>replaced</c> (e.g. "_albedo", or "" for a first layer without a suffix),
	/// subsequent array entries contain the suffix to replace the first entry with (e.g. "_normals").
	///
	/// An example would be:
	/// <code>new string[] { "", "_normals" }</code> for loading a base diffuse texture named "skeletonname.png" and
	/// a normalmap named "skeletonname_normals.png".</param>
	public XnaTextureLoader(Texture2D targetAtlas, bool loadMultipleTextureLayers = false, string[] textureSuffixes = null)
	{
		this.targetAtlas = targetAtlas;
		if (loadMultipleTextureLayers)
		{
			this.textureLayerSuffixes = textureSuffixes;
		}
	}

	public void Load(AtlasPage page, string path)
	{
		Texture2D texture = targetAtlas;
		page.width = texture?.Width ?? 0;
		page.height = texture?.Height ?? 0;

		if (textureLayerSuffixes == null)
		{
			page.rendererObject = texture;
		}
		else
		{
			// TODO: Currently, we do not support loading multiple atlas layers, if
			// this is needed, contact Skirt
			throw new NotImplementedException("TODO: multiple atlas layers not implemented, contact Skirt if needed");
		}
	}

	public void Unload(object texture)
	{
		((Texture2D)texture).Dispose();
	}
}
