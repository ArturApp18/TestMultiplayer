using PaintCore;
using PaintIn3D;
using PowerWash;
using PowerWash.Nozzle;
using PowerWash.Scripts.PowerWash;
using PowerWash.Scripts.PowerWash.Nozzle;
using UnityEditor;
using UnityEngine;

namespace Sycoforge.Easy_Decal.Scripts.Editor
{
    [CustomEditor(typeof(WetSurfacePaintAssembler))]
    [CanEditMultipleObjects]
    public class WetSurfacePaintAssemblerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            WetSurfacePaintAssembler customMeshPaintAssembler = (WetSurfacePaintAssembler)target;
            GameObject targetObject = customMeshPaintAssembler.gameObject;

            if (GUILayout.Button("Make Paintable"))
            {
                MakePaintable(targetObject);
            }

            if (GUILayout.Button("Remove"))
            {
                RemovePaintable(targetObject);
            }

            if (GUILayout.Button("Make All Paintable"))
            {
                MakeAllPaintable();
            }
        }

        private void MakePaintable(GameObject targetObject)
        {
            if (!targetObject.TryGetComponent(out WetSurface _))
            {
                targetObject.AddComponent<WetSurface>();
                CwPaintableMesh cwPaintableMesh = targetObject.AddComponent<CwPaintableMesh>();
                cwPaintableMesh.UseMesh = CwMeshModel.UseMeshType.AutoSeamFix;
                CwPaintableMeshTexture cwPaintableMeshTexture = targetObject.AddComponent<CwPaintableMeshTexture>();
                CwGraduallyFade cwGraduallyFade = targetObject.AddComponent<CwGraduallyFade>();
                targetObject.AddComponent<UniqueId>();
                cwGraduallyFade.PaintableTexture = cwPaintableMeshTexture;
                cwGraduallyFade.Speed = 0.15f;
                int nameToLayer = LayerMask.NameToLayer("WetSurface");
                targetObject.layer = nameToLayer;
                Debug.Log(nameToLayer);

                Debug.Log("DirtComponent added!");
            }
            else
            {
                Debug.Log("DirtComponent already exists!");
            }

            EditorUtility.SetDirty(target);
        }

        private void RemovePaintable(GameObject targetObject)
        {
            if (targetObject.TryGetComponent(out WetSurface dirt))
            {
                DestroyImmediate(dirt);
                DestroyImmediate(targetObject.GetComponent<WetSurface>());
                DestroyImmediate(targetObject.GetComponent<CwPaintableMesh>());
                DestroyImmediate(targetObject.GetComponent<CwPaintableMeshTexture>());
                DestroyImmediate(targetObject.GetComponent<CwGraduallyFade>());
                DestroyImmediate(targetObject.GetComponent<WetSurfacePaintAssembler>());
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
            WetSurfacePaintAssembler[] assemblers = FindObjectsOfType<WetSurfacePaintAssembler>();
            foreach (WetSurfacePaintAssembler assembler in assemblers)
            {
                MakePaintable(assembler.gameObject);
            }

            Debug.Log("All components made paintable!");
        }
    }
}