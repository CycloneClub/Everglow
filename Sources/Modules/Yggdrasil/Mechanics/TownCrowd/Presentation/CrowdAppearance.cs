namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

[Pipeline(typeof(WCSPipeline))]
public class CrowdAppearance : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public enum CrowdMoveState
	{
		Stand,
		Walk,
		Jump,
	}

	public Vector2 Position;

	public int State;

	public int Direction;

	public int Frame;

	public int HairStyle;

	public int Sex;

	public int ShoeStyle;

	public Color EyeColor;

	public Color SkinColor;

	public Color HairColor;

	public Color ClothColor;

	public Color ShoeColor;

	/// <summary>Updates presentation from the owning traveler.</summary>
	public Action<CrowdAppearance> UpdateController;

	/// <summary>Allows an owning world object to draw this person behind its foreground.</summary>
	public Func<bool> DrawInForeground;

	public float Opacity = 1f;

	public float RetirementOpacity = 1f;

	public float EffectiveOpacity => Opacity * RetirementOpacity;

	public bool Sitting;

	public float SeatYOffset = -4;

	// PlayerDrawLayers.DrawSittingLegs, ordinary clothing branch. Position uses our 48-pixel body.
	internal static void SittingLegStrip(Vector2 position, int direction, int strip, float seatOffset, out Rectangle frame, out Vector2 target)
	{
		frame = new Rectangle(0, 42 + strip * 2, 40, strip == 2 ? 10 : 2);
		target = position + new Vector2((2 + strip * 2) * direction, 38 + seatOffset);
	}

	/// <summary>Creates an appearance without registering or simulating a person.</summary>
	internal static CrowdAppearance Create(Vector2 position)
	{
		CrowdAppearance person = new CrowdAppearance()
		{
			Active = true,
			Visible = true,
			Position = position,
			State = (int)CrowdAppearance.CrowdMoveState.Walk,
			SkinColor = ChooseSkinColor(),
			HairColor = new Vector3(Main.rand.NextFloat(360), MathF.Pow(Main.rand.NextFloat(), 0.25f), MathF.Pow(Main.rand.NextFloat(), 0.5f)).HSVToRGB_Color(1f),
			ClothColor = new Vector3(Main.rand.NextFloat(360), MathF.Pow(Main.rand.NextFloat(), 2f), MathF.Pow(Main.rand.NextFloat(), 1.2f)).HSVToRGB_Color(1f),
			ShoeColor = new Vector3(Main.rand.NextFloat(-30, 60), MathF.Pow(Main.rand.NextFloat(), 2f), MathF.Pow(Main.rand.NextFloat(), 2.5f)).HSVToRGB_Color(1f),
			EyeColor = new Vector3(Main.rand.NextFloat(360), MathF.Pow(Main.rand.NextFloat(), 0.5f), Main.rand.NextFloat()).HSVToRGB_Color(1f),
			HairStyle = Main.rand.Next(165),
			Direction = Main.rand.NextBool() ? -1 : 1,
			Sex = Main.rand.NextBool() ? 0 : 1,
			ShoeStyle = Main.rand.Next(8),
		};
		return person;
	}

	private static Color ChooseSkinColor()
	{
		switch (Main.rand.Next(7))
		{
			case 0:
				return new Color(209, 160, 156);
			case 1:
				return new Color(255, 221, 221);
			case 2:
				return new Color(255, 221, 193);
			case 3:
				return new Color(206, 135, 111);
			case 4:
				return new Color(104, 66, 75);
			case 5:
				return new Color(232, 205, 176);
			case 6:
				return new Color(232, 202, 194);
		}

		return new Color(209, 160, 156);
	}

	public override void Update() => UpdateController?.Invoke(this);

	public override void Draw()
	{
		if (DrawInForeground?.Invoke() != false)
		{
			DrawAppearance(null);
		}
	}

	internal void DrawAppearance(SpriteBatch spriteBatch)
	{
		void DrawPart(Texture2D texture, Vector2 position, Rectangle frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, bool upperBody = true)
		{
			if (Sitting && upperBody)
			{
				position.Y += SeatYOffset;
			}
			if (spriteBatch == null)
			{
				Ins.Batch.Draw(texture, position, frame, color, rotation, origin, scale, effects);
			}
			else
			{
				spriteBatch.Draw(texture, position - Main.screenPosition, frame, color, rotation, origin, scale, effects, 0);
			}
		}

		// Eye and Head
		SpriteEffects flip = Direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		Texture2D head = Terraria.GameContent.TextureAssets.Players[0, 0].Value;
		Rectangle headFrame = new Rectangle(0, Frame * 56, 40, 56);
		DrawPart(head, Position + new Vector2(0, 52), headFrame, AppearanceColor(SkinColor), 0, new Vector2(headFrame.Width * 0.5f, headFrame.Height), 1f, flip);
		Texture2D sclera = Terraria.GameContent.TextureAssets.Players[0, 1].Value;
		DrawPart(sclera, Position + new Vector2(0, 52), headFrame, AppearanceColor(Color.White), 0, new Vector2(headFrame.Width * 0.5f, headFrame.Height), 1f, flip);
		Texture2D pupil = Terraria.GameContent.TextureAssets.Players[0, 2].Value;
		DrawPart(pupil, Position + new Vector2(0, 52), headFrame, AppearanceColor(EyeColor), 0, new Vector2(headFrame.Width * 0.5f, headFrame.Height), 1f, flip);

		float bodyOffset = 0;
		if (Frame is >= 14 and <= 16 || Frame is >= 7 and <= 9)
		{
			bodyOffset = -2;
		}

		// Chest
		Texture2D chest = Terraria.GameContent.TextureAssets.Players[0, 3].Value;
		Rectangle chestFrame = new Rectangle(0, 0, 40, 56);
		DrawPart(chest, Position + new Vector2(0, 24 + bodyOffset), chestFrame, AppearanceColor(SkinColor), 0, chestFrame.Size() * 0.5f, 1f, flip);

		// Armx and hands
		Texture2D arm = Terraria.GameContent.TextureAssets.Players[0, 4].Value;
		Texture2D hand = Terraria.GameContent.TextureAssets.Players[0, 5].Value;
		Rectangle armFrame = new Rectangle(120, 56, 40, 56);
		int offsetFrame = Frame + 2;
		offsetFrame -= 6;
		offsetFrame %= 14;
		offsetFrame += 6;
		if (offsetFrame is 6 or 7 or 19)
		{
			armFrame.X = 160;
		}
		if (offsetFrame is 8 or 9 or 17 or 18)
		{
			armFrame.X = 120;
		}
		if (offsetFrame is 10 or 11 or 15 or 16)
		{
			armFrame.X = 200;
		}
		if (offsetFrame is 12 or 13 or 14)
		{
			armFrame.X = 240;
		}
		if (State == (int)CrowdMoveState.Stand)
		{
			armFrame = new Rectangle(80, 0, 40, 56);
		}
		Rectangle armFrame_back = armFrame;
		armFrame_back.Y += 112;
		Rectangle handFrame = armFrame;
		Rectangle handFrame_back = armFrame;
		handFrame_back.Y += 112;
		DrawPart(hand, Position + new Vector2(0, 24 + bodyOffset), handFrame_back, AppearanceColor(SkinColor), 0, handFrame_back.Size() * 0.5f, 1f, flip);
		DrawPart(arm, Position + new Vector2(0, 24 + bodyOffset), armFrame_back, AppearanceColor(SkinColor), 0, armFrame_back.Size() * 0.5f, 1f, flip);

		// Clothes
		Texture2D innerCloth = Terraria.GameContent.TextureAssets.Players[0, 6].Value;
		Texture2D sleeve = Terraria.GameContent.TextureAssets.Players[0, 8].Value;
		Rectangle innerClothFrame = chestFrame;
		if (Sex == 1)
		{
			innerClothFrame.Y += 112;
		}
		DrawPart(innerCloth, Position + new Vector2(0, 24 + bodyOffset), innerClothFrame, AppearanceColor(ClothColor), 0, innerClothFrame.Size() * 0.5f, 1f, flip);
		DrawPart(sleeve, Position + new Vector2(0, 24 + bodyOffset), armFrame_back, AppearanceColor(ClothColor), 0, armFrame_back.Size() * 0.5f, 1f, flip);
		DrawPart(hand, Position + new Vector2(0, 24 + bodyOffset), handFrame, AppearanceColor(SkinColor), 0, handFrame.Size() * 0.5f, 1f, flip);
		DrawPart(arm, Position + new Vector2(0, 24 + bodyOffset), armFrame, AppearanceColor(SkinColor), 0, armFrame.Size() * 0.5f, 1f, flip);
		DrawPart(sleeve, Position + new Vector2(0, 24 + bodyOffset), armFrame, AppearanceColor(ClothColor), 0, armFrame.Size() * 0.5f, 1f, flip);

		// Foot
		Texture2D foot = Terraria.GameContent.TextureAssets.Players[0, 10].Value;
		Rectangle footFrame = new Rectangle(0, Frame * 56, 40, 56);
		void DrawLegs(Texture2D texture, Color color)
		{
			if (!Sitting)
			{
				DrawPart(texture, Position + new Vector2(0, -4), footFrame, color, 0, new Vector2(20, 0), 1, flip, false);
				return;
			}
			// Same strip construction as vanilla sitting legs: bent thighs, then a lower leg.
			for (int strip = 2; strip >= 0; strip--)
			{
				SittingLegStrip(Position, Direction, strip, SeatYOffset, out var frame, out var target);
				DrawPart(texture, target, frame,
					color, 0, new Vector2(20, 0), 1, flip, false);
			}
		}
		DrawLegs(foot, AppearanceColor(ShoeStyle % 2 == 0 ? ShoeColor : SkinColor));

		// Shoes
		Texture2D shoe = Terraria.GameContent.TextureAssets.Players[0, 11].Value;
		switch (ShoeStyle)
		{
			case 0:
				shoe = Terraria.GameContent.TextureAssets.Players[0, 11].Value;
				break;
			case 1:
				shoe = Terraria.GameContent.TextureAssets.Players[0, 12].Value;
				break;
			case 2:
				shoe = Terraria.GameContent.TextureAssets.Players[1, 11].Value;
				break;
			case 3:
				shoe = Terraria.GameContent.TextureAssets.Players[1, 12].Value;
				break;
			case 4:
				shoe = Terraria.GameContent.TextureAssets.Players[2, 11].Value;
				break;
			case 5:
				shoe = Terraria.GameContent.TextureAssets.Players[2, 12].Value;
				break;
			case 6:
				shoe = Terraria.GameContent.TextureAssets.Players[3, 11].Value;
				break;
			case 7:
				shoe = Terraria.GameContent.TextureAssets.Players[3, 12].Value;
				break;
		}

		DrawLegs(shoe, AppearanceColor(ShoeColor));

		// Hair
		Main.instance.LoadHair(HairStyle);
		Texture2D hair = Terraria.GameContent.TextureAssets.PlayerHair[HairStyle].Value;
		Rectangle hairFrame = new Rectangle(0, (Frame - 6) * 56, 40, 56);
		if (Frame < 6)
		{
			hairFrame.Y = 0;
		}
		DrawPart(hair, Position + new Vector2(0, 24), hairFrame, AppearanceColor(HairColor), 0, hairFrame.Size() * 0.5f, 1f, flip);
	}

	private Color AppearanceColor(Color color) => Lighting.GetColor(Position.ToTileCoordinates(), color) * EffectiveOpacity;
}
