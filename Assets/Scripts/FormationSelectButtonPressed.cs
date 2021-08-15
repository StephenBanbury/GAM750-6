using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class FormationSelectButtonPressed : MonoBehaviour
    {
        [SerializeField] private Text _formationIdText;
        [SerializeField] private int _formationId;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                var scenes = MediaDisplayManager.instance.Scenes;
                var sceneName = GetCurrentSceneFromParent();
                var scene = scenes.First(s => s.Name == sceneName).Scene;

                if (MediaDisplayManager.instance.CanTransformScene.Contains(scene))
                {
                    var gameManager = GameObject.Find("GameManager");
                    var formationSelect = gameManager.GetComponent<FormationSelect>();

                    //formationSelect.SetFormationId(scene, _formationId, 10);

                    formationSelect.KeepInSync(scene, _formationId);

                    _formationIdText.text = _formationId.ToString();
                }
            }
        }

        private string GetCurrentSceneFromParent()
        {
            var parentScene = transform.parent.parent.parent.gameObject;
            return parentScene.name;
        }

    }
}