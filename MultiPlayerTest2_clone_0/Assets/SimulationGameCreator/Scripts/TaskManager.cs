using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace SimulationGameCreator
{
    public class TaskManager : MonoBehaviour
    {
        public List<TaskItem> Tasks;
        [HideInInspector]
        public List<TaskItemUI> TaskUIs;
        public GameObject TaskPrefab;
        public GameObject Panel_Task;
        public static TaskManager Instance;
        public Sprite Sprite_Tick;
        public bool AssignTheTaskOnStart = true;
        public bool ResetOnStart = true;

        private void Awake()
        {
            Instance = this;
        }

        public void AssignTask(int taskID)
        {
            StartCoroutine(WaitAndAssign(taskID));
        }

        IEnumerator WaitAndAssign(int taskID)
        {
            yield return new WaitForSeconds(3f);
            TaskItem task = Tasks.Where(x => x.ID == taskID).FirstOrDefault();
            if (task == null) yield break;

            task.isDone = (PlayerPrefs.GetInt("Task" + task.ID.ToString(), 0) == 0 ? false : true);
            task.Amount = PlayerPrefs.GetInt("Task_Amount" + task.ID.ToString(), task.TotalAmount);
            if (task.isDone) yield break;

            var addedBefore = TaskUIs.Where(x => x.ID == task.ID).FirstOrDefault();
            if (addedBefore == null)
            {
                AudioManager.Instance.Play_ObjectiveAssigned();
                GameObject newTask = Instantiate(TaskPrefab, Panel_Task.transform);
                newTask.GetComponent<TaskItemUI>().Description.text = task.Description;
                newTask.GetComponent<TaskItemUI>().Image.sprite = task.Icon;
                newTask.GetComponent<TaskItemUI>().ID = task.ID;
                if (task.Amount > 1)
                {
                    newTask.GetComponent<TaskItemUI>().Counter_Image.gameObject.SetActive(true);
                    newTask.GetComponent<TaskItemUI>().Counter_Text.text = task.Amount.ToString();
                }
                else
                {
                    newTask.GetComponent<TaskItemUI>().Counter_Image.gameObject.SetActive(false);
                }
                TaskUIs.Add(newTask.GetComponent<TaskItemUI>());
                PlayerPrefs.SetInt("TaskAssigned" + task.ID.ToString(), 1);
                PlayerPrefs.Save();

                if (!string.IsNullOrEmpty(task.Starting_GameObject))
                {
                    GameObject obj = GameObject.Find(task.Starting_GameObject);
                    if (obj != null)
                    {
                        obj.SendMessage(task.Starting_Event_Name);
                    }
                }

                task.isAssigned = true;
                if (task.ShowMarker && !string.IsNullOrEmpty(task.MarkerName))
                {
                    GameObject markerPoint = GameObject.Find(task.MarkerName);
                    Debug.Log(markerPoint.name);
                    if (markerPoint != null)
                    {
                        TargetPointer.Instance.enabled = true;
                        TargetPointer.Instance.PointedTarget = markerPoint;
                    }
                }
            }
        }

        private void Start()
        {
            if (ResetOnStart)
            {
                for (int i = 0; i < Tasks.Count; i++)
                {
                    Tasks[i].isAssigned = false;
                    Tasks[i].isDone = false;
                    PlayerPrefs.DeleteKey("Task" + Tasks[i].ID.ToString());
                    PlayerPrefs.DeleteKey("TaskAssigned" + Tasks[i].ID.ToString());
                }
            }

            for (int i = 0; i < Tasks.Count; i++)
            {
                if (PlayerPrefs.GetInt("Task" + Tasks[i].ID.ToString(), 0) != 0)
                {
                    Tasks[i].isDone = true;
                }
                else
                {
                    Tasks[i].isDone = false;
                    int amount = PlayerPrefs.GetInt("Task_Amount" + Tasks[i].ID.ToString(), Tasks[i].TotalAmount);
                    Tasks[i].Amount = amount;
                }
                if (PlayerPrefs.GetInt("TaskAssigned" + Tasks[i].ID.ToString(), 0) != 0)
                {
                    Tasks[i].isAssigned = true;
                }
                else
                {
                    Tasks[i].isAssigned = false;
                }
            }

            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].isDone == false && Tasks[i].isAssigned)
                {
                    GameObject newTask = Instantiate(TaskPrefab, Panel_Task.transform);
                    newTask.GetComponent<TaskItemUI>().Description.text = Tasks[i].Description;
                    newTask.GetComponent<TaskItemUI>().Image.sprite = Tasks[i].Icon;
                    newTask.GetComponent<TaskItemUI>().ID = Tasks[i].ID;
                    if (Tasks[i].Amount > 1)
                    {
                        newTask.GetComponent<TaskItemUI>().Counter_Image.gameObject.SetActive(true);
                        newTask.GetComponent<TaskItemUI>().Counter_Text.text = Tasks[i].Amount.ToString();
                    }
                    else
                    {
                        newTask.GetComponent<TaskItemUI>().Counter_Image.gameObject.SetActive(false);
                    }
                    TaskUIs.Add(newTask.GetComponent<TaskItemUI>());

                    if (!string.IsNullOrEmpty(Tasks[i].Starting_GameObject))
                    {
                        GameObject obj = GameObject.Find(Tasks[i].Starting_GameObject);
                        if (obj != null)
                        {
                            obj.SendMessage(Tasks[i].Starting_Event_Name);
                        }
                    }

                    if (Tasks[i].ShowMarker && !string.IsNullOrEmpty(Tasks[i].MarkerName))
                    {
                        GameObject markerPoint = GameObject.Find(Tasks[i].MarkerName);
                        if (markerPoint != null)
                        {
                            TargetPointer.Instance.enabled = true;
                            TargetPointer.Instance.PointedTarget = markerPoint;
                        }
                    }
                }
            }

            if(AssignTheTaskOnStart)
            {
                var firstTask = Tasks.OrderBy(x => x.ID).FirstOrDefault();
                if(firstTask != null && !firstTask.isDone)
                {
                    AssignTask(firstTask.ID);
                }
            }
        }

        public void CheckTask(string objectiveName)
        {
            foreach (var taskUI in TaskUIs)
            {
                TaskItem task = Tasks.Where(x => x.ID == taskUI.ID).FirstOrDefault();
                if (task != null && task.isDone == false && task.isAssigned)
                {
                    if (task.ObjectiveName == objectiveName)
                    {
                        DoneTask(taskUI.ID);
                    }
                }
            }
        }

        public void ReverseCheckTask(string objectiveName)
        {
            foreach (var taskUI in TaskUIs)
            {
                TaskItem task = Tasks.Where(x => x.ID == taskUI.ID).FirstOrDefault();
                if (task != null && task.isDone == false && task.isAssigned)
                {
                    if (task.ObjectiveName == objectiveName)
                    {
                        UnDoneTask(taskUI.ID);
                    }
                }
            }
        }

        public void UnDoneTask(int taskID)
        {
            TaskItem task = Tasks.Where(x => x.ID == taskID).FirstOrDefault();
            var UndoneAny = Tasks.Where(x => x.ID < taskID && x.isDone == false).FirstOrDefault();

            if (task.isDone) return;
            if (UndoneAny != null) return;

            task.Amount = task.Amount + 1;
            PlayerPrefs.SetInt("Task_Amount" + task.ID.ToString(), task.Amount);
            PlayerPrefs.Save();

            foreach (var taskUI in TaskUIs)
            {
                if (taskUI.ID == task.ID)
                {
                    taskUI.Counter_Text.text = task.Amount.ToString();
                }
            }
        }

        public void DoneTask(int taskID)
        {
            TaskItem task = Tasks.Where(x => x.ID == taskID).FirstOrDefault();
            var UndoneAny = Tasks.Where(x => x.ID < taskID && x.isDone == false).FirstOrDefault();

            if (task.isDone) return;
            if (UndoneAny != null) return;

            task.Amount = task.Amount - 1;
            if (task.Amount < 0) task.Amount = 0;

            if (task.Amount == 0)
            {
                task.isDone = true;
                PlayerPrefs.SetInt("Task" + task.ID.ToString(), 1);
                PlayerPrefs.Save();
                StartCoroutine(TaskDone(taskID));
            }
            else
            {
                PlayerPrefs.SetInt("Task_Amount" + task.ID.ToString(), task.Amount);
                PlayerPrefs.Save();

                foreach (var taskUI in TaskUIs)
                {
                    if (taskUI.ID == task.ID)
                    {
                        taskUI.Counter_Text.text = task.Amount.ToString();
                    }
                }
            }
        }

        IEnumerator TaskDone(int taskID)
        {
            TaskItem task = Tasks.Where(x => x.ID == taskID).FirstOrDefault();
            TaskItemUI taskUI = TaskUIs.Where(x => x.ID == taskID).FirstOrDefault();

            if (task == null || taskUI == null) yield break;

            taskUI.Image.sprite = Sprite_Tick;
            taskUI.Description.color = Color.green;
            RemoveTarget();

            yield return new WaitForSeconds(2f);
            if (task.CoinAmount > 0)
            {
                AdvancedGameManager.Instance.Get(CollactableType.Money, task.CoinAmount);
                taskUI.gameObject.SetActive(false);
            }
            else
            {
                taskUI.gameObject.SetActive(false);
            }
            AudioManager.Instance.Play_ObjectiveCompleted();
            if (!string.IsNullOrEmpty(task.End_GameObject))
            {
                GameObject obj = GameObject.Find(task.End_GameObject);
                if (obj != null)
                {
                    obj.SendMessage(task.End_Event_Name);
                }
            }
            yield return new WaitForSeconds(1);
            if(task.NextTaskIDToAssign != 0)
            {
                AssignTask(task.NextTaskIDToAssign);
            }
        }



        public void RemoveTarget()
        {
            if (TargetPointer.Instance != null)
            {
                TargetPointer.Instance.PointedTarget = null;
            }
        }
    }
}

