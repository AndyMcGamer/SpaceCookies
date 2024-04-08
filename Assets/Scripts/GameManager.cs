using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("PowerupReferences")]
    [SerializeField] private CanvasGroup powerupUI;
    [SerializeField] private GameObject bulletwrap;
    [SerializeField] private GameObject bulletduration;

    [Header("Growth Settings")]
    [SerializeField] private float a = 3.0f;
    [SerializeField] private float b = 0.2f;
    [SerializeField] private float c = -1f;
    [SerializeField] private float d = 1.3f;

    [Header("Camera Settings")]
    [SerializeField] private float shakeStrength;

    [Header("Spaceship Settings")]
    public float thrust;
    public float maxThrust;
    public float playerHealth;
    public float maxPlayerHealth;
    public float fireRate;
    public float bulletSpeed;
    public float bulletDamage;
    public float bulletDuration;
    public float overloadCapacity;
    public float currentOverload;
    public float overloadRate;
    public float cooldownRate;
    public float[] levelExpReqs;
    public int currentLevel;
    public float currentExp;
    public bool coolingDown;

    [Header("Asteroid Settings")]
    public float asteroidStartingHealth = 10f;
    public float asteroidOffset = 0.135f;
    public float[] asteroidScales;
    public float[] asteroidExp;

    public Dictionary<string, bool> powerups = new()
    {
        {"Wrap", false },
        {"Bomb", false },
        {"Speed", false }
    };

    public bool increaseTimer;
    private float timeElapsed;

    public float GrowthFactor => a * Mathf.Log10(b * (timeElapsed - c)) + d > 0 ? a * Mathf.Log10(b * (timeElapsed - c)) + d : 0.001f;
    public int MaxAsteroids => (int)(1 + GrowthFactor);

    public float ExpThreshold => currentLevel >= levelExpReqs.Length ? levelExpReqs[^1] * Mathf.Pow(1.1f, currentLevel - levelExpReqs.Length + 1) : levelExpReqs[currentLevel];

    public float ScreenHeight => 2.0f * cam.orthographicSize;
    public float ScreenWidth => ScreenHeight * cam.aspect;

    public float score;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        StartGame();
    }

    public void StartGame()
    {
        timeElapsed = 0.0f;
        currentOverload = 0.0f;
        currentLevel = 1;
        currentExp = 0.0f;
        score = 0;
        playerHealth = maxPlayerHealth;
        powerupUI.alpha = 0f;
        bulletduration.SetActive(false);
        bulletwrap.SetActive(false);
        if (!PlayerPrefs.HasKey("HighScore")) PlayerPrefs.SetInt("HighScore", 0);
        if (!PlayerPrefs.HasKey("Score")) PlayerPrefs.SetInt("Score", 0);
    }

    private void Update()
    {
        if(increaseTimer) timeElapsed += Time.deltaTime;
        if(currentExp > ExpThreshold)
        {
            LevelUp();
        }
        if(currentOverload > 0)
        {
            currentOverload -= (coolingDown ? cooldownRate * 1.3f : cooldownRate) * Time.deltaTime;
        }else if(currentOverload < 0)
        {
            currentOverload = 0;
            
        }
        if(currentOverload >= overloadCapacity && !coolingDown)
        {
            coolingDown = true;
            currentOverload = overloadCapacity;
        }
        if (coolingDown && currentOverload == 0)
        {
            coolingDown = false;
        }
    }

    public void GameOver()
    {
        score += timeElapsed * 100f;
        int realScore = Mathf.RoundToInt(score);

        if(realScore > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", realScore);
        }
        PlayerPrefs.SetInt("Score", realScore);
        SceneController.instance.LoadScene("GameOver");
    }

    private void LevelUp()
    {
        currentExp -= ExpThreshold;
        score += ExpThreshold;
        currentLevel++;
        AudioManager.instance.Play("LevelUp");
        Time.timeScale = 0.0f;
        if (powerups["Wrap"])
        {
            bulletduration.SetActive(true);
        }
        else
        {
            bulletwrap.SetActive(true);
        }
        powerupUI.DOFade(1f, 0.5f).SetUpdate(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        powerupUI.DOFade(0f, 0.5f);
    }

    public void ShakeCamera(float duration)
    {
        cam.transform.DOShakePosition(duration, strength: shakeStrength);
    }

    public void BulletDamage()
    {
        bulletDamage *= 1.2f;
    }

    public void BulletSpeed()
    {
        bulletSpeed *= 1.05f;
        bulletDuration *= 1.2f;
    }

    public void FireRate()
    {
        fireRate /= 1.25f;
        overloadRate /= 1.15f;
    }

    public void BulletWrap()
    {
        powerups["Wrap"] = true;
    }

    public void Heal()
    {
        playerHealth = maxPlayerHealth;
    }
}
