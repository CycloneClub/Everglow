namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class AnemoShell_Smog : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public int Timer;

	public int TimeMax;

	public Vector2 Center;

	public override void Update()
	{
		base.Update();
		Timer++;
		if (Timer >= TimeMax)
		{
			Active = false;
			return;
		}
	}

	public override void Draw()
	{
		Texture2D smoggy = ModAsset.AnemoShell_Smog.Value;
		int width = 128;
		int height = 110;
		int frame = Timer / 2;
		Rectangle clip = default;
		if (frame < 24)
		{
			int x = frame % 8;
			int y = frame / 8;
			clip.X = x * width;
			clip.Y = y * height;
			clip.Width = width;
			clip.Height = height;
			var drawRect = new Rectangle((int)Center.X - width, (int)Center.Y - height - 60, width * 2, height * 2);
			Ins.Batch.Draw(smoggy, drawRect, clip, Lighting.GetColor(drawRect.Center().ToTileCoordinates()) * ((TimeMax - Timer) / (float)TimeMax));
		}
	}
}
