using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coineffect : MonoBehaviour
{

    [SerializeField] Vector3 targetPos;
    [SerializeField] float duration;

    void Start()
    {
        transform.DOLocalMove(targetPos, duration).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }

    void Update()
    {
        //メインカメラと同じ方向を向く
        transform.rotation = Camera.main.transform.rotation;
    }
}
