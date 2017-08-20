#if UNITY_EDITOR
	using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MobileShadow : MonoBehaviour
{
	public enum Dimension
	{
		x256 = 256,
		x512 = 512,
		x1024 = 1024,
		x2048 = 2048,
	}

	public Light SunLight;

	[Space] public Dimension TextureSize = Dimension.x512;

	public LayerMask CastShadowLayers;
	public Texture2D ShadowFadeTexture;
	[Space] public float ShadowDistance = 25.0f;
    [Range(0, 1)]
    public float NearPlaneOffset = 0.9f;

    [Space]
    [Range(0, 1)] public float ShadowOpacity = 0.7f;

	private Material _shadowEdgeFadeMaterial;

	private float _focusRadius;

	private int _shadowMatrixID;
    private int _shadowColorID;
    private int _shadowTextureID;
	private int _shadowBlurID;
	private int _shadowOpacityID;
	private int _sunPositionID;

	private Camera _viewCamera;
	private GameObject _shadowCameraGO;
	private Transform _sunTransform;
	private Camera _shadowCamera;
	private RenderTexture _shadowTexture;
	private Matrix4x4 _shadowMatrix;

	private Vector3[] _frustrumCorners = new Vector3[8];

	[Space] public bool SoftShadow;
	public float BlurSize = 1.0f;

    [Space]
    public bool TintShadow;
    public Color ShadowColor;

    [Space] public bool ShowDebug;

	private readonly Matrix4x4 _shadowSpaceMatrix = new Matrix4x4()
	{
		m00 = 0.5f,
		m01 = 0.0f,
		m02 = 0.0f,
		m03 = 0.5f,
		m10 = 0.0f,
		m11 = 0.5f,
		m12 = 0.0f,
		m13 = 0.5f,
		m20 = 0.0f,
		m21 = 0.0f,
		m22 = 0.5f,
		m23 = 0.5f,
		m30 = 0.0f,
		m31 = 0.0f,
		m32 = 0.0f,
		m33 = 1.0f,
	};

	void Awake()
	{
		_viewCamera = GetComponent<Camera>();

		_shadowMatrixID = Shader.PropertyToID("_MobileShadowMatrix");
		_shadowTextureID = Shader.PropertyToID("_MobileShadowTexture");
		_shadowBlurID = Shader.PropertyToID("_MobileShadowBlur");
		_shadowOpacityID = Shader.PropertyToID("_MobileShadowOpacity");
		_sunPositionID = Shader.PropertyToID("_MobileShadowSunPosition");
	    _shadowColorID = Shader.PropertyToID("_MobileShadowColor");
        _shadowMatrix = Matrix4x4.identity;

		Init();
	}

	void Init()
	{
		if (SunLight == null) return;

		_sunTransform = SunLight.transform;

		if (_shadowEdgeFadeMaterial == null)
		{
			_shadowEdgeFadeMaterial = new Material(Shader.Find("Hidden/MobileShadow"));
			_shadowEdgeFadeMaterial.hideFlags = HideFlags.HideAndDontSave;
			_shadowEdgeFadeMaterial.SetTexture("_FadeTex", ShadowFadeTexture);
		}

		if (_shadowCameraGO == null)
		{
			_shadowCameraGO = new GameObject("_Shadow Camera");

			if (ShowDebug)
				_shadowCameraGO.hideFlags = HideFlags.DontSave;
			else
			{
				_shadowCameraGO.hideFlags = HideFlags.HideAndDontSave;
			}

			_shadowCamera = _shadowCameraGO.AddComponent<Camera>();
			_shadowCamera.renderingPath = RenderingPath.VertexLit;
			_shadowCamera.clearFlags = CameraClearFlags.Color;
			_shadowCamera.backgroundColor = Color.black;
			_shadowCamera.depthTextureMode = DepthTextureMode.None;
			_shadowCamera.useOcclusionCulling = false;
			_shadowCamera.orthographic = true;
			_shadowCamera.depth = -100;
			_shadowCamera.aspect = 1f;
			_shadowCamera.cullingMask = CastShadowLayers;
			_shadowCamera.SetReplacementShader(Shader.Find("Hidden/MobileShadowReplacementShader"), "MobileShadow");
			_shadowCamera.enabled = false;

			AllocateTexture();
			_shadowCamera.targetTexture = _shadowTexture;
		}
	}

	void OnPreRender()
	{
		if (_sunTransform == null)
		{
			Init();
			return;
		}

		UpdateFocus();

		if ((int) TextureSize != _shadowTexture.width)
		{
			AllocateNewTexture();
		}

		Shader.SetGlobalFloat(_shadowOpacityID, ShadowOpacity);
		Shader.SetGlobalVector(_sunPositionID, _sunTransform.forward);
		_shadowCamera.cullingMask = CastShadowLayers;

		RenderTexture render = RenderTexture.GetTemporary(_shadowTexture.width, _shadowTexture.height, 16);
		_shadowCamera.targetTexture = render;
		
		_shadowCamera.Render();
		
		if (SoftShadow)
		{
			_shadowEdgeFadeMaterial.SetFloat(_shadowBlurID, BlurSize);
			RenderTexture bl1 = RenderTexture.GetTemporary(_shadowTexture.width, _shadowTexture.height, 16);
			Graphics.Blit(render, bl1, _shadowEdgeFadeMaterial, 0);
			_shadowCamera.targetTexture = null;
			RenderTexture.ReleaseTemporary(render);
			
			RenderTexture bl2 = RenderTexture.GetTemporary(_shadowTexture.width, _shadowTexture.height, 16);
			Graphics.Blit(bl1, bl2, _shadowEdgeFadeMaterial, 1);
			RenderTexture.ReleaseTemporary(bl1);
			
			_shadowTexture.DiscardContents();
			Graphics.Blit(bl2, _shadowTexture, _shadowEdgeFadeMaterial, 2);
			RenderTexture.ReleaseTemporary(bl2);
		}
		else
		{
			_shadowTexture.DiscardContents();
			Graphics.Blit(render, _shadowTexture, _shadowEdgeFadeMaterial, 2);
			_shadowCamera.targetTexture = null;
			RenderTexture.ReleaseTemporary(render);
		}

	    if (TintShadow)
	    {
	        Shader.EnableKeyword("SHADOWTINT");
            Shader.SetGlobalColor(_shadowColorID, ShadowColor);
	    }
	    else
	    {
            Shader.DisableKeyword("SHADOWTINT");
        }

		Shader.SetGlobalTexture(_shadowTextureID, _shadowTexture);
		Shader.SetGlobalMatrix(_shadowMatrixID, _shadowMatrix);
	}

	void OnEnable()
	{
		if (_shadowCameraGO == null || _shadowCamera == null)
		{
			Init();
		}
	}

	void OnDisable()
	{
		ReleaseTarget();
	}

	void AllocateTexture()
	{
		_shadowTexture = new RenderTexture((int) TextureSize, (int) TextureSize, 16);
		_shadowTexture.filterMode = FilterMode.Bilinear;
		_shadowTexture.useMipMap = false;
		_shadowTexture.autoGenerateMips = false;
	}

	void AllocateNewTexture()
	{
		_shadowCamera.targetTexture = null;
		DestroyImmediate(_shadowTexture);

		_shadowTexture = new RenderTexture((int) TextureSize, (int) TextureSize, 16);
		_shadowTexture.filterMode = FilterMode.Bilinear;
		_shadowTexture.useMipMap = false;
		_shadowTexture.autoGenerateMips = false;
		_shadowCamera.targetTexture = _shadowTexture;
	}

	void ReleaseTarget()
	{
		if (_shadowCamera != null)
			_shadowCamera.targetTexture = null;
		DestroyImmediate(_shadowCamera);
		DestroyImmediate(_shadowCameraGO);
		DestroyImmediate(_shadowEdgeFadeMaterial);
		DestroyImmediate(_shadowTexture);
		_shadowTexture = null;
	}

	void UpdateFocus()
	{
		var fC = FindFrustrumCenter();

		var eye = fC - _sunTransform.forward * _focusRadius * 2.0f;
		_shadowCamera.transform.position = eye;
		_shadowCamera.transform.LookAt(fC);

		var shadowViewMat = _shadowCamera.worldToCameraMatrix;
		_shadowCamera.orthographicSize = _focusRadius * 2.0f;
		_shadowCamera.nearClipPlane = 0;
		_shadowCamera.farClipPlane = _focusRadius * 6.0f;
		var shadowProjection = Matrix4x4.Ortho(-_focusRadius, _focusRadius, -_focusRadius, _focusRadius, 0,
			_focusRadius * 6.0f);
		_shadowCamera.projectionMatrix = shadowProjection;

		_shadowMatrix = _shadowSpaceMatrix * shadowProjection * shadowViewMat;
	}

	Vector3 FindFrustrumCenter()
	{
		FindFrustrumCorners(ShadowDistance * NearPlaneOffset, ShadowDistance);

		for (int i = 0; i < _frustrumCorners.Length; i++)
		{
			_frustrumCorners[i] = _viewCamera.ViewportToWorldPoint(_frustrumCorners[i]);
		}

		Vector3 frustrumCenter = Vector3.zero;
		for (int i = 0; i < 8; i++)
		{
			frustrumCenter = frustrumCenter + _frustrumCorners[i];
		}
		frustrumCenter = frustrumCenter / 8.0f;

		_focusRadius = (_frustrumCorners[0] - _frustrumCorners[6]).magnitude / 2.0f;

		var texelsPerUnit = (float) TextureSize / (_focusRadius * 2.0f);
		var scalar = Matrix4x4.Scale(new Vector3(texelsPerUnit, texelsPerUnit, texelsPerUnit));

		var sunRotation = _sunTransform.rotation.eulerAngles;

		var sunMatrix = Matrix4x4.TRS(_sunTransform.position,
			Quaternion.Euler(_sunTransform.rotation.eulerAngles.x, _sunTransform.rotation.eulerAngles.y, 0),
			_sunTransform.localScale);

		frustrumCenter = sunMatrix.inverse.MultiplyPoint3x4(frustrumCenter);
		frustrumCenter = scalar.MultiplyPoint3x4(frustrumCenter);
		frustrumCenter.x = Mathf.Floor(frustrumCenter.x);
		frustrumCenter.y = Mathf.Floor(frustrumCenter.y);
		frustrumCenter = scalar.inverse.MultiplyPoint3x4(frustrumCenter);
		frustrumCenter = sunMatrix.MultiplyPoint3x4(frustrumCenter);
		return frustrumCenter;
	}

	void FindFrustrumCorners(float nearDistance, float farDistance)
	{
		_frustrumCorners[0].x = 0;
		_frustrumCorners[0].y = 1;
		_frustrumCorners[0].z = nearDistance;

		_frustrumCorners[1].x = 1;
		_frustrumCorners[1].y = 1;
		_frustrumCorners[1].z = nearDistance;

		_frustrumCorners[2].x = 1;
		_frustrumCorners[2].y = 0;
		_frustrumCorners[2].z = nearDistance;

		_frustrumCorners[3].x = 0;
		_frustrumCorners[3].y = 0;
		_frustrumCorners[3].z = nearDistance;

		_frustrumCorners[4].x = 0;
		_frustrumCorners[4].y = 1;
		_frustrumCorners[4].z = farDistance;

		_frustrumCorners[5].x = 1;
		_frustrumCorners[5].y = 1;
		_frustrumCorners[5].z = farDistance;

		_frustrumCorners[6].x = 1;
		_frustrumCorners[6].y = 0;
		_frustrumCorners[6].z = farDistance;

		_frustrumCorners[7].x = 0;
		_frustrumCorners[7].y = 0;
		_frustrumCorners[7].z = farDistance;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (_sunTransform != null)
			Shader.SetGlobalVector(_sunPositionID, _sunTransform.forward);

		if (_shadowEdgeFadeMaterial != null)
		{
			_shadowEdgeFadeMaterial.SetTexture("_FadeTex", ShadowFadeTexture);
		}

		if (_shadowCameraGO != null)
		{
			if (ShowDebug)
			{
				_shadowCameraGO.hideFlags = HideFlags.DontSave;
			}
			else
			{
				_shadowCameraGO.hideFlags = HideFlags.HideAndDontSave;
				EditorApplication.DirtyHierarchyWindowSorting();
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (!ShowDebug) return;

		FindFrustrumCorners(ShadowDistance * NearPlaneOffset, ShadowDistance);
		for (int i = 0; i < _frustrumCorners.Length; i++)
		{
			_frustrumCorners[i] = GetComponent<Camera>().ViewportToWorldPoint(_frustrumCorners[i]);
		}

		Vector3 frustrumCenter = Vector3.zero;
		for (int i = 0; i < 8; i++)
		{
			frustrumCenter = frustrumCenter + _frustrumCorners[i];
		}
		frustrumCenter = frustrumCenter / 8.0f;

		_focusRadius = (_frustrumCorners[0] - _frustrumCorners[6]).magnitude / 2.0f;

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(frustrumCenter, _focusRadius);


	}
#endif

}
