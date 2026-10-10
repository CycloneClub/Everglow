using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.Common.Fish;

public class FishGlobalItem : GlobalItem
{
	/// <summary>
	/// 是否可以被钩取
	/// </summary>
	public bool Fishable = false;

	/// <summary>
	/// 在水面上漂流的速度
	/// </summary>
	public float FloatSpeed = 0f;

	/// <summary>
	/// 现在被哪个浮标勾住
	/// </summary>
	public Projectile HookedBy = null;

	/// <summary>
	/// 是否被鼠标覆盖
	/// </summary>
	public bool Hovered = false;

	/// <summary>
	/// 当前是否有鱼钩在附近
	/// </summary>
	public bool Hookable = false;

	/// <summary>
	/// 生成这个物品的 FishableItem
	/// </summary>
	public FishableItem FishableInfo;

	public int HoverTick = 0;

	public override bool InstancePerEntity => true;

	public static bool IsFishable(Item item)
	{
		if (item.TryGetGlobalItem(out FishGlobalItem globalItem))
		{
			return globalItem.Fishable;
		}
		return false;
	}

	public static void MarkFishable(Item item, bool fishable)
	{
		if (item.TryGetGlobalItem(out FishGlobalItem globalItem))
		{
			globalItem.Fishable = fishable;
		}
	}

	public override void OnSpawn(WorldItem worldItem, IEntitySource source)
	{
		base.OnSpawn(worldItem, source);
	}

	public override bool OnPickup(WorldItem worldItem, Player player)
	{
		HookedBy = null;
		Hookable = false;
		Hovered = false;
		return base.OnPickup(worldItem, player);
	}

	public override void OnStack(Item destination, Item source, int numToTransfer)
	{
		base.OnStack(destination, source, numToTransfer);
	}

	public override void SplitStack(Item destination, Item source, int numToTransfer)
	{
		base.SplitStack(destination, source, numToTransfer);
	}

	public override void OnConsumeItem(Item item, Player player)
	{
		base.OnConsumeItem(item, player);
	}

	public override bool ConsumeItem(Item item, Player player)
	{
		return base.ConsumeItem(item, player);
	}

	public void CheckHover(WorldItem worldItem)
	{
		if (HookedBy != null)
		{
			HoverTick = 0;
			Hovered = false;
			return;
		}
		Vector2 screenPos = worldItem.position - Main.screenPosition;
		if (
			Main.mouseX >= screenPos.X - 10 && Main.mouseY >= screenPos.Y - 10 &&
			Main.mouseX < screenPos.X + worldItem.width + 10 &&
			Main.mouseY < screenPos.Y + worldItem.height + 10
		)
		{
			Hovered = true;
			HoverTick = 0;
		}
		else
		{
			Hovered = false;
		}
	}

	public void CheckHookable(WorldItem worldItem)
	{
		Hookable = false;
		foreach (var proj in Main.projectile)
		{
			if (!proj.bobber || !proj.active)
			{
				continue;
			}
			if (proj.Center.Distance(worldItem.Center) < 64f)
			{
				Hookable = true;
			}
		}
	}

	public void CheckRightClick(WorldItem worldItem)
	{
		if (!Main.mouseRight)
		{
			return;
		}
		HookedBy = null;
		if (Hookable && Hovered)
		{
			foreach (var proj in Main.projectile)
			{
				if (!proj.bobber || !proj.active)
				{
					continue;
				}
				if (proj.Center.Distance(worldItem.Center) < 64f)
				{
					HookedBy = proj;
				}
			}
		}
	}

	public void UpdateOnVanillaLiquid(WorldItem worldItem, ref float gravity)
	{
		Point itemPos = worldItem.Center.ToTileCoordinates();
		byte liquid = Main.tile[itemPos].LiquidAmount;
		if (liquid > 0)
		{
			float percent = liquid / 255f;
			float liqY = percent * 16f + itemPos.Y * 16f;
			float delta = worldItem.Center.Y - liqY;
			if (delta < 0 || liquid == 255)
			{
				float ratio = Main.rand.NextFloat(1) + 0.5f;
				float buoyancy = Math.Clamp(-delta / worldItem.height * 2, 0, 1);
				gravity *= -ratio * buoyancy;
				worldItem.velocity.Y *= 0.9f;
				worldItem.velocity.X = FloatSpeed;
			}
		}
	}

	public override void Update(WorldItem worldItem, ref float gravity, ref float maxFallSpeed)
	{
		if (!IsFishable(worldItem.inner))
		{
			return;
		}
		if (HookedBy != null)
		{
			worldItem.position = HookedBy.position;
			worldItem.velocity = HookedBy.velocity;
			Hovered = false;
			Hookable = false;
			return;
		}
		else
		{
			UpdateOnVanillaLiquid(worldItem, ref gravity);
		}
		CheckHover(worldItem);
		CheckHookable(worldItem);
		CheckRightClick(worldItem);
		base.Update(worldItem, ref gravity, ref maxFallSpeed);
	}

	public override void GrabRange(WorldItem worldItem, Player player, ref int grabRange)
	{
		if (Fishable && HookedBy == null)
		{
			grabRange = 0;
		}
		base.GrabRange(worldItem, player, ref grabRange);
	}

	public override bool CanPickup(WorldItem worldItem, Player player)
	{
		if (Fishable && HookedBy == null)
		{
			return false;
		}
		return base.CanPickup(worldItem, player);
	}

	public override void PostDrawInWorld(WorldItem worldItem, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
	{
		Vector2 basePos = worldItem.Center - Main.screenPosition;
		if (Hookable)
		{
			Texture2D icon = ModAsset.HookItemIcon.Value;
			spriteBatch.Draw(icon, new Rectangle((int)basePos.X - worldItem.width / 2 - 4, (int)basePos.Y - worldItem.width / 2 - 4, worldItem.width + 8, worldItem.width + 8), Color.White);
		}
		if (Hovered && Hookable)
		{
			// var font = FontAssets.MouseText.Value;
			// string hint = "Right click to hook";
			// Vector2 size = font.MeasureString(hint);
			// spriteBatch.DrawString(font, hint, basePos - size / 2 - new Vector2(0, worldItem.height / 2 + 20), Color.Gold);
		}
		base.PostDrawInWorld(worldItem, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
	}
}
