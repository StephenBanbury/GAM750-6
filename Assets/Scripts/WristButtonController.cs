using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class WristButtonController : MonoBehaviour
    {
        [SerializeField] private Text _messageText;
        [SerializeField] private int _buttonNumber;
        private string[] _messages;
        private bool _show;

        void Awake()
        {
            _show = false;
            _messages = new[] {"Show", "Hide"};
            if (_buttonNumber == 1)
            {
                ChangeButtonMessage();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                if (_buttonNumber == 1)
                {
                    ChangeButtonMessage();
                }
                else
                {
                    MediaDisplayManager.instance.PresetSelect(1);
                }
            }
        }

        private void ChangeButtonMessage()
        {
            _messageText.text = _messages[_show ? 1 : 0];
            ShowPanel();

            _show = !_show;
        }

        private void ShowPanel()
        {
            foreach (var panel in GameObject.FindGameObjectsWithTag("SelectionPanel"))
            {
                foreach (Transform child in panel.transform)
                {
                    child.gameObject.SetActive(_show);
                }
            }
        }
    }
}
