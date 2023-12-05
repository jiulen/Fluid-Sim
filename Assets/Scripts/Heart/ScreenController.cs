using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenController : MonoBehaviour
{
    public static ScreenController Instance { get; private set; }

    public Image blackScreen;
    public float fadeDuration = 1.0f;
    public float pulseDuration = 1.0f;
    public float pulseScale = 1.2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = blackScreen.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            blackScreen.color = color;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;
        Color color = blackScreen.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            blackScreen.color = color;
            yield return null;
        }
    }

    IEnumerator WhitePulse()
    {
       blackScreen.color = Color.white;
        while(blackScreen.color.a > 0)
        {
            blackScreen.color = Color.Lerp(blackScreen.color, new Color(0, 0, 0, 0), Time.deltaTime);
            yield return null;
        }
    }

    private void Start()
    {
        blackScreen.gameObject.SetActive(true);
        StartBlackScreen();
    }

    public void StartBlackScreen()
    {
        StartCoroutine(FadeIn());
    }

    public void EndBlackScreen()
    {
        StartCoroutine(FadeOut());
    }

    public void StartWhitePulse()
    {
        StartCoroutine(WhitePulse());
    }
}
