using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace Igloo
{
    /// <summary>
    /// This is an Editor script to apply the various project settings required by the Igloo System and it's various plugins
    /// A single click of the Apply Project Settings button will cause: 
    /// - Two input axis to be added to the Player->Input Settings
    /// - A shader to be added to the Player->Graphics Settings
    /// - 2 Scoped Registry packages added to the Packages Manifest
    /// - 3 nuget packages added to the Packages Manifest.
    /// A popup will then confirm the actions on completion.
    /// </summary>
    public class ApplyInputSettings : MonoBehaviour
    {
        private static bool _PleaseWaitAcknowledge, _axisXApplied, _axisYApplied, _IglooLiftApplied, _layerScreenApplied, _layerPlayerApplied, _layerCompositeQuadApplied, _NugetRegistryAdded, _NugetPackageAdded, _tagOutputCameraApplied, _KeijiroAdded, _KlakSpoutAdded, _KlakNDIAdded, _EXTOSCAdded, _EXTOSC_DependencyAdded;
        
        private static ManifestJson manifest;
        private static string manifestPath => Path.Combine(Application.dataPath, "..", "Packages/manifest.json");

        /// <summary>
        /// Called when the Apply Project Settings button is clicked in the Igloo Menu.
        /// Runs through the various functions within this class, before finally displaying the 
        /// resolution diaglog. 
        /// </summary>
        [MenuItem("Igloo/Apply Igloo Settings")]
        public static void SetupInput()
        {
            _axisXApplied = AddAxis(new InputAxis() { name = "Right Stick X Axis", dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 4 });
            _axisYApplied = AddAxis(new InputAxis() { name = "Right Stick Y Axis", invert = true, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 5 });
            _IglooLiftApplied = AddAxis(new InputAxis() { name = "IglooLift", dead = 0.2f, sensitivity = 1f, type = AxisType.KeyOrMouseButton, positiveButton = "E", negativeButton = "Q" });

            _layerScreenApplied = CreateLayer("IglooScreen");
            _layerPlayerApplied = CreateLayer("IglooPlayer");
            _layerCompositeQuadApplied = CreateLayer("IglooCompositeQuad");
            _tagOutputCameraApplied = CreateTag("WarpBlendOutputCamera");
            _KeijiroAdded = AddNewScopedRegistry("Keijiro", "https://registry.npmjs.com", new string[] { "jp.keijiro" });
            _EXTOSCAdded = AddNewScopedRegistry("package.openupm.com", "https://package.openupm.com", new string[] { "com.iam1337.extosc" });
            _NugetRegistryAdded = AddNewScopedRegistry("Unity NuGet", "https://unitynuget-registry.azurewebsites.net", new string[] { "org.nuget" });
            _KlakNDIAdded = AddNewDependency("jp.keijiro.klak.ndi", "2.1.0");
            _EXTOSC_DependencyAdded = AddNewDependency("com.iam1337.extosc", "1.20.1");
            _KlakSpoutAdded = AddNewDependency("jp.keijiro.klak.spout", "2.0.3");
            _NugetPackageAdded = AddNewDependency("org.nuget.system.memory", "4.5.5");
            if (_KlakNDIAdded || _EXTOSCAdded || _KlakNDIAdded || _KlakSpoutAdded || _EXTOSC_DependencyAdded || _NugetPackageAdded || _NugetRegistryAdded) {
                WriteChangesToManifest();
                _PleaseWaitAcknowledge = true;
            }
            OnCompleteDialog();
        }

        /// <summary>
        /// Gets the child of the serialised property name
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns>null, or the child property</returns>
        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        /// <summary>
        /// Find the Axis name in the list of player->Input axis
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns>True if axis found, false otherwise.</returns>
        private static bool AxisDefined(string axisName)
        {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                if (axis.hasChildren)
                {
                    axis.Next(true);
                    if (axis.stringValue == axisName) return true;
                }
                else return false;
            }
            return false;
        }

        /// <summary>
        /// Enum type for the Axis system
        /// </summary>
        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        /// <summary>
        /// Creates a popup dialog to inform the player what has happened
        /// </summary>
        private static void OnCompleteDialog() {
            EditorUtility.DisplayDialog("Igloo Settings Applied", 
                $"The following Igloo settings have been applied:\n - RightStick X Axis added: {_axisXApplied} \n - IglooLift axis added: {_IglooLiftApplied} \n - RightStick Y Axis added: {_axisYApplied} \n" + 
                $" - Layer IglooScreen added: {_layerScreenApplied} \n - Layer IglooPlayer added: {_layerPlayerApplied} \n - Layer Composite Quad added: {_layerCompositeQuadApplied} \n" +
                $" - Igloo Warp Blend Tag added: {_tagOutputCameraApplied} \n - Keijero Scoped Registry added: {_KeijiroAdded} \n - OSC Scoped Registry added: {_EXTOSCAdded} \n" +
                $" - Klak NDI Added: {_KlakNDIAdded} \n - Klak Spout Added: {_KlakSpoutAdded} \n - EXT OSC Added: {_EXTOSC_DependencyAdded} \n - Nuget Registry Added: {_NugetRegistryAdded} \n" + 
                $" - Nuget Package Added: {_NugetPackageAdded} \n {(_PleaseWaitAcknowledge ? "\n Please wait for the packages to download. \n It should only take a few seconds" : "")}",
                "Okay");
            if (_PleaseWaitAcknowledge) {
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
                _PleaseWaitAcknowledge = false;
            }
        }

        /// <summary>
        /// The Input Axis Structure
        /// </summary>
        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            public AxisType type;

            public int axis;
            public int joyNum;
        }

        /// <summary>
        /// Adds the given Input Axis to the ProjectSettings->Input system
        /// </summary>
        /// <param name="axis">The Input Axis to add</param>
        private static bool AddAxis(InputAxis axis)
        {
            if (AxisDefined(axis.name)) return false;

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

            serializedObject.ApplyModifiedProperties();
            return true;
        }

        /// <summary>
        /// Creates a new object layer, based on the new layer name given. 
        /// </summary>
        /// <param name="name">New Layer name</param>
        public static bool CreateLayer(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerProps = tagManager.FindProperty("layers");
            var propCount = layerProps.arraySize;

            SerializedProperty firstEmptyProp = null;

            for (var i = 0; i < propCount; i++)
            {
                var layerProp = layerProps.GetArrayElementAtIndex(i);

                var stringValue = layerProp.stringValue;

                if (stringValue == name) return false;

                if (i < 8 || stringValue != string.Empty) continue;

                if (firstEmptyProp == null)
                    firstEmptyProp = layerProp;
            }

            if (firstEmptyProp == null)
            {
                UnityEngine.Debug.LogError("<b>[Igloo]</b> Maximum limit of " + propCount + " layers exceeded. Layer \"" + name + "\" not created.");
                return false;
            }

            firstEmptyProp.stringValue = name;
            tagManager.ApplyModifiedProperties();
            return true;
        }

        /// <summary>
        /// Creates a new object tag based on the new tag name given.
        /// </summary>
        /// <param name="name">New Tag name</param>
        public static bool CreateTag(string name)
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");
            for(int i = 0; i <tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(name)) return false;
            }
            // if we get to here, we'e not found the tag so make it.
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = name;

            tagManager.ApplyModifiedProperties();
            return true;
        }

        /// <summary>
        /// Gets the complete path of the project
        /// </summary>
        /// <returns>empty string, or the complete project path.</returns>
        public static string GetProjectPath() {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) {
                if (args[i].Equals("-projectpath", System.StringComparison.InvariantCultureIgnoreCase)) return args[i + 1];
            }
            return string.Empty;
        }

        /// <summary>
        /// Creates a scoped registry and adds it to the package manager using hte AddScopedRegistry 
        /// if CheckScopedRegistry() returns false it will add the new scoped registry and return true for the 
        /// function. 
        /// Otherwise if CheckScopedRegistry() returns true [indicating that it already exists] it will return false for the function
        /// </summary>
        public static bool AddNewScopedRegistry(string registryName, string url, string[] scopes) {
            if (!CheckScopedRegistry(url)) {
                AddScopedRegistry(new ScopedRegistry {
                    name = registryName,
                    url = url,
                    scopes = scopes
                });
                return true;
            } 
            else return false;
        }

        /// <summary>
        /// Looks for a dependency existing in the manifest, then adds it. 
        /// If it does already exist, it will check the version and update it. 
        /// </summary>
        /// <param name="dependencyName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool AddNewDependency(string dependencyName, string version) {
            if (!CheckDependency(dependencyName)) {
                AddDependency(dependencyName, version);
                Debug.Log($"Adding new dependency: {dependencyName} at version {version}");
                return true;
            } else {
                CheckDependencyVersionAndUpdate(dependencyName, version);
                Debug.Log($"Updating dependency: {dependencyName} to version {version}");
                return false;
            }
        }

        /// <summary>
        /// Loads the manifest if it hasn't already been loaded, and looks for the 
        /// dependency within it. 
        /// </summary>
        /// <param name="dependencyName"></param>
        /// <returns></returns>
        private static bool CheckDependency(string dependencyName) {
            if (manifest == null) LoadManifestCache();
            return manifest.dependencies.ContainsKey(dependencyName);
        }

        /// <summary>
        /// Adds the dependency to the manifest if it doesn't already exist.
        /// </summary>
        /// <param name="dependencyName"></param>
        /// <param name="version"></param>
        private static void AddDependency(string dependencyName, string version) {
            manifest.dependencies.Add(dependencyName, version);
        }

        /// <summary>
        /// With the dependency confimed to exist already, the list of dependencies just gets updated
        /// with the most recent version of that dependency.
        /// </summary>
        /// <param name="dependencyName"></param>
        /// <param name="version"></param>
        private static void CheckDependencyVersionAndUpdate(string dependencyName, string version) {
            manifest.dependencies[dependencyName] = version;
        }

        /// <summary>
        /// Static class to add a scoped registry to the package manager
        /// </summary>
        /// <param name="pScopeRegistry"></param>
        public static void AddScopedRegistry(ScopedRegistry pScopeRegistry) {
            if (manifest == null) LoadManifestCache();

            manifest.scopedRegistries.Add(pScopeRegistry);
        }

        public static void LoadManifestCache() {
            var manifestJson = File.ReadAllText(manifestPath);

            manifest = JsonConvert.DeserializeObject<ManifestJson>(manifestJson);
        }

        public static void WriteChangesToManifest() {
            File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        }

        /// <summary>
        /// checks the existing scoped registry for a URL match
        /// </summary>
        /// <param name="registryURL"></param>
        /// <returns></returns>
        public static bool CheckScopedRegistry(string registryURL) {
            if (manifest == null) LoadManifestCache();

            if (manifest.scopedRegistries.Find(x => x.url == registryURL) == null) {
                return false;
            } else return true;
        }

        /// <summary>
        /// Scoped Registry Object Container
        /// </summary>
        public class ScopedRegistry {
            public string name;
            public string url;
            public string[] scopes;
        }

        /// <summary>
        /// JSON Manifest Container
        /// </summary>
        public class ManifestJson {
            public Dictionary<string, string> dependencies = new Dictionary<string, string>();

            public List<ScopedRegistry> scopedRegistries = new List<ScopedRegistry>();
        }
    }
}

