using System.Collections;
using System.Collections.Generic;
using Articy.Articy_Tutorial;
using Articy.Unity;
using Articy.Unity.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    [Header("UI")]
    // Reference to Dialog UI
    [SerializeField]
    GameObject dialogueWidget;

    // Reference to dialogue text
    [SerializeField]
    Text dialogueText;

    // Reference to speaker
    [SerializeField]
    Text dialogueSpeaker;

    [SerializeField]
    Button dialogueButton;

    [SerializeField]
    Button endDialogueButton;

    // To check if we are currently showing the dialog ui interface
    public bool DialogueActive { get; set; }

    private ArticyFlowPlayer flowPlayer;

    void Start()
    {
        flowPlayer = GetComponent<ArticyFlowPlayer>();
        dialogueButton.onClick.AddListener(ContinueDialogue);
        endDialogueButton.onClick.AddListener(CloseDialogueBox);
    }

    public void StartDialogue(IArticyObject aObject)
    {
        DialogueActive = true;
        dialogueWidget.SetActive(DialogueActive);
        flowPlayer.StartOn = aObject;
        // Set the dialogue button active again (we hide it when we reach the end in OnBranchesUpdated)
        dialogueButton.gameObject.SetActive(DialogueActive);
    }

    public void ContinueDialogue()
    {
        flowPlayer.Play();
    }

    public void CloseDialogueBox()
    {
        DialogueActive = false;
        dialogueWidget.SetActive(DialogueActive);
        // When we hide the dialogue UI, we also want to set the Close button back to an inactive state
        endDialogueButton.gameObject.SetActive(DialogueActive);
        // Last object might have an output pin containing scripts, so we execute them by telling flowPlayer to finish current paused object
        flowPlayer.FinishCurrentPausedObject();
    }

    // This is called every time the flow player reaches an object of interest
    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        //Clear data
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;
        // If we paused on an object that has a "Text" property fetch this text and present it
        var objectWithText = aObject as IObjectWithLocalizableText;
        if (objectWithText != null)
        {
            dialogueText.text = objectWithText.Text;
        }
        // If we paused on an object with a speaker name
        var objectWithSpeaker = aObject as IObjectWithSpeaker;
        if (objectWithSpeaker != null)
        {
            var speakerEntity = objectWithSpeaker.Speaker as Entity;
            if (speakerEntity != null)
            {
                dialogueSpeaker.text = speakerEntity.DisplayName;
            }
        }
    }

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        // Here we get passed a list of all branches following the current node
        bool dialogIsFinished = true;
        foreach (var branch in aBranches)
        {
            // As long as we have Dialogue Fragment following the current node, the dialogue continues
            if (branch.Target is IDialogueFragment)
            {
                dialogIsFinished = false;
            }
        }
        if (dialogIsFinished)
        {
            dialogueButton.gameObject.SetActive(false);
            endDialogueButton.gameObject.SetActive(true);
        }
    }
}
