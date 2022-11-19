using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasChange : MonoBehaviour
{
    [SerializeField] private List<Sprite> canvasSpriteList = new List<Sprite>();

    [SerializeField] private Image image;
    private int index;
    private int maxIndex;
    void Start()
    {
        index = 0;
        image.sprite = canvasSpriteList[0];
        maxIndex = canvasSpriteList.Count;
    }

    [ContextMenu("NextPage")]
    public void NextPage()
    {
        index++;
        if (index < maxIndex)
        {
            image.sprite = canvasSpriteList[index];
        }
        else
        {
            index = maxIndex -1;
        }
    }

    [ContextMenu("BackPage")]
    public void BackPage()
    {
        index--;
        if (0 <= index)
        {
            image.sprite = canvasSpriteList[index];
        }
        else
        {
            index = 0;
        }
    }
}
