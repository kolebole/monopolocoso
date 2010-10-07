// Project: XnaShooterVs2005, File: SpriteHelper.cs
// Namespace: XnaShooter.Graphics, Class: SpriteHelper
// Path: C:\code\XnaBook\XnaShooter\Graphics, Author: Abi
// Code lines: 368, Size of file: 11,31 KB
// Creation date: 27.11.2006 04:51
// Last modified: 27.11.2006 05:07
// Generated with Commenter by abi.exDream.com

#region Using directives
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Texture = XnaShooter.Graphics.Texture;
using XnaShooter.Game;
#endregion

namespace XnaShooter.Graphics
{
	/// <summary>
	/// Sprite helper class to manage and render sprites.
	/// </summary>
	internal class SpriteHelper
	{
		#region SpriteToRender helper class
		/// <summary>
		/// Sprite to render
		/// </summary>
		class SpriteToRender
		{
			#region Variables
			/// <summary>
			/// Texture
			/// </summary>
			public Texture texture;
			/// <summary>
			/// Rectangle
			/// </summary>
			public Rectangle rect;
			/// <summary>
			/// Source pixel rectangle
			/// </summary>
			public Rectangle pixelRect;
			/// <summary>
			/// Color
			/// </summary>
			public Color color;
			/// <summary>
			/// Rotation
			/// </summary>
			public float rotation = 0;
			/// <summary>
			/// Rotation point for rotated sprites.
			/// </summary>
			public Vector2 rotationPoint = Vector2.Zero;
			/// <summary>
			/// Blend mode, defaults to SpriteBlendMode.AlphaBlend
			/// </summary>
			public SpriteBlendMode blendMode = SpriteBlendMode.AlphaBlend;
			#endregion

			#region Constructor
			/// <summary>
			/// Create sprite to render
			/// </summary>
			/// <param name="setTexture">Set texture</param>
			/// <param name="setRect">Set rectangle</param>
			/// <param name="setPixelRect">Set source rectangle</param>
			/// <param name="setColor">Set color</param>
			public SpriteToRender(Texture setTexture, Rectangle setRect,
				Rectangle setPixelRect, Color setColor)
			{
				texture = setTexture;
				rect = setRect;
				pixelRect = setPixelRect;
				color = setColor;
			} // SpriteToRender(setTexture, setRect, setSourceRect)
			
			/// <summary>
			/// Create sprite to render
			/// </summary>
			/// <param name="setTex">Set tex</param>
			/// <param name="setRect">Set rectangle</param>
			/// <param name="setPixelRect">Set pixel rectangle</param>
			/// <param name="setColor">Set color</param>
			/// <param name="alphaMode">Alpha mode</param>
			public SpriteToRender(Texture setTex, Rectangle setRect,
				Rectangle setPixelRect, Color setColor,
				SpriteBlendMode setBlendMode)
			{
				texture = setTex;
				rect = setRect;
				pixelRect = setPixelRect;
				color = setColor;
				blendMode = setBlendMode;
			} // SpriteToRender(setTex, setRect, setPixelRect)

			/// <summary>
			/// Create sprite to render
			/// </summary>
			/// <param name="setTex">Set tex</param>
			/// <param name="setRect">Set rectangle</param>
			/// <param name="setPixelRect">Set pixel rectangle</param>
			/// <param name="setRotation">Set rotation</param>
			/// <param name="setRotationPoint">Set rotation point</param>
			public SpriteToRender(Texture setTex, Rectangle setRect,
				Rectangle setPixelRect, float setRotation, Vector2 setRotationPoint)
			{
				texture = setTex;
				rect = setRect;
				pixelRect = setPixelRect;
				rotation = setRotation;
				rotationPoint = setRotationPoint;
			} // SpriteToRender(setTex, setRect, setPixelRect)
			#endregion
			
			#region Render
			/// <summary>
			/// Render
			/// </summary>
			/// <param name="uiSprites">User interface sprites</param>
			public void Render(SpriteBatch uiSprites)
			{
				// Don't render if texture is null (else XNA throws an exception!)
				if (texture == null ||
					texture.XnaTexture == null ||
					color.A == 0)
					return;

				if (rotation == 0)
					uiSprites.Draw(texture.XnaTexture, rect, pixelRect, color);
				else
					uiSprites.Draw(texture.XnaTexture, rect, pixelRect, color, rotation,
						//new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2),
						rotationPoint,
						SpriteEffects.None, 0);
			} // Render(uiSprites)
			#endregion
		} // class SpriteToRender
		#endregion

		#region Variables
		/// <summary>
		/// Keep a list of all sprites we have to render this frame.
		/// </summary>
		static List<SpriteToRender> sprites =
			new List<SpriteToRender>();
		/// <summary>
		/// Sprite batch for rendering
		/// </summary>
		static SpriteBatch spriteBatch = null;
		#endregion

		#region Dispose
		/// <summary>
		/// Dispose the static spriteBatch and sprites helpers in case
		/// the device gets lost.
		/// </summary>
		public static void Dispose()
		{
			sprites.Clear();
			if (spriteBatch != null)
				spriteBatch.Dispose();
			spriteBatch = null;
		} // Dispose()
		#endregion

		#region Private constructor
		/// <summary>
		/// Create sprite helper, private. Instantiation is not allowed.
		/// </summary>
		private SpriteHelper()
		{
		} // SpriteHelper()
		#endregion

		#region Add sprite to render
		/// <summary>
		/// Add sprite to render
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <param name="rect">Rectangle</param>
		/// <param name="gfxRect">Gfx rectangle</param>
		/// <param name="color">Color</param>
		public static void AddSpriteToRender(
			Texture texture, Rectangle rect, Rectangle gfxRect, Color color)
		{
			sprites.Add(new SpriteToRender(texture, rect, gfxRect, color));
		} // AddSpriteToRender(texture, rect, gfxRect)

		/// <summary>
		/// Add sprite to render
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <param name="rect">Rectangle</param>
		/// <param name="gfxRect">Gfx rectangle</param>
		public static void AddSpriteToRender(
			Texture texture, Rectangle rect, Rectangle gfxRect)
		{
			sprites.Add(new SpriteToRender(texture, rect, gfxRect, Color.White));
		} // AddSpriteToRender(texture, rect, gfxRect)
		
		/// <summary>
		/// Add sprite to render
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <param name="rect">Rectangle</param>
		/// <param name="gfxRect">Gfx rectangle</param>
		public static void AddSpriteToRender(
			Texture texture, Rectangle rect, Rectangle gfxRect, Color color,
			SpriteBlendMode blendMode)
		{
			sprites.Add(
				new SpriteToRender(texture, rect, gfxRect, color, blendMode));
		} // AddSpriteToRender(texture, rect, gfxRect)

		/// <summary>
		/// Add sprite to render
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <param name="rect">Rectangle</param>
		public static void AddSpriteToRender(
			Texture texture, Rectangle rect, Color color)
		{
			AddSpriteToRender(texture, rect, texture.GfxRectangle, color);
		} // AddSpriteToRender(texture, rect, color)
		
		/// <summary>
		/// Add sprite to render
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <param name="rect">Rectangle</param>
		public static void AddSpriteToRender(
			Texture texture, Rectangle rect)
		{
			AddSpriteToRender(texture, rect, texture.GfxRectangle, Color.White);
		} // AddSpriteToRender(texture, rect)

		/// <summary>
		/// Add sprite to render
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <param name="x">X</param>
		/// <param name="y">Y</param>
		public static void AddSpriteToRender(
			Texture texture, int x, int y, Color color)
		{
			AddSpriteToRender(texture,
				new Rectangle(x, y,
				texture.GfxRectangle.Width, texture.GfxRectangle.Height),
				color);
		} // AddSpriteToRender(texture, x, y)

		/// <summary>
		/// Add sprite to render
		/// </summary>
		/// <param name="texture">Texture</param>
		public static void AddSpriteToRender(
			Texture texture, int x, int y)
		{
			AddSpriteToRender(texture,
				new Rectangle(x, y,
				texture.GfxRectangle.Width, texture.GfxRectangle.Height));
		} // AddSpriteToRender(texture, x, y)

		/// <summary>
		/// Add sprite to render
		/// </summary>
		/// <param name="texture">Texture</param>
		public static void AddSpriteToRender(
			Texture texture)
		{
			AddSpriteToRender(texture, new Rectangle(0, 0, 1024, 768));
		} // AddSpriteToRender(texture)
		
		/// <summary>
		/// Add sprite to render
		/// </summary>
		/// <param name="tex">Tex</param>
		/// <param name="rect">Rectangle</param>
		/// <param name="pixelRect">Pixel rectangle</param>
		/// <param name="rotation">Rotation</param>
		public static void AddSpriteToRender(Texture tex,
			Rectangle rect, Rectangle pixelRect,
			float rotation, Vector2 rotationPoint)
		{
			sprites.Add(new SpriteToRender(
				tex, rect, pixelRect, rotation, rotationPoint));
		} // AddSpriteToRender(tex, rect, pixelRect)

		/// <summary>
		/// Add sprite to render centered
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <param name="x">X</param>
		/// <param name="y">Y</param>
		/// <param name="scale">Scale</param>
		public static void AddSpriteToRenderCentered(
			Texture texture, float x, float y, float scale)
		{
			AddSpriteToRender(texture, new Rectangle(
				(int)(x * 1024 - scale * texture.GfxRectangle.Width/2),
				(int)(y * 768 - scale * texture.GfxRectangle.Height/2),
				(int)(scale * texture.GfxRectangle.Width),
				(int)(scale * texture.GfxRectangle.Height)));
		} // AddSpriteToRenderCentered(texture, x, y)

		/// <summary>
		/// Add sprite to render centered
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <param name="pos">Position</param>
		public static void AddSpriteToRenderCentered(
			Texture texture, float x, float y)
		{
			AddSpriteToRenderCentered(texture, x, y, 1);
		} // AddSpriteToRenderCentered(texture, x, y)

		/// <summary>
		/// Add sprite to render centered
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <param name="pos">Position</param>
		public static void AddSpriteToRenderCentered(
			Texture texture, Vector2 pos)
		{
			AddSpriteToRenderCentered(texture, pos.X, pos.Y);
		} // AddSpriteToRenderCentered(texture, pos)
		#endregion

		#region DrawSprites
		/// <summary>
		/// Draw sprites
		/// </summary>
		/// <param name="width">Width</param>
		/// <param name="height">Height</param>
		public static void DrawSprites(int width, int height)
		{
			// No need to render if we got no sprites this frame
			if (sprites.Count == 0)
				return;

			// Create sprite batch if we have not done it yet.
			// Use device from texture to create the sprite batch.
			if (spriteBatch == null)
				spriteBatch = new SpriteBatch(BaseGame.Device);

			// Render all textures on our ui sprite batch
			if (sprites.Count > 0)
			{
				// We can improve a little performance by rendering
				// the additive stuff at first end!
				bool startedAdditiveSpriteMode = false;
				for (int spriteNum = 0; spriteNum < sprites.Count; spriteNum++)
				{
					SpriteToRender sprite = sprites[spriteNum];
					if (sprite.blendMode == SpriteBlendMode.Additive)
					{
						if (startedAdditiveSpriteMode == false)
						{
							startedAdditiveSpriteMode = true;
							spriteBatch.Begin(
								SpriteBlendMode.Additive,
								SpriteSortMode.BackToFront,
								SaveStateMode.None);
						} // if (startedAdditiveSpriteMode)

						sprite.Render(spriteBatch);
					} // if (sprite.blendMode)
				} // for (spriteNum)

				if (startedAdditiveSpriteMode)
					spriteBatch.End();

				// Handle all remembered sprites
				for (int spriteNum = 0; spriteNum < sprites.Count; spriteNum++)
				{
					SpriteToRender sprite = sprites[spriteNum];
					if (sprite.blendMode != SpriteBlendMode.Additive)
					{
						spriteBatch.Begin(
							//SpriteBlendMode.Additive,//.AlphaBlend,
							sprite.blendMode,
							SpriteSortMode.BackToFront,
							SaveStateMode.None);

						sprite.Render(spriteBatch);

						// Dunno why, but for some reason we have start a new sprite
						// for each texture change we have. Else stuff is not rendered
						// in the correct order on top of each other.
						spriteBatch.End();
					} // if (sprite.blendMode)
				} // for (spriteNum)

				// Kill list of remembered sprites
				sprites.Clear();
			} // if (sprites.Count)
		} // DrawSprites(width, height)
		#endregion
	} // class SpriteHelper
} // namespace XnaShooter.Graphics
