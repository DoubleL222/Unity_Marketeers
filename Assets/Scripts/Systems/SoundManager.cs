using UnityEngine;
using Utils;

namespace Systems
{
    public class SoundManager : SingletonBehaviour<SoundManager>
    {
        public AudioClip[] GatherWood;
        public AudioClip 
            GatherIron,
            GatherSpices,
            GatherGem,
            MoneyBag
            ;
        public AudioSource Music, Waves, Gulls, Player1Sfx1Source, Player2Sfx1Source, Player3Sfx1Source, Player4Sfx1Source, Player1BoatFx, Player2BoatFx, Player3BoatFx, Player4BoatFx, ShipGlobalFx, ShipLocalFx;
    }
}