using Cinemachine;
using Manager_Coroutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterContentState
{
    INITIALIZATION,
    ATTACK,
    BLOCK
}

public enum CharacterFlowState
{
    NONE,
    INCOMBAT,
    DEAD
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [HideInInspector] public PlayerMovement PlayerMovement;
    [HideInInspector] public PlayerAttack PlayerAttack;
    [HideInInspector] public PlayerInteraction PlayerInteraction;
    [HideInInspector] public PlayerStats PlayerStats;

    [HideInInspector] public MonoBehaviour myMonoBehaviour;
    public CinemachineFreeLook thirdPersonCamera;
    private float m_YAxisMaxSpeed;
    private float m_XAxisMaxSpeed;


    private void Awake() { if (instance == null) instance = this; }

    private void Start()
    {
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerAttack = GetComponent<PlayerAttack>();
        PlayerInteraction = GetComponentInChildren<PlayerInteraction>();
        PlayerStats = GetComponent<PlayerStats>();
        myMonoBehaviour = GetComponent<MonoBehaviour>();
    }

    public void InitializeCoroutine(ref Coroutine _coroutine, IEnumerator _enumerator, MonoBehaviour _behaviour = null)
    {
        _behaviour = myMonoBehaviour;
        CoroutineManager.InitializeCoroutine(ref _coroutine, _enumerator, _behaviour);
    }

    public void TerminateCoroutine(ref Coroutine _coroutine, IEnumerator _enumerator, MonoBehaviour _behaviour = null)
    {
        _behaviour = myMonoBehaviour;
        CoroutineManager.TerminateCoroutine(ref _coroutine, _enumerator, _behaviour);
    }

    public void ToggleCameraInput()
    {
        if (thirdPersonCamera.m_YAxis.m_MaxSpeed == 0 && thirdPersonCamera.m_XAxis.m_MaxSpeed == 0)
        {
            thirdPersonCamera.m_YAxis.m_MaxSpeed = m_YAxisMaxSpeed;
            thirdPersonCamera.m_XAxis.m_MaxSpeed = m_XAxisMaxSpeed;
        }
        else
        {
            m_YAxisMaxSpeed = thirdPersonCamera.m_YAxis.m_MaxSpeed;
            m_XAxisMaxSpeed = thirdPersonCamera.m_XAxis.m_MaxSpeed;

            thirdPersonCamera.m_YAxis.m_MaxSpeed = 0;
            thirdPersonCamera.m_XAxis.m_MaxSpeed = 0;
        }

    }
}
