using Everglow.Commons.Modules;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SubworldLibrary;
using Terraria.Graphics.Effects;
using Terraria.Map;

namespace Everglow.Yggdrasil;

public class YggdrasilModule : EverglowModule
{
	public override string Name => "Yggdrasil";

	private RenderTarget2D screen = null;
	private RenderTarget2D occlusionRender = null;
	private RenderTarget2D effectTarget = null;
	private RenderTarget2D totalEffeftsRender = null;

	private Effect screenOcclusion;

	internal event Action OnStartDay;

	internal event Action OnStartNight;

	public override void Load()
	{
		if (!Main.dedServ)
		{
			On_FilterManager.EndCapture += FilterManager_EndCapture;
			On_WorldGen.oceanDepths += WorldGen_oceanDepths;
			screenOcclusion = ModContent.Request<Effect>("Everglow/Yggdrasil/Effects/Occlusion", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			IL_Main.DrawToMap += IL_Main_DrawToMap;
			IL_Main.DrawToMap_Section += IL_Main_DrawToMap_Section;
			On_Main.UpdateTime_StartDay += On_Main_UpdateTime_StartDay;
			On_Main.UpdateTime_StartNight += On_Main_UpdateTime_StartNight;
			On_Main.CanStartInvasion += On_Main_CanStartInvasion;
			On_Main.StartInvasion += On_Main_StartInvasion;
		}
	}

	private void On_Main_StartInvasion(On_Main.orig_StartInvasion orig, int type)
	{
		if (YggdrasilWorld.InYggdrasil)
		{
			return;
		}

		orig(type);
	}

	private bool On_Main_CanStartInvasion(On_Main.orig_CanStartInvasion orig, int type, bool ignoreDelay)
	{
		if (YggdrasilWorld.InYggdrasil)
		{
			return false;
		}

		return orig(type, ignoreDelay);
	}

	private void On_Main_UpdateTime_StartNight(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
	{
		orig(ref stopEvents);
		if (!stopEvents)
		{
			OnStartNight?.Invoke();
		}
	}

	private void On_Main_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents)
	{
		orig(ref stopEvents);
		if (!stopEvents)
		{
			OnStartDay?.Invoke();
		}
	}

	private bool WorldGen_oceanDepths(On_WorldGen.orig_oceanDepths orig, int x, int y)
	{
		if (SubworldSystem.IsActive<YggdrasilWorld>())
		{
			return false;
		}
		return orig(x, y);
	}

	private void IL_Main_DrawToMap_Section(ILContext il)
	{
		ILCursor c = new(il);
		if (!c.TryGotoNext(MoveType.After, i => i.MatchCall(typeof(MapHelper), nameof(MapHelper.GetMapTileXnaColor))))
		{
			throw new OperationCanceledException("fail to il drawtomap_section");
		}
		c.Emit(OpCodes.Ldloc, 12);
		c.Emit(OpCodes.Ldloc, 11);
		c.EmitDelegate(ILHook_DrawToMap_Section_ChangeMapTargetColor);
	}

	private static Color ILHook_DrawToMap_Section_ChangeMapTargetColor(Color color, int x, int y)
	{
		ILHook_DrawToMap_ChangeMapTargetColor(ref color, x, y);
		return color;
	}

	private void IL_Main_DrawToMap(ILContext il)
	{
		ILCursor c = new(il);
		if (!c.TryGotoNext(MoveType.After, i => i.MatchStloc(23)))
		{
			throw new OperationCanceledException("fail to il drawtomap");
		}
		c.Emit(OpCodes.Ldloca, 23);
		c.Emit(OpCodes.Ldloc, 16);
		c.Emit(OpCodes.Ldloc, 17);
		c.EmitDelegate(ILHook_DrawToMap_ChangeMapTargetColor);
	}

	private static void ILHook_DrawToMap_ChangeMapTargetColor(ref Color color, int x, int y)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			return;
		}

		// 修改小地图绘制颜色在这里写
		Tile tile = Main.tile[x, y];
		if (tile.HasTile || tile.WallType > 0 || tile.LiquidAmount > 0)
		{
			return;
		}
		if (y > Main.maxTilesY - 2100)
		{
			color = new Color(0, 0, 0);
			return;
		}
		else if (y <= Main.maxTilesY - 2100 && y > Main.maxTilesY - 4200)
		{
			color = new Color(73, 99, 62);
			return;
		}
		color = Color.Lerp(Color.Green, Color.Red, (float)y / Main.maxTilesY);
	}

	private void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
	{
		bool enable = false;
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.ModProjectile is IOcclusionProjectile && proj.active)
			{
				enable = true;
			}
		}
		if (!enable)
		{
			orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
			return;
		}

		// 直接从RT池子里取
		var renderTargets = Ins.RenderTargetPool.GetRenderTarget2DArray(4);
		screen = renderTargets.Resource[0];
		occlusionRender = renderTargets.Resource[1];
		effectTarget = renderTargets.Resource[2];
		totalEffeftsRender = renderTargets.Resource[3];

		GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
		GetOrig(graphicsDevice);

		graphicsDevice.SetRenderTarget(occlusionRender);
		graphicsDevice.Clear(Color.Transparent);
		bool flag = DrawOcclusion(Ins.Batch);

		graphicsDevice.SetRenderTarget(effectTarget);
		graphicsDevice.Clear(Color.Transparent);
		DrawEffect(Ins.Batch);

		if (flag)
		{
			// 保存原图
			graphicsDevice.SetRenderTarget(screen);
			graphicsDevice.Clear(Color.Black);
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
			Main.spriteBatch.End();

			graphicsDevice.SetRenderTarget(totalEffeftsRender);
			graphicsDevice.Clear(Color.Transparent);
			graphicsDevice.Textures[1] = occlusionRender;
			graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			screenOcclusion.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(effectTarget, Vector2.Zero, Color.White);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

			graphicsDevice.SetRenderTarget(Main.screenTarget);
			graphicsDevice.Clear(Color.Black);

			Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Main.spriteBatch.Draw(totalEffeftsRender, Vector2.Zero, Color.White);
			Main.spriteBatch.End();
		}
		screen = null;
		renderTargets.Release();
		orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
	}

	private bool DrawOcclusion(VFXBatch spriteBatch)// 遮盖层
	{
		spriteBatch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);

		Effect effect = ModAsset.Null.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		bool flag = false;
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active)
			{
				if (proj.ModProjectile is IOcclusionProjectile ModProj)
				{
					flag = true;
					ModProj.DrawOcclusion(spriteBatch);
				}
			}
		}
		spriteBatch.End();
		return flag;
	}

	private bool DrawEffect(VFXBatch spriteBatch)// 特效层
	{
		spriteBatch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);

		Effect MeleeTrail = ModAsset.FlameTrail.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		MeleeTrail.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.007f);
		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Everglow/Yggdrasil/CorruptWormHive/Projectiles/FlameLine", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		MeleeTrail.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>(ModAsset.DeathSickle_Color_Mod, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
		MeleeTrail.CurrentTechnique.Passes["Trail"].Apply();
		bool flag = false;
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active)
			{
				if (proj.ModProjectile is IOcclusionProjectile ModProj)
				{
					flag = true;
					ModProj.DrawEffect(spriteBatch);
				}
			}
		}
		spriteBatch.End();
		return flag;
	}

	private void GetOrig(GraphicsDevice graphicsDevice)
	{
		graphicsDevice.SetRenderTarget(screen);
		graphicsDevice.Clear(Color.Transparent);
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		Ins.Batch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
		Ins.Batch.End();
	}
}
