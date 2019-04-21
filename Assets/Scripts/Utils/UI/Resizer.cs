using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Resizer : MonoBehaviour
{

    [SerializeField]
    private float m_Duration;
    [SerializeField]
    private Ease m_EaseGrow;
    [SerializeField]
    private Ease m_EaseShrink;

    private Tweener tweener;

    [SerializeField]
    private Vector3 m_MinSize;
    [SerializeField]
    private Vector3 m_MaxSize;

    public Tweener Grow(float delay = 0)
    {
        if (tweener != null)
            tweener.Kill();

        tweener = transform.DOScale(m_MaxSize, m_Duration).SetEase(m_EaseGrow).SetDelay(delay);

        return tweener;
    }

    public Tweener Shrink(float delay = 0, float duration = 0)
    {
        if (tweener != null)
            tweener.Kill();

        tweener = transform.DOScale(m_MinSize, duration == 0 ? m_Duration : duration).SetEase(m_EaseShrink).SetDelay(delay);

        return tweener;
    }

    public void ShrinkForUnityEvents()
    {
        if (tweener != null)
            tweener.Kill();

        tweener = transform.DOScale(m_MinSize, m_Duration).SetEase(m_EaseShrink);
    }

    public void InstantShrink()
    {
        if (tweener != null)
            tweener.Kill();

        transform.localScale = m_MinSize;
    }

    public void InstantGrow()
    {
        if (tweener != null)
            tweener.Kill();

        transform.localScale = m_MaxSize;
    }

    public void ToPrincipal(float delay = 0)
    {
        Grow(0);
    }

    public void ToSecondary(float delay = 0)
    {
        Shrink(0);
    }

    public void ToPrincipalInstant()
    {
        InstantGrow();
    }

    public void ToSecondaryInstant()
    {
        InstantShrink();
    }
}