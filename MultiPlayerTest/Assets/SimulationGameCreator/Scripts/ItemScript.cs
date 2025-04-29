using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SimulationGameCreator
{
    public class ItemScript : MonoBehaviour
    {
        public ItemType itemType;
        public string Name = "";
        public InteractionType interactionType;
        public UnityEvent eventToInvokeWhenInteract;
        public Hand_Type NeededHandType = Hand_Type.Any;
        [HideInInspector]
        public bool isInteracted = false;
        public SpriteRenderer sprite;


        private void Start()
        {

        }

        public void Interact()
        {
            if (Time.time < HeroPlayerScript.Instance.LastInteractedTime + 0.5f) return;

            HeroPlayerScript.Instance.LastInteractedTime = Time.time;

            if (itemType == ItemType.Door)
            {
                GetComponent<DoorScript>().TryToOpen();
            }
            else if (itemType == ItemType.Seller)
            {
                GetComponent<SellerScript>().Talk();
            }
            else if (itemType == ItemType.MoneyBag)
            {
                AdvancedGameManager.Instance.Get(CollactableType.Money, transform.GetComponent<GainScript>().amount);
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject);
            }
            else if (itemType == ItemType.NPC)
            {
                GetComponent<CivilianController>().SpeakwithHero();
            }
            else if (itemType == ItemType.ItemToMaintain)
            {
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                AudioManager.Instance.Play_Audio_PressAndHoldMaintainDone();
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Flower)
            {
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                AudioManager.Instance.Play_Item_Grab();
                Destroy(gameObject);
            }
            else if (itemType == ItemType.OldPaper)
            {
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                AudioManager.Instance.Play_audioClip_PaperCrease();
                isInteracted = true;
                StartCoroutine(OldPaperAnimate());
            }
            else if (itemType == ItemType.Key)
            {
                AudioManager.Instance.Play_Item_Grab();
                GetComponent<KeyScript>().isGrabbed = true;
                HeroPlayerScript.Instance.Grab_Key(GetComponent<KeyScript>().KeyID);
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Note && !GetComponent<NoteScript>().isReading)
            {
                GetComponent<NoteScript>().Read();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Note && GetComponent<NoteScript>().isReading)
            {
                GameCanvas.Instance.Hide_Note();
            }
            else if (itemType == ItemType.Box)
            {
                GetComponent<BoxScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Cabinet)
            {
                GetComponent<CabinetScript>().Open();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Drawer)
            {
                GetComponent<DrawerScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.MedKit)
            {
                AudioManager.Instance.Play_Item_Grab();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Bed)
            {
                if (DayNightManager.Instance.time < 3600 * DayNightManager.Instance.sunSetHour && DayNightManager.Instance.time > 3600 * DayNightManager.Instance.sunRiseHour)
                {
                    // Player can't sleep before sun set
                    GameCanvas.Instance.Show_Warning_Not("You can't go to bed before sunset!", false);
                    return;
                }
                StartCoroutine(SleepNow());
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            if(itemType == ItemType.Food || itemType == ItemType.Drink)
            {
                // Add it to the inventory!
                int currentamount = PlayerPrefs.GetInt("Storage_" + Name, 0);
                currentamount = currentamount + 1;
                PlayerPrefs.SetInt("Storage_" + Name, currentamount);
                PlayerPrefs.Save();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                AudioManager.Instance.Play_Item_Grab();
                Destroy(gameObject);
            }

            GameCanvas.Instance.Hide_Warning();
            TaskManager.Instance.CheckTask(interactionType.ToString());
            TaskManager.Instance.CheckTask(Name);
        }

        private void OnDestroy()
        {
            if (HeroPlayerScript.Instance.InteractableObject == this) HeroPlayerScript.Instance.InteractableObject = null;
        }

        public void UpdateSprite()
        {
            if (sprite != null)
            {
                Color color = sprite.color;
                color.a = imageToFill.fillAmount;
                sprite.color = color;
            }
        }

        IEnumerator OldPaperAnimate()
        {
            GetComponentInChildren<Animation>().Play();
            yield return new WaitForSeconds(1.1f);
            GetComponent<Rigidbody>().isKinematic = false;
            transform.GetChild(1).gameObject.SetActive(false);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }

        IEnumerator SleepNow()
        {
            HeroPlayerScript.Instance.DeactivatePlayer();
            CameraScript.Instance.enabled = false;
            GameCanvas.Instance.image_Blinking.GetComponent<Animation>().Play("GotoSleep");
            yield return new WaitForSeconds(2f);
            HeroPlayerScript.Instance.Heal();
            DayNightManager.Instance.Sleep();
            HeroPlayerScript.Instance.Rest();
            GameCanvas.Instance.image_Blinking.GetComponent<Animation>().Play("Blink");
            yield return new WaitForSeconds(2.5f);
            HeroPlayerScript.Instance.ActivatePlayer();
            CameraScript.Instance.enabled = true;
        }

        public void DeactivateCollidersAndRigidbody()
        {
            Collider[] allColliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < allColliders.Length; i++)
            {
                allColliders[i].enabled = false;
                transform.tag = "Untagged";
            }
            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Rigidbody>().useGravity = false;
            }
        }


        public void ActivateCollidersAndRigidbody()
        {
            transform.localScale = new Vector3(1, 1, 1);
            Collider[] allColliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < allColliders.Length; i++)
            {
                allColliders[i].enabled = true;
                transform.tag = "Item";
            }
            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
            }
        }

        public Image imageToFill;
        public float lastTime = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (!this.enabled) return;

            if (other.CompareTag("Item") && other.GetComponent<ItemScript>() != null && other.GetComponent<ItemScript>().itemType == ItemType.Box && itemType == ItemType.Bin)
            {
                // Let's send it to Bin
                if (other.GetComponent<BoxScript>() != null && other.GetComponent<BoxScript>().rigidbody.isKinematic == false)
                {
                    eventToInvokeWhenInteract.Invoke();
                    if (other.GetComponent<ItemScript>().Name == "Box")
                    {
                        if (GetComponent<BinScript>().Boxes[0].activeSelf)
                        {
                            GetComponent<BinScript>().Boxes[1].SetActive(true);
                        }
                        else
                        {
                            GetComponent<BinScript>().Boxes[0].SetActive(true);
                        }
                    }
                    else if (other.GetComponent<ItemScript>().Name == "Plastic Barrel Trash")
                    {
                        if (GetComponent<BinScript>().trashBags[0].activeSelf)
                        {
                            GetComponent<BinScript>().trashBags[1].SetActive(true);
                        }
                        else
                        {
                            GetComponent<BinScript>().trashBags[0].SetActive(true);
                        }
                    }
                    Destroy(other.gameObject);
                }
            }
        }
    }

    public enum ItemType
    {
        Door,
        Key,
        Note,
        Cabinet,
        None,
        Drawer,
        Box,
        MedKit,
        Bed,
        ItemToMaintain,
        NPC,
        Shop,
        MoneyBag,
        Seller,
        Bin,
        Equipment,
        OldPaper,
        Flower,
        Food,
        Drink
    }

    public enum InteractionType
    {
        Grab,
        Sleep,
        Carry,
        Read,
        Open,
        Clean,
        Talk,
        Design,
        Sit,
        Repair,
        None,
        Follow,
        Remove,
        Interact,
        Build
    }
}