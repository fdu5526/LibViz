using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------


namespace RenderHeads.Media.AVProVideo
{
	/// <summary>
	/// Draws video over the whole background using the special "background" tag on the shader.
	/// Useful for augmented reality.
	/// NOTE: This doesn't work with the camera clear mode set to 'skybox'
	/// </summary>
	[AddComponentMenu("AVPro Video/Display Background")]
	[ExecuteInEditMode]
	public class DisplayBackground : MonoBehaviour
	{
		public IMediaProducer _source;

		public Texture2D _texture;
		public Material _material;
		
		//-------------------------------------------------------------------------

		public void OnRenderObject()
		{
			if (_material == null || _texture == null)
				return;
			
			Vector4 uv = new Vector4(0f, 0f, 1f, 1f);
			_material.SetPass(0);
			UnityEngine.GL.PushMatrix();
			UnityEngine.GL.LoadOrtho();
			UnityEngine.GL.Begin(UnityEngine.GL.QUADS);
			
			UnityEngine.GL.TexCoord2(uv.x, uv.y);
			UnityEngine.GL.Vertex3(0.0f, 0.0f, 0.1f);
			
			UnityEngine.GL.TexCoord2(uv.z, uv.y);
			UnityEngine.GL.Vertex3(1.0f, 0.0f, 0.1f);
			
			UnityEngine.GL.TexCoord2(uv.z, uv.w);		
			UnityEngine.GL.Vertex3(1.0f, 1.0f, 0.1f);
			
			UnityEngine.GL.TexCoord2(uv.x, uv.w);
			UnityEngine.GL.Vertex3(0.0f, 1.0f, 0.1f);
			
			UnityEngine.GL.End();
			UnityEngine.GL.PopMatrix();
		}
	}
}