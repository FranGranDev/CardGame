using System;
using Cards;
using UnityEngine;

namespace Managament
{
    class Serialization
    {
        public static object DeserializeVector2Int(byte[] data)
        {
            Vector2Int result = new Vector2Int();

            result.x = BitConverter.ToInt32(data, 0);
            result.y = BitConverter.ToInt32(data, 4);

            return result;
        }
        public static byte[] SerializeVector2Int(object data)
        {
            Vector2Int vector = (Vector2Int)data;
            byte[] result = new byte[8];

            BitConverter.GetBytes(vector.x).CopyTo(result, 0);
            BitConverter.GetBytes(vector.y).CopyTo(result, 4);

            return result;
        }

        public static object DeserializeCardInfo(byte[] data)
        {
            return new CardInfo(BitConverter.ToInt32(data, 0), BitConverter.ToInt32(data, 4));
        }
        public static byte[] SerializeCardInfo(object data)
        {
            CardInfo info = (CardInfo)data;
            byte[] result = new byte[8];

            BitConverter.GetBytes(info.index).CopyTo(result, 0);
            BitConverter.GetBytes(info.suit).CopyTo(result, 4);

            return result;
        }

        public static object DeserializePlayerState(byte[] data)
        {
            return new PlayerWrapper.Data(BitConverter.ToInt32(data, 0), BitConverter.ToInt32(data, 4), BitConverter.ToInt32(data, 8));
        }
        public static byte[] SerializePlayerState(object data)
        {
            PlayerWrapper.Data playerData = (PlayerWrapper.Data)data;
            byte[] result = new byte[12];

            BitConverter.GetBytes(playerData.id).CopyTo(result, 0);
            BitConverter.GetBytes((int)playerData.moveState).CopyTo(result, 4);
            BitConverter.GetBytes((int)playerData.playerState).CopyTo(result, 8);

            return result;
        }
    }
}
