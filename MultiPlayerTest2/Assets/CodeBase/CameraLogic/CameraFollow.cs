namespace CodeBase.CameraLogic
{
    using UnityEngine;

    public class CameraFollow : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _offset = new Vector3(0, 5, -7);

        private void Start()
        {
            if (Camera.main != gameObject.GetComponent<Camera>())
            {
                Debug.LogWarning(
                    $"CameraFollow on {gameObject.name} is not MainCamera! Current MainCamera: {Camera.main?.name}");
            }
        }

        public void SetTarget(Transform newTarget)
        {
            if (newTarget == null)
            {
                Debug.LogError("CameraFollow: Attempted to set null target!");
                return;
            }

            _target = newTarget;
            Debug.Log($"CameraFollow set to target: {newTarget.name} (Position: {newTarget.position})");
        }

        private void LateUpdate()
        {
            if (_target != null)
            {
                transform.position = _target.position + _offset;
                transform.LookAt(_target);
            }
        }
    }
}