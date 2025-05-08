using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace PowerWash.Nozzle
{
    public class Stream : MonoBehaviour
    {
        private const string WashableLayer = "Washable";
        private const string WetSurface = "WetSurface";
        private const string NpcManagers = "NPCManagers";
        [SerializeField] private LineRenderer _lineRenderer;
        [field: SerializeField] public ParticleSystem ParticleSystem { get; private set; }
        [SerializeField] private float _maxDistance;
        [SerializeField] private float _speed = 1.75f;
        [SerializeField] private int _pointCount = 10;

        private int _damage = 1;
        private float _durationReaction = 1f;
        private float _timeDurationReaction;
        private Vector3 _targetPosition;
        private Coroutine _streamRoutine;
        private int _layerMask;
        private bool _isHitting;
        private Transform _origin;
        private Camera _mainCamera;
        private Coroutine _particleRoutine;

        private void Awake() =>
            _layerMask = 1 << LayerMask.NameToLayer(WashableLayer) | 1 << LayerMask.NameToLayer(WetSurface) | 1 << LayerMask.NameToLayer(NpcManagers);

        private void Start()
        {
            _mainCamera = Camera.main;
            _pointCount = _lineRenderer.positionCount;
            UpdateLinePositions();
        }

        public void AdjustWidth(float selectedNozzleWidth, float selectedNozzleDistance)
        {
            _lineRenderer.widthMultiplier = selectedNozzleWidth * 0.5f;
            ParticleSystem.ShapeModule shape = ParticleSystem.shape;
            shape.radius = selectedNozzleWidth * 0.05f;
            _maxDistance = selectedNozzleDistance;
        }

        public void Begin()
        {
            gameObject.SetActive(true);
            ParticleSystem.gameObject.SetActive(true);
            ResetStream();
            AdjustParticle();
            _streamRoutine = StartCoroutine(BeginStream());
        }

        public void End()
        {
            if (_streamRoutine != null)
                StopCoroutine(_streamRoutine);

            if (gameObject.activeSelf)
                _streamRoutine = StartCoroutine(EndPour());
        }

        public void SetOrigin(Transform origin) =>
            _origin = origin;
        
        private IEnumerator EndPour()
        {
            while (!HasReachedTargetPosition(_pointCount - 1, _targetPosition))
            {
                AnimateToPosition(_pointCount - 1, _targetPosition);
                UpdateIntermediatePoints();
                yield return null;
            }

            ReturnToBasePosition();
        }

        private void ReturnToBasePosition()
        {
            ResetStream();
            gameObject.SetActive(false);
        }

        private void ResetStream() =>
            UpdateLinePositions();

        public void RotateLine(NozzleController nozzle)
        {
            ParticleSystem.ShapeModule shape = ParticleSystem.shape;
            float targetAngle = nozzle.CurrentOrientation == SprayOrientation.Vertical ? 90f : 0f;

            DOTween.Kill(shape);
            DOTween.To(() => shape.rotation.z,
                x => shape.rotation = new Vector3(0, 0, x),
                targetAngle,
                0.25f
            );

            Vector3 targetForward =
                nozzle.CurrentOrientation == SprayOrientation.Vertical ? Vector3.up : Vector3.forward;
            ParticleSystem.transform.DORotateQuaternion(Quaternion.LookRotation(targetForward), 0.25f)
                .SetEase(Ease.Linear);
        }

        private void AdjustParticle()
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(UpdateParticle(Vector3.up));
            }
        }
        private IEnumerator BeginStream()
        {
            while (gameObject.activeSelf)
            {
                (Vector3 point, Vector3 normal) = FindEndPoint();
                _targetPosition = point;
                MoveToPosition(0, transform.position);
                AnimateToPosition(_pointCount - 1, _targetPosition);
                UpdateIntermediatePoints();

                if (HasReachedTargetPosition(_pointCount - 1, _targetPosition) && _isHitting)
                {
                    _particleRoutine ??= StartCoroutine(UpdateParticle(normal));
                    ParticleSystem.gameObject.SetActive(true);
                }
                else
                {
                    if (_particleRoutine != null)
                    {
                        StopCoroutine(_particleRoutine);
                        _particleRoutine = null;
                    }
                    ParticleSystem.gameObject.SetActive(false);
                }

                yield return null;
            }
        }

        private (Vector3 point, Vector3 normal) FindEndPoint()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                return (transform.position, Vector3.up);

            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _layerMask))
            {
                _isHitting = true;
                return (hit.point, hit.normal);
            }
            else
            {
                _isHitting = false;
                return (ray.GetPoint(_maxDistance), Vector3.up);
            }
        }

        private void MoveToPosition(int index, Vector3 position) =>
            _lineRenderer.SetPosition(index, position);

        private void AnimateToPosition(int index, Vector3 targetPosition)
        {
            Vector3 currentPosition = _lineRenderer.GetPosition(index);
            Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * _speed);
            _lineRenderer.SetPosition(index, newPosition);
        }

        private void UpdateIntermediatePoints()
        {
            Vector3 start = _lineRenderer.GetPosition(0);
            Vector3 end = _lineRenderer.GetPosition(_pointCount - 1);
            for (int i = 1; i < _pointCount - 1; i++)
            {
                float t = (float)i / (_pointCount - 1);
                Vector3 position = Vector3.Lerp(start, end, t);
                _lineRenderer.SetPosition(i, position);
            }
        }

        private bool HasReachedTargetPosition(int index, Vector3 targetPosition)
        {
            Vector3 currentPosition = _lineRenderer.GetPosition(index);
            return Vector3.Distance(currentPosition, targetPosition) < 0.1f;
        }
        private void UpdateLinePositions()
        {
            if (_mainCamera == null)
                return;

            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            float fovFactor = Mathf.Lerp(0.2f, 1f, (_mainCamera.fieldOfView - 30f) / 60f);
            Vector3 startPoint = ray.origin + ray.direction * -fovFactor;

            for (int i = 0; i < _pointCount; i++)
            {
                Vector3 adjustedPosition = startPoint + ray.direction * (i * (_maxDistance / _pointCount));
                _lineRenderer.SetPosition(i, adjustedPosition);
            }
        }

        private IEnumerator UpdateParticle(Vector3 surfaceNormal)
        {
            Vector3 cameraRight = Camera.main.transform.right;
            Vector3 right = Vector3.ProjectOnPlane(cameraRight, surfaceNormal).normalized;
            Vector3 forward = Vector3.Cross(surfaceNormal, right).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(forward, surfaceNormal);

            while (gameObject.activeSelf && _isHitting)
            {
                Vector3 endPoint = _lineRenderer.GetPosition(_pointCount - 1);
                Vector3 targetPosition = endPoint + surfaceNormal * 0.1f;
                
                if (_isHitting)
                {
                    ParticleSystem.transform.DOMove(targetPosition, 0.2f).SetEase(Ease.Linear);
                }
                else
                {
                    ParticleSystem.transform.position = targetPosition;
                }
                
                ParticleSystem.transform.DORotateQuaternion(targetRotation, 0.3f).SetEase(Ease.Linear);

                yield return new WaitForSeconds(0.1f);
            }
        }

    }
}