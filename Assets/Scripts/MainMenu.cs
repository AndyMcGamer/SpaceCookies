using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Tweener tween;
    private void Awake()
    {
        tween = text.DOFade(0.0f, 1.75f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneController.instance.LoadScene("MainGame");
        }
    }

    private void OnDestroy()
    {
        tween.Kill();
    }
}
