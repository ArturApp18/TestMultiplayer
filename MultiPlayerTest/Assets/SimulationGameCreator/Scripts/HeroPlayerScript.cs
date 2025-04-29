using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SimulationGameCreator
{
    public class HeroPlayerScript : MonoBehaviour
    {
        public static HeroPlayerScript Instance;

        public FirstPersonController firstPersonController;
        public CharacterController characterController;
        public float Health = 100;
        [HideInInspector]
        public float TotalHealth = 100;
        public float Stamina = 1000;
        public float Fullness = 100;
        public float Hydration = 100;
        [HideInInspector]
        public float TotalStamina = 1000;
        [HideInInspector]
        public float TotalFullness = 1000;
        [HideInInspector]
        public float TotalHydration = 1000;
        public List<int> Keys_Grabbed = new List<int>();
        [HideInInspector]
        public GameObject FPSHands;

        [HideInInspector]
        public bool isHoldingBox = false;
        public GameObject MainCamera;
        private int currentKeyIdinHands = -1;
        private bool isHeroBusy = false;
        [HideInInspector]
        public float LastInteractedTime = 0;
        [HideInInspector]
        public ItemScript InteractableObject;
        [HideInInspector]
        public PhysicalEquipmentDetails GrableObject;
        public CapsuleCollider CapsuleCollider;
        public AudioSource AudioSource;

        void Start()
        {
            TotalHealth = Health;
            TotalStamina = Stamina;
            TotalFullness = Fullness;
            TotalHydration = Hydration;

            Time.timeScale = 1;
            GameCanvas.Instance.UpdateHealth();
            GameCanvas.Instance.UpdateStamina();
            InvokeRepeating("IncreaseStamina", 1, 0.25f);
            InvokeRepeating("CheckHungrinessAndThirst", 5, 10);
        }

        public void IncreaseStamina()
        {
            if (Stamina < TotalStamina && Health > 0 && !Input.GetKey(KeyCode.LeftShift))
            {
                Stamina = Stamina + 1;
                GameCanvas.Instance.UpdateStamina();
            }
        }

        public void CheckHungrinessAndThirst()
        {
            if (Health > 0 && AdvancedGameManager.Instance.HungrinessActive)
            {
                Fullness = Fullness - AdvancedGameManager.Instance.HungerSpeed;
                if (Fullness <= 0) Fullness = 0;
                GameCanvas.Instance.UpdateHungerUI();
            }
            if (Health > 0 && AdvancedGameManager.Instance.ThirstActive)
            {
                Hydration = Hydration - AdvancedGameManager.Instance.ThirstSpeed;
                if (Hydration <= 0) Hydration = 0;
                GameCanvas.Instance.UpdateThirstUI();
            }

            if (Health > 0 && (Fullness == 0 || Hydration == 0))
            {
                // We are dead!
                GetDamage(System.Convert.ToInt32(Health) + 1);
            }
        }

        public void Eat(float amount)
        {
            Fullness = Fullness + amount;
            if (Fullness > 100) Fullness = 100;
            GameCanvas.Instance.UpdateHungerUI();
        }

        public void Drink(float amount)
        {
            Hydration = Hydration + amount;
            if (Hydration > 100) Hydration = 100;
            GameCanvas.Instance.UpdateThirstUI();
        }

        public void SetHeroBusy(bool b)
        {
            isHeroBusy = b;
        }

        public bool GetHeroBusy()
        {
            return isHeroBusy;
        }

        public int GetCurrentKey()
        {
            return currentKeyIdinHands;
        }


        public void GetDamage(int Damage)
        {
            Health = Health - Damage;
            GameCanvas.Instance.Show_Blood_Effect();
            if (Health < 0) Health = 0;
            GameCanvas.Instance.UpdateHealth();
            if (Health <= 0)
            {
                firstPersonController.enabled = false;
                characterController.enabled = false;
                CameraScript.Instance.fCamShakeImpulse = 0.2f;
                GameCanvas.Instance.Show_GameOverPanel();
                HeroPlayerScript.Instance.FPSHands.SetActive(false);
            }
            else
            {
                CameraScript.Instance.fCamShakeImpulse = 0.2f;
            }
        }

        private void Awake()
        {
            Instance = this;
        }



        public void Grab_Key(int ID)
        {
            Keys_Grabbed.Add(ID);
        }

        public void Heal()
        {
            Health = Health + 50;
            if (Health > TotalHealth) Health = 100;
            GameCanvas.Instance.UpdateHealth();
        }

        public void Rest()
        {
            Stamina = Stamina + 50;
            if (Stamina > TotalStamina) Stamina = 100;
            GameCanvas.Instance.UpdateStamina();
        }

        public void ActivatePlayer()
        {
            transform.eulerAngles = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            firstPersonController.enabled = true;
            characterController.enabled = true;
            transform.eulerAngles = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }

        public void DeactivatePlayer()
        {
            firstPersonController.enabled = false;
            characterController.enabled = false;
        }

        public void ChangeTag(bool hide)
        {
            if (hide)
            {
                gameObject.tag = "Untagged";
            }
            else
            {
                gameObject.tag = "Player";
            }
        }

        public void ActivatePlayerInputs()
        {
            firstPersonController.enabled = true;
            characterController.enabled = true;
        }

        public void RemovingBox()
        {
            StartCoroutine(RemovingBoxNow());
        }

        IEnumerator RemovingBoxNow()
        {
            HeroPlayerScript.Instance.CapsuleCollider.enabled = false;
            HeroPlayerScript.Instance.characterController.enabled = false;
            yield return new WaitForSeconds(0.2f);
            HeroPlayerScript.Instance.CapsuleCollider.enabled = true;
            HeroPlayerScript.Instance.characterController.enabled = true;
        }

        public LayerMask LayersToCheck;

        public bool isButtonInteractHeld = false;

        public void PointerUp()
        {
            isButtonInteractHeld = false;
        }

        public void PointerDown()
        {
            isButtonInteractHeld = true;
        }


        public void Interact()
        {
            if (InteractableObject != null)
            {
                if (InteractableObject.interactionType == InteractionType.Clean || InteractableObject.interactionType == InteractionType.Build)
                {
                    if (InteractableObject.NeededHandType == Hand_Type.Any || InteractableObject.NeededHandType == FPSHandRotator.Instance.Current_HandType)
                    {
                        if (Time.time > InteractableObject.lastTime + 0.25f)
                        {
                            InteractableObject.lastTime = Time.time;
                            InteractableObject.imageToFill.fillAmount = InteractableObject.imageToFill.fillAmount - (1f / InteractableObject.GetComponent<ItemToMaintainScript>().durationForMaintain);
                            InteractableObject.UpdateSprite();
                            HeroPlayerScript.Instance.SetHeroBusy(true);
                            FPSHandRotator.Instance.AnimateHand(InteractableObject.interactionType);
                            if (InteractableObject.imageToFill.fillAmount <= 0)
                            {
                                InteractableObject.Interact();
                                if (FPSHandRotator.Instance.Current_HandType == Hand_Type.Build)
                                {
                                    FPSHandRotator.Instance.Switch_Hand(Hand_Type.Free);
                                }
                            }
                        }
                        InteractableObject.imageToFill.gameObject.SetActive(true);
                    }
                    else
                    {
                        GameCanvas.Instance.Show_Warning_Not(InteractableObject.NeededHandType.ToString() + " Needed!", false);
                    }
                }
                else
                {
                    if (InteractableObject.itemType == ItemType.Box && InteractableObject.GetComponent<BoxScript>().isHolding)
                    {
                        if (InteractableObject.NeededHandType == Hand_Type.Any || InteractableObject.NeededHandType == FPSHandRotator.Instance.Current_HandType)
                        {
                            InteractableObject.Interact();
                        }
                        else
                        {
                            GameCanvas.Instance.Show_Warning_Not(InteractableObject.NeededHandType.ToString() + " Needed!", false);
                        }
                    }
                    else
                    {
                        if (InteractableObject.NeededHandType == Hand_Type.Any || InteractableObject.NeededHandType == FPSHandRotator.Instance.Current_HandType)
                        {
                            if (!InteractableObject.isInteracted)
                            {
                                InteractableObject.Interact();
                                FPSHandRotator.Instance.AnimateHand(InteractableObject.interactionType);
                            }
                        }
                        else
                        {
                            GameCanvas.Instance.Show_Warning_Not(InteractableObject.NeededHandType.ToString() + " Needed!", false);
                        }
                    }
                }
            }
        }

        public void GrabBack()
        {
            GrabTheItemBackToInventory(GrableObject.GetComponent<PhysicalEquipmentDetails>());
            GameCanvas.Instance.Hide_GrabbingHint();
        }

        public void Repair()
        {
            if (FPSHandRotator.Instance.Current_HandType == Hand_Type.Repair)
            {
                EquipmentScript equipment = InventoryManager.Instance.Inventories.Where(x => x.EquipmentDetail.Name == GrableObject.Name).FirstOrDefault();
                if (equipment == null) return;
                AdvancedGameManager.Instance.UpdatePlayerValues();
                int neededMoney = equipment.EquipmentDetail.RepairingPrice;

                if (AdvancedGameManager.Instance.playerValues.Money < neededMoney)
                {
                    GameCanvas.Instance.Show_Warning_Not("Insufficent Money!", false);
                    return;
                }
                GrableObject.Fix();
                FPSHandRotator.Instance.AnimateHand(InteractionType.Repair);
                AdvancedGameManager.Instance.Spend(neededMoney);
                GameCanvas.Instance.Hide_RepareHint();
            }
            else
            {
                GameCanvas.Instance.Show_Warning_Not(Hand_Type.Repair + " Needed!", false);
            }
        }

        private void Update()
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(GameCanvas.Instance.Crosshair.transform.position);
                RaycastHit other;
                if (Physics.Raycast(ray, out other, 2, LayersToCheck) && !isHoldingBox)
                {
                    // Let's check that is it an Inventory Equipment or Not
                    if (other.collider.CompareTag("Item") && other.collider.GetComponent<PhysicalEquipmentDetails>() != null && other.collider.GetComponent<ObjectPlacing>() == null)
                    {
                        if (GrableObject == null || other.collider.GetComponent<PhysicalEquipmentDetails>() != GrableObject)
                        {
                            GrableObject = other.collider.GetComponent<PhysicalEquipmentDetails>();
                            GameCanvas.Instance.Show_GrabbingHint();
                        }
                        if (Input.GetKeyDown(AdvancedGameManager.Instance.GrabbingKey))
                        {
                            GrabTheItemBackToInventory(other.collider.GetComponent<PhysicalEquipmentDetails>());
                            GameCanvas.Instance.Hide_GrabbingHint();
                        }
                        if (GrableObject.Health < GrableObject.TotalHealth)
                        {
                            GameCanvas.Instance.Show_RepareHint();
                            if (Input.GetKeyUp(AdvancedGameManager.Instance.RepairingKey))
                            {
                                if (FPSHandRotator.Instance.Current_HandType == Hand_Type.Repair)
                                {
                                    EquipmentScript equipment = InventoryManager.Instance.Inventories.Where(x => x.EquipmentDetail.Name == GrableObject.Name).FirstOrDefault();
                                    if (equipment == null) return;
                                    AdvancedGameManager.Instance.UpdatePlayerValues();
                                    int neededMoney = equipment.EquipmentDetail.RepairingPrice;

                                    if (AdvancedGameManager.Instance.playerValues.Money < neededMoney)
                                    {
                                        GameCanvas.Instance.Show_Warning_Not("Insufficent Money!", false);
                                        return;
                                    }
                                    GrableObject.Fix();
                                    FPSHandRotator.Instance.AnimateHand(InteractionType.Repair);
                                    AdvancedGameManager.Instance.Spend(neededMoney);
                                    GameCanvas.Instance.Hide_RepareHint();
                                }
                                else
                                {
                                    GameCanvas.Instance.Show_Warning_Not(Hand_Type.Repair + " Needed!", false);
                                }
                            }
                        }
                    }

                    if ((other.collider.CompareTag("Item") || other.collider.CompareTag("Character")) && other.collider.GetComponent<ObjectPlacing>() == null)
                    {
                        var Item = other.collider.GetComponent<ItemScript>();
                        if (Item != null && Item.isInteracted) return;

                        if (Item == null || !Item.enabled) return;

                        if (InteractableObject != null && InteractableObject != Item)
                        {
                        }
                        InteractableObject = Item;

                        if (!isHoldingBox)
                        {
                            GameCanvas.Instance.Show_Warning(Item);
                        }
                    }
                }
                else
                {
                    if (GrableObject != null)
                    {
                        GrableObject = null;
                        GameCanvas.Instance.Hide_GrabbingHint();
                    }
                    if (InteractableObject != null)
                    {
                        if ((InteractableObject.CompareTag("Item") || InteractableObject.CompareTag("Character")))
                        {
                            if (!InteractableObject.enabled) return;

                            GameCanvas.Instance.Hide_Warning();
                            if ((InteractableObject.interactionType == InteractionType.Clean || InteractableObject.interactionType == InteractionType.Build) && InteractableObject.GetComponent<ItemToMaintainScript>() != null)
                            {
                                InteractableObject.imageToFill.fillAmount = 1;
                                InteractableObject.imageToFill.gameObject.SetActive(false);
                            }
                        }
                        if (!isHoldingBox)
                        {
                            InteractableObject = null;
                        }
                    }
                }

                if (InteractableObject == null)
                {
                    GameCanvas.Instance.Hide_Warning();
                }
            }
            if (InteractableObject != null)
            {
                if (InteractableObject.interactionType == InteractionType.Clean || InteractableObject.interactionType == InteractionType.Build)
                {
                    if (Input.GetKey(AdvancedGameManager.Instance.InteractingKey))
                    {
                        if (InteractableObject.NeededHandType == Hand_Type.Any || InteractableObject.NeededHandType == FPSHandRotator.Instance.Current_HandType)
                        {
                            if (Time.time > InteractableObject.lastTime + 0.25f)
                            {
                                InteractableObject.lastTime = Time.time;
                                InteractableObject.imageToFill.fillAmount = InteractableObject.imageToFill.fillAmount - (1f / InteractableObject.GetComponent<ItemToMaintainScript>().durationForMaintain);
                                InteractableObject.UpdateSprite();
                                HeroPlayerScript.Instance.SetHeroBusy(true);
                                FPSHandRotator.Instance.AnimateHand(InteractableObject.interactionType);
                                if (InteractableObject.imageToFill.fillAmount <= 0)
                                {
                                    InteractableObject.Interact();
                                    if (FPSHandRotator.Instance.Current_HandType == Hand_Type.Build)
                                    {
                                        FPSHandRotator.Instance.Switch_Hand(Hand_Type.Free);
                                    }
                                }
                            }
                            InteractableObject.imageToFill.gameObject.SetActive(true);
                        }
                        else
                        {
                            GameCanvas.Instance.Show_Warning_Not(InteractableObject.NeededHandType.ToString() + " Needed!", false);
                        }
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        InteractableObject.imageToFill.fillAmount = 1;
                        InteractableObject.imageToFill.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (Input.GetKeyUp(AdvancedGameManager.Instance.InteractingKey))
                    {
                        if (InteractableObject.itemType == ItemType.Box && InteractableObject.GetComponent<BoxScript>().isHolding)
                        {
                            if (InteractableObject.NeededHandType == Hand_Type.Any || InteractableObject.NeededHandType == FPSHandRotator.Instance.Current_HandType)
                            {
                                InteractableObject.Interact();
                            }
                            else
                            {
                                GameCanvas.Instance.Show_Warning_Not(InteractableObject.NeededHandType.ToString() + " Needed!", false);
                            }
                        }
                        else
                        {
                            if (InteractableObject.NeededHandType == Hand_Type.Any || InteractableObject.NeededHandType == FPSHandRotator.Instance.Current_HandType)
                            {
                                if (!InteractableObject.isInteracted)
                                {
                                    InteractableObject.Interact();
                                    FPSHandRotator.Instance.AnimateHand(InteractableObject.interactionType);
                                }
                            }
                            else
                            {
                                GameCanvas.Instance.Show_Warning_Not(InteractableObject.NeededHandType.ToString() + " Needed!", false);
                            }
                        }
                    }
                }
            }



            if (AdvancedGameManager.Instance.controllerType == ControllerType.Mobile && isButtonInteractHeld)
            {
                Interact();
            }
        }

        public void GrabTheItemBackToInventory(PhysicalEquipmentDetails itemDetail)
        {
            AudioManager.Instance.Play_Item_Grab();
            InventoryManager.Instance.Recover(itemDetail.Name);
            InventoryManager.Instance.CurrentEquipmentList.Remove(itemDetail.gameObject);
            InventoryManager.Instance.RemovefromSystem(itemDetail.transform, itemDetail.Name);
            TaskManager.Instance.ReverseCheckTask(itemDetail.equipmentType.ToString());
            Destroy(itemDetail.gameObject);
        }
    }
}