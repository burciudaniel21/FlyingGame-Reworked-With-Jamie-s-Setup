using UnityEngine;
using UnityEditor;
using System.Diagnostics;

namespace Igloo.Common
{
    [CustomEditor(typeof(IglooManager))]
    public class IglooManagerEditor : Editor
    {
        Texture logo;
        bool advanced = false;
        IglooManager manager;

        /// <summary>
        /// Changes the view of the Igloo Manager inspector, depending on Regular or Pro Unity.
        /// Also adds the open settings, and add spout resources button to the package. (Klak Depending)
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (logo)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(logo, GUILayout.Width(200), GUILayout.Height(108), GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
            }
            else
            {
                bool hasPro = UnityEditorInternal.InternalEditorUtility.HasPro();
                if (hasPro) logo = Resources.Load<Texture>("IglooLogoWhite");
                else logo = Resources.Load<Texture>("IglooLogoBlack");
            }

            DrawDefaultInspector();

            manager = (IglooManager)target;
            GUILayout.Space(20);
#if KLAK_SPOUT
            if(manager.spoutResources == null) {
                if (GUILayout.Button("Add Spout Resources")) {
                    FindAndAddSpoutResources();
                    EditorUtility.SetDirty(manager);
                }
                GUILayout.Space(5f);
            }
#endif
#if KLAK_NDI
            if(manager.ndiResources == null) {
                if (GUILayout.Button("Add NDI Resources")) {
                    FindAndAddNDIResources();
                    EditorUtility.SetDirty(manager);
                }
                GUILayout.Space(5f);
            }
#endif
            if (GUILayout.Button("Open Settings File"))
            {
                string path = System.IO.Path.Combine(Utils.GetDataPath(), "IglooSettings.xml");
                Process.Start(path);
            }

            GUILayout.Space(10f);

            if (Utils.IsAdvancedMode())
            {
                if (advanced)
                {
                    if (GUILayout.Button("Hide Advanced"))
                    {
                        advanced = false;
                    }
                    GUILayout.Space(10f);

                    if (GUILayout.Button("Save current settings"))
                    {
                        manager.GetAndSaveSettings();
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

        /// <summary>
        /// Looks through the packages for the Klak Spout package, and adds the SpoutResources file to
        /// the Igloo Manager system. As this file is located in the packages section, it's hard to find. 
        /// This makes it easier for users.
        /// </summary>
        private void FindAndAddSpoutResources() 
        {
#if KLAK_SPOUT
            
            manager.spoutResources = AssetDatabase.LoadAssetAtPath<Klak.Spout.SpoutResources>("Packages/jp.keijiro.klak.spout/Editor/SpoutResources.asset");
            if(manager.spoutResources == null) {
                UnityEngine.Debug.LogWarning("<b>[Igloo]</b> Could not locate Spout Resources in the Klak Spout package folder, it must be added manually.");
            } 
#endif
        }

        /// <summary>
        /// Looks through the packages for the Klak NDI package, and adds the NDIResources file to
        /// the Igloo Manager system. As this file is located in the packages section, it's hard to find. 
        /// This makes it easier for users.
        /// </summary>
        private void FindAndAddNDIResources() {
#if KLAK_NDI

            manager.ndiResources = AssetDatabase.LoadAssetAtPath<Klak.Ndi.NdiResources>("Packages/jp.keijiro.klak.ndi/Runtime/Resource/NDIResources.asset");
            if (manager.spoutResources == null) {
                UnityEngine.Debug.LogWarning("<b>[Igloo]</b> Could not locate NDI Resources in the Klak NDI package folder, it must be added manually.");
            }
#endif
        }
    }
}

