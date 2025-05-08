using System;
using Photon.Pun;

namespace CodeBase.Network
{
	public class Coin : MonoBehaviourPun
	{
		public event Action<PhotonView> OnCoinCollected;

		public void Pickup(PhotonView collectorView)
		{
			photonView.RPC(nameof(CollectCoin), RpcTarget.All, collectorView.ViewID);
		}

		[PunRPC]
		public void CollectCoin(int collectorID)
		{
			var collectorView = PhotonView.Find(collectorID);
			OnCoinCollected?.Invoke(collectorView);
			if (!PhotonNetwork.IsMasterClient) return;
			PhotonNetwork.Destroy(gameObject);
		}
	}
}