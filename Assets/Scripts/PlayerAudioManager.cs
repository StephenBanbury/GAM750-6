using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{

    public class PlayerAudioManager : MonoBehaviour
    {
        public static PlayerAudioManager instance;

        [SerializeField] private List<AudioClip> _audioClips;
        [SerializeField] private AudioSource _audioSource;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            //Debug.Log($"Number of player audio clips: {_audioClips.Count}");
            //foreach (var audioClip in _audioClips)
            //{
            //    Debug.Log(audioClip.name);
            //}
        }

        public void PlayAudioClip(string clipName)
        {
            AudioClip audioClip = _audioClips.FirstOrDefault(a => a.name == clipName);
            _audioSource.PlayOneShot(audioClip);
        }
    }

}