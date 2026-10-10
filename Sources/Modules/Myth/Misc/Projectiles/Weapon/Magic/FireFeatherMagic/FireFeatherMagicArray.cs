using Everglow.SpellAndSkull.GlobalItems;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic.FireFeatherMagic;

internal class FlameRingPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.FlameRing;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_melting.Value);
		effect.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_flameRing.Value);
		Texture2D halo = Commons.ModAsset.Trail.Value;
		Ins.Batch.BindTexture<Vertex2D>(halo);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(FlameRingPipeline), typeof(BloomPipeline))]
internal class FireFeatherMagicArray : VisualProjectile
{
	public float WingPower = 0;
	public bool OldControlUp = false;
	public int timer = 0;
	public Vector2 ringPos = Vector2.Zero;

	public override string Texture => "Everglow/" + ModAsset.FireFeatherMagic_Path;

	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 100000;
		Projectile.tileCollide = false;
		base.SetDefaults();
	}

	public override bool? CanCutTiles()
	{
		return false;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = Projectile.Center * 0.7f + (player.Center + new Vector2(-player.direction * 22, -12 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d))) * 0.3f;
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.HeldItem.type == ModContent.ItemType<Items.Weapons.FireFeatherMagic>() && player.GetModPlayer<MagicBookPlayer>().MagicBookLevel == 1 && player.active && !player.dead)
		{
			Projectile.timeLeft = player.itemTime + 60;
			if (player.itemTime > 0)
			{
				if (timer < 30)
				{
					timer++;
				}
				Player.CompositeArmStretchAmount playerCASA = Player.CompositeArmStretchAmount.Full;
				player.SetCompositeArmFront(true, playerCASA, (float)(-Math.Sin(Main.timeForVisualEffects / 18d) * 0.6 + 1.2) * -player.direction);
				Vector2 vTOMouse = Main.MouseWorld - player.Center;
				player.SetCompositeArmBack(true, playerCASA, (float)(Math.Atan2(vTOMouse.Y, vTOMouse.X) - Math.PI / 2d));
			}
			else
			{
				if (timer > 15)
				{
					timer--;
				}
			}
		}
		else
		{
			timer--;
			if (timer < 0)
			{
				Projectile.Kill();
			}
		}

		Projectile.rotation = player.fullRotation;
		ringPos = ringPos * 0.9f + new Vector2(-12 * player.direction, -24 * player.gravDir) * 0.1f;

		FireFeatherOwner mplayer = player.GetModPlayer<FireFeatherOwner>();
		mplayer.HasFlameWing = false;
		if ((player.wingTime <= 0 || player.wings == 0) && !player.mount._active)
		{
			if (WingPower > 0)
			{
				if (player.controlUp && player.velocity.Y != 0)
				{
					mplayer.HasFlameWing = true;
					WingPower -= 0.8f;
					Item item = new Item();
					item.SetDefaults(ItemID.FlameWings);
					player.equippedWings = item;
					player.wings = item.wingSlot;
					player.wingsLogic = item.wingSlot;
				}
			}
		}
		if (player.controlUp && player.velocity.Y != 0)
		{
			if ((!OldControlUp && player.wingTime <= 0) || player.wingTime == 2)
			{
				for (int j = 0; j < 16; j++)
				{
					Vector2 v = new Vector2(0, Main.rand.NextFloat(7, 20)).RotatedByRandom(MathHelper.TwoPi);
					Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<Dusts.FireFeather>(), v.X, v.Y, 150, default, Main.rand.NextFloat(1.8f, 3.7f));
				}
			}
		}
		if (WingPower >= 210)
		{
			WingPower = 210;
		}
		OldControlUp = player.controlUp && player.velocity.Y != 0;
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			// _coroutineManager.StartCoroutine(new Coroutine(RightClick((int)(WingPower / 21))));
			if (WingPower < 21)
			{
				return;
			}
			WingPower -= 21;
			Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-600, 600), -1600);
			Vector2 vel = Vector2.Normalize(Main.MouseWorld - pos) * 60;
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<GiantFireFeather>(), player.HeldItem.damage * 5, 10, Projectile.owner);
			timer = 30;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Projectile.hide = false;
		return false;
	}

	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public override void Draw()
	{
		Vector2 toBottom = new Vector2(0, 40);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int x = 0; x < 40; x++)
		{
			float pocession = 1 - timer / 30f;
			Vector2 radius = toBottom.RotatedBy(x / 20d * Math.PI);
			float width = 75f;
			if (x / 40f > WingPower / 210f)
			{
				pocession += 0.7f;
			}
			Vector2 normalizedRadious = radius / 40f * MathF.Sin(x / 40f * MathF.PI) * width;
			bars.Add(new Vertex2D(Projectile.Center + radius + normalizedRadious, new Color(x / 40f, 0.0f, pocession, 0.0f), new Vector3(x / 25f, 0 + (float)Main.time * 0.009f, 0)));
			bars.Add(new Vertex2D(Projectile.Center + radius, new Color(x / 40f, 0.8f, pocession, 0.0f), new Vector3(x / 25f, 0.5f + (float)Main.time * 0.009f, 0)));
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}

internal class FireFeatherOwner : ModPlayer
{
	public bool HasFlameWing = false;

	public override void PostUpdateMiscEffects()
	{
		if (HasFlameWing)
		{
			if (Player.ownedProjectileCounts[ModContent.ProjectileType<FireFeatherMagicArray>()] < 1)
			{
				HasFlameWing = false;
				return;
			}
			Item item = new Item();
			item.SetDefaults(ItemID.FlameWings);
			Player.equippedWings = item;
			Player.wings = item.wingSlot;
			Player.wingsLogic = item.wingSlot;
			Player.wingTime += 1;
		}
	}
}
