using System.Collections.Generic;
using PowerWash.Scripts.PowerWash;
using PowerWash.Scripts.PowerWash.Dirts;
using UnityEditor;
using UnityEngine;

namespace Sycoforge.Easy_Decal.Scripts.Editor
{
	[CustomEditor(typeof(RoomDirtAssembler))]
	[CanEditMultipleObjects]
	public class RoomDirtAssemblerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			RoomDirtAssembler roomDirtAssembler = (RoomDirtAssembler)target;
			if (GUILayout.Button("Collect Dirt"))
			{
				List<Dirt> collectedDirt = CollectComponentsInChildren<Dirt>(roomDirtAssembler.transform);
				Debug.Log($"Collected {collectedDirt.Count} Dirt components.");

				roomDirtAssembler.SetDirt(collectedDirt);
			}
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