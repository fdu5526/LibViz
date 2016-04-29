using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012-2015 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro QuickTime/Manager (required)")]
public class AVProQuickTimeManager : MonoBehaviour
{
	private static AVProQuickTimeManager _instance;

	// Format conversion
	public Shader _shaderBGRA;
	public Shader _shaderYUV2;
	public Shader _shaderYUV2_709;
	public Shader _shaderCopy;
	public Shader _shaderHap_YCoCg;

	private bool _isInitialised;
	
	//-----------------------------------------------------------------------------
	
	public static AVProQuickTimeManager Instance  
	{
		get
		{
			if (_instance == null)
			{
				_instance = (AVProQuickTimeManager)GameObject.FindObjectOfType(typeof(AVProQuickTimeManager));
				if (_instance == null)
				{
					Debug.LogError("AVProQuickTimeManager component required");
					return null;
				}
				else
				{
					if (!_instance._isInitialised)
						_instance.Init();
				}
			}

			return _instance;
		}
	}
	
	//-------------------------------------------------------------------------
	
	void Awake()
	{
		if (!_isInitialised)
		{
			_instance = this;
			Init();
		}
	}
	
	void OnDestroy()
	{
		Deinit();
	}
	
	protected bool Init()
	{
		try
		{
			if (AVProQuickTimePlugin.Init())
			{
				Debug.Log("[AVProQuickTime] version " + AVProQuickTimePlugin.GetPluginVersion().ToString("F2") + " initialised");
			}
			else
			{
				Debug.LogError("[AVProQuickTime] failed to initialise.");
				this.enabled = false;
				Deinit();
				return false;
			}
		}
		catch (DllNotFoundException e)
		{
			Debug.LogError("[AVProQuickTime] Unity couldn't find the DLL.  Please move the 'Plugins' folder to the root of your project, and then restart Unity.");
			Debug.LogException(e);
			return false;
		}
		
		GetConversionMethod();
		SetUnityFeatures();

		//StartCoroutine("FinalRenderCapture");

		_isInitialised = true;

		return _isInitialised;
	}

	private void GetConversionMethod()
	{
		bool swapRedBlue = false;

        if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 11"))
        {
#if UNITY_5_0
			// DX11 has red and blue channels swapped around
			if (!SystemInfo.SupportsTextureFormat(TextureFormat.BGRA32))
				swapRedBlue = true;
#else
            swapRedBlue = true;
#endif
        }

		if (swapRedBlue)
		{
			Shader.DisableKeyword("SWAP_RED_BLUE_OFF");
			Shader.EnableKeyword("SWAP_RED_BLUE_ON");
		}
		else
		{
			Shader.DisableKeyword("SWAP_RED_BLUE_ON");
			Shader.EnableKeyword("SWAP_RED_BLUE_OFF");
		}

        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
        {
            Shader.DisableKeyword("AVPRO_GAMMACORRECTION_OFF");
            Shader.EnableKeyword("AVPRO_GAMMACORRECTION");
        }
        else
        {
            Shader.DisableKeyword("AVPRO_GAMMACORRECTION");
            Shader.EnableKeyword("AVPRO_GAMMACORRECTION_OFF");
        }
	}

	private void SetUnityFeatures()
	{
		AVProQuickTimePlugin.SetUnityFeatures(false);
	}

	void Update()
	{
		UnityEngine.GL.IssuePluginEvent(AVProQuickTimePlugin.PluginID | (int)AVProQuickTimePlugin.PluginEvent.UpdateAllTextures);
	}

	private IEnumerator FinalRenderCapture()
	{
		while (Application.isPlaying)
		{
			yield return new WaitForEndOfFrame();
			
			UnityEngine.GL.IssuePluginEvent(AVProQuickTimePlugin.PluginID | (int)AVProQuickTimePlugin.PluginEvent.UpdateAllTextures);
		}
	}

	public void Deinit()
	{
		// Clean up any open movies
		AVProQuickTimeMovie[] movies = (AVProQuickTimeMovie[])FindObjectsOfType(typeof(AVProQuickTimeMovie));
		if (movies != null && movies.Length > 0)
		{
			for (int i = 0; i < movies.Length; i++)
			{
				movies[i].UnloadMovie();
			}
		}

		_instance = null;
		_isInitialised = false;
		
		AVProQuickTimePlugin.Deinit();
	}
	
	public Shader GetPixelConversionShader(AVProQuickTimePlugin.PixelFormat format, bool yuvHD)
	{
		Shader result = null;
		switch (format)
		{
		case AVProQuickTimePlugin.PixelFormat.RGBA32:
			result = _shaderBGRA;
			break;
		case AVProQuickTimePlugin.PixelFormat.YCbCr:
			result = _shaderYUV2;
			if (yuvHD)
				result = _shaderYUV2_709;
			break;
		case AVProQuickTimePlugin.PixelFormat.Hap_RGB:
			result = _shaderCopy;
			break;
		case AVProQuickTimePlugin.PixelFormat.Hap_RGBA:
			result = _shaderCopy;
			break;
		case AVProQuickTimePlugin.PixelFormat.Hap_RGB_HQ:
			result = _shaderHap_YCoCg;
			break;
		default:
			Debug.LogError("[AVProQuickTime] Unknown video format '" + format);
			break;
		}
		return result;
	}
}