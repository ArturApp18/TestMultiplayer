using System;
using System.Collections.Generic;
using System.Linq;
using PowerWash.Dirt;
using PowerWash.Scripts.PowerWash.Dirts;
using UnityEngine;

namespace PowerWash.Scripts.PowerWash
{
	public class RoomDirtAssembler : MonoBehaviour
	{
		[SerializeField] private DirtTracker _dirtTracker;
		
		private void OnValidate()
		{
			_dirtTracker ??= FindFirstObjectByType<DirtTracker>();
		}

		private void Start()
		{
			transform.SetParent(null);
		}

		public void SetDirt(List<Dirts.Dirt> collectedDirt)
		{
			_dirtTracker.SetDirty(collectedDirt);
		}
		
		public void CollectAndSetDirt()
		{
			List<Dirts.Dirt> collectedDirt = CollectComponentsInChildren<Dirts.Dirt>(transform);
			List<Dirts.Dirt> copy = new List<Dirts.Dirt>();
			
			foreach (Dirts.Dirt dirt in collectedDirt)
			{
				if (dirt.gameObject.activeInHierarchy)
					copy.Add(dirt);
			}
			
			Debug.Log($"Collected {copy.Count} Dirt components.");
			SetDirt(copy);
		}

		private List<T> CollectComponentsInChildren<T>(Transform parent) where T : Component
		{
			List<T> components = new List<T>();
			CollectComponentsInChildrenRecursive(parent, components);
			return components;
		}

		private void CollectComponentsInChildrenRecursive<T>(Transform parent, List<T> components) where T : Component
		{
			foreach (Transform child in parent)
			{
				T component = child.GetComponent<T>();
				if (component != null)
				{
					components.Add(component);
				}
				CollectComponentsInChildrenRecursive(child, components);
			}
		}
	}
}