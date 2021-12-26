using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OneLvHero : MonoBehaviour
{
    public GameObject h_Idle;
    public GameObject h_Attack;
    public GameObject h_Hit;
    public GameObject h_Use;
    public GameObject canvas;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;

    private Coroutine waveCoroutine = null;

    public void StartWave()
    {
        canvas.SetActive(false);
        waveCoroutine = StartCoroutine(Wave());
    }

    public void StopWave()
    {
        StopCoroutine(waveCoroutine);
        canvas.SetActive(true);
        transform.position = new Vector3(-5, -3, 0);
    }

    public void Victory()
    {
        StopCoroutine(waveCoroutine);
        canvas.SetActive(false);
        transform.position = new Vector3(0, -3, 0);
    }

    IEnumerator Wave()
    {
        float time = 0f;

        while (true)
        {
            while (time < 0.2f)
            {
                time += Time.deltaTime;
                transform.position += new Vector3(0, 0.5f * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }

            while (time > 0f)
            {
                time -= Time.deltaTime;
                transform.position -= new Vector3(0, 0.5f * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
