using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Slime : MonoBehaviour
{
    public GameObject h_Idle;
    public GameObject h_Attack;
    public GameObject h_Hit;
    public GameObject h_Special;
    public GameObject canvas;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public SpriteRenderer HitSprite;
    public SpriteRenderer IdleSprite;
    public SpriteRenderer AttackSprite;
    public Sprite SpecialSp;
    public Sprite IdleSp;
    public Sprite BonusSp;

    public void SlimeDie()
    {
        h_Idle.SetActive(false);
        h_Hit.SetActive(true);
        StartCoroutine(SlimeDieMotion());
    }

    IEnumerator SlimeDieMotion()
    {
        float time = 1f;

        while(time > 0f)
        {
            time -= Time.deltaTime;
            Color color = HitSprite.color;
            color.a = time;
            HitSprite.color = color;
            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }
}
