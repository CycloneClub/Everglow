namespace Everglow.Commons.Enums;

public enum CodeLayer
{
	None,
	PostSetupContent,

	// 绘制
	PreDrawFilter,
	[Obsolete("换用PreDrawFilter")]
	PostDrawFilter,
	PostDrawTiles,
	PostDrawProjectiles,
	PostDrawDusts,
	PostDrawNPCs,
	PostDrawPlayers,
	PostDrawMapIcons,
	PostDrawBG,

	// 加载
	PostUpdateEverything,
	PostUpdateProjectiles,
	PostUpdatePlayers,
	PostUpdateNPCs,
	PostUpdateDusts,
	PostUpdateInvasions,

	PreEnterWorld_Single,
	PostEnterWorld_Single,
	PreEnterWorld_Server,
	PostEnterWorld_Server,
	PreSaveAndQuit,
	PostSaveAndQuit,

	ResolutionChanged,

	PreDrawMinimapAndOverlayIcons,
	PreDrawFullscreenMapIcons,
}
