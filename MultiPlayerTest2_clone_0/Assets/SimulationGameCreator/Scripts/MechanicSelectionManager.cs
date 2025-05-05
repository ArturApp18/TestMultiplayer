using UnityEngine;

namespace SimulationGameCreator
{
    public class MechanicSelectionManager : MonoBehaviour
    {
        public GameObject Panel_MechanicSelection;
        public GameObject Button_Free;
        public GameObject Button_Build;
        public GameObject Button_Fix;
        public GameObject Button_Clean;
        public static MechanicSelectionManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void Update_FoodDrinkList()
        {
            if(AdvancedGameManager.Instance.HungrinessActive || AdvancedGameManager.Instance.ThirstActive)
            {
                foreach (Transform child in GameCanvas.Instance.Container_FoodDrink.transform)
                {
                    Destroy(child.gameObject);
                }

                // Let's load the Objects to the Inventory UI
                for (int i = 0; i < InventoryManager.Instance.FoodAndDrinks.Count; i++)
                {
                    int amount = PlayerPrefs.GetInt("Storage_" + InventoryManager.Instance.FoodAndDrinks[i].GetComponent<ItemScript>().Name, 0);
                    if (amount > 0)
                    {
                        // There are! Let's showed it on the Panel:
                        GameObject listItem = Instantiate(GameCanvas.Instance.Item_FoodDrink, GameCanvas.Instance.Container_FoodDrink.transform);
                        if (InventoryManager.Instance.FoodAndDrinks[i].GetComponent<FoodScript>() != null)
                        {
                            // This is a food!
                            listItem.GetComponent<FoodDrinkConsume>().Assign(InventoryManager.Instance.FoodAndDrinks[i].GetComponent<FoodScript>().Sprite, InventoryManager.Instance.FoodAndDrinks[i].GetComponent<ItemScript>().Name, amount);
                        }
                        else if (InventoryManager.Instance.FoodAndDrinks[i].GetComponent<DrinkScript>() != null)
                        {
                            // This is a drink!
                            listItem.GetComponent<FoodDrinkConsume>().Assign(InventoryManager.Instance.FoodAndDrinks[i].GetComponent<DrinkScript>().Sprite, InventoryManager.Instance.FoodAndDrinks[i].GetComponent<ItemScript>().Name, amount);
                        }
                        InventoryManager.Instance.Current_Owned_FoodDrinks_InGame.Add(listItem.GetComponent<FoodDrinkConsume>());
                    }
                }
            }
        }


        public void Toogle_Panel_MechanicSelectionForSure()
        {
            Panel_MechanicSelection.SetActive(false);
            GameCanvas.Instance.Panel_Inventory.SetActive(false);
            HeroPlayerScript.Instance.ActivatePlayer();
            CameraScript.Instance.enabled = true;
            if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            Button_Free.GetComponent<UnityEngine.UI.Outline>().enabled = false;
            Button_Build.GetComponent<UnityEngine.UI.Outline>().enabled = false;
            Button_Fix.GetComponent<UnityEngine.UI.Outline>().enabled = false;
            Button_Clean.GetComponent<UnityEngine.UI.Outline>().enabled = false;
        }

        public void Toogle_Panel_MechanicSelection(bool? Forceclose = null)
        {
            Update_FoodDrinkList();
            if (Forceclose == null)
            {
                if (AdvancedGameManager.Instance.CurrentMode == Mode.InMechanicSelecting)
                {
                    Panel_MechanicSelection.SetActive(false);
                    GameCanvas.Instance.Panel_Inventory.SetActive(false);
                    HeroPlayerScript.Instance.ActivatePlayer();
                    CameraScript.Instance.enabled = true;
                    if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                    AdvancedGameManager.Instance.CurrentMode = Mode.Free;
                }
                else
                {
                    InventoryManager.Instance.CancelLocating();
                    Panel_MechanicSelection.SetActive(true);
                    HeroPlayerScript.Instance.DeactivatePlayer();
                    AdvancedGameManager.Instance.CurrentMode = Mode.InMechanicSelecting;
                    CameraScript.Instance.enabled = false;
                    if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
                    {
                        Cursor.lockState = CursorLockMode.Confined;
                        Cursor.visible = true;
                    }
                }
            }
            else if (Forceclose != null && Forceclose.Value == true)
            {
                if (Panel_MechanicSelection.activeSelf)
                {
                    Panel_MechanicSelection.SetActive(false);
                    GameCanvas.Instance.Panel_Inventory.SetActive(false);
                    HeroPlayerScript.Instance.ActivatePlayer();
                    CameraScript.Instance.enabled = true;
                    if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }
            Button_Free.GetComponent<UnityEngine.UI.Outline>().enabled = false;
            Button_Build.GetComponent<UnityEngine.UI.Outline>().enabled = false;
            Button_Fix.GetComponent<UnityEngine.UI.Outline>().enabled = false;
            Button_Clean.GetComponent<UnityEngine.UI.Outline>().enabled = false;
        }

        public void Click_Select_Mechanic(int i)
        {
            switch (i)
            {
                case 1:
                    FPSHandRotator.Instance.Switch_Hand(Hand_Type.Free);
                    Toogle_Panel_MechanicSelection();
                    break;
                case 2:
                    FPSHandRotator.Instance.Switch_Hand(Hand_Type.Build);
                    Panel_MechanicSelection.SetActive(false);
                    GameCanvas.Instance.Show_Inventory();
                    break;
                case 3:
                    FPSHandRotator.Instance.Switch_Hand(Hand_Type.Repair);
                    Toogle_Panel_MechanicSelection();
                    break;
                case 4:
                    FPSHandRotator.Instance.Switch_Hand(Hand_Type.Clean);
                    Toogle_Panel_MechanicSelection();
                    break;
            }
        }
    }
}