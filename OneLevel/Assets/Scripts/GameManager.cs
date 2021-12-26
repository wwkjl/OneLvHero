using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int currentRound = 1;
    private bool canAttack = true;
    private bool isSeed = true;
    private bool canTree = false;
    private bool isTree = false;
    private bool isGrowSeed = false;

    public GameObject attackButton;
    public GameObject waitButton;
    public GameObject againButton;
    public GameObject resetButton;
    public GameObject nextButton;
    public Transform backGround;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public OneLvHero hero;
    public Slime[] slimes;
    public GrowSeed growSeed;

    private Coroutine slimeCoroutine;


    private void Start()
    {
        SoundManager.Instance.PlayBGMSound(true);
    }


    public void ChangeTitleText(string text)
    {
        titleText.text = text;
    }

    public void ChangeDescriptionText(string text)
    {
        descriptionText.text = text;
    }

    public void PressAttack()
    {
        UnactiveButtons();
        ChangeDescriptionText(slimes[currentRound-1].nameText.text + "�� ����!");

        if(canAttack)
        {
            HeroAttack(true, "");
        }
        else
        {
            switch (currentRound)
            {
                case 2:
                    HeroAttack(false, "���� �����Ǿ� �׾���...");
                    break;
                case 3:
                    HeroAttack(false, "���� ���� ������ ��������...");
                    slimeCoroutine = StartCoroutine(KindSlimeHeal());
                    break;
                case 4:
                    if (!canTree)
                    {
                        HeroAttackMiss();
                    }
                    else
                    {
                        HeroAttack(false, "���� ���� ������ �׾���...");
                    }
                    break;
                case 5:
                    HeroAttack(false, "���� ���� ������ �׾���...");
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                default:
                    break;
            }
        }

    }

    public void PressWait()
    {
        UnactiveButtons();
        ChangeDescriptionText("�ϴ��� ��Ȳ�� ���Ѻ���...");

        switch (currentRound)
        {
            case 1:
                StartCoroutine(NormalSlimeWait());
                break;
            case 2:
                if (!canAttack)
                {
                    StartCoroutine(PoisonSlimeWait());
                }
                else
                {
                    StartCoroutine(NormalSlimeWait());
                }
                break;
            case 3:
                StartCoroutine(KindSlimeWait());
                break;
            case 4:

                if(isTree)
                {
                    StartCoroutine(TreeSlimeWait());
                }
                else
                {
                    if (isSeed)
                    {
                        StartCoroutine(SeedSlimeWait());
                    }
                    else
                    {
                        StartCoroutine(LeafSlimeWait());
                    }
                }
                break;
            case 5:
                if (isGrowSeed)
                {
                    StartCoroutine(HornSlimeWait());
                }
                else
                {
                    StartCoroutine(NormalSlimeWait());
                }
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            default:
                break;
        }
    }

    public void PressAgain()
    {
        if(slimeCoroutine != null) StopCoroutine(slimeCoroutine);


        descriptionText.gameObject.SetActive(true);
        ChangeTitleText("Stage 1-" + currentRound);
        ChangeDescriptionText("����� �ൿ�� �����ϼ���!");

        slimes[currentRound - 1].transform.position = new Vector3(5, -3, 0);
        slimes[currentRound - 1].canvas.SetActive(true);
        slimes[currentRound - 1].h_Attack.SetActive(false);
        slimes[currentRound - 1].h_Hit.SetActive(false);
        slimes[currentRound - 1].IdleSprite.sprite = slimes[currentRound - 1].IdleSp;
        slimes[currentRound - 1].h_Idle.SetActive(true);

        hero.StopWave();
        hero.levelText.text = "level. 1";
        hero.h_Hit.SetActive(false);
        hero.h_Idle.SetActive(true);

        attackButton.gameObject.SetActive(true);
        waitButton.gameObject.SetActive(true);
    }

    public void PressReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PressNext()
    {
        currentRound++;

        if (currentRound == 5 && isGrowSeed)
        {
            growSeed.SeedNext();
        }
        ChangeTitleText("");

        if(currentRound >= 6)
        {
            SoundManager.Instance.PlayBGMSound(false);
            StartCoroutine(VictoryMotion());
        }
        else
        {
            StartCoroutine(NextMotion());
        }

        
    }

    IEnumerator NextMotion()
    {
        float time = 0f;

        hero.StartWave();

        while (time < 1.25f)
        {
            time += Time.deltaTime;
            backGround.position -= new Vector3(2f * Time.deltaTime, 0, 0);
            slimes[currentRound-1].transform.position -= new Vector3(8f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        canAttack = false;
        PressAgain();
    }

    IEnumerator VictoryMotion()
    {
        float time = 0f;

        hero.StartWave();

        while (time < 1.25f)
        {
            time += Time.deltaTime;
            hero.transform.position += new Vector3(4f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        hero.Victory();

        hero.h_Idle.SetActive(false);
        hero.h_Use.SetActive(true);

        ChangeTitleText("Ŭ �� ��!");

        resetButton.SetActive(true);
    }

    private void HeroAttack(bool flag, string des)
    {
        if (flag)
        {
            StartCoroutine(HeroAttackSuccessMotion());
        }
        else
        {
            StartCoroutine(HeroAttackFailMotion(des));
        }
        
    }

    private void HeroAttackMiss()
    {
        StartCoroutine(HeroAttackMissMotion());
    }

    IEnumerator HeroAttackSuccessMotion()
    {
        float time = 0f;

        yield return new WaitForSeconds(0.4f);

        hero.h_Idle.SetActive(false);
        hero.h_Attack.SetActive(true);

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            hero.transform.position
                += new Vector3(16f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        SoundManager.Instance.PlaySFXSound("Ÿ����.mp3");

        while (time > 0f)
        {
            time -= Time.deltaTime;
            hero.transform.position
                -= new Vector3(16f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        hero.h_Idle.SetActive(true);
        hero.h_Attack.SetActive(false);

        StartCoroutine(SlimesDownMotion());
    }

    IEnumerator HeroAttackFailMotion(string des)
    {
        float time = 0f;

        yield return new WaitForSeconds(0.4f);

        hero.h_Idle.SetActive(false);
        hero.h_Attack.SetActive(true);

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            hero.transform.position
                += new Vector3(16f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        SoundManager.Instance.PlaySFXSound("Ÿ����.mp3");

        while (time > 0f)
        {
            time -= Time.deltaTime;
            hero.transform.position
                -= new Vector3(16f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        hero.h_Hit.SetActive(true);
        hero.h_Attack.SetActive(false);

        ChangeDescriptionText(des);
        HeroDown();
    }

    IEnumerator HeroAttackMissMotion()
    {
        float time = 0f;

        yield return new WaitForSeconds(0.4f);

        hero.h_Idle.SetActive(false);
        hero.h_Attack.SetActive(true);

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            hero.transform.position
                += new Vector3(16f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        while (time > 0f)
        {
            time -= Time.deltaTime;
            hero.transform.position
                -= new Vector3(16f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        hero.h_Idle.SetActive(true);
        hero.h_Attack.SetActive(false);

        ChangeDescriptionText("���� �ʹ� �۾Ƽ� ��������!");

        yield return new WaitForSeconds(1f);

        ChangeDescriptionText("���ѽ������� �������� �����ƴ�!");

        while (time < 1.5f)
        {
            time += Time.deltaTime;
            slimes[currentRound - 1].transform.position
                += new Vector3(8f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        ChangeTitleText("�� ��!");
        ChangeDescriptionText("");
        nextButton.SetActive(true);
    }

    IEnumerator SlimesDownMotion()
    {
        float time = 0f;

        slimes[currentRound - 1].SlimeDie();

        while (time < 1.1f)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ChangeTitleText("�� ��!");
        ChangeDescriptionText("");
        nextButton.SetActive(true);
    }


    private void SlimesAttack()
    {
        ChangeDescriptionText(slimes[currentRound - 1].nameText.text + "�� ����!");
        StartCoroutine(SlimesAttackMotion());
    }

    IEnumerator SlimesAttackMotion()
    {
        float time = 0f;

        yield return new WaitForSeconds(0.4f);

        slimes[currentRound - 1].h_Idle.SetActive(false);
        slimes[currentRound - 1].h_Attack.SetActive(true);

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            slimes[currentRound - 1].transform.position
                -= new Vector3(18f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        SoundManager.Instance.PlaySFXSound("Ÿ����.mp3");

        while (time > 0f)
        {
            time -= Time.deltaTime;
            slimes[currentRound - 1].transform.position
                += new Vector3(18f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        slimes[currentRound - 1].h_Idle.SetActive(true);
        slimes[currentRound - 1].h_Attack.SetActive(false);

        slimes[currentRound - 1].transform.position = new Vector3(5, -3, 0);

        ChangeDescriptionText("�׾���...");
        HeroDown();
    }

    private void HeroDown()
    {
        hero.h_Idle.SetActive(false);
        hero.h_Hit.SetActive(true);
        hero.levelText.text = "level. 0";

        againButton.SetActive(true);
        resetButton.SetActive(true);
    }

    private void UnactiveButtons()
    {
        attackButton.gameObject.SetActive(false);
        waitButton.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        StartCoroutine(StartMove());
        hero.StartWave();
    }

    IEnumerator StartMove()
    {
        float time = 0f;

        while(time < 1.25f)
        {
            time += Time.deltaTime;
            hero.transform.position -= new Vector3(4.2f * Time.deltaTime, 0, 0);
            backGround.position -= new Vector3(2f * Time.deltaTime, 0, 0);
            slimes[0].transform.position -= new Vector3(8f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        descriptionText.gameObject.SetActive(true);
        ChangeTitleText("Stage 1-" + currentRound);
        ChangeDescriptionText("����� �ൿ�� �����ϼ���!");
        slimes[0].transform.position = new Vector3(5, -3, 0);
        slimes[0].canvas.SetActive(true);
        hero.StopWave();

        attackButton.gameObject.SetActive(true);
        waitButton.gameObject.SetActive(true); 
    }

    IEnumerator NormalSlimeWait()
    {
        yield return new WaitForSeconds(1.2f);
        SlimesAttack();
    }

    IEnumerator PoisonSlimeWait()
    {
        yield return new WaitForSeconds(1.2f);
        PoisonSlimeHeal();
    }

    private void PoisonSlimeHeal()
    {
        ChangeDescriptionText(slimes[currentRound - 1].nameText.text + "�� ������ ����!");
        StartCoroutine(PoisonSlimeHealMotion());
    }

    IEnumerator PoisonSlimeHealMotion()
    {
        float time = 0f;

        yield return new WaitForSeconds(0.4f);

        while (time < 1f)
        {
            time += Time.deltaTime;
            slimes[currentRound-1].transform.position += new Vector3(9f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        slimes[currentRound - 1].IdleSprite.sprite = slimes[currentRound - 1].SpecialSp;
        slimes[currentRound - 1].nameText.text = "�ǰ��� ������";
        slimes[currentRound - 1].levelText.text = "level. 1";

        while (time > 0f)
        {
            time -= Time.deltaTime;
            slimes[currentRound - 1].transform.position -= new Vector3(9f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }
        slimes[currentRound - 1].transform.position = new Vector3(5, -3, 0);
        canAttack = true;

        slimes[currentRound - 1].IdleSp = slimes[currentRound - 1].SpecialSp;
        ChangeDescriptionText("�ǰ�������!");

        yield return new WaitForSeconds(1f);

        PressAgain();
    }

    IEnumerator KindSlimeWait()
    {
        yield return new WaitForSeconds(1.2f);
        ChangeDescriptionText(slimes[currentRound - 1].nameText.text + "�� �׳� ���� �ִ�!");
        yield return new WaitForSeconds(1f);

        PressAgain();
    }

    IEnumerator KindSlimeHeal()
    {
        float time = 0f;

        yield return new WaitForSeconds(1.5f);
        slimes[currentRound - 1].IdleSprite.sprite = slimes[currentRound - 1].SpecialSp;
        yield return new WaitForSeconds(1f);

        slimes[currentRound - 1].h_Idle.SetActive(false);
        slimes[currentRound - 1].h_Attack.SetActive(true);

        while (time < 1.5f)
        {
            time += Time.deltaTime;
            slimes[currentRound - 1].transform.position
                -= new Vector3(6f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        ChangeDescriptionText("ģ���� �������� �ڽ��� ������ �������־���!");
        hero.h_Idle.SetActive(true);
        hero.h_Hit.SetActive(false);
        hero.levelText.text = "level. 1";
        slimes[currentRound - 1].levelText.text = "level. 1";

        againButton.SetActive(false);
        resetButton.SetActive(false);

        while (time > 0f)
        {
            time -= Time.deltaTime;
            slimes[currentRound - 1].transform.position
                += new Vector3(6f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        slimes[currentRound - 1].h_Idle.SetActive(true);
        slimes[currentRound - 1].h_Attack.SetActive(false);
        slimes[currentRound - 1].IdleSprite.sprite = slimes[currentRound - 1].IdleSp;
        slimes[currentRound - 1].transform.position = new Vector3(5, -3, 0);
        canAttack = true;

        PressAgain();
    }

    IEnumerator SeedSlimeWait()
    {
        float time = 0f;
        Color currentColor = Color.white;

        yield return new WaitForSeconds(1.2f);

        ChangeDescriptionText("���ѽ�������...");

        slimes[currentRound - 1].canvas.SetActive(false);
        StartCoroutine(SeedSlimeGrow());

        SoundManager.Instance.PlaySFXSound("������.mp3");

        while (time < 1f)
        {
            time += Time.deltaTime;

            currentColor = Color.Lerp(Color.white, Color.black, time);
            slimes[currentRound - 1].IdleSprite.color = currentColor;
            yield return new WaitForEndOfFrame();
        }

        slimes[currentRound - 1].IdleSprite.sprite = slimes[currentRound - 1].SpecialSp;
        slimes[currentRound - 1].IdleSp = slimes[currentRound - 1].SpecialSp;

        while (time > 0f)
        {
            time -= Time.deltaTime;

            currentColor = Color.Lerp(Color.white, Color.black, time);
            slimes[currentRound - 1].IdleSprite.color = currentColor;
            yield return new WaitForEndOfFrame();
        }

        ChangeDescriptionText("�����Ͽ� ���Ͻ������� �Ǿ���!");

        yield return new WaitForSeconds(1.2f);

        slimes[currentRound - 1].canvas.SetActive(true);
        slimes[currentRound - 1].nameText.text = "���Ͻ�����";
        slimes[currentRound - 1].levelText.text = "level. 1";
        canAttack = true;
        isSeed = false;

        PressAgain();
    }

    IEnumerator LeafSlimeWait()
    {
        float time = 0f;
        Color currentColor = Color.white;
        yield return new WaitForSeconds(1.2f);


        if(canTree)
        {
            ChangeDescriptionText("���Ͻ�������...");
            slimes[currentRound - 1].canvas.SetActive(false);

            SoundManager.Instance.PlaySFXSound("������.mp3");

            while (time < 1f)
            {
                time += Time.deltaTime;

                currentColor = Color.Lerp(Color.white, Color.black, time);
                slimes[currentRound - 1].IdleSprite.color = currentColor;
                yield return new WaitForEndOfFrame();
            }

            slimes[currentRound - 1].IdleSprite.sprite = slimes[currentRound - 1].BonusSp;
            slimes[currentRound - 1].IdleSp = slimes[currentRound - 1].BonusSp;

            while (time > 0f)
            {
                time -= Time.deltaTime;

                currentColor = Color.Lerp(Color.white, Color.black, time);
                slimes[currentRound - 1].IdleSprite.color = currentColor;
                yield return new WaitForEndOfFrame();
            }

            ChangeDescriptionText("�����Ͽ� ������������ �Ǿ���!");

            yield return new WaitForSeconds(1.2f);

            slimes[currentRound - 1].canvas.SetActive(true);
            slimes[currentRound - 1].nameText.text = "����������";
            slimes[currentRound - 1].levelText.text = "level. 2";
            canAttack = false;
            isTree = true;

            PressAgain();

        }
        else
        {
            growSeed.gameObject.SetActive(true);
            growSeed.transform.position = slimes[currentRound - 1].transform.position;

            ChangeDescriptionText("���Ͻ������� ������ �ѷȴ�!");

            while (time < 1f)
            {
                time += Time.deltaTime;
                growSeed.transform.position += new Vector3(0, 20f * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }

            growSeed.transform.position = new Vector3(0, 15.4f, 0);

            while (time > 0f)
            {
                time -= Time.deltaTime;
                growSeed.transform.position += new Vector3(0, -20f * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }

            growSeed.transform.position = new Vector3(0, -4.4f, 0);

            yield return new WaitForSeconds(0.5f);

            canTree = true;
            isGrowSeed = true;
            PressAgain();
        }

    }

    IEnumerator SeedSlimeGrow()
    {
        float time = 0f;

        while (time < 2f)
        {
            time += Time.deltaTime;

            slimes[currentRound - 1].h_Idle.transform.localScale = new Vector3
                (slimes[currentRound - 1].h_Idle.transform.localScale.x + 0.35f * Time.deltaTime,
                slimes[currentRound - 1].h_Idle.transform.localScale.y + 0.35f * Time.deltaTime, 0);

            slimes[currentRound - 1].h_Idle.transform.localPosition = new Vector3
                (0, slimes[currentRound - 1].h_Idle.transform.localPosition.y + 0.7f * Time.deltaTime, 0);
            yield return new WaitForEndOfFrame();
        }

        slimes[currentRound - 1].h_Idle.transform.localScale = Vector3.one;
        slimes[currentRound - 1].h_Idle.transform.localPosition = Vector3.zero;
    }

    IEnumerator TreeSlimeWait()
    {
        yield return new WaitForSeconds(1.2f);
        ChangeDescriptionText(slimes[currentRound - 1].nameText.text + "�� �׳� ��� ���ִ�!");
        yield return new WaitForSeconds(1f);

        PressAgain();
    }

    IEnumerator HornSlimeWait()
    {
        float time = 0f;

        ChangeDescriptionText(slimes[currentRound - 1].nameText.text + "�� ����!");

        yield return new WaitForSeconds(0.4f);

        slimes[currentRound - 1].h_Idle.SetActive(false);
        slimes[currentRound - 1].h_Attack.SetActive(true);

        while (time < 0.25f)
        {
            time += Time.deltaTime;
            slimes[currentRound - 1].transform.position
                -= new Vector3(18f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        SoundManager.Instance.PlaySFXSound("�η���.mp3");

        growSeed.SeedDie();

        slimes[currentRound - 1].IdleSprite.sprite = slimes[currentRound - 1].SpecialSp;
        slimes[currentRound - 1].IdleSp = slimes[currentRound - 1].SpecialSp;
        slimes[currentRound - 1].AttackSprite.sprite = slimes[currentRound - 1].BonusSp;
        slimes[currentRound - 1].levelText.text = "level. 1";

        while (time > 0f)
        {
            time -= Time.deltaTime;
            slimes[currentRound - 1].transform.position
                += new Vector3(18f * Time.deltaTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        slimes[currentRound - 1].h_Idle.SetActive(true);
        slimes[currentRound - 1].h_Attack.SetActive(false);

        slimes[currentRound - 1].transform.position = new Vector3(5, -3, 0);

        yield return new WaitForSeconds(0.8f);

        ChangeDescriptionText("������ ���ѽ������� ��� �¾Ҵ�!");

        yield return new WaitForSeconds(1f);

        isGrowSeed = false;
        canAttack = true;
        PressAgain();
    }
}
