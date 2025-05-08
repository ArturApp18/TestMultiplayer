using UnityEngine;

namespace PowerWash.Scripts.PowerWash
{
	public class PowerWashProgressService : MonoBehaviour
	{
		public static PowerWashProgressService Instance { get; private set; }
		
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}