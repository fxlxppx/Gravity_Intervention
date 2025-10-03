using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHearts : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Corações na UI")]
    public Image[] hearts;

    private int maxHearts;
    private int currentHearts;

    private void Start()
    {
        maxHearts = hearts.Length;
        currentHearts = maxHearts;
        UpdateHearts(currentHearts);
    }

    public void UpdateHearts(int value)
    {
        currentHearts = Mathf.Clamp(value, 0, maxHearts);

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHearts)
                hearts[i].sprite = fullHeart;
            else
            {
                Debug.Log("Setting heart " + i + " to empty.");
                hearts[i].sprite = emptyHeart;

            }
        }
    }
}
