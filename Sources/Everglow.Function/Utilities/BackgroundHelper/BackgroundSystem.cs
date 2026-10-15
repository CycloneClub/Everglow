using Everglow.Commons.DataStructures;
using Everglow.Commons.Enums;

namespace Everglow.Commons.Utilities.BackgroundHelper;

public class BackgroundSystem : ModSystem
{
	private const int MaxSingleInstanceNumber = 20;
	public const int MaxMultipleInstanceNumber = 50;

	private List<BackgroundSlideBase> backgroundSlides;

	public override void Load()
	{
		backgroundSlides = [];
	}

	public override void Unload()
	{
		backgroundSlides.Clear();
		backgroundSlides = null;
	}

	public override void OnModLoad()
	{
		if (!NetUtils.IsServer)
		{
			Ins.HookManager.AddHook(CodeLayer.PostDrawBG, DrawBackground);
		}
	}

	public override void OnWorldUnload()
	{
		backgroundSlides.Clear();
	}

	public override void PostUpdateEverything()
	{
		backgroundSlides.RemoveAll(x => x.Active == false);
		foreach (var slide in backgroundSlides)
		{
			slide.Update();
		}
	}

	private void DrawBackground()
	{
		if (backgroundSlides.Count <= 0)
		{
			return;
		}

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
		Effect lastEffect = null;
		foreach (var bg in backgroundSlides.OrderBy(b => b.LayerPriority).ThenByDescending(b => b.Distance)) // Adapt to dynamic distance.
		{
			bool shouldChangeSpriteBatch = bg.Shader != lastEffect;
			if (shouldChangeSpriteBatch)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
				bg.Shader?.CurrentTechnique.Passes[0].Apply();
			}
			bg.Draw();
			lastEffect = bg.Shader;
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public bool AddBackgroundSlide(BackgroundSlideBase bg)
	{
		if (!bg.AllowMultiple)
		{
			if (backgroundSlides.Any(x => x.UniqueName == bg.UniqueName))
			{
				return false;
			}

			if (MaxSingleInstanceNumber > 0)
			{
				if (backgroundSlides.Count(x => !x.AllowMultiple) > MaxSingleInstanceNumber)
				{
					return false;
				}
			}
		}
		else
		{
			if (bg.MaxInstanceNumber > 0)
			{
				if (backgroundSlides.Count(x => x.GetType() == bg.GetType()) > bg.MaxInstanceNumber)
				{
					return false;
				}
			}
		}

		backgroundSlides.Add(bg);
		bg.SetDefaults();

		return true;
	}

	public bool HasBgSlide(string uniqueName)
	{
		return backgroundSlides.Any(x => x.UniqueName == uniqueName);
	}
}
