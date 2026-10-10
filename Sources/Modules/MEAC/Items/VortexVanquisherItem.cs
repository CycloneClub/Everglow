using Everglow.MEAC.NonTrueMeleeProj;
using Everglow.MEAC.PlanetBeFall.Projectiles.PlanetBefall;
using Everglow.MEAC.Projectiles;
using ReLogic.Graphics;
using Terraria.GameContent;

namespace Everglow.MEAC.Items;

public class VortexVanquisherItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 86;
		Item.height = 86;
		Item.useAnimation = 5;
		Item.useTime = 5;
		Item.shootSpeed = 5f;
		Item.knockBack = 6.5f;
		Item.damage = 608;
		Item.rare = ItemRarityID.Purple;

		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.value = 648000;
	}

	private int coolTimeForE = 0;
	private int coolTimeForQ = 0;

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		var slotSize = new Vector2(42f, 42f);
		position -= slotSize * Main.inventoryScale / 2f - frame.Size() * scale / 2f;
		Vector2 drawPos = position + slotSize * Main.inventoryScale / 2f;
		Texture2D vortexE = ModAsset.Post_png.Value;
		Texture2D vortexQ = ModAsset.SkillQ.Value;
		if (!Main.gamePaused)
		{
			if (coolTimeForE > 0)
			{
				coolTimeForE--;
				spriteBatch.Draw(vortexE, drawPos + new Vector2(6.23f) * scale + new Vector2(0, 0), null, new Color(0, 0, 0, 255), 0f, vortexE.Size() / 2f, scale * 1.91f, SpriteEffects.None, 0f);
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, (coolTimeForE / 60f).ToString("#.#"), drawPos + new Vector2(6.23f) * scale, Color.White, 0f, Vector2.Zero, scale * 1.91f, SpriteEffects.None, 0);
			}
			else
			{
				coolTimeForE = 0;
				spriteBatch.Draw(vortexE, drawPos + new Vector2(6.23f) * scale + new Vector2(0, 0), null, new Color(155, 155, 155, 50), 0f, vortexE.Size() / 2f, scale * 1.91f, SpriteEffects.None, 0f);
			}
			if (coolTimeForQ > 0)
			{
				coolTimeForQ--;
				spriteBatch.Draw(vortexQ, position + new Vector2(0, slotSize.Y * Main.inventoryScale / 2f), null, new Color(0, 0, 0, 255), 0f, vortexQ.Size() / 2f, scale * 1.91f, SpriteEffects.None, 0f);
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, (coolTimeForQ / 60f).ToString("#.#"), drawPos + new Vector2(22.91f) * scale + new Vector2(-30, 0), Color.White, 0f, Vector2.Zero, scale * 1.91f, SpriteEffects.None, 0);
			}
			else
			{
				coolTimeForQ = 0;
				spriteBatch.Draw(vortexQ, position + new Vector2(0, slotSize.Y * Main.inventoryScale / 2f), null, new Color(155, 155, 155, 50), 0f, vortexQ.Size() / 2f, scale * 1.91f, SpriteEffects.None, 0f);
			}
		}
	}

	public override bool CanUseItem(Player player)
	{
		if (base.CanUseItem(player))
		{
			return false;
		}
		return base.CanUseItem(player);
	}

	internal bool LeftClick = false;

	public override void HoldItem(Player player)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<VortexVanquisher>()] + player.ownedProjectileCounts[ModContent.ProjectileType<VortexVanquisherThump>()] < 1)
		{
			if (Main.myPlayer == player.whoAmI)
			{
				if (Main.mouseMiddle && Main.mouseMiddleRelease)
				{
					if (coolTimeForQ > 0)
					{
						return;
					}
					coolTimeForQ = 1440;
					if (player.name == "Omni")
					{
						coolTimeForQ = 10;
					}
					Projectile PlanetBeFall = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), /*Main.MouseWorld*/new Vector2(player.Center.X, Main.MouseWorld.Y - 1500), Vector2.Zero, ModContent.ProjectileType<PlanetBeFall_Proj>(), Item.damage * 27, Item.knockBack * 10, player.whoAmI);
				}
				if (player.altFunctionUse != 2)
				{
					if (LeftClick && !Main.mouseLeft)
					{
						Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<VortexVanquisher>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
						proj.scale *= Item.scale;
					}
				}
				else
				{
					if (coolTimeForE > 0)
					{
						return;
					}
					coolTimeForE = 720;
					if (player.name == "Omni")
					{
						coolTimeForE = 10;
					}
					bool HasProj = false;
					foreach (Projectile proj in Main.projectile)
					{
						if (proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<GoldShield>() && proj.active)
						{
							proj.timeLeft = 1200;
							proj.ai[1] = player.statLifeMax * 0.6f; // 盾量
							HasProj = true;
						}
						if (proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<GoldShield_backTextureSubProj>() && proj.active)
						{
							proj.timeLeft = 1200;
						}
					}
					if (!HasProj)
					{
						Projectile proj2 = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GoldShield>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
						proj2.ai[1] = player.statLifeMax * 0.6f; // 盾量
						Projectile subProj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.zeroVector, ModContent.ProjectileType<GoldShield_backTextureSubProj>(), 0, 0, player.whoAmI);
						if (subProj != null)
						{
							GoldShield_backTextureSubProj gSBTSP = subProj.ModProjectile as GoldShield_backTextureSubProj;
							gSBTSP.MainProj = proj2;
						}
					}
					Vector2 CheckPoint = Main.MouseWorld;
					for (int y = 0; y < 120; y++)
					{
						if (Collision.SolidCollision(CheckPoint, 1, 1))
						{
							break;
						}
						else
						{
							CheckPoint += new Vector2(0, 8) * player.gravDir;
						}
					}
					if (!Collision.SolidCollision(CheckPoint, 1, 1))
					{
						return;
					}

					Vector2 TotalVector = Vector2.Zero; // 合向量
					int TCount = 0;
					for (int a = 0; a < 12; a++)
					{
						Vector2 v0 = new Vector2(20, 0).RotatedBy(a / 6d * Math.PI);
						if (Collision.SolidCollision(CheckPoint + v0, 1, 1))
						{
							TotalVector -= v0;
							TCount++;
						}
						else
						{
							TotalVector += v0;
						}
					}
					for (int a = 0; a < 24; a++)
					{
						Vector2 v0 = new Vector2(40, 0).RotatedBy(a / 12d * Math.PI);
						if (Collision.SolidCollision(CheckPoint + v0, 1, 1))
						{
							TotalVector -= v0 * 0.5f;
							TCount++;
						}
						else
						{
							TotalVector += v0 * 0.5f;
						}
					}
					if (TotalVector == Vector2.Zero || TCount > 30)
					{
						return;
					}

					Projectile p0 = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), CheckPoint, Vector2.Zero, ModContent.ProjectileType<StonePost>(), Item.damage / 3, 0, player.whoAmI, 1);
					float Angle = (float)Math.Atan2(TotalVector.Y, TotalVector.X);
					p0.rotation = (float)(Angle - Math.PI * 1.5);
				}
			}
			if (LeftClick)
			{
				clickTime++;
				if (clickTime > 30)
				{
					int playerdir = Main.MouseWorld.X > player.Center.X ? 1 : -1;
					player.direction = playerdir;
					Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, new Vector2(Math.Sign(Main.MouseWorld.X - player.Center.X), 0), ModContent.ProjectileType<VortexVanquisherThump>(), Item.damage, 0, player.whoAmI); // Original: Item.damage * 6
					clickTime = 0;
				}
			}
		}
		else
		{
			clickTime = 0;
		}
		LeftClick = Main.mouseLeft;
		base.HoldItem(player);
	}

	private int clickTime = 0;

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.Trident);
		recipe.AddIngredient(ItemID.Spear);
		recipe.AddIngredient(ItemID.Amber, 45);
		recipe.AddIngredient(ItemID.LunarBar, 24);
		recipe.AddIngredient(ItemID.SunplateBlock, 30);
		recipe.AddTile(TileID.LunarCraftingStation);
		recipe.Register();
	}
}
