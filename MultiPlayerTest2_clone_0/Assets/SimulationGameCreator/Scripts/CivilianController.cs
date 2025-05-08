using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace SimulationGameCreator
{
    public class CivilianController : MonoBehaviour
    {
        private NavMeshAgent agent;
        public string Name;
        private Animator animator;
        private bool hasPath = false;
        private Transform target;
        private float lastCheckTime = 0;
        public string[] Conversations;
        private float LastTimeSpeking = 0;

        public void SpeakwithHero()
        {
            LastTimeSpeking = Time.time;
            agent.isStopped = true;
            animator.SetTrigger("Talk" + UnityEngine.Random.Range(0, 2).ToString());
            string conversation = Conversations[Random.Range(0, Conversations.Length)];
            SpeechManager.instance.Show_Speach(conversation, Name, gameObject);
        }

        private void OnEnable()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            StartCoroutine(SetTargetAndWalk());
        }

        private void Start()
        {
            agent.enabled = false;
            transform.position = CityPointsManager.Instance.GetRandomSpawnPoint();
            agent.enabled = true;
        }

        void Update()
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            if (Time.time < LastTimeSpeking + 4)
            {
                Vector3 lookDirection = HeroPlayerScript.Instance.transform.position - transform.position;
                lookDirection.Normalize();
                agent.isStopped = true;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), 2 * Time.deltaTime);
            }
            if (hasPath == false || Time.time < LastTimeSpeking + 4) return;
            if (agent.hasPath && Time.time > lastCheckTime + 0.5f && Vector3.Distance(transform.position, target.position) < agent.stoppingDistance)
            {
                lastCheckTime = Time.time;
                hasPath = false;
                if (isGoingHome)
                {
                    agent.isStopped = true;
                    gameObject.SetActive(false);
                }
                else
                {
                    StartCoroutine(SetTargetAndWalk());
                }
            }
        }

        [HideInInspector]
        public bool isGoingHome = false;
        public void GoToHome(Transform homeDoor)
        {
            isGoingHome = true;
            hasPath = false;
            SetTargetAndWalkToHome(homeDoor);
        }

        public void OutFromHome()
        {
            gameObject.SetActive(true);
            isGoingHome = false;
            hasPath = false;
            StartCoroutine(SetTargetAndWalk());
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(SetTargetAndWalk());
            }
        }

        public void GoSomewhere()
        {
            target = CityPointsManager.Instance.GetRandomTargetPoint();
            agent.speed = Random.Range(1.1f, 1.2f);
            agent.isStopped = false;
            agent.SetDestination(target.position);
            hasPath = true;
        }

        public IEnumerator SetTargetAndWalk()
        {
            if (gameObject.activeSelf)
            {
                if (isGoingHome)
                {
                    agent.speed = Random.Range(1.1f, 1.2f);
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    hasPath = true;
                }
                else
                {
                    float time = Random.Range(3, 10);
                    agent.isStopped = true;
                    yield return new WaitForSeconds(time);
                    agent.isStopped = false;
                    target = CityPointsManager.Instance.GetRandomTargetPoint();
                    agent.speed = Random.Range(1.1f, 1.2f);
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    hasPath = true;
                }

            }
        }

        private void SetTargetAndWalkToHome(Transform home)
        {
            if (gameObject.activeSelf)
            {
                agent.isStopped = false;
                target = home;
                agent.speed = Random.Range(1.5f, 1.75f);
                agent.isStopped = false;
                agent.SetDestination(target.position);
                hasPath = true;
            }
        }
    }
}