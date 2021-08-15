using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.Enums;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.Scripts
{
    public class ScreenPanelController : MonoBehaviour
    {
        private bool _triggerIsInAction;
        private int _waitForSeconds = 2;
        private ScreenFormation _currentScreenFormation;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                if (!_triggerIsInAction)
                {
                    StartCoroutine(TriggerAction(controllerName: other.name));
                }
            }
        }
        IEnumerator TriggerAction(string controllerName)
        {
            var leftHand = controllerName == "LeftHandCollider";
            var rightHand = controllerName == "RightHandCollider";

            var leftTrigger = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger) > 0;
            var rightTrigger = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0;

            if (leftHand && leftTrigger || rightHand && rightTrigger)
            {
                if(leftTrigger)
                    OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.LTouch);

                if(rightTrigger)
                    OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);

                Transform parent = gameObject.transform.parent;
                int screenId = int.Parse(parent.name.Replace("Screen", "").Replace("Variant", "").Trim());

                ScreenAction nextAction = MediaDisplayManager.instance.GetNextScreenAction(screenId);

                _triggerIsInAction = true;

                //GameObject gameManager = GameObject.Find("GameManager");
                //var portalDisplaySelect = gameManager.GetComponent<PortalSelect>();

                switch (nextAction)
                {
                    case ScreenAction.ChangeVideoClip:
                        Debug.Log("Doing action: Change video clip");
                        SelectRandomVideoClip();
                        break;

                    case ScreenAction.ChangeVideoStream:
                        Debug.Log("Doing action: Change video stream");
                        SelectRandomVideoStream();
                        break;

                    case ScreenAction.ChangeFormation:
                        Debug.Log("Doing action: Change screen formation");
                        ChangeScreenFormation();
                        break;

                    case ScreenAction.CreatePortal:
                        Debug.Log("Doing action: Create portal");
                        break;

                    case ScreenAction.DoTeleport:
                        Debug.Log("Doing action: Teleport");
                        int sceneId = MediaDisplayManager.instance.GetSceneIdFromScreenId(screenId);
                        int destinationSceneId = MediaDisplayManager.instance.TargetedTeleportation(screenId);
                        MediaDisplayManager.instance.StoreRealtimeScreenPortalState(screenId, destinationSceneId);
                        break;
                }

                yield return new WaitForSeconds(_waitForSeconds);

                MediaDisplayManager.instance.SetNextScreenAction(screenId);

                _triggerIsInAction = false;
            }
        }

        // Raise then lower mesh collider so that player can momentarily leave the scene. 
        IEnumerator RaiseMeshCollider()
        {
            var meshCollider = gameObject.GetComponent<MeshCollider>().transform;

            var x = meshCollider.position.x;
            var y = meshCollider.position.y;
            var z = meshCollider.position.z;

            meshCollider.position = new Vector3(x, y + 20f, z);

            PlayerAudioManager.instance.PlayAudioClip("Door 3 Open");

            yield return new WaitForSeconds(3); 

            meshCollider.position = new Vector3(x, y, z);
        }

        private void ChangeScreenFormation()
        {
            string sceneName = GetCurrentSceneFromParent();
            var scenes = MediaDisplayManager.instance.Scenes;
            var scene = scenes.First(s => s.Name == sceneName).Scene;

            if (MediaDisplayManager.instance.CanTransformScene.Contains(scene))
            {
                int numberOfFormations = Enum.GetValues(typeof(ScreenFormation)).Cast<int>().Max();

                ScreenFormation randomFormation;
                do
                {
                    randomFormation = (ScreenFormation) Math.Ceiling(Random.value * numberOfFormations);
                } while (randomFormation == _currentScreenFormation || randomFormation == ScreenFormation.None);

                _currentScreenFormation = randomFormation;

                GameObject gameManager = GameObject.Find("GameManager");
                FormationSelect formationSelect = gameManager.GetComponent<FormationSelect>();

                //formationSelect.SetFormationId(scene, (int) _currentScreenFormation, 10);
                formationSelect.KeepInSync(scene, (int)_currentScreenFormation);
            }
        }

        private string GetCurrentSceneFromParent()
        {
            var parentScene = transform.parent.parent.parent.gameObject;
            return parentScene.name;
        }

        private void SelectRandomVideoClip()
        {
            try
            {
                var parent = gameObject.transform.parent;
                var videos = MediaDisplayManager.instance.Videos;
                if (videos.Count > 0)
                {
                    int randIx = (int) Math.Ceiling(Random.value * videos.Count);

                    Debug.Log($"Random video index: {randIx} of {videos.Count}");

                    var videoId = videos[randIx].Id;
                    var screenId = int.Parse(parent.name.Replace("Screen", "").Replace("Variant", "").Trim());

                    Debug.Log($"Random video: video {videoId} onto screen {screenId}");

                    MediaDisplayManager.instance.StoreRealtimeScreenMediaState(videoId, (int) MediaType.VideoClip,
                        screenId);
                    MediaDisplayManager.instance.StoreBufferScreenMediaState(videoId, (int) MediaType.VideoClip,
                        screenId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SelectRandomVideoStream()
        {
            try
            {
                var parent = gameObject.transform.parent;
            var streams = AgoraController.instance.AgoraUsers;

            if (streams.Count > 0)
            {
                int randIx = (int)Math.Ceiling(Random.value * streams.Count);
                
                Debug.Log($"Random stream index: {randIx} of {streams.Count}");

                var streamId = streams[randIx].Id;
                var screenId = int.Parse(parent.name.Replace("Screen", "").Replace("Variant", "").Trim());

                Debug.Log($"Random video: video {streamId} onto screen {screenId}");

                MediaDisplayManager.instance.StoreRealtimeScreenMediaState(streamId, (int)MediaType.VideoStream, screenId);
                MediaDisplayManager.instance.StoreBufferScreenMediaState(streamId, (int)MediaType.VideoStream, screenId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
