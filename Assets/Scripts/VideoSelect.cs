using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class VideoSelect : MonoBehaviour
    {
        [SerializeField] private Text _videoIdText;

        private int _videoId;
        private int _previousId;
        
        private VideoSelectSync _videoSelectSync;


        void Start()
        {
            _videoSelectSync = gameObject.GetComponentInParent<VideoSelectSync>();
        }

        public void SetVideoId(int id)
        {
            _videoId = id;

            if (_videoId > 0 && _videoId != _previousId)
            {
                _videoIdText.text = _videoId.ToString();
                MediaDisplayManager.instance.SelectedVideo = _videoId;
                MediaDisplayManager.instance.SelectedMediaType = MediaType.VideoClip;
            }
        }

        public void KeepInSync()
        {
            // If the id has changed, call SetId on the sync component
            if (_videoId != _previousId)
            {
                _videoSelectSync.SetId(_videoId);
                _previousId = _videoId;
            }
        }
    }
}