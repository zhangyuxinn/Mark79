using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIPanel : UIBasePanel
{
    [Header("UI组件")]
    public List<Button> callGuardButtons;
    public Slider playerManaSlider;
    public TextMeshProUGUI playerKillNum;
    public TextMeshProUGUI playerDieNum;
    private int killNum;
    private int dieNum;
    [Header("事件参数")]
    [SerializeField] private CallGuardEventChannel callGuardEventChannel;
    [SerializeField] private StatChangeEventChannel statChangeEventChannelPlayerHealth;
    [SerializeField] private GuardChangeEventChannel statChangeEventChannelPlayerKill;
    [SerializeField] private StatChangeEventChannel statChangeEventChannelPlayerMana;
    public void BtnCallGuard(int index)
    {
        callGuardEventChannel.Broadcast(index,Team.A);
        callGuardEventChannel.Broadcast(index,Team.B);
    }

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

    }
    
    
    private void OnKillNumChange(Team team)
    {
        if(team==Team.B)
            playerKillNum.text =  "" + ++killNum;
        if(team==Team.A)
            playerDieNum.text =  "" + ++dieNum;
    }

    public void BackToStart()
    {
        SceneManager.LoadScene(0);
    }
}
