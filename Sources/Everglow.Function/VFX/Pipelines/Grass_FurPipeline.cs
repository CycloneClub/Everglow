using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.VFX.Pipelines;

/// <summary>
/// 草皮Pipeline,合批到RenderTarget2D上进行脏绘制,用力场图使草皮弯曲/*开销过大暂且停用*/,会自动-Main.screenPosition
/// </summary>
public class Grass_FurPipeline : Pipeline
{
	private float windTimer; // 风速变化计时器
	private RenderTarget2D saveScreenTarget; // 保存的原始屏幕
	private RenderTarget2D grass_FurScreen; // 草皮RenderTarget
	private RenderTarget2D grass_FurScreenSwapII; // 旧草皮RenderTarget
	private RenderTarget2D pushForceField; // 力场,红绿0.5的时候不扭,红0扭右绿0扭下蓝强度倍率,
	private RenderTarget2D grassHardnessScreen; // 可扭性图
	private RenderTarget2D grassHardnessScreenII; // 旧可扭性图
	public static Vector2 TotalMovedPosition; // 距离上次刷新的屏幕坐标
	public static Vector2 LastRenderPosition; // 上次刷新的屏幕坐标
	public static float RefreshDistance = 200f; // 刷新距离阈值
	public static int RenderStyle = 0; // 0的时候画草,1的时候画可扭性图
	public static bool ShouldUpdateRenderTarget; // 如果物块的帧发生了更新就重置画板

	public override void Load()
	{
		Ins.MainThread.AddTask(() =>
		{
			AllocateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));
		});
		Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
		{
			saveScreenTarget?.Dispose();
			grass_FurScreen?.Dispose();
			grass_FurScreenSwapII?.Dispose();
			pushForceField?.Dispose();
			grassHardnessScreen?.Dispose();
			grassHardnessScreenII?.Dispose();
			AllocateRenderTarget(size);
		}, "Realloc RenderTarget");
		effect = VFXManager.DefaultEffect;
	}

	private void AllocateRenderTarget(Vector2 size)
	{
		var gd = Main.instance.GraphicsDevice;
		saveScreenTarget = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		grass_FurScreen = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		grass_FurScreenSwapII = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		pushForceField = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		grassHardnessScreen = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		grassHardnessScreenII = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
	}

	/// <summary>
	/// 绘制推动力场
	/// </summary>
	public void UpdateForceField()
	{
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;

		// 保存原画
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Invert(Main.GameViewMatrix.TransformationMatrix));
		var cur = Main.screenTarget;
		gd.SetRenderTarget(saveScreenTarget);
		gd.Clear(Color.Transparent);
		sb.Draw(cur, Vector2.Zero, Color.White);
		sb.End();

		// 推动力场
		float timeValue = windTimer * 0.003f;
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		gd.SetRenderTarget(pushForceField);
		gd.Clear(Color.Black);
		Color windPushColor = new Color(0.5f + Main.windSpeedCurrent * 0.5f, 0.5f, 1f, 0);
		Color dampingColor = new Color(0.5f, 0.5f, 0f, 1 - MathF.Abs(Main.windSpeedCurrent));

		// 绘制风流
		List<Vertex2D> currentPush = new List<Vertex2D>();
		currentPush.Add(new Vector2(0, 0), windPushColor, new Vector3(timeValue, 0, 0));
		currentPush.Add(new Vector2(Main.screenWidth, 0), windPushColor, new Vector3(1f + timeValue, 0f, 0));
		currentPush.Add(new Vector2(0, Main.screenHeight), windPushColor, new Vector3(timeValue, 1f, 0));
		currentPush.Add(new Vector2(Main.screenWidth, Main.screenHeight), windPushColor, new Vector3(1f + timeValue, 1f, 0));
		if (currentPush.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Noise_rgb.Value;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, currentPush.ToArray(), 0, currentPush.Count - 2);
		}

		// 绘制中和阻滞层
		// List<Vertex2D> dampingLayer = new List<Vertex2D>();
		// dampingLayer.Add(new Vector2(0, 0), dampingColor, new Vector3(0, 0, 0));
		// dampingLayer.Add(new Vector2(Main.screenWidth, 0), dampingColor, new Vector3(0, 0, 0));
		// dampingLayer.Add(new Vector2(0, Main.screenHeight), dampingColor, new Vector3(0, 0, 0));
		// dampingLayer.Add(new Vector2(Main.screenWidth, Main.screenHeight), dampingColor, new Vector3(0, 0, 0));
		// if (dampingLayer.Count > 2)
		// {
		// Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
		// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, dampingLayer.ToArray(), 0, dampingLayer.Count - 2);
		// }

		// foreach(Player player in Main.player)
		// {
		// if(player == null || player.dead || !player.active)
		// {
		// continue;
		// }
		// //玩家推挤
		// List<Vertex2D> playerPress = new List<Vertex2D>();
		// playerPress.Add(player.Hitbox.TopLeft() - LastRenderPosition, Color.White, new Vector3(0, 0, 0));
		// playerPress.Add(player.Hitbox.TopRight() - LastRenderPosition, Color.White, new Vector3(1, 0f, 0));
		// playerPress.Add(player.Hitbox.BottomLeft() - LastRenderPosition, Color.White, new Vector3(0, 1f, 0));
		// playerPress.Add(player.Hitbox.BottomRight() - LastRenderPosition, Color.White, new Vector3(1f, 1f, 0));
		// if (playerPress.Count > 2)
		// {
		// Main.graphics.GraphicsDevice.Textures[0] = ModAsset.BoxPush.Value;
		// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, playerPress.ToArray(), 0, playerPress.Count - 2);
		// }
		// }
		sb.End();
		gd.SetRenderTarget(Main.screenTarget);
		gd.Clear(Color.Transparent);
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		sb.Draw(saveScreenTarget, Vector2.Zero, Color.White);
		sb.End();
	}

	public override void BeginRender()
	{
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;

		// 保存原画
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Invert(Main.GameViewMatrix.TransformationMatrix));
		var cur = Main.screenTarget;
		gd.SetRenderTarget(saveScreenTarget);
		gd.Clear(Color.Transparent);
		sb.Draw(cur, Vector2.Zero, Color.White);
		sb.End();

		// 保存旧草皮RenderTarget
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		gd.SetRenderTarget(grass_FurScreenSwapII);
		gd.Clear(Color.Transparent);
		sb.Draw(grass_FurScreen, Vector2.Zero, Color.White);
		sb.End();
		////保存旧可扭性图RenderTarget
		// sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		// gd.SetRenderTarget(grassHardnessScreenII);
		// gd.Clear(Color.Transparent);
		// sb.Draw(grassHardnessScreen, Vector2.Zero, Color.White);
		// sb.End();
		// 准备绘制刷新后的草皮
		gd.SetRenderTarget(grass_FurScreen);
		gd.Clear(Color.Transparent);
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		sb.Draw(grass_FurScreenSwapII, -TotalMovedPosition, Color.White);
		sb.End();
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
			Main.GameViewMatrix.EffectMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.CurrentTechnique.Passes[0].Apply();
	}

	public void ClearRender()
	{
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;

		// 保存原画
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Invert(Main.GameViewMatrix.TransformationMatrix));
		var cur = Main.screenTarget;
		gd.SetRenderTarget(saveScreenTarget);
		gd.Clear(Color.Transparent);
		sb.Draw(cur, Vector2.Zero, Color.White);
		sb.End();

		gd.SetRenderTarget(grass_FurScreen);
		gd.Clear(Color.Transparent);

		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
			Main.GameViewMatrix.EffectMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.CurrentTechnique.Passes[0].Apply();
	}

	public override void Render(IEnumerable<IVisual> visuals)
	{
		if (grass_FurScreen == null)
		{
			return;
		}

		// windTimer += Main.windSpeedCurrent;
		TotalMovedPosition = Main.screenPosition - LastRenderPosition;

		// UpdateForceField();
		EndRender(); // 未超出刷新阈值,直接画已经处理好的RenderTarget即可结束渲染

		// 满足以下条件刷新RenderTarget
		bool condition1 = TotalMovedPosition.Length() > RefreshDistance;
		bool condition2 = ShouldUpdateRenderTarget;

		if (condition2)// 由于物块变化引起的刷新
		{
			var sb = Main.spriteBatch;
			var gd = Main.instance.GraphicsDevice;

			ClearRender();

			LastRenderPosition = Main.screenPosition;
			RenderStyle = 0;
			foreach (var visual in visuals)
			{
				visual.Draw();
			}
			Ins.Batch.End();

			gd.SetRenderTarget(Main.screenTarget);
			gd.Clear(Color.Transparent);
			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			sb.Draw(saveScreenTarget, Vector2.Zero, Color.White);
			sb.End();

			ShouldUpdateRenderTarget = false;
		}
		else if (condition1)
		{
			var sb = Main.spriteBatch;
			var gd = Main.instance.GraphicsDevice;
			BeginRender();

			LastRenderPosition = Main.screenPosition;
			RenderStyle = 0;
			foreach (var visual in visuals)
			{
				visual.Draw();
			}
			Ins.Batch.End();
			////准备绘制刷新后的可扭性图,默认为可扭性最差的黑色底
			// gd.SetRenderTarget(grassHardnessScreen);
			// gd.Clear(Color.Black);
			// sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
			// sb.Draw(grassHardnessScreenII, -TotalMovedPosition, Color.White);
			// sb.End();

			// RenderStyle = 1;
			// Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
			// effect.Value.Parameters["uTransform"].SetValue(
			// Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
			// Main.GameViewMatrix.EffectMatrix *
			// Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
			// effect.Value.CurrentTechnique.Passes[0].Apply();
			// foreach (var visual in visuals)
			// {
			// visual.Draw();
			// }
			// Ins.Batch.End();
			// 绘制屏幕
			gd.SetRenderTarget(Main.screenTarget);
			gd.Clear(Color.Transparent);
			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			sb.Draw(saveScreenTarget, Vector2.Zero, Color.White);
			sb.End();
		}
	}

	public override void EndRender()
	{
		Effect screenWarp = ModAsset.GrassWave.Value;
		var sb = Main.spriteBatch;
		List<Vertex2D> bars = new List<Vertex2D>();

		// 直接用已经处理好的RenderTarget渲染。
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		for (int y = 0; y < 31; y++)
		{
			for (int x = 0; x < 30; x++)
			{
				float posX = -TotalMovedPosition.X + x / 30f * grass_FurScreen.Width;
				float posY = -TotalMovedPosition.Y + y / 30f * grass_FurScreen.Height;
				Color lightColor0 = Lighting.GetColor((int)(posX + LastRenderPosition.X + TotalMovedPosition.X) / 16, (int)(posY + LastRenderPosition.Y + TotalMovedPosition.Y) / 16);
				Color lightColor2 = Lighting.GetColor((int)(posX + LastRenderPosition.X + TotalMovedPosition.X) / 16, (int)(posY + LastRenderPosition.Y + grass_FurScreen.Height / 30f + TotalMovedPosition.Y) / 16);
				if (x == 0)
				{
					bars.Add(new Vector2(posX, posY), Color.Transparent, new Vector3(x / 30f, y / 30f, 0));
					bars.Add(new Vector2(posX, posY + grass_FurScreen.Height / 30f), Color.Transparent, new Vector3(x / 30f, (y + 1) / 30f, 0));
				}
				bars.Add(new Vector2(posX, posY), lightColor0, new Vector3(x / 30f, y / 30f, 0));
				bars.Add(new Vector2(posX, posY + grass_FurScreen.Height / 30f), lightColor2, new Vector3(x / 30f, (y + 1) / 30f, 0));
				if (x == 29)
				{
					bars.Add(new Vector2(posX, posY), Color.Transparent, new Vector3(x / 30f, y / 30f, 0));
					bars.Add(new Vector2(posX, posY + grass_FurScreen.Height / 30f), Color.Transparent, new Vector3(x / 30f, (y + 1) / 30f, 0));
				}
			}
		}
		if (bars.Count > 2)
		{
			// screenWarp.Parameters["strength"].SetValue(0.005f);
			// screenWarp.CurrentTechnique.Passes["SwayGrass"].Apply();
			Main.graphics.GraphicsDevice.Textures[0] = grass_FurScreen; // TextureAssets.MagicPixel.Value,grass_FurScreen

			// Main.graphics.GraphicsDevice.Textures[1] = pushForceField;
			// Main.graphics.GraphicsDevice.Textures[2] = grassHardnessScreen;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		sb.End();

		// 超出刷新阈值的瞬间帧用新处理的RenderTarget。
	}
}
