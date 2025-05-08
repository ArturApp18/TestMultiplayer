using System.Collections;
using PowerWash.Scripts.PowerWash.Dirts;
using Steamworks.PowerWash.Scripts.Data;
using UnityEngine;

namespace PowerWash.Scripts.Data
{
	public class SaveLoadService : MonoBehaviour
	{
		private const string ProgressKey = "Progress";
		
		public static SaveLoadService Instance;
		public PlayerProgress Progress { get; private set; }
		

		[SerializeField] private DirtTracker _dirtTracker;
		
		private int _cleanedDirtCount;
		
		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);

			LoadProgressOrInitNew();
		}

		private IEnumerator Start()
		{
			yield return new WaitUntil(() => _dirtTracker.DirtComponents.Count > 0);

			foreach (PowerWash.Dirts.Dirt dirt in _dirtTracker.DirtComponents)
			{
				dirt.OnCleanedDirt += OnDirtCleaned;
				if (Progress.HasCleanedDirt(dirt))
					InitReader(dirt);
			}

			StartCoroutine(_dirtTracker.Construct());
		}

		private void InitReader(PowerWash.Dirts.Dirt dirt)
		{
			dirt.LoadProgress(Progress);
			dirt.OnCleanedDirt -= OnDirtCleaned;
			dirt.DestroyMeshCopy();
			_cleanedDirtCount++;
		}

		private void OnDirtCleaned(PowerWash.Dirts.Dirt dirt)
		{
			SaveProgress(dirt);
		}

		private void SaveProgress(PowerWash.Dirts.Dirt dirt)
		{
			dirt.UpdateProgress(Progress);
			print(dirt.name);
			print(dirt.IsCleaned);
			PlayerPrefs.SetString(ProgressKey, Progress.ToJson());
		}

		private PlayerProgress LoadProgress()
		{
			PlayerProgress playerProgress = PlayerPrefs.GetString(ProgressKey)?
				.ToDeserialized<PlayerProgress>();
			
			return playerProgress;
		}

		private void LoadProgressOrInitNew() =>
			Progress = LoadProgress() ?? NewProgress();

		private PlayerProgress NewProgress()
		{
			PlayerProgress progress = new PlayerProgress();
			return progress;
		}

		private void RemoveProgress() =>
			PlayerPrefs.DeleteKey(ProgressKey);
	}
}