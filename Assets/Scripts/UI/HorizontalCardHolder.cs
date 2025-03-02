/*using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class HorizontalCardHolder : MonoBehaviour
{

    [SerializeField] private Card 当前选中的卡片;
    [SerializeReference] private Card 当前鼠标悬停的卡片;

    [SerializeField] private GameObject 用于存储卡片槽的预制件;
    private RectTransform RectTransform控制UI元素的位置和大小;

    [Header("Spawn Settings")]
    [SerializeField] private int 要生成的卡片数量 = 7;
    public List<Card> 列表字段用于存储所有卡片的引用;

    bool 是否正在交换位置 = false;
    [SerializeField] private bool 返回时是否使用动画 = true;

    void Start()
    {
        for (int i = 0; i < 要生成的卡片数量; i++)
        {
            Instantiate(用于存储卡片槽的预制件, transform);
        }

        RectTransform控制UI元素的位置和大小 = GetComponent<RectTransform>();
        列表字段用于存储所有卡片的引用 = GetComponentsInChildren<Card>().ToList();

        int cardCount = 0;
        //初始化一个计数器，用于给卡片命名
        foreach (Card card in 列表字段用于存储所有卡片的引用)
        {
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.name = cardCount.ToString();
            cardCount++;
        }
        //为每张卡片添加事件监听器，并给它们命名
        StartCoroutine(Frame());
        //开始一个协程，用于更新卡片的视觉索引
        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            for (int i = 0; i < 列表字段用于存储所有卡片的引用.Count; i++)
            {
                if (列表字段用于存储所有卡片的引用[i].卡片视觉处理器 != null)
                    列表字段用于存储所有卡片的引用[i].卡片视觉处理器.UpdateIndex(transform.childCount);
            }
        }
        //协程Frame，它等待0.1秒，然后更新所有卡片的视觉索引
    }

    private void BeginDrag(Card card)
    {
        当前选中的卡片 = card;
    }
    //当卡片开始被拖动时调用的方法。将当前拖动的卡片设置为选中状态。

    void EndDrag(Card card)
    {
        if (当前选中的卡片 == null)
            return;//如果没有任何卡片被选中，就结束方法。

        当前选中的卡片.transform.DOLocalMove(当前选中的卡片.被选择 ? new Vector3(0, 当前选中的卡片.selectionOffset,0) : Vector3.zero, 返回时是否使用动画 ? .15f : 0).SetEase(Ease.OutBack);

        RectTransform控制UI元素的位置和大小.sizeDelta += Vector2.right;
        RectTransform控制UI元素的位置和大小.sizeDelta -= Vector2.right;
        //这两行代码看起来是多余的，可能是用于触发布局更新。
        当前选中的卡片 = null;
        //将选中的卡片重置为null。
    }

    void CardPointerEnter(Card card)
    {
        当前鼠标悬停的卡片 = card;
    }
    //当鼠标指针进入卡片区域时调用的方法,将鼠标悬停的卡片设置为当前卡片。
    void CardPointerExit(Card card)
    {
        当前鼠标悬停的卡片 = null;
    }
    //当鼠标指针离开卡片区域时调用的方法。将鼠标悬停的卡片重置为null。
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (当前鼠标悬停的卡片 != null)
            {
                Destroy(当前鼠标悬停的卡片.transform.parent.gameObject);
                列表字段用于存储所有卡片的引用.Remove(当前鼠标悬停的卡片);

            }
        }
        //如果按下Delete键，并且有卡片被悬停，销毁该卡片的游戏对象，并从列表中移除
        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in 列表字段用于存储所有卡片的引用)
            {
                card.Deselect();
            }
        }//如果按下鼠标右键，取消选择所有卡片

        if (当前选中的卡片 == null)
            return;
        //如果没有卡片被选中，就结束Update方法
        if (是否正在交换位置)
            return;
        //如果卡片正在交换位置，就结束Update方法
        for (int i = 0; i < 列表字段用于存储所有卡片的引用.Count; i++)
        {//开始一个循环，检查每张卡片。

            if (当前选中的卡片.transform.position.x > 列表字段用于存储所有卡片的引用[i].transform.position.x)
            {
                if (当前选中的卡片.ParentIndex() < 列表字段用于存储所有卡片的引用[i].ParentIndex())
                {
                    交换两张卡片位置的函数(i);
                    break;
                }
            }//如果选中的卡片在x轴上的位置大于当前卡片，并且它的父索引小于当前卡片的父索引，调用Swap方法交换它们的位置。

            if (当前选中的卡片.transform.position.x < 列表字段用于存储所有卡片的引用[i].transform.position.x)
            {
                if (当前选中的卡片.ParentIndex() > 列表字段用于存储所有卡片的引用[i].ParentIndex())
                {
                    交换两张卡片位置的函数(i);
                    break;
                }
            }
        }
    }

    void 交换两张卡片位置的函数(int index)
    {
        是否正在交换位置 = true;

        Transform focusedParent = 当前选中的卡片.transform.parent;
        Transform crossedParent = 列表字段用于存储所有卡片的引用[index].transform.parent;

        列表字段用于存储所有卡片的引用[index].transform.SetParent(focusedParent);
        列表字段用于存储所有卡片的引用[index].transform.localPosition = 列表字段用于存储所有卡片的引用[index].被选择 ? new Vector3(0, 列表字段用于存储所有卡片的引用[index].selectionOffset, 0) : Vector3.zero;
        当前选中的卡片.transform.SetParent(crossedParent);

        是否正在交换位置 = false;

        if (列表字段用于存储所有卡片的引用[index].卡片视觉处理器 == null)
            return;

        bool swapIsRight = 列表字段用于存储所有卡片的引用[index].ParentIndex() > 当前选中的卡片.ParentIndex();
        列表字段用于存储所有卡片的引用[index].卡片视觉处理器.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (Card card in 列表字段用于存储所有卡片的引用)
        {
            card.卡片视觉处理器.UpdateIndex(transform.childCount);
        }
    }

}
*/