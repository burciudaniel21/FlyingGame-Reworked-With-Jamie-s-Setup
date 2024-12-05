using UnityEngine;

namespace Igloo.UI
{
    public class DemoScrollManager : MonoBehaviour
    {
        [Header("RESOURCES")]
        public SettingsManager panelManager;
        public float topValue;
        public float bottomValue;
        // public string testFloat;

        public void GoToPanel(float panelValue)
        {
            panelManager.enableScrolling = false;
            panelManager.animValue = panelValue;
        }
    }
}