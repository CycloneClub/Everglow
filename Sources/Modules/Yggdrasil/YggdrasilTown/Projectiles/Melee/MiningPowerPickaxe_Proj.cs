using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.ProjectileEffects;
using Terraria.Audio;
using Terraria.DataStructures;
using static Everglow.Yggdrasil.YggdrasilTown.Items.Tools.MiningPowerPickaxe;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class MiningPowerPickaxe_Proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	private static Point16 MouseTileTargetCoord => new Point16(Player.tileTargetX, Player.tileTargetY);

	private static Tile MouseTileTarget => Framing.GetTileSafely(MouseTileTargetCoord);

	public override string Texture => ModAsset.MiningPowerPickaxe_Mod;

	private Player Owner => Main.player[Projectile.owner];

	private Vector2 OwnerMouseWorld => Owner.MouseWorld();

	/// <summary>
	/// The first pick tile target type.
	/// <para/>To help determining target tile type after the tile target is killed by vanilla picking code.
	/// <para/> Passed from <see cref="MiningPowerPickaxe.Shoot(Player, EntitySource_ItemUse_WithAmmo, Vector2, Vector2, int, int, float)"/>.
	/// </summary>
	private int FirstTileTargetType => (int)Projectile.ai[2];

	/// <summary>
	/// A symbol to indicate this pick is first time to pick tiles.
	/// <para/> Used to adapt vanilla picking code.
	/// </summary>
	private bool FirstPick { get; set; } = true;

	/// <summary>
	/// Tiles to be picked by chain-mining function.
	/// </summary>
	private List<Point16> TargetTiles { get; set; } = [];

	public override void SetDefaults()
	{
		Projectile.width = 56;
		Projectile.height = 50;

		Projectile.aiStyle = -1;
		Projectile.timeLeft = 360000;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;
		Projectile.friendly = true;

		Projectile.ownerHitCheck = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
	}

	public override void AI()
	{
		Owner.heldProj = Projectile.whoAmI;
		Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Owner.gravDir * Projectile.rotation - MathHelper.PiOver2 - 0.0f * MathHelper.PiOver4 * Owner.direction);
		Owner.direction = (OwnerMouseWorld - Owner.MountedCenter).X < 0 ? -1 : 1;

		if (NetUtils.IsClient && Main.myPlayer == Projectile.owner)
		{
			Owner.ListenMouseWorld();
		}

		if (Owner.controlUseItem && Owner.HeldItem.ModItem is MiningPowerPickaxe)
		{
			var mouseToPlayer = Vector2.Normalize(OwnerMouseWorld - Owner.MountedCenter);
			Projectile.rotation = (float)(Math.Atan2(mouseToPlayer.Y, mouseToPlayer.X) + MathHelper.PiOver4 * 0.6f * Owner.direction * Owner.gravDir);

			var mainOffset = Vector2.Normalize(mouseToPlayer) * 26f;
			var shakeOffset = mainOffset.NormalizeSafe() * MathF.Sin((float)Main.timeForVisualEffects) * 0.5f;
			Projectile.Center = Owner.MountedCenter + mainOffset + shakeOffset;

			Projectile.velocity = Vector2.Zero;

			// Smoke dust from pickaxe body
			if (Main.rand.NextBool(2))
			{
				var dustCenter = Projectile.Center + new Vector2(18, 10 * Owner.direction * Owner.gravDir).RotatedBy(Projectile.rotation);
				Dust.NewDust(dustCenter - new Vector2(10, 10), 20, 20, DustID.IceTorch, Scale: 1.4f);
			}

			// Flame dust from pickaxe drill
			if (Main.rand.NextBool(20))
			{
				Dust.NewDustDirect(Projectile.Center - new Vector2(Projectile.width, Projectile.height) / 3, Projectile.width / 3, Projectile.height / 3, DustID.Smoke, 0, 0, newColor: new Color(0.4f, 0.4f, 0.4f), Scale: 1f);
			}

			if (Owner.itemTime == Owner.itemTimeMax - 1)
			{
				// The function code only runs on owner's client.
				if (Projectile.owner == Main.myPlayer)
				{
					// Chain-mining function.
					ChainMining(Owner);

					// General pickaxe picking code.
					if (FirstPick)
					{
						FirstPick = false;

						SoundEngine.PlaySound(SoundID.Item23);
					}
					else
					{
						if (Owner.IsTargetTileInItemRange(Owner.HeldItem)
							&& !(Main.tileHammer[MouseTileTarget.type] || Main.tileAxe[MouseTileTarget.type]))
						{
							Owner.PickTile(Player.tileTargetX, Player.tileTargetY, Pick);
						}

						SoundEngine.PlaySound(SoundID.Item22);
					}
				}
			}
			else if (Owner.itemTime == 0)
			{
				Owner.itemTime = Owner.itemTimeMax;
			}

			Owner.direction = Projectile.Center.X < Owner.MountedCenter.X ? -1 : 1;
		}
		else
		{
			Projectile.Kill();
		}
	}

	/// <summary>
	/// Mining power pickaxe main function: Chain-mining.
	/// 1. Search valid tiles linked to the clicked tile. (The tiles will be synced to all players' client.)
	/// 2. If the target tiles are valid, pick the nearest [PickTileMax] tiles n by n from target tiles.
	/// </summary>
	/// <param name="player"></param>
	private void ChainMining(Player player)
	{
		var pickaxeItem = player.HeldItem.ModItem as MiningPowerPickaxe;
		if (pickaxeItem.Charge >= ChargeCost && !Main.SmartCursorIsUsed)
		{
			// If the mouse tile target is valid, fill the target tiles list.
			if (TargetTiles.Count == 0
				&& player.IsTargetTileInItemRange(pickaxeItem.Item)
				&& (FirstPick || CheckTile(MouseTileTarget))
				&& CheckPickPower(player, MouseTileTarget, MouseTileTargetCoord))
			{
				// Get all linked tiles (Order by distance to player).
				TargetTiles = GetLinkedTiles(MouseTileTargetCoord, FirstPick ? FirstTileTargetType : MouseTileTarget.type, SearchTileMax, player);
				//TargetTiles.Remove(MouseTileTargetCoord);

				MiningPowerPickaxe_Proj_Wave mPPPW = new MiningPowerPickaxe_Proj_Wave()
				{
					Value = 30,
					Owner = player,
					Tiles = TargetTiles,
					Visible = true,
					Active = true,
				};
				Ins.VFXManager.Add(mPPPW);

				MiningPowerPickaxe_Proj_TileBound mPPPTB = new MiningPowerPickaxe_Proj_TileBound()
				{
					Value = 30,
					Owner = player,
					CurrentTiles = TargetTiles,
					OreType = MouseTileTarget.TileType,
					Visible = true,
					Active = true,
				};
				Ins.VFXManager.Add(mPPPTB);

				Projectile.netUpdate = true;
			}

			// Pick the nearest [PickTileMax] tiles from target tiles.
			if (TargetTiles.Count != 0)
			{
				// Decrease charge
				pickaxeItem.Charge = Math.Max(pickaxeItem.Charge - ChargeCost, 0);

				foreach (var tile in TargetTiles.Take(PickTileMax))
				{
					player.PickTile(tile.X, tile.Y, Pick);
				}

				// Remove picked tiles from target tiles.
				if (TargetTiles.RemoveAll(p => !Main.tile[p].HasTile) > 0)
				{
					Projectile.netUpdate = true;
				}
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var rotation = Projectile.rotation;
		var effects = (Owner.direction == 1 && Owner.gravDir == 1) || (Owner.gravDir == -1 && Owner.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipVertically;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, rotation, texture.Size() * 0.5f, Projectile.scale, effects, 0f);
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		// Draw signal over tile to represent they're selected.
		// if (TargetTiles.Count > 0)
		// {
		// foreach (var tile in TargetTiles)
		// {
		// var drawPos = tile.ToWorldCoordinates() - Main.screenPosition;
		// var texture = Commons.ModAsset.Point.Value;
		// Main.spriteBatch.Draw(texture, drawPos, null, new Color(1f, 1f, 1f, 0f), 0f, texture.Size() * 0.5f, 0.08f, SpriteEffects.None, 0f);
		// }
		// }
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		// Send tiles data
		writer.Write(TargetTiles.Count);
		for (int i = 0; i < TargetTiles.Count; i++)
		{
			writer.Write(TargetTiles[i].X);
			writer.Write(TargetTiles[i].Y);
		}
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		// Read tiles data
		int length = reader.ReadInt32();
		var newTiles = new List<Point16>();
		for (int i = 0; i < length; i++)
		{
			var tileX = reader.ReadUInt16();
			var tileY = reader.ReadUInt16();
			newTiles.Add(new Point16(tileX, tileY));
		}
		TargetTiles = newTiles;
	}

	private static List<Point16> GetLinkedTiles(Point16 clickedTilePos, int clickedTileType, int tileNumMax, Player player)
	{
		if (clickedTileType < 0)
		{
			return [];
		}

		// Search linked tiles
		var result = new List<Point16>();
		Queue<Point16> tileQueue = new();
		tileQueue.Enqueue(clickedTilePos);
		while (tileQueue.Count != 0)
		{
			var currentTile = tileQueue.Dequeue();
			result.Add(currentTile);

			if (result.Count > tileNumMax)
			{
				break;
			}

			// Search nearby tiles
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					var nearbyTilePos = new Point16(currentTile.X + i, currentTile.Y + j);
					var nearbyTile = Framing.GetTileSafely(nearbyTilePos); // var nearbyTile = Main.tile[nearbyTilePos];
					if (!CheckTilePos(nearbyTilePos) || !CheckTile(nearbyTile)
						|| nearbyTile.type != clickedTileType
						|| tileQueue.Any(t => t == nearbyTilePos)
						|| result.Any(t => t == nearbyTilePos)
						|| Vector2.Distance(nearbyTilePos.ToWorldCoordinates(), player.Center) > SearchTileRange)
					{
						continue;
					}

					tileQueue.Enqueue(nearbyTilePos);
				}
			}
		}

		return result
			.Distinct()
			.OrderBy(p => Vector2.Distance(p.ToWorldCoordinates(), player.Center))
			.ToList();
	}

	private static bool CheckTilePos(Point16 pos) =>
		pos.X < Main.maxTilesX && pos.X >= 0 && pos.Y < Main.maxTilesY && pos.Y >= 0;

	private static bool CheckTile(Tile tile) =>
		tile != null
		&& tile.HasTile
		&& TileID.Sets.Ore[tile.type];

	private static bool CheckPickPower(Player player, Tile tile, Point16 clickPos) =>
		player.GetPickaxeDamage(clickPos.X, clickPos.Y, Pick, 0, tile) > 0;
}
