namespace Everglow.Commons.Graphics;

public static class CustomBlendStates
{
	public static readonly BlendState Subtract = NewSubtract();

	public static readonly BlendState Reverse = NewBlendState("BlendState.Reverse", Blend.InverseDestinationColor, Blend.SourceAlpha, Blend.InverseSourceColor, Blend.SourceAlpha);

	public static readonly BlendState SoftAdditive = NewBlendState("BlendState.SoftAdditve", Blend.InverseDestinationColor, Blend.SourceAlpha, Blend.One, Blend.One);

	public static readonly BlendState MinAlpha = NewMinAlpha();

	public static BlendState NewMinAlpha()
	{
		BlendState bs = NewBlendState("BlendState.MinAlpha", Blend.One, Blend.One, Blend.Zero, Blend.Zero);
		bs.ColorBlendFunction = BlendFunction.Add;
		bs.AlphaBlendFunction = BlendFunction.Min;
		return bs;
	}

	public static BlendState NewSubtract()
	{
		BlendState bs = NewBlendState("BlendState.Subtract", Blend.SourceAlpha, Blend.SourceAlpha, Blend.One, Blend.One);
		bs.ColorBlendFunction = BlendFunction.ReverseSubtract;
		bs.AlphaBlendFunction = BlendFunction.ReverseSubtract;
		return bs;
	}

	public static BlendState NewBlendState(string name, Blend colorSourceBlend, Blend alphaSourceBlend, Blend colorDestBlend, Blend alphaDestBlend)
	{
		BlendState bs = new BlendState();
		bs.ColorSourceBlend = Blend.One;
		bs.AlphaSourceBlend = Blend.One;
		bs.ColorDestinationBlend = Blend.Zero;
		bs.AlphaDestinationBlend = Blend.Zero;
		bs.ColorBlendFunction = BlendFunction.Add;
		bs.AlphaBlendFunction = BlendFunction.Add;

		bs.ColorWriteChannels = ColorWriteChannels.All;
		bs.ColorWriteChannels1 = ColorWriteChannels.All;
		bs.ColorWriteChannels2 = ColorWriteChannels.All;
		bs.ColorWriteChannels3 = ColorWriteChannels.All;
		bs.BlendFactor = Color.White;
		bs.MultiSampleMask = -1;

		bs.Name = name;
		bs.ColorSourceBlend = colorSourceBlend;
		bs.AlphaSourceBlend = alphaSourceBlend;
		bs.ColorDestinationBlend = colorDestBlend;
		bs.AlphaDestinationBlend = alphaDestBlend;

		return bs;
	}
}
