using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LayoutGroupPositionFix : MonoBehaviour
{
    LayoutGroup lg;
    ContentSizeFitter csf;
    public LayoutGroupPositionFix lgpf_Parent;

    void Start()
    {
        // BECAUSE UNITY UI IS BUGGY AND NEEDS REFRESHING :P
        lg = gameObject.GetComponent<LayoutGroup>();
        csf = gameObject.GetComponent<ContentSizeFitter>();
        StartCoroutine(ExecuteAfterTime(0.01f));
    }

    public void FixPosition() {
        if (gameObject.activeInHierarchy) {
            StartCoroutine(ExecuteAfterTime(0.01f));
        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        Debug.Log("<b>[Igloo]</b> Refreshing component on " + gameObject.name);
        yield return new WaitForEndOfFrame();
        if (lg) {
            lg.enabled = false;
            yield return new WaitForEndOfFrame();
            lg.enabled = true;
        }
        
        if (csf) {
            csf.enabled = false;
            yield return new WaitForEndOfFrame();
            csf.enabled = true;
        }

        if (lgpf_Parent) lgpf_Parent.FixPosition();
        yield return null;
    }
}
