using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Twilight;

[AutoloadEquip(EquipType.Legs)]
public class TwilightWoodLeggings : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public const int MoveSpeedBonus = 5;
	public const int JumpSpeedBonus = 5;
	public int WalkStep = 0;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = Item.buyPrice(silver: 37, copper: 50);
		Item.rare = ItemRarityID.White;
		Item.defense = 2;
		WalkStep = 0;
	}

	public override void UpdateEquip(Player player)
	{
		player.moveSpeed += MoveSpeedBonus / 100f;
		player.jumpSpeedBoost += JumpSpeedBonus / 100f;

		if (player.velocity.Y == 0 && player.velocity.Length() >= 1E-05f)
		{
			WalkStep++;
			if (WalkStep % 3 == 2)
			{
				WalkStep = 0;
				float totalRot = Main.rand.NextFloat(-0.5f, 0.5f);
				int length = Main.rand.Next(4, 12);
				float scale = Main.rand.NextFloat(1, 2) / 10f;
				Vector2 offset = Vector2.zeroVector;
				var tile = TileUtils.SafeGetTile((player.Bottom + new Vector2(0, 12)).ToTileCoordinates());
				if (tile.Slope == SlopeType.SlopeUpLeft)
				{
					offset = new Vector2(-8, -8);
				}
				if (tile.Slope == SlopeType.SlopeUpRight)
				{
					offset = new Vector2(8, -8);
				}
				if (tile.Slope == SlopeType.SlopeDownLeft)
				{
					offset = new Vector2(-8, 8);
				}
				if (tile.Slope == SlopeType.SlopeDownRight)
				{
					offset = new Vector2(8, 8);
				}
				if (tile.halfBrick())
				{
					offset = new Vector2(0, 8);
				}
				for (int j = 0; j < length; j++)
				{
					float sideSpeed = 1f;
					if (j >= length - 4)
					{
						sideSpeed *= (length - j - 1) / 4f;
					}
					var dustVFXLeft = new MagicalBoomerangDust
					{
						Velocity = new Vector2(sideSpeed, -j).RotatedBy(totalRot) * scale,
						Active = true,
						Visible = true,
						Position = player.position + new Vector2(player.width / 2, player.height) + offset,
						MaxTime = Main.rand.Next(30, 46),
						Scale = Main.rand.NextFloat(3, 4),
						Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
						ai = new float[] { 0, 0, 0 },
					};
					Ins.VFXManager.Add(dustVFXLeft);

					var dustVFXRight = new MagicalBoomerangDust
					{
						Velocity = new Vector2(-sideSpeed, -j).RotatedBy(totalRot) * scale,
						Active = true,
						Visible = true,
						Position = player.position + new Vector2(player.width / 2, player.height) + offset,
						MaxTime = Main.rand.Next(30, 46),
						Scale = Main.rand.NextFloat(3, 4),
						Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
						ai = new float[] { 0, 0, 0 },
					};
					Ins.VFXManager.Add(dustVFXRight);
				}
			}
		}
	}
}
