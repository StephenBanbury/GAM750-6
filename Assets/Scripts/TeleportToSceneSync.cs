using Normal.Realtime;
using UnityEngine;

namespace Assets.Scripts
{
    public class TeleportToSceneSync : RealtimeComponent<TeleportToSceneModel>
    {
        private int _sceneId;

        private void Awake()
        {
            _sceneId = 0;
        }

        protected override void OnRealtimeModelReplaced(TeleportToSceneModel previousModel,
            TeleportToSceneModel currentModel)
        {
            if (previousModel != null)
            {
                previousModel.sceneIdDidChange -= SceneIdDidChange;
                //previousModel.secondsDelayDidChange -= SceneIdDidChange;
            }

            if (currentModel != null)
            {
                if (currentModel.isFreshModel)
                {
                    currentModel.sceneId = _sceneId;
                    //currentModel.secondsDelay = _secondsDelay;
                }

                DoUpdate();

                currentModel.sceneIdDidChange += SceneIdDidChange;
            }
        }

        private void SceneIdDidChange(TeleportToSceneModel model, int value)
        {
            Debug.Log($"SceneIdDidChange: SceneId {value}");
            DoUpdate();
        }

        public void DoUpdate()
        {
            Debug.Log($"DoUpdate! SceneId {model.sceneId}");
            StartCoroutine(MediaDisplayManager.instance.DoTeleportation(model.sceneId));
        }

        public void SetNewScene(int id)
        {
            Debug.Log($"Model is null: {model == null}");
            model.sceneId = id;
        }
    }
}