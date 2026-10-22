using System.Reflection;
using Everglow.Commons.Coroutines;
using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities;
using ReLogic.Graphics;
using SubworldLibrary;
using Terraria.GameContent;
using Terraria.UI.Chat;
using static Everglow.Commons.Mechanics.BiomesText.VanillaBiomes;

namespace Everglow.Commons.Mechanics.BiomesText;

public class BiomeLabelSystem : ModSystem
{
	public const int Height = 100;
	public const int TimeLag = 50;

	public int duration = -1;
	private CoroutineManager _coroutineManager;

	public List<ModBiome> TotalModBiomes;
	public Dictionary<ModBiome, int> ActiveModBiomeCache;
	public Dictionary<string, int> ActiveVanillaBiomeCache;
	public Queue<(string, Texture2D)> DisplayBiomeQueue;

	public override void Load()
	{
		_coroutineManager = new();
		ActiveModBiomeCache = [];
		ActiveVanillaBiomeCache = [];
		DisplayBiomeQueue = [];
	}

	public override void Unload()
	{
		_coroutineManager = null;

		TotalModBiomes?.Clear();
		TotalModBiomes = null;

		ActiveModBiomeCache?.Clear();
		ActiveModBiomeCache = null;

		ActiveVanillaBiomeCache?.Clear();
		ActiveVanillaBiomeCache = null;

		DisplayBiomeQueue?.Clear();
		DisplayBiomeQueue = null;
	}

	public override void PostSetupContent()
	{
		TotalModBiomes ??= (List<ModBiome>)typeof(BiomeLoader).GetField("list", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(LoaderManager.Get<BiomeLoader>());
	}

	public override void OnWorldLoad()
	{
		foreach (ModBiome modBiome in TotalModBiomes)
		{
			ActiveModBiomeCache[modBiome] = 0;
		}

		foreach (var key in BiomeKeys)
		{
			ActiveVanillaBiomeCache[key] = 0;
		}
	}

	public override void PostDrawInterface(SpriteBatch spriteBatch)
	{
		Player player = Main.LocalPlayer;
		foreach (ModBiome modBiome in TotalModBiomes)
		{
			if (modBiome.IsBiomeActive(player))
			{
				if (ActiveModBiomeCache[modBiome] < TimeLag)
				{
					ActiveModBiomeCache[modBiome]++;
					if (ActiveModBiomeCache[modBiome] == TimeLag)
					{
						var biomeIcon = ModContent.HasAsset(modBiome.BestiaryIcon)
							? ModContent.Request<Texture2D>(modBiome.BestiaryIcon, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value
							: ModAsset.Cavern.Value;

						DisplayBiomeQueue.Enqueue((modBiome.DisplayName.Value, biomeIcon));
					}
				}
			}
			else
			{
				if (ActiveModBiomeCache[modBiome] > 0)
				{
					ActiveModBiomeCache[modBiome] = 0;
				}
			}
		}

		if (!SubworldSystem.AnyActive())
		{
			foreach (var key in ActiveVanillaBiomeCache.Keys)
			{
				CheckNewActiveVanillaBiome(key, player);
			}
		}

		if (DisplayBiomeQueue.Count > 4)
		{
			DisplayBiomeQueue.Dequeue();
		}
		if (duration == -1)
		{
			if (DisplayBiomeQueue.Count > 1)
			{
				_coroutineManager.StartCoroutine(new Coroutine(DrawBiomeLabel(spriteBatch, DisplayBiomeQueue.Dequeue(), 60)));
			}
			else if (DisplayBiomeQueue.Count > 0)
			{
				_coroutineManager.StartCoroutine(new Coroutine(DrawBiomeLabel(spriteBatch, DisplayBiomeQueue.Dequeue(), 240)));
			}
		}
		_coroutineManager.Update();
	}

	private void CheckNewActiveVanillaBiome(string key, Player player)
	{
		if (BiomeKeys.Contains(key))
		{
			var biomeData = VanillaBiomeIndex[key];
			if (biomeData.Condition(player))
			{
				if (ActiveVanillaBiomeCache[key] < TimeLag)
				{
					ActiveVanillaBiomeCache[key]++;
					if (ActiveVanillaBiomeCache[key] == TimeLag)
					{
						DisplayBiomeQueue.Enqueue((biomeData.DisplayName, biomeData.Icon));
					}
				}
			}
			else
			{
				if (ActiveVanillaBiomeCache[key] > 0)
				{
					ActiveVanillaBiomeCache[key] = 0;
				}
			}
		}
	}

	private IEnumerator<ICoroutineInstruction> DrawBiomeLabel(SpriteBatch spriteBatch, (string DisplayName, Texture2D Icon) biomedata, int time = 120)
	{
		Texture2D labelTexture = ModAsset.Textboard.Value;
		Texture2D biomeIcon = biomedata.Icon;
		Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, biomedata.DisplayName, Vector2.One);
		float mulWidth = 1f;
		if (stringSize.X > 58)
		{
			mulWidth = stringSize.X / 58f;
		}
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Main.GameViewMatrix.EffectMatrix;
		float screenCoordY = 100;
		for (int i = 0; i < time; i++)
		{
			duration = i;
			SpriteBatchState spriteBatchState = spriteBatch.GetState().Value;
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, spriteBatch.transformMatrix);
			int drawY = Height * (int)(i / (time / 120f));
			spriteBatch.Draw(labelTexture, new Vector2(Main.screenWidth / 2f - 29 * mulWidth, screenCoordY), new Rectangle(0, drawY, 144, Height), Color.White, 0, new Vector2(144, 50), 1f, SpriteEffects.None, 0);
			spriteBatch.Draw(labelTexture, new Vector2(Main.screenWidth / 2f, screenCoordY), new Rectangle(144, drawY, 58, Height), Color.White, 0, new Vector2(29, 50), new Vector2(mulWidth, 1f), SpriteEffects.None, 0);
			spriteBatch.Draw(labelTexture, new Vector2(Main.screenWidth / 2f + 29 * mulWidth, screenCoordY), new Rectangle(202, drawY, 122, Height), Color.White, 0, new Vector2(0, 50), 1f, SpriteEffects.None, 0);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
			Effect dissolve = ModAsset.Dissolve_BiomeLabel.Value;

			float dissolveDuration = (1f - i / (float)time) * 2;
			float alpha = (i - time / 9f) / (time / 18f);
			alpha = Math.Clamp(alpha, 0, 1);
			dissolve.Parameters["uTransform"].SetValue(model * projection);
			dissolve.Parameters["uNoise"].SetValue(ModAsset.Noise_Spider_Net.Value);
			dissolve.Parameters["duration"].SetValue(dissolveDuration);
			dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
			dissolve.Parameters["uNoiseSize"].SetValue(16f);
			dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(0.44f, 0.36f));
			dissolve.CurrentTechnique.Passes[0].Apply();
			spriteBatch.DrawString(FontAssets.MouseText.Value, biomedata.DisplayName, new Vector2(Main.screenWidth / 2f, screenCoordY - 20), Color.White * alpha, 0, new Vector2(stringSize.X * 0.5f, 0), 1f, SpriteEffects.None, 0);
			spriteBatch.Draw(biomeIcon, new Vector2(Main.screenWidth / 2f - 29 * mulWidth - 37, screenCoordY - 4), null, Color.White * alpha, 0, biomeIcon.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			spriteBatch.End();
			spriteBatch.Begin(spriteBatchState);
			if (i == time - 1)
			{
				duration = -1;
			}
			else
			{
				if (Main.gamePaused)
				{
					i -= 1;
				}
			}
			yield return new SkipThisFrame();
		}
	}
}
