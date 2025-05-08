using System;
using PowerWash.Scripts.PowerWash.Dirts;
using UnityEngine;

namespace Steamworks.PowerWash.Scripts.Data
{
	[Serializable]
	public class PlayerProgress
	{
		[SerializeField] private CleanedDirtDictionary _cleanedDirt = new CleanedDirtDictionary();
		
		public void SetClean(Dirt collectedDirt)
		{
			if (HasCleanedDirt(collectedDirt))
				return;

			_cleanedDirt.Dictionary.Add(collectedDirt.UniqueId.Id, (int)collectedDirt.InitialCleanValue);
		}

		public bool HasCleanedDirt(Dirt dirt)
		{
			Debug.Log("Dirt has cleaned: " + dirt.UniqueId);
			return _cleanedDirt.Dictionary.ContainsKey(dirt.UniqueId.Id);
		}
		
		public int GetInitialDirtAlpha(Dirt dirt)
		{
			return _cleanedDirt.Dictionary.TryGetValue(dirt.UniqueId.Id, out int initialAlpha) ? initialAlpha : 0;
		}
	}
}