using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class VideoSelectButtonPressed : MonoBehaviour
    {
        [SerializeField] private Text _videoIdText;
        [SerializeField] private int _videoId;
        [SerializeField] private Text _buttonText;

        public bool IsPlaying { get; set; }

        public void PlayingOnScreen(string text)
        {
            _buttonText.text = text;
        }

        public int VideoId => _videoId;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                MediaDisplayManager.instance.MediaSelect(videoId: _videoId);
                _videoIdText.text = _videoId.ToString();
            }
        }
    }
}