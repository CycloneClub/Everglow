using Everglow.Yggdrasil.GreenCore.Projectiles.Melee;

namespace Everglow.Yggdrasil.GreenCore.Items.Weapons
{
	public class ShadowEulogist : ModItem
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.width = 30;
			Item.height = 30;
			Item.useAnimation = 5;
			Item.useTime = 5;
			Item.shootSpeed = 5f;
			Item.knockBack = 6.5f;
			Item.damage = 90;
			Item.rare = ItemRarityID.Green;

			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.noUseGraphic = true;

			Item.value = Item.sellPrice(gold: 1);
		}

		public static BlendState bs;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			/*
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate,CustomBlendStates.Reverse, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            Main.spriteBatch.Draw(Commons.ModAsset.Trail.Value, new Vector2(500, 500), null, Color.White, 0, Vector2.Zero, 1, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            */
		}

		public override bool CanUseItem(Player player)
		{
			if (Main.myPlayer == player.whoAmI)
			{
				Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<ShadowEulogistProj>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
			}
			return base.CanUseItem(player);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			for (int i = 0; i < tooltips.Count; i++)
			{
				float a = 0.5f + (float)Math.Sin(Main.timeForVisualEffects / 50f + i * 4f / tooltips.Count) + 0.5f;
				Color c = new Color(a, a, a, 1);
				tooltips[i].OverrideColor = c;
			}
		}
	}
}
