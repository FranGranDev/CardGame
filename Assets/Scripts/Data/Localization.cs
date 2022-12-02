using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using UnityEngine;

public static class Localization
{
    private static string json;
    private static string currantLanguage
    {
        get => DataBase.Language;
        set => DataBase.Language = value;
    }
    public static readonly List<string> AllLanguages = new List<string>(){ "en_US", "ru_RU"};

    private static MonoBehaviour monoBehaviour;
    private static Coroutine loadDataCoroutine;

    public static LanguageData CurrantData { get; private set; }


    
    public static void LoadLanguage(string language, MonoBehaviour mono, System.Action<LanguageData> callback = null)
    {
        if(!AllLanguages.Contains(language))
        {
            Debug.LogError($"No such language finded: {language}");
            return;
        }

        monoBehaviour = mono;
        currantLanguage = language;

        string path = Application.streamingAssetsPath + "/Languages/" + currantLanguage + ".json";


#if UNITY_ANDROID && !UNITY_EDITOR
        LoadDataAndroid(path, callback);
#elif UNITY_EDITOR
        LoadDataPC(path, callback);
#endif
    }
    private static void LoadDataPC(string path, System.Action<LanguageData> callback = null)
    {
        json = File.ReadAllText(path);
        LanguageData data = JsonUtility.FromJson<LanguageData>(json);

        CurrantData = data;
        callback?.Invoke(data);
    }
    private static void LoadDataAndroid(string path, System.Action<LanguageData> callback = null)
    {
        if(loadDataCoroutine == null)
        {
            loadDataCoroutine = monoBehaviour.StartCoroutine(LoadDataAndroidCour(path, callback));
        }
    }
    private static IEnumerator LoadDataAndroidCour(string path, System.Action<LanguageData> callback)
    {
        WWW www = new WWW(path);
        yield return www;
        LanguageData data = JsonUtility.FromJson<LanguageData>(www.text);

        CurrantData = data;
        callback?.Invoke(data);
        loadDataCoroutine = null;
        yield break;
    }
}

[System.Serializable]
public class LanguageData
{
    public LobbyData Lobby;
    public GameData Game;
}

[System.Serializable]
public struct LobbyData
{
    //Lobby Start
    public string EnterPlayerName;
    public string PlayerName;

    public string CreateRoom;
    public string JoinRandomRoom;

    public string Stats;

    public string EnterText;
    public string Loading;

    //RoomCreate
    public string RoomName;
    public string Create;
    public string Back;

    public string Ready;
    public string StartGame;
    public string Leave;

    //StatsPanel
    public string GameStats;
    public string MatchcesCount;
    public string Victories;
    public string Defeats;
    public string WinRate;
    public string LastMatches;
}

[System.Serializable]
public struct GameData
{
    public string Pass;
    public string EnemyTurn;
    public string YourTurn;
    public string Take;
    public string Done;
    public string Winner;
    public string Looser;
    public string MoveCount;
    public string Exit;
    public string Ready;
    public string Wait;
    public string WaitOthers;
    public string IamReady;
    public string PlayAgain;
    public string YouWin;
    public string YouLose;
}