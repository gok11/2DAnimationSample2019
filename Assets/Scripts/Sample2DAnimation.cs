using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Sample2DAnimation : MonoBehaviour
{
    private PlayableGraph _playableGraph;
    private AnimationMixerPlayable _animationMixer;
    private AnimationClipPlayable _basePlayable, _overridePlayable;

    public bool UseOverrideClips
    {
        private get => _useOverrideClips;
        set
        {
            _useOverrideClips = value;

            var overrideWeight = _useOverrideClips ? 1 : 0;
            _animationMixer.SetInputWeight(0, overrideWeight);
        }
    }
    private bool _useOverrideClips;

    [SerializeField]
    private List<ClipInfo> clipInfos = new List<ClipInfo>();

    [Serializable]
    private class ClipInfo
    {
        public string name = "";
        public AnimationClip baseClip = null, overrideClip = null;
    }

    void Awake()
    {
        _playableGraph = PlayableGraph.Create(name);
        AnimationPlayableOutput.Create(_playableGraph, name, GetComponent<Animator>());
    }

    void OnDestroy()
    {
       _playableGraph.Destroy(); 
    }

    void Start()
    {
        _animationMixer = AnimationMixerPlayable.Create(_playableGraph, 2, true);

        var output = _playableGraph.GetOutput(0);
        output.SetSourcePlayable(_animationMixer);

        if (clipInfos.Count != 0)
        {
            Play(0);
            UseOverrideClips = false;
            _playableGraph.Play();
        }
    }

    public void Play(string animationName)
    {
        var index = clipInfos.FindIndex((n) => n.name == animationName);
        Play(index);
    }

    public void Play(int index)
    {
        DisconnectPlayables();
        
        _basePlayable = AnimationClipPlayable.Create(_playableGraph, clipInfos[0].baseClip);
        _overridePlayable = AnimationClipPlayable.Create(_playableGraph, clipInfos[0].overrideClip);
            
        _animationMixer.ConnectInput(0, _overridePlayable, 0);

        _animationMixer.ConnectInput(1, _basePlayable, 0);
        _animationMixer.SetInputWeight(1, 1);
    }

    void DisconnectPlayables()
    {
        _playableGraph.Disconnect(_animationMixer, 0);
        _playableGraph.Disconnect(_animationMixer, 1);
        
        if (_basePlayable.IsValid()) _basePlayable.Destroy();
        if (_overridePlayable.IsValid()) _overridePlayable.Destroy();
    }
}
