// Project: XnaShooter, File: VBScreenHelper.cs
// Namespace: XnaShooter.Shaders, Class: VBScreenHelper
// Path: C:\code\XnaShooter\Shaders, Author: Abi
// Code lines: 294, Size of file: 8,99 KB
// Creation date: 11.09.2006 09:09
// Last modified: 11.09.2006 09:10
// Generated with Commenter by abi.exDream.com

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using XnaShooter.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaShooter.Game;
#endregion

namespace XnaShooter.Shaders
{
	/// <summary>
	/// The VBScreenHelper class helps you to create and use vertex buffers for
	/// on screen rendering used in PreScreen and PostScreen shaders.
	/// You don't have to create a VB for every shader anymore, just call
	/// VBScreenHelper.Render() and everything will be created for you and
	/// reused if same parameters are requested again!
	/// Supports also Grid screen rendering required for radial motion blur.
	/// </summary>
	public class VBScreenHelper
	{
		#region VBScreen helper class
		/// <summary>
		/// VBScreen holds all data for the vbScreens list to reuse existing
		/// VBScreens. Handles also the VB, creation and rendering.
		/// </summary>
		private class VBScreen : IGraphicContent
		{
			#region Variables
			/// <summary>
			/// Vertex buffer to render stuff on screen.
			/// </summary>
			private VertexBuffer vbScreen;
			private VertexDeclaration decl;
			#endregion

			#region Constructor
			/// <summary>
			/// Create VB screen
			/// </summary>
			public VBScreen()
			{
				Load();
				BaseGame.RegisterGraphicContentObject(this);
			} // VBScreen()
			#endregion

			#region Load
			/// <summary>
			/// Load the font, also used for reloading.
			/// </summary>
			public void Load()
			{
				if (vbScreen != null)
					return;

				VertexPositionTexture[] vertices = new VertexPositionTexture[]
				{
					new VertexPositionTexture(
						new Vector3(-1.0f, -1.0f, 0.5f),
						new Vector2(0, 1)),
					new VertexPositionTexture(
						new Vector3(-1.0f, 1.0f, 0.5f),
						new Vector2(0, 0)),
					new VertexPositionTexture(
						new Vector3(1.0f, -1.0f, 0.5f),
						new Vector2(1, 1)),
					new VertexPositionTexture(
						new Vector3(1.0f, 1.0f, 0.5f),
						new Vector2(1, 0)),
				};
				vbScreen = new VertexBuffer(
					BaseGame.Device,
					typeof(VertexPositionTexture),
					vertices.Length,
					BufferUsage.WriteOnly);

				vbScreen.SetData(vertices);

				decl = new VertexDeclaration(
					BaseGame.Device, VertexPositionTexture.VertexElements);
			} // Load()
			#endregion

			#region Dispose
			/// <summary>
			/// Dispose
			/// </summary>
			public void Dispose()
			{
				if (vbScreen != null)
					vbScreen.Dispose();
				vbScreen = null;
			} // Dispose()
			#endregion

			#region Render
			/// <summary>
			/// Render
			/// </summary>
			public void Render()
			{
				// Rendering is pretty straight forward (if you know how anyway).
				BaseGame.Device.VertexDeclaration = decl;
				BaseGame.Device.Vertices[0].SetSource(vbScreen, 0,
					VertexPositionTexture.SizeInBytes);
				BaseGame.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			} // Render()
			#endregion
		} // class VBScreen
		#endregion

		#region Render
		/// <summary>
		/// Vb screen instance
		/// </summary>
		static VBScreen vbScreenInstance = null;
		/// <summary>
		/// Just render a vertex buffer with the screen coordinates.
		/// No subTexelSize stuff is performed, do that in the fx file.
		/// </summary>
		public static void Render()
		{
			if (vbScreenInstance == null)
				vbScreenInstance = new VBScreen();

			vbScreenInstance.Render();
		} // Render()
		#endregion

		#region GridScreen helper class
		/// <summary>
		/// Another vertex and index buffer for a screen grid, basically
		/// used for the same purpose as VBScreen, but allows us to create
		/// a grid (e.g. 10x10), very useful for advanced post screen shaders.
		/// </summary>
		private class GridScreen : IGraphicContent
		{
			#region Variables
			/// <summary>
			/// Grid dimension
			/// </summary>
			int gridWidth, gridHeight;
			/// <summary>
			/// Index buffer
			/// </summary>
			IndexBuffer indexBuffer = null;
			/// <summary>
			/// Vertex buffer
			/// </summary>
			VertexBuffer vertexBuffer = null;
			
			/// <summary>
			/// Vertex declaration
			/// </summary>
			VertexDeclaration decl;
			#endregion

			#region Constructor
			/// <summary>
			/// Create grid screen
			/// </summary>
			/// <param name="setGridDimension">Set grid dimension</param>
			public GridScreen(int setGridWidth, int setGridHeight)
			{
				if (setGridWidth < 2 ||
					setGridHeight < 2)
					throw new ArgumentException(
						"setGridWidth=" + setGridWidth + ", setGridHeight=" +setGridHeight,
						"Grid size must be at least (2, 2).");
				gridWidth = setGridWidth;
				gridHeight = setGridHeight;
				BaseGame.RegisterGraphicContentObject(this);
			} // GridScreen(setGridDimension)
			#endregion

			#region Load
			/// <summary>
			/// Load the font, also used for reloading.
			/// </summary>
			public void Load()
			{
				if (vertexBuffer != null)
					return;

				// Create vertex buffer
				vertexBuffer = new VertexBuffer(
					BaseGame.Device,
					typeof(VertexPositionTexture),
					gridWidth * gridHeight,
					BufferUsage.WriteOnly);

				// Create all vertices
				VertexPositionTexture[] vertices =
					new VertexPositionTexture[gridWidth * gridHeight];
				// Just simply create all vertices of the grid
				for (int x = 0; x < gridWidth; x++)
					for (int y = 0; y < gridHeight; y++)
					{
						vertices[x + y * gridWidth] =
							new VertexPositionTexture(new Vector3(
							-1.0f + 2.0f * (float)x / (float)(gridWidth - 1),
							-1.0f + 2.0f * (float)y / (float)(gridHeight - 1),
							0.5f),
							new Vector2((float)x / (float)(gridWidth - 1),
							// XNA expect bottom up for the screen rendering.
							1.0f - ((float)y / (float)(gridHeight - 1))));
					} // for for (int)
				vertexBuffer.SetData(vertices);

				// Index buffer
				indexBuffer = new IndexBuffer(
					BaseGame.Device,
					typeof(ushort),
					(gridWidth - 1) * (gridHeight - 1) * 2 * 3,
					BufferUsage.WriteOnly);
				
				ushort[] indices = new ushort[
					(gridWidth - 1) * (gridHeight - 1) * 3 * 2];
				// Just simply create all indices of the grid
				int num = 0;
				for (int x = 0; x < gridWidth - 1; x++)
					for (int y = 0; y < gridHeight - 1; y++)
					{
						ushort index1 = (ushort)(x + y * gridWidth);
						ushort index2 = (ushort)((x + 1) + y * gridWidth);
						ushort index3 = (ushort)((x + 1) + (y + 1) * gridWidth);
						ushort index4 = (ushort)(x + (y + 1) * gridWidth);

						indices[num] = index1;
						indices[num + 1] = index3;
						indices[num + 2] = index2;
						indices[num + 3] = index1;
						indices[num + 4] = index4;
						indices[num + 5] = index3;

						num += 6;
					} // for for (int)
				indexBuffer.SetData(indices);

				decl = new VertexDeclaration(
					BaseGame.Device, VertexPositionTexture.VertexElements);
			} // Load()
			#endregion

			#region Dispose
			/// <summary>
			/// Dispose
			/// </summary>
			public void Dispose()
			{
				if (indexBuffer != null)
					indexBuffer.Dispose();
				indexBuffer = null;
				if (vertexBuffer != null)
					vertexBuffer.Dispose();
				vertexBuffer = null;
			} // Dispose()
			#endregion

			#region Render
			/// <summary>
			/// Render
			/// </summary>
			public void Render()
			{
				// Rendering is pretty straight forward (if you know how anyway).
				BaseGame.Device.VertexDeclaration = decl;
				BaseGame.Device.Vertices[0].SetSource(vertexBuffer, 0,
					VertexPositionTexture.SizeInBytes);
				BaseGame.Device.Indices = indexBuffer;
				BaseGame.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
					0, 0, gridWidth * gridHeight,
					0, (gridWidth - 1) * (gridHeight - 1) * 2);
			} // Render()
			#endregion
		} // class GridScreen
		#endregion

		#region Render 10x10 screen grid
		/// <summary>
		/// Grid screen 1 0x 10 instance
		/// </summary>
		static GridScreen gridScreen10x10Instance = null;
		/// <summary>
		/// Just render a 10x10 grid with help of GridScreen on the screen.
		/// No subTexelSize stuff is performed, do that in the fx file.
		/// </summary>
		public static void Render10x10Grid()
		{
			if (gridScreen10x10Instance == null)
				gridScreen10x10Instance = new GridScreen(10, 10);

			gridScreen10x10Instance.Render();
		} // Render10x10Grid()
		#endregion
	} // class VBScreenHelper
} // namespace XnaShooter.Shaders
