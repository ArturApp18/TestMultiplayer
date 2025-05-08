using System;
using DG.Tweening;
using PowerWash.Nozzle;
using PowerWash.Scripts.PowerWash.Dirts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PowerWash.Scripts.PowerWash
{
	[DisallowMultipleComponent]
	public class NozzleUI : MonoBehaviour
	{
		public static NozzleUI Instance { get; private set; }

		[Header("External References")]
		[SerializeField] private NozzleController _nozzleController;
		[SerializeField] private DirtTracker _dirtTracker;
		[SerializeField] private MeshRenderer _powerWashNozzleColor;
		[SerializeField] private Image _cleanedPercentImage;
		[SerializeField] private TextMeshProUGUI _percentText;
		[SerializeField] private GameObject _crosshair;
		[SerializeField] private Image _powerWashCrosshairImage;
		[SerializeField] private GameObject _powerTriplePointImage;
		
		[Header("Settings")]
		[SerializeField] private Color _blueNozzleColor;
		[SerializeField] private Color _greenNozzleColor;
		[SerializeField] private Color _purpleNozzleColor;
		[SerializeField] private Color _orangeNozzleColor;

		public bool IsRotating { get; private set; }

		private void OnValidate()
		{
			_nozzleController ??= FindObjectOfType<NozzleController>();
			_dirtTracker ??= FindObjectOfType<DirtTracker>();
		}

		private void Awake()
			=> Instance = this;

		private void Start()
		{
			SetNozzleMaterial(_nozzleController.SelectedNozzle.NozzleType);
			_nozzleController.OnNozzleSwitched += OnNozzleSwitched;
			_nozzleController.OnToggleRotated += OnToggleRotated;
		}

		private void OnDestroy()
		{
			_nozzleController.OnNozzleSwitched -= OnNozzleSwitched;
			_nozzleController.OnToggleRotated -= OnToggleRotated;
		}

		public void SetCrosshairActive()
		{
			_crosshair.SetActive(true);
			_powerWashCrosshairImage.gameObject.SetActive(true);
			_powerTriplePointImage.SetActive(false);
		}

		public void SetCrosshairDeactivate()
		{
			_powerWashCrosshairImage.gameObject.SetActive(false);
			_crosshair.SetActive(false);
			_powerTriplePointImage.SetActive(true);
		}

		public void UpdateProgress(float percentage)
		{
			_cleanedPercentImage.fillAmount = percentage;
			if (_percentText != null)
				_percentText.text = $"{(percentage * 100f):F2}%";
		}
		private void SetNozzleMaterial(NozzleType nozzleType)
		{
			if (_powerWashNozzleColor == null)
				return;

			Material lastMaterial = _powerWashNozzleColor.materials[_powerWashNozzleColor.materials.Length - 1];
			switch (nozzleType)
			{
				case NozzleType.Unknown:
					break;
				case NozzleType.Purple:
					lastMaterial.color = _purpleNozzleColor;
					break;
				case NozzleType.Blue:
					lastMaterial.color = _blueNozzleColor;
					break;
				case NozzleType.Green:
					lastMaterial.color = _greenNozzleColor;
					break;
				case NozzleType.Orange:
					lastMaterial.color = _orangeNozzleColor;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(nozzleType), nozzleType, null);
			}
		}
		private void OnNozzleSwitched(NozzleType nozzleType)
		{
			SetNozzleMaterial(nozzleType);
			AdjustCrosshairSize(nozzleType);
		}

		private void AdjustCrosshairSize(NozzleType nozzleType)
		{
			RectTransform crosshairRectTransform = _powerTriplePointImage.GetComponent<RectTransform>();
			crosshairRectTransform.localScale = nozzleType switch {
				NozzleType.Purple => new Vector3(0.5f, 0.5f, 1f),
				NozzleType.Blue => new Vector3(1f, 1f, 1f),
				NozzleType.Green => new Vector3(1.25f, 1.25f, 1f),
				NozzleType.Orange => new Vector3(0.75f, 0.75f, 1f),
				_ => new Vector3(1f, 1f, 1f)
			};
		}
		private void OnToggleRotated(global::PowerWash.Nozzle.Nozzle obj)
		{
			RotateImage(_powerTriplePointImage);
		}

		private void RotateImage(GameObject image)
		{
			if (IsRotating) return;

			IsRotating = true;
			image.transform.DORotate(new Vector3(0, 0, image.transform.rotation.eulerAngles.z + 90), 0.5f)
				.SetEase(Ease.Linear)
				.OnComplete(() => IsRotating = false);
		}
	}
}