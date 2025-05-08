using System;
using System.Collections.Generic;
using DG.Tweening;
using PaintCore;
using PaintIn3D;
using PowerWash.Dirt;
using PowerWash.Scripts.Data;
using PowerWash.Scripts.PowerWash.Nozzle;
using Steamworks.PowerWash.Scripts.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace PowerWash.Scripts.PowerWash.Dirts
{
	[DisallowMultipleComponent]
	public class Dirt : MonoBehaviour
	{
		private static readonly int s_emissionColor = Shader.PropertyToID("_EmissionColor");
		private const string DirtPngPath = "PowerWashResource/Dirt";
		private const string DirtMaterialPath = "PowerWashResource/Cyber_Chassis";
		public event Action OnCleaned;
		public event Action<Dirt> OnCleanedDirt;

		public bool IsCleaned { get; private set; }
		public bool HasLoadedProgress { get; private set; }
		public bool DisplayingHint { get; private set; }
		
		public float InitialCleanValue
		{
			get => _initialCleanValue;
			private set => _initialCleanValue = value;
		}
		
		public CwPaintableTexture PaintableTexture { get; private set; }
		public float InitialLoadedCleanValue { get; private set; }

		public WetSurface HintObject { get; set; }
		
		[field: FormerlySerializedAs("_channelCounter")]
		[field: Header("External References")]
		[field: SerializeField] public UniqueId UniqueId { get; private set; }

		[field: SerializeField] public CwChannelCounter ChannelCounter { get; private set; }

		[field: Header("Settings")]
		[field: SerializeField] public DirtType DirtType { get; private set; } = DirtType.Fresh;

		[SerializeField, Range(0, 100)] private float _stopPercent = 10f;
		[field: SerializeField] public bool ShouldDestroyCopyAfterCleaning { get; set; }
		[SerializeField] private Color _hintColor = Color.cyan;
		[SerializeField] private Color _finishNotificationColor = Color.white;
		[SerializeField] private float _defaultScaleMultiplier;
		
		[Header("Mesh Copy Settings")]
		[SerializeField,
		 Tooltip(
			 "If marked, it will create a copy of current object's mesh, it will put it as a child object, and initialize it. This way, make sure that the component has 'Dirt' component attached to it")]
		private bool _shouldCreateMeshCopy;

		[Space(10)]
		[SerializeField] private Material _customMaterial;
		[SerializeField] private Texture2D _dirtTexture;
		[SerializeField] private LayerMask _dirtLayer;
		[SerializeField] private int _paintableMeshTextureSlotIndex;
		[SerializeField] private string _paintableMeshTextureName = "_MainTex";
		
		[SerializeField] private bool _scaleX;
		[SerializeField] private bool _scaleY;
		[SerializeField] private bool _scaleZ;

		[Space(10)]
		[SerializeField] private Vector3 _meshCopyOffset;
		
		private float _initialCleanValue;
		private bool _isInitialized;

		private GameObject _meshCopyGameObject;
		private readonly Color _transparentColor = new Color(0f, 0f, 0f, 0f);
		private SaveLoadService _saveLoadService;

		private void OnValidate()
		{
			if (string.IsNullOrEmpty(_paintableMeshTextureName))
				_paintableMeshTextureName = "_MainTex";

			if (!TryGetComponent(out UniqueId _))
			{
				UniqueId = gameObject.AddComponent<UniqueId>();
				print(name);
			}
		}

		private void Awake()
		{
			PaintableTexture = GetComponent<CwPaintableTexture>();
			TryCreateMeshCopy();
		}

		private void Start()
		{
			if (ChannelCounter == null) return;

			ChannelCounter.OnUpdated += OnChannelCounterUpdated;
			ChannelCounter.OnCountAChanged += OnCountAChanged;

			_isInitialized = false;
			_saveLoadService = SaveLoadService.Instance;
		}

		private void OnDestroy()
		{
			if (ChannelCounter == null) return;

			ChannelCounter.OnUpdated -= OnChannelCounterUpdated;
			ChannelCounter.OnCountAChanged -= OnCountAChanged;
		}

		public void MakeSimpleSurface(CwChannelCounter cwChannelCounter)
		{
			_dirtLayer = 1 << LayerMask.NameToLayer("Dirt");

			Texture2D dirt = Resources.Load<Texture2D>(DirtPngPath);
			CwPaintableMeshTexture cwPaintableMeshTexture = GetComponent<CwPaintableMeshTexture>();
			GetComponent<MeshRenderer>().material = Resources.Load<Material>(DirtMaterialPath);
			cwPaintableMeshTexture.Texture = dirt;
			ChannelCounter = cwChannelCounter;
			cwChannelCounter.PaintableTexture = cwPaintableMeshTexture;
		}

		public void DestroyMeshCopy()
		{
			if (_meshCopyGameObject)
				Destroy(_meshCopyGameObject);
		}

		public void MakePaintable()
		{
			_shouldCreateMeshCopy = true;
			_dirtLayer = 1 << LayerMask.NameToLayer("Dirt");
			_customMaterial = Resources.Load<Material>(DirtMaterialPath);
			_dirtTexture = Resources.Load<Texture2D>(DirtPngPath);
		}

		public void UpdateProgress(PlayerProgress playerProgress)
		{
			print("Saved single dirt progress");
			playerProgress.SetClean(this);
		}

		public void LoadProgress(PlayerProgress playerProgress)
		{
			HasLoadedProgress = true;
			
			if (!playerProgress.HasCleanedDirt(this))
				return;

			IsCleaned = true;
			ChannelCounter.CountA = 0;

			if (!_shouldCreateMeshCopy)
			{
				Destroy(ChannelCounter);
				gameObject.SetActive(false);
			}
				
			InitialLoadedCleanValue = playerProgress.GetInitialDirtAlpha(this);
		}

		public void PerformHintBlink()
		{
			if (DisplayingHint) return;

			DisplayingHint = true;
			Material material = HintObject != null
				? HintObject.gameObject.GetComponent<Renderer>().material
				: GetComponent<Renderer>().material;
			
			material.ToggleEmission(true);
			DOTween.Sequence()
				.Append(material.DOColor(_hintColor, s_emissionColor, 0.55f))
				.Append(material.DOColor(Color.black, s_emissionColor, 0.55f))
				.SetEase(Ease.InSine)
				.OnComplete(() =>
				{
					material.SetColor(s_emissionColor, Color.black);
					DisplayingHint = false;
				});
		}
		
		private void OnChannelCounterUpdated()
		{
			if (_isInitialized) return;
			_initialCleanValue = ChannelCounter.CountA;
			_isInitialized = true;
		}

		private void OnCountAChanged(int countAValue)
		{
			if (_initialCleanValue == 0) return;

			float dirtyPercent = countAValue / _initialCleanValue * 100f;

			if (IsCleaned || !( dirtyPercent <= _stopPercent )) return;

			ChannelCounter.OnCountAChanged -= OnCountAChanged;
			ChannelCounter.CountA = 0;
			IsCleaned = true;
			
			OnCleaned?.Invoke();
			OnCleanedDirt?.Invoke(this);
			
			if (_shouldCreateMeshCopy)
			{
				Material material = GetComponent<Renderer>().material;
				material.ToggleEmission(true);
				DOTween.Sequence()
					.Append(material.DOColor(_finishNotificationColor, s_emissionColor, 0.55f))
					.Append(material.DOColor(Color.black, s_emissionColor, 0.55f))
					.SetEase(Ease.InSine)
					.OnComplete(() => material.SetColor(s_emissionColor, Color.black));
			}

			if (ShouldDestroyCopyAfterCleaning)
			{
				if (_meshCopyGameObject)
					Destroy(_meshCopyGameObject);

				Resources.UnloadUnusedAssets();
				return;
			}

			if (_meshCopyGameObject)
			{
				_meshCopyGameObject.GetComponent<CwPaintableMesh>().ClearAll(_transparentColor);
				return;
			}

			GetComponent<CwPaintableMesh>().ClearAll(_transparentColor);
		}

		private void TryCreateMeshCopy()
		{
			if (!_shouldCreateMeshCopy)
			{
				print("SimpleDirt initialized");
				return;
			}

			MeshFilter originalMeshFilter = GetComponent<MeshFilter>();
			MeshRenderer originalMeshRenderer = GetComponent<MeshRenderer>();

			if (originalMeshFilter == null || originalMeshRenderer == null)
			{
				Debug.LogError("MeshFilter or MeshRenderer is missing!", this);
				return;
			}

			GameObject childObject = new GameObject("MeshCopy");
			childObject.transform.SetParent(transform);
			childObject.transform.localPosition = Vector3.zero + _meshCopyOffset;
			childObject.transform.localRotation = Quaternion.identity;

			// Modified scaling logic
			Vector3 scale;
			if (_scaleX || _scaleY || _scaleZ) // If any toggle is true, scale selectively
			{
				scale = Vector3.one; // Start with default scale (1, 1, 1)
				if (_scaleX) scale.x += _defaultScaleMultiplier;
				if (_scaleY) scale.y += _defaultScaleMultiplier;
				if (_scaleZ) scale.z += _defaultScaleMultiplier;
			}
			else // If no toggles are true, scale all axes uniformly
			{
				scale = Vector3.one + Vector3.one * _defaultScaleMultiplier;
			}
			childObject.transform.localScale = scale;

			int layerIndex = (int) Mathf.Log(_dirtLayer.value, 2);
			childObject.layer = layerIndex;

			MeshFilter childMeshFilter = childObject.AddComponent<MeshFilter>();
			childMeshFilter.sharedMesh = originalMeshFilter.sharedMesh;

			MeshRenderer childMeshRenderer = childObject.AddComponent<MeshRenderer>();
			childMeshRenderer.material = _customMaterial;

			childObject.AddComponent<CwPaintableMesh>();

			CwPaintableMeshTexture paintableMeshTexture = childObject.AddComponent<CwPaintableMeshTexture>();
			paintableMeshTexture.Slot = new CwSlot(_paintableMeshTextureSlotIndex, _paintableMeshTextureName);
			paintableMeshTexture.Texture = _dirtTexture;

			CwChannelCounter cwChannelCounter = childObject.AddComponent<CwChannelCounter>();
			cwChannelCounter.MaskMesh = originalMeshFilter.sharedMesh;
			cwChannelCounter.PaintableTexture = paintableMeshTexture;

			ChannelCounter = cwChannelCounter;
			_meshCopyGameObject = childObject;
			
			print("CustomMesh initialized. Created mesh copy for " + gameObject.name);
		}
	}
}