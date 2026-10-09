using Everglow.SpellAndSkull.Items;
using Everglow.SpellAndSkull.Projectiles.BookofSkulls;
using Everglow.SpellAndSkull.Projectiles.CrystalStorm;
using Everglow.SpellAndSkull.Projectiles.CursedFlames;
using Everglow.SpellAndSkull.Projectiles.GoldenShower;
using Everglow.SpellAndSkull.Projectiles.LunarFlare;
using Everglow.SpellAndSkull.Projectiles.MagnetSphere;
using Everglow.SpellAndSkull.Projectiles.RazorbladeTyphoon;
using Everglow.SpellAndSkull.Projectiles.WaterBolt;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Everglow.SpellAndSkull.GlobalItems;

public class MagicBooksReplace : GlobalItem
{
	private static string SpellbookExtra = "Mods.Everglow.ExtraTooltip.Spellbook.";

	private static string SpellbookGTV(string key) => Language.GetTextValue(SpellbookExtra + key);

	public static List<int> MagicBookType = new List<int>();

	public override void SetDefaults(Item entity)
	{
		AddContentsToMagicBookList(ItemID.WaterBolt);
		AddContentsToMagicBookList(ItemID.DemonScythe);
		AddContentsToMagicBookList(ItemID.BookofSkulls);
		AddContentsToMagicBookList(ItemID.CrystalStorm);
		AddContentsToMagicBookList(ItemID.CursedFlames);
		AddContentsToMagicBookList(ItemID.GoldenShower);
		AddContentsToMagicBookList(ItemID.MagnetSphere);
		AddContentsToMagicBookList(ItemID.RazorbladeTyphoon);
		AddContentsToMagicBookList(ItemID.LunarFlareBook);
		base.SetDefaults(entity);
	}

	public void AddContentsToMagicBookList(int type)
	{
		if (!MagicBookType.Contains(type))
		{
			MagicBookType.Add(type);
		}
	}

	public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
	{
		if (Main.LocalPlayer.TryGetModPlayer(out MagicBookPlayer modplayer))
		{
			if (modplayer.MagicBookLevel == 1)
			{
				if (item.type == ItemID.WaterBolt)
				{
				}
			}
		}
		return base.PreDrawTooltipLine(item, line, ref yOffset);
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (Main.LocalPlayer.GetModPlayer<MagicBookPlayer>().MagicBookLevel == 1)
		{
			if (item.type == ItemID.WaterBolt)
			{
				tooltips.Add(new TooltipLine(ModLoader.GetMod("Everglow"), "ExtraTooltip", SpellbookGTV("WaterBolt")));
			}

			if (item.type == ItemID.DemonScythe)
			{
				tooltips.Add(new TooltipLine(ModLoader.GetMod("Everglow"), "ExtraTooltip", SpellbookGTV("DemonScythe")));
			}

			if (item.type == ItemID.BookofSkulls)
			{
				tooltips.Add(new TooltipLine(ModLoader.GetMod("Everglow"), "ExtraTooltip", SpellbookGTV("BookofSkulls")));
			}

			if (item.type == ItemID.CrystalStorm)
			{
				tooltips.Add(new TooltipLine(ModLoader.GetMod("Everglow"), "ExtraTooltip", SpellbookGTV("CrystalStorm")));
			}

			if (item.type == ItemID.CursedFlames)
			{
				tooltips.Add(new TooltipLine(ModLoader.GetMod("Everglow"), "ExtraTooltip", SpellbookGTV("CursedFlames")));
			}

			if (item.type == ItemID.GoldenShower)
			{
				tooltips.Add(new TooltipLine(ModLoader.GetMod("Everglow"), "ExtraTooltip", SpellbookGTV("GoldenShower")));
			}

			if (item.type == ItemID.MagnetSphere)
			{
				tooltips.Add(new TooltipLine(ModLoader.GetMod("Everglow"), "ExtraTooltip", SpellbookGTV("MagnetSphere")));
			}

			if (item.type == ItemID.RazorbladeTyphoon)
			{
				tooltips.Add(new TooltipLine(ModLoader.GetMod("Everglow"), "ExtraTooltip", SpellbookGTV("RazorbladeTyphoon")));
			}

			if (item.type == ItemID.LunarFlareBook)
			{
				tooltips.Add(new TooltipLine(ModLoader.GetMod("Everglow"), "ExtraTooltip", SpellbookGTV("LunarFlareBook")));
			}
		}
	}

	internal void CheckItemIsSpellOrNot(Item item, Player player)
	{
		if (player.GetModPlayer<MagicBookPlayer>().MagicBookLevel == 0)
		{
			foreach (int type in MagicBookType)
			{
				if (item.type == type)
				{
					item.noUseGraphic = false;
					if (type == ItemID.MagnetSphere || type == ItemID.DemonScythe)
					{
						item.autoReuse = true;
					}
					break;
				}
			}
		}
		if (player.GetModPlayer<MagicBookPlayer>().MagicBookLevel == 1)
		{
			foreach (int type in MagicBookType)
			{
				if (item.type == type)
				{
					item.noUseGraphic = true;
					if (type == ItemID.MagnetSphere || type == ItemID.DemonScythe)
					{
						item.autoReuse = true;
					}
					break;
				}
			}
		}
	}

	internal void CheckItemIsModSpell(Item item, Player player)
	{
		if (item.ModItem is SpellTomeItem)
		{
			AddContentsToMagicBookList(item.type);
			SpellTomeItem sTI = item.ModItem as SpellTomeItem;
			if (sTI != null)
			{
				foreach (int type in sTI.DecorativeProjectileTypes)
				{
					if (player.ownedProjectileCounts[type] < 1)
					{
						Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, type, 0, 0, player.whoAmI);
					}
				}
			}
		}
	}

	public override bool? UseItem(Item item, Player player)
	{
		CheckItemIsSpellOrNot(item, player);
		if (player.GetModPlayer<MagicBookPlayer>().MagicBookLevel == 1)
		{
			if (item.type == ItemID.WaterBolt)
			{
				int aimType = ModContent.ProjectileType<WaterBoltBook>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}

				aimType = ModContent.ProjectileType<WaterBoltArray>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}
			}
			if (item.type == ItemID.DemonScythe)
			{
				int aimType = ModContent.ProjectileType<Projectiles.DemonScythe.DemonScytheBook>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}

				aimType = ModContent.ProjectileType<Projectiles.DemonScythe.DemonScytheArray>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}
			}
			if (item.type == ItemID.MagnetSphere)
			{
				int aimType = ModContent.ProjectileType<MagnetSphereBook>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}

				aimType = ModContent.ProjectileType<MagnetSphereArray>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}
			}
			if (item.type == ItemID.RazorbladeTyphoon)
			{
				int aimType = ModContent.ProjectileType<RazorbladeTyphoonBook>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, player.HeldItem.damage, 0, player.whoAmI);
				}

				aimType = ModContent.ProjectileType<RazorbladeTyphoonArray>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}
			}
			if (item.type == ItemID.CursedFlames)
			{
				int aimType = ModContent.ProjectileType<CursedFlamesBook>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}

				aimType = ModContent.ProjectileType<CursedFlamesArray>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}
			}
			if (item.type == ItemID.CrystalStorm)
			{
				int aimType = ModContent.ProjectileType<CrystalStormBook>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}

				aimType = ModContent.ProjectileType<CrystalStormArray>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}
			}
			if (item.type == ItemID.BookofSkulls)
			{
				int aimType = ModContent.ProjectileType<BookofSkullsBook>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}

				aimType = ModContent.ProjectileType<BookofSkullsArray>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}
			}
			if (item.type == ItemID.GoldenShower)
			{
				int aimType = ModContent.ProjectileType<GoldenShowerBook>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}

				aimType = ModContent.ProjectileType<GoldenShowerArray>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}
			}
			if (item.type == ItemID.LunarFlareBook)
			{
				int aimType = ModContent.ProjectileType<LunarFlareBook>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}

				aimType = ModContent.ProjectileType<LunarFlareArray>();
				if (player.ownedProjectileCounts[aimType] < 1)
				{
					Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, aimType, 0, 0, player.whoAmI);
				}
			}
			CheckItemIsModSpell(item, player);
		}
		return base.UseItem(item, player);
	}

	public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.GetModPlayer<MagicBookPlayer>().MagicBookLevel == 0)
		{
			return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
		}

		if (item.type == ItemID.WaterBolt)
		{
			return false;
		}

		if (item.type == ItemID.DemonScythe)
		{
			return false;
		}

		if (item.type == ItemID.BookofSkulls)
		{
			return false;
		}

		if (item.type == ItemID.GoldenShower)
		{
			return false;
		}

		if (item.type == ItemID.CursedFlames)
		{
			return false;
		}

		if (item.type == ItemID.CrystalStorm)
		{
			return false;
		}

		if (item.type == ItemID.MagnetSphere)
		{
			return false;
		}

		if (item.type == ItemID.RazorbladeTyphoon)
		{
			return false;
		}

		if (item.type == ItemID.LunarFlareBook)
		{
			return false;
		}

		return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
	}

	public override void HoldItem(Item item, Player player)
	{
		MagicBookPlayer mplayer = player.GetModPlayer<MagicBookPlayer>();
		if (item.type == ItemID.BookofSkulls)
		{
			if (player.GetModPlayer<MagicBookPlayer>().MagicBookLevel == 1)
			{
				if (Main.mouseRight && Main.mouseRightRelease && mplayer.HandCooling <= 0 && player.statMana > player.HeldItem.mana * 2)
				{
					for (int g = -5; g < 150; g++)
					{
						if (Collision.SolidCollision(Main.MouseWorld + new Vector2(0, g * 5 * player.gravDir), 1, 1))
						{
							Vector2 ReleasePoint = Main.MouseWorld + new Vector2(0, g * 5 * player.gravDir);
							var p = Projectile.NewProjectileDirect(item.GetSource_FromAI(), ReleasePoint, Vector2.Zero, ModContent.ProjectileType<SkullHand>(), player.HeldItem.damage * 3, player.HeldItem.knockBack * 6, player.whoAmI);
							p.CritChance = player.GetWeaponCrit(player.HeldItem);

							mplayer.HandCooling = 18;
							player.statMana -= player.HeldItem.mana * 4;
							break;
						}
					}
				}
			}
		}
	}
}

public class MagicBookPlayer : ModPlayer
{
	public int MagicBookLevel = 0;
	public int WaterBoltHasHit = 0;
	public int HandCooling = 0;

	public override void PreUpdate()
	{
		MagicBookLevel = 0;
		if (HandCooling > 0)
		{
			HandCooling--;
		}

		base.PreUpdate();
	}

	public override bool PreItemCheck()
	{
		// MagicBookLevel = 0;
		if (WaterBoltHasHit > 0)
		{
			if (Player.HeldItem.type != ItemID.WaterBolt || MagicBookLevel == 0)
			{
				WaterBoltHasHit = 0;
			}
		}
		return base.PreItemCheck();
	}
}
