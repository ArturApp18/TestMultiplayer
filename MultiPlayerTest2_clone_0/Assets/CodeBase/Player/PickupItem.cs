using Photon.Pun;
using UnityEngine;

public class PickupItem : MonoBehaviour, IPunObservable
{
	public PhotonView photonView;
	private Rigidbody rb;
	public bool isHeld = false;
	private Transform holder;

	void Awake()
	{
		photonView = GetComponent<PhotonView>();
		rb = GetComponent<Rigidbody>();
	}

	[PunRPC]
	public void PickUp(int playerViewID)
	{
		isHeld = true;
		rb.isKinematic = true;
		// Находим PhotonView игрока по viewID
		PhotonView playerPv = PhotonView.Find(playerViewID);
		
		if (playerPv == null)
			return;

		// Привязываем к точке HoldPoint игрока
		transform.SetParent(playerPv.transform.Find("HandPosition"));
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		holder = transform.parent;
	}

	[PunRPC]
	public void Drop(Vector3 throwForce)
	{
		isHeld = false;
		transform.SetParent(null);
		rb.isKinematic = false;
		rb.AddForce(throwForce, ForceMode.Impulse);
		holder = null;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(isHeld);
		}
		else
		{
			isHeld = (bool)stream.ReceiveNext();
		}
	}
}