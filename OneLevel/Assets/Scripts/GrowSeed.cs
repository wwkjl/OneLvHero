using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowSeed : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite specialSprite;
    public Sprite dieSprite;


    public void SeedNext()
    {
        StartCoroutine(SeedNextMotion());
    }

    public void SeedDie()
    {
        spriteRenderer.sprite = dieSprite;
        StartCoroutine(SeedDieMotion());
    }


    IEnumerator SeedNextMotion()
    {
        float time = 0f;
        Color currentColor = Color.white;

        StartCoroutine(SeedGrow());

        while (time < 0.5f)
        {
            time += Time.deltaTime;

            currentColor = Color.Lerp(Color.white, Color.black, time);
            spriteRenderer.color = currentColor;
            yield return new WaitForEndOfFrame();
        }

        spriteRenderer.sprite = specialSprite;

        while (time > 0f)
        {
            time -= Time.deltaTime;

            currentColor = Color.Lerp(Color.white, Color.black, time);
            spriteRenderer.color = currentColor;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator SeedGrow()
    {
        float time = 0f;

        SoundManager.Instance.PlaySFXSound("º∫¿Â¿Ω.mp3");

        while (time < 1f)
        {
            time += Time.deltaTime;

            transform.localScale = new Vector3
                (transform.localScale.x + 0.7f * Time.deltaTime,
                transform.localScale.y + 0.7f * Time.deltaTime, 0);

            transform.localPosition = new Vector3
                (0, transform.localPosition.y + 1.4f * Time.deltaTime, 0);
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = Vector3.one;
        transform.position = new Vector3(0, -3f, 0); ;
    }

    IEnumerator SeedDieMotion()
    {
        float time = 1f;

        while (time > 0f)
        {
            time -= Time.deltaTime;
            Color color = spriteRenderer.color;
            color.a = time;
            spriteRenderer.color = color;
            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }
}
