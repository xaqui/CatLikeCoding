using UnityEngine;

public class AnimationSelector : MonoBehaviour
{

    [SerializeField, Min(1)] int AnimIndex = 1;
    [SerializeField] Animator animator;


    private void Awake() {
        string animTrigger = "Anim" + AnimIndex;
        animator.SetTrigger(animTrigger);

    }




}
