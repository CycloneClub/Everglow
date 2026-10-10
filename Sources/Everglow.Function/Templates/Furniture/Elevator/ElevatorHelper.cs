using Everglow.Commons.Enums;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;
using Terraria.Audio;
using static Terraria.GameContent.Drawing.WindGrid;

namespace Everglow.Commons.Templates.Furniture.Elevator;

[Pipeline(typeof(WCSPipeline))]
public class ElevatorHelper : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public ElevatorBase ParentElevator;

	public Vector2 RelativePos;

	public Vector2 Position;

	public Point WinchCoord;

	public int WinchTileType;

	public Player Owner;

	public float AnimationTimer;

	public bool Closing = false;

	public class UICircle
	{
		public float Size;
		public Vector2 AddCenter;
		public Texture2D PanelTexture;
		public Texture2D ContainTexture;

		public UICircle(float size, Vector2 addCenter, Texture2D panel, Texture2D contain)
		{
			Size = size;
			AddCenter = addCenter;
			PanelTexture = panel;
			ContainTexture = contain;
		}
	}

	public UICircle LampPanel = new(0f, Vector2.Zero, ModAsset.Wires_0.Value, ModAsset.LampOff.Value);
	public UICircle KillPanel = new(0f, Vector2.Zero, ModAsset.Wires_0.Value, ModAsset.Elevator_Remove.Value);

	public override void Update()
	{
		// Data validation
		if (Owner?.HeldItem is null)
		{
			throw new InvalidOperationException("Owner must be initialized correctly before updating.");
		}
		if (Owner == null || ParentElevator is null || !ParentElevator.Active || !Owner.active || Owner.dead)
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
			Position = ParentElevator.Box.Center + RelativePos;
			UpdateUI(LampPanel, 0);
			UpdateUI(KillPanel, 1);
		}
		else
		{
			if (ParentElevator is not null && ParentElevator.Active)
			{
				Position = ParentElevator.Box.Center + RelativePos;
			}
			UpdateUI(LampPanel, 0);
			UpdateUI(KillPanel, 1);
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

	public void UpdateUI(UICircle ui, int Index)
	{
		ui.AddCenter = new Vector2(0, Index * 40);
		if (AnimationTimer < 30)
		{
			ui.Size = AnimationTimer / 30f;
		}
		if (ParentElevator is null || !ParentElevator.Active)
		{
			if (AnimationTimer == 27)
			{
				foreach (var item in Main.item)
				{
					if (item is not null && item.active)
					{
						if (item.inner.createTile == WinchTileType)
						{
							Vector2 toKillPos = WinchCoord.ToWorldCoordinates() - item.Center;
							if (toKillPos.Length() < 200)
							{
								item.position = Main.LocalPlayer.MountedCenter + new Vector2(0, -32);
								return;
							}
						}
					}
				}
			}
			return;
		}
		else
		{
			WinchCoord = ParentElevator.WinchCoord;
			WinchTileType = ParentElevator.WinchTileType;
			if (Index == 0)
			{
				ui.ContainTexture = ParentElevator.LightSourceOn ? ModAsset.LampOff.Value : ModAsset.LampOn.Value;
			}
		}

		if (AnimationTimer >= 30)
		{
			CheckMouseOver(ui);
		}
		CheckMouseClick(ui, Index);
	}

	public void CheckMouseOver(UICircle ui)
	{
		if (ParentElevator is null || !ParentElevator.Active)
		{
			return;
		}
		if ((Main.MouseWorld - Position - ui.AddCenter).Length() < 20 && ui.PanelTexture != ModAsset.Wires_1.Value)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			ui.Size = 1.2f;
			ui.PanelTexture = ModAsset.Wires_1.Value;
		}
		if ((Main.MouseWorld - Position - ui.AddCenter).Length() >= 20 && ui.PanelTexture != ModAsset.Wires_0.Value)
		{
			ui.Size = 1f;
			ui.PanelTexture = ModAsset.Wires_0.Value;
		}
	}

	public void CheckMouseClick(UICircle ui, int Index)
	{
		if (ParentElevator is null || !ParentElevator.Active)
		{
			return;
		}
		if ((Main.MouseWorld - Position - ui.AddCenter).Length() < 20)
		{
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				if (Index == 0)
				{
					ParentElevator.LightSourceOn = !ParentElevator.LightSourceOn;
				}
				if (Index == 1)
				{
					Closing = true;
					WorldGen.KillTile(WinchCoord.X, WinchCoord.Y);
				}
			}
		}
	}

	public override void Draw()
	{
		DrawUICircle(LampPanel);
		DrawUICircle(KillPanel);
	}

	public void DrawUICircle(UICircle ui)
	{
		Ins.Batch.Draw(ui.PanelTexture, Position + ui.AddCenter, null, Color.White, 0, ui.PanelTexture.Size() / 2f, ui.Size, SpriteEffects.None);
		Ins.Batch.Draw(ui.ContainTexture, Position + ui.AddCenter, null, Color.White, 0, ui.ContainTexture.Size() / 2f, ui.Size, SpriteEffects.None);
	}
}
