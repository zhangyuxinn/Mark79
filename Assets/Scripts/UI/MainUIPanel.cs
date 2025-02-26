using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIPanel : UIBasePanel//这行定义了一个名为MainUIPanel的公共类，它继承自UIBasePanel。
{
    [Header("UI组件")]
    public List<Button> callGuardButtons;
    public Slider playerManaSlider;
    public TextMeshProUGUI playerKillNum;
    public TextMeshProUGUI playerDieNum;
    private int killNum;
    private int dieNum;
    //这里定义了一些UI组件的引用，包括按钮列表、滑块、文本显示，以及两个私有的整型变量用于记录击杀和死亡次数。
    [Header("事件参数")]
    [SerializeField] private CallGuardEventChannel callGuardEventChannel;
    [SerializeField] private StatChangeEventChannel statChangeEventChannelPlayerHealth;
    [SerializeField] private GuardChangeEventChannel statChangeEventChannelPlayerKill;
    [SerializeField] private StatChangeEventChannel statChangeEventChannelPlayerMana;
    //这些是序列化的私有字段，用于存储事件通道的引用，这些事件通道可能用于在不同部分的代码之间传递事件。
    public  void BtnCallGuard(int index)
    {
        callGuardEventChannel.Broadcast(index,Team.A);
        callGuardEventChannel.Broadcast(index,Team.B);
    }
    //这是一个公共方法，当"调用守卫"按钮被点击时调用。
    //它通过callGuardEventChannel广播守卫调用事件，为两个队伍（A和B）

    //修改:将逻辑改为卡片拖动结束后，广播生成守卫事件。注释这里的按钮函数
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        statChangeEventChannelPlayerHealth.AddListener(OnHealthChange);
        statChangeEventChannelPlayerMana.AddListener(OnManaChange);
        statChangeEventChannelPlayerKill.AddListener(OnKillNumChange);
        killNum = -1;
        dieNum = -1;
        OnKillNumChange(Team.B);
        OnKillNumChange(Team.A);
        //Init方法初始化事件监听器，并将击杀和死亡次数重置为-1。
        //然后，它调用OnKillNumChange方法来初始化击杀和死亡次数的显示。
    }

    private void OnHealthChange(int healthNum, Team team)
    {
        /*if(team==Team.A)
            playerHealthNum.text =  "A队主角血量：" + healthNum;*/
        /*if(team==Team.B)
            playerHealthNum.text =  "B队主角血量：" + healthNum;*/

    }
    private void OnManaChange(int ManaNum, Team team)
    {
        if(team==Team.A)
            playerManaSlider.value = ManaNum/12f;
        //当A队的法力值变化时，这个方法会更新UI中的滑块值
    }


    private void OnKillNumChange(Team team)
    {
        if(team==Team.B)
            playerKillNum.text =  "" + ++killNum;
        if(team==Team.A)
            playerDieNum.text =  "" + ++dieNum;
    }
    //当击杀或死亡次数变化时，这个方法会更新UI中的文本显示。对于B队，它增加击杀次数；对于A队，它增加死亡次数。
    public void BackToStart()
    {
        SceneManager.LoadScene(0);
    }
}//这是一个公共方法，用于返回到游戏的开始场景。它使用SceneManager来重新加载索引为0的场景。
