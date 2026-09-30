using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.ProjectileEffects;

[Pipeline(typeof(MiningPowerPickaxe_Proj_WavePipeline))]
public class MiningPowerPickaxe_Proj_Wave : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public List<Point16> Tiles = [];

	public float Value;

	public Player Owner;

	public bool ShouldKill = false;

	public override void Update()
	{
		if (Owner is null || Owner.HeldItem is null || Owner.HeldItem.type != ModContent.ItemType<MiningPowerPickaxe>() || !Owner.controlUseItem)
		{
			ShouldKill = true;
		}
		if (!ShouldKill)
		{
			if (Value < 30)
			{
				Value++;
			}
		}
		else
		{
			Value--;
		}
		if (Value <= 0)
		{
			Active = false;
			return;
		}
	}

	public override void Draw()
	{
		Texture2D tex = Commons.ModAsset.Noise_forceField_medium.Value;
		float value = ((float)Main.time * 0.2f) % 8f;
		foreach (var pos in Tiles)
		{
			var tile = TileUtils.SafeGetTile(pos);
			Rectangle frame = new Rectangle(pos.X * 16, pos.Y * 16, 16, 16);
			Color c0 = Color.White;
			Color drawC = Color.Lerp(c0, new Color(0.05f, 0.4f, 0.8f, 0.3f), value / 8f) * (Value / 30f);
			drawC = Color.Lerp(drawC, Color.Transparent, value / 8f);
			Ins.Batch.Draw(tex, frame, frame, drawC);
		}
	}
}
