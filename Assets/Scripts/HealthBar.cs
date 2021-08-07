using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Took script from Jason Weimann Youtube. It updates the healthbar above the ships in an nice animation.
/// It needs a good review because it doesn't work like it should - TODO.
/// </summary>
public class HealthBar : MonoBehaviour
{
    [SerializeField]
    public Image fillImage;
    [SerializeField]
    private float updateSpeedSeconds = 0.5f;

    private void Awake()
    {
        GetComponentInParent<Health>().OnHealthPctChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(float pct)
    {
        StartCoroutine(ChangeToPct(pct));
    }

    private IEnumerator ChangeToPct(float pct)
    {
        float preChangePct = fillImage.fillAmount;
        float elapsed = 0f;
        
        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);
            yield return null;
        }

        fillImage.fillAmount = pct;
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
