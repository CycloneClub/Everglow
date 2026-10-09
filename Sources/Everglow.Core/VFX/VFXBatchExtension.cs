namespace Everglow.Commons.VFX;

public static class VFXBatchExtension
{
	public static void Draw(this VFXBatch spriteBatch, Texture2D texture, Vector2 position, Color color)
		=> spriteBatch.BindTexture(texture).Draw(position, color);

	public static void Draw(this VFXBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		=> spriteBatch.BindTexture(texture).Draw(position, sourceRectangle, color);

	public static void Draw(this VFXBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects)
		=> spriteBatch.BindTexture(texture).Draw(position, sourceRectangle, color, rotation, origin, scale, effects);

	public static void Draw(this VFXBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
		=> spriteBatch.BindTexture(texture).Draw(position, sourceRectangle, color, rotation, origin, scale, effects);

	public static void Draw(this VFXBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, Matrix matrix)
		=> spriteBatch.BindTexture(texture).Draw(position, sourceRectangle, color, matrix);

	public static void Draw(this VFXBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, Color color)
		=> spriteBatch.BindTexture(texture).Draw(destinationRectangle, color);

	public static void Draw(this VFXBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		=> spriteBatch.BindTexture(texture).Draw(destinationRectangle, sourceRectangle, color);

	public static void Draw(this VFXBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects)
		=> spriteBatch.BindTexture(texture).Draw(destinationRectangle, sourceRectangle, color, rotation, origin, effects);

	public static void Draw<T>(this VFXBatch spriteBatch, Texture2D texture, IEnumerable<T> vertices, PrimitiveType type)
		where T : struct, IVertexType
		=> spriteBatch.BindTexture<T>(texture).Draw(vertices, type);
}
