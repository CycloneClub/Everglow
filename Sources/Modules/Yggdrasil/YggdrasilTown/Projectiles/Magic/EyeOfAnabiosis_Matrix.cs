using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class EyeOfAnabiosis_Matrix : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => Commons.ModAsset.Point_Mod;

	private Player Owner => Main.player[Projectile.owner];

	public float LightStrength { get; set; }

	private Vector2 OwnerMouseScreen
	{
		get => new Vector2(Projectile.ai[0], Projectile.ai[1]);
		set
		{
			Projectile.ai[0] = value.X;
			Projectile.ai[1] = value.Y;
		}
	}

	public int WinkTimer = 0;

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 60;
	}

	public override void AI()
	{
		UpdateLifeTime();
		SyncOwnerMouseWorld();
		Projectile.Center = Owner.MountedCenter + new Vector2(0, -24) * Owner.gravDir;

		if (Owner.controlUseItem)
		{
			LightStrength += 0.01f;
		}
		else
		{
			LightStrength -= 0.01f;
		}
		if (LightStrength >= 1)
		{
			LightStrength = 1;
		}
		else if (LightStrength <= 0.3f)
		{
			LightStrength = 0.3f;
		}

		WinkTimer++;
	}

	public void UpdateLifeTime()
	{
		if (Owner == null
			|| !Owner.active
			|| Owner.dead
			|| Owner.CCed
			|| Owner.noItems)
		{
			Projectile.Kill();
			return;
		}

		if (Owner.HeldItem.type == ModContent.ItemType<EyeOfAnabiosis>())
		{
			Projectile.timeLeft = 60;
		}
	}

	private void SyncOwnerMouseWorld()
	{
		if (Projectile.owner != Main.myPlayer)
		{
			return;
		}

		if (Main.MouseScreen == OwnerMouseScreen)
		{
			return;
		}
		OwnerMouseScreen = Main.MouseScreen;
		Projectile.netUpdate = true;
	}



	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		float wink = Math.Clamp(Math.Min(WinkTimer, Projectile.timeLeft) / 60f, 0, 1);
		wink = MathF.Pow(wink, 2);
		float timeValue = (float)Main.time * 0.03f;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		DrawRing(drawCenter, timeValue, wink);

		// Draw Hexagram
		Color hexColor = new Color(0f, 0.7f, 1f, 0f) * LightStrength;
		DrawEquilateralTriangle(drawCenter, timeValue, wink, hexColor);
		DrawEquilateralTriangle(drawCenter, timeValue, -wink, hexColor);

		DrawEye(drawCenter, timeValue, wink);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		// Draw light on created
		if (WinkTimer < 60f)
		{
			Texture2D light = Commons.ModAsset.StarSlash.Value;
			float dark = (60 - WinkTimer) / 60f;
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0f, 0.7f, 1f, 0f), MathHelper.PiOver2, light.Size() / 2f, new Vector2(dark * dark * 0.8f, 1.4f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0f, 0.7f, 1f, 0f), 0, light.Size() / 2f, new Vector2(dark * dark * 0.4f, 0.5f), SpriteEffects.None, 0);
		}
		return false;
	}

	private void DrawEquilateralTriangle(Vector2 drawCenter, float timeValue, float scale, Color drawColor)
	{
		scale *= 0.4f;
		int length = 10;
		int step = 8;
		float rotation = timeValue * 0.05f;

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.AlienWriting.Value;
		for (int i = 0; i < 3; i++)
		{
			List<Vertex2D> vertexArray = [];
			for (int j = -length; j <= length; j++)
			{
				float yoffset = j % 2 == 0 ? 0 : -step;
				Vector2 positionOffset = new Vector2(step * j * MathF.Sqrt(3), step * length + yoffset).RotatedBy(rotation + i * MathHelper.Pi / 3);
				float lerpValue = (MathF.Sin(20 * positionOffset.ToRotation()) + 1) * 0.5f;
				var finColor = Color.Lerp(drawColor, new Color(0f, 0f, 0.3f, 0f), lerpValue);
				var texCoord = new Vector3(j * 0.1f + (float)Main.timeForVisualEffects * 0.003f, j % 2 != 0 ? 1 : 0.92f, 0);
				vertexArray.Add(drawCenter + scale * positionOffset, finColor, texCoord);
			}
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertexArray.ToArray(), 0, vertexArray.Count - 2);
		}
	}

	private void DrawRing(Vector2 drawCenter, float timeValue, float wink)
	{
		float frameHeightOut = 0.750f;
		float frameHeightIn = 0.875f;

		// dark peripheral ring
		List<Vertex2D> bars = [];
		for (int i = 0; i <= 100; i++)
		{
			Color drawColor = Color.White * 0.8f * wink;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * 0.1f;
			bars.Add(drawCenter + new Vector2(0, 100).RotatedBy(rotValue), drawColor, new Vector3(i / 50f, frameHeightOut, 0));
			bars.Add(drawCenter + new Vector2(0, 80).RotatedBy(rotValue), drawColor, new Vector3(i / 50f, frameHeightIn, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.AlienWriting_black.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// colorful peripheral ring
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			float lerpValue = (MathF.Sin(i / 25f * MathHelper.TwoPi + timeValue) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(0f, 0.7f, 1f, 0f), new Color(0f, 0f, 0.3f, 0f), lerpValue) * wink;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * 0.1f;
			bars.Add(drawCenter + new Vector2(0, 100).RotatedBy(rotValue), drawColor, new Vector3(i / 50f, frameHeightOut, 0));
			bars.Add(drawCenter + new Vector2(0, 80).RotatedBy(rotValue), drawColor, new Vector3(i / 50f, frameHeightIn, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.AlienWriting.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// dark inner ring0
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			Color drawColor = Color.White * 0.3f * wink;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * -0.03f;
			bars.Add(drawCenter + new Vector2(0, 74).RotatedBy(rotValue), drawColor, new Vector3(i / 50f, frameHeightOut, 0));
			bars.Add(drawCenter + new Vector2(0, 62).RotatedBy(rotValue), drawColor, new Vector3(i / 50f, frameHeightIn, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.AlienWriting_black.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// colorful inner ring0
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			float lerpValue = (MathF.Sin(i / 6.25f * MathHelper.TwoPi + timeValue) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(0f, 0.7f, 1f, 0f), new Color(0.1f, 0.2f, 0.6f, 0f), lerpValue) * wink;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * -0.03f;
			bars.Add(drawCenter + new Vector2(0, 74).RotatedBy(rotValue), drawColor, new Vector3(i / 50f, frameHeightOut, 0));
			bars.Add(drawCenter + new Vector2(0, 62).RotatedBy(rotValue), new Color(0f, 0.7f, 1f, 0f) * wink, new Vector3(i / 50f, frameHeightIn, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.AlienWriting.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// dark inner ring1
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			Color drawColor = Color.White * 0.8f * wink;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * 0.03f;
			bars.Add(drawCenter + new Vector2(0, 88).RotatedBy(rotValue), drawColor, new Vector3(i / 25f, 0.2f, 0));
			bars.Add(drawCenter + new Vector2(0, 76).RotatedBy(rotValue), drawColor, new Vector3(i / 25f, 0.5f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1_black.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// colorful inner ring1
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			float lerpValue = (MathF.Sin(i / 12.5f * MathHelper.TwoPi + timeValue) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(0f, 0.7f, 1f, 0f), new Color(0f, 0f, 0.3f, 0f), lerpValue) * wink;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * 0.03f;
			bars.Add(drawCenter + new Vector2(0, 88).RotatedBy(rotValue), new Color(0f, 0.7f, 1f, 0f) * wink * 0.9f, new Vector3(i / 25f, 0.2f, 0));
			bars.Add(drawCenter + new Vector2(0, 76).RotatedBy(rotValue), drawColor, new Vector3(i / 25f, 0.5f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// dark inner ring2
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			Color drawColor = Color.White * 0.3f * wink;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * 0.03f;
			bars.Add(drawCenter + new Vector2(0, 80).RotatedBy(rotValue), drawColor, new Vector3(i / 25f, 0.2f, 0));
			bars.Add(drawCenter + new Vector2(0, 68).RotatedBy(rotValue), drawColor, new Vector3(i / 25f, 0.5f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Star_black.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// colorful inner ring2
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			float lerpValue = (MathF.Sin(i / 6.25f * MathHelper.TwoPi + timeValue) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(0f, 0.7f, 1f, 0f), new Color(0f, 0f, 0.3f, 0f), lerpValue) * wink;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * 0.03f;
			bars.Add(drawCenter + new Vector2(0, 80).RotatedBy(rotValue), drawColor, new Vector3(i / 25f, 0.2f, 0));
			bars.Add(drawCenter + new Vector2(0, 68).RotatedBy(rotValue), drawColor, new Vector3(i / 25f, 0.5f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Textures_Star.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	}

	private void DrawEye(Vector2 drawCenter, float timeValue, float wink)
	{
		List<Vertex2D> bars;

		// Upper eyelids
		bars = [];
		List<Vertex2D> barsBlack = [];
		List<Vertex2D> eyelash = [];
		for (int i = 0; i <= 100; i++)
		{
			float lerpValue = (MathF.Sin(i / 33f * MathHelper.TwoPi + timeValue) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(0f, 0.7f, 1f, 0f), new Color(0f, 0f, 0.3f, 0f), lerpValue) * wink;
			var x = (i - 50) / 50f;
			var y = -1f / MathF.Sqrt(3f) + MathF.Sqrt(4f / 3 - x * x);
			float height = y * wink * 30;
			float deltaX = (i - 50) * 0.5f;
			bars.Add(drawCenter + new Vector2(deltaX, -height - 5), drawColor, new Vector3(i / 25f, 0f, 0));
			bars.Add(drawCenter + new Vector2(deltaX, -height), drawColor, new Vector3(i / 25f, 0.5f, 0));

			barsBlack.Add(drawCenter + new Vector2(deltaX, -height - 10), Color.White * wink * 0.7f, new Vector3(i / 25f, 0f, 0));
			barsBlack.Add(drawCenter + new Vector2(deltaX, -height), Color.White * wink * 0.7f, new Vector3(i / 25f, 0.5f, 0));
			if (i % 10 == 0)
			{
				var eyelashStart = new Vector2(deltaX, -height);
				Vector2 eyelashEnd = Vector2.Normalize(eyelashStart) * 76;
				Vector2 eyelashWidth = Vector2.Normalize(eyelashEnd - eyelashStart).RotatedBy(MathHelper.PiOver2) * 0.5f;
				eyelash.Add(drawCenter + eyelashStart + eyelashWidth * 2, drawColor, new Vector3(i / 25f, 0f + timeValue * 0.1f, 0));
				eyelash.Add(drawCenter + eyelashStart - eyelashWidth * 2, drawColor, new Vector3(i / 25f + 0.25f, 0f + timeValue * 0.1f, 0));
				eyelash.Add(drawCenter + eyelashEnd - eyelashWidth, drawColor, new Vector3(i / 25f + 0.25f, 0.2f + timeValue * 0.1f, 0));

				eyelash.Add(drawCenter + eyelashEnd - eyelashWidth, drawColor, new Vector3(i / 25f + 0.25f, 0.2f + timeValue * 0.1f, 0));
				eyelash.Add(drawCenter + eyelashEnd + eyelashWidth, drawColor, new Vector3(i / 25f, 0.2f + timeValue * 0.1f, 0));
				eyelash.Add(drawCenter + eyelashStart + eyelashWidth * 2, drawColor, new Vector3(i / 25f, 0f + timeValue * 0.1f, 0));
			}
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1_black.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsBlack.ToArray(), 0, barsBlack.Count - 2);

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// Lower eyelids
		bars = [];
		eyelash = [];
		for (int i = 0; i <= 100; i++)
		{
			float lerpValue = (MathF.Sin((100 - i) / 100f * MathHelper.TwoPi + timeValue) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(0f, 0.7f, 1f, 0f), new Color(0f, 0f, 0.3f, 0f), lerpValue) * wink;
			var x = (i - 50) / 50f;
			var y = -1f / MathF.Sqrt(3f) + MathF.Sqrt(4f / 3 - x * x);
			float height = y * wink * -30;
			float deltaX = (i - 50) * 0.5f;
			bars.Add(drawCenter + new Vector2(deltaX, -height - 5), drawColor, new Vector3(i / 25f, 0f, 0));
			bars.Add(drawCenter + new Vector2(deltaX, -height), drawColor, new Vector3(i / 25f, 0.5f, 0));
			if (i % 10 == 0)
			{
				var eyelashStart = new Vector2(deltaX, -height);
				Vector2 eyelashEnd = Vector2.Normalize(eyelashStart) * 76;
				Vector2 eyelashWidth = Vector2.Normalize(eyelashEnd - eyelashStart).RotatedBy(MathHelper.PiOver2) * 0.5f;
				eyelash.Add(drawCenter + eyelashStart + eyelashWidth * 2, drawColor, new Vector3(i / 25f, 0f + timeValue * 0.1f, 0));
				eyelash.Add(drawCenter + eyelashStart - eyelashWidth * 2, drawColor, new Vector3(i / 25f + 0.25f, 0f + timeValue * 0.1f, 0));
				eyelash.Add(drawCenter + eyelashEnd - eyelashWidth, drawColor, new Vector3(i / 25f + 0.25f, 0.2f + timeValue * 0.1f, 0));

				eyelash.Add(drawCenter + eyelashEnd - eyelashWidth, drawColor, new Vector3(i / 25f + 0.25f, 0.2f + timeValue * 0.1f, 0));
				eyelash.Add(drawCenter + eyelashEnd + eyelashWidth, drawColor, new Vector3(i / 25f, 0.2f + timeValue * 0.1f, 0));
				eyelash.Add(drawCenter + eyelashStart + eyelashWidth * 2, drawColor, new Vector3(i / 25f, 0f + timeValue * 0.1f, 0));
			}
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		float pupilOuterRadius = 24 * 0.4f;
		float pupilInnerRadius = 10 * 0.4f;

		// pupil black
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			Color drawColor = Color.Black * wink;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * 0.03f;
			bars.Add(drawCenter + new Vector2(0, pupilOuterRadius * wink).RotatedBy(rotValue), drawColor, new Vector3(i / 20f, 0.2f + timeValue * 0.05f, 0));
			bars.Add(drawCenter + new Vector2(0, pupilInnerRadius * wink).RotatedBy(rotValue), new Color(0f, 0f, 0.3f, 0f), new Vector3(i / 20f, 0.4f + timeValue * 0.05f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_melting.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// pupil
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			float lerpValue = (MathF.Sin(i / 50f * MathHelper.TwoPi + timeValue) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(0f, 0.7f, 1f, 0f), new Color(0f, 0.4f, 0.7f, 0f), lerpValue) * wink * LightStrength;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * 0.03f;
			bars.Add(drawCenter + new Vector2(0, pupilOuterRadius * wink).RotatedBy(rotValue), drawColor, new Vector3(i / 20f, 0.2f + timeValue * 0.05f, 0));
			bars.Add(drawCenter + new Vector2(0, pupilInnerRadius * wink).RotatedBy(rotValue), new Color(0f, 0f, 0.3f, 0f), new Vector3(i / 20f, 0.4f + timeValue * 0.05f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_melting.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// pupil bloom
		bars = [];
		for (int i = 0; i <= 100; i++)
		{
			float lerpValue = (MathF.Sin(i / 50f * MathHelper.TwoPi + timeValue) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(0f, 0.7f, 1f, 0f), new Color(0f, 0.4f, 0.7f, 0f), lerpValue) * wink * LightStrength;
			float rotValue = i / 100f * MathHelper.TwoPi + timeValue * 0.03f;
			bars.Add(drawCenter + new Vector2(0, pupilOuterRadius * wink).RotatedBy(rotValue), drawColor, new Vector3(i / 25f, 0.5f, 0));
			bars.Add(drawCenter + new Vector2(0, 20 * wink).RotatedBy(rotValue), new Color(0f, 0f, 0.3f, 0f), new Vector3(i / 25f, 0.7f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_6.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// pupil center
		bars = [];
		Vector2 addPos = (OwnerMouseScreen - drawCenter) * 0.06f;
		if (addPos.Length() > 6)
		{
			addPos = Vector2.Normalize(addPos) * 6;
		}
		for (int i = 0; i <= 10; i++)
		{
			float lerpValue = (MathF.Sin(i / 5f * MathHelper.TwoPi + timeValue) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(0f, 0.7f, 1f, 0f), new Color(0f, 0.4f, 0.7f, 0f), lerpValue) * wink;
			float rotValue = i / 10f * MathHelper.TwoPi + timeValue * 0.03f;
			bars.Add(drawCenter + addPos + new Vector2(0, 3 * wink).RotatedBy(rotValue), drawColor, new Vector3(i / 25f, 0.5f, 0));
			bars.Add(drawCenter + addPos + new Vector2(0, 1 * wink).RotatedBy(rotValue), new Color(0f, 0f, 0.3f, 0f), new Vector3(i / 25f, 0.7f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_6.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	}
}
