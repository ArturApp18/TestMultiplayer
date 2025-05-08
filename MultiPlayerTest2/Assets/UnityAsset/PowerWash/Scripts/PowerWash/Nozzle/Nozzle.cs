using DG.Tweening;
using PaintIn3D;
using Photon.Pun;
using PowerWash.Dirt;
using PowerWash.Scripts.PowerWash.Nozzle;
using UnityEngine;

namespace PowerWash.Nozzle
{
	public abstract class Nozzle : MonoBehaviour
	{
		public Vector3 Normal { private set; get; }
		
		private static readonly RaycastHit[] s_results = new RaycastHit[3];

		[field: Header("Nozzle Settings")]
		[field: SerializeField] public float Length { get; protected set; }

		[field: SerializeField] public float Width { get; protected set; }
		[field: SerializeField] public float Height { get; protected set; }
		[field: SerializeField] public float Power { get; protected set; } = 2f;
		[field: SerializeField] public float Radius { get; protected set; }
		[field: SerializeField] public float Distance { get; protected set; }


		[field: Space(10)]
		[field: SerializeField] public NozzleType NozzleType { get; protected set; } = NozzleType.Unknown;

		[SerializeField] private float _cleaningCooldown = 0.1f;
		private CwPaintSphere _paintSphere;
		private CwPaintSphere _waterPrintSphere;
		private SprayOrientation _currentOrientation;
		private PowerWashVFX _powerWashVFX;
		private LayerMask _dirtLayer;
		private Camera _camera;
		private float _lastCleaningTime;
		private readonly int _priority = 0;
		private readonly int _seed = 0;
		private readonly float _pressure = 1.0f;

		private void Awake()
		{
			OnAwake();
		}

		private void Start()
		{
			_camera = Camera.main;
			OnStart();
		}

		public void Construct(CwPaintSphere paintSphere, CwPaintSphere waterPrintSphere,
			LayerMask dirtLayer)
		{
			_paintSphere = paintSphere;
			_waterPrintSphere = waterPrintSphere;
			_dirtLayer = dirtLayer;
			ApplySettingsToPaintSphere();
		}

		public abstract void Spray();

		public void ToggleOrientation(SprayOrientation currentOrientation)
		{
			_currentOrientation = currentOrientation;
			ApplySettingsToPaintSphere();
		}
		
		public void SetVFX(PowerWashVFX powerWashVFX) =>
			_powerWashVFX = powerWashVFX;

		protected void TryCleaningDirt()
		{
			if (Time.time - _lastCleaningTime < _cleaningCooldown)
				return;

			_lastCleaningTime = Time.time;
			Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
			int size = Physics.RaycastNonAlloc(ray, s_results, Distance, _dirtLayer);

			for (int i = 0; i < size; i++)
			{
				Normal = s_results[i].normal;

				Vector3 right = Vector3.ProjectOnPlane(_camera.transform.right, Normal).normalized;
				Vector3 forward = Vector3.Cross(Normal, right).normalized;

				Quaternion cleanRotation = Quaternion.LookRotation(forward, Normal);

				if (s_results[i].transform.TryGetComponent(out WetSurface _))
				{
					print("TryWetSurface");
					_waterPrintSphere.HandleHitPoint(false, _priority, _pressure, _seed, 
						s_results[i].point, cleanRotation);
				}
				
				if (!s_results[i].transform.TryGetComponent(out Scripts.PowerWash.Dirts.Dirt dirt)) 
					continue;
				print(dirt.name);
				if (dirt.IsCleaned) continue;
				if (CanCleanDirt(dirt.DirtType, NozzleType))
				{
					print(dirt.name);
					_paintSphere.HandleHitPoint(false, _priority, _pressure, _seed, 
						s_results[i].point, cleanRotation);
				}
			}
		}
		
		protected virtual void OnAwake() { }

		protected virtual void OnStart() { }

		private void ApplySettingsToPaintSphere()
		{
			Vector3 targetScale = _currentOrientation == SprayOrientation.Horizontal
				? new Vector3(Length, Height, Width)
				: new Vector3(Width, Width, Length);

			if (_paintSphere != null)
			{
				_paintSphere.Scale = targetScale;
				_paintSphere.Hardness = Power;
				_paintSphere.Radius = Radius;
			}

			if (_waterPrintSphere == null) return;
			_waterPrintSphere.Radius = Radius;
			_waterPrintSphere.Scale = targetScale;
		}

		private bool CanCleanDirt(DirtType dirtType, NozzleType nozzle)
		{
			return dirtType switch {
				DirtType.Fresh => true,
				DirtType.Special => nozzle == NozzleType.Orange,
				_ => false
			};
		}
	}
}