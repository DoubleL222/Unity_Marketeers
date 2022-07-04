using UnityEngine;

namespace World
{
    public class SpawnResource : MonoBehaviour
    {

        public GameObject[] resourcePrefabs;
        public GameObject resourceHolder;
        public bool spawnIron;
        public bool spawnWood;
        public bool spawnSpice;
        public bool spawnGem;

        //Iron ores
        private GameObject[] ironPrefabs = new GameObject[15];

        //Woods
        private GameObject[] woodPrefabs = new GameObject[15];

        //Spices
        private GameObject[] spicePrefabs = new GameObject[15];

        //Gems
        private GameObject[] gemPrefabs = new GameObject[15];


        //ǂŦȒƗǦǦƐȒǂ
        //ŜǺĿŦŶ¿
        //ŦŎXƗƆ
        //ǁĠȂMǝŎƂĴǝƇŦǁ
        //ǂȒǝǨŦĿǕĿǂ

        void Start()
        {
            if (spawnIron)
            {
                for (int i = 0; i < ironPrefabs.Length; i++)
                {
                    Transform[] ironSpawn = new Transform[15];
                    ironSpawn[i] = gameObject.transform.GetChild(0).transform.Find("IronSpawn (" + i + ")").GetComponent<Transform>();
                    ironPrefabs[i] = Instantiate(resourcePrefabs[0], ironSpawn[i].position, ironSpawn[i].rotation, resourceHolder.transform);
                }
                Destroy(gameObject.transform.GetChild(0).gameObject);
            }

            if (spawnWood)
            {
                for (int i = 0; i < woodPrefabs.Length; i++)
                {
                    Transform[] woodSpawn = new Transform[15];
                    woodSpawn[i] = gameObject.transform.GetChild(1).transform.Find("WoodSpawn (" + i + ")").GetComponent<Transform>();
                    woodPrefabs[i] = Instantiate(resourcePrefabs[1], woodSpawn[i].position, woodSpawn[i].rotation, resourceHolder.transform);
                }
                Destroy(gameObject.transform.GetChild(1).gameObject);
            }

            if (spawnSpice)
            {
                for (int i = 0; i < spicePrefabs.Length; i++)
                {
                    Transform[] spiceSpawn = new Transform[15];
                    spiceSpawn[i] = gameObject.transform.GetChild(2).transform.Find("SpiceSpawn (" + i + ")").GetComponent<Transform>();
                    spicePrefabs[i] = Instantiate(resourcePrefabs[2], spiceSpawn[i].position, spiceSpawn[i].rotation, resourceHolder.transform);
                }
                Destroy(gameObject.transform.GetChild(2).gameObject);
            }

            if (spawnGem)
            {
                for (int i = 0; i < gemPrefabs.Length; i++)
                {
                    Transform[] gemSpawn = new Transform[15];
                    gemSpawn[i] = gameObject.transform.GetChild(3).transform.Find("GemSpawn (" + i + ")").GetComponent<Transform>();
                    gemPrefabs[i] = Instantiate(resourcePrefabs[3], gemSpawn[i].position, gemSpawn[i].rotation, resourceHolder.transform);
                }
                Destroy(gameObject.transform.GetChild(3).gameObject);
            }
        }
    }
}
