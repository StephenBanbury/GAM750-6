using System;
using System.Collections;
using Assets.Scripts.Models;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Services;
using DG.Tweening;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MediaDisplayManager: RealtimeComponent<MediaScreenDisplayModel>
    {
        public static MediaDisplayManager instance;
        private int _sceneIndex;
        private float _floorAdjust = 1.25f;
        private int _compoundScreenId = 0;
        private int _currentSelectedVideoClip;
        private int _currentSelectedVideoStream;

        [SerializeField] private VideoClip[] _videoClips = new VideoClip[5];
        [SerializeField] private Transform _streamButton;
        [SerializeField] private GameObject _screen;
        [SerializeField] private GameObject _screenVariant;
        [SerializeField] private AudioSource _sceneAudio;
        [SerializeField] private GameObject _sceneLights;
        [SerializeField] private GameObject _selectionPanels;
        [SerializeField] private Text _lobbyStatusInfoText;
        [SerializeField] private GameObject _startButton;
        [SerializeField] private Text _debugText;
        [SerializeField] private Text _hudText;
        [SerializeField] private Material _skybox1;
        [SerializeField] private Material _skybox2;
        [SerializeField] private Text _messageText;

        private List<Camera> _allCameras;

        private List<ScreenPortalBufferState> _screenPortalBuffer;
        private List<MediaScreenAssignState> _mediaStateBuffer;
        private List<MediaScreenAssignState> _mediaStatePreparationBuffer;
        // _lastSelectionSelected = Scene=1; Formation=2; Stream=3; Clip=4; Screen=5; Portal=6
        private int _lastSelectionSelected;

        const bool showHiddenScreens = false;
        const bool showNumbers = false;

        public List<SceneDetail> Scenes { get; private set; }
        public List<Scene> CanTransformScene { get; set; }
        public Scene MyCurrentScene { get; set; }
        public List<MediaDetail> Videos { get; private set; }
        public List<ScreenActionModel> ScreenActions { get; private set; }

        private PresetService _presetService;
        private bool _presetIsPlaying = false;
        private int _currentPresetId = 0;

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
            Debug.Log($"mediaScreenDisplayStates: {model.mediaScreenDisplayStates.Count}");

            _startButton.SetActive(true);
            SetSkybox(false);
            StartCoroutine(AwaitVideosFromApiBeforeStart());
        }

        private IEnumerator AwaitVideosFromApiBeforeStart()
        {
            Videos = new List<MediaDetail>();
            _mediaStatePreparationBuffer = new List<MediaScreenAssignState>();
            _mediaStateBuffer = new List<MediaScreenAssignState>();
            _screenPortalBuffer = new List<ScreenPortalBufferState>();

            GetLocalVideosDetails();

            //GetVideoLinksFromTextFile();

            yield return StartCoroutine(GetVideosFromApi());

            Debug.Log(
                $"Number of videos: " +
                $"Local={Videos.Count(v => v.Source == Source.LocalFile)}; " +
                $"External={Videos.Count(v => v.Source == Source.Url)}");

            yield return StartCoroutine(DownloadVideoFiles(Videos));

            Scenes = new List<SceneDetail>();
            ScreenActions = new List<ScreenActionModel>();

            _sceneIndex = 1;

            CanTransformScene = new List<Scene> {Scene.Scene1};

            SpawnScene(Scene.Scene1, ScreenFormation.LargeSquare, true);
            SpawnScene(Scene.Scene2, ScreenFormation.SmallSquare, true);
            SpawnScene(Scene.Scene3, ScreenFormation.Circle, true);
            SpawnScene(Scene.Scene4, ScreenFormation.Cross, true);
            SpawnScene(Scene.Scene5, ScreenFormation.ShortRectangle, true);
            SpawnScene(Scene.Scene6, ScreenFormation.LargeStar, true);
            SpawnScene(Scene.Scene7, ScreenFormation.Triangle, true);
            SpawnScene(Scene.Scene8, ScreenFormation.LongRectangle, true);

            CreateStreamSelectButtons();

            // Test: set preset screen displays
            _presetService = new PresetService();
            PresetSet();

            MyCurrentScene = Scene.Scene1;

            ShowSelectionPanel(true, false);

            //InitialiseAllCameras();
            //CameraSelect("Camera 0");
            
        }
        
        protected override void OnRealtimeModelReplaced(MediaScreenDisplayModel previousModel, MediaScreenDisplayModel currentModel)
        {
            Debug.Log("OnRealtimeModelReplaced");

            if (previousModel != null)
            {
                Debug.Log("previousModel != null");

                // Unregister from events
                previousModel.mediaScreenDisplayStates.modelAdded -= MediaAssignedToDisplay;
                previousModel.mediaScreenDisplayStates.modelRemoved -= MediaRemovedFromDisplay;
                previousModel.screenPortalStates.modelAdded -= PortalAssignedToDisplay;
            }

            if (currentModel != null)
            {
                Debug.Log($"currentModel != null. Models: {currentModel.mediaScreenDisplayStates.Count}");

                AssignMediaToDisplaysFromArray();
                AssignPortalToDisplaysFromArray();

                // Let us know when a new screen has changed 
                currentModel.mediaScreenDisplayStates.modelAdded += MediaAssignedToDisplay;
                currentModel.mediaScreenDisplayStates.modelRemoved += MediaRemovedFromDisplay;
                currentModel.screenPortalStates.modelAdded += PortalAssignedToDisplay;
            }
        }

        private void MediaAssignedToDisplay(RealtimeSet<MediaScreenDisplayStateModel> mediaScreenDisplayStates, MediaScreenDisplayStateModel mediaScreenDisplayState, bool remote)
        {
            Debug.Log($"mediaScreenDisplayStates: {mediaScreenDisplayStates.Count}");
            Debug.Log($"mediaScreenDisplayState - screen: {mediaScreenDisplayState.screenDisplayId}, mediaId: {mediaScreenDisplayState.mediaId}");
            Debug.Log($"remote: {remote}");

            bool assigned = false;

            switch (mediaScreenDisplayState.mediaTypeId)
            {
                case (int)MediaType.VideoClip:
                    assigned = AssignVideoToDisplay(mediaScreenDisplayState.mediaId, mediaScreenDisplayState.screenDisplayId);
                    break;

                case (int)MediaType.VideoStream:
                    assigned = AssignStreamToDisplay(mediaScreenDisplayState.mediaId, mediaScreenDisplayState.screenDisplayId);
                    break;
            }

            if (assigned)
            {
                _mediaStateBuffer.Add(new MediaScreenAssignState
                {
                    MediaTypeId = mediaScreenDisplayState.mediaTypeId,
                    MediaId = mediaScreenDisplayState.mediaId,
                    ScreenDisplayId = mediaScreenDisplayState.screenDisplayId
                });
            }
        }

        private void MediaRemovedFromDisplay(RealtimeSet<MediaScreenDisplayStateModel> mediaScreenDisplayStates, MediaScreenDisplayStateModel mediaScreenDisplayState, bool remote)
        {
            Debug.Log($"MediaRemovedFromDisplay: {mediaScreenDisplayStates.Count}");
            Debug.Log($"MediaRemovedFromDisplay - screen: {mediaScreenDisplayState.screenDisplayId}, mediaId: {mediaScreenDisplayState.mediaId}");
            Debug.Log($"remote: {remote}");

            var unCompoundScreenId = UnCompoundScreenId(mediaScreenDisplayState.screenDisplayId);

            ClearScreen(unCompoundScreenId);
        }

        private void PortalAssignedToDisplay(RealtimeSet<ScreenPortalStateModel> screenPortalStates,
            ScreenPortalStateModel screenPortalState, bool remote)
        {
            Debug.Log("PortalAssignedToDisplay: -");
            foreach (var modelScreenPortalState in model.screenPortalStates)
            {
                Debug.Log($"RealtimeSet: {modelScreenPortalState.screenId} is portal: {modelScreenPortalState.isPortal}");
            }

            AssignPortalToDisplaysFromArray();
        }

        private void AssignPortalToDisplaysFromArray()
        {
            // If this is a new player joining the room then they may realtime and local buffer arrays may differ

            foreach (var portalState in model.screenPortalStates)
            {
                var existingBufferRecord = _screenPortalBuffer.FirstOrDefault(p => p.ScreenId == portalState.screenId);

                Debug.Log($"Existing portal buffer record: {existingBufferRecord != null}");

                if (existingBufferRecord != null)
                {
                    if (existingBufferRecord.IsPortal == portalState.isPortal && existingBufferRecord.DestinationSceneId == portalState.destinationSceneId)
                        Debug.Log($"Portal on screen {portalState.screenId} is already set to {portalState.isPortal} and destination scene {portalState.destinationSceneId}");
                    else if (existingBufferRecord.IsPortal == portalState.isPortal)
                    {
                        Debug.Log($"Portal on screen {portalState.screenId} will change to scene {portalState.destinationSceneId}");
                        existingBufferRecord.DestinationSceneId = portalState.destinationSceneId;
                    }
                    else
                    {
                        Debug.Log($"Screen {portalState.screenId} will become a portal to scene {portalState.destinationSceneId}");
                        existingBufferRecord.IsPortal = portalState.isPortal;
                        AssignPortalToScreen(portalState.screenId, portalState.isPortal);
                    }
                }
                else
                {
                    Debug.Log($"Assigning portal: screen: {portalState.screenId}; destination scene: {portalState.destinationSceneId}; isPortal: {portalState.isPortal}");
                    _screenPortalBuffer.Add(new ScreenPortalBufferState
                    {
                        ScreenId = portalState.screenId,
                        DestinationSceneId = portalState.destinationSceneId,
                        IsPortal = portalState.isPortal
                    });
                    AssignPortalToScreen(portalState.screenId, portalState.isPortal);
                }
            }
        }

        public void AssignMediaToDisplaysFromArray()
        {
            foreach (var mediaInfo in model.mediaScreenDisplayStates)
            {
                var existing = _mediaStateBuffer.FirstOrDefault(m =>
                    m.ScreenDisplayId == mediaInfo.screenDisplayId &&
                    m.MediaTypeId == mediaInfo.mediaTypeId &&
                    m.MediaId == mediaInfo.mediaId);

                if (existing != null)
                {
                    Debug.Log($"MediaId {mediaInfo.mediaId} already exists on screen {mediaInfo.screenDisplayId}");
                }
                else
                {
                    bool assigned = false;

                    switch (mediaInfo.mediaTypeId)
                    {
                        case (int)MediaType.VideoClip:
                            assigned = AssignVideoToDisplay(mediaInfo.mediaId, mediaInfo.screenDisplayId);
                            break;

                        case (int)MediaType.VideoStream:
                            assigned = AssignStreamToDisplay(mediaInfo.mediaId, mediaInfo.screenDisplayId);
                            break;
                    }

                    if (assigned)
                    {
                        _mediaStateBuffer.Add(new MediaScreenAssignState
                        {
                            MediaTypeId = mediaInfo.mediaTypeId,
                            MediaId = mediaInfo.mediaId,
                            ScreenDisplayId = mediaInfo.screenDisplayId
                        });
                    }
                }
            }
        }

        public void StoreBufferScreenMediaState(int mediaId, int mediaTypeId, int screenId)
        {
            var existingMediaOnScreen =
                _mediaStateBuffer
                    .FirstOrDefault(s => s.ScreenDisplayId == screenId);

            Debug.Log($"StoreBufferScreenMediaState. Media exists in buffer: {existingMediaOnScreen != null}");

            if (existingMediaOnScreen != null)
            {
                existingMediaOnScreen.MediaTypeId = mediaTypeId;
                existingMediaOnScreen.MediaId = mediaId;
            }
            else
            {
                _mediaStateBuffer.Add(new MediaScreenAssignState
                {
                    ScreenDisplayId = screenId,
                    MediaTypeId = mediaTypeId,
                    MediaId = mediaId
                });
            }
        }

        public void StoreBufferScreenMediaState()
        {
            Debug.Log("In StoreBufferScreenMediaState");

            foreach (var prepBuffer in _mediaStatePreparationBuffer)
            {
                StoreBufferScreenMediaState(prepBuffer.MediaId, prepBuffer.MediaTypeId, prepBuffer.ScreenDisplayId);
            }
        }

        public bool StoreRealtimeScreenMediaState(int mediaId, int mediaTypeId, int screenId)
        {
            var isAlreadyOnScreen =
                model.mediaScreenDisplayStates
                    .Any(s => s.screenDisplayId == screenId
                              && s.mediaTypeId == mediaTypeId
                              && s.mediaId == mediaId);

            Debug.Log($"StoreRealtimeScreenMediaState. This exact media already on screen: {isAlreadyOnScreen}");

            if (isAlreadyOnScreen) return true;

            var existingMediaOnScreen =
                model.mediaScreenDisplayStates
                    .FirstOrDefault(s => s.screenDisplayId == screenId);

            Debug.Log($"StoreRealtimeScreenMediaState. Other media exists on screen: {existingMediaOnScreen != null}");

            if (existingMediaOnScreen != null)
            {
                model.mediaScreenDisplayStates.Remove(existingMediaOnScreen);
            }

            model.mediaScreenDisplayStates.Add(new MediaScreenDisplayStateModel
            {
                screenDisplayId = screenId,
                mediaTypeId = mediaTypeId,
                mediaId = mediaId
            });

            return false;
        }

        public void StoreRealtimeScreenMediaState()
        {
            Debug.Log("In StoreRealtimeScreenMediaState");
            Debug.Log($"_mediaStatePreparationBuffer: {_mediaStatePreparationBuffer.Count}");

            foreach (var buffer in _mediaStatePreparationBuffer)
            {
                StoreRealtimeScreenMediaState(buffer.MediaId, buffer.MediaTypeId, buffer.ScreenDisplayId);
            }
        }

        public void StoreRealtimeScreenPortalState(int thisScreenId, int destinationSceneId)
        {
            var existingRealtimeState =
                model.screenPortalStates.FirstOrDefault(p => p.screenId == thisScreenId);

            Debug.Log($"StoreRealtimeScreenPortalState. Exists: {existingRealtimeState != null}");

            if (existingRealtimeState != null)
            {
                // TODO find a way of making this into an event change

                if (existingRealtimeState.destinationSceneId == destinationSceneId)
                {
                    Debug.Log($"Change existing portal state on screen {thisScreenId} from {existingRealtimeState.isPortal} to {!existingRealtimeState.isPortal}");
                    existingRealtimeState.isPortal = !existingRealtimeState.isPortal;
                }
                else
                {
                    Debug.Log($"Change existing portal on screen {thisScreenId} destination from scene {existingRealtimeState.destinationSceneId} to scene {destinationSceneId}");
                    existingRealtimeState.destinationSceneId = destinationSceneId;
                }

                // Buffer state
                var existingBufferState = _screenPortalBuffer.FirstOrDefault(p => p.ScreenId == thisScreenId);
                if (existingBufferState.DestinationSceneId == destinationSceneId)
                {
                    Debug.Log($"Change existing portal state on screen {thisScreenId} from {existingBufferState.IsPortal} to {!existingBufferState.IsPortal}");
                    existingBufferState.IsPortal = !existingBufferState.IsPortal;
                }
                else
                {
                    Debug.Log($"Change existing portal destination on screen {thisScreenId} from scene {existingBufferState.DestinationSceneId} to scene {destinationSceneId}");
                    existingBufferState.DestinationSceneId = destinationSceneId;
                }

                if (!existingBufferState.IsPortal)
                {
                    Transform screenObject = GetScreenObjectFromScreenId(thisScreenId);
                    if (screenObject != null)
                    {
                        Transform portal = screenObject.Find("Portal");
                        portal.gameObject.SetActive(false);
                    }

                    ScreenActionModel screenAction = ScreenActions.FirstOrDefault(a => a.ScreenId == thisScreenId);
                    screenAction.NextAction = ScreenAction.ChangeVideoClip;
                }
            }
            else
            {
                ScreenPortalStateModel state = new ScreenPortalStateModel
                {
                    screenId = _compoundScreenId,
                    destinationSceneId = destinationSceneId,
                    isPortal = true
                };

                model.screenPortalStates.Add(state);
            }

            //Debug.Log("StoreRealtimeScreenPortalState: -");
            //foreach (var model in model.screenPortalStates)
            //{
            //    Debug.Log($"ScreenId {model.screenId} is portal: {model.isPortal}");
            //}
        }

        public ScreenAction GetNextScreenAction(int screenId)
        {
            var screenAction = ScreenActions.FirstOrDefault(a => a.ScreenId == screenId);
            Debug.Log($"GetNextScreenAction on screenId {screenId}: {screenAction.NextAction}");
            return screenAction.NextAction;
        }

        public void SetNextScreenAction(int screenId)
        {
            ScreenAction newAction;
            ScreenActionModel screenAction = ScreenActions.FirstOrDefault(a => a.ScreenId == screenId);
            ScreenAction lastAction = screenAction.NextAction;
            Scene scene = GetSceneFromScreenId(screenId);

            if (lastAction == ScreenAction.CreatePortal)
            {
                newAction = ScreenAction.DoTeleport;
            }
            else
            {
                bool canDoFormation = CanTransformScene.Contains(scene);
                bool hasVideoStreams = AgoraController.instance.AgoraUsers.Count > 0;
                int numberOfActions = Enum.GetValues(typeof(ScreenAction)).Cast<int>().Max();
                do
                {
                    newAction = (ScreenAction) Math.Ceiling(Random.value * numberOfActions);
                } while (newAction == lastAction
                         || newAction == ScreenAction.CreatePortal // TEMPORARY - TODO: establish if we can create a portal this way
                         || newAction == ScreenAction.ChangeFormation && !canDoFormation
                         || newAction == ScreenAction.DoTeleport && lastAction != ScreenAction.CreatePortal 
                         || newAction == ScreenAction.ChangeVideoStream && !hasVideoStreams);
            }

            //Debug.Log($"SetNextScreenAction on screenId {screenId}: {newAction}");

            screenAction.NextAction = newAction;
        }

        public Scene GetSceneFromScreenId(int screenId)
        {
            int sceneId = GetSceneIdFromScreenId(screenId);
            Scene scene = Scenes.FirstOrDefault(s => s.Id == sceneId).Scene;
            return scene;
        }

        public int GetSceneIdFromScreenId(int screenId)
        {
            string sceneIdString = screenId.ToString().Substring(0, 1);
            int sceneId = int.Parse(sceneIdString);
            return sceneId;
        }

        public int TargetedTeleportation(int screenId)
        {
            Debug.Log("TargetedTeleportation");

            int destinationSceneId = _screenPortalBuffer.First(s => s.ScreenId == screenId && s.IsPortal).DestinationSceneId;
                    
            StartCoroutine(DoTeleportation(destinationSceneId));
            return destinationSceneId;
        }

        public IEnumerator DoTeleportation(int sceneId, bool scatter = false)
        {
            Debug.Log($"DoTeleportation: {sceneId}");

            string spawnPointName = $"Spawn Point {sceneId}";
            Transform spawnPoint = GameObject.Find(spawnPointName).transform;
            Transform player = GameObject.Find("Player").transform;
            var playerController = player.GetComponent<OVRPlayerController>();
            var sceneSampleController = player.GetComponent<OVRSceneSampleController>();

            //Debug.Log($"Teleporting to {spawnPointName}");
            //Debug.Log($"SpawnPoint position: {spawnPoint.position}");

            playerController.enabled = false;
            sceneSampleController.enabled = false;

            PlayerAudioManager.instance.PlayAudioClip("Teleport 3_1");

            yield return new WaitForSeconds(2f);

            PlayerAudioManager.instance.PlayAudioClip("Teleport 3_2");

            Vector3 newPosition = spawnPoint.position;

            if (scatter)
            {
                float x = newPosition.x + Random.Range(-1f, 1f);
                float y = newPosition.y + Random.Range(-1f, 1f);
                float z = newPosition.z;
                newPosition = new Vector3(x, y, z);
            }

            player.position = newPosition;

            if (sceneId == 9) MediaDisplayManager.instance.SetSkybox(true);

            yield return new WaitForSeconds(0.5f);
            
            playerController.enabled = true;
            sceneSampleController.enabled = true;

            MyCurrentScene = (Scene) sceneId;

            ShowSelectionPanel(false, false);
        }

        private void ShowPanelButtonMessage(int screenId, int sceneId, string message)
        {
            var screenIdText = screenId.ToString();
            screenIdText = 3 > screenIdText.Length ? screenIdText : screenIdText.Substring(screenIdText.Length - 1);

            screenId = int.Parse(screenIdText);

            foreach (var panel in GameObject.FindGameObjectsWithTag("SelectionPanel"))
            {
                //Debug.Log($"Panel name: {panel.name}");

                if (panel.name == $"Selection Panel {sceneId}")
                {
                    //Debug.Log($"{panel.name} found.");

                    var texts = panel.GetComponentsInChildren<Text>();


                    if (texts != null)
                    {
                        //Debug.Log($"{texts.Length} texts found.");

                        var textName = $"Current Media {screenId}";

                        //Debug.Log($"Looking for {textName}");

                        var text = texts.FirstOrDefault(t => t.name == textName);

                        if (text != null)
                        {
                            //Debug.Log($"Found text {textName}");

                            text.text = message;
                        }

                        break;
                    }
                }
            }
        }

        public void ShowSelectionPanel(bool applyToAll, bool show)
        {
            Debug.Log($"ShowSelectionPanel - applyToAll: {applyToAll}");

            ShowSelectionPanel((int) MyCurrentScene, applyToAll, show);
        }

        private void ShowSelectionPanel(int sceneId, bool applyToAll, bool show)
        {
            Debug.Log($"ShowSelectionPanel - sceneId: {sceneId}, applyToAll: {applyToAll}");

            foreach (var panel in GameObject.FindGameObjectsWithTag("SelectionPanel"))
            {
                //Debug.Log($"Panel name: {panel.name}");

                if (applyToAll || panel.name == $"Selection Panel {sceneId}")
                {
                    foreach (Transform child in panel.transform)
                    {
                        child.gameObject.SetActive(show);
                    }
                }
            }
        }

        private IEnumerator DownloadVideoFiles(List<MediaDetail> mediaDetails)
        {
            //Debug.Log("Downloading video files: -");
            _lobbyStatusInfoText.text = "Downloading video files: -";
            _debugText.text = "Saving video file to: ";

            var mediaDetailsToUse = mediaDetails.Where(m => m.Source == Source.Url);
            //.Take(20)

            foreach (var mediaDetail in mediaDetailsToUse)
            {
                string savePath;
                if (Application.platform == RuntimePlatform.Android)
                {
                    string rootPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal));
                    savePath = Path.Combine(Path.Combine(rootPath, "Android/Data/com.MachineAppStudios.GAM7506/files"), mediaDetail.Filename);
                }
                else
                {
                    savePath = $"{Application.persistentDataPath}/{mediaDetail.Filename}";
                }

                mediaDetail.LocalPath = savePath;

                _debugText.text += $"\n{savePath}";
                _lobbyStatusInfoText.text = $"{mediaDetail.Filename}\n";

                if (File.Exists(savePath))
                {
                    _lobbyStatusInfoText.text += " - exists.";
                }
                else
                {
                    _lobbyStatusInfoText.text += " - downloading.";
                    string url = mediaDetail.Url;
                    using (UnityWebRequest www = UnityWebRequest.Get(url))
                    {
                        yield return www.Send();
                        if (www.isNetworkError || www.isHttpError)
                        {
                            Debug.Log(www.error);
                        }
                        else
                        {
                            Debug.Log($"Saving video file to: {savePath}");
                            System.IO.File.WriteAllBytes(savePath, www.downloadHandler.data);
                        }

                        if (File.Exists(savePath))
                            _debugText.text += " - Saved!";
                        else
                            _debugText.text += " - Not saved!";
                    }
                }
            }

            _lobbyStatusInfoText.text = "\nAll downloads finished.";
            _startButton.SetActive(true);
        }

        public void CreateStreamSelectButtons()
        {
            if (Scenes == null)
                Scenes = new List<SceneDetail>();

            var agoraUsers = AgoraController.instance.AgoraUsers;

            if (agoraUsers != null)
            {
                foreach (var sceneDetail in Scenes)
                {
                    GameObject scene = GameObject.Find(sceneDetail.Name);
                    Transform panels = scene.transform.Find($"Selection Panel {sceneDetail.Id}");

                    if (panels != null)
                    {
                        Transform selectorPanel = panels.Find("StreamSelectorPanel");

                        if (selectorPanel != null)
                        {
                            foreach (Transform child in selectorPanel)
                            {
                                Destroy(child.gameObject);
                            }

                            var joinedUsers = agoraUsers.Where(u => !(u.IsLocal || u.LeftRoom)).ToList();

                            Debug.Log($"Non-local agora users: {joinedUsers.Count}");

                            var xPos = selectorPanel.position.x;
                            var yStart = 0.5f;
                            var zPos = selectorPanel.position.z;

                            var i = 1;

                            foreach (var joinedUser in joinedUsers)
                            {
                                var buttonName = $"Button{sceneDetail.Id}{i}";
                                var yPos = yStart - (i - 1) * 0.117f;
                                var button = Instantiate(_streamButton, new Vector3(xPos, yPos, zPos),
                                    Quaternion.identity);

                                button.name = buttonName;

                                Text buttonText = button.GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
                                buttonText.text = joinedUser.Uid.ToString();

                                var buttonScript = button.gameObject.GetComponent<StreamSelectButtonPressed>();
                                buttonScript.StreamId = joinedUser.Id;

                                button.transform.SetParent(selectorPanel);

                                i++;
                            }
                        }
                    }
                }
            }
        }


        private void GetLocalVideosDetails()
        {
            Debug.Log("Get Videos from local storage");
            var videoService = new VideoService();
            Videos = videoService.GetLocalVideos();
        }

        private void GetVideoLinksFromTextFile()
        {
            // Get external video URLs from text file
            Debug.Log("Get Videos from text file");

            var textLines = GetVideosExternal.GetFromTextFile();

            var i = 1;
            foreach (var textLine in textLines)
            {
                var video = new MediaDetail
                {
                    Id = i,
                    Title = $"Video {Videos.Count + 1}",
                    MediaType = MediaType.VideoClip,
                    Source = Source.Url,
                    Url = textLine
                };

                Videos.Add(video);

                i++;
            }
        }

        public IEnumerator GetVideosFromApi()
        {
            // Get external video URLs from database
            var apiService = new ApiService();
            var videosFromApi = apiService.VideosGet();

            yield return new WaitUntil(() => videosFromApi.Count > 0);

            Debug.Log($"GetVideosFromApi - done: {videosFromApi.Count}");
            Videos.AddRange(videosFromApi);
        }

        public void PresetSet()
        {
            // Test

            var mediaScreenAssignStates = new List<MediaScreenAssignState>();

            // test preset 1
            for (int i = 1; i <= 16; i++)
            {
                mediaScreenAssignStates.Add(
                    new MediaScreenAssignState
                    {
                        MediaId = i,
                        MediaTypeId = (int)MediaType.VideoClip,
                        ScreenDisplayId = i
                    }
                );
            }

            int presetId = _presetService.SetPresets(mediaScreenAssignStates);

            Debug.Log($"Set presetId: {presetId}");

            mediaScreenAssignStates = new List<MediaScreenAssignState>();

            // test preset 2
            for (int i = 17; i <= 32; i++)
            {
                mediaScreenAssignStates.Add(
                    new MediaScreenAssignState
                    {
                        MediaId = i - 16 + 100,
                        MediaTypeId = (int)MediaType.VideoClip,
                        ScreenDisplayId = i - 16
                    }
                );
            }

            presetId = _presetService.SetPresets(mediaScreenAssignStates);

            Debug.Log($"Set presetId: {presetId}");

        }

        public void PresetSelect(int id)
        {
            //_currentPresetId++;
            _currentPresetId = 1;
            PresetSelect();
        }

        private void PresetSelect()
        {
            _presetIsPlaying = !_presetIsPlaying;

            if (_presetIsPlaying)
            {
                Debug.Log($"Select preset {_currentPresetId}");
                var preset = _presetService.GetPreset(_currentPresetId);

                _mediaStatePreparationBuffer = new List<MediaScreenAssignState>();

                foreach (var state in preset.MediaScreenAssignStates)
                {
                    var screenId = CompoundScreenId(state.ScreenDisplayId);

                    Debug.Log(
                        $"Preset State - MediaType: {state.MediaTypeId}, MediaId: {state.MediaId}, Display: {screenId}");

                    _mediaStatePreparationBuffer.Add(
                        new MediaScreenAssignState
                        {
                            MediaId = state.MediaId,
                            MediaTypeId = state.MediaTypeId,
                            ScreenDisplayId = screenId
                        });
                }

                Apply();
            }
            else
            {
                Debug.Log("Stop playing preset");

                foreach (var displayState in model.mediaScreenDisplayStates)
                {
                    var isRemoved = model.mediaScreenDisplayStates.Remove(displayState);
                    Debug.Log($"isRemoved: {isRemoved}");
                }
            }

            //Apply();
        }

        public void Apply()
        {
            StoreRealtimeScreenMediaState();
            StoreBufferScreenMediaState();

            //AssignMediaToDisplaysFromArray();
            
            _mediaStatePreparationBuffer.Clear();
        }

        public void RemoveVideo(int screenId)
        {
            var compoundScreenId = CompoundScreenId(screenId);

            var displayState =
                model.mediaScreenDisplayStates.FirstOrDefault(d => d.screenDisplayId == compoundScreenId);

            var isRemoved = model.mediaScreenDisplayStates.Remove(displayState);
            Debug.Log($"isRemoved: {isRemoved}");
        }

        public void ClearScreen(int screenId)
        {
            if (screenId > 0)
            {
                var compoundScreenId = CompoundScreenId(screenId);

                Debug.Log($"Clear display {compoundScreenId}");

                var displaySuffix = "Wide";

                var canvasDisplayName = $"CanvasDisplay{displaySuffix}";
                var videoDisplayName = $"VideoDisplay{displaySuffix}";

                Transform screenObject = GetScreenObjectFromScreenId(compoundScreenId);

                var videoDisplay = screenObject.transform.Find(videoDisplayName);
                var canvasDisplay = screenObject.transform.Find(canvasDisplayName);

                videoDisplay.gameObject.SetActive(true);
                canvasDisplay.gameObject.SetActive(false);

                var videoPlayer = videoDisplay.GetComponentInChildren<VideoPlayer>();

                if (videoPlayer != null)
                {
                    videoPlayer.enabled = false;

                    // Remove media?
                    // Remove from RealtimeSet?


                    //ShowPanelButtonMessage(screenId, sceneId, $"Video: {videoId}");
                }
            }
        }

        private Transform GetScreenObjectFromScreenId(int screenId)
        {
            var sceneId = int.Parse(screenId.ToString().Substring(0, 1));
            var screensContainerName = $"Screens {sceneId}";
            var screenName = $"Screen {screenId}";
            var screenVariantName = $"Screen Variant {screenId}";
            var sceneName = Scenes.First(s => s.Id == sceneId).Name;
            var scene = GameObject.Find(sceneName);
            var screensContainer = scene.transform.Find(screensContainerName);
            var screenObject = screensContainer.transform.Find(screenName);
            if (screenObject == null) screenObject = screensContainer.transform.Find(screenVariantName);
            return screenObject;
        }

        private int CompoundScreenId(int screenId)
        {
            var compoundId = (int) MyCurrentScene * 100 + screenId;
            return compoundId;
        }
        private int UnCompoundScreenId(int screenId)
        {
            var unCompoundId = screenId - (int) MyCurrentScene * 100;
            return unCompoundId;
        }

        public void MediaSelect(int videoId = 0, int streamId = 0)
        {
            _currentSelectedVideoClip = videoId;
            _currentSelectedVideoStream = streamId;
        }

        public MediaType ScreenSelectAndPlayMedia(int screenId)
        {
            var currentMediaType = MediaType.None;

            _compoundScreenId = CompoundScreenId(screenId);

            currentMediaType = _currentSelectedVideoClip != 0
                ? MediaType.VideoClip
                : MediaType.VideoStream;

            bool isAlreadyPlaying = 
                StoreRealtimeScreenMediaState(_currentSelectedVideoClip, (int)currentMediaType, _compoundScreenId);

            if(isAlreadyPlaying && currentMediaType == MediaType.VideoClip) 
                VideoControl(screenId, VideoControlVariant.Play);
            
            StoreBufferScreenMediaState(_currentSelectedVideoClip, (int)currentMediaType, _compoundScreenId);

            return currentMediaType;
        }
        
        public void VideoControl(int screenId, VideoControlVariant videoControl, int numberOfFrames = 96)
        {
            if (screenId > 0)
            {
                var compoundScreenId = CompoundScreenId(screenId);

                Debug.Log($"videoControl: {videoControl}, screenId: {compoundScreenId}");

                Transform screenObject = GetScreenObjectFromScreenId(compoundScreenId);

                Debug.Log($"screenObject exists: {screenObject != null}");

                var videoDisplay = screenObject.transform.Find("VideoDisplayWide");
                if (videoDisplay == null) videoDisplay = screenObject.transform.Find("VideoDisplayTall");

                Debug.Log($"videoDisplay exists: {videoDisplay != null}");

                if (videoDisplay != null)
                {
                    var videoPlayer = videoDisplay.GetComponentInChildren<VideoPlayer>();

                    Debug.Log($"videoPlayer exists: {videoPlayer != null}");

                    if (videoPlayer != null)
                    {
                        videoPlayer.enabled = true;

                        switch (videoControl)
                        {
                            case VideoControlVariant.Pause:
                                Debug.Log("videoPlayer.Pause");
                                videoPlayer.Pause();
                                break;
                            case VideoControlVariant.Play:
                                Debug.Log("videoPlayer.Play");
                                videoPlayer.Play();
                                break;
                            case VideoControlVariant.Stop:
                                Debug.Log("videoPlayer.Stop");
                                videoPlayer.Stop();
                                break;
                            case VideoControlVariant.FastForward:
                                Debug.Log("videoPlayer.FastForward");
                                videoPlayer.frame = videoPlayer.frame + (numberOfFrames);
                                break;
                            case VideoControlVariant.Rewind:
                                Debug.Log("videoPlayer.Rewind");
                                videoPlayer.frame = videoPlayer.frame - (numberOfFrames);
                                break;
                            case VideoControlVariant.JumpToFrame:
                                Debug.Log("videoPlayer.JumpToFrame");
                                videoPlayer.frame = numberOfFrames;
                                break;
                        }
                    }
                }
            }
        }

        private bool AssignVideoToDisplay(int videoId, int screenId)
        {
            Debug.Log($"Assign video clip {videoId} to display {screenId}");

            //try
            //{
            //Debug.Log("AssignVideoToDisplay");
            //Debug.Log($"videoId: {videoId}");
            //Debug.Log($"displayId: {displayId}");

            var displaySuffix = "Wide";

            var canvasDisplayName = $"CanvasDisplay{displaySuffix}";
            var videoDisplayName = $"VideoDisplay{displaySuffix}";

            if (videoId > 0 && screenId > 0) // && _displayVideo[localDisplayId].Id != videoId)
            {
                //Debug.Log($"Assign videoId: {videoId}");

                Transform screenObject = GetScreenObjectFromScreenId(screenId);

                var thisVideoClip = Videos.FirstOrDefault(v => v.Id == videoId);

                //Debug.Log($"Show video '{thisVideoClip.Title}' on display {screenObject.name}");

                var videoDisplay = screenObject.transform.Find(videoDisplayName);
                var canvasDisplay = screenObject.transform.Find(canvasDisplayName);

                videoDisplay.gameObject.SetActive(true);
                canvasDisplay.gameObject.SetActive(false);

                var videoPlayer = videoDisplay.GetComponentInChildren<VideoPlayer>();

                //Add AudioSource
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();

                if (thisVideoClip.Source == Source.Url)
                {
                    //Debug.Log($"new path: {thisVideoClip.LocalPath}; current path on {videoPlayer.name}: {videoPlayer.url}");

                    if (thisVideoClip.LocalPath == videoPlayer.url)
                    {
                        Debug.Log($"Video {videoId} is already playing on screen {screenId}");
                        if (!videoPlayer.isPlaying)
                        {
                            videoPlayer.enabled = true;
                            videoPlayer.Play();
                        }
                    }
                    else
                    {
                        // Video clip from Url
                        //Debug.Log("URL video clip");

                        videoPlayer.source = VideoSource.Url;


                        // Set mode to Audio Source.
                        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

                        // We want to control one audio track with the video player
                        videoPlayer.controlledAudioTrackCount = 1;

                        // We enable the first track, which has the id zero
                        videoPlayer.EnableAudioTrack(0, true);

                        // ...and we set the audio source for this track
                        videoPlayer.SetTargetAudioSource(0, audioSource);


                        videoPlayer.url = thisVideoClip.LocalPath;

                        videoPlayer.enabled = true;

                        StartCoroutine(PrepareVideo(videoPlayer));

                        var sceneId = GetSceneIdFromScreenId(screenId);

                        ShowPanelButtonMessage(screenId, sceneId, $"Video: {videoId}");
                    }
                }
                else
                {
                    // Video clip from local storage
                    //Debug.Log("Local video clip");
                    var vc = _videoClips[videoId - 1];
                    videoPlayer.clip = vc;
                }

                //var currentVideoClip = GetMedia(videoId, MediaType.VideoClip);

                //currentVideoClip.IsDisplay = true;

                return true;
            }

            return false;
            //}
            //catch (Exception exception)
            //{
            //    Debug.Log(exception);
            //    return false;
            //}

        }

        private MediaScreenAssignState GetMedia(int mediaId, MediaType mediaType)
        {
            var media =
                _mediaStateBuffer
                    .FirstOrDefault(m => m.MediaId == mediaId
                                         && m.MediaTypeId == (int) mediaType);

            return media;
        }

        private bool AssignStreamToDisplay(int streamId, int displayId)
        {
            Debug.Log($"Assign video stream {streamId} to display {displayId}");

            //try
            //{
            if (streamId > 0 && displayId > 0)
            {
                var agoraUsers = AgoraController.instance.AgoraUsers;

                if (agoraUsers.Any())
                {
                    Debug.Log("agoraUsers: -");
                    foreach (var user in agoraUsers)
                    {
                        Debug.Log(
                            $" - {user.Uid} (isLocal: {user.IsLocal}, leftRoom: {user.LeftRoom}, id: {user.Id})");
                    }

                    var agoraUser = agoraUsers.FirstOrDefault(u => u.Id == streamId);

                    Debug.Log($"agoraUser exists: {agoraUser != null}");

                    if (agoraUser != null)
                    {
                        if (agoraUser.IsLocal || agoraUser.LeftRoom)
                        {
                            Debug.Log(
                                $"Something has gone wrong - is local: {agoraUser.IsLocal}, left room: {agoraUser.LeftRoom}.");
                        }
                        else
                        {
                            agoraUser.DisplayId = displayId;
                            AgoraController.instance.AssignStreamToDisplay(agoraUser);
                        }
                    }
                }

                return true;
            }

            return false;
            //}
            //catch (Exception exception)
            //{
            //    Debug.Log(exception);
            //    return false;
            //}
        }

        private void AssignPortalToScreen(int screenId, bool isActive)
        {
            if (_screenPortalBuffer.Any(p => p.ScreenId == screenId))
            {
                var screenAction = ScreenActions.FirstOrDefault(a => a.ScreenId == screenId);

                if (isActive)
                {
                    Debug.Log($"Assigning NextAction to screen {screenId}");
                    screenAction.NextAction = ScreenAction.DoTeleport;
                }
                else
                {
                    Debug.Log($"Removing portal on screen {screenId}");
                    screenAction.NextAction = ScreenAction.ChangeVideoClip;
                }

                Transform screenObject = GetScreenObjectFromScreenId(screenId);
                if (screenObject != null)
                {
                    Transform portal = screenObject.Find("Portal");
                    portal.gameObject.SetActive(isActive);
                }
            }
        }

        IEnumerator PrepareVideo(VideoPlayer videoPlayer)
        {
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                yield return new WaitForEndOfFrame();
            }

            videoPlayer.Play();
        }

        private void HudClear()
        {
            _hudText.text = "";
        }

        private void HudStartMessage(string message)
        {
            //_hudText.text = $"\n{message}";
            Debug.Log(message);
        }

        private void SpawnScene(Scene scene, ScreenFormation formation, bool includeSelectionPanels = false)
        {
            var thisFormation = new List<ScreenPosition>();
            var screenFormationService = new ScreenFormationService(scene);

            switch (formation)
            {
                case ScreenFormation.LargeSquare:
                    thisFormation = screenFormationService.LargeSquare();
                    break;
                case ScreenFormation.SmallSquare:
                    thisFormation = screenFormationService.SmallSquare();
                    break;
                case ScreenFormation.Cross:
                    thisFormation = screenFormationService.Cross();
                    break;
                //case ScreenFormation.SmallStar:
                //    thisFormation = screenFormationService.SmallStar();
                //    break;
                case ScreenFormation.LargeStar:
                    thisFormation = screenFormationService.LargeStar();
                    break;
                case ScreenFormation.Circle:
                    thisFormation = screenFormationService.Circle();
                    break;
                case ScreenFormation.Triangle:
                    thisFormation = screenFormationService.Triangle();
                    break;
                case ScreenFormation.ShortRectangle:
                    thisFormation = screenFormationService.ShortRectangle();
                    break;
                case ScreenFormation.LongRectangle:
                    thisFormation = screenFormationService.LongRectangle();
                    break;
            }


            var scenePosition = screenFormationService.ScenePosition;
            //var sceneObject = Instantiate(_sceneObject, scenePosition, Quaternion.identity);

            var sceneName = $"Scene {_sceneIndex}";
            var sceneObject = GameObject.Find(sceneName);

            if (sceneObject == null)
            {
                //Debug.Log($"{sceneName} not found");
                sceneObject = new GameObject(sceneName);
            }


            // Instantiate selection panels, audio source and lighting as part of scene object

            if (includeSelectionPanels)
            {
                var selectionPanelsTrans = sceneObject.transform.Find($"Selection Panel {_sceneIndex}");
                if (selectionPanelsTrans == null)
                {
                    GameObject selectionPanels = Instantiate(_selectionPanels,
                        _selectionPanels.transform.position + scenePosition, Quaternion.identity);
                    selectionPanels.transform.SetParent(sceneObject.transform);
                    selectionPanels.name = $"Selection Panel {_sceneIndex}";

                    Text indicator = selectionPanels.transform.Find("SceneSelectorView/Canvas/SceneText")
                        .GetComponent<Text>();
                    if (indicator != null)
                    {
                        indicator.text = sceneName;
                    }
                }
            }

            var sceneAudioTrans = sceneObject.transform.Find($"Scene Audio {_sceneIndex}");
            if (sceneAudioTrans == null)
            {
                AudioSource sceneAudio = Instantiate(_sceneAudio, _sceneAudio.transform.position + scenePosition,
                    Quaternion.identity);
                sceneAudio.transform.SetParent(sceneObject.transform);
                sceneAudio.name = $"Scene Audio {_sceneIndex}";
            }

            var sceneLightsTrans = sceneObject.transform.Find($"Scene Lights {_sceneIndex}");
            if (sceneLightsTrans == null)
            {
                GameObject sceneLights = Instantiate(_sceneLights, _sceneLights.transform.position + scenePosition,
                    Quaternion.identity);
                sceneLights.transform.SetParent(sceneObject.transform);
                sceneLights.name = $"Scene Lights {_sceneIndex}";
            }

            Scenes.Add(new SceneDetail
            {
                Id = _sceneIndex,
                Scene = scene,
                Name = sceneName,
                ScreenFormation = formation,
                ScenePosition = scenePosition,
                CurrentScreens = new List<GameObject>()
            });

            GameObject screensContainer = GameObject.Find($"Screens {_sceneIndex}");

            if (screensContainer == null)
            {
                //Debug.Log($"Screens {_sceneIndex} not found");
                screensContainer = new GameObject($"Screens {_sceneIndex}");
                screensContainer.transform.SetParent(sceneObject.transform);
            }

            var currentScene = Scenes.First(s => s.Id == _sceneIndex);

            foreach (var screenPosition in thisFormation)
            {
                var screenId = _sceneIndex * 100 + screenPosition.Id;

                //if (!screenPosition.Hide)
                //{
                var vector3 = screenPosition.Vector3;
                vector3.y += _floorAdjust;

                GameObject screen;

                GameObject thisScreen;
                string screenName;

                if (screenPosition.Id % 2 != 0)
                {
                    screenName = $"Screen {screenId}";
                    thisScreen = _screen;
                }
                else
                {
                    screenName = $"Screen Variant {screenId}";
                    thisScreen = _screenVariant;
                }

                screen = GameObject.Find(screenName);

                if (screen == null)
                {
                    screen = Instantiate(thisScreen, vector3, Quaternion.identity);
                    //screen = Realtime.Instantiate(screenName, vector3, Quaternion.identity);
                    screen.transform.Rotate(0, screenPosition.Rotation, 0);
                    screen.transform.SetParent(screensContainer.transform);
                    if (!showHiddenScreens && screenPosition.Hide)
                    {
                        screen.SetActive(false);
                    }
                }
                //else
                //{
                //    Debug.Log($"{screenName} exists");
                //}

                screen.name = screenName;

                var screenNumber = screen.GetComponentInChildren<Text>();
                if (screenNumber != null)
                {
                    screenNumber.text = screenPosition.Id.ToString();
                    screenNumber.enabled = showNumbers;
                }

                var screenCamera = screen.GetComponentInChildren<Camera>();
                if (screenCamera != null)
                {
                    screenCamera.name = $"Camera {screenId}";
                }

                currentScene.CurrentScreens.Add(screen);

                ScreenActions.Add(new ScreenActionModel
                {
                    ScreenId = screenId
                });

                SetNextScreenAction(screenId);
                //}
            }

            _sceneIndex++;
        }

        private void InitialiseAllCameras()
        {
            _allCameras = new List<Camera>();
            foreach (var camera in Camera.allCameras)
            {
                _allCameras.Add(camera);
                //Debug.Log($"{camera} added to _allCameras");
            }
        }

        public void CameraSelect(string cameraName)
        {
            foreach (var camera in _allCameras)
            {
                camera.enabled = camera.name == cameraName;
                camera.GetComponent<AudioListener>().enabled = camera.name == cameraName;
            }
        }

        public void TweenScreens(ScreenFormation newFormation, int tweenTimeSeconds)
        {
            TweenScreens(MyCurrentScene, newFormation, tweenTimeSeconds);
        }

        public void TweenScreens(Scene scene, ScreenFormation newFormation, int tweenTimeSeconds)
        {
            var thisFormation = new List<ScreenPosition>();
            var screenFormationService = new ScreenFormationService(scene);

            switch (newFormation)
            {
                case ScreenFormation.LargeSquare:
                    thisFormation = screenFormationService.LargeSquare();
                    break;
                case ScreenFormation.SmallSquare:
                    thisFormation = screenFormationService.SmallSquare();
                    break;
                case ScreenFormation.Cross:
                    thisFormation = screenFormationService.Cross();
                    break;
                //case ScreenFormation.SmallStar:
                //    thisFormation = screenFormationService.SmallStar();
                //    break;
                case ScreenFormation.LargeStar:
                    thisFormation = screenFormationService.LargeStar();
                    break;
                case ScreenFormation.Circle:
                    thisFormation = screenFormationService.Circle();
                    break;
                case ScreenFormation.Triangle:
                    thisFormation = screenFormationService.Triangle();
                    break;
                case ScreenFormation.ShortRectangle:
                    thisFormation = screenFormationService.ShortRectangle();
                    break;
                case ScreenFormation.LongRectangle:
                    thisFormation = screenFormationService.LongRectangle();
                    break;
            }

            var thisScene = Scenes.FirstOrDefault(s => s.Scene == scene);

            if (thisScene != null)
            {
                var audioSource =
                    GameObject.Find(thisScene.Name)
                        .transform.Find($"Scene Audio {thisScene.Id}")
                        .GetComponent<AudioSource>();

                audioSource.Play();

                foreach (var screenPosition in thisFormation)
                {
                    var screenPositionPrev = thisScene.CurrentScreens[screenPosition.Id - 1];

                    var vector3To = screenPosition.Vector3;
                    vector3To.y += _floorAdjust;
                    
                    screenPositionPrev.transform.DOMove(vector3To, tweenTimeSeconds).SetEase(Ease.Linear);
                    screenPositionPrev.transform.DORotate(new Vector3(0, screenPosition.Rotation, 0), 3)
                        .SetEase(Ease.Linear);
                }

            }
        }

        public void GrandFinale()
        {
            StartCoroutine(DoTeleportation(9, true));
        }

        public void SetSkybox(bool useFinale)
        {
            if (!useFinale)
            {
                RenderSettings.skybox = _skybox1;
            }
            else
            {
                RenderSettings.skybox = _skybox2;

                GameObject vpContainer = GameObject.Find("SkyboxVideoPlayer");
                if (vpContainer != null)
                {
                    VideoPlayer vp = vpContainer.GetComponent<VideoPlayer>();
                    vp.Play();
                }
            }
        }
    }
}
