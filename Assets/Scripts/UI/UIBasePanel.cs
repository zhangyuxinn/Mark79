using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBasePanel : MonoBehaviour
{
    protected UIInfo UIInfo;//存储UI的信息
    public bool isShow { get; private set; }//当前UI是否正在展示
    protected GameObject panelRoot;//当前UI的根节点
 
    public virtual void OnInit(UIInfo info)
    {
        UIInfo=info;
        panelRoot = transform.Find("Root").gameObject;
    }
    public virtual void OnOpen()//开启UI，由UIManager进行控制
    {
        if (!isShow)
        {
            ShowUI();
        }
 
    }
    public virtual void OnClose()//关闭UI，由UIManager进行控制
    {
        if (isShow)
        {
            HideUI();
           
            if (!UIInfo.IsCache)
            {
                Destroy(gameObject);
            }
            
        }
 
    }
    public virtual void ShowUI()//展示UI，播放UI入场动画
    {
        isShow = true;
        panelRoot.SetActive(true);
        
    }
    public virtual void HideUI()//隐藏UI，播放UI关闭动画
    {
        isShow = false;
        panelRoot.SetActive(false);
    }
 
}

public class UIInfo
{
    public string UIName;
    public bool IsCache;
    public bool IsDynamic;//是否是动态UI，例如屏幕外导航
    public UIOverlayMode OverlayMode;
    public UIControlMode ControlMode;
    public UILayer Layer;
 
    public UIBasePanel PanelInstance;
    public bool IsOpen = false;
}
public enum UILayer
{
    Back,
    Mid,
    Front
}
public enum UIOverlayMode
{
    NewPanel,//独立的UI界面，同时只能显示一个,例如活动页面，点击活动后跳转的页面
    Additive//叠加模式，例如弹窗
}
public enum UIControlMode
{
    System,//完全由代码控制UI的开关，例如物品不足提示UI
    Player//玩家可以主动开启或关闭，点击按钮或者使用快捷键开关
}
