using Everglow.Commons.CustomTiles;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Commons.Templates.Furniture.Elevator;

public abstract class WinchTileBase<TElevator> : ModTile
	where TElevator : ElevatorBase, new()
{
	public Texture2D PreviewTexture;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileBlendAll[Type] = true;
		Main.tileBlockLight[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Origin = new(0, 0);
		TileObjectData.newTile.AnchorBottom = new(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorLeft = new(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorRight = new(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, 1, 0);
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(112, 75, 75));
		DustType = DustID.Iron;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		// Skip closer updates.
		if (closer)
		{
			return;
		}

		Point winchTileCoord = new Point(i, j);
		Tile winchTile = Main.tile[winchTileCoord];
		if (!ColliderManager.Instance.OfType<TElevator>()
			.Any(r => r.WinchCoord == winchTileCoord))
		{
			EmitElevator(i, j);
		}
	}

	public virtual void EmitElevator(int i, int j)
	{
		Point winchTileCoord = new Point(i, j);
		var newElevator = ColliderManager.Instance.Add<TElevator>(new Vector2(i, j + 15) * 16 - new Vector2(48, 8));
		newElevator.WinchTileType = Type;
		newElevator.WinchCoord = winchTileCoord;
		EmitAuxiliaryStructure(newElevator);
	}

	public virtual void EmitAuxiliaryStructure(ElevatorBase parentElevator)
	{
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		Texture2D t = ModContent.Request<Texture2D>(Texture).Value;
		Color c0 = Lighting.GetColor(i, j);
		spriteBatch.Draw(t, new Vector2(i * 16, j * 16 + 6) - Main.screenPosition + zero, null, c0, 0, t.Size() / 2f, 1, SpriteEffects.None, 0);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}

	public override bool PreDrawPlacementPreview(int i, int j, SpriteBatch spriteBatch, ref Rectangle frame, ref Vector2 position, ref Color color, bool validPlacement, ref SpriteEffects spriteEffects)
	{
		if (PreviewTexture == null)
		{
			PreviewTexture = ModAsset.DefaultElevator_Winch_Preview.Value;
			string checkName = GetType().FullName;
			checkName = checkName.Replace('.', '/');
			checkName += "_Preview";
			string checkNameWithExtension = checkName + ".png";
			if (ModContent.FileExists(checkNameWithExtension))
			{
				PreviewTexture = ModContent.Request<Texture2D>(checkName, AssetRequestMode.ImmediateLoad).Value;
			}
		}
		spriteBatch.Draw(PreviewTexture, position, null, color * 0.5f, 0f, new Vector2(PreviewTexture.Width / 2f, 0), 1f, spriteEffects, 0f);
		Texture2D mainTex = ModContent.Request<Texture2D>(Texture).Value;
		spriteBatch.Draw(mainTex, position + new Vector2(0, 6), null, color * 0.5f, 0f, mainTex.Size() * 0.5f, 1f, spriteEffects, 0f);
		return false;
	}

	public override void PostDrawPlacementPreview(int i, int j, SpriteBatch spriteBatch, Rectangle frame, Vector2 position, Color color, bool validPlacement, SpriteEffects spriteEffects)
	{
	}
}
