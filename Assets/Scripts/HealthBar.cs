using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Damageable Damageable;
    public TextMeshProUGUI HealthText;
    public Image HealthRemainingBar;

    public float AdjustmentSpeed;

    private float lastShowedHealth = -1;

    public void Update()
    {
        if (lastShowedHealth < 0)
        {
            lastShowedHealth = Damageable.Health;
        }
        var targetHealth = Damageable.Health;
        lastShowedHealth = Mathf.Lerp(lastShowedHealth, targetHealth, AdjustmentSpeed);

        HealthText.text = Damageable.Health + " / " + Damageable.MaxHealth;
        HealthRemainingBar.transform.localScale = new Vector3(lastShowedHealth / Damageable.MaxHealth, 1, 1);
    }
}
