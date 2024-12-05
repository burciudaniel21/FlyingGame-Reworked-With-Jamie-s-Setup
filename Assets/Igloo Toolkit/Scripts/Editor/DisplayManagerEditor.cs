using UnityEngine;
using UnityEditor;

namespace Igloo.Common
{
    [CustomEditor(typeof(DisplayManager))]

    public class DisplayManagerEditor : Editor
    {
        bool advanced = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DisplayManager manager = (DisplayManager)target;
            GUILayout.Space(10f);

            if (Utils.IsAdvancedMode())
            {
                if (advanced)
                {
                    if (GUILayout.Button("Hide Advanced"))
                    {
                        advanced = false;
                    }
                    if (GUILayout.Button("Generate Default Cylinder Display"))
                    {
                        manager.Setup(DefaultConfigurations.EquirectangularCylinder().DisplaySettings);
                    }
                    if (GUILayout.Button("Generate Default Full Equirectangular Display"))
                    {
                        manager.Setup(DefaultConfigurations.Equirectangular360().DisplaySettings);
                    }
                    if (GUILayout.Button("Generate Default 5 Camera Array"))
                    {
                        manager.Setup(DefaultConfigurations.CylinderFiveCamera().DisplaySettings);
                    }
                }
                else
                {
                    if (GUILayout.Button("Show Advanced"))
                    {
                        advanced = true;
                    }
                }
            }
        }
    }
}

