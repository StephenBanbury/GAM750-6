using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class StreamSelectButtonPressed : MonoBehaviour
    {
        public int StreamId { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                MediaDisplayManager.instance.MediaSelect(streamId: StreamId);
                //Debug.Log($"StreamSelectButtonPressed: {StreamId}");
            }
        }
    }
}