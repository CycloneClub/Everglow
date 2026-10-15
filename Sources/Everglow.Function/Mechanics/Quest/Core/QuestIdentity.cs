namespace Everglow.Commons.Mechanics.Quest.Core;

public readonly record struct QuestIdentity(
	QuestSide Side,
	string DefinitionId,
	string InstanceId);
