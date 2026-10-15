using Everglow.Commons.Utilities;
using Terraria.GameContent;

namespace Everglow.Commons.Mechanics.Quest.Presentation.Icons;

public class NPCQuestIcon : QuestIconBase
{
	private NPCQuestIcon()
	{
	}

	private int nPCType;
	private string tooltip;

	public override string Tooltip
	{
		get
		{
			if (string.IsNullOrWhiteSpace(tooltip))
			{
				var npc = new NPC();
				npc.SetDefaults(nPCType);
				return npc.TypeName;
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

		if (nPCType >= TextureAssets.Npc.Length)
		{
			throw new InvalidDataException();
		}

		var texture = TextureAssets.Npc[nPCType]?.Value;
		var frameRect = texture.Frame(verticalFrames: Main.npcFrameCount[nPCType], frameY: (int)(Main.timeForVisualEffects / 10) % Main.npcFrameCount[nPCType]);
		var origin = new Vector2(texture.Width, texture.Height / Main.npcFrameCount[nPCType]) / 2;
		float scale = GetTextureScale(destinationRectangle, frameRect, baseScale) * baseScale;

		spriteBatch.Draw(texture, drawCenter, frameRect, color, 0, origin, scale, SpriteEffects.None, 0);
	}

	public static NPCQuestIcon Create(int nPCType, string tooltip = null)
	{
		if (nPCType >= TextureAssets.Npc.Length)
		{
			throw new InvalidDataException();
		}
		AssetUtils.LoadVanillaNPCTexture(nPCType);

		return new NPCQuestIcon()
		{
			nPCType = nPCType,
			tooltip = tooltip ?? string.Empty,
		};
	}
}
