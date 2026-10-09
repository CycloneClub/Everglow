using Everglow.Myth.Common;
using Terraria.Graphics.Effects;

namespace Everglow.Myth.LanternMoon.Skies;

public class LanternSky : CustomSky
{
	public static bool Open = false;
	public override void Deactivate(params object[] args)
	{
		skyActive = false;
	}

	public override void Reset()
	{
		skyActive = false;
	}

	public override bool IsActive()
	{
		return skyActive || opacity > 0f;
	}
	public override void Activate(Vector2 position, params object[] args)
	{
		TimeLeft = 600;
		StarPos = Vector2.Zero;
		OldStar = new Vector2[240];
		HitTimer = 0;
		MoonLight = 0;
		skyActive = true;
	}
	public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
	{
		if (maxDepth >= 3E+38f && minDepth < 3E+38f)
		{
			Texture2D LightE = ModAsset.LightEffect.Value;
			Main.spriteBatch.Draw(LightE, StarPos, null, new Color(0.3f, 0.21f, 0, 0), -(float)Math.Sin(Main.time / 26d) + 0.6f, new Vector2(128f, 128f), (1.5f + (float)(0.75 * Math.Sin(Main.time / 26d))) * 0.05f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(LightE, StarPos, null, new Color(1f, 0.7f, 0, 0), (float)Math.Sin(Main.time / 12d + 2) + 1.6f, new Vector2(128f, 128f), (1.5f + (float)(0.75 * Math.Sin(Main.time / 26d))) * 0.05f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(LightE, StarPos, null, new Color(0.3f, 0.21f, 0, 0), (float)Math.PI / 2f + (float)(Main.time / 9d), new Vector2(128f, 128f), (1.5f + (float)(0.75 * Math.Sin(Main.time / 26d + 1.57))) * 0.05f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(LightE, StarPos, null, new Color(1f, 0.7f, 0, 0), (float)(Main.time / 26d), new Vector2(128f, 128f), (1.5f + (float)(0.75 * Math.Sin(Main.time / 26d + 3.14))) * 0.05f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(LightE, StarPos, null, new Color(1f, 0.7f, 0, 0), -(float)(Main.time / 26d), new Vector2(128f, 128f), (1.5f + (float)(0.75 * Math.Sin(Main.time / 26d + 4.71))) * 0.05f, SpriteEffects.None, 0);
		}
		Texture2D LMoon = ModAsset.LanternMoon.Value;
		float HalfMaxTime = Main.dayTime ? 27000 : 16200;
		float rotation = (float)(Main.time / HalfMaxTime) - 7.3f;
		if ((StarPos - MythContent.GetSunPos()).Length() < 30)
		{
			HitTimer++;
			MoonLight = Math.Clamp(HitTimer * HitTimer / 500f, 0, 1);
			spriteBatch.Draw(LMoon, MythContent.GetSunPos(), new Rectangle(0, Main.moonPhase * 25, 50, 50), new Color(MoonLight, MoonLight, MoonLight, MoonLight), rotation, new Vector2(25), Main.ForcedMinimumZoom, SpriteEffects.None, 0);
		}
		if (StarPos == Vector2.Zero)
		{
			StarPos = new Vector2(Main.screenWidth, Main.screenHeight * 2) - MythContent.GetSunPos();
			StarVel = Vector2.Normalize(MythContent.GetSunPos() - StarPos).RotatedBy(0.6);
		}
		var StarAcc = Vector2.Normalize(MythContent.GetSunPos() - StarVel * 4f - StarPos);
		StarVel = StarVel * 0.99f + StarAcc * 0.0095f;
		StarPos += StarVel;
		var bars = new List<Vertex2D>();
		float width = 6;
		if (TimeLeft < 60)
		{
			width = TimeLeft / 10f;
		}

		OldStar[0] = StarPos;
		for (int x = OldStar.Length - 1; x > 0; x--)
		{
			OldStar[x] = OldStar[x - 1];
		}
		int TrueL = 0;
		for (int i = 1; i < OldStar.Length; ++i)
		{
			TrueL++;
			if (OldStar[i] == Vector2.Zero)
			{
				break;
			}
		}
		for (int i = 1; i < OldStar.Length; ++i)
		{
			if (OldStar[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = OldStar[i - 1] - OldStar[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

			var factor = i / (float)TrueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);

			bars.Add(new Vertex2D(OldStar[i] + normalDir * width, new Color(255, 0, 0, 0), new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(OldStar[i] + normalDir * -width, new Color(255, 0, 0, 0), new Vector3(factor, 0, w)));
		}
		var Vx = new List<Vertex2D>();
		if (bars.Count > 2)
		{
			Vx.Add(bars[0]);
			var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + Vector2.Normalize(StarVel) * 30, new Color(255, 0, 0, 0), new Vector3(0, 0.5f, 1));
			Vx.Add(bars[1]);
			Vx.Add(vertex);
			for (int i = 0; i < bars.Count - 2; i += 2)
			{
				Vx.Add(bars[i]);
				Vx.Add(bars[i + 2]);
				Vx.Add(bars[i + 1]);

				Vx.Add(bars[i + 1]);
				Vx.Add(bars[i + 2]);
				Vx.Add(bars[i + 3]);
			}
		}
		if (Vx.Count > 2)
		{
			Texture2D t = ModAsset.LBloodEffect.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (skyActive && opacity < 1f)
		{
			opacity += 0.02f;
			return;
		}
		if (!skyActive && opacity > 0f)
		{
			opacity -= 0.02f;
		}

		TimeLeft--;
		if (TimeLeft <= 0)
		{
			Deactivate();
		}
	}
	public override float GetCloudAlpha()
	{
		return (1f - opacity) * 0.97f + 0.03f;
	}
	private Vector2 StarPos = Vector2.Zero;

	private Vector2 StarVel;

	private Vector2[] OldStar = new Vector2[240];

	private bool skyActive;

	private float opacity;

	private float MoonLight;

	private int HitTimer;

	public int TimeLeft = 600;
}
