using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyText : MonoBehaviour
{
    public float duration = 2;
    public float moveY = 4;
    public Ease ease;

    void Start()
    {
        transform.DOMoveY(transform.position.y + moveY, duration).SetEase(ease);
        Destroy(gameObject, duration);
    }
}
