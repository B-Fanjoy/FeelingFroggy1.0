using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Collider))]
public class DialogueObjective : LevelObjective, IUISupportedObjective
{
    public static event Action<DialogueObjective> DialogueStarted;
    public static event Action<DialogueObjective> DialogueEnded;

    [Header("Objective UI Settings")]
    public string objectiveName;

    [Header("Dialogue Settings")]
    public DialogueLine[] dialogueLines;
    public float delayPerFinalLine = 3f;

    private Coroutine _currentFaceTargetCoroutine;
    private bool _skipLine;

    public bool IsInProgress { get; private set; }

    public void StartConversation()
    {
        if (!IsEnabled || IsFinished || IsInProgress)
        {
            return;
        }

        IsInProgress = true;

        DialogueStarted?.Invoke(this);

        FreezePlayer();

        StartCoroutine(RunConversation());
    }

    public void SkipLine()
    {
        _skipLine = true;
    }

    private void FreezePlayer()
    {
        PlayerController.Instance.Movement.StopPlayer();
        PlayerController.Instance.Movement.FreezePlayer(freezeGravity: false);
    }

    private void UnfreezePlayer()
    {
        PlayerController.Instance.Movement.UnfreezePlayer();
    }

    private void StartFaceTarget(GameObject target, float speed)
    {
        if (target == null)
        {
            return;
        }
        
        if (_currentFaceTargetCoroutine != null)
        {
            StopCoroutine(_currentFaceTargetCoroutine);
        }

        _currentFaceTargetCoroutine = StartCoroutine(FaceTarget(target, speed));
    }

    private IEnumerator FaceTarget(GameObject target, float speed)
    {
        var player = PlayerController.Instance.gameObject;

        while (true)
        {
            var targetRotation = Quaternion.LookRotation(target.transform.position - player.transform.position, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, speed * Time.deltaTime);

            if (Quaternion.Angle(player.transform.rotation, targetRotation) < 1f)
            {
                break;
            }

            yield return null;
        }

        _currentFaceTargetCoroutine = null;
    }

    private IEnumerator RunConversation()
    {
        var lines = new Queue<DialogueUIController.DialogueBox>();

        // Show each line
        foreach (var dialogueLine in dialogueLines)
        {
            // Invoke line start event
            dialogueLine.lineStart?.Invoke();

            // Start face target coroutine
            StartFaceTarget(dialogueLine.facingTarget, dialogueLine.facingSpeed);

            // Calc time per character
            var timePerCharacter = dialogueLine.timeToDisplayOverride > 0
                ? (dialogueLine.timeToDisplayOverride / dialogueLine.text.Length)
                : DialogueLine.timePerCharGlobal;

            // Create wait for seconds object for re-use
            var waitForSeconds = new WaitForSeconds(timePerCharacter);

            // Create dialogue box
            var dialogueBox = DialogueUIController.Instance.CreateDialogueBox(dialogueLine.speakerName);
            lines.Enqueue(dialogueBox);

            // Display each character, separated by a slight delay
            for (int i = 0; i < dialogueLine.text.Length; i++)
            {
                if (_skipLine)
                {
                    dialogueBox.AppendText(dialogueLine.text[i..]);
                    _skipLine = false;
                    break;
                }

                dialogueBox.AppendText(dialogueLine.text[i]);
                yield return waitForSeconds;
            }

            // If this is not the last line
            if (dialogueLine != dialogueLines[^1])
            {
                // Delay after line is shown
                var delayAfterLine = dialogueLine.timeAfterDisplayOverride > 0
                    ? dialogueLine.timeAfterDisplayOverride
                    : DialogueLine.timeAfterDisplayGlobal;

                var startTime = Time.time;
                while (Time.time - startTime < delayAfterLine)
                {
                    // Allow skipping delay
                    if (_skipLine)
                    {
                        _skipLine = false;
                        break;
                    }

                    yield return null;
                }
            }

            // Invoke line end event
            dialogueLine.lineEnd?.Invoke();

            // Fade out old lines
            if (lines.Count > 1)
            {
                var oldLine = lines.Dequeue();
                oldLine.FadeOut();
            }
        }

        // Set objective as finished
        SetFinished(true);

        // Set conversation as no longer in progress
        IsInProgress = false;

        // Invoke dialogue ended event
        DialogueEnded?.Invoke(this);

        // Update UI
        ProgressUpdated?.Invoke(this);

        // Unfreeze player
        UnfreezePlayer();

        // Fade out final lines
        var finalLineWait = new WaitForSeconds(delayPerFinalLine);

        // Fix look glitch
        if (_currentFaceTargetCoroutine != null)
        {
            StopCoroutine(_currentFaceTargetCoroutine);
        }

        while (lines.Count > 0)
        {
            var oldLine = lines.Dequeue();
            oldLine.FadeOut();

            yield return finalLineWait;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsInProgress)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            StartConversation();
        }
    }

    public string GetUIText()
    {
        return objectiveName;
    }

    public event Action<IUISupportedObjective> ProgressUpdated;
}