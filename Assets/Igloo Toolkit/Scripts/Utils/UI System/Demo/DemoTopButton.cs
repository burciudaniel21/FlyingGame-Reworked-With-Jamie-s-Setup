using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

namespace Michsky.UI.ModernUIPack
{
    public class DemoTopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Animator buttonAnimator;

        void Start()
        {
            buttonAnimator = this.GetComponent<Animator>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed") && !buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Normal to Hover");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed") && !buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Hover to Normal");
        }

        public async void PostCloseReset()
        {
            await Task.Delay(1000);
            buttonAnimator.Play("Pressed to Normal");
        }

    }
}
