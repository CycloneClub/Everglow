using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Everglow.Commons.Templates.Weapons.CrossBow
{
	public abstract class CrossBowProjectile : ModProjectile
	{
		// Keep virtual draw helpers on the player supplied by tML, including mannequins.
		protected Player DrawPlayer { get; set; }

		public Texture2D CrossBowTexture;
		public Texture2D ChordTexture;
		public Vector2 HeldPoint;
		public Vector2 HeldProjectileOffset;
		public int ShootProjType = -1;
		public Item Weapon;

		public override void SetDefaults()
		{
			Projectile.drawLayer = ProjectileDrawLayerID.HeldProj;
			Projectile.usesOwnerLight = true;
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Ranged;
			HeldProjectileOffset = new Vector2(0);
			SetDef();
		}

		public virtual void SetDef()
		{
		}

		public override void OnSpawn(IEntitySource source)
		{
			Player player = Main.player[Projectile.owner];
			Weapon = player.HeldItem;
			Projectile.timeLeft = player.itemTime + 1;
			base.OnSpawn(source);
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			if (Weapon == null)
			{
				Projectile.Kill();
				return;
			}
			if (Main.myPlayer == Projectile.owner)
			{
				Vector2 toMouse = Main.MouseWorld - player.RotatedRelativePoint(player.MountedCenter);
				toMouse.Normalize();
				if (toMouse.HasNaNs())
				{
					toMouse = Vector2.UnitX * player.direction;
				}
				if (toMouse.X < 0)
				{
					player.direction = -1;
				}
				else
				{
					player.direction = 1;
				}
				float mulHeldPos = 4f;
				if (player.itemAnimationMax - player.itemAnimation < 5)
				{
					float value = (player.itemAnimationMax - player.itemAnimation) / 5f;
					mulHeldPos *= value * value;
				}
				mulHeldPos += 8f;
				toMouse *= mulHeldPos;
				if (toMouse.X != Projectile.velocity.X || toMouse.Y != Projectile.velocity.Y)
				{
					Projectile.netUpdate = true;
				}
				Projectile.velocity = toMouse;
				Projectile.rotation = toMouse.ToRotation();
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
				if (player.gravDir == -1)
				{
					player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, new Vector2(toMouse.X, -toMouse.Y).ToRotation() - MathHelper.PiOver2);
				}

				Projectile.Center = player.MountedCenter + toMouse + new Vector2(HeldProjectileOffset.X * player.direction, HeldProjectileOffset.Y * player.gravDir);
				player.heldProj = Projectile.whoAmI;
				if (player.itemTime == 1 && Weapon != null)
				{
					Shoot();
				}
			}
			else
			{
				Projectile.Kill();
			}
		}

		public virtual void Shoot()
		{
			if (Weapon == null || ShootProjType == -1)
			{
				return;
			}
			Player player = Main.player[Projectile.owner];
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + HeldPoint, Projectile.velocity.SafeNormalize(Vector2.zeroVector) * Weapon.shootSpeed, ShootProjType, Weapon.damage, Weapon.knockBack, player.whoAmI);
			SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
			Projectile.Kill();
		}

		public override bool PreDraw(Player player, ref Color lightColor)
		{
			Player previousDrawPlayer = DrawPlayer;
			DrawPlayer = player;
			try
			{
				DrawArrow(Main.spriteBatch, lightColor);
				DrawCrossBow(Main.spriteBatch, lightColor);
				DrawChord(Main.spriteBatch, lightColor);
				return false;
			}
			finally
			{
				DrawPlayer = previousDrawPlayer;
			}
		}

		public virtual void DrawArrow(SpriteBatch spriteBatch, Color lightColor)
		{
			Player player = DrawPlayer ?? Main.player[Projectile.owner];
			ModItem crossBow = player.HeldItem.ModItem;
			if (ShootProjType > 0)
			{
				Texture2D arrow = TextureAssets.Projectile[ShootProjType].Value;
			}
		}

		public virtual void DrawCrossBow(SpriteBatch spriteBatch, Color lightColor)
		{
			Player player = DrawPlayer ?? Main.player[Projectile.owner];
			if (player == null)
			{
				return;
			}
			if (CrossBowTexture != null)
			{
				spriteBatch.Draw(CrossBowTexture, Projectile.Center + HeldPoint - Main.screenPosition, null, lightColor, Projectile.rotation, CrossBowTexture.Size() * 0.5f, Projectile.scale, player.direction * player.gravDir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
			}
		}

		public virtual void DrawChord(SpriteBatch spriteBatch, Color lightColor)
		{
		}
	}
}
