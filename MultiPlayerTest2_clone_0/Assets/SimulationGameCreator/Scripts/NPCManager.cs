using System.Collections.Generic;
using UnityEngine;

namespace SimulationGameCreator
{
    public class NPCManager : MonoBehaviour
    {
        public List<CivilianController> Civilians;
        [HideInInspector]
        public bool SendEveryoneToHome = false;
        public static NPCManager Instance;

        public float GoingOutHour = 8;
        public float GoingHomeHour = 20;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            InvokeRepeating("CheckTimeAndDecide", 10, 10);
        }

        public void CheckTimeAndDecide()
        {
            if (DayNightManager.Instance.time > 3600 * GoingHomeHour && !SendEveryoneToHome)
            {
                foreach (var civilian in Civilians)
                {
                    civilian.GoToHome(CityPointsManager.Instance.GetRandomDoor());
                }
                SendEveryoneToHome = true;
            }
            else if (DayNightManager.Instance.time > 3600 * GoingOutHour && DayNightManager.Instance.time < 3600 * GoingHomeHour && SendEveryoneToHome)
            {
                foreach (var civilian in Civilians)
                {
                    civilian.OutFromHome();
                }
                SendEveryoneToHome = false;
            }
        }
    }
}
