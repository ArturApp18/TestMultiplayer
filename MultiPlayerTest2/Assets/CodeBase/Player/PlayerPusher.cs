using UnityEngine;
using Photon.Pun;

namespace CodeBase.Player
{
	public class PlayerPusher : MonoBehaviourPun
	{
		[SerializeField] private float _pushForce = 5f;
		[SerializeField] private float _pushDistance = 2f;
		[SerializeField] private LayerMask _pushableLayers;
		[SerializeField] private PlayerPickup _playerPickup;

		private void Update()
		{
			if (!photonView.IsMine) return;

			if (Input.GetKeyDown(KeyCode.E))
			{
				TryPush();
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Vector3 origin = transform.position + Vector3.up * 0.5f; // немного выше центра
			Vector3 direction = transform.forward * _pushDistance;

			Gizmos.DrawLine(origin, origin + direction);
			Gizmos.DrawSphere(origin + direction, 0.1f);
		}

		private void TryPush()
		{
			Vector3 origin = transform.position + Vector3.up * 0.5f; // немного выше центра
			Vector3 direction = transform.forward;

			if (Physics.Raycast(origin, direction, out RaycastHit hit, _pushDistance, _pushableLayers))
			{
				PhotonView targetView = hit.collider.GetComponentInParent<PhotonView>();
				if (targetView != null && !targetView.IsMine)
				{
					Vector3 force = direction.normalized * _pushForce;
					targetView.RPC("ReceivePush", targetView.Owner, force); // ТОЛЬКО на владельце
					Debug.Log($"Pushed {targetView.Owner.NickName} with force {force}");
				}
			}
		}

		[PunRPC]
		private void ReceivePush(Vector3 force)
		{
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb == null) return;

			rb.AddForce(force, ForceMode.Impulse);
			print("Received push force: " + force);
			// Если у нас есть предмет в руках, то отпускаем его
			if (_playerPickup != null && _playerPickup.CurrentItem != null)
			{
				_playerPickup.DropItem();
				Debug.Log("Dropped item while being pushed.");
			}
		}
	}
}