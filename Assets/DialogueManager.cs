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
    public TMP_Text characterName;
    public Character[] characters;

    public float charsPerSecond = 30f;

    private Dialogue currentDialogue;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;


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
        }
        else
        {
            characterName.text = "";
            character.transform.localPosition = new Vector3(0, -5000, 0);
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
            dialogueText.text += fullText[i];
            AudioManager.Instance.PlaySound("AddText");
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
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
}

[System.Serializable]
public class Character
{
    public int id;
    public Sprite image;
    public string name;
}
