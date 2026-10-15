using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using FontStashSharp;

namespace Everglow.Commons.UI;

public class FontManager : ILoadable
{
	private const int ResolutionFactor = 8;

	private static readonly ReadOnlyCollection<string> AcceptableFontExtensionList =
		new ReadOnlyCollectionBuilder<string>() { ".ttf" }
		.ToReadOnlyCollection();

	public static FontManager Instance { get; private set; }

	public static FontSystem FusionPixel12 => Instance.Fonts[nameof(FusionPixel12)];

	private Dictionary<string, FontSystem> _fonts = [];

	public IReadOnlyDictionary<string, FontSystem> Fonts => _fonts;

	public FontManager()
	{
		Instance = Instance == null
			? this
			: throw new InvalidOperationException("Mutiple font manager created, please check the loading of font manager.");
	}

	public void Load(Mod mod)
	{
		mod.GetFileNames().ForEach(fileName =>
		{
			if (AcceptableFontExtensionList.Contains(Path.GetExtension(fileName)))
			{
				var settings = new FontSystemSettings()
				{
					FontResolutionFactor = ResolutionFactor,
					KernelWidth = ResolutionFactor,
					KernelHeight = ResolutionFactor,
				};

				var fs = new FontSystem(settings);
				fs.AddFont(mod.GetFileBytes(fileName));
				_fonts.Add(Path.GetFileNameWithoutExtension(fileName), fs);
			}
		});
	}

	public void Unload()
	{
		foreach (var (_, fs) in _fonts)
		{
			fs.Dispose();
		}
		_fonts.Clear();
		Instance = null;
	}
}
