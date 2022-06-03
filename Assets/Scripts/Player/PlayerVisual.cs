using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerVisual : MonoBehaviour
{
    private ThirdPersonController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<ThirdPersonController>();
        controller.StartMovingEvent.AddListener(StartMoveTween);
        controller.StopMovingEvent.AddListener(StopMoveTween);
        controller.JumpEvent.AddListener(JumpTween);
        controller.LandEvent.AddListener(LandTween);
    }

    void StartMoveTween()
    {
    }

    void StopMoveTween()
    {
    }

    void JumpTween()
    {
    }

    void LandTween()
    {
    }
}
