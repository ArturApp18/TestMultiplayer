using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SimulationGameCreator
{
    public class InventoryManager : MonoBehaviour
    {
        public List<PhysicalEquipmentDetails> Equipments;
        [HideInInspector]
        public List<EquipmentScript> Inventories;


        public GameObject EquipmentUIPrefab;
        public GameObject Container_Decoration;
        public GameObject Container_Poster;

        public static InventoryManager Instance;
        public LayerMask layermask;

        [HideInInspector]
        public GameObject CreatedObject;

        public GameObject Panel_LocatingInfo;
        private EquipmentScript currentEquipment;
        public GameObject shop;
        public GameObject buildParticle;
        [HideInInspector]
        public List<GameObject> CurrentEquipmentList;
        public InventoryMode inventoryMode;

        public List<ItemScript> FoodAndDrinks;
        public List<FoodDrinkConsume> Current_Owned_FoodDrinks_InGame;


        private void Awake()
        {
            Instance = this;
            CurrentEquipmentList = new List<GameObject>();
        }

        private void Start()
        {
            LoadAllObjects();
        }

        public void Buy(EquipmentScript equipment)
        {
            GameCanvas.Instance.Hide_Warning();
            AdvancedGameManager.Instance.UpdatePlayerValues();
            if (AdvancedGameManager.Instance.playerValues.Experience < equipment.EquipmentDetail.RequiredExperience)
            {
                GameCanvas.Instance.Show_Warning_Not("Insufficent Experience!", false);
                return;
            }
            if (AdvancedGameManager.Instance.playerValues.Money < equipment.EquipmentDetail.Price)
            {
                GameCanvas.Instance.Show_Warning_Not("Insufficent Money!", false);
                return;
            }
            AdvancedGameManager.Instance.Spend(equipment.EquipmentDetail.Price);
            equipment.CurrentInStorage = equipment.CurrentInStorage + 1;
            PlayerPrefs.SetInt(equipment.EquipmentDetail.Name, equipment.CurrentInStorage);
            PlayerPrefs.Save();
            GameCanvas.Instance.Show_Warning_Not("Added to Inventory!", true);
        }

        public void Recover(string name)
        {
            EquipmentScript equipment = Inventories.Where(x => x.EquipmentDetail.Name == name).FirstOrDefault();
            equipment.CurrentInStorage = equipment.CurrentInStorage + 1;
            PlayerPrefs.SetInt(equipment.EquipmentDetail.Name, equipment.CurrentInStorage);
            PlayerPrefs.Save();
            GameCanvas.Instance.Show_Warning_Not("Added to Inventory!", true);
        }

        public void Use(EquipmentScript equipment)
        {
            if (equipment.CurrentInStorage > 0)
            {
                FPSHandRotator.Instance.Switch_Hand(Hand_Type.Build);
                CreatedObject = Instantiate(equipment.EquipmentDetail.gameObject);
                CreatedObject.layer = LayerMask.NameToLayer("Hologram");
                currentPlacingScript = CreatedObject.GetComponent<ObjectPlacing>();
                currentEquipment = equipment;
                Panel_LocatingInfo.SetActive(true);
                GameCanvas.Instance.Hide_Panel_SellerShop();
                AdvancedGameManager.Instance.CurrentMode = Mode.InInventoryLocating;
            }
        }

        private ObjectPlacing currentPlacingScript;

        public float HologramOffset = 10;

        private void Update()
        {
            if (AdvancedGameManager.Instance.CurrentMode == Mode.InInventoryLocating)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(GameCanvas.Instance.Crosshair.transform.position.x, GameCanvas.Instance.Crosshair.transform.position.y - HologramOffset, GameCanvas.Instance.Crosshair.transform.position.z));

                RaycastHit[] hits = Physics.RaycastAll(ray, 6, layermask);
                foreach (RaycastHit hit in hits)
                {
                    if (!hit.collider.CompareTag("Shop") && hit.collider.gameObject != currentPlacingScript.gameObject)
                    {
                        if (currentPlacingScript.GetComponent<PhysicalEquipmentDetails>().equipmentType == EquipmentType.Poster)
                        {
                            CreatedObject.transform.position = hit.point;
                            CreatedObject.SetActive(true);
                            if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
                            {
                                CreatedObject.transform.Rotate(Input.GetAxis("Mouse ScrollWheel") * Vector3.right * 100);
                            }
                        }
                        else
                        {
                            CreatedObject.transform.position = new Vector3(hit.point.x, hit.collider.transform.position.y, hit.point.z);
                            CreatedObject.SetActive(true);
                            if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
                            {
                                CreatedObject.transform.Rotate(Input.GetAxis("Mouse ScrollWheel") * Vector3.up * 100);
                            }
                        }
                    }
                }
                if (hits.Length == 0)
                {
                    CreatedObject.SetActive(false);
                }
                else if (hits.Length == 1 && hits[0].collider.CompareTag("Shop"))
                {
                    CreatedObject.SetActive(false);
                }


                if (AdvancedGameManager.Instance.controllerType == ControllerType.PC && Input.GetMouseButtonDown(0))
                {
                    if (currentPlacingScript.CanLocate)
                    {
                        Click_Button_Locate();
                    }
                }
            }
        }

        public void Click_Button_Rotate()
        {
            if (currentPlacingScript.GetComponent<PhysicalEquipmentDetails>().equipmentType == EquipmentType.Poster)
            {
                CreatedObject.transform.Rotate(0.1f * Vector3.right * 100);
            }
            else
            {
                CreatedObject.transform.Rotate(0.1f * Vector3.up * 100);
            }
        }

        public void Click_Button_Locate()
        {
            if (currentPlacingScript.CanLocate)
            {
                AdvancedGameManager.Instance.CurrentMode = Mode.Free;
                CreatedObject.transform.parent = shop.transform;
                CreatedObject.layer = LayerMask.NameToLayer("Default");
                Instantiate(InventoryManager.Instance.buildParticle, CreatedObject.transform.position, Quaternion.identity);

                currentPlacingScript.Collider_HasTrigger_Placing.isTrigger = false;
                Destroy(currentPlacingScript.Rigidbody_Placing);
                Destroy(currentPlacingScript);

                Panel_LocatingInfo.SetActive(false);
                TaskManager.Instance.CheckTask(currentEquipment.EquipmentDetail.equipmentType.ToString());
                TaskManager.Instance.CheckTask("Build");
                FPSHandRotator.Instance.AnimateHand(InteractionType.None);
                StartCoroutine(FPSHandRotator.Instance.Switch_Hand_InTime(Hand_Type.Free, 1.5f));
                SavetoSystem(CreatedObject, currentEquipment);
                currentEquipment.DecreaseItFromInventory();
                CurrentEquipmentList.Add(CreatedObject);
                CreatedObject = null;
            }
        }

        public void CancelLocating()
        {
            AdvancedGameManager.Instance.CurrentMode = Mode.Free;
            if (CreatedObject != null)
            {
                Destroy(CreatedObject);
            }
            Panel_LocatingInfo.SetActive(false);
            CreatedObject = null;
        }

        public void SavetoSystem(GameObject newOne, EquipmentScript equipment)
        {
            int objectCount = PlayerPrefs.GetInt(equipment.EquipmentDetail.Name + "_Count", -1);
            objectCount = objectCount + 1;
            PlayerPrefs.SetInt(equipment.EquipmentDetail.Name + "_Count", objectCount);

            PlayerPrefs.SetFloat(equipment.EquipmentDetail.Name + objectCount.ToString() + "_PosX", newOne.transform.position.x);
            PlayerPrefs.SetFloat(equipment.EquipmentDetail.Name + objectCount.ToString() + "_PosY", newOne.transform.position.y);
            PlayerPrefs.SetFloat(equipment.EquipmentDetail.Name + objectCount.ToString() + "_PosZ", newOne.transform.position.z);

            PlayerPrefs.SetFloat(equipment.EquipmentDetail.Name + objectCount.ToString() + "_RotX", newOne.transform.eulerAngles.x);
            PlayerPrefs.SetFloat(equipment.EquipmentDetail.Name + objectCount.ToString() + "_RotY", newOne.transform.eulerAngles.y);
            PlayerPrefs.SetFloat(equipment.EquipmentDetail.Name + objectCount.ToString() + "_RotZ", newOne.transform.eulerAngles.z);

            PlayerPrefs.SetFloat(equipment.EquipmentDetail.Name + objectCount.ToString() + "_ScaleX", newOne.transform.localScale.x);
            PlayerPrefs.SetFloat(equipment.EquipmentDetail.Name + objectCount.ToString() + "_ScaleY", newOne.transform.localScale.y);
            PlayerPrefs.SetFloat(equipment.EquipmentDetail.Name + objectCount.ToString() + "_ScaleZ", newOne.transform.localScale.z);

            newOne.GetComponent<PhysicalEquipmentDetails>().equipmentIndex = objectCount;
            PlayerPrefs.Save();
        }

        public void RemovefromSystem(Transform oldOne, string name)
        {
            EquipmentScript equipment = Inventories.Where(x => x.EquipmentDetail.Name == name).FirstOrDefault();

            int objectCount = PlayerPrefs.GetInt(equipment.EquipmentDetail.Name + "_Count", -1);

            for (int i = 0; i < objectCount + 1; i++)
            {
                if (PlayerPrefs.GetFloat(equipment.EquipmentDetail.Name + i.ToString() + "_PosX", 0) != 0
                    && PlayerPrefs.GetFloat(equipment.EquipmentDetail.Name + i.ToString() + "_PosY", 0) != 0
                    && PlayerPrefs.GetFloat(equipment.EquipmentDetail.Name + i.ToString() + "_PosZ", 0) != 0)
                {
                    float x = PlayerPrefs.GetFloat(equipment.EquipmentDetail.Name + i.ToString() + "_PosX", 0);
                    float y = PlayerPrefs.GetFloat(equipment.EquipmentDetail.Name + i.ToString() + "_PosY", 0);
                    float z = PlayerPrefs.GetFloat(equipment.EquipmentDetail.Name + i.ToString() + "_PosZ", 0);
                    if (x == oldOne.position.x && y == oldOne.position.y && z == oldOne.position.z)
                    {
                        PlayerPrefs.DeleteKey(equipment.EquipmentDetail.Name + i.ToString() + "_PosX");
                        PlayerPrefs.DeleteKey(equipment.EquipmentDetail.Name + i.ToString() + "_PosY");
                        PlayerPrefs.DeleteKey(equipment.EquipmentDetail.Name + i.ToString() + "_PosZ");

                        PlayerPrefs.DeleteKey(equipment.EquipmentDetail.Name + i.ToString() + "_RotX");
                        PlayerPrefs.DeleteKey(equipment.EquipmentDetail.Name + i.ToString() + "_RotY");
                        PlayerPrefs.DeleteKey(equipment.EquipmentDetail.Name + i.ToString() + "_RotZ");

                        PlayerPrefs.DeleteKey(equipment.EquipmentDetail.Name + i.ToString() + "_ScaleX");
                        PlayerPrefs.DeleteKey(equipment.EquipmentDetail.Name + i.ToString() + "_ScaleY");
                        PlayerPrefs.DeleteKey(equipment.EquipmentDetail.Name + i.ToString() + "_ScaleZ");

                        PlayerPrefs.Save();
                    }
                }
            }
        }

        public void LoadAllObjects()
        {
            inventoryMode = InventoryMode.SellerShopIsOpen;
            Inventories.Clear();
            foreach (Transform child in Container_Decoration.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in Container_Poster.transform)
            {
                Destroy(child.gameObject);
            }

            // Let's load the Objects to the Inventory UI
            for (int i = 0; i < Equipments.Count; i++)
            {
                if (Equipments[i].equipmentType == EquipmentType.Decoration)
                {
                    GameObject listItem = Instantiate(EquipmentUIPrefab, Container_Decoration.transform);
                    listItem.GetComponent<EquipmentScript>().AssignDetails(Equipments[i]);
                    Inventories.Add(listItem.GetComponent<EquipmentScript>());
                }
                else if (Equipments[i].equipmentType == EquipmentType.Poster)
                {
                    GameObject listItem = Instantiate(EquipmentUIPrefab, Container_Poster.transform);
                    listItem.GetComponent<EquipmentScript>().AssignDetails(Equipments[i]);
                    Inventories.Add(listItem.GetComponent<EquipmentScript>());
                }
            }
        }

        public void LoadMyInventory()
        {
            inventoryMode = InventoryMode.PlayerInventoryIsOpen;
            foreach (var item in Inventories)
            {
                item.CurrentInStorage = PlayerPrefs.GetInt(item.EquipmentDetail.Name, 0);
                if (item.CurrentInStorage > 0)
                {
                    item.gameObject.SetActive(true);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }

                if (item.CurrentInStorage > 0)
                {
                    item.gameObject.SetActive(true);
                    item.Text_Money.text = "You have " + item.CurrentInStorage.ToString();
                    item.Text_Name.text = item.EquipmentDetail.Name.ToString();
                    item.Text_Experience.text = "";
                    item.Image_Money.sprite = item.ImageSpriteInventory;
                    item.Image_Experience.gameObject.SetActive(false);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
                item.Image.sprite = item.EquipmentDetail.ImageSprite;
            }
        }

        public void LoadBuiltObjects()
        {
            // Lets load our current Inventory
            foreach (var item in Inventories)
            {
                int objectCount = PlayerPrefs.GetInt(item.EquipmentDetail.Name + "_Count", -1);
                for (int i = 0; i < objectCount + 1; i++)
                {
                    if (PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_PosX", 0) != 0
                        && PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_PosY", 0) != 0
                        && PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_PosZ", 0) != 0)
                    {
                        Vector3 position = new Vector3(
                                                        PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_PosX", transform.position.x),
                                                        PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_PosY", transform.position.y),
                                                        PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_PosZ", transform.position.z)
                                                    );

                        Vector3 rotation = new Vector3(
                                                        PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_RotX", transform.eulerAngles.x),
                                                        PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_RotY", transform.eulerAngles.y),
                                                        PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_RotZ", transform.eulerAngles.z)
                                                    );

                        Vector3 scale = new Vector3(
                                                        PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_ScaleX", transform.localScale.x),
                                                        PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_ScaleY", transform.localScale.y),
                                                        PlayerPrefs.GetFloat(item.EquipmentDetail.Name + i.ToString() + "_ScaleZ", transform.localScale.z)
                                                    );

                        GameObject newEquipment = Instantiate(item.EquipmentDetail.gameObject, position, Quaternion.Euler(rotation), shop.transform);
                        newEquipment.layer = LayerMask.NameToLayer("Default");
                        newEquipment.GetComponent<ObjectPlacing>().Collider_HasTrigger_Placing.isTrigger = false;
                        Destroy(newEquipment.GetComponent<ObjectPlacing>().Rigidbody_Placing);
                        Destroy(newEquipment.GetComponent<ObjectPlacing>());
                        newEquipment.transform.localScale = scale;
                        newEquipment.GetComponent<PhysicalEquipmentDetails>().equipmentIndex = i;
                        newEquipment.GetComponent<PhysicalEquipmentDetails>().LoadProfile();
                        CurrentEquipmentList.Add(newEquipment);
                    }
                }
            }
        }
    }

    public enum InventoryMode
    {
        SellerShopIsOpen,
        PlayerInventoryIsOpen
    }
}