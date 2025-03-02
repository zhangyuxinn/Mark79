using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameMode : MonoBehaviour
{
    public static GameMode Instance;

    [Header("A队")] 
    [SerializeField] private PlayerController playerControllerA;
    [SerializeField] private MainHome mainHomeA;
    public int occupyHomesANum=0;
    
    [Header("B队")]
    [SerializeField] private PlayerController playerControllerB;
    [SerializeField] private MainHome mainHomeB;
    public int occupyHomesBNum=0;
    [Header("野外基地")] 
    [SerializeField] private List<OutsideHome> outsideHomes;
    [Header("事件")]
    public OutsideHomeOccupiedEventChannel outsideHomeOccupiedEventChannel;
    public HomeBoomEventChannel homeBoomEventChannel;
    public ProcessChangeEventChannel processChangeEventChannel;
    
    [Header("流程")] 
    public GameProcess gameProcess ;
    public GameModeStateMachine gameModeStateMachine;
    [SerializeField] private Volume volume;
    [Header("开始界面")] 
    [SerializeField] private AudioClip clickStart;
    [SerializeField] private AudioClip clickRedButton;
    [SerializeField] private AudioClip sleepAudio;
    [SerializeField] private int btnScaleCount = 10;
    [SerializeField] private GameObject buttonStart;
    [SerializeField] private string mainScene;
    [SerializeField] private GameObject panelBegin;
    [Header("阶段1")] 
    [SerializeField] private GameObject talkStartFight;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private AudioClip processOneBackgroundMusic;
    public GameObject processOneText;
    [Header("阶段2")] 
    [SerializeField] private GameObject aiController;
    [SerializeField] private float processTwoDuringTime = 30;
    [SerializeField] private AudioClip processTwoStartBackgroundMusic;
    [SerializeField] private List<AudioClip> processTwoBackgroundMusics;
    [SerializeField] private VolumeProfile profileProcessTwo;
    [Header("阶段3")] 
    [SerializeField] private GameObject dreamEnemy;
    [SerializeField] private float processThreeDuringTime = 10;
    [SerializeField] private int enemyNum;
    [SerializeField] private List<GameObject> dreamEnemies;
    [SerializeField] private AudioClip processThreeBackgroundMusic;
    [SerializeField] private AudioClip winAudioEffect;
    [SerializeField] private AudioClip loseAudioEffect;
    [SerializeField] private VolumeProfile profileProcessThree;
    [Header("阶段4")]
    [SerializeField] private GameObject endButton;
    [SerializeField] private float endButtonScaleCount=1;
    private void Awake()
    {       
        Instance = this;

    }

    private void Start()
    {
        if(gameProcess==GameProcess.Zero)
            AudioManager.Instance.PlayMultipleSound(sleepAudio,true);

    }

    private void OnEnable()
    {
        outsideHomeOccupiedEventChannel.AddListener(OnOccupy);
        homeBoomEventChannel.AddListener(OnHomeBoom);
    }

    private void OnDisable()
    {
        outsideHomeOccupiedEventChannel.RemoveListener(OnOccupy);
        homeBoomEventChannel.RemoveListener(OnHomeBoom);
    }

    public void OnOccupy(Team team)
    {
        if (team == Team.A)
        {
            occupyHomesANum++;
        }
        else if(team == Team.B)
        {
            occupyHomesBNum++;
        }
    }

    public void PlayerReborn(PlayerController playerController)
    {
        if(playerController==playerControllerA)
            playerController.gameObject.transform.position = mainHomeA.transform.position;
        else
        {
            playerController.gameObject.transform.position = mainHomeB.transform.position;
        }
        playerController.PlayerInit();
    }
    public void OnHomeBoom(Team teamBoom, HomeType boomHomeType)
    {
        if(boomHomeType==HomeType.OutSide)return;
        gameProcess = GameProcess.Four;
        //PlayerReborn(playerControllerA);
        endButton.SetActive(true);
        EndProcessThree();

        if (teamBoom == Team.A)
        {
            //开始阶段4
            AudioManager.Instance.PlayMultipleSound(loseAudioEffect);
            endButton.transform.position = mainHomeA.transform.position;
        }else if (teamBoom == Team.B)
        {
            //开始阶段4
            AudioManager.Instance.PlayMultipleSound(winAudioEffect);
            endButton.transform.position = mainHomeB.transform.position;
        }
        AudioManager.Instance.PlayBackgroundMusic(processOneBackgroundMusic);

    }

    #region ProcessZero

    

    public void BeginGame()
    {
        //panelBegin.SetActive(false);
        AudioManager.Instance.PlayMultipleSound(clickStart);
        SceneManager.LoadScene("MenuMap");
    }
    /* 
   public void AfterStartBtn()
   {
       AudioManager.Instance.PlayMultipleSound(clickRedButton);
         if (btnScaleCount != 1)
         {
             btnScaleCount--;
             buttonStart.transform.localScale = 
                 new Vector3((float)btnScaleCount / 1,
                     (float)btnScaleCount / 1,
                     (float)btnScaleCount / 1);
         }
         else
         {
             gameProcess = GameProcess.One;
             AudioManager.Instance.PlayBackgroundMusic(processOneBackgroundMusic);
             StartCoroutine(StartProcessBackgroundMusic());
             AudioManager.Instance.StopAllMultipleSound();
             //SceneManager.LoadScene(mainScene);

             //更改了跳转场景


         
    SceneManager.LoadScene("MenuMap");
    }
    }*/
    IEnumerator StartProcessBackgroundMusic()
    {
        yield return new WaitForSeconds(0.1f);
        AudioManager.Instance.PlayBackgroundMusic(processOneBackgroundMusic);

    }
    #endregion

    public IEnumerator StartProcessChange()
    {
        InitOne(false);
        processOneText.SetActive(false);
        AudioManager.Instance.PlayBackgroundMusic(processTwoStartBackgroundMusic);
        //gameProcess = GameProcess.Two;
        //EndProcessThree();

        yield return new WaitForSeconds(3f);
        while (gameProcess!=GameProcess.Four||gameProcess!=GameProcess.One)
        {

            if (gameProcess == GameProcess.Three)
            {
                EndProcessThree();
                Debug.Log(gameProcess);
                gameProcess = GameProcess.Two;
                //processChangeEventChannel.Broadcast(2);
                yield return new WaitForSeconds(processTwoDuringTime);
            }
            else
            {
                gameProcess = GameProcess.Three;
                //processChangeEventChannel.Broadcast(3);
                StartProcessThree();
                yield return new WaitForSeconds(processThreeDuringTime);
                Debug.Log(gameProcess);

            }
        }
    }

    #region ProcessOne

    public void StartProcessOne()
    {
        gameProcess = GameProcess.One;
    }

    public void UpdateProcessOne()
    {
        
    }

    public void EndProcessOne()
    {
        
    }

    #endregion
    
    #region ProcessTwoAndThree

    public void StartProcessTwoAndThree()
    {
        gameProcess = GameProcess.Two;
    }

    public void UpdateProcessTwoAndThree()
    {
        
    }

    public void EndProcessTwoAndThree()
    {
        
    }

    #endregion


    #region ProcessFour

    public void StartProcessFour()
    {
        gameProcess = GameProcess.Four;
        PlayerReborn(playerControllerA);
        endButton.SetActive(true);
        endButton.transform.position = mainHomeA.transform.position;
    }

    public void UpdateProcessFour()
    {
        
    }

    public void EndProcessFour()
    {
        
    }

    #endregion
    public void ActiveEndButton()
    {
        endButtonScaleCount++;
        endButton.transform.localScale =
            new Vector3(endButtonScaleCount / 1, endButtonScaleCount / 1, endButtonScaleCount / 1);
        if (endButtonScaleCount == 10f)
        {
            gameProcess = GameProcess.One;
            InitOne(true);
            PlayerReborn(playerControllerA);
        }
    }

    public void StartProcessThree()
    {
        AudioManager.Instance.PlayBackgroundMusic(processThreeBackgroundMusic);
        volume.profile = profileProcessThree;
        if (dreamEnemies.Count < enemyNum)
        {
            int curEnemyNum = dreamEnemies.Count;
            for (int i = 0; i < enemyNum-curEnemyNum; i++)
            {
                dreamEnemies.Add(Instantiate(dreamEnemy,RandomPointOnNavMesh(),quaternion.identity));
            }
        }
        else
        {
            foreach (var variableEnemy in dreamEnemies)
            {
                variableEnemy.transform.position = RandomPointOnNavMesh();
                variableEnemy.SetActive(true);
            }
        }
    }

    public void EndProcessThree()
    {
        volume.profile = profileProcessTwo;
        int i = Random.Range(0, processTwoBackgroundMusics.Count-1);
        AudioManager.Instance.PlayBackgroundMusic(processTwoBackgroundMusics[i]);
        // 使用临时列表来存储要删除的对象，避免在遍历列表时直接删除导致索引错误
        List<GameObject> objectsToRemove = new List<GameObject>();
        
        foreach (var variableEnemy in dreamEnemies)
        {
            if (variableEnemy == null)
            {
                objectsToRemove.Add(variableEnemy);
            }
            variableEnemy.GetComponent<GuardBase>().Init();
            variableEnemy.SetActive(false);
        }

        foreach (var removeObj in objectsToRemove)
        {
            dreamEnemies.Remove(removeObj);
        }
    }

    //todo 得改一下，不能直接生成，性能比较差
    private void InitOne(bool bol)
    {
        mainPanel.SetActive(!bol);
        aiController.SetActive(!bol);
        talkStartFight.SetActive(bol);
        mainHomeA.gameObject.SetActive(true);
        mainHomeB.gameObject.SetActive(true);
        mainHomeA.Init();
        mainHomeB.Init();
        endButton.SetActive(false);
        endButtonScaleCount = 1;
        endButton.transform.localScale =
            new Vector3(endButtonScaleCount / 1, endButtonScaleCount / 1, endButtonScaleCount / 1);
        foreach (var variableOutsideHome in outsideHomes)
        {
            variableOutsideHome.gameObject.SetActive(true);
            variableOutsideHome.gameObject.layer = 12;
            variableOutsideHome.Init();
        }
    }
    
    //todo 好好学一下这个代码
    //  在NavMesh区域内随机选一个点
    Vector3 RandomPointOnNavMesh()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // 随机选择一个三角形索引
        int randomTriangleIndex = Random.Range(0, navMeshData.indices.Length / 3);

        // 获取所选三角形的顶点索引
        int vertexIndex1 = navMeshData.indices[randomTriangleIndex * 3];
        int vertexIndex2 = navMeshData.indices[randomTriangleIndex * 3 + 1];
        int vertexIndex3 = navMeshData.indices[randomTriangleIndex * 3 + 2];

        // 获取三角形的三个顶点坐标
        Vector3 vertex1 = navMeshData.vertices[vertexIndex1];
        Vector3 vertex2 = navMeshData.vertices[vertexIndex2];
        Vector3 vertex3 = navMeshData.vertices[vertexIndex3];

        // 在所选三角形内随机生成一个点
        float r1 = Random.Range(0f, 1f);
        float r2 = Random.Range(0f, 1f);
        float s = 1f - Mathf.Sqrt(r1);
        float t = r2 * Mathf.Sqrt(r1);

        Vector3 randomPoint = (s * vertex1) + (t * vertex2) + ((1f - s - t) * vertex3);

        return randomPoint;
    }
}

public enum GameProcess
{
    Zero,
    One,
    Two,
    Three,
    Four,
}



