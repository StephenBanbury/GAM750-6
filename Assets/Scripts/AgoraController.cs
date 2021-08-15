using System;
using Assets.Scripts.Models;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using agora_gaming_rtc;

#if (UNITY_ANDROID)
using System.Collections;
using UnityEngine.Android;
#endif

namespace Assets.Scripts
{
    public class AgoraController : MonoBehaviour
    {
        public static AgoraController instance;

        // Use this for initializations
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        private ArrayList permissionList = new ArrayList();
#endif

        private static AgoraInterface _app = null;
        private static List<AgoraUser> _agoraUsers;
        //private string _homeSceneName = "JoinRoom";
        //private string _playSceneName = "MainRoom";

        [SerializeField] private string _appID = "54f15673a8fd43318b10d4e42f8dd781";
        [SerializeField] private string _roomName;
        [SerializeField] private Text _testText;

        public List<AgoraUser> AgoraUsers
        {
            get { return _joinedUsers; }
        }

        private List<AgoraUser> _joinedUsers = new List<AgoraUser>();

        void Awake()
        {
#if (UNITY_ANDROID)
            //permissionList.Add(Permission.Microphone);
            //permissionList.Add(Permission.Camera);
#endif

            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            // keep this alive across scenes
            DontDestroyOnLoad(this.gameObject);

            JoinRoom();
        }

        void Update()
        {
            CheckPermissions();
        }

        public void UserJoinsRoom(uint uid)
        {
            if (uid < 99999999)
            {
                Debug.Log("Agora: UserJoinsRoom");

                var userAlreadyJoined = _joinedUsers.Any(u => u.Uid == uid);

                if (userAlreadyJoined)
                {
                    Debug.Log($"Agora: User already joined");
                }
                else
                {
                    var newId = _joinedUsers.Any() ? _joinedUsers.Max(u => u.Id) + 1 : 1;

                    var agoraUser = new AgoraUser
                    {
                        Id = newId,
                        Uid = uid,
                        DateJoined = DateTime.UtcNow,
                        Display = false
                    };

                    _joinedUsers.Add(agoraUser);

                    Debug.Log($"Agora: Number joined: {_joinedUsers.Count}");
                    foreach (var user in _joinedUsers)
                    {
                        Debug.Log($" - Uid: {user.Uid})");
                    }

                    MediaDisplayManager.instance.CreateStreamSelectButtons();
                }
            }
        }

        public void UserLeavesRoom(uint uid)
        {
            var leavingUser = _joinedUsers.FirstOrDefault(u => u.Uid == uid);
            if (leavingUser != null)
            {
                _joinedUsers.Remove(leavingUser);

                GameObject go = GameObject.Find(uid.ToString());
                if (!ReferenceEquals(go, null))
                {
                    UnityEngine.Object.Destroy(go);
                }

                //MediaDisplayManager.instance.CreateStreamSelectButtons();
            }
        }

        public void AssignStreamToDisplay(AgoraUser agoraUser)
        {
            Debug.Log("Agora: AssignStreamToDisplay");

            // Create a GameObject and assign to this new user
            VideoSurface videoSurface = MakeImageSurface(agoraUser);

            if (!ReferenceEquals(videoSurface, null))
            {
                // configure videoSurface
                videoSurface.SetForUser(agoraUser.Uid);
                videoSurface.SetEnable(true);
                videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                videoSurface.SetGameFps(30);
            }
        }
        
        private VideoSurface MakeImageSurface(AgoraUser user)
        {
            var displaySuffix = "Tall";

            var canvasDisplayName = $"CanvasDisplay{displaySuffix}";
            var videoDisplayName = $"VideoDisplay{displaySuffix}";

            Debug.Log("Agora: MakeImageSurface");

            // find a game object to render video stream from 'uid'

            var goName = user.Uid.ToString();
            var displayId = user.DisplayId;
            var displayName = $"{goName}_{displayId}";
            var sceneId = int.Parse(displayId.ToString().Substring(0, 1));

            //Debug.Log($"In MakeImageSurface. displayName: {displayName}");

            GameObject go = GameObject.Find(displayName);

            if (!ReferenceEquals(go, null))
            {
                return null;
            }

            go = new GameObject {name = displayName};

            // To be rendered onto
            go.AddComponent<RawImage>();

            var screensContainerName = $"Screens {sceneId}";
            var screenName = $"Screen {displayId}";
            var screenVariantName = $"Screen Variant {displayId}";

            var screensContainer = GameObject.Find(screensContainerName);
            var screenObject = screensContainer.transform.Find(screenName);
            if (screenObject == null) screenObject = screensContainer.transform.Find(screenVariantName);

            //Debug.Log($"screenObject: {screenObject.name}");

            var videoDisplay = screenObject.transform.Find(videoDisplayName);
            var canvasDisplay = screenObject.transform.Find(canvasDisplayName);

            videoDisplay.gameObject.SetActive(false);
            canvasDisplay.gameObject.SetActive(true);

            //Debug.Log($"canvasDisplay: {canvasDisplay.name}");

            VideoSurface videoSurface;

            // If display canvas already has an agora video surface it will be re-used
            // otherwise, a new surface will be created
            if (canvasDisplay.transform.childCount > 0)
            {
                go = canvasDisplay.transform.GetChild(0).gameObject;
                videoSurface = go.GetComponent<VideoSurface>();
            }
            else
            {
                go.transform.SetParent(canvasDisplay.transform);
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = new Vector3(0.19f, 0.39f, 0.1f);

                videoSurface = go.AddComponent<VideoSurface>();
            }

            return videoSurface;
        }

        private void CheckAppId()
        {
            Debug.Assert(_appID.Length > 10, "Please fill in your AppId first on Game Controller object.");
        }

        /// <summary>
        ///   Checks for platform dependent permissions.
        /// </summary>
        private void CheckPermissions()
        {
#if (UNITY_ANDROID)
            foreach (string permission in permissionList)
            {
                if (!Permission.HasUserAuthorizedPermission(permission))
                {
                    Permission.RequestUserPermission(permission);
                }
            }
#endif
        }

        public void JoinRoom()
        {
            Debug.Log($"Agora: JoinRoom: {_app}");

            // create app if nonexistent
            if (ReferenceEquals(_app, null))
            {
                _app = new AgoraInterface(); // create app
                _app.LoadEngine(_appID); // load engine
            }

            var joined = _app.Join(_roomName);

            if (joined)
            {
                //SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
                //_testText.text = $"Joined {_roomName}";
            }
        }


        void OnApplicationQuit()
        {
            if (!ReferenceEquals(_app, null))
            {
                _app.Leave();
                _app.UnloadEngine();
            }
        }
    }
}