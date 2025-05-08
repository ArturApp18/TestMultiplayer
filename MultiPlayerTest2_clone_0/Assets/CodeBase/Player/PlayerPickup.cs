using Photon.Pun;
using UnityEngine;

public class PlayerPickup : MonoBehaviourPunCallbacks
{
	public float pickupRange = 2f;
	public Transform holdPoint;
	private PickupItem heldItem = null;

	void Start()
	{
		if (!photonView.IsMine) enabled = false;
	}

	void Update()
	{
		if (!photonView.IsMine) return;

		if (Input.GetKeyDown(KeyCode.E))
		{
			if (heldItem == null)
			{
				TryPickup();
			}
			else
			{
				DropItem();
			}
		}
	}

	void TryPickup()
	{
		if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, pickupRange))
		{
			PickupItem item = hit.collider.GetComponent<PickupItem>();
			if (item != null && !item.isHeld)
			{
				heldItem = item;
				// Передаем ViewID игрока вместо PhotonView
				item.photonView.RPC("PickUp", RpcTarget.All, photonView.ViewID);
			}
		}
	}

	void DropItem()
	{
		if (heldItem != null)
		{
			Vector3 throwForce = transform.forward * 5f;
			heldItem.photonView.RPC("Drop", RpcTarget.All, throwForce);
			heldItem = null;
		}
	}
}