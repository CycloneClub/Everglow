using Terraria.GameContent;

namespace Everglow.Commons.Mechanics.Mission.Presentation.Icons;

public class ItemMissionIcon : MissionIconBase
{
	private ItemMissionIcon()
	{
	}

	private int itemType;
	private string tooltip;

	public override string Tooltip
	{
		get
		{
			if (string.IsNullOrWhiteSpace(tooltip))
			{
				var item = new Item();
				item.SetDefaults(itemType);
				return item.Name;
			}
			else
			{
				return tooltip;
			}
		}
	}

	public override void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, float baseScale)
	{
		var drawCenter = new Vector2(
			destinationRectangle.X + destinationRectangle.Width / 2,
			destinationRectangle.Y + destinationRectangle.Height / 2);

		if (itemType >= TextureAssets.Item.Length)
		{
			throw new InvalidDataException();
		}

		var texture = TextureAssets.Item[itemType].Value;

		var frameRect = new Rectangle(0, 0, texture.Width, texture.Height);
		var origin = texture.Size() / 2;
		if (Main.itemAnimationsRegistered.Contains(itemType))
		{
			frameRect = Main.itemAnimations[itemType].GetFrame(texture);
			origin = new Vector2(frameRect.Width, frameRect.Height) / 2;
		}
		var scale = GetTextureScale(destinationRectangle, frameRect, baseScale);

		spriteBatch.Draw(texture, drawCenter, frameRect, color, 0, origin, scale, SpriteEffects.None, 0);
	}

	public static ItemMissionIcon Create(int itemType, string tooltip = null)
	{
		if (itemType >= TextureAssets.Item.Length)
		{
			throw new InvalidDataException();
		}

		return new ItemMissionIcon()
		{
			itemType = itemType,
			tooltip = tooltip ?? string.Empty,
		};
	}
}
