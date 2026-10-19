using Terraria.GameContent.UI;

namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>Ambient conversation between lightweight travelers, without NPC hooks or slots.</summary>
internal static class CrowdConversation
{
	internal const int OpportunityInterval = 120;
	internal const int CooldownTicks = 600;
	private const int BubbleDuration = 100;
	private const int BubbleInterval = 120;

	private sealed class CrowdConversationSession(CrowdAgent visitor, CrowdAgent peer, long until)
	{
		public readonly CrowdAgent Visitor = visitor;
		public readonly CrowdAgent Peer = peer;
		public readonly long Until = until;
		public int Bubble = -1;
		public int Turn;
		public long NextBubble;
	}

	private static readonly Dictionary<CrowdAgent, CrowdConversationSession> Visitors = [];
	private static readonly int[] Topics = [EmoteID.WeatherSunny, EmoteID.ItemSoup, EmoteID.ItemFishingRod, EmoteID.ItemSword];

	internal static bool InRange(Entity first, Entity second) => Vector2.DistanceSquared(first.Center, second.Center)
		<= MathF.Pow(CrowdSettings.ChatRangeTiles * 16, 2)
		&& Collision.CanHitLine(first.Center, 1, 1, second.Center, 1, 1);

	private static bool Safe(Entity body) => !CrowdThreats.EnemyNear(body.Center) && !CrowdThreats.Dangerous(body.Bottom);

	internal static bool BeginPeers(CrowdAgent visitor, CrowdAgent peer, int duration)
	{
		if (ReferenceEquals(visitor, peer) || !visitor.CanConverse || !peer.CanConverse
			|| Visitors.ContainsKey(visitor) || Visitors.ContainsKey(peer) || !InRange(visitor.Body, peer.Body)
			|| !Safe(visitor.Body) || !Safe(peer.Body))
		{
			return false;
		}
		visitor.PauseForConversation();
		peer.PauseForConversation();
		var session = new CrowdConversationSession(visitor, peer, TownCrowdSystem.Clock + duration);
		Visitors.Add(visitor, session);
		Visitors.Add(peer, session);
		return true;
	}

	internal static bool TryNearby(CrowdAgent visitor)
	{
		if (!visitor.CanConverse || !Safe(visitor.Body))
		{
			return false;
		}
		int duration = Main.rand.Next(CrowdSettings.ChatMinTicks, CrowdSettings.ChatMaxTicks + 1);
		return TownCrowdSystem.ConversationPeers(visitor).Any(peer => BeginPeers(visitor, peer, duration));
	}

	internal static bool ContinueNearby(CrowdAgent visitor)
	{
		if (!Visitors.TryGetValue(visitor, out var session))
		{
			return false;
		}
		if (!Valid(session))
		{
			End(session.Visitor, TownCrowdSystem.Clock >= session.Until);
			return false;
		}
		visitor = session.Visitor;
		visitor.FaceConversation(session.Peer.Body);
		session.Peer.FaceConversation(visitor.Body);
		if (TownCrowdSystem.Clock >= session.NextBubble)
		{
			ClearBubble(session);
			Entity speaker = session.Turn++ % 2 == 0 ? visitor.Body : session.Peer.Body;
			int topic = Topics[(int)(((uint)visitor.RouteSeed + (uint)((session.Turn - 1) / 2)) % (uint)Topics.Length)];
			session.Bubble = EmoteBubble.NewBubble(topic, new WorldUIAnchor(speaker), BubbleDuration);
			session.NextBubble = TownCrowdSystem.Clock + BubbleInterval;
		}
		return true;
	}

	private static bool Valid(CrowdConversationSession session) => TownCrowdSystem.Enabled && session.Visitor.Active && session.Peer.Active
		&& session.Visitor.Stage == CrowdStage.Conversing && session.Peer.Stage == CrowdStage.Conversing
		&& !session.Visitor.IsEvacuating && !session.Peer.IsEvacuating && TownCrowdSystem.Clock < session.Until
		&& InRange(session.Visitor.Body, session.Peer.Body) && Safe(session.Peer.Body) && Safe(session.Visitor.Body);

	internal static void End(CrowdAgent visitor, bool completed = false)
	{
		if (Visitors.Remove(visitor, out var session))
		{
			ClearBubble(session);
			Visitors.Remove(session.Visitor);
			Visitors.Remove(session.Peer);
			session.Visitor.ResumeAfterConversation(completed);
			session.Peer.ResumeAfterConversation(completed);
		}
	}

	private static void ClearBubble(CrowdConversationSession session)
	{
		if (session.Bubble >= 0 && EmoteBubble.GetExistingEmoteBubble(session.Bubble) is { } bubble)
		{
			bubble.lifeTime = 0;
		}
		session.Bubble = -1;
	}

	internal static void Clear()
	{
		foreach (var visitor in Visitors.Keys.ToArray())
		{
			End(visitor);
		}
	}
}
