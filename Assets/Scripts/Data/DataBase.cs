using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    #region Constant

    //Matches
    private const string GAME_COUNT_OF_MATCHES = "matches_count";
    private const string GAME_WINNER_NAME = "winner_name_";
    private const string GAME_LOOSER_NAME = "looser_name_";
    private const string GAME_IS_VICTORY = "is_victory_";
    private const string GAME_MOVES_COUNT = "moves_count_";
    private const string GAME_END_TYPE = "game_end_type_";
    //Lobby
    private const string LOBBY_NAME = "name";
    private const string LOBBY_ROOM = "room";
    private const string LOBBY_PREV_ROOM = "prev_room";
    //Language
    private const string LANGUAGE_CURRANT = "language";
    //Sound
    private const string SOUND_CURRANT = "sound";
    #endregion

    #region Localization

    public static string Language
    {
        get => PlayerPrefs.GetString(LANGUAGE_CURRANT, "en_US");
        set => PlayerPrefs.SetString(LANGUAGE_CURRANT, value);
    }

    #endregion

    #region Sound

    public static bool SoundMuted
    {
        get => PlayerPrefs.GetInt(SOUND_CURRANT, 0) == 1;
        set => PlayerPrefs.SetInt(SOUND_CURRANT, value ? 1 : 0);
    }

    #endregion

    #region Matches

    public static void RecordGame(MatchData data)
    {
        PlayerPrefs.SetString(GAME_WINNER_NAME + MatchCount.ToString(), data.Winner);
        PlayerPrefs.SetString(GAME_LOOSER_NAME + MatchCount.ToString(), data.Looser);
        PlayerPrefs.SetInt(GAME_IS_VICTORY + MatchCount.ToString(), data.Victory ? 1 : 0);
        PlayerPrefs.SetInt(GAME_MOVES_COUNT + MatchCount.ToString(), data.MoveCount);
        PlayerPrefs.SetInt(GAME_END_TYPE + MatchCount.ToString(), (int)data.EndType);

        MatchCount++;
    }

    public static int MatchCount
    {
        get => PlayerPrefs.GetInt(GAME_COUNT_OF_MATCHES, 0);
        set => PlayerPrefs.SetInt(GAME_COUNT_OF_MATCHES, value);
    } //PRIMARY KEY AI
    public static IEnumerable<MatchData> GetAllMatches
    {
        get
        {
            List<MatchData> data = new List<MatchData>();
            
            for(int i = 0; i < MatchCount; i++)
            {
                data.Add(new MatchData
                (
                    PlayerPrefs.GetString(GAME_WINNER_NAME + i.ToString()),
                    PlayerPrefs.GetString(GAME_LOOSER_NAME + i.ToString()),
                    PlayerPrefs.GetInt(GAME_IS_VICTORY + i.ToString()) == 1,
                    PlayerPrefs.GetInt(GAME_MOVES_COUNT + i.ToString()),
                    PlayerPrefs.GetInt(GAME_END_TYPE + i.ToString())
                )) ;
            }

            return data;
        }
    }

    #endregion

    #region Lobby

    public static string Name
    {
        get => PlayerPrefs.GetString(LOBBY_NAME, $"Player {Random.Range(0, 999)}");
        set => PlayerPrefs.SetString(LOBBY_NAME, value);
    }

    public static string Room
    {
        get => PlayerPrefs.GetString(LOBBY_ROOM, $"Room {Random.Range(0, 999)}");
        set => PlayerPrefs.SetString(LOBBY_ROOM, value);
    }

    public static string PrevRoom
    {
        get => PlayerPrefs.GetString(LOBBY_PREV_ROOM, null);
        set => PlayerPrefs.SetString(LOBBY_PREV_ROOM, value);
    }

    #endregion

    #region Objects

    public struct MatchData
    {
        public string Winner;
        public string Looser;
        public bool Victory;
        public int MoveCount;
        public Types EndType;

        public int Id;

        public enum Types
        {
            Game,
            Surrender,
            Leave,
        }

        public MatchData(string winner, string looser, bool victory, int moveCount, int endType)
        {
            Winner = winner;
            Looser = looser;
            Victory = victory;
            MoveCount = moveCount;
            EndType = (Types)endType;
            Id = -1;
        }
        public MatchData(string winner, string looser, bool victory, int moveCount, int endType, int id)
        {
            Winner = winner;
            Looser = looser;
            Victory = victory;
            MoveCount = moveCount;
            EndType = (Types)endType;
            Id = id;
        }
    }

    #endregion
}
