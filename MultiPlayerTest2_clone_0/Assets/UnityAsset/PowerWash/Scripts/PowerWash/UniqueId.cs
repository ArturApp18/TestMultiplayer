using System;
using UnityEngine;

namespace PowerWash.Scripts.PowerWash
{
	[DisallowMultipleComponent]
	public class UniqueId : MonoBehaviour
	{
		public string Id;

		public void GenerateId() =>
			Id = $"{gameObject.scene.name}_{Guid.NewGuid().ToString()}";
	}
}