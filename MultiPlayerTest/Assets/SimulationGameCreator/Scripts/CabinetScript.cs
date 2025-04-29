using System.Collections;
using UnityEngine;

namespace SimulationGameCreator
{
    public class CabinetScript : MonoBehaviour
    {
        public bool isOpened = false;
        public GameObject itemInCabinet;
        void Start()
        {
            if (itemInCabinet != null)
            {
                itemInCabinet.GetComponent<Collider>().enabled = false;
            }
        }

        public void Open()
        {
            if (isOpened == false)
            {
                isOpened = true;
                AudioManager.Instance.Play_Audio_Cabinet_Open();
                GetComponent<Animation>().Play("Open");
                GetComponent<SphereCollider>().enabled = false;
                StartCoroutine(ActivateInsideCollider());
            }
        }

        IEnumerator ActivateInsideCollider()
        {
            yield return new WaitForSeconds(1);
            if (itemInCabinet != null)
            {
                itemInCabinet.GetComponent<Collider>().enabled = true;
            }
        }
    }
}