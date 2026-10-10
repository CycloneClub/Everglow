using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;
using MonoMod.Utils;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;

namespace Everglow.Commons.Hooks;

public class HookSystem : ModSystem
{
	private HookManager _manager;

	public HookSystem()
	{
		_manager = Ins.HookManager as HookManager;
		Debug.Assert(_manager is not null);
	}

	public void Invoke(CodeLayer layer)
	{
		foreach (var handler in _manager.hooks[layer].Where(h => h.Enable))
		{
			try
			{
				var hook = (Action)handler.Hook;
				hook.Invoke();
			}
			catch (Exception ex)
			{
				Debug.Fail(ex.ToString());
				Ins.Logger.Error($"{handler} 抛出了异常 {ex}");
				handler.Enable = false;
			}
		}
	}

	public override void Load()
	{
		On_Main.DrawDust += Main_DrawDust;
		On_Main.DrawProjectiles += Main_DrawProjectiles;
		On_Main.DrawNPCs += Main_DrawNPCs;
		On_LegacyPlayerRenderer.DrawPlayers += LegacyPlayerRenderer_DrawPlayers;

		On_WorldGen.playWorld += WorldGen_playWorld;
		On_WorldGen.playWorldCallBack += WorldGen_playWorldCallBack;
		On_WorldGen.serverLoadWorld += WorldGen_serverLoadWorld;
		On_WorldGen.serverLoadWorldCallBack += WorldGen_serverLoadWorldCallBack;
		On_WorldGen.SaveAndQuit += WorldGen_SaveAndQuit;
		On_WorldGen.SaveAndQuitCallBack += On_WorldGen_SaveAndQuitCallBack;

		On_Main.DrawMiscMapIcons += Main_DrawMiscMapIcons;
		On_Main.DrawBG += Main_DrawBG;
		On_Main.DrawBackground += Main_DrawBackground;
		On_Main.DoDraw_WallsTilesNPCs += Main_DoDraw_WallsTilesNPCs;
		On_FilterManager.EndCapture_RenderTarget2D_RenderTarget2D_RenderTarget2D_Vector2_Vector2_Vector2 += On_FilterManager_EndCapture;
		Main.OnResolutionChanged += Main_OnResolutionChanged;
	}

	public override void Unload()
	{
		On_Main.DrawDust -= Main_DrawDust;
		On_Main.DrawProjectiles -= Main_DrawProjectiles;
		On_Main.DrawNPCs -= Main_DrawNPCs;
		On_LegacyPlayerRenderer.DrawPlayers -= LegacyPlayerRenderer_DrawPlayers;

		On_WorldGen.playWorld -= WorldGen_playWorld;
		On_WorldGen.playWorldCallBack -= WorldGen_playWorldCallBack;
		On_WorldGen.serverLoadWorld -= WorldGen_serverLoadWorld;
		On_WorldGen.serverLoadWorldCallBack -= WorldGen_serverLoadWorldCallBack;
		On_WorldGen.SaveAndQuit -= WorldGen_SaveAndQuit;
		On_WorldGen.SaveAndQuitCallBack -= On_WorldGen_SaveAndQuitCallBack;

		On_Main.DrawMiscMapIcons -= Main_DrawMiscMapIcons;
		On_Main.DrawBG -= Main_DrawBG;
		On_Main.DrawBackground -= Main_DrawBackground;
		On_Main.DoDraw_WallsTilesNPCs -= Main_DoDraw_WallsTilesNPCs;
		On_FilterManager.EndCapture_RenderTarget2D_RenderTarget2D_RenderTarget2D_Vector2_Vector2_Vector2 -= On_FilterManager_EndCapture;
		Main.OnResolutionChanged -= Main_OnResolutionChanged;
	}

	private void On_FilterManager_EndCapture(On_FilterManager.orig_EndCapture_RenderTarget2D_RenderTarget2D_RenderTarget2D_Vector2_Vector2_Vector2 orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Vector2 screenSize, Vector2 sceneSize, Vector2 sceneOffset)
	{
		Invoke(CodeLayer.PreDrawFilter);
		orig(self, finalTexture, screenTarget1, screenTarget2, screenSize, sceneSize, sceneOffset);
	}

	public override void PostDrawTiles()
	{
		Invoke(CodeLayer.PostDrawTiles);
	}

	public override void PostUpdateDusts()
	{
		Invoke(CodeLayer.PostUpdateDusts);
	}

	public override void PostUpdateEverything()
	{
		Invoke(CodeLayer.PostUpdateEverything);
	}

	public override void PostUpdateInvasions()
	{
		Invoke(CodeLayer.PostUpdateInvasions);
	}

	public override void PostUpdateNPCs()
	{
		Invoke(CodeLayer.PostUpdateNPCs);
	}

	public override void PostUpdatePlayers()
	{
		Invoke(CodeLayer.PostUpdatePlayers);
	}

	public override void PostUpdateProjectiles()
	{
		Invoke(CodeLayer.PostUpdateProjectiles);
	}

	public void RemoveHook(IHookHandler handler)
	{
		_manager.RemoveHook(handler);
	}

	private void LegacyPlayerRenderer_DrawPlayers(On_LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Terraria.Graphics.Camera camera, IEnumerable<Player> players)
	{
		orig.Invoke(self, camera, players);
		Invoke(CodeLayer.PostDrawPlayers);
	}

	private void Main_DoDraw_WallsTilesNPCs(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
	{
		Invoke(CodeLayer.PostDrawBG);
		orig(self);
	}

	private void Main_DrawBackground(On_Main.orig_DrawBackground orig, Main self)
	{
		if (_manager.disableFlags.Has(TerrariaFunction.DrawBackground))
		{
			return;
		}

		orig(self);
	}

	private void Main_DrawBG(On_Main.orig_DrawBG orig, Main self, Terraria.DataStructures.SpriteBatchBeginner parentSpriteBatchBeginner)
	{
		if (_manager.disableFlags.Has(TerrariaFunction.DrawSkyAndHell))
		{
			return;
		}

		orig(self, parentSpriteBatchBeginner);
	}

	private void Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
	{
		orig.Invoke(self);
		Invoke(CodeLayer.PostDrawDusts);
	}

	private void Main_DrawMiscMapIcons(On_Main.orig_DrawMiscMapIcons orig, Main self, SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString)
	{
		orig(self, spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
		foreach (var handler in _manager.hooks[CodeLayer.PostDrawMapIcons].Where(h => h.Enable))
		{
			try
			{
				var hook = (Action<Vector2, Vector2, Rectangle?, float>)handler.Hook;
				hook.Invoke(mapTopLeft, mapX2Y2AndOff, mapRect, mapScale);
			}
			catch (Exception ex)
			{
				Debug.Fail(ex.ToString());
				Ins.Logger.Error($"{handler} 抛出了异常 {ex}");
				handler.Enable = false;
			}
		}
	}

	private void Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
	{
		orig.Invoke(self, behindTiles);
		if (!behindTiles)
		{
			Invoke(CodeLayer.PostDrawNPCs);
		}
	}

	private void Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
	{
		orig.Invoke(self);
		Invoke(CodeLayer.PostDrawProjectiles);
	}

	private void Main_OnResolutionChanged(Vector2 obj)
	{
		foreach (var handler in _manager.hooks[CodeLayer.ResolutionChanged].Where(h => h.Enable))
		{
			try
			{
				var hook = (Action<Vector2>)handler.Hook;
				hook.Invoke(obj);
			}
			catch (Exception ex)
			{
				Debug.Fail(ex.ToString());
				Ins.Logger.Error($"{handler} 抛出了异常 {ex}");
				handler.Enable = false;
			}
		}
	}

	private void WorldGen_playWorld(On_WorldGen.orig_playWorld orig)
	{
		orig();
		Invoke(CodeLayer.PreEnterWorld_Single);
	}

	private void WorldGen_playWorldCallBack(On_WorldGen.orig_playWorldCallBack orig, object threadContext)
	{
		orig(threadContext);
		Invoke(CodeLayer.PostEnterWorld_Single);
	}

	private Task WorldGen_serverLoadWorld(On_WorldGen.orig_serverLoadWorld orig)
	{
		var task = orig();
		Invoke(CodeLayer.PreEnterWorld_Server);
		return task;
	}

	private void WorldGen_serverLoadWorldCallBack(On_WorldGen.orig_serverLoadWorldCallBack orig)
	{
		orig();
		Invoke(CodeLayer.PostEnterWorld_Server);
	}

	private void WorldGen_SaveAndQuit(On_WorldGen.orig_SaveAndQuit orig, Action callback)
	{
		orig(callback);
		Invoke(CodeLayer.PreSaveAndQuit);
	}

	private void On_WorldGen_SaveAndQuitCallBack(On_WorldGen.orig_SaveAndQuitCallBack orig, object threadContext)
	{
		orig(threadContext);
		Invoke(CodeLayer.PostSaveAndQuit);
	}
}
