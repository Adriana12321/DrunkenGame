using UnityEngine;
using UnityEngine.UI;

public class NpcReputationUI : MonoBehaviour
{
    public Slider reputationSlider;
    public Text reputationText;

    private static NpcReputationUI instance;

    private void Awake()
    {
        instance = this;

        // Ensure Awake is called by enabling the GameObject at load time
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            gameObject.SetActive(false); // deactivate immediately to stay hidden
        }
    }

    public static void Show(int current, int max)
    {
        if (instance == null)
        {
            Debug.LogWarning("NpcReputationUI instance is null!");
            return;
        }

        Debug.Log($"NpcReputationUI.Show() called. Score: {current}/{max}");

        instance.reputationSlider.maxValue = max;
        instance.reputationSlider.value = current;
        instance.reputationText.text = $"Reputation: {current} / {max}";
        instance.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        if (instance == null) return;
        instance.gameObject.SetActive(false);
    }

    public static void UpdateScore(int current, int max)
    {
        if (instance == null || !instance.gameObject.activeSelf) return;

        instance.reputationSlider.value = current;
        instance.reputationText.text = $"Reputation: {current} / {max}";
    }
}
