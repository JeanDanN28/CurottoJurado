using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class IntroController : MonoBehaviour
{
    [System.Serializable]
    public class IntroPart
    {
        public Sprite image;
        [TextArea(3, 8)]
        public string text;
    }

    public Image fadeImage;
    public Image storyImage;
    public TMP_Text storyText;
    public TMP_Text pressKeyText;

    public List<IntroPart> parts;
    private int index = 0;

    void Start()
    {
        pressKeyText.gameObject.SetActive(false);
        fadeImage.color = new Color(0, 0, 0, 1);
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        while (index < parts.Count)
        {
            IntroPart part = parts[index];

            storyImage.sprite = part.image;
            storyText.text = part.text;

            // FADE-IN
            yield return StartCoroutine(Fade(1, 0, 1f));

            // mostrar texto de presionar tecla
            pressKeyText.gameObject.SetActive(true);

            // esperar tecla
            yield return StartCoroutine(WaitForKey());

            pressKeyText.gameObject.SetActive(false);

            // FADE-OUT
            yield return StartCoroutine(Fade(0, 1, 1f));

            index++;
        }

        LoadLevel();
    }

    IEnumerator Fade(float start, float end, float time)
    {
        float t = 0f;
        while (t < time)
        {
            float a = Mathf.Lerp(start, end, t / time);
            fadeImage.color = new Color(0, 0, 0, a);
            t += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, end);
    }

    IEnumerator WaitForKey()
    {
        while (!Input.anyKeyDown)
            yield return null;
    }

    void LoadLevel()
    {
        SceneManager.LoadScene("Level2");
    }
}
