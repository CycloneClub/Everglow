using Everglow.Commons.Mechanics.CustomBackground;

namespace Everglow.Example.BgSlides;

public class ExampleBgSlideCloud : BackgroundSlideBase
{
	public int TimeLeft = 600;

	public override bool AllowMultiple => true;

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = Terraria.GameContent.TextureAssets.Cloud[Main.rand.Next(Terraria.GameContent.TextureAssets.Cloud.Length)].Value;
		Distance = Main.rand.NextFloat(10f, 120f);
		Scale = 40f;
	}

	public override void FadeIn()
	{
		if (Alpha < 0.5f)
		{
			Alpha += 0.01f;
		}
		else
		{
			Alpha = 0.5f;
		}
	}

	public override void Update()
	{
		base.Update();
		TimeLeft--;
		WorldAnchor += new Vector2(50f / Distance, 0f);
	}

	public override void Draw()
	{
		DrawPreset_Piece(this);
	}

	public override bool CanActive()
	{
		return TimeLeft > 0;
	}
}
