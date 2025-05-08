

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using PaintIn3D;
using Photon.Pun;
using PowerWash.Scripts.PowerWash;
using UnityEngine;

namespace PowerWash.Nozzle
{
	[DisallowMultipleComponent]
	public class NozzleController : MonoBehaviour
	{
		public event Action<NozzleType> OnNozzleSwitched;
		public event Action<Nozzle> OnToggleRotated;

		public Nozzle SelectedNozzle
		{
			get => _selectedNozzle;
			set => _selectedNozzle = value;
		}
		
		public SprayOrientation CurrentOrientation { get; private set; } = SprayOrientation.Horizontal;

		[Header("Nozzle Settings")]
		[SerializeField] private BlueNozzle _blueNozzle;
		[SerializeField] private GreenNozzle _greenNozzle;
		[SerializeField] private OrangeNozzle _orangeNozzle;
		[SerializeField] private PurpleNozzle _purpleNozzle;

		[Space(10)]
		[SerializeField] private Nozzle _defaultNozzle;

		[Header("External References")]
		[SerializeField] private CwPaintSphere _paintSphere;
		[SerializeField] private CwPaintSphere _waterPrintSphere;
		[SerializeField] private PowerWashVFX _powerWashVFX;

		[Header("Detection Settings")]
		[SerializeField] private LayerMask _dirtAndWetSurfaceLayer;

		[Header("Audio")]
		[SerializeField] private EventReference _startSpray;
		[SerializeField] private EventReference _endSpray;
		
		[SerializeField] private bool _isActive;
		[SerializeField] private bool _testMode;
		
		private Nozzle _selectedNozzle;
		private int _currentNozzleIndex;

		private Dictionary<int, Nozzle> _nozzleIndexes = new();
		private NozzleUI _nozzleUI;

		private void Awake()
		{
			SetDefaultNozzle();
		}

		private IEnumerator Start()
		{
			yield return new WaitUntil(() => NozzleUI.Instance != null);
			_nozzleUI = NozzleUI.Instance;
		}

		private void Update()
		{
			if (!_isActive) return;
			TrySprayWithSelectedNozzle();
			TrySwitchNozzle();
			TryToggleOrientation();
		}

		public void Activate()
		{
			_isActive = true;
			_powerWashVFX.Activate();
			// NozzleUI.Instance.SetCrosshairDeactivate();
		}

		public void Deactivate()
		{
			_isActive = false;
			_powerWashVFX.Deactivate();
			// NozzleUI.Instance.SetCrosshairActive();
		}

		private void TryToggleOrientation()
		{
			// if (_nozzleUI.IsRotating)
			// 	return;
			
			if (!Input.GetKeyDown(KeyCode.Mouse1)) return;

			CurrentOrientation = CurrentOrientation == SprayOrientation.Vertical
				? SprayOrientation.Horizontal
				: SprayOrientation.Vertical;
			
			_selectedNozzle.ToggleOrientation(CurrentOrientation);
			OnToggleRotated?.Invoke(_selectedNozzle);
		}

		private void TrySprayWithSelectedNozzle()
		{
			if (Input.GetKey(KeyCode.Mouse0))
				_selectedNozzle.Spray();
		}

		private void TrySwitchNozzle()
		{
			if (!Input.GetKeyDown(KeyCode.R)) return;

			_currentNozzleIndex++;

			if (_currentNozzleIndex >= _nozzleIndexes.Count)
				_currentNozzleIndex = 0;

			_selectedNozzle = _nozzleIndexes[_currentNozzleIndex];

			_selectedNozzle.Construct(_paintSphere, _waterPrintSphere, _dirtAndWetSurfaceLayer);
			OnNozzleSwitched?.Invoke(_selectedNozzle.NozzleType);
		}

		private void SetDefaultNozzle()
		{
			_nozzleIndexes = new Dictionary<int, Nozzle> {
				{ 0, _blueNozzle },
				{ 1, _greenNozzle },
				{ 2, _orangeNozzle },
				{ 3, _purpleNozzle }
			};

			_currentNozzleIndex = _nozzleIndexes.FirstOrDefault(x => x.Value == _defaultNozzle).Key;

			_selectedNozzle = _defaultNozzle;
			_selectedNozzle.Construct(_paintSphere, _waterPrintSphere, _dirtAndWetSurfaceLayer);
			OnNozzleSwitched?.Invoke(_selectedNozzle.NozzleType);
		}
	}
}