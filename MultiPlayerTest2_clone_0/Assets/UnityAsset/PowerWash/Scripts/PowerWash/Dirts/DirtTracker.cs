using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace PowerWash.Scripts.PowerWash.Dirts
{
    [DisallowMultipleComponent]
    public class DirtTracker : MonoBehaviour
    {
        public event Action OnAllDirtCleaned;
        public event Action<float> OnSingleDirtCleaned;
        public event Action OnCleanedDirtThresholdExceeded;
        
        [field: SerializeField] public List<Dirt> DirtComponents { get; private set; } = new List<Dirt>();
        [HideInInspector, SerializeField] private int _totalDirtCount;
        [SerializeField] private float _allCleanedDirtThreshold = 70f;
        
        private StudioEventEmitter _fmodEmitter;
        private NozzleUI _nozzleUI;
        private int _cleanedDirtCount;
        private bool _thresholdExceeded;

        private void Awake() => _fmodEmitter = GetComponent<StudioEventEmitter>();

        public IEnumerator Construct()
        {
            yield return new WaitUntil(() => DirtComponents.Count > 0);
            foreach (Dirt dirt in DirtComponents)
            {
                if (dirt.IsCleaned)
                    _cleanedDirtCount++;
                    
                dirt.OnCleaned += OnDirtCleaned;
            }
            
            yield return new WaitUntil(() => NozzleUI.Instance != null);
            
            _nozzleUI = NozzleUI.Instance;
        }

        private void Update()
        {
            _nozzleUI?.UpdateProgress(GetCleanedPercent());
            
            if (Input.GetKeyDown(KeyCode.CapsLock))
                HintAllDirtObjects();
        }

        private void OnDestroy()
        {
            foreach (Dirt dirt in DirtComponents)
            {
                if (dirt == null) continue;
                dirt.OnCleaned -= OnDirtCleaned;
            }
        }

        public void SetDirty(List<Dirt> collectedDirt)
        {
            DirtComponents = collectedDirt;
            _totalDirtCount = DirtComponents.Count;
            _cleanedDirtCount = 0;

            print($"Total Dirt Count: {_totalDirtCount}");
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private void HintAllDirtObjects()
        {
            foreach (Dirt dirt in DirtComponents)
            {
                if (dirt.IsCleaned || dirt.DisplayingHint) continue;
                dirt.PerformHintBlink();
            }
        }
        
        private float GetCleanedPercent()
            => _cleanedDirtCount / (float)_totalDirtCount;
        
        private void OnDirtCleaned()
        {
            _cleanedDirtCount++;
            float normalizedPercent = GetCleanedPercent();
            _nozzleUI.UpdateProgress(normalizedPercent);

            float percent = normalizedPercent * 100f;
            
            OnSingleDirtCleaned?.Invoke(normalizedPercent);

            if (percent >= _allCleanedDirtThreshold && !_thresholdExceeded)
            {
                _thresholdExceeded = true;
                OnCleanedDirtThresholdExceeded?.Invoke();
            }
            
            if (_cleanedDirtCount < _totalDirtCount) return;
            OnAllDirtCleaned?.Invoke();
        }
    }
}