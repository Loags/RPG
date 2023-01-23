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

public class SM_CharacterController : MonoBehaviour
{
    public static SM_CharacterController playerInstance;
    [HideInInspector] public SM_CharacterMovement SM_CharacterMovement;
    [HideInInspector] public SM_CharacterAttack SM_CharacterAttack;
    [HideInInspector] public SM_CharacterInteraction SM_CharacterInteraction;
    [HideInInspector] public PlayerStats PlayerStats;

    [HideInInspector] public MonoBehaviour myMonoBehaviour;
    public CinemachineFreeLook thirdPersonCamera;
    private float m_YAxisMaxSpeed;
    private float m_XAxisMaxSpeed;


    private void Awake() { if (playerInstance == null) playerInstance = this; }

    private void Start()
    {
        SM_CharacterMovement = GetComponent<SM_CharacterMovement>();
        SM_CharacterAttack = GetComponent<SM_CharacterAttack>();
        SM_CharacterInteraction = GetComponentInChildren<SM_CharacterInteraction>();
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
