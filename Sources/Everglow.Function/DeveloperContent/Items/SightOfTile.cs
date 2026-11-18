using Everglow.Commons.TileHelper;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Commons.DeveloperContent.Items;

public class SightOfTile : ModItem
{
	public override string LocalizationCategory => Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Magic;
		Item.damage = 0;
		Item.width = 34;
		Item.height = 34;
		Item.useTime = 2;
		Item.useAnimation = 2;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item44;
		Item.shoot = ModContent.ProjectileType<SightOfTileProj>();
		Item.noUseGraphic = true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (ModContent.GetInstance<SightOfTileUI>().InTile)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.rand.NextFloat(0, 200f), Main.rand.NextFloat(0, 200f));
			return false;
		}
		if (ModContent.GetInstance<SightOfTileUI>().OutTile)
		{
			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SightOfTileProjRead>(), damage, knockback, player.whoAmI, Main.rand.NextFloat(0, 200f), Main.rand.NextFloat(0, 200f));
			return false;
		}
		return false;
	}

	public override bool CanUseItem(Player player)
	{
		return !ModContent.GetInstance<SightOfTileUI>().EnableMapIOUI;
	}

	public override void HoldItem(Player player)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<SightOfTileProj>()] + player.ownedProjectileCounts[ModContent.ProjectileType<SightOfTileProjRead>()] == 0)
		{
			if (Main.mouseRight && Main.mouseRightRelease)
			{
				if (ModContent.GetInstance<SightOfTileUI>().EnableMapIOUI)
				{
					ModContent.GetInstance<SightOfTileUI>().CloseUI();
				}
				else
				{
					ModContent.GetInstance<SightOfTileUI>().OpenUI();
				}
			}
		}
		base.HoldItem(player);
	}
}

internal class SightOfTileUI : ModSystem
{
	public class UICircle
	{
		public float Size;
		public Vector2 AddCenter;
		public Texture2D texcoord;
		public Texture2D contain;

		public UICircle(float size, Vector2 addCenter, Texture2D texcoord, Texture2D contain)
		{
			Size = 0f;
			AddCenter = Vector2.Zero;
			this.texcoord = ModAsset.Wires_0.Value;
			this.contain = ModAsset.InTile.Value;
		}
	}

	private UICircle circle0 = new(0f, Vector2.Zero, ModAsset.Wires_0.Value, ModAsset.InTile.Value);
	private UICircle circle1 = new(0f, Vector2.Zero, ModAsset.Wires_0.Value, ModAsset.OutTile.Value);
	public Vector2 DrawCenter;
	private int animationTimer = 0;
	public bool EnableMapIOUI = false;
	public bool InTile = true;
	public bool OutTile = true;
	public List<string> fileName = new();
	public string OutFileName;

	public override void PostDrawInterface(SpriteBatch spriteBatch)
	{
		if (EnableMapIOUI)
		{
			if (animationTimer < 30)
			{
				animationTimer += 3;
			}
			else
			{
				animationTimer = 30;
			}
		}
		else
		{
			if (animationTimer > 0)
			{
				animationTimer -= 3;
			}
			else
			{
				animationTimer = 0;
			}
		}
		if (animationTimer > 0)
		{
			circle0.contain = ModAsset.InTile.Value;
			circle1.contain = ModAsset.OutTile.Value;
			UpdateAndDraw(circle0, 0);
			UpdateAndDraw(circle1, 1);
		}
		base.PostDrawInterface(spriteBatch);
	}

	public void UpdateAndDraw(UICircle c0, int Count)
	{
		c0.AddCenter = new Vector2(0, animationTimer * 1.0f).RotatedBy(animationTimer / 60d * Math.PI + Count * Math.PI);
		if (animationTimer < 30)
		{
			c0.Size = animationTimer / 30f;
		}
		else
		{
			CheckMouseOver(c0);
		}
		DrawUICircle(c0);
		CheckMouseClick(c0, Count);
		if (Count == 1 && fileName != null && OutTile)
		{
			DrawSquireUI(c0, fileName);
		}
	}

	public void CheckMouseClick(UICircle c0, int Count)
	{
		if ((Main.MouseScreen - DrawCenter - c0.AddCenter).Length() < 20)
		{
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				if (Count == 0)
				{
					InTile = true;
					OutTile = false;
					CloseUI();
				}
				if (Count == 1)
				{
					OutTile = !OutTile;
					string rootpath = Path.Combine(Main.SavePath, "Mods", "ModDatas", Mod.Name);
					if (Directory.Exists(rootpath))
					{
						List<string> result = new();

						// 查找根目录下全部以.mapio为结尾的文件
						FindFiles(rootpath, ".mapio", ref result);
						fileName = result;

						// 获取根目录下当前层级全部与*.mapio匹配的文件(仅文件)
						// string[] files = Directory.GetFiles(rootpath, "*.mapio");
					}
					else
					{
						Directory.CreateDirectory(rootpath);
					}
				}
			}
		}
	}

	public void FindFiles(string rootpath, string extension, ref List<string> filepaths)
	{
		// 获取根目录下所有的文件和文件夹路径
		string[] paths = Directory.GetFileSystemEntries(rootpath);
		foreach (string path in paths)
		{
			// 判断是不是文件
			if (File.Exists(path))
			{
				// 查询文件名是否以目标结尾
				if (path.EndsWith(extension))
				{
					filepaths.Add(path);
				}
			}
			else
			{
				// 查询这个文件夹下以目标结尾的文件
				FindFiles(path, extension, ref filepaths);
			}
		}
	}

	public void DrawSquireUI(UICircle c0, List<string> path)
	{
		if (!InTile)
		{
			int count = 0;
			int Dx = 18;
			int Dy = 0;
			foreach (string name in path)
			{
				count++;
				Dx += 18;
				Texture2D sqrt = ModAsset.TileBlock.Value;
				Vector2 DrawPos = DrawCenter + c0.AddCenter + new Vector2(Dx, Dy);
				if (!new Rectangle(0, 0, Main.screenWidth, Main.screenHeight).Contains(new Rectangle((int)DrawPos.X - 9, (int)DrawPos.Y - 9, 18, 18)))
				{
					Dx = 36;
					Dy += 18;
				}
				DrawPos = DrawCenter + c0.AddCenter + new Vector2(Dx, Dy);
				Color DrawColor = Color.White;
				float DrawScale = 1f;
				if (Math.Abs((Main.MouseScreen - DrawPos).X) < 9 && Math.Abs((Main.MouseScreen - DrawPos).Y) < 9)
				{
					DrawScale = 1.2f;
					DrawColor = Color.Yellow;
					int index = name.LastIndexOf("\\");
					var mapIO = new MapIO((int)(Main.MouseWorld.X / 16), (int)(Main.MouseWorld.Y / 16));
					int TWidth = mapIO.ReadWidth(name);
					int THeight = mapIO.ReadHeight(name);
					string LastTime = "Creat at: " + File.GetCreationTime(name).ToString(string.Empty);
					string TSize = TWidth.ToString() + "x" + THeight.ToString();
					Main.instance.MouseText(name[(index + 1)..] + "\n" + TSize + "\n" + LastTime);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						OutFileName = name;
						InTile = false;
						OutTile = true;
						CloseUI();
					}
				}
				Main.spriteBatch.Draw(sqrt, DrawPos, null, DrawColor, 0, sqrt.Size() / 2f, DrawScale, SpriteEffects.None, 0);
			}
		}
	}

	public void CheckMouseOver(UICircle c0)
	{
		if ((Main.MouseScreen - DrawCenter - c0.AddCenter).Length() < 20 && c0.texcoord != ModAsset.Wires_1.Value)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			c0.Size = 1.2f;
			c0.texcoord = ModAsset.Wires_1.Value;
		}
		if ((Main.MouseScreen - DrawCenter - c0.AddCenter).Length() >= 20 && c0.texcoord != ModAsset.Wires_0.Value)
		{
			c0.Size = 1f;
			c0.texcoord = ModAsset.Wires_0.Value;
		}
	}

	public void DrawUICircle(UICircle c0)
	{
		Main.spriteBatch.Draw(c0.texcoord, DrawCenter + c0.AddCenter, null, Color.White, 0, c0.texcoord.Size() / 2f, c0.Size, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(c0.contain, DrawCenter + c0.AddCenter, null, Color.White, 0, c0.contain.Size() / 2f, c0.Size, SpriteEffects.None, 0);
	}

	public void OpenUI()
	{
		DrawCenter = Main.MouseScreen / Main.UIScale;
		circle0.texcoord = ModAsset.Wires_0.Value;
		circle1.texcoord = ModAsset.Wires_0.Value;
		InTile = false;
		OutTile = false;
		EnableMapIOUI = true;
	}

	public void CloseUI()
	{
		circle0.texcoord = ModAsset.Wires_0.Value;
		circle1.texcoord = ModAsset.Wires_0.Value;
		EnableMapIOUI = false;
	}

	public override void UpdateUI(GameTime gameTime)
	{
		if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<SightOfTile>())
		{
			CloseUI();
		}

		base.UpdateUI(gameTime);
	}
}
