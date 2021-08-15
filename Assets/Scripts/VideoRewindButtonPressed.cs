using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts
{
    public class VideoRewindButtonPressed : MonoBehaviour
    {
        [SerializeField] private int _screenId;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                MediaDisplayManager.instance.VideoControl(_screenId, VideoControlVariant.Rewind);
            }
        }
    }
}