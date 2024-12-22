using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private FloatEventChannel scrollEventChannel;
    [SerializeField] private float maxOrthoSize;
    [SerializeField] private float minOrthoSize;
    private void Start()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        scrollEventChannel.AddListener(OnScroll);
    }

    private void OnDisable()
    {
        scrollEventChannel.RemoveListener(OnScroll);
    }

    private void OnScroll(float num)
    {
        if(cinemachineVirtualCamera.m_Lens.OrthographicSize+num>minOrthoSize&&cinemachineVirtualCamera.m_Lens.OrthographicSize+num<maxOrthoSize)
            cinemachineVirtualCamera.m_Lens.OrthographicSize += num;
        
    }
    
    
}
