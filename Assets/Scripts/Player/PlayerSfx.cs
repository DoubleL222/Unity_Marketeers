using Systems;
using AssetStore.SplitScreenAudio.Code;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerSfx : MonoBehaviour
    {
        public VirtualAudioSource GatherAudioSource { get; private set; }

        void Start()
        {
            GatherAudioSource = GetComponent<VirtualAudioSource>();

            switch (GetComponent<PlayerInput>().playerNumber)
            {
                case 1:
                    GatherAudioSource.mySource = SoundManager.Instance.Player1Sfx1Source;
                    break;
                case 2:
                    GatherAudioSource.mySource = SoundManager.Instance.Player2Sfx1Source;
                    break;
                case 3:
                    GatherAudioSource.mySource = SoundManager.Instance.Player3Sfx1Source;
                    break;
                case 4:
                    GatherAudioSource.mySource = SoundManager.Instance.Player4Sfx1Source;
                    break;
            }
        }

        public void PlayWoodGather()
        {
            GatherAudioSource.clip = SoundManager.Instance.GatherWood[Random.Range(0, SoundManager.Instance.GatherWood.Length - 1)];
            GatherAudioSource.pitch = Random.Range(0.9f, 1.1f);
            GatherAudioSource.Play();
        }

        public void PlayIronGather()
        {
            GatherAudioSource.clip = SoundManager.Instance.GatherIron;
            GatherAudioSource.pitch = Random.Range(0.85f, 1.0f);
            GatherAudioSource.Play();
        }

        public void PlaySpiceGather()
        {
            GatherAudioSource.clip = SoundManager.Instance.GatherSpices;
            GatherAudioSource.pitch = Random.Range(0.9f, 1.1f);
            GatherAudioSource.Play();
        }

        public void PlayGemGather()
        {
            GatherAudioSource.clip = SoundManager.Instance.GatherGem;
            GatherAudioSource.pitch = Random.Range(0.9f, 1.1f);
            GatherAudioSource.Play();
        }
    }
}
