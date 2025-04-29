using Photon.Pun;
using UnityEngine;

namespace CodeBase.Player
{
	public class PlayerPickup : MonoBehaviourPun
	{
		[SerializeField] private float _pickupRange = 3f; // Радиус, в котором можно взять предмет
		[SerializeField] private Transform _handTransform; // Точка, куда будет прикрепляться предмет в руке

		public GameObject CurrentItem { get; private set; }

		private void Update()
		{
			if (!photonView.IsMine) return;

			// Проверка нажатия кнопки для подбора предмета
			if (Input.GetKeyDown(KeyCode.F))
			{
				if (CurrentItem == null)
				{
					TryPickupItem();
				}
				else
				{
					DropItem();
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Vector3 rayOrigin = transform.position;
			Vector3 rayDirection = transform.forward * _pickupRange;

			Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection);
			Gizmos.DrawSphere(rayOrigin + rayDirection, 0.1f);
		}

		public void DropItem()
		{
			if (CurrentItem != null)
			{
				PhotonView itemView = CurrentItem.GetComponent<PhotonView>();
				if (itemView != null)
				{
					itemView.RPC("Drop", RpcTarget.AllBuffered);
				}

				CurrentItem = null;
			}
		}

		private void TryPickupItem()
		{
			// Отправляем луч вперед от игрока
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _pickupRange))
			{
				// Проверяем, если объект с этим тегом может быть подобран
				if (hit.collider.CompareTag("Pickable"))
				{
					// Если предмет подходит, вызываем RPC, чтобы взять его в руки
					PhotonView targetView = hit.collider.GetComponent<PhotonView>();
					if (targetView != null)
					{
						targetView.RPC("Pickup", RpcTarget.AllBuffered, photonView.ViewID);
						Debug.Log("Picked up item: " + hit.collider.name);
						CurrentItem = hit.collider.gameObject;
						// Привязываем предмет к руке игрока
						CurrentItem.transform.SetParent(_handTransform);
						CurrentItem.transform.localPosition = Vector3.zero; // Устанавливаем позицию в ноль относительно руки
						CurrentItem.transform.localRotation = Quaternion.identity; // Устанавливаем поворот в ноль
					}
				}
			}
		}
	}
}