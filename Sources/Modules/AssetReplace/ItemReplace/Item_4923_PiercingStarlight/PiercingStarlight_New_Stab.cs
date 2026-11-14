using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Terraria.DataStructures;

namespace Everglow.AssetReplace.ItemReplace.Item_4923_PiercingStarlight
{
	public class PiercingStarlight_New_Stab : StabbingProjectile_Stab
	{
		public override void SetCustomDefaults()
		{
			StabDistance = 2.4f;
		}

		public override void OnSpawn(IEntitySource source)
		{
			base.OnSpawn(source);
			float timeValue = (float)(Main.time / 120f);
			StabColor = Main.hslToRgb(timeValue % 1.0f, 1, 0.5f);
			StabColor = Color.Lerp(StabColor, new Color(147, 242, 255), 0.2f);
			StabColor.A = 0;
			HitTileSparkColor = StabColor * 1.2f;
		}
	}
}
