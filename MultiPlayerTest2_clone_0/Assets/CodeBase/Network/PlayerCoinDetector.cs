using Photon.Pun;
using UnityEngine;

namespace CodeBase.Network
{
	public class PlayerCoinDetector : MonoBehaviourPun
	{
		[SerializeField] private PlayerView _playerView;

		private void OnTriggerEnter(Collider collider)
		{
			if (!photonView.IsMine) return;

			if (collider.gameObject.TryGetComponent(out Coin coin))
			{
				coin.OnCoinCollected += _playerView.OnCoinCollected;
				coin.Pickup(photonView);
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			if (collider.gameObject.TryGetComponent(out Coin coin))
			{
				coin.OnCoinCollected -= _playerView.OnCoinCollected;
			}
		}
	}
}