using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PowerWash.Dirt
{
	[DisallowMultipleComponent]
	public class DirtAwaker : MonoBehaviour
	{
		private Dictionary<string, bool> _cleanedDirt;
		
		public void SetDirt(List<Scripts.PowerWash.Dirts.Dirt> collectedDirt)
		{
			_cleanedDirt = collectedDirt.ToDictionary(x => x.UniqueId.Id, x => x.IsCleaned);
		}
	}
}