using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.Commons.Mechanics.Quest.Presentation;

public sealed record QuestPresentationEntry(
	QuestView View,
	IReadOnlyList<QuestAction> Actions);
