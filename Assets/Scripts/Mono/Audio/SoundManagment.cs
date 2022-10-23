using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Audio
{
    public class SoundManagment : MonoBehaviour
    {
        public static SoundManagment Active { get; private set; }

        [SerializeField] private SoundData Data;
        [SerializeField] private GameObject SoundPrefab;

        [SerializeField] private AudioSource MusicSource;
        [SerializeField] private List<AudioClip> musicClips;

        private List<string> playingSounds = new List<string>();

        private void DestroySoundOnEnd(AudioSource obj, float Delay, System.Action onDestroyed)
        {
            try
            {
                StartCoroutine(DestroySoundOnEndCour(obj, Delay, onDestroyed));
            }
            catch { }
        }
        private IEnumerator DestroySoundOnEndCour(AudioSource obj, float Delay, System.Action onDestroyed)
        {
            yield return new WaitForSeconds(Delay);
            while (obj != null && obj.isPlaying)
            {
                yield return new WaitForFixedUpdate();
            }
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
            onDestroyed?.Invoke();
            yield break;
        }

        public static void PlaySound(string id, Transform obj = null, float volume = 1f, float Delay = 0)
        {
            if (!Active)
                return;
            if (Active.Data.Sounds.Exists(item => item.id == id))
            {
                SoundItem ItemData = Active.Data.Sounds.Find(item => item.id == id);

                if (ItemData.maxInstance != 0 && Active.playingSounds.FindAll((x) => x == id).Count > ItemData.maxInstance)
                    return;
                GameObject SoundObject = Instantiate(Active.SoundPrefab, null);
                if (obj != null)
                {
                    SoundObject.transform.position = obj.position;
                }
                SoundObject.name = $"source: {id}";

                AudioSource SoundSource = SoundObject.GetComponent<AudioSource>();
                SoundSource.clip = ItemData.Clip;
                SoundSource.volume = ItemData.Volume * volume;
                SoundSource.pitch = ItemData.Pitch;
                SoundSource.spatialBlend = ItemData.SpatialBlend;
                SoundSource.maxDistance = ItemData.MaxDistance;

                if (SoundObject == null)
                    return;

                SoundSource.PlayDelayed(Delay);
                Active.playingSounds.Add(id);
                Active.DestroySoundOnEnd(SoundSource, Delay, () => Active.playingSounds.Remove(id));
            }
            else
            {
                Debug.LogWarning($"Звука с таким Id({id}) не найден");
            }
        }

        public static void PlaySound(string id, ref AudioSource source, float volume = 1f, float Delay = 0)
        {
            if (Active.Data.Sounds.Exists(item => item.id == id))
            {
                SoundItem ItemData = Active.Data.Sounds.Find(item => item.id == id);

                source.clip = ItemData.Clip;
                source.volume = ItemData.Volume * volume;
                source.pitch = ItemData.Pitch;
                source.spatialBlend = ItemData.SpatialBlend;
                source.maxDistance = ItemData.MaxDistance;

                source.PlayDelayed(Delay);
            }
            else
            {
                Debug.Log($"Звука с таким Id({id}) не найден");
            }
        }
    }
}