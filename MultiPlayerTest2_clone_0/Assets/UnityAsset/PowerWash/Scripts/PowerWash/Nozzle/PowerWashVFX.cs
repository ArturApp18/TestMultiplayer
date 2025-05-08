using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace PowerWash.Nozzle
{
	public class PowerWashVFX : MonoBehaviour
	{
		[SerializeField] private Transform _origin;
		[SerializeField] private Stream _streamPrefab;
		[SerializeField] private NozzleController _nozzleController;
		[SerializeField] private bool _isActive;
		[SerializeField] private bool _testMode;

		public bool IsWashing { get; private set; }
		[field: SerializeField] public Stream Stream { get; private set; }
		private Coroutine _adjustRoutine;
		
		private void OnValidate() =>
			_nozzleController ??= FindFirstObjectByType<NozzleController>();

		private void Start()
		{
			_nozzleController.OnNozzleSwitched += AdjustWidth;
			_nozzleController.OnToggleRotated += RotateLine;
			_streamPrefab.AdjustWidth(_nozzleController.SelectedNozzle.Length, _nozzleController.SelectedNozzle.Distance);
		}

		private void OnDestroy()
		{
			_nozzleController.OnNozzleSwitched -= AdjustWidth;
			_nozzleController.OnToggleRotated -= RotateLine;
			_streamPrefab.AdjustWidth(_nozzleController.SelectedNozzle.Width,
				_nozzleController.SelectedNozzle.Distance);
		}

		public void Activate() =>
			_isActive = true;

		public void Deactivate()
		{
			StopWashing();
			_isActive = false;
		}

		private void RotateLine(Nozzle nozzle)
		{
			if (Stream == null) return;
			print("rotating line");
			Stream.RotateLine(_nozzleController);
		}

		private void AdjustWidth(NozzleType obj)
		{
			RotateLine(_nozzleController.SelectedNozzle);
			_adjustRoutine = StartCoroutine(AdjustingWidth(_nozzleController.SelectedNozzle.Width,
				_nozzleController.SelectedNozzle.Distance));
		}

		private IEnumerator AdjustingWidth(float selectedNozzleWidth, float selectedNozzleDistance)
		{
			yield return new WaitUntil(() => Stream != null);
			Stream.AdjustWidth(selectedNozzleWidth, selectedNozzleDistance);
		}

		private void Update()
		{
			if (!_isActive) return;
			if (Input.GetKeyDown(KeyCode.Mouse0))
				StartWashing();
			else if (Input.GetKeyUp(KeyCode.Mouse0))
				StopWashing();
		}

		private void StartWashing()
		{
			IsWashing = true;
			Stream ??= CreateStream();
			Stream.Begin();
		}

		private void StopWashing()
		{
			IsWashing = false;

			if (Stream != null)
				Stream.End();
			if (_adjustRoutine != null)
				StopCoroutine(_adjustRoutine);
		}

		private Stream CreateStream()
		{
			Stream stream = Instantiate(_streamPrefab, _origin.position, _origin.localRotation, transform);
			stream.SetOrigin(_origin);
			return stream;
		}
	}
}