using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class SimpleAnimationAnimator : MonoBehaviour
{
    private SimpleAnimation _simpleAnimation;
    private UnityChanController _controller;

    private enum AnimState
    {
        Stand, Jump, Down, Grounded, Run
    }

    private StateMachine<AnimState> _animationFsm;

    private const string StandStateName = "Default";
    private const string JumpStartStateName = "JumpStart";
    private const string JumpStateName = "Jump";
    private const string DownStartStateName = "DownStart";
    private const string DownStateName = "Down";
    private const string GroundedStateName = "Grounded";
    private const string RunStateName = "Run";

    private float animationTimer = 0f;
    
    void Awake()
    {
        _simpleAnimation = GetComponent<SimpleAnimation>();
        _controller = GetComponent<UnityChanController>();

        _animationFsm = StateMachine<AnimState>.Initialize(this);
    }

    void Start()
    {
        _animationFsm.ChangeState(AnimState.Stand);
    }

    void LateUpdate()
    {
        _controller.ResetTriggers();
    }

    // --- Stand ---
    void Stand_Enter()
    {
        _simpleAnimation.Play(StandStateName);
    }
    
    void Stand_Update()
    {
        if (_controller.JumpTriggered)
        {
            _animationFsm.ChangeState(AnimState.Jump);
            return;
        }

        if (_controller.HorAbsInput != 0)
        {
            _animationFsm.ChangeState(AnimState.Run);
            return;
        }
    }

    // --- Jump ---
    void Jump_Enter()
    {
        _simpleAnimation.Play(JumpStartStateName);
        animationTimer = 0f;
    }

    void Jump_Update()
    {
        animationTimer += Time.deltaTime;
        var length = _simpleAnimation.GetState(JumpStartStateName).length;
        if (animationTimer >= length)
            _simpleAnimation.Play(JumpStateName);
        
        if (_controller.VerSpeed >= 0) return;
        _animationFsm.ChangeState(AnimState.Down);
    }

    // --- Down ---
    void Down_Enter()
    {
        _simpleAnimation.Play(DownStartStateName);
        animationTimer = 0f;
    }
    
    void Down_Update()
    {
        animationTimer += Time.deltaTime;
        var length = _simpleAnimation.GetState(DownStartStateName).length;
        if (animationTimer >= length)
            _simpleAnimation.Play(DownStateName);
        
        if (!_controller.GroundedTriggered) return;
        _animationFsm.ChangeState(AnimState.Grounded);
    }

    // --- Grounded ---
    IEnumerator Grounded_Enter()
    {
        _simpleAnimation.Play(GroundedStateName);
        yield return null;
        _animationFsm.ChangeState(AnimState.Stand);
    }

    // --- Run ---
    void Run_Enter()
    {
        _simpleAnimation.Play(RunStateName);
    }
    
    void Run_Update()
    {
        if (_controller.JumpTriggered)
        {
            _animationFsm.ChangeState(AnimState.Jump);
            return;
        }

        if (_controller.HorAbsInput == 0)
        {
            _animationFsm.ChangeState(AnimState.Stand);
        }
    }
}
