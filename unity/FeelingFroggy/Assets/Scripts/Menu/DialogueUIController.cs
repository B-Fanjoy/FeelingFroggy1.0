using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    public static DialogueUIController Instance { get; private set; }

    [Header("Required Components")]
    public RectTransform dialogueUI;
    public GameObject dialogueBoxPrefab;

    [Header("Settings")]
    public float fadeOutTime = 3f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public DialogueBox CreateDialogueBox(string speaker)
    {
        var dialogueBoxObject = Instantiate(dialogueBoxPrefab, dialogueUI.transform);

        var tmpSpeaker = dialogueBoxObject.transform.Find("Speaker").GetComponent<TextMeshProUGUI>();
        var tmpText = dialogueBoxObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        var dialogueBox = new DialogueBox(dialogueBoxObject, this, tmpSpeaker, tmpText);

        dialogueBox.SetSpeaker(speaker);
        
        FixLayout();

        return dialogueBox;
    }

    public void DestroyDialogueBox(DialogueBox dialogueBox)
    {
        Destroy(dialogueBox.GameObject);
        FixLayout();
    }

    public void FadeOutDialogueBox(DialogueBox dialogueBox)
    {
        StartCoroutine(FadeOut(dialogueBox));
    }

    private IEnumerator FadeOut(DialogueBox dialogueBox)
    {
        var startTime = Time.time;

        while (Time.time - startTime < fadeOutTime)
        {
            var alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / fadeOutTime);
            dialogueBox.Speaker.alpha = alpha;
            dialogueBox.Text.alpha = alpha;
            yield return null;
        }

        DestroyDialogueBox(dialogueBox);
    }

    private void FixLayout()
    {
        // Hacky solution for a problem where the layout doesn't update properly
        // when the text changes, dialogue boxes are added, or dialogue boxes are destroyed
        // The solution only works when it's called twice in a row
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueUI);
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueUI);
    }

    public class DialogueBox
    {
        public GameObject GameObject { get; }

        public TextMeshProUGUI Speaker { get; }
        public TextMeshProUGUI Text { get; }

        private readonly DialogueUIController _controller;

        public DialogueBox(GameObject gameObject, DialogueUIController controller, TextMeshProUGUI speaker, TextMeshProUGUI text)
        {
            GameObject = gameObject;
            _controller = controller;
            Speaker = speaker;
            Text = text;
        }

        public void SetSpeaker(string speaker)
        {
            Speaker.text = speaker;
            _controller.FixLayout();
        }

        public void SetText(string text)
        {
            Text.text = text;
            _controller.FixLayout();
        }

        public void AppendText(string text)
        {
            Text.text += text;
            _controller.FixLayout();
        }

        public void AppendText(char character)
        {
            Text.text += character;
            _controller.FixLayout();
        }

        public void FadeOut()
        {
            _controller.FadeOutDialogueBox(this);
        }

        public void Destroy()
        {
            _controller.DestroyDialogueBox(this);
        }
    }
}
