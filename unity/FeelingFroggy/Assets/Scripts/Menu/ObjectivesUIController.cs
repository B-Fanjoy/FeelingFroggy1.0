using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ObjectivesUIController : MonoBehaviour
{
    public static ObjectivesUIController Instance { get; private set; }

    [Header("UI Objects")]
    public GameObject objectivesUIContainer;
    public GameObject objectiveUIPrefab;

    [Header("Objectives")]
    public LevelObjective[] initialObjectives;

    private readonly Dictionary<IUISupportedObjective, GameObject> _objectives = new();
    private readonly List<IUISupportedObjective> _objectivesOrder = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitUI());

        GameController.Instance.GamePaused += OnGamePaused;
        GameController.Instance.GameUnpaused += OnGameUnpaused;
    }

    private void OnDestroy()
    {
        Instance = null;

        GameController.Instance.GamePaused -= OnGamePaused;
        GameController.Instance.GameUnpaused -= OnGameUnpaused;
    }

    private IEnumerator InitUI()
    {
        UpdateObjectiveContainer();

        // Wait for initial objectives to be set
        yield return null;

        // Create initial objective UI elements from objectives
        foreach (var objective in initialObjectives.OfType<IUISupportedObjective>())
        {
            InitObjective(objective);
        }

        UpdateObjectiveContainer();
    }

    public void AddObjective(IUISupportedObjective objective)
    {
        InitObjective(objective);
    }

    public void RemoveObjective(IUISupportedObjective objective)
    {
        var objectiveUIElement = _objectives[objective];

        DeleteObjective(objective, objectiveUIElement);
    }

    private void InitObjective(IUISupportedObjective objective)
    {
        var objectiveUIElement = Instantiate(objectiveUIPrefab, objectivesUIContainer.transform);

        _objectives.Add(objective, objectiveUIElement);
        _objectivesOrder.Add(objective);

        objective.ProgressUpdated += OnProgressUpdated;

        UpdateObjectiveUIElement(objective, objectiveUIElement);
    }

    private void DeleteObjective(IUISupportedObjective objective, GameObject objectiveUIElement)
    {
        _objectives.Remove(objective);
        _objectivesOrder.Remove(objective);

        objective.ProgressUpdated -= OnProgressUpdated;

        Destroy(objectiveUIElement);
    }

    private void UpdateObjectiveUIElement(IUISupportedObjective objective, GameObject objectiveUIElement)
    {
        var objectiveTextElement = objectiveUIElement.transform.Find("ObjectiveText");
        var objectiveFinishedElement = objectiveUIElement.transform.Find("Background/ObjectiveFinished");

        if (objectiveTextElement != null)
        {
            var objectiveText = objectiveTextElement.GetComponent<TextMeshProUGUI>();

            if (objectiveText != null)
            {
                objectiveText.text = objective.GetUIText();

                if (objectiveFinishedElement != null)
                {
                    objectiveText.fontStyle = objective.IsFinished ? FontStyles.Strikethrough : FontStyles.Normal;
                }
            }
        }

        if (objectiveFinishedElement != null)
        {
            objectiveFinishedElement.gameObject.SetActive(objective.IsFinished);
        }
    }

    private void UpdateObjectiveContainer()
    {
        // Show objectives container if there are any objectives and the game is not paused
        objectivesUIContainer.SetActive(_objectives.Count > 0 && !GameController.Instance.IsGamePaused);
    }

    private void OnProgressUpdated(IUISupportedObjective objective)
    {
        var objectiveUIElement = _objectives[objective];

        UpdateObjectiveUIElement(objective, objectiveUIElement);
    }

    private void OnGamePaused()
    {
        UpdateObjectiveContainer();
    }

    private void OnGameUnpaused()
    {
        UpdateObjectiveContainer();
    }
}
