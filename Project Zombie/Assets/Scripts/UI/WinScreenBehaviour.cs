using System.Collections;
using UnityEngine;
using TMPro;

public class WinScreenBehaviour : MonoBehaviour
{
    float heightOffset;

    float lerpDuration = 1f;
    float startPos;
    float endPos;
    float valueToLerp;

    PlayerStats targetPlayer;

    void Start() {
        heightOffset = DirectorZombieSpawner.Instance.mainCanvasTransform.gameObject.GetComponent<RectTransform>().rect.height;

        targetPlayer = FindObjectOfType<PlayerStats>();     // TODO: Change this for Multiplayer

        DisplayStats();

        StartCoroutine(PopupAnimation());
    }

    IEnumerator PopupAnimation() {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration) {
            transform.localPosition = Vector3.up * Mathf.Lerp(heightOffset, 0f, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        valueToLerp = endPos;
    }

    void DisplayStats() {
        int[] counts = { targetPlayer.CommonsKilledInMatch, targetPlayer.FastsKilledInMatch, targetPlayer.BrutesKilledInMatch };

        Transform valueGroup = transform.Find("Information Panel").Find("Group Values");
        for (int i = 0; i < valueGroup.childCount; i++) {
            valueGroup.GetChild(i).GetComponent<TextMeshProUGUI>().text = "" + counts[i];
        }
    }

    public void SendPlayerBack() {
        RoundCurrencyManager.SendPlayerBack(targetPlayer);
    }
}
