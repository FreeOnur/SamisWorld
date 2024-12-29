using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnExit : StateMachineBehaviour
{
    [SerializeField] private Animations animation;
    [SerializeField] private bool lockLayer;
    [SerializeField] private float crossfade = 0.2f;
    [HideInInspector] public bool cancel = false;
    [HideInInspector] public int layerIndex = -1;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cancel = false;
        PlayerMovement.instance.StartCoroutine(Wait());
        // it waits until crossfade is finished
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(stateInfo.length - crossfade);


            if (cancel) yield break;

            AnimatorBrain target = animator.GetComponent<AnimatorBrain>();
            //unlock player
            target.SetLocked(false,layerIndex);
            //play next one
            target.Play(animation,layerIndex,lockLayer,false,crossfade);
        }
    }
}
