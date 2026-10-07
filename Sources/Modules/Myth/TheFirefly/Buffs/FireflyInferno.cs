using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.TheFirefly.VFXs;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Everglow.Myth.TheFirefly.Buffs;

public class FireflyInferno : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		int buffDamage = (int)(5 + npc.velocity.Length() * 8);

		npc.lifeRegen = -buffDamage;
		npc.SetLifeRegenExpectedLossPerSecond((int)(1 + npc.velocity.Length() * 2.4f));

		if (Main.rand.NextBool(4))
		{
			GenerateSmog(1, new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height)) + npc.position, 1);
		}
		if (Main.rand.NextBool(4))
		{
			GenerateFire(1, new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height)) + npc.position, 1);
		}
		if (Main.rand.NextBool(1))
		{
			GenerateSpark(1, new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height)) + npc.position, 1);
		}

		base.Update(npc, ref buffIndex);
	}

	public override void Update(Player player, ref int buffIndex)
	{
		int buffDamage = (int)(5 + player.velocity.Length() * 8);

		player.lifeRegen = -buffDamage;

		if (Main.rand.NextBool(4))
		{
			GenerateSmog(1, new Vector2(Main.rand.NextFloat(player.width), Main.rand.NextFloat(player.height)) + player.position, 1);
		}
		if (Main.rand.NextBool(4))
		{
			GenerateFire(1, new Vector2(Main.rand.NextFloat(player.width), Main.rand.NextFloat(player.height)) + player.position, 1);
		}
		if (Main.rand.NextBool(1))
		{
			GenerateSpark(1, new Vector2(Main.rand.NextFloat(player.width), Main.rand.NextFloat(player.height)) + player.position, 1);
		}

		base.Update(player, ref buffIndex);
	}

	public void GenerateSmog(int frequency, Vector2 position, int scale)
	{
		float mulVelocity = scale * 0.5f;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new FireSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = position + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 4,
				MaxTime = Main.rand.Next(37, 85),
				Scale = Main.rand.NextFloat(2f, 7f) * mulVelocity,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void GenerateFire(int frequency, Vector2 position, int scale)
	{
		float mulVelocity = scale * 0.5f;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var fire = new MothBlueFireDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = position + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 4,
				MaxTime = Main.rand.Next(9, 55),
				Scale = Main.rand.NextFloat(2f, 7f) * mulVelocity,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void GenerateSpark(int frequency, Vector2 position, int scale)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0.4f, 2.6f)).RotatedByRandom(MathHelper.TwoPi) * scale;
			var smog = new MothShimmerScaleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = position + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				Coord = new Vector2(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f)),
				MaxTime = Main.rand.Next(20, 85),
				Scale = Main.rand.NextFloat(0.4f, 3.4f),
				Rotation = Main.rand.NextFloat(6.283f),
				Rotation2 = Main.rand.NextFloat(6.283f),
				Omega = Main.rand.NextFloat(-30f, 30f),
				Phi = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
	}
}
