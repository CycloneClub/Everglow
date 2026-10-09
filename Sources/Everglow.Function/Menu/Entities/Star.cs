namespace Everglow.Commons.Menu.Entities;

internal class Star
{
	public Vector2 position;
	public Color color = Color.White;
	public float alpha = 0;
	public int maxTime = 10;
	public int timeLeft;
	public float scale = 1;

	private float baseScale = 1;
	public virtual void Update()
	{

		if (timeLeft == maxTime)
			baseScale = scale;
		Vector2 rotCenter = new Vector2(960, 820) * new Vector2(Main.UIScale * Main.screenWidth / 1920f, Main.UIScale * Main.screenHeight / 1080f);
		;
		Vector2 rotVec = position - rotCenter;
		rotVec = rotVec.RotatedBy(0.0003f + 0.0004 * baseScale / 2); //星星转速与大小相关，增强空间感
		position = rotCenter + rotVec;
		if (timeLeft > maxTime - 20)
		{
			alpha = MathHelper.Lerp(alpha, 1f, 0.1f);
		}
		else if (timeLeft < 20)
		{
			alpha = MathHelper.Lerp(alpha, 0f, 0.1f);
		}
		scale += (float)Math.Sin(timeLeft * 0.06f) * 0.06f;

	}
	public virtual void Draw()
	{
		Texture2D tex = ModAsset.Entities_Star.Value;
		Main.spriteBatch.Draw(tex, position, null, color * alpha, 0, tex.Size() / 2, scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(tex, position, null, color * alpha, 0, tex.Size() / 2, scale, SpriteEffects.None, 0);
	}
}
