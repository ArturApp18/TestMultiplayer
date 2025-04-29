using UnityEngine;
using Photon.Pun;

namespace CodeBase.Items
{
	public class PickupItem : MonoBehaviourPun
	{
		[SerializeField] private string itemName = "Default Item";

		private void Start()
		{
			if (photonView.IsMine)
			{
				// Обеспечиваем, чтобы физика была включена у объекта
				Rigidbody rb = GetComponent<Rigidbody>();
				if (rb != null)
				{
					rb.isKinematic = false;
				}
			}
		}

		// Исправленный RPC метод для подъема предмета
		[PunRPC]
		public void Pickup(int itemID)
		{
			// Например, мы можем использовать itemID для дополнительных действий
			// Например, для разных типов предметов или их идентификации.
			Debug.Log($"Pickup item with ID: {itemID}");

			// Отключаем физику для поднятого предмета
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.isKinematic = true;
			}
		}

		// Метод для отпускания предмета
		[PunRPC]
		public void Drop(int itemID)
		{
			// Восстановление физики при отпускании
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.isKinematic = false;
			}
		}
	}
}