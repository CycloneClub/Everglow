using Everglow.Yggdrasil.KelpCurtain.Projectiles.Pets;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs
{
	public class Photophore_lightPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<Photophore_lightPetProj>());
		}
	}
}
