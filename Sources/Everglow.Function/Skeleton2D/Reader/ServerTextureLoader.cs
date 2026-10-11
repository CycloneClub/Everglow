using Spine;

namespace Everglow.Commons.Skeleton2D.Reader;

internal sealed class ServerTextureLoader : TextureLoader
{
	public void Load(AtlasPage page, string path)
	{
		if (page.width <= 0 || page.height <= 0)
		{
			throw new InvalidDataException($"Atlas page '{page.name}' must specify positive dimensions when loaded without a texture.");
		}
	}

	public void Unload(object texture)
	{
	}
}
