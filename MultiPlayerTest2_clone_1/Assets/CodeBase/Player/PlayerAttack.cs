using Photon.Pun;
using UnityEngine;

namespace CodeBase.Player
{
	public class PlayerAttack : MonoBehaviourPunCallbacks
	{
		[SerializeField] private float _attackRange = 5f; // Дальность атаки
		[SerializeField] private float _damage = 10f; // Урон от атаки
		[SerializeField] private LayerMask _targetLayer; // Слой для целей (например, Player)
		[SerializeField] private Transform _attackPoint; // Точка, откуда начинается Raycast (опционально)

		private void Start()
		{
			if (!photonView.IsMine) 
				enabled = false; // Только для локального игрока

			// Если _attackPoint не указан, используем центр игрока
			if (_attackPoint == null)
			{
				_attackPoint = transform;
			}
		}

		private void Update()
		{
			if (!photonView.IsMine) 
				return;

			if (Input.GetMouseButtonDown(0)) // ЛКМ для атаки
			{
				TryAttack();
			}
		}

		private void TryAttack()
		{
			// Raycast начинается от _attackPoint и идет в направлении игрока
			Vector3 rayOrigin = transform.position;
			Vector3 rayDirection = transform.forward; // Направление игрока

			if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _attackRange, _targetLayer))
			{
				HeroHealth targetHealth = hit.collider.GetComponent<HeroHealth>();
				PhotonView targetPhotonView = hit.collider.GetComponent<PhotonView>();

				if (targetHealth != null && targetPhotonView != null && targetPhotonView.ViewID != photonView.ViewID)
				{
					// Вызываем урон через RPC
					targetPhotonView.RPC("ApplyDamage", RpcTarget.All, _damage);
				}
			}
		}
	}
}