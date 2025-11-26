using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public GameObject DialogueUI;
    public TMP_Text dialogueText;
    public Image character;
    public Image secondCharacter;
    public TMP_Text characterName;
    public Character[] characters;

    public float charsPerSecond = 30f;

    private Dialogue currentDialogue;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private float bounceHeight = 7f;       
    private float bounceDuration = 0.12f;   
    private int charsPerBounce = 5;
    private bool isBouncing = false;

    private bool tutorialDialogue = true;
    public bool workerDialogue = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void showDialoge(Dialogue actualDialogue)
    {
        DialogueUI.SetActive(true);
        currentDialogue = actualDialogue;
        GameManager.Instance.isPlaying = false;
        GameManager.Instance.UIOpen = true;
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        if (currentDialogue.ids[currentIndex] != -1)
        {
            Character currentCharacter = characters[currentDialogue.ids[currentIndex]];
            characterName.text = currentCharacter.name;
            character.sprite = currentCharacter.image;
            character.transform.localPosition = currentDialogue.positions[currentIndex];
            character.transform.localScale = new Vector3(currentDialogue.orientations[currentIndex], 1, 1);
            character.transform.localPosition -= new Vector3(0, -128, 0);
        }
        else
        {
            characterName.text = "";
            character.transform.localPosition = new Vector3(0, -5000, 0);
        }

        if (currentDialogue.secondaryIds[currentIndex] != -1)
        {
            Character currentSecondaryCharacter = characters[currentDialogue.secondaryIds[currentIndex]];
            secondCharacter.sprite = currentSecondaryCharacter.image;
            secondCharacter.transform.localPosition = currentDialogue.secondaryPositions[currentIndex];
            secondCharacter.transform.localScale = new Vector3(currentDialogue.secondaryOrientations[currentIndex], 1, 1);
            //secondCharacter.transform.localPosition -= new Vector3(0, -128, 0);
        }
        else
        {
            secondCharacter.transform.localPosition = new Vector3(0, -5000, 0);
        }
       
        typingCoroutine = StartCoroutine(TypeText(currentDialogue.lines[currentIndex]));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";

        float delay = 1f / charsPerSecond;

        for (int i = 0; i < fullText.Length; i++)
        {
            if (currentDialogue.ids[currentIndex] != -1)
            {
                if (i % charsPerBounce == 0)
                {
                    if (!isBouncing)
                    {
                        StartCoroutine(BounceOnce());
                    }
                }
            }
            dialogueText.text += fullText[i];

            AudioManager.Instance.PlaySound("AddText");
            yield return new WaitForSeconds(delay);
        }
        
        isTyping = false;
    }
    IEnumerator BounceOnce()
    {
        isBouncing = true;
        RectTransform characterTransform = character.GetComponent<RectTransform>();
        Vector2 startPos = currentDialogue.positions[currentIndex];
        Vector2 upPos = startPos + Vector2.up * bounceHeight;

        float halfDuration = bounceDuration / 2f;
        float t = 0f;

        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / halfDuration;
            characterTransform.anchoredPosition = Vector2.Lerp(startPos, upPos, lerp);
            yield return null;
        }

        t = 0f;
        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / halfDuration;
            characterTransform.anchoredPosition = Vector2.Lerp(upPos, startPos, lerp);
            yield return null;
        }

        characterTransform.anchoredPosition = currentDialogue.positions[currentIndex];
        isBouncing = false;
    }
    public void NextLine()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.text = currentDialogue.lines[currentIndex];
            isTyping = false;
        }
        else
        {
            currentIndex++;
            if (currentIndex < currentDialogue.lines.Length)
            {
                ShowCurrentLine();
            }
            else
            {
                dialogueText.text = "";
                GameManager.Instance.isPlaying = true;
                GameManager.Instance.UIOpen = false;
                DialogueUI.SetActive(false);
                if (tutorialDialogue)
                {
                    GameManager.Instance.isPlaying = false;
                    tutorialDialogue = false;
                    GameManager.Instance.addGold(50);
                    GlobalCustomerManager.Instance.maxCustomersInScene = 3;
                }
                if (workerDialogue)
                {
                    workerDialogue = false;
                    GameManager.Instance.addGold(100);
                    GlobalCustomerManager.Instance.maxCustomersInScene = 5;
                }
                currentIndex = 0;
            }
        }
    }
}

[System.Serializable]
public class Dialogue
{
    public int[] ids;
    public string[] lines;
    public Vector3[] positions;
    public int[] orientations;
    public int[] secondaryIds;
    public Vector3[] secondaryPositions;
    public int[] secondaryOrientations;
}

[System.Serializable]
public class Character
{
    public int id;
    public Sprite image;
    public string name;
}
