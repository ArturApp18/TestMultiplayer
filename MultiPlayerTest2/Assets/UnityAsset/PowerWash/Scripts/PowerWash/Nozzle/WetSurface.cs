using DG.Tweening;
using PaintCore;
using PaintIn3D;
using UnityEngine;
using UnityEngine.Serialization;

namespace PowerWash.Scripts.PowerWash.Nozzle
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [DisallowMultipleComponent]
    public class WetSurface : MonoBehaviour
    {
        private static readonly int s_emissionColor = Shader.PropertyToID("_EmissionColor");
        [SerializeField] private Dirts.Dirt[] _dirts;
        [SerializeField] private Color _finishNotificationColor = Color.white;

        public CwPaintableMeshTexture PaintableMeshTexture { get; private set; }
        public CwGraduallyFade GraduallyFade { get; private set; }

        private bool _staticDetected;
        private MeshFilter _meshFilter;
        private Mesh _newMesh;
        private int _totalDirtOnSurface;

        private void Awake()
        {
            PaintableMeshTexture = GetComponent<CwPaintableMeshTexture>();
            GraduallyFade = GetComponent<CwGraduallyFade>();

            TryCopyMesh();
        }

        private void Start()
        {
            TryInsertMesh();
            foreach (Dirts.Dirt dirt in _dirts)
            { 
                dirt.OnCleaned += OnDirtCleaned;
            }
        }

        private void OnDirtCleaned()
        {
            _totalDirtOnSurface++;
            if (_totalDirtOnSurface != _dirts.Length)
                return;

            Material material = GetComponent<Renderer>().material;
            material.ToggleEmission(true);
            DOTween.Sequence()
                .Append(material.DOColor(_finishNotificationColor, s_emissionColor, 0.55f))
                .Append(material.DOColor(Color.black, s_emissionColor, 0.55f))
                .SetEase(Ease.InSine)
                .OnComplete(() => material.SetColor(s_emissionColor, Color.black));
        }

        private void TryCopyMesh()
        {
            if (!gameObject.isStatic) return;

            _meshFilter = GetComponent<MeshFilter>();
            _newMesh = Instantiate(_meshFilter.sharedMesh);
            _newMesh.name = "MeshCopy";
            _staticDetected = true;
        }

        private void TryInsertMesh()
        {
            if (!_staticDetected) return;
            _meshFilter.mesh = _newMesh;
        }
    }
}