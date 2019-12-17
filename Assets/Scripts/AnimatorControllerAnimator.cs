using UnityEngine;

public class AnimatorControllerAnimator : MonoBehaviour
{
    private Animator _animator;
    private UnityChanController _controller;
    
    private readonly int _groundedTrigger = Animator.StringToHash("GroundedTrigger");
    private readonly int _jumpTrigger = Animator.StringToHash("JumpTrigger");
    private readonly int _horAbsInput = Animator.StringToHash("HorAbsInput");
    private readonly int _verSpeed = Animator.StringToHash("VerSpeed");

    private float _shootAnimationTime = 0.5f;
    private float _shootTimer;
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<UnityChanController>();
        _shootTimer = _shootAnimationTime;
    }

    void Update()
    {
        _animator.SetBool(_groundedTrigger, _controller.GroundedTriggered);
        _animator.SetBool(_jumpTrigger, _controller.JumpTriggered);
        _animator.SetFloat(_horAbsInput, _controller.HorAbsInput);
        _animator.SetFloat(_verSpeed, _controller.VerSpeed);

        if (_controller.FireTriggered) _shootTimer = 0;
        if (_shootTimer < _shootAnimationTime)
        {
            _shootTimer += Time.deltaTime;
            var weight = _shootTimer >= _shootAnimationTime ? 0 : 1;
            _animator.SetLayerWeight(1, weight);
        }

        _controller.ResetTriggers();
    }
}
