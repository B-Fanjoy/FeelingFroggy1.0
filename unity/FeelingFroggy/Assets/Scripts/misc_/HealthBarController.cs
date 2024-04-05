using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public GameObject healthBarObject;
    public Slider hpSlider;

    private void Start()
    {
        GameController.Instance.GamePaused += UpdateHealthBarVisible;
        GameController.Instance.GameUnpaused += UpdateHealthBarVisible;
    }

    private void OnDestroy()
    {
        GameController.Instance.GamePaused -= UpdateHealthBarVisible;
        GameController.Instance.GameUnpaused -= UpdateHealthBarVisible;
    }

    public void SetHealth(float newHp)
    {
        hpSlider.value = newHp;
    }

    public void SetMaxHealth(float maxHp)
    {
        hpSlider.maxValue = maxHp;
    }

    private void UpdateHealthBarVisible()
    {
        // Show health bar when the game is not paused
        healthBarObject.SetActive(!GameController.Instance.IsGamePaused);
    }
}
