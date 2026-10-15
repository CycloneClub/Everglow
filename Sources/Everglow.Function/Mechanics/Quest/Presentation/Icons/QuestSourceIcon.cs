using Everglow.Commons.Mechanics.Quest.Core;
using Terraria.ModLoader.Config;

namespace Everglow.Commons.Mechanics.Quest.Presentation.Icons;

public class QuestSourceIcon : QuestIconBase
{
	private QuestSourceIcon()
	{
	}

	public QuestSourceBase Source { get; private set; }

	[NullAllowed]
	public QuestSourceBase SubSource { get; private set; }

	public override string Tooltip => SubSource is null
		? Source.Name
		: Source.Name + " - " + SubSource.Name;

	public override void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, float baseScale)
	{
		Texture2D sourceTexture = Source.Texture;
		Vector2 sourceOrigin = sourceTexture.Size() / 2;
		float sourceScale = sourceTexture.Width >= sourceTexture.Height
			? destinationRectangle.Width / (float)sourceTexture.Width
			: destinationRectangle.Height / (float)sourceTexture.Height;

		spriteBatch.Draw(sourceTexture, destinationRectangle.Center(), null, Color.White, 0, sourceOrigin, sourceScale, SpriteEffects.None, 0);

		if (SubSource is not null)
		{
			Texture2D subSourceTex = SubSource.Texture;
			Vector2 subSourceDrawPos = destinationRectangle.Center() + new Vector2(destinationRectangle.Width * 0.24f, destinationRectangle.Height * 0.24f);
			Vector2 subSourceOrigin = subSourceTex.Size() / 2;
			float subSourceScale = subSourceTex.Width >= subSourceTex.Height
				? destinationRectangle.Width / (float)subSourceTex.Width * 0.3f
				: destinationRectangle.Height / (float)subSourceTex.Height * 0.3f;

			spriteBatch.Draw(subSourceTex, subSourceDrawPos, null, Color.White, 0, subSourceOrigin, subSourceScale, SpriteEffects.None, 0);
		}
	}

	public static QuestSourceIcon Create(QuestSourceBase source, QuestSourceBase subSource)
	{
		var icon = new QuestSourceIcon
		{
			Source = source,
			SubSource = subSource,
		};

		return icon;
	}
}
