using System.Collections;
using UnityEngine;
using System.Linq;

namespace SimulationGameCreator
{
    public class DoorScript : MonoBehaviour
    {
        public bool isLocked = false;
        public bool giveLockMessage = false;
        public bool isOpened = false;
        public int KeyID_ToOpen = 0;
        public float AngleForOpening = 60;
        private Animation animation;
        public bool leftDoor = false;

        private void Start()
        {
            animation = GetComponent<Animation>();
        }

        IEnumerator OpenTheDoor()
        {
            LastTimeTry = LastTimeTry - 1;
            yield return new WaitForSeconds(0.5f);
            TryToOpen();
        }

        public void UnlockAndOpen()
        {
            isLocked = false;
            StartCoroutine(OpenTheDoor());
        }

        private float LastTimeTry = 0;
        public void TryToOpen()
        {
            if (Time.time > LastTimeTry + 1)
            {
                LastTimeTry = Time.time;
                if (isLocked)
                {
                    if (HeroPlayerScript.Instance.Keys_Grabbed.Contains(KeyID_ToOpen))
                    {
                        isLocked = false;
                        AudioManager.Instance.Play_Door_UnLock();
                        GameObject nextDoor = Physics.OverlapSphere(transform.position, 7).Where(x => x.GetComponent<DoorScript>() != null).Where(x => x.transform != transform).Select(x => x.gameObject).FirstOrDefault();
                        if (nextDoor != null)
                        {
                            nextDoor.GetComponent<DoorScript>().UnlockAndOpen();
                        }
                        StartCoroutine(OpenTheDoor());
                    }
                    else
                    {
                        AudioManager.Instance.Play_Door_TryOpen();
                        if (giveLockMessage)
                        {
                            GameCanvas.Instance.Show_Warning_Not("It is locked. Need the right Key!", false);
                        }
                        return;
                    }
                }
                else
                {
                    GameObject nextDoor = Physics.OverlapSphere(transform.position, 7).Where(x => x.GetComponent<DoorScript>() != null).Where(x => x.transform != transform).Select(x => x.gameObject).FirstOrDefault();
                    if (nextDoor != null)
                    {
                        nextDoor.GetComponent<DoorScript>().TryToOpen();
                    }
                    if (isOpened == false)
                    {
                        if (leftDoor)
                        {
                            animation["Custom_Animation_DoorLeft_Open"].time = 0;
                            animation["Custom_Animation_DoorLeft_Open"].speed = 1;
                            animation.Play("Custom_Animation_DoorLeft_Open");
                        }
                        else
                        {
                            animation["Custom_Animation_DoorRight_Open"].time = 0;
                            animation["Custom_Animation_DoorRight_Open"].speed = 1;
                            animation.Play("Custom_Animation_DoorRight_Open");
                        }
                        isOpened = true;
                        AudioManager.Instance.Play_Door_Wooden_Open();
                    }
                    else
                    {
                        isOpened = false;
                        AudioManager.Instance.Play_Door_Close();
                        if (leftDoor)
                        {
                            animation["Custom_Animation_DoorLeft_Open"].time = animation["Custom_Animation_DoorLeft_Open"].length;
                            animation["Custom_Animation_DoorLeft_Open"].speed = -1;
                            animation.Play("Custom_Animation_DoorLeft_Open");
                        }
                        else
                        {
                            animation["Custom_Animation_DoorRight_Open"].time = animation["Custom_Animation_DoorRight_Open"].length;
                            animation["Custom_Animation_DoorRight_Open"].speed = -1;
                            animation.Play("Custom_Animation_DoorRight_Open");
                        }
                    }
                }
            }
        }
    }
}