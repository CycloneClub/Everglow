using Everglow.Commons.CustomTiles;
using Everglow.Commons.CustomTiles.Abstracts;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Commons.Templates.Furniture.Elevator;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Miscs;

namespace Everglow.Yggdrasil.KelpCurtain.CustomTiles;

public class BlackAwningBoat : BoxEntity
{
	public bool OnSelect = false;

	public bool LightOn = false;

	public BlackAwningBoat_ControlUI LocalUIHelper;

	public override void SetDefaults()
	{
		Size = new Vector2(92, 20);
		LocalUIHelper = null;
	}

	public override Color MapColor => new Color(51, 40, 38);

	public override void AI()
	{
		var bottomTile = TileUtils.SafeGetTile(new Vector2(Box.Center.X, Box.Bottom).ToTileCoordinates());
		if (bottomTile.LiquidAmount > 0)
		{
			Velocity = new Vector2(Velocity.X, 0);
		}
		else
		{
			Velocity = new Vector2(Velocity.X, 2);
		}
		List<Point> collideTile = TileUtils.GetAABBAreaOfTile(Position + Velocity, Size);
		foreach (var pos in collideTile)
		{
			var tile = TileUtils.SafeGetTile(pos);
			if (tile.HasTile && Main.tileSolid[tile.type])
			{
				Velocity *= 0;
				break;
			}
		}
		Position += Velocity;
		OnSelect = Box.Contain(Main.MouseWorld);
		if (OnSelect && Main.mouseRightRelease && Main.mouseRight)
		{
			RightClick();
		}
		if (!CanClick())
		{
			if (LocalUIHelper is not null)
			{
				LocalUIHelper.Closing = true;
			}
		}
		if (Velocity.Length() > 0.001f)
		{
			bool shouldReleaseProj = true;
			foreach (var proj in Main.projectile)
			{
				if (proj is not null && proj.active && proj.type == ModContent.ProjectileType<BlackAwningBoat_WaterDistort>())
				{
					BlackAwningBoat_WaterDistort bABWD0 = proj.ModProjectile as BlackAwningBoat_WaterDistort;
					if (bABWD0 is not null && bABWD0.ParentBoat == this)
					{
						shouldReleaseProj = false;
						break;
					}
				}
			}
			if (shouldReleaseProj)
			{
				Projectile p0 = Projectile.NewProjectileDirect(Main.LocalPlayer.GetSource_FromAI(), Box.Center, Velocity, ModContent.ProjectileType<BlackAwningBoat_WaterDistort>(), 0, 0, Main.myPlayer);
				BlackAwningBoat_WaterDistort bABWD = p0.ModProjectile as BlackAwningBoat_WaterDistort;
				if (bABWD is not null)
				{
					bABWD.ParentBoat = this;
				}
			}
		}
		if (LightOn)
		{
			Lighting.AddLight(Box.Center, new Vector3(1.2f, 0.7f, 0.3f) * 1.25f);
		}
	}

	public override void Draw()
	{
		Texture2D boatTex = ModAsset.BlackAwningBoat.Value;
		var frame = new Rectangle(0, 50, 100, 50);
		if (!LightOn)
		{
			frame.Y = 0;
		}
		Main.spriteBatch.Draw(boatTex, new Vector2(Box.Center.X, Box.Bottom) - Main.screenPosition, frame, Lighting.GetColor(new Vector2(Box.Center.X, Box.Bottom).ToTileCoordinates()), 0, new Vector2(frame.Width / 2f, frame.Height), 1, SpriteEffects.None, 0);
		if (LightOn)
		{
			frame.X += 100;
			Main.spriteBatch.Draw(boatTex, new Vector2(Box.Center.X, Box.Bottom) - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0), 0, new Vector2(frame.Width / 2f, frame.Height), 1, SpriteEffects.None, 0);
		}
		if (OnSelect)
		{
			Texture2D boatTex_highlight = ModAsset.BlackAwningBoat_Single_Highlight.Value;
			Main.spriteBatch.Draw(boatTex_highlight, new Vector2(Box.Center.X, Box.Bottom) - Main.screenPosition, null, Lighting.GetColor(new Vector2(Box.Center.X, Box.Bottom).ToTileCoordinates()), 0, new Vector2(boatTex_highlight.Width / 2f, boatTex_highlight.Height), 1, SpriteEffects.None, 0);
		}
	}

	public void RightClick()
	{
		if (LocalUIHelper is null || !LocalUIHelper.Active)
		{
			LocalUIHelper = new BlackAwningBoat_ControlUI
			{
				AnimationTimer = 0,
				ParentBoat = this,
				Owner = Main.LocalPlayer,
				RelativePos = new Vector2(0, -120),
				Visible = true,
				Active = true,
			};
			Ins.VFXManager.Add(LocalUIHelper);
			foreach (var customTile in ColliderManager.Instance.OfType<BlackAwningBoat>())
			{
				if (customTile.LocalUIHelper is not null && customTile.LocalUIHelper.Active && customTile.LocalUIHelper.Owner == Main.LocalPlayer && !customTile.LocalUIHelper.Closing && customTile.LocalUIHelper != LocalUIHelper)
				{
					customTile.LocalUIHelper.Closing = true;
				}
			}
		}
		else
		{
			LocalUIHelper.Closing = !LocalUIHelper.Closing;
		}
	}

	public override Vector2 StandAccelerate(IBox obj)
	{
		return base.StandAccelerate(obj) * 2;
	}

	public virtual bool CanClick()
	{
		return (Main.LocalPlayer.MountedCenter - Box.Center).Length() <= 200;
	}
}
