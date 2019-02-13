using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour {

    public TextMeshProUGUI nameText, dialogueText;
    public GameObject controller;

    private Queue<string> sentences;

	// Use this for initialization
	void Start () {
        sentences = new Queue<string>();
	}
	
	public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("started talking to " + dialogue.name);
        nameText.SetText(dialogue.name);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string curSentence = sentences.Dequeue();
        Debug.Log(curSentence);
        dialogueText.SetText(curSentence);
    }

    public void EndDialogue()
    {
        Debug.Log("end of dialogue!");
        controller.GetComponent<Controller>().DialogueOver();
    }
}
