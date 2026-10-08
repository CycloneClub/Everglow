using Everglow.EternalResolve.Items.Weapons.StabbingSwords;

namespace Everglow.EternalResolve.Bosses.Projectiles
{
	public class CrutchRapier_Stab_Hostile : Rapier_Hostile_Stab
	{
		public override void SetDefaults()
		{
			Color = new Color(155, 162, 164);
			base.SetDefaults();
			OldColorFactor = 0.3f;
			CurrentColorFactor = 0.2f;
			ShadeMultiplicative_Modifier = 0.64f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 1f;
			LightColorValueMultiplicative_Modifier = 0.4f;
			AttackLength = 1.05f;
			AttackEffectWidth = 0.4f;
			ItemType = ModContent.ItemType<CrutchBayonet>();

		}
		public override void DrawEffect(Color lightColor)
		{
			base.DrawEffect(lightColor);
		}
		public override void AI()
		{
			base.AI();
		}
	}
}
