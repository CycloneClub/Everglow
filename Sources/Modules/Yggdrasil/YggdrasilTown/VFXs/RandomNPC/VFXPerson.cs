using Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.RandomNPC;

[Pipeline(typeof(WCSPipeline))]
public class VFXPerson : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public enum MoveState
	{
		Stand,
		Walk,
		Jump,
	}

	public Vector2 Position;

	public Vector2 Velocity;

	public float Timer;

	public float MaxTime;

	public int State;

	public int Direction;

	public int Frame;

	public int HairStyle;

	public int Sex;

	public int ShoeStyle;

	public bool Blocked;

	public Color EyeColor;

	public Color SkinColor;

	public Color HairColor;

	public Color ClothColor;

	public Color ShoeColor;

	public override void OnSpawn()
	{
	}

	public override void Update()
	{
		CheckEliminate();
		UpdateFrame();
		AI();

		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public void AI()
	{
		if (TileUtils.PlatformCollision(Position + new Vector2(-8, 0), 16, 48))
		{
			if (!Blocked)
			{
				Velocity.Y *= 0;
				for (int t = 0; t < 16; t++)
				{
					if (!TileUtils.PlatformCollision(Position + new Vector2(-8, -1), 16, 48))
					{
						break;
					}
					Position.Y -= 1;
					if (t == 15)
					{
						Position.Y += 16;
						Velocity.X *= -1;
						Direction *= -1;
						Blocked = true;
					}
				}
			}
			else
			{
				Velocity.Y *= 0;
				for (int t = 0; t < 32; t++)
				{
					if (!TileUtils.PlatformCollision(Position + new Vector2(-8, -1), 16, 48))
					{
						break;
					}
					Position.Y -= 16;
					if (t == 31)
					{
						Position.Y += 16 * 32;
					}
				}
				for (int t = 0; t < 16; t++)
				{
					if (!TileUtils.PlatformCollision(Position + new Vector2(-8, -1), 16, 48))
					{
						break;
					}
					Position.Y += 16;
					if (t == 15)
					{
						Position.Y -= 16 * 16;
						Active = false;
						return;
					}
				}
				Blocked = false;
			}
		}
		else
		{
			Blocked = false;
			Velocity.Y += 0.25f;
			if (Velocity.Y > 8)
			{
				Velocity.Y = 8;
			}
		}
		if (State == (int)MoveState.Walk)
		{
			Velocity.X = 1.5f * Direction;
			if (Main.rand.NextBool(240))
			{
				State = (int)MoveState.Stand;
			}
		}
		if (State == (int)MoveState.Stand)
		{
			Velocity.X = 0;
			if (Main.rand.NextBool(240))
			{
				State = (int)MoveState.Walk;
				if (Main.rand.NextBool())
				{
					Direction *= -1;
				}
			}
		}
		Position += Velocity;
	}

	public void CheckEliminate()
	{
		if (Main.mouseMiddle && Main.mouseMiddleRelease && Main.LocalPlayer.HeldItem.type == ModContent.ItemType<VFXPersonItem>())
		{
			Active = false;
			return;
		}
	}

	public void UpdateFrame()
	{
		if (Timer % 3 == 0)
		{
			Frame++;
			switch (State)
			{
				case (int)MoveState.Stand:
					{
						Frame = 0;
						break;
					}
				case (int)MoveState.Walk:
					{
						if (Frame >= 20)
						{
							Frame = 6;
						}
						break;
					}
			}
		}
	}

	public override void Draw()
	{
		// Eye and Head
		SpriteEffects flip = Direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		Texture2D head = Terraria.GameContent.TextureAssets.Players[0, 0].Value;
		Rectangle headFrame = new Rectangle(0, Frame * 56, 40, 56);
		Ins.Batch.Draw(head, Position + new Vector2(0, 52), headFrame, Lighting.GetColor(Position.ToTileCoordinates(), SkinColor), 0, new Vector2(headFrame.Width * 0.5f, headFrame.Height), 1f, flip);
		Texture2D sclera = Terraria.GameContent.TextureAssets.Players[0, 1].Value;
		Ins.Batch.Draw(sclera, Position + new Vector2(0, 52), headFrame, Lighting.GetColor(Position.ToTileCoordinates(), Color.White), 0, new Vector2(headFrame.Width * 0.5f, headFrame.Height), 1f, flip);
		Texture2D pupil = Terraria.GameContent.TextureAssets.Players[0, 2].Value;
		Ins.Batch.Draw(pupil, Position + new Vector2(0, 52), headFrame, Lighting.GetColor(Position.ToTileCoordinates(), EyeColor), 0, new Vector2(headFrame.Width * 0.5f, headFrame.Height), 1f, flip);

		float bodyOffset = 0;
		if (Frame is >= 14 and <= 16 || Frame is >= 7 and <= 9)
		{
			bodyOffset = -2;
		}

		// Chest
		Texture2D chest = Terraria.GameContent.TextureAssets.Players[0, 3].Value;
		Rectangle chestFrame = new Rectangle(0, 0, 40, 56);
		Ins.Batch.Draw(chest, Position + new Vector2(0, 24 + bodyOffset), chestFrame, Lighting.GetColor(Position.ToTileCoordinates(), SkinColor), 0, chestFrame.Size() * 0.5f, 1f, flip);

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
		if (State == (int)MoveState.Stand)
		{
			armFrame = new Rectangle(80, 0, 40, 56);
		}
		Rectangle armFrame_back = armFrame;
		armFrame_back.Y += 112;
		Rectangle handFrame = armFrame;
		Rectangle handFrame_back = armFrame;
		handFrame_back.Y += 112;
		Ins.Batch.Draw(hand, Position + new Vector2(0, 24 + bodyOffset), handFrame_back, Lighting.GetColor(Position.ToTileCoordinates(), SkinColor), 0, handFrame_back.Size() * 0.5f, 1f, flip);
		Ins.Batch.Draw(arm, Position + new Vector2(0, 24 + bodyOffset), armFrame_back, Lighting.GetColor(Position.ToTileCoordinates(), SkinColor), 0, armFrame_back.Size() * 0.5f, 1f, flip);

		// Clothes
		Texture2D innerCloth = Terraria.GameContent.TextureAssets.Players[0, 6].Value;
		Texture2D sleeve = Terraria.GameContent.TextureAssets.Players[0, 8].Value;
		Rectangle innerClothFrame = chestFrame;
		if (Sex == 1)
		{
			innerClothFrame.Y += 112;
		}
		Ins.Batch.Draw(innerCloth, Position + new Vector2(0, 24 + bodyOffset), innerClothFrame, Lighting.GetColor(Position.ToTileCoordinates(), ClothColor), 0, innerClothFrame.Size() * 0.5f, 1f, flip);
		Ins.Batch.Draw(sleeve, Position + new Vector2(0, 24 + bodyOffset), armFrame_back, Lighting.GetColor(Position.ToTileCoordinates(), ClothColor), 0, armFrame_back.Size() * 0.5f, 1f, flip);
		Ins.Batch.Draw(hand, Position + new Vector2(0, 24 + bodyOffset), handFrame, Lighting.GetColor(Position.ToTileCoordinates(), SkinColor), 0, handFrame.Size() * 0.5f, 1f, flip);
		Ins.Batch.Draw(arm, Position + new Vector2(0, 24 + bodyOffset), armFrame, Lighting.GetColor(Position.ToTileCoordinates(), SkinColor), 0, armFrame.Size() * 0.5f, 1f, flip);
		Ins.Batch.Draw(sleeve, Position + new Vector2(0, 24 + bodyOffset), armFrame, Lighting.GetColor(Position.ToTileCoordinates(), ClothColor), 0, armFrame.Size() * 0.5f, 1f, flip);

		// Foot
		Texture2D foot = Terraria.GameContent.TextureAssets.Players[0, 10].Value;
		Rectangle footFrame = new Rectangle(0, Frame * 56, 40, 56);
		Ins.Batch.Draw(foot, Position + new Vector2(0, -4), footFrame, Lighting.GetColor(Position.ToTileCoordinates(), ShoeStyle % 2 == 0 ? ShoeColor : SkinColor), 0, new Vector2(footFrame.Width * 0.5f, 0), 1f, flip);

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

		Ins.Batch.Draw(shoe, Position + new Vector2(0, -4), footFrame, Lighting.GetColor(Position.ToTileCoordinates(), ShoeColor), 0, new Vector2(footFrame.Width * 0.5f, 0), 1f, flip);

		// Hair
		Main.instance.LoadHair(HairStyle);
		Texture2D hair = Terraria.GameContent.TextureAssets.PlayerHair[HairStyle].Value;
		Rectangle hairFrame = new Rectangle(0, (Frame - 6) * 56, 40, 56);
		if (Frame < 6)
		{
			hairFrame.Y = 0;
		}
		Ins.Batch.Draw(hair, Position + new Vector2(0, 24), hairFrame, Lighting.GetColor(Position.ToTileCoordinates(), HairColor), 0, hairFrame.Size() * 0.5f, 1f, flip);
	}
}
