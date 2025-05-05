using Photon.Pun;
using UnityEngine;

namespace CodeBase.Player
{
	using Photon.Pun;
	using UnityEngine;

	namespace CodeBase.Player
	{ }
	public class PlayerHeal : MonoBehaviourPunCallbacks
	{
		[SerializeField] private float healRange = 3f; // Дистанция для лечения
		[SerializeField] private float healPercentage = 10f; // 10% от максимального HP

		private void Start()
		{
			if (!photonView.IsMine) enabled = false; // Только для локального игрока
		}

		private void Update()
		{
			if (!photonView.IsMine) return;

			if (Input.GetKeyDown(KeyCode.F))
			{
				TryHeal();
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawWireSphere(transform.position, healRange);
		}

		private void TryHeal()
		{
			// Проверяем игроков в радиусе
			Collider[] hits = Physics.OverlapSphere(transform.position, healRange, LayerMask.GetMask("Player"));
			foreach (Collider hit in hits)
			{
				if (hit.gameObject != gameObject) // Не лечим себя
				{
					HeroHealth targetHealth = hit.GetComponent<HeroHealth>();
					if (targetHealth != null && targetHealth.Current > 0) // Лечим только живых
					{
						print("Healing " + targetHealth.gameObject.name);
						// Вызываем лечение через RPC
						targetHealth.GetComponent<PhotonView>().RPC("ApplyHeal", RpcTarget.All, healPercentage);
						return; // Лечим только одного игрока
					}
				}
			}
		}
	}
}