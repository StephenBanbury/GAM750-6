using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ScreenClearButtonPressed : MonoBehaviour
    {
        [SerializeField] private int _screenId;

        private Renderer _rend;
        private Color _playColour;
        private GameObject _playButton;
        private Text _buttonText;

        private void Start()
        {
            _playButton = GameObject.Find($"PlayButton{_screenId}");
            if (_playButton != null)
            {
                _rend = _playButton.GetComponent<Renderer>();
                _playColour = _rend.material.GetColor("_Color");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                MediaDisplayManager.instance.RemoveVideo(_screenId);

                if (_playButton)
                {
                    _buttonText = _playButton.GetComponentInChildren<Text>();
                    _buttonText.text = "Play";
                    _rend.material.SetColor("_Color", _playColour);
                    _playButton.GetComponent<ScreenSelectButtonPressed>().IsPlaying = false;
                }
            }
        }
    }
}