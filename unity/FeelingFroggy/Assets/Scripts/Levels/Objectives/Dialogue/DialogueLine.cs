using UnityEngine;
using UnityEngine.Events;

public class DialogueLine : MonoBehaviour
{
    public string speakerName;
    public string text;

    public GameObject facingTarget;
    public float facingSpeed = 5f;

    public float timeToDisplayOverride = -1;
    public float timeAfterDisplayOverride = -1;

    public static float timePerCharGlobal = 0.075f;
    public static float timeAfterDisplayGlobal = 4f;

    public UnityAction lineStart;
    public UnityAction lineEnd;
}