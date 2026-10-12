namespace Everglow.Commons.Mechanics.Quest.Presentation.Icons;

/// <summary>
/// Supplies task-panel animation frames for NPC textures that are not uniform vertical strips.
/// </summary>
public interface INPCQuestIconFrameProvider
{
	Rectangle GetQuestIconFrame(int animationFrame);
}
