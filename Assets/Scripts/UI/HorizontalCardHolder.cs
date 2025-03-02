/*using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class HorizontalCardHolder : MonoBehaviour
{

    [SerializeField] private Card ��ǰѡ�еĿ�Ƭ;
    [SerializeReference] private Card ��ǰ�����ͣ�Ŀ�Ƭ;

    [SerializeField] private GameObject ���ڴ洢��Ƭ�۵�Ԥ�Ƽ�;
    private RectTransform RectTransform����UIԪ�ص�λ�úʹ�С;

    [Header("Spawn Settings")]
    [SerializeField] private int Ҫ���ɵĿ�Ƭ���� = 7;
    public List<Card> �б��ֶ����ڴ洢���п�Ƭ������;

    bool �Ƿ����ڽ���λ�� = false;
    [SerializeField] private bool ����ʱ�Ƿ�ʹ�ö��� = true;

    void Start()
    {
        for (int i = 0; i < Ҫ���ɵĿ�Ƭ����; i++)
        {
            Instantiate(���ڴ洢��Ƭ�۵�Ԥ�Ƽ�, transform);
        }

        RectTransform����UIԪ�ص�λ�úʹ�С = GetComponent<RectTransform>();
        �б��ֶ����ڴ洢���п�Ƭ������ = GetComponentsInChildren<Card>().ToList();

        int cardCount = 0;
        //��ʼ��һ�������������ڸ���Ƭ����
        foreach (Card card in �б��ֶ����ڴ洢���п�Ƭ������)
        {
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.name = cardCount.ToString();
            cardCount++;
        }
        //Ϊÿ�ſ�Ƭ����¼���������������������
        StartCoroutine(Frame());
        //��ʼһ��Э�̣����ڸ��¿�Ƭ���Ӿ�����
        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            for (int i = 0; i < �б��ֶ����ڴ洢���п�Ƭ������.Count; i++)
            {
                if (�б��ֶ����ڴ洢���п�Ƭ������[i].��Ƭ�Ӿ������� != null)
                    �б��ֶ����ڴ洢���п�Ƭ������[i].��Ƭ�Ӿ�������.UpdateIndex(transform.childCount);
            }
        }
        //Э��Frame�����ȴ�0.1�룬Ȼ��������п�Ƭ���Ӿ�����
    }

    private void BeginDrag(Card card)
    {
        ��ǰѡ�еĿ�Ƭ = card;
    }
    //����Ƭ��ʼ���϶�ʱ���õķ���������ǰ�϶��Ŀ�Ƭ����Ϊѡ��״̬��

    void EndDrag(Card card)
    {
        if (��ǰѡ�еĿ�Ƭ == null)
            return;//���û���κο�Ƭ��ѡ�У��ͽ���������

        ��ǰѡ�еĿ�Ƭ.transform.DOLocalMove(��ǰѡ�еĿ�Ƭ.��ѡ�� ? new Vector3(0, ��ǰѡ�еĿ�Ƭ.selectionOffset,0) : Vector3.zero, ����ʱ�Ƿ�ʹ�ö��� ? .15f : 0).SetEase(Ease.OutBack);

        RectTransform����UIԪ�ص�λ�úʹ�С.sizeDelta += Vector2.right;
        RectTransform����UIԪ�ص�λ�úʹ�С.sizeDelta -= Vector2.right;
        //�����д��뿴�����Ƕ���ģ����������ڴ������ָ��¡�
        ��ǰѡ�еĿ�Ƭ = null;
        //��ѡ�еĿ�Ƭ����Ϊnull��
    }

    void CardPointerEnter(Card card)
    {
        ��ǰ�����ͣ�Ŀ�Ƭ = card;
    }
    //�����ָ����뿨Ƭ����ʱ���õķ���,�������ͣ�Ŀ�Ƭ����Ϊ��ǰ��Ƭ��
    void CardPointerExit(Card card)
    {
        ��ǰ�����ͣ�Ŀ�Ƭ = null;
    }
    //�����ָ���뿪��Ƭ����ʱ���õķ������������ͣ�Ŀ�Ƭ����Ϊnull��
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (��ǰ�����ͣ�Ŀ�Ƭ != null)
            {
                Destroy(��ǰ�����ͣ�Ŀ�Ƭ.transform.parent.gameObject);
                �б��ֶ����ڴ洢���п�Ƭ������.Remove(��ǰ�����ͣ�Ŀ�Ƭ);

            }
        }
        //�������Delete���������п�Ƭ����ͣ�����ٸÿ�Ƭ����Ϸ���󣬲����б����Ƴ�
        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in �б��ֶ����ڴ洢���п�Ƭ������)
            {
                card.Deselect();
            }
        }//�����������Ҽ���ȡ��ѡ�����п�Ƭ

        if (��ǰѡ�еĿ�Ƭ == null)
            return;
        //���û�п�Ƭ��ѡ�У��ͽ���Update����
        if (�Ƿ����ڽ���λ��)
            return;
        //�����Ƭ���ڽ���λ�ã��ͽ���Update����
        for (int i = 0; i < �б��ֶ����ڴ洢���п�Ƭ������.Count; i++)
        {//��ʼһ��ѭ�������ÿ�ſ�Ƭ��

            if (��ǰѡ�еĿ�Ƭ.transform.position.x > �б��ֶ����ڴ洢���п�Ƭ������[i].transform.position.x)
            {
                if (��ǰѡ�еĿ�Ƭ.ParentIndex() < �б��ֶ����ڴ洢���п�Ƭ������[i].ParentIndex())
                {
                    �������ſ�Ƭλ�õĺ���(i);
                    break;
                }
            }//���ѡ�еĿ�Ƭ��x���ϵ�λ�ô��ڵ�ǰ��Ƭ���������ĸ�����С�ڵ�ǰ��Ƭ�ĸ�����������Swap�����������ǵ�λ�á�

            if (��ǰѡ�еĿ�Ƭ.transform.position.x < �б��ֶ����ڴ洢���п�Ƭ������[i].transform.position.x)
            {
                if (��ǰѡ�еĿ�Ƭ.ParentIndex() > �б��ֶ����ڴ洢���п�Ƭ������[i].ParentIndex())
                {
                    �������ſ�Ƭλ�õĺ���(i);
                    break;
                }
            }
        }
    }

    void �������ſ�Ƭλ�õĺ���(int index)
    {
        �Ƿ����ڽ���λ�� = true;

        Transform focusedParent = ��ǰѡ�еĿ�Ƭ.transform.parent;
        Transform crossedParent = �б��ֶ����ڴ洢���п�Ƭ������[index].transform.parent;

        �б��ֶ����ڴ洢���п�Ƭ������[index].transform.SetParent(focusedParent);
        �б��ֶ����ڴ洢���п�Ƭ������[index].transform.localPosition = �б��ֶ����ڴ洢���п�Ƭ������[index].��ѡ�� ? new Vector3(0, �б��ֶ����ڴ洢���п�Ƭ������[index].selectionOffset, 0) : Vector3.zero;
        ��ǰѡ�еĿ�Ƭ.transform.SetParent(crossedParent);

        �Ƿ����ڽ���λ�� = false;

        if (�б��ֶ����ڴ洢���п�Ƭ������[index].��Ƭ�Ӿ������� == null)
            return;

        bool swapIsRight = �б��ֶ����ڴ洢���п�Ƭ������[index].ParentIndex() > ��ǰѡ�еĿ�Ƭ.ParentIndex();
        �б��ֶ����ڴ洢���п�Ƭ������[index].��Ƭ�Ӿ�������.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (Card card in �б��ֶ����ڴ洢���п�Ƭ������)
        {
            card.��Ƭ�Ӿ�������.UpdateIndex(transform.childCount);
        }
    }

}
*/