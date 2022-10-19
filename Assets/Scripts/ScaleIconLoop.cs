using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleIconLoop : MonoBehaviour
{
    public float targetScale = 1.25f;
    public float duration = 0.5f;
    public Ease ease;
    public LoopType loopType;

    private void Start()
    {
        Application.targetFrameRate = 500;
        transform.DOScaleX(targetScale, duration).SetEase(ease).SetLoops(-1, loopType);
		transform.DOScaleY(targetScale, duration).SetEase(ease).SetLoops(-1, loopType);
    }
}
