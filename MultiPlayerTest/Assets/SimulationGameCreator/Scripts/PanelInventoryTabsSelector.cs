using UnityEngine;

namespace SimulationGameCreator
{
    public class PanelInventoryTabsSelector : MonoBehaviour
    {
        public GameObject[] Tabs;
        public GameObject[] TabContents;

        public void Select(int index)
        {
            for (int i = 0; i < Tabs.Length; i++)
            {
                Tabs[i].GetComponent<UnityEngine.UI.Outline>().enabled = false;
            }
            for (int i = 0; i < TabContents.Length; i++)
            {
                TabContents[i].SetActive(false);
            }
            Tabs[index].GetComponent<UnityEngine.UI.Outline>().enabled = true;
            TabContents[index].SetActive(true);

        }
    }
}