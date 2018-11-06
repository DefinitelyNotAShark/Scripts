using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateCountdown : MonoBehaviour
{
    #region buttonAnimationValues
    [SerializeField]
    float maxScale = 2f;

    [SerializeField]
    float scaleMargin = .1f;

    [SerializeField]
    float effectTime = .5f;
    #endregion

    //this animates numbers to scale up and then down. Good for countdowns
    public IEnumerator AnimateCountdownText()
    {
        Vector3 newScale = transform.localScale;
        // Scale up on X and Y axes
        while (transform.localScale.x < maxScale - scaleMargin || transform.localScale.y < maxScale - scaleMargin)
        {
            newScale.x = Mathf.Lerp(transform.localScale.x, maxScale, effectTime);
            newScale.y = Mathf.Lerp(transform.localScale.y, maxScale, effectTime);
            transform.localScale = newScale;
            yield return null;
        }
        // Snap to scale afterward
        newScale.x = maxScale;
        newScale.y = maxScale;
        transform.localScale = newScale;
        // Then scale back down
        while (transform.localScale.x > 1f + scaleMargin || transform.localScale.y > 1f + scaleMargin)
        {
            newScale.x = Mathf.Lerp(transform.localScale.x, 1f, effectTime);
            newScale.y = Mathf.Lerp(transform.localScale.y, 1f, effectTime);
            transform.localScale = newScale;
            yield return null;
        }
        // Snap back afterward
        newScale.x = 1f;
        newScale.y = 1f;
        transform.localScale = newScale;
    }
}
