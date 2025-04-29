using UnityEngine;

namespace CodeBase.CameraLogic
{
	public class CameraFollow : MonoBehaviour
	{
		private Transform target;
		private Vector3 offset = new Vector3(0, 5, -7); // Позиция камеры относительно игрока

		public void SetTarget(Transform newTarget)
		{
			target = newTarget;
		}

		private void LateUpdate()
		{
			if (target != null)
			{
				transform.position = target.position + offset;
				transform.LookAt(target);
			}
		}
	}
}