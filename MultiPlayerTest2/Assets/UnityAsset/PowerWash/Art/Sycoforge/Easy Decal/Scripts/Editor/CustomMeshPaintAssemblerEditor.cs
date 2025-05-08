using PaintCore;
using PaintIn3D;
using PowerWash.Scripts.PowerWash;
using PowerWash.Scripts.PowerWash.Dirts;
using PowerWash.Scripts.PowerWash.Nozzle;
using UnityEditor;
using UnityEngine;

namespace Sycoforge.Easy_Decal.Scripts.Editor
{
	[CustomEditor(typeof(CustomMeshPaintAssembler))]
	[CanEditMultipleObjects]
	public class CustomMeshPaintAssemblerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			CustomMeshPaintAssembler customMeshPaintAssembler = (CustomMeshPaintAssembler) target;
			GameObject targetObject = customMeshPaintAssembler.gameObject;

			if (GUILayout.Button("Make Paintable"))
			{
				MakePaintable(targetObject);
				EditorUtility.SetDirty(target);
			}

			if (GUILayout.Button("Remove"))
			{
				RemovePaintable(targetObject);
			}

			if (GUILayout.Button("Make All Paintable"))
			{
				MakeAllPaintable();
				EditorUtility.SetDirty(target);
			}
		}

		private void MakePaintable(GameObject targetObject)
		{
			if (!targetObject.TryGetComponent(out Dirt _))
			{
				Dirt dirt = targetObject.AddComponent<Dirt>();
				dirt.MakePaintable();
				targetObject.AddComponent<WetSurface>();
				CwPaintableMesh cwPaintableMesh = targetObject.AddComponent<CwPaintableMesh>();
				cwPaintableMesh.UseMesh = CwMeshModel.UseMeshType.AutoSeamFix;
				CwPaintableMeshTexture cwPaintableMeshTexture = targetObject.AddComponent<CwPaintableMeshTexture>();
				CwGraduallyFade cwGraduallyFade = targetObject.AddComponent<CwGraduallyFade>();
				targetObject.AddComponent<UniqueId>();
				cwGraduallyFade.PaintableTexture = cwPaintableMeshTexture;
				cwGraduallyFade.Speed = 0.15f;

				MeshRenderer meshRenderer = targetObject.GetComponent<MeshRenderer>();
				if (meshRenderer != null)
				{
					Material material = meshRenderer.sharedMaterial;
					if (material != null)
					{
						Shader shader = material.shader;
						int propertyCount = ShaderUtil.GetPropertyCount(shader);
						for (int i = 0; i < propertyCount; i++)
						{
							if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
							{
								string texturePropertyName = ShaderUtil.GetPropertyName(shader, i);
								if (material.HasProperty(texturePropertyName))
								{
									Texture texture = material.GetTexture(texturePropertyName);
									if (texture != null)
									{
										cwPaintableMeshTexture.Slot = new CwSlot(0, texture.name);
										Debug.Log($"Первая текстура материала: {texturePropertyName}");
										break;
									}
								}
							}
						}
					}
					else
					{
						Debug.LogWarning("Материал отсутствует.");
					}
				}
				else
				{
					Debug.LogWarning("MeshRenderer отсутствует.");
				}

				int nameToLayer = LayerMask.NameToLayer("WetSurface");
				targetObject.layer = nameToLayer;
				Debug.Log(nameToLayer);

				Debug.Log("DirtComponent added!");
			}
			else
			{
				Debug.Log("DirtComponent already exists!");
			}
		}

		private void RemovePaintable(GameObject targetObject)
		{
			if (targetObject.TryGetComponent(out Dirt dirt))
			{
				DestroyImmediate(dirt);
				DestroyImmediate(targetObject.GetComponent<WetSurface>());
				DestroyImmediate(targetObject.GetComponent<CwPaintableMesh>());
				DestroyImmediate(targetObject.GetComponent<CwPaintableMeshTexture>());
				DestroyImmediate(targetObject.GetComponent<CwGraduallyFade>());
				DestroyImmediate(targetObject.GetComponent<CustomMeshPaintAssembler>());
				targetObject.layer = 0;
				Debug.Log("DirtComponent removed!");
			}
			else
			{
				Debug.Log("DirtComponent already removed!");
			}
		}

		private void MakeAllPaintable()
		{
			CustomMeshPaintAssembler[] assemblers = FindObjectsOfType<CustomMeshPaintAssembler>();
			foreach (CustomMeshPaintAssembler assembler in assemblers)
			{
				GameObject targetObject = assembler.gameObject;
				MakePaintable(targetObject);
			}
		}
	}
}