// Project: XnaShooter, File: ShadowMapShader.cs
// Namespace: XnaShooter.Shaders, Class: ShadowMapShader
// Path: C:\code\XnaShooter\Shaders, Author: Abi
// Code lines: 655, Size of file: 19,49 KB
// Creation date: 07.10.2006 10:23
// Last modified: 14.10.2006 10:10
// Generated with Commenter by abi.exDream.com

#region Using directives
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using XnaShooter.Game;
using XnaShooter.Graphics;
using XnaShooter.GameScreens;
using Model = XnaShooter.Graphics.Model;
using Texture = XnaShooter.Graphics.Texture;
using XnaShooter.Helpers;
#endregion

namespace XnaShooter.Shaders
{
	/// <summary>
	/// Shadow map shader
	/// </summary>
	public class ShadowMapShader : ShaderEffect
	{
		#region Variables
		/// <summary>
		/// Shadow mapping shader filename
		/// </summary>
		const string ShaderFilename = "ShadowMap.fx";

		/// <summary>
		/// Shadow map texture we render to.
		/// </summary>
		internal RenderToTexture
			shadowMapTexture = null;

		/// <summary>
		/// Restrict near and far plane for much better depth resolution!
		/// </summary>
		const float
			//good for outdoor:
			//orthographicWidth = 70,
			//orthographicHeight = 100,
			shadowNearPlane = Mission.ViewDistance * 6.5f,
			shadowFarPlane = Mission.ViewDistance * 9.0f;

		/// <summary>
		/// Virtual point light parameters for directional shadow map lighting.
		/// Used to create a point light position for the directional light.
		/// </summary>
		const float
			virtualLightDistance = 8.0f * Mission.ViewDistance,
			virtualVisibleRange = 1.25f * Mission.ViewDistance;

		/// <summary>
		/// Shadow distance
		/// </summary>
		/// <returns>Float</returns>
		public float ShadowDistance
		{
			get
			{
				return virtualLightDistance;
			} // get
		} // ShadowDistance

		private Vector3 shadowLightPos = Vector3.Zero;

		/// <summary>
		/// Shadow light position
		/// </summary>
		/// <returns>Vector 3</returns>
		public Vector3 ShadowLightPos
		{
			get
			{
				return shadowLightPos;
			} // get
		} // ShadowLightPos

		/*see RenderToTexture!
		/// <summary>
		/// Shadow map width and height, by default 1024x1024,
		/// but 2048x2048 or better would also be great!
		/// 4096x4096 would be nice, but I guess that is slow.
		/// Used and set in RenderToTexture
		/// </summary>
		internal static int shadowMapWidth = 2048,//1024,
			shadowMapHeight = 2048;//1024;
		 */

		/// <summary>
		/// Texel width and height and offset for texScaleBiasMatrix,
		/// this way we can directly access the middle of each texel.
		/// </summary>
		float
			texelWidth = 1.0f / 1024.0f,
			texelHeight = 1.0f / 1024.0f,
			texOffsetX = 0.5f,
			texOffsetY = 0.5f;

		/// <summary>
		/// Factor for shadow map generation filter, makes shadow map more soft.
		/// </summary>
		const float TexelFilterFactor = 4.0f;//3.0f;//2.5f;

		/// <summary>
		/// Compare depth bias
		/// </summary>
		internal float compareDepthBias =
				0.00025f;
			//0.0025f;
			//0.0001f;//0.001f;//0.005f;//0.025f;//0.015f;

		/// <summary>
		/// Tex extra scale
		/// </summary>
		/// <returns>1.0f</returns>
		internal float texExtraScale = 1.0f;//1.0015f;//1.0075f;

		/// <summary>
		/// Shadow map depth bias value
		/// </summary>
		/// <returns>+</returns>
		internal float shadowMapDepthBiasValue =
				0.00025f;
			//0.0005f;
			//0.00025f;//+0.005f;

		/// <summary>
		/// The matrix to convert proj screen coordinates in the -1..1 range
		/// to the shadow depth map texture coordinates.
		/// </summary>
		Matrix texScaleBiasMatrix;

		/// <summary>
		/// Used matrices for the light casting the shadows.
		/// </summary>
		internal Matrix lightProjectionMatrix, lightViewMatrix;

		/// <summary>
		/// Additional xnaEffect handles
		/// </summary>
		private EffectParameter
			shadowTexTransform,
			worldViewProjLight,
			nearPlane,
			farPlane,
			depthBias,
			shadowMapDepthBias,
			shadowMap,
			shadowMapTexelSize,
			shadowDistanceFadeoutTexture;

		/// <summary>
		/// Shadow map blur post screen shader, used in RenderShadows
		/// to blur the shadow results.
		/// </summary>
		internal ShadowMapBlur shadowMapBlur = null;
		#endregion

		#region Calc shadow map bias matrix
		/// <summary>
		/// Calculate the texScaleBiasMatrix for converting proj screen
		/// coordinates in the -1..1 range to the shadow depth map
		/// texture coordinates.
		/// </summary>
		internal void CalcShadowMapBiasMatrix()
		{
			texelWidth = TexelFilterFactor * (1.0f / (float)shadowMapTexture.Width);
			texelHeight = TexelFilterFactor * (1.0f / (float)shadowMapTexture.Height);
			texOffsetX = 0.5f + (0.5f / (float)shadowMapTexture.Width);
			texOffsetY = 0.5f + (0.5f / (float)shadowMapTexture.Height);

			texScaleBiasMatrix = new Matrix(
				0.5f * texExtraScale, 0.0f, 0.0f, 0.0f,
				0.0f, -0.5f * texExtraScale, 0.0f, 0.0f,
				0.0f, 0.0f, texExtraScale, 0.0f,
				texOffsetX, texOffsetY, 0.0f, 1.0f);
		} // CalcShadowMapBiasMatrix()
		#endregion

		#region Properties
		/// <summary>
		/// Shadow map texture
		/// </summary>
		internal RenderToTexture ShadowMapTexture
		{
			get
			{
				return shadowMapTexture;
			} // get
		} // ShadowMapTexture (RenderToTexture)
		#endregion

		#region Constructors
		/// <summary>
		/// Shadow map shader
		/// </summary>
		public ShadowMapShader()
			: base(ShaderFilename)
		{
			if (BaseGame.CanUsePS20)
				// We use R32F, etc. and have a lot of precision
				compareDepthBias = 0.0001f;
			else
				// A8R8G8B8 isn't very precise, increase comparing depth bias
				compareDepthBias = 0.0075f;// 0.025f;

			Load();

			CalcShadowMapBiasMatrix();

			shadowMapBlur = new ShadowMapBlur();
		} // ShadowMapShader(tryToUsePS20, setBackFaceCheck, setShadowMapSize)
		#endregion

		#region Dispose
		/// <summary>
		/// Dispose
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();
			if (shadowMapTexture != null)
				shadowMapTexture.Dispose();
			shadowMapTexture = null;
		} // Dispose()
		#endregion

		#region Load
		/// <summary>
		/// Load in case the device got lost.
		/// </summary>
		public override void Load()
		{
			base.Load();
			// Ok, time to create the shadow map render target
			if (shadowMapTexture == null)
				shadowMapTexture = new RenderToTexture(
					RenderToTexture.SizeType.ShadowMap);
		} // Load()
		#endregion

		#region Get parameters
		/// <summary>
		/// Get parameters
		/// </summary>
		protected override void GetParameters()
		{
			// Can't get parameters if loading failed!
			if (xnaEffect == null)
				return;

			base.GetParameters();

			// Get additional parameters
			shadowTexTransform = xnaEffect.Parameters["shadowTexTransform"];
			worldViewProjLight = xnaEffect.Parameters["worldViewProjLight"];
			nearPlane = xnaEffect.Parameters["nearPlane"];
			farPlane = xnaEffect.Parameters["farPlane"];
			depthBias = xnaEffect.Parameters["depthBias"];
			shadowMapDepthBias = xnaEffect.Parameters["shadowMapDepthBias"];
			shadowMap = xnaEffect.Parameters["shadowMap"];
			shadowMapTexelSize = xnaEffect.Parameters["shadowMapTexelSize"];
			shadowDistanceFadeoutTexture =
				xnaEffect.Parameters["shadowDistanceFadeoutTexture"];
			// Load shadowDistanceFadeoutTexture
			if (shadowDistanceFadeoutTexture != null)
				shadowDistanceFadeoutTexture.SetValue(
					new Texture("ShadowDistanceFadeoutMap").XnaTexture);
		} // GetParameters()
		#endregion

		#region Update parameters
		/// <summary>
		/// Update parameters
		/// </summary>
		public override void SetParameters(Material setMat)
		{
			// Can't set parameters if loading failed!
			if (xnaEffect == null)
				return;

			/*already set, don't change!
			shadowNearPlane = 1.0f;
			shadowFarPlane = 28;
			virtualLightDistance = 24;
			virtualVisibleRange = 23.5f;
			 */
			if (BaseGame.CanUsePS20)
			{
				compareDepthBias = 0.00075f;//0.00025f;
				shadowMapDepthBiasValue = 0.00075f;//0.00025f;
			} // if (BaseGame.CanUsePS20)
			else
			{
				// Ps 1.1 isn't as percise!
				compareDepthBias = 0.0025f;
				shadowMapDepthBiasValue = 0.002f;
			} // else

			base.SetParameters(setMat);

			// Set all extra parameters for this shader
			depthBias.SetValue(compareDepthBias);
			shadowMapDepthBias.SetValue(shadowMapDepthBiasValue);
			shadowMapTexelSize.SetValue(
				new Vector2(texelWidth, texelHeight));
			nearPlane.SetValue(shadowNearPlane);
			farPlane.SetValue(shadowFarPlane);
		} // UpdateShadowParameters()
		#endregion

		#region Create simple directional shadow mapping matrix
		/// <summary>
		/// Calc simple directional shadow mapping matrix
		/// </summary>
		private void CalcSimpleDirectionalShadowMappingMatrix()
		{
			// Put light for directional mode away from origin (create virutal point
			// light). But adjust field of view to see enough of the visible area.
			// Note: Using light that is 10 times farther away to appear more ortogonal!
			float virtualFieldOfView = (float)Math.Atan2(
				virtualVisibleRange, virtualLightDistance);//*10);

			// Set projection matrix for light
			BaseGame.ProjectionMatrix = lightProjectionMatrix =
				Matrix.CreatePerspectiveFieldOfView(
				virtualFieldOfView,
				1.0f,
				shadowNearPlane,
				shadowFarPlane);// * 12);
				/*kills depth.z values
				Matrix.CreateOrthographic(
				orthographicWidth,
				orthographicHeight,
				shadowNearPlane,
				shadowFarPlane);
				/*obs
				Matrix.CreatePerspectiveFieldOfView(
				// Don't use graphics fov and aspect ratio in directional lighting mode
				MathHelper.Pi*0.1f,//virtualFieldOfView,
				1.0f,
				shadowNearPlane,
				shadowFarPlane);
				 */

			// Calc light look pos, put it a little bit in front of our car
			Vector3 shadowLookPos = Mission.LookAtPosition + new Vector3(-8, +6, 3);

			// Well, this is how directional lights are done:
			BaseGame.ViewMatrix = lightViewMatrix = Matrix.CreateLookAt(
				// Use our current car position for our light look at origin!
				shadowLookPos +
				BaseGame.LightDirection * virtualLightDistance,// * 10,//virtualLightDistance,
				shadowLookPos,
				new Vector3(0, 1, 0));

			// Update light pos
			Matrix invView = Matrix.Invert(lightViewMatrix);
			shadowLightPos = new Vector3(invView.M41, invView.M42, invView.M43);
		} // CalcSimpleDirectionalShadowMappingMatrix()
		#endregion

		#region Generate shadow
		/// <summary>
		/// Update shadow world matrix.
		/// Calling this function is important to keep the shaders
		/// WorldMatrix and WorldViewProjMatrix up to date.
		/// </summary>
		/// <param name="setWorldMatrix">World matrix</param>
		internal void UpdateGenerateShadowWorldMatrix(Matrix setWorldMatrix)
		{
			Matrix world = setWorldMatrix;
			WorldMatrix = world;
			WorldViewProjMatrix =
				world * lightViewMatrix * lightProjectionMatrix;
			xnaEffect.CommitChanges();
		} // UpdateGenerateShadowWorldMatrix(setWorldMatrix)

		/// <summary>
		/// Generate shadow
		/// </summary>
		internal void GenerateShadows(BaseGame.RenderDelegate renderObjects)
		{
			// Can't generate shadow if loading failed!
			if (xnaEffect == null)
				return;

			// This method sets all required shader variables.
			this.SetParameters();
			Matrix remViewMatrix = BaseGame.ViewMatrix;
			Matrix remProjMatrix = BaseGame.ProjectionMatrix;
			CalcSimpleDirectionalShadowMappingMatrix();
			
			DepthStencilBuffer remBackBufferSurface = null;
			// Time to generate the shadow texture
			try
			{
				// Start rendering onto the shadow map
				shadowMapTexture.SetRenderTarget();
				if (shadowMapTexture.ZBufferSurface != null)
				{
					remBackBufferSurface = BaseGame.Device.DepthStencilBuffer;
					BaseGame.Device.DepthStencilBuffer =
						shadowMapTexture.ZBufferSurface;
				} // if (shadowMapTexture.ZBufferSurface)

				// Make sure depth buffer is on
				BaseGame.Device.RenderState.DepthBufferEnable = true;
				// Disable alpha
				BaseGame.Device.RenderState.AlphaBlendEnable = false;

				// Clear render target
				shadowMapTexture.Clear(Color.White);

				//tst:
				//ShaderEffect.normalMapping.RenderSinglePassShader(renderObjects);

				if (BaseGame.CanUsePS20)
					xnaEffect.CurrentTechnique = xnaEffect.Techniques["GenerateShadowMap20"];
				else
					xnaEffect.CurrentTechnique = xnaEffect.Techniques["GenerateShadowMap"];

				// Render shadows with help of the GenerateShadowMap shader
				RenderSinglePassShader(renderObjects);

				/*
				xnaEffect.Begin(SaveStateMode.None);
				foreach (EffectPass pass in xnaEffect.CurrentTechnique.Passes)
				{
					pass.Begin();
					renderObjects();
					pass.End();
				} // foreach (pass)
				 */
			} // try
			finally
			{
				//xnaEffect.End();

				// Resolve the render target to get the texture (required for Xbox)
				shadowMapTexture.Resolve(false);

				// Set render target back to default
				//BaseGame.ResetRenderTarget(false);
				
				if (shadowMapTexture.ZBufferSurface != null)
					BaseGame.Device.DepthStencilBuffer = remBackBufferSurface;

				/*
#if DEBUG
				//tst: show light shadow view pos
				if (Input.Keyboard.IsKeyDown(Keys.Tab) == false)
#endif
				{
				 */
					BaseGame.ProjectionMatrix = remProjMatrix;
					BaseGame.ViewMatrix = remViewMatrix;
				//} // if
			} // finally
		} // GenerateShadows()
		#endregion

		#region Use shadow
		/// <summary>
		/// Update calc shadow world matrix, has to be done for each object
		/// we want to render in CalcShadows.
		/// </summary>
		/// <param name="setWorldMatrix">Set world matrix</param>
		internal void UpdateCalcShadowWorldMatrix(Matrix setWorldMatrix)
		{
			this.WorldMatrix = setWorldMatrix;
			this.WorldViewProjMatrix =
				setWorldMatrix * BaseGame.ViewMatrix * BaseGame.ProjectionMatrix;

			// Compute the matrix to transform from view space to light proj:
			// inverse of view matrix * light view matrix * light proj matrix
			Matrix lightTransformMatrix =
				setWorldMatrix *
				lightViewMatrix *
				lightProjectionMatrix *
				texScaleBiasMatrix;
			shadowTexTransform.SetValue(lightTransformMatrix);

			Matrix worldViewProjLightMatrix =
				setWorldMatrix *
				lightViewMatrix *
				lightProjectionMatrix;
			worldViewProjLight.SetValue(worldViewProjLightMatrix);

			xnaEffect.CommitChanges();

			/*tst
			ShaderEffect.normalMapping.SetParameters();
			ShaderEffect.normalMapping.WorldMatrix = setWorldMatrix;
			ShaderEffect.normalMapping.Update();
			 */
		} // UpdateCalcShadowWorldMatrix(setWorldMatrix)

		/// <summary>
		/// Calc shadows with help of generated light depth map,
		/// all objects have to be rendered again for comparing.
		/// We could save a pass when directly rendering here, but this
		/// has 2 disadvantages: 1. we can't post screen blur the shadow
		/// and 2. we can't use any other shader, especially bump and specular
		/// mapping shaders don't have any instructions left with ps_1_1.
		/// This way everything is kept simple, we can do as complex shaders
		/// as we want, the shadow shaders work seperately.
		/// </summary>
		/// <param name="renderObjects">Render objects</param>
		public void RenderShadows(BaseGame.RenderDelegate renderObjects)
		{
			// Can't calc shadows if loading failed!
			if (xnaEffect == null)
				return;

			// Make sure z buffer and writing z buffer is on
			BaseGame.Device.RenderState.DepthBufferEnable = true;
			BaseGame.Device.RenderState.DepthBufferWriteEnable = true;

			// Render shadows into our shadowMapBlur render target
			shadowMapBlur.RenderShadows(
				delegate
				{
					if (BaseGame.CanUsePS20)
						xnaEffect.CurrentTechnique = xnaEffect.Techniques["UseShadowMap20"];
					else
						xnaEffect.CurrentTechnique = xnaEffect.Techniques["UseShadowMap"];

					// This method sets all required shader variables.
					this.SetParameters();

					// Use the shadow map texture here which was generated in
					// GenerateShadows().
					shadowMap.SetValue(shadowMapTexture.XnaTexture);

					//ShaderEffect.normalMapping.RenderSinglePassShader(renderObjects);

					// Render shadows with help of the UseShadowMap shader
					RenderSinglePassShader(renderObjects);

					/*not required
					// Make sure warping is on, stupid xna bug again
					BaseGame.Device.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
					BaseGame.Device.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
					BaseGame.Device.SamplerStates[1].AddressU = TextureAddressMode.Wrap;
					BaseGame.Device.SamplerStates[1].AddressV = TextureAddressMode.Wrap;
					 */

					/*
					// Render everything into the current render target (set before
					// or just use the screen).
					try
					{
						xnaEffect.Begin(SaveStateMode.None);
						foreach (EffectPass pass in xnaEffect.CurrentTechnique.Passes)
						{
							pass.Begin();
							renderObjects();
							pass.End();
						} // foreach (pass)
					} // try
					finally
					{
						xnaEffect.End();
					} // finally
					 */
				});

			// Start rendering the shadow map blur (pass 1, which messes up our
			// background), pass 2 can be done below without any render targets.
			shadowMapBlur.ShowShadowsPart1();

			// Kill background z buffer
			XnaShooterGame.Device.Clear(ClearOptions.DepthBuffer, Color.Black, 1, 0);
		} // RenderShadows(renderObjects)
		#endregion

		#region ShowShadows
		/// <summary>
		/// Show Shadows
		/// </summary>
		public void ShowShadows()
		{
			shadowMapBlur.ShowShadowsPart2();
		} // ShowShadows()
		#endregion

		#region Unit Testing
#if DEBUG
		/// <summary>
		/// Test shadow mapping
		/// </summary>
		public static void TestShadowMapping()
		{
			Model testModel = null;
			Model testGround = null;

			TestGame.Start("TestShadowMapping",
				delegate
				{
					testModel = new Model("Building");
					testGround = new Model("BackgroundGround");
				},
				delegate
				{
					if (Input.Keyboard.IsKeyDown(Keys.Tab))
						BaseGame.ViewMatrix = Matrix.CreateLookAt(
							Mission.LookAtPosition +
							BaseGame.LightDirection * Mission.ViewDistance,
							Mission.LookAtPosition,
							new Vector3(0, 1, 0));

					Matrix renderMatrix = Matrix.CreateScale(15) *
						Matrix.CreateRotationZ(0.85f) *
						Matrix.CreateTranslation(0, 0, -16);
					Matrix groundMatrix = Matrix.CreateScale(100);

					if (Input.Keyboard.IsKeyUp(Keys.LeftAlt) &&
						Input.GamePadXPressed == false)
					{
						// Generate shadows
						ShaderEffect.shadowMapping.GenerateShadows(
							delegate
							{
								testModel.GenerateShadow(renderMatrix);
							});

						// Render shadows
						ShaderEffect.shadowMapping.RenderShadows(
							delegate
							{
								testModel.UseShadow(renderMatrix);
								testGround.UseShadow(groundMatrix);
							});
					} // if

					testModel.Render(renderMatrix);
					testGround.Render(groundMatrix);
					BaseGame.MeshRenderManager.Render();

					if (Input.Keyboard.IsKeyUp(Keys.LeftAlt) &&
						Input.GamePadXPressed == false)
					{
						ShaderEffect.shadowMapping.ShowShadows();
					} // if

					if (Input.Keyboard.IsKeyDown(Keys.LeftShift) ||
						Input.GamePadAPressed)
					{
						//if (Input.KeyboardRightPressed)// == false)
						ShaderEffect.shadowMapping.shadowMapTexture.RenderOnScreen(
							new Rectangle(10, 10, 256, 256));
						//if (Input.KeyboardLeftPressed)// == false)
						ShaderEffect.shadowMapping.shadowMapBlur.SceneMapTexture.
							RenderOnScreen(
							new Rectangle(10 + 256 + 10, 10, 256, 256));
						// Unused on xbox360
//#if !XBOX360
						//ShaderEffect.shadowMapping.shadowMapBlur.BlurMapTexture.
						//	RenderOnScreen(
						//	new Rectangle(10 + 256 + 10+256+10, 10, 256, 256));
//#endif
					} // if (Input.Keyboard.IsKeyDown)

					//tst:
					//XnaShooterGame.GlowShader.Show();

					TextureFont.WriteText(2, 510,
						"Press left Shift or A to show all shadow pass textures.");
					TextureFont.WriteText(2, 540,
						"Press Alt or X to skip shadow map rendering.");
					TextureFont.WriteText(2, 570,
						"Camera pos="+BaseGame.CameraPos);//+
						//", "+ShaderEffect.shadowMapping.shadowMapTexture.ZBufferSurface);
				});
		} // TestShadowMapping()
#endif
		#endregion
	} // class ShadowMapShader
} // namespace Abi.Graphic.ShadowMapping
