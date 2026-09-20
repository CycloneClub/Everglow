namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class TelescopicPlatformVFX : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public int TimeLeft = int.MaxValue;

	public Texture2D PlatformTexture;

	public Texture2D PlatformTexture_Glow;

	public Rectangle FrontPillarFrame;

	public Rectangle BackPillarFrame;

	public Rectangle PlatformFrame;

	public int CurrentHeight;

	public int PillarCount;

	public int BodyDrawOffsetY;

	public Vector2 CurrentPos = Vector2.zeroVector;

	public event Action AddLight;

	public override void Update()
	{
		base.Update();
		TimeLeft--;
		if (TimeLeft <= 0)
		{
			Active = false;
			return;
		}
		if (AddLight is not null && TimeLeft >= 30)
		{
			AddLight.Invoke();
		}
	}

	public void PrepareToKill()
	{
		if (TimeLeft > 30)
		{
			TimeLeft = 30;
		}
	}

	public override void Draw()
	{
		Player player = Main.LocalPlayer;
		TelescopePlatformPlayer tPlayer = player.GetModPlayer<TelescopePlatformPlayer>();
		if (TimeLeft <= 0 || tPlayer == null)
		{
			Active = false;
			return;
		}
		if (!tPlayer.InPlatform)
		{
			PrepareToKill();
		}
		if (tPlayer.Platform == null || tPlayer.NowHeight == 0)
		{
			PrepareToKill();
		}
		else
		{
			CurrentHeight = tPlayer.NowHeight;
			CurrentPos = player.Bottom;
		}
		float perHeight = (float)(CurrentHeight + 24) / PillarCount;
		float pillarLength = MathF.Sqrt(MathF.Pow(FrontPillarFrame.Width, 2) + MathF.Pow(FrontPillarFrame.Height, 2));
		float pillarAngle = MathF.Asin(perHeight / pillarLength * 1.2f);
		float pillarWidth = pillarLength * MathF.Cos(pillarAngle);
		Vector2 bodyPos = CurrentPos - new Vector2(PlatformFrame.Width / 2, PlatformFrame.Height - BodyDrawOffsetY);

		// 下面的支柱
		for (int i = 0; i < PillarCount; i++)
		{
			float height = perHeight * i;
			Vector2 pos = CurrentPos + new Vector2(0, height + perHeight / 2);
			Point point = new Point((int)CurrentPos.X / 16, ((int)CurrentPos.Y + (int)height) / 16);
			float angle = pillarAngle - MathF.PI / 4;
			Color stickColor = Lighting.GetColor(point);
			if (TimeLeft <= 30)
			{
				stickColor = Color.Black * (TimeLeft / 30f);
			}
			Ins.Batch.Draw(PlatformTexture, pos, BackPillarFrame, stickColor, angle, BackPillarFrame.Size() / 2, 1f, SpriteEffects.None);
			Ins.Batch.Draw(PlatformTexture, pos, FrontPillarFrame, stickColor, -angle, FrontPillarFrame.Size() / 2, 1f, SpriteEffects.None);
		}
		Color platformColor = Lighting.GetColor(player.Center.ToTileCoordinates());
		if (TimeLeft <= 30)
		{
			platformColor = Color.Black * (TimeLeft / 30f);
		}
		Ins.Batch.Draw(PlatformTexture, bodyPos, PlatformFrame, platformColor);
		if (PlatformTexture_Glow != null)
		{
			Ins.Batch.Draw(PlatformTexture_Glow, bodyPos, PlatformFrame, new Color(1f, 1f, 1f, 0));
		}
	}
}
