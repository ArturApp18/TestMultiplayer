using PaintCore;
using PaintIn3D;
using PowerWash.Nozzle;
using PowerWash.Scripts.PowerWash;
using PowerWash.Scripts.PowerWash.Dirts;
using PowerWash.Scripts.PowerWash.Nozzle;
using UnityEditor;
using UnityEngine;

namespace Sycoforge.Easy_Decal.Scripts.Editor
{
	[CustomEditor(typeof(SimpleSurfacePaintAssembler))]
	[CanEditMultipleObjects]
	public class SimpleSurfacePaintAssemblerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			SimpleSurfacePaintAssembler customMeshPaintAssembler = (SimpleSurfacePaintAssembler)target;
			GameObject targetObject = customMeshPaintAssembler.gameObject; 
			if (GUILayout.Button("Make Paintable"))
			{
				if (!targetObject.TryGetComponent(out Dirt _))
				{
					Dirt dirt = targetObject.AddComponent<Dirt>();
					CwPaintableMesh cwPaintableMesh = targetObject.AddComponent<CwPaintableMesh>();
					cwPaintableMesh.UseMesh = CwMeshModel.UseMeshType.AutoSeamFix;
					targetObject.AddComponent<CwPaintableMeshTexture>();
					CwChannelCounter cwChannelCounter = targetObject.AddComponent<CwChannelCounter>();
					targetObject.AddComponent<UniqueId>();
					int nameToLayer = LayerMask.NameToLayer("Dirt");
					targetObject.layer = nameToLayer;
					
					dirt.MakeSimpleSurface(cwChannelCounter);
					Debug.Log("DirtComponent added!");
				}
				else
				{
					Debug.Log("DirtComponent already exists!");
				}
				
				EditorUtility.SetDirty(target);
			}

			if (GUILayout.Button("Remove"))
			{
				if (targetObject.TryGetComponent(out Dirt dirt))
				{
					DestroyImmediate(dirt);
					DestroyImmediate(targetObject.GetComponent<WetSurface>());
					DestroyImmediate(targetObject.GetComponent<CwPaintableMesh>());
					DestroyImmediate(targetObject.GetComponent<CwPaintableMeshTexture>());
					DestroyImmediate(targetObject.GetComponent<CwChannelCounter>());
					DestroyImmediate(targetObject.GetComponent<SimpleSurfacePaintAssembler>());
					targetObject.layer = 0;
					Debug.Log("DirtComponent removed!");
				}
				else
				{
					Debug.Log("DirtComponent already removed!");
				}
			}
		}
	}
}