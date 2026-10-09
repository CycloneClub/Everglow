using Everglow.Myth.Acytaea.VFXs;

namespace Everglow.Myth.Acytaea.Buffs;

public class AcytaeaInferno : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}
	public override void Update(NPC npc, ref int buffIndex)
	{
		int buffDamage = 100;

		npc.lifeRegen = -buffDamage;
		npc.SetLifeRegenExpectedLossPerSecond(10);

		GenerateFire(1, npc.Center);

		base.Update(npc, ref buffIndex);
	}
	public override void Update(Player player, ref int buffIndex)
	{
		int buffDamage = (int)(5 + player.velocity.Length() * 8);

		player.lifeRegen = -buffDamage;

		GenerateFire(1, player.Center);
		base.Update(player, ref buffIndex);
	}
	public void GenerateFire(int amount, Vector2 position)
	{
		for (int x = 0; x < amount; x++)
		{
			Vector2 newVec = new Vector2(0, Main.rand.NextFloat(1.8f, 4f)).RotatedByRandom(6.238f);
			var positionVFX = position;

			var acytaeaFlame = new AcytaeaFlameDust
			{
				velocity = newVec,
				Active = true,
				Visible = true,
				position = positionVFX - newVec * 4,
				maxTime = Main.rand.Next(14, 26),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.10f, 0.10f), Main.rand.NextFloat(8f, 11f) },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
	}
}
