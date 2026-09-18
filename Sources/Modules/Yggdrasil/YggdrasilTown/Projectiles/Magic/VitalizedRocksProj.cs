using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class VitalizedRocksProj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.magic = true;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.width = 50;
		Projectile.height = 50;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 60;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
	}

	private Vector2 p1 = new Vector2(Main.rand.NextFloat(), Main.rand.NextFloat());
	private Vector2 p2 = new Vector2(Main.rand.NextFloat(), Main.rand.NextFloat());
	private Vector2 p3 = new Vector2(Main.rand.NextFloat(), Main.rand.NextFloat());
	private int timer = 0;

	public override void AI()
	{
		Projectile.frameCounter++;
		if (Projectile.frameCounter > 6)
		{
			Projectile.frameCounter = 0;
			Projectile.frame++;
			if (Projectile.frame >= 6)
			{
				Projectile.frame = 0;
			}
		}
		timer--;
		Player player = Main.player[Projectile.owner];
		PlayerHeadToMouse();
		if (player.HeldItem.type == ModContent.ItemType<Items.Weapons.RockElemental.VitalizedRocks>())
		{
			Projectile.timeLeft = 120;
			if (Projectile.ai[0] < 60)
			{
				Projectile.ai[0] += 0.66f;
			}
			if (Projectile.ai[1] < 60)
			{
				Projectile.ai[1] += 0.75f;
			}
		}
		else
		{
			Projectile.ai[0] -= 0.75f;
			Projectile.ai[1] -= 0.66f;
		}
		Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center + new Vector2(player.direction * 2, MathF.Sin((float)Main.timeForVisualEffects / 20)) * 20, 0.2f);
		if (timer <= 0 && player.controlUseItem && player.HeldItem.type == ModContent.ItemType<Items.Weapons.RockElemental.VitalizedRocks>() && player.CheckMana(player.HeldItem.mana, true))
		{
			timer = 30;
			player.itemTime = 33;
			player.itemAnimation = 33;
			if (Main.MouseWorld.X > player.Center.X)
			{
				player.direction = 1;
			}
			else
			{
				player.direction = -1;
			}
			for (int i = 0; i < 3; i++)
			{
				Vector2 Pos = player.Center + Vector2.unitXVector.RotatedByRandom(MathF.PI * 2) * player.direction * Main.rand.NextFloat(25, 75);
				for (int j = 0; j < 50; j++)
				{
					Pos = player.Center + Vector2.unitXVector.RotatedByRandom(MathF.PI * 2) * player.direction * Main.rand.NextFloat(25, 75);
					if (Collision.CanHit(Pos, 38, 38, Main.MouseWorld, 0, 0))
					{
						break;
					}
				}
				Vector2 toMouse = (Main.MouseWorld - Pos).NormalizeSafe();
				int type = ModContent.ProjectileType<VitalizedRocksStone>();
				float speed = 9;
				float damageScale = 1;
				if (i >= 1)
				{
					type = ModContent.ProjectileType<VitalizedRocksStone_small>();
					speed = 8;
					damageScale = 0.6f;
					Vector2 dustPos = Projectile.Center;
					for (int t = 0; t < 200; t++)
					{
						dustPos += (Pos - dustPos).SafeNormalize(Vector2.zeroVector) * 2;
						var dust = Dust.NewDustDirect(dustPos - new Vector2(4), 0, 0, ModContent.DustType<RockElemental_Energy_normal>());
						dust.velocity *= 0;
						dust.noGravity = true;
						dust.scale = 0.6f;
						if ((Pos - dustPos).Length() < 1)
						{
							break;
						}
					}
				}
				else
				{
					Vector2 dustPos = Projectile.Center;
					for (int t = 0; t < 200; t++)
					{
						dustPos += (Pos - dustPos).SafeNormalize(Vector2.zeroVector) * 2;
						var dust = Dust.NewDustDirect(dustPos - new Vector2(4), 0, 0, ModContent.DustType<RockElemental_Energy_normal>());
						dust.velocity *= 0;
						dust.noGravity = true;
						dust.scale = 1f;
						if ((Pos - dustPos).Length() < 1)
						{
							break;
						}
					}
				}
				Projectile.NewProjectileDirect(player.GetSource_FromAI(), Pos, toMouse * speed, type, (int)(Projectile.damage * damageScale), Projectile.knockBack, player.whoAmI);
				switch (Main.rand.Next(4))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle(ModAsset.vitalizedrocks_attack_1_Mod), Projectile.Center);
						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle(ModAsset.vitalizedrocks_attack_2_Mod), Projectile.Center);
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle(ModAsset.vitalizedrocks_attack_3_Mod), Projectile.Center);
						break;
					case 3:
						SoundEngine.PlaySound(new SoundStyle(ModAsset.vitalizedrocks_attack_4_Mod), Projectile.Center);
						break;
				}
			}
		}
		Lighting.AddLight(Projectile.Center, new Vector3(0.7f, 0.2f, 1f) * Projectile.ai[0] / 60f);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect dissolve = ModAsset.DissolveOnPoint.Value;
		float range1 = Projectile.ai[0] / 60;
		float range2 = Projectile.ai[1] / 60;

		dissolve.Parameters["frames"].SetValue(6);
		dissolve.Parameters["range1"].SetValue(range1);
		dissolve.Parameters["range2"].SetValue(range2);
		dissolve.Parameters["p1"].SetValue(p1);
		dissolve.Parameters["p2"].SetValue(p2);
		dissolve.Parameters["p3"].SetValue(p3);

		dissolve.CurrentTechnique.Passes[0].Apply();
		Texture2D texture = ModAsset.VitalizedRocksProj.Value;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 50, 50, 50), lightColor, Projectile.rotation, new Vector2(25), Projectile.scale, SpriteEffects.None, 0);
		texture = ModAsset.VitalizedRocksProj_glow.Value;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 50, 50, 50), new Color(0.7f, 0.1f, 1f, 0), Projectile.rotation, new Vector2(25), Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		return false;
	}

	public void PlayerHeadToMouse()
	{
		Player player = Main.player[Projectile.owner];
		player.headRotation = (player.MouseWorld() - player.Center).ToRotation();
	}
}
