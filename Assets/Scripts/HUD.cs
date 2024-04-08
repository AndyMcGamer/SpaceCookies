using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private SlicedFilledImage hpBar;
    [SerializeField] private SlicedFilledImage batteryBar;
    [SerializeField] private SlicedFilledImage levelBar;

    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private Color normalCharge;
    [SerializeField] private Color powerCharge;

    private void Update()
    {
        hpBar.fillAmount = GameManager.instance.playerHealth / GameManager.instance.maxPlayerHealth;
        batteryBar.fillAmount = GameManager.instance.currentOverload / GameManager.instance.overloadCapacity;
        batteryBar.color = GameManager.instance.coolingDown ? powerCharge : normalCharge;
        levelBar.fillAmount = GameManager.instance.currentExp / GameManager.instance.ExpThreshold;
        levelText.text = $"LVL {GameManager.instance.currentLevel}";
    }
}
