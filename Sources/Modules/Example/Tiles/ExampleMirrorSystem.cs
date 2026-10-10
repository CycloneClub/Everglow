using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.Example.Items;
using Terraria.Graphics.Renderers;

namespace Everglow.Example.Tiles;

/// <summary>
/// 为 MirrorWall 背景墙绘制玩家倒影；玩家自身决定镜像轴，墙只限定显示区域。
/// </summary>
public class ExampleMirrorSystem : ModSystem
{
	private readonly List<Point> visibleWalls = new();

	public override void Load()
	{
		if (!Main.dedServ)
		{
			On_Main.DoDraw_WallsAndBlacks += DrawAfterWalls;
		}
	}

	public override void Unload()
	{
		if (!Main.dedServ)
		{
			On_Main.DoDraw_WallsAndBlacks -= DrawAfterWalls;
		}
		visibleWalls.Clear();
	}

	public override void OnWorldLoad() => visibleWalls.Clear();

	public override void OnWorldUnload() => visibleWalls.Clear();

	private void DrawAfterWalls(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
	{
		orig(self);
		if (Main.dedServ || Main.gameMenu || Main.drawToScreen || !Ins.VisualQuality.High)
		{
			return;
		}
		FindVisibleWalls();
		if (visibleWalls.Count == 0)
		{
			return;
		}

		// 原版此处仍有绘制背景墙的批次，先提交；倒影结束后继续正常前景绘制。
		var state = Main.spriteBatch.GetState();
		if (state.HasValue)
		{
			Main.spriteBatch.End();
		}
		try
		{
			DrawMirrors();
		}
		finally
		{
			if (state.HasValue)
			{
				Main.spriteBatch.Begin(state.Value);
			}
		}
	}

	private void FindVisibleWalls()
	{
		visibleWalls.Clear();
		Matrix inverseView = Matrix.Invert(Main.GameViewMatrix.TransformationMatrix);
		Vector2 first = Vector2.Transform(Vector2.Zero, inverseView) + Main.screenPosition;
		Vector2 second = Vector2.Transform(new Vector2(Main.screenWidth, Main.screenHeight), inverseView) + Main.screenPosition;
		Vector2 topLeft = Vector2.Min(first, second);
		Vector2 bottomRight = Vector2.Max(first, second);
		int startX = Math.Max(0, (int)(topLeft.X / 16) - 1);
		int startY = Math.Max(0, (int)(topLeft.Y / 16) - 1);
		int endX = Math.Min(Main.maxTilesX - 1, (int)(bottomRight.X / 16) + 1);
		int endY = Math.Min(Main.maxTilesY - 1, (int)(bottomRight.Y / 16) + 1);
		for (int x = startX; x <= endX; x++)
		{
			for (int y = startY; y <= endY; y++)
			{
				Tile tile = Main.tile[x, y];
				if (!MirrorWall.IsMirror(x, y) || tile.IsWallInvisible)
				{
					continue;
				}
				// 不按 HasTile 丢弃整格；随后绘制的前景纹理负责逐像素遮挡。
				visibleWalls.Add(new Point(x, y));
			}
		}
	}

	private void DrawMirrors()
	{
		GraphicsDevice device = Main.graphics.GraphicsDevice;
		RenderTargetBinding[] bindings = device.GetRenderTargets();
		if (bindings.Length != 1 || bindings[0].RenderTarget != Main.screenTarget)
		{
			return;
		}

		var targets = Ins.RenderTargetPool.GetRenderTarget2DArray(2);
		try
		{
			RenderTarget2D scene = targets.Resource[0];
			RenderTarget2D reflection = targets.Resource[1];
			device.SetRenderTarget(scene);
			CopyScreen(Main.screenTarget);
			device.SetRenderTarget(reflection);
			// 玩家单独捕获，背景不进入倒影纹理。
			device.Clear(Color.Transparent);
			CapturePlayers();

			device.SetRenderTargets(bindings);
			CopyScreen(scene);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
				DepthStencilState.None, RasterizerState.CullNone);
			try
			{
				foreach (Point wall in visibleWalls)
				{
					DrawWallReflection(wall, reflection);
				}
			}
			finally
			{
				Main.spriteBatch.End();
			}
		}
		finally
		{
			try
			{
				// DiscardContents 目标只能在确实切走时恢复，不能重复绑定。
				RenderTargetBinding[] current = device.GetRenderTargets();
				if (current.Length != 1 || current[0].RenderTarget != bindings[0].RenderTarget)
				{
					device.SetRenderTargets(bindings);
					CopyScreen(targets.Resource[0]);
				}
			}
			finally
			{
				targets.Release();
			}
		}
	}

	private static void CapturePlayers()
	{
		ExampleMirrorConfigUI settings = ModContent.GetInstance<ExampleMirrorConfigUI>();
		foreach (Player player in Main.player)
		{
			if (!player.active || player.dead || player.ghost || player.invis)
			{
				continue;
			}
			var camera = Main.Camera;
			SamplerState sampler = player.mount.Active && player.fullRotation != 0
				? LegacyPlayerRenderer.MountedSamplerState : camera.Sampler;
			// DrawPlayer 是底层入口，不会 Begin；必须匹配 DrawPlayerFull 的立即绘制状态。
			Vector2 feet = player.Bottom + new Vector2(0, player.gfxOffY) - Main.screenPosition;
			Matrix reflection = MirrorWallGeometry.CreatePlayerReflectionTransform(
				feet, new Vector2(settings.OffsetX, settings.OffsetY), settings.ReflectionScale,
				settings.FlipHorizontally, camera.GameViewMatrix.TransformationMatrix);
			camera.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, sampler,
				DepthStencilState.None, RasterizerState.CullNone, null, reflection);
			try
			{
				Vector2 position = player.position + new Vector2(0, player.gfxOffY);
				Main.PlayerRenderer.DrawPlayer(camera, player, position, player.fullRotation, player.fullRotationOrigin);
			}
			finally
			{
				camera.SpriteBatch.End();
			}
		}
	}

	private static void CopyScreen(Texture2D texture)
	{
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
			DepthStencilState.None, RasterizerState.CullNone);
		try
		{
			Main.spriteBatch.Draw(texture, Vector2.Zero, Color.White);
		}
		finally
		{
			Main.spriteBatch.End();
		}
	}

	private void DrawWallReflection(Point point, Texture2D reflection)
	{
		Rectangle cell = ScreenRectangle(point.ToVector2() * 16, new Vector2(16));
		Rectangle viewport = new(0, 0, reflection.Width, reflection.Height);
		if (MirrorWallGeometry.TryGetReflectionSlice(cell, viewport, out Rectangle slice))
		{
			Main.spriteBatch.Draw(reflection, slice, slice, Color.White * 0.85f);
		}
	}

	private static Rectangle ScreenRectangle(Vector2 worldPosition, Vector2 size)
	{
		Matrix view = Main.GameViewMatrix.TransformationMatrix;
		Vector2 first = Vector2.Transform(worldPosition - Main.screenPosition, view);
		Vector2 second = Vector2.Transform(worldPosition + size - Main.screenPosition, view);
		Vector2 topLeft = Vector2.Min(first, second);
		Vector2 bottomRight = Vector2.Max(first, second);
		// 共用边界取整，缩放时相邻墙格既不留缝也不重复混合。
		return new Rectangle((int)MathF.Floor(topLeft.X), (int)MathF.Floor(topLeft.Y),
			(int)MathF.Floor(bottomRight.X) - (int)MathF.Floor(topLeft.X),
			(int)MathF.Floor(bottomRight.Y) - (int)MathF.Floor(topLeft.Y));
	}
}
