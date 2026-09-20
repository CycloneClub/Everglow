using Terraria.Audio;

namespace Everglow.Yggdrasil.KelpCurtain.CustomTiles;

[Pipeline(typeof(WCSPipeline))]
public class BlackAwningBoat_ControlUI : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public BlackAwningBoat ParentBoat;

	public Vector2 RelativePos;

	public Vector2 Position;

	public Vector2 MouseWorldCurrent;

	public Player Owner;

	public float AnimationTimer;

	public bool Closing = false;

	public UICircle LeftButtom = new(0f, Vector2.Zero, 0);
	public UICircle PauseButtom = new(0f, Vector2.Zero, 1);
	public UICircle RightButtom = new(0f, Vector2.Zero, 2);
	public UICircle LampButtom = new(0f, Vector2.Zero, 3);
	public UICircle KillButtom = new(0f, Vector2.Zero, 4);

	public class UICircle
	{
		public float Scale;
		public Vector2 RelativeCenter;
		public int UIType;

		public UICircle(float size, Vector2 relativeCenter, int uiType)
		{
			Scale = size;
			RelativeCenter = relativeCenter;
			UIType = uiType;
		}
	}

	public override void Update()
	{
		// Data validation
		if (Owner?.HeldItem is null)
		{
			throw new InvalidOperationException("Owner must be initialized correctly before updating.");
		}
		if (Owner == null || ParentBoat is null || !ParentBoat.Active || !Owner.active || Owner.dead)
		{
			Closing = true;
		}
		if (!Closing)
		{
			if (AnimationTimer < 30)
			{
				AnimationTimer += 3;
			}
			else
			{
				AnimationTimer = 30;
			}
			Position = ParentBoat.Box.Center + RelativePos;
			UpdateUI(LeftButtom);
			UpdateUI(PauseButtom);
			UpdateUI(RightButtom);
			UpdateUI(LampButtom);
			UpdateUI(KillButtom);
		}
		else
		{
			if (ParentBoat is not null && ParentBoat.Active)
			{
				Position = ParentBoat.Box.Center + RelativePos;
			}
			UpdateUI(LeftButtom);
			UpdateUI(PauseButtom);
			UpdateUI(RightButtom);
			UpdateUI(LampButtom);
			UpdateUI(KillButtom);
			if (AnimationTimer > 0)
			{
				AnimationTimer -= 3;
			}
			else
			{
				Active = false;
				Visible = false;
				return;
			}
		}
	}

	public void UpdateUI(UICircle ui)
	{
		MouseWorldCurrent = Main.MouseWorld;
		ui.RelativeCenter = new Vector2((ui.UIType - 2) * 30, 60);
		if (AnimationTimer < 30)
		{
			ui.Scale = AnimationTimer / 30f;
		}
		if (AnimationTimer >= 30)
		{
			CheckMouseOver(ui);
		}
		CheckMouseClick(ui);
	}

	public void CheckMouseOver(UICircle ui)
	{
		if (ParentBoat is null || !ParentBoat.Active)
		{
			return;
		}
		if ((MouseWorldCurrent - Position - ui.RelativeCenter).Length() < 14 && ui.Scale < 1.1f)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			ui.Scale = 1.2f;
		}
		if ((MouseWorldCurrent - Position - ui.RelativeCenter).Length() >= 14 && ui.Scale > 1.1f)
		{
			ui.Scale = 1f;
		}
	}

	public void CheckMouseClick(UICircle ui)
	{
		if (ParentBoat is null || !ParentBoat.Active)
		{
			return;
		}
		if ((MouseWorldCurrent - Position - ui.RelativeCenter).Length() < 20)
		{
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				if (ui.UIType == 0)
				{
					ParentBoat.Velocity += new Vector2(-1, 0);
					Owner.velocity += new Vector2(-1, 0);
				}
				if (ui.UIType == 1)
				{
					ParentBoat.Velocity *= 0;
					Owner.velocity *= 0;
				}
				if (ui.UIType == 2)
				{
					ParentBoat.Velocity += new Vector2(1, 0);
					Owner.velocity += new Vector2(1, 0);
				}
				if (ui.UIType == 3)
				{
					ParentBoat.LightOn = !ParentBoat.LightOn;
				}
				if (ui.UIType == 4)
				{
					for (int l = 0; l < 16; l++)
					{
						int type;
						switch (Main.rand.Next(3))
						{
							case 0:
								type = GoreID.ChimneySmoke1;
								break;
							case 1:
								type = GoreID.ChimneySmoke2;
								break;
							case 2:
								type = GoreID.ChimneySmoke3;
								break;
							default:
								type = GoreID.ChimneySmoke1;
								break;
						}

						var gore = Gore.NewGorePerfect(ParentBoat.Position + new Vector2(Main.rand.NextFloat(92), Main.rand.NextFloat(20)) - new Vector2(17, 18), new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(MathHelper.TwoPi), type);
						gore.timeLeft = Main.rand.Next(60, 120);
					}
					ParentBoat.Active = false;
				}
			}
		}
	}

	public override void Draw()
	{
		DrawUICircle(LeftButtom);
		DrawUICircle(PauseButtom);
		DrawUICircle(RightButtom);
		DrawUICircle(LampButtom);
		DrawUICircle(KillButtom);
	}

	public void DrawUICircle(UICircle ui)
	{
		Texture2D tex = ModAsset.BlackAwningBoat_ControlUI.Value;
		Rectangle frame = new Rectangle(ui.UIType * 32, 0, 32, 32);
		if (ui.Scale > 1.1f)
		{
			frame.Y += 32;
		}
		Ins.Batch.Draw(tex, Position + ui.RelativeCenter, frame, Color.White, 0, frame.Size() / 2f, ui.Scale, SpriteEffects.None);
	}
}
