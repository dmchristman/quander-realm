/* Code by Srivathsan Prakash (Filament), Rob Frank (Filament), Tianle Liu (UChicago)
 * 
 * This class uploads/downloads players' save data by communicating with a firebase app
 * It also sends analytics data to an AWS server hosted by UChicago (Implemented by Tianle)
 *   All AWS related code is marked with a comments: "// AWS"
 */
using System.Collections;
using UnityEngine;
using BeauRoutine;
using System;
using UnityEngine.Networking;
using System.Linq;
using System.Threading.Tasks;

#if !UNITY_WEBGL
using Firebase;
using Firebase.Database;
#else
using System.Runtime.InteropServices;
#endif

namespace Wrapper
{
    public class SaveManager : MonoBehaviour
    {
        [HideInInspector] public bool isUserLoggedIn = false;
        [HideInInspector] public UserSave currentUserSave = null;
        public int researchCodeLength = 6;

        private float networkRequestTimeout = 5f;
        private bool isDatabaseReady = false;
        // private string[] gameSaveURLs = new string[5] { "blackbox", "circuits", "twintanglement", "queuebits", "qupcakery"}; // AWS
        private string[] gameSaveURLs = new string[6] { "blackbox", "circuits", "twintanglement", "queuebits", "qupcakery", "rewards"}; // AWS
        public readonly string awsURL = "https://backend-quantime.link/"; // AWS
        private bool isConnectedToInternet = false;

#if !UNITY_WEBGL
        private FirebaseApp app;
        private DatabaseReference dbReference;
        private DataSnapshot databaseSnapshot;
        private bool uploadSuccess = false;
#else
        private string researchCodeExists = "";
        private string loadDataJson = "";
        private bool webGLUploadSuccess = false;
#endif

#if PRODUCTION_FB
        public static readonly string firebaseURL = "https://quander-production-default-rtdb.firebaseio.com/";
        public readonly string testConnectionURL = "https://console.firebase.google.com/project/quander-production/database/quander-production-default-rtdb/data";
#else
        public static readonly string firebaseURL = "https://filament-zombies-default-rtdb.firebaseio.com/";
        public readonly string testConnectionURL = "https://console.firebase.google.com/project/filament-zombies/database/filament-zombies-default-rtdb/data";
#endif

        private void Awake()
        {
            Routine.Start(InitFirebase());
        }

        private void OnEnable()
        {
            Events.AddReward += AddReward;
            Events.ClearSaveFile += ClearSave;
            Events.SubmitResearchCode += Login;
            Events.IsRewardUnlocked += IsRewardUnlocked;
            Events.UpdateRemoteSave += UpdateRemoteSave;
            Events.GetPlayerResearchCode += GetResearchCode;
            Events.GetMinigameSaveData += GetMinigameSaveData;
            Events.UpdateMinigameSaveData += UpdateMinigameSaveData;
            Events.SaveMinigameResearchData += SaveMinigameResearchData; // AWS
            Events.GetRewardDialogStats += GetRewardStatsForDialog;
            Events.SetRewardTextSeen += ToggleRewardDialogueSeen;
            Events.GetFirstRewardBool += GetHasFirstReward;
        }

        private void OnDisable()
        {
            Events.AddReward -= AddReward;
            Events.ClearSaveFile -= ClearSave;
            Events.SubmitResearchCode -= Login;
            Events.IsRewardUnlocked -= IsRewardUnlocked;
            Events.UpdateRemoteSave -= UpdateRemoteSave;
            Events.GetPlayerResearchCode -= GetResearchCode;
            Events.GetMinigameSaveData -= GetMinigameSaveData;
            Events.UpdateMinigameSaveData -= UpdateMinigameSaveData;
            Events.SaveMinigameResearchData -= SaveMinigameResearchData; // AWS
            Events.GetRewardDialogStats -= GetRewardStatsForDialog;
            Events.SetRewardTextSeen -= ToggleRewardDialogueSeen;
            Events.GetFirstRewardBool -= GetHasFirstReward;
        }

#if !UNITY_WEBGL
        private IEnumerator InitFirebase()
        {
            bool isFirebaseReady = false;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                DependencyStatus dependencyStatus = task.Result;

                if (dependencyStatus != DependencyStatus.Available)
                    Debug.LogError("Error: Could not resolve all Firebase dependencies");
                else
                {
                    app = FirebaseApp.DefaultInstance;
                    app.Options.DatabaseUrl = new System.Uri(firebaseURL);
                    isFirebaseReady = true;
                }
            });

            yield return Routine.Race(
                Routine.WaitCondition(() => isFirebaseReady),
                Routine.WaitSeconds(networkRequestTimeout));

            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            dbReference.KeepSynced(true);
        }
#else
        private IEnumerator InitFirebase()
        {
            yield return null;
        }
#endif

        #region Login

        public void Logout()
        {
            currentUserSave = null;
            isUserLoggedIn = false;
        }

        private void Login(string researchCode)
        {
            isUserLoggedIn = false;
            currentUserSave = new UserSave();
            Routine.Start(LoginRoutine(researchCode));
        }

#if LITE_VERSION
        private IEnumerator LoginRoutine(string researchCode)
        {
            currentUserSave.id = "GUEST"; 
	        Events.UpdateLoginStatus?.Invoke(LoginStatus.Success);
            isUserLoggedIn = true;
            return null;
	    }

#elif !UNITY_WEBGL
        private IEnumerator LoginRoutine(string researchCode)
        {
            // Test internet connection
            yield return TestInternetConnection();
            if (!(isConnectedToInternet))
            {
                Debug.LogError("Error: Internet connection issue");
                Events.UpdateLoginStatus?.Invoke(LoginStatus.ConnectionError);
                yield break;
            }

            // Get Database Snapshot
            yield return GetDatabaseSnapshot();
            if (!(isDatabaseReady))
            {
                Debug.LogError("Error: Could not retrieve database snapshot");
                Events.UpdateLoginStatus?.Invoke(LoginStatus.DatabaseError);
                yield break;
            }

            // Check Research Code format
            string formattedCode = researchCode.Trim();
            if (researchCode.Length != researchCodeLength || !(researchCode.All(char.IsLetterOrDigit)))
            {
                Debug.LogErrorFormat("Error: Research code must be a {0} character long alphanumeric string", researchCodeLength);
                Events.UpdateLoginStatus?.Invoke(LoginStatus.FormatError);
                yield break;
            }

            // Check if user exists
            bool isUserVerified = databaseSnapshot.Child("researchCodes").HasChild(formattedCode);
            if (!(isUserVerified))
            {
                Debug.LogErrorFormat("Error: Could not find user {0} in database", formattedCode);
                Events.UpdateLoginStatus?.Invoke(LoginStatus.NonExistentUserError);
                yield break;
            }

            // Check if user's save data exists, create new save file if not
            bool isUserDataPresent = databaseSnapshot.Child("userData").HasChild(formattedCode);  
            if (!isUserDataPresent)
            {
                currentUserSave.id = formattedCode;

                UpdateRemoteSave(); 
                yield return Routine.Race(
                    Routine.WaitCondition(() => uploadSuccess == true), 
                    Routine.WaitSeconds(networkRequestTimeout));

                //yield return Routine.Race(
                //    UpdateRemoteSave(),
                //    Routine.WaitCondition(() => uploadSuccess == true),
                //    Routine.WaitSeconds(networkRequestTimeout));

                yield return GetDatabaseSnapshot();
            }

            // Load save file and complete login
            currentUserSave = JsonUtility.FromJson<UserSave>(
                databaseSnapshot.Child("userData").Child(formattedCode).GetRawJsonValue());

            Events.UpdateLoginStatus?.Invoke(LoginStatus.Success);
            Events.SetNewPlayerStatus?.Invoke(currentUserSave.IsNewSave());
            isUserLoggedIn = true;
        }
#else
        private IEnumerator LoginRoutine(string researchCode)
        {
            yield return TestInternetConnection();
            if (!(isConnectedToInternet))
            {
                Debug.LogError("Error: Internet connection issue");
                Events.UpdateLoginStatus?.Invoke(LoginStatus.ConnectionError);
                yield break;
            }

            // Get Database Snapshot
            yield return GetDatabaseSnapshot();
            if (!(isDatabaseReady))
            {
                Debug.LogError("Error: Could not retrieve database snapshot");
                Events.UpdateLoginStatus?.Invoke(LoginStatus.DatabaseError);
                yield break;
            }

            // Check Research Code format
            string formattedCode = researchCode.Trim();
            if (researchCode.Length != researchCodeLength || !(researchCode.All(char.IsLetterOrDigit)))
            {
                Debug.LogErrorFormat("Error: Research code must be a {0} character long alphanumeric string", researchCodeLength);
                Events.UpdateLoginStatus?.Invoke(LoginStatus.FormatError);
                yield break;
            }

            // Check if code exists
            DoesResearchCodeExist(formattedCode);
            while (string.IsNullOrEmpty(researchCodeExists))
                yield return null;

            if (researchCodeExists == "F")
            {
                researchCodeExists = "";
                Debug.LogErrorFormat("Error: Could not find user {0} in database", formattedCode);
                Events.UpdateLoginStatus?.Invoke(LoginStatus.NonExistentUserError);
                yield break;
            }
            researchCodeExists = "";

            // Code exists, great. Now let's see if they have save data already
            loadDataJson = "";
            LoadData(formattedCode);
            while (string.IsNullOrEmpty(loadDataJson))
                yield return null;

            // Save file doesn't exist, create a new one
            Debug.Log(loadDataJson);
            if (loadDataJson == "none")
            {
                currentUserSave.id = formattedCode;

                UpdateRemoteSave(); 
                yield return Routine.Race(
                    Routine.WaitCondition(() => webGLUploadSuccess == true), 
                    Routine.WaitSeconds(networkRequestTimeout));

                loadDataJson = "";
                LoadData(formattedCode);
                while (string.IsNullOrEmpty(loadDataJson))
                    yield return null;
            }

            currentUserSave = JsonUtility.FromJson<UserSave>(loadDataJson);
            Events.UpdateLoginStatus?.Invoke(LoginStatus.Success);
            Events.SetNewPlayerStatus?.Invoke(currentUserSave.IsNewSave());
            isUserLoggedIn = true;
        }
#endif

#if !UNITY_WEBGL
            private IEnumerator GetDatabaseSnapshot()
        {
            if (dbReference == null)
                yield break;

            dbReference.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                    Debug.LogErrorFormat("db snapshot task is faulted: {0}", task.Exception);

                else if (task.IsCompleted)
                {
                    databaseSnapshot = task.Result;
                    isDatabaseReady = true;
                }
            });

            yield return Routine.Race(
                Routine.WaitCondition(() => isDatabaseReady),
                Routine.WaitSeconds(networkRequestTimeout));
        }
#else
        private IEnumerator GetDatabaseSnapshot()
        {
            isDatabaseReady = true;
            yield return null;
        }
#endif

#if !UNITY_WEBGL
        private IEnumerator TestInternetConnection()
        {
            isConnectedToInternet = false;

            UnityWebRequest request = new UnityWebRequest(testConnectionURL);
            request.timeout = (int)networkRequestTimeout;
            yield return request.SendWebRequest();

            if (request.error == null && request.result != UnityWebRequest.Result.ConnectionError)
                isConnectedToInternet = true;
        }

#else
        private IEnumerator TestInternetConnection()
        {
            isConnectedToInternet = false;

            UnityWebRequest request = new UnityWebRequest(Application.absoluteURL);
            request.timeout = (int)networkRequestTimeout;
            yield return request.SendWebRequest();

            if (request.error == null && request.result != UnityWebRequest.Result.ConnectionError)
                isConnectedToInternet = true;
        }
#endif

#endregion

#region UserSave

        private bool AddReward(string rewardID)
        {
            bool rewardAdded = currentUserSave.AddReward(rewardID);
            UpdateRemoteSave();

            return rewardAdded;
        }

        private bool IsRewardUnlocked(string rewardID)
        {
            return currentUserSave.HasReward(rewardID);
        }

        private string GetMinigameSaveData(Game game)
        {
            return currentUserSave.GetMinigameSave(game);
        }

        private void UpdateMinigameSaveData(Game game, object minigameSave)
        {
            currentUserSave.UpdateMinigameSave(game, minigameSave);
            UpdateRemoteSave();
        }

        private void UpdateRemoteSave()
        {
            Routine.Start(UpdateRemoteSaveRoutine());
        }

#if LITE_VERSION
        private IEnumerator UpdateRemoteSaveRoutine()
        {
            return null;
        }
#elif !UNITY_WEBGL
        private IEnumerator UpdateRemoteSaveRoutine()
        {
            uploadSuccess = false;

            if (dbReference == null)
            {
                Debug.LogError("No database reference on save");
                yield break;
            }

            if (currentUserSave.id.Equals(string.Empty))
            {
                Debug.LogError("Aborting save attempt: Empty research code in current player's save file (UserSave.id). " +
                    "Please make sure the player is logged in before saving.");
                yield break;
            }

            string json = JsonUtility.ToJson(currentUserSave);
            if (json.Equals(string.Empty))
            {
                Debug.LogError("Empty UserSave");
                yield break;
            }

            Task uploadTask = dbReference.Child("userData").Child(currentUserSave.id).SetRawJsonValueAsync(json);

            yield return Routine.Race(
                Routine.WaitCondition(() => uploadTask.Status == TaskStatus.RanToCompletion),
                Routine.WaitSeconds(networkRequestTimeout));

            if (uploadTask.Status == TaskStatus.RanToCompletion)
            {
                Events.ToggleUploadFailurePopup?.Invoke(false);
                uploadSuccess = true;
            }
            else
                Events.ToggleUploadFailurePopup?.Invoke(true);
        }
#else
        private IEnumerator UpdateRemoteSaveRoutine()
        {
            webGLUploadSuccess = false;
            string json = JsonUtility.ToJson(currentUserSave);
            if (json.Equals(string.Empty))
            {
                Debug.LogError("Empty UserSave");
                yield break;
            }

            // Call the JS SaveData function
            SaveData(currentUserSave.id, json);

            // Wait for whichever completes first: the SaveData callback function or the network timeout 
            yield return Routine.Race(
                Routine.WaitCondition(() => webGLUploadSuccess == true),
                Routine.WaitSeconds(networkRequestTimeout));

            if (webGLUploadSuccess)
                Events.ToggleUploadFailurePopup?.Invoke(false);
            else
                Events.ToggleUploadFailurePopup?.Invoke(true);
        }
#endif

            // AWS
            private void SaveMinigameResearchData(Game game, object minigameSave)
        {
#if !LITE_VERSION
            StartCoroutine(SendResearchDataToRemote(game, minigameSave));
#endif
        }

        // AWS
        private IEnumerator SendResearchDataToRemote(Game game, object minigameSave)
        {
            string dataJson = JsonUtility.ToJson(minigameSave);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(dataJson);

            string url = awsURL + "/" + gameSaveURLs[(int)game] + "_save";

            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Sending research data to aws failed: " + www.error);
                }
            }
        }

        private void ClearSave()
        {
            string id = currentUserSave.id;
            currentUserSave = new UserSave(id);
            UpdateRemoteSave();
        }

        //todo: merge intro dialogue methods or make it a property with get/set
        public bool HasPlayerSeenIntroDialogue()
        {
            return currentUserSave.introDialogueSeen;
        }

        public void ToggleIntroDialogueSeen(bool hasSeen)
        {
            currentUserSave.introDialogueSeen = hasSeen;
            UpdateRemoteSave();
        }

        private string GetResearchCode()
        {
            return currentUserSave.id;
        }

        public void ToggleRewardDialogueSeen(bool hasSeen)
        {
            currentUserSave.rewardDialogueSeen = hasSeen;
            UpdateRemoteSave();
        }

        (bool, bool) GetRewardStatsForDialog()
        {
            return (currentUserSave.rewardDialogueSeen, currentUserSave.HasAnyRewards());
        }

        bool GetHasFirstReward(string gamePrefix)
        {
            return currentUserSave.FirstRewardFromGame(gamePrefix);
        }

#endregion

#region WebGL dll imports

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void DoesResearchCodeExist(string codeString);
        [DllImport("__Internal")]
        private static extern void LoadData(string codeString);
        [DllImport("__Internal")]
        private static extern void SaveData(string codeString, string json);

        public void ResearchCodeCallback(string str)
        {
            researchCodeExists = str;
        }

        public void LoadDataCallback(string str)
        {
            loadDataJson = str;
        }

        public void SaveDataCallback(string str)
        {
            webGLUploadSuccess = str.Equals("success") ? true : false;
        }
#endif

#endregion
    }
}
