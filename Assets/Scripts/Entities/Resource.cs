using System.Collections;
using Market.Generics;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Entities
{
    public class Resource : MonoBehaviour
    {

        public ResourceType Type = ResourceType.Wood;
        public int MinYield = 1, MaxYield = 3;
        public int MinNumOfYields = 1, MaxNumOfYields = 3;
        [Range(0.1f, 10f)]
        public float SecondsBetweenYields = 1f;
        [Range(6f, 240f)]
        public float MinRespawnTime = 40f;
        [Range(6f, 240f)]
        public float MaxRespawnTime = 100f;
        public float _yieldTimer, _respawnTimer;
        public int _numOfYields;
        private BoxCollider _col;
        public bool IsActive { get; private set; }
        public bool StartActive = true;
        public ParticleSystem glitter;
        public ParticleSystem enableResource;
        public ParticleSystem disableResource;
        public GameObject YieldParticlePrefab;
        public Vector3 GatherParticlesOffset;
        public SpriteRenderer MapMarker;
        public Color DisabledOverlay;
        private bool _mapMarkerShowing;

        public class Yield
        {
            public ResourceType Type { get; set; }
            public int Amount { get; set; }

            public Yield(ResourceType type, int amount)
            {
                this.Type = type;
                this.Amount = amount;
            }
        }

        private void Start()
        {
            _col = GetComponent<BoxCollider>();
            enableResource.transform.rotation = Quaternion.identity;
            //enableResource.GetComponent<Transform>().eulerAngles = Vector3.zero;

            _yieldTimer = SecondsBetweenYields;

            // Apply constraints to not make the 
            if (MinYield > MaxYield)
                MinYield = MaxYield;

            if (MinNumOfYields > MaxNumOfYields)
                MinNumOfYields = MaxNumOfYields;

            _numOfYields = Random.Range(MinNumOfYields, MaxNumOfYields+1);

            //ToggleActive(StartActive);
            IsActive = true;

            glitter.Play();
            disableResource.Stop();
            enableResource.Stop();

            MapMarker.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        void Update()
        {
            if (!IsActive)
            {
                _respawnTimer -= Time.deltaTime;
                if (_respawnTimer < 0f)
                {
                    _numOfYields = Random.Range(MinNumOfYields, MaxNumOfYields);
                    ToggleActive(true);
                }
            }
        }

        public Yield GetYield()
        {
            if (_numOfYields <= 0)
                return null;

            _yieldTimer -= Time.deltaTime;

            if (_yieldTimer <= 0)
            {
                var ps = Instantiate(YieldParticlePrefab, transform.position + GatherParticlesOffset, transform.rotation).GetComponent<ParticleSystem>();
                ps.Play();
                Destroy(ps.gameObject, ps.main.duration + 0.1f);

                _numOfYields--;

                if (_numOfYields == 0)
                {
                    _respawnTimer = Random.Range(MinRespawnTime, MaxRespawnTime);
                    ToggleActive(false);
                }

                _yieldTimer = SecondsBetweenYields;

                return new Yield(Type, Random.Range(MinYield, MaxYield+1));
            }
            return null;
        }

        private void ToggleActive(bool activate)
        {
            IsActive = activate;

            // Disable/enable gathering collider instantly.
            _col.enabled = activate;

            // Make all children fade out over 5 seconds, and then set them inactive.
            gameObject.DoActionOnGameObjectAndChildren((o) =>
            {
                if (o != gameObject)
                {
                    // Swap the comment/uncomment status of these next lines, to enable fading.
                    //StartCoroutine(Fade(o, show, 5f));
                    //o.SetActive(activate);
                    StartCoroutine(ActivateObject(o, activate));
                }
            });

            /*
        if (activate)
        {
            enableResource.Play();
        }
        else
        {
            disableResource.Play();
        }
        */
        }

        private IEnumerator ActivateObject(GameObject go, bool show)
        {
            var meshRenderer = go.GetComponent<MeshRenderer>();
            var meshCollider = go.GetComponent<MeshCollider>();
            MapMarker.color = show ? Color.white : DisabledOverlay;
            if (show)
            {
                go.SetActive(true);
                ToggleBadge(_mapMarkerShowing);
                enableResource.Play();
                if (meshRenderer)
                    meshRenderer.enabled = false;
                if (meshCollider)
                    meshCollider.enabled = false;
                yield return new WaitForSeconds(enableResource.main.duration - 0.4f);
                if (meshRenderer)
                    meshRenderer.enabled = true;
                if (meshCollider)
                    meshCollider.enabled = true;

                glitter.Play();
            }
            else
            {
                if (meshRenderer)
                    meshRenderer.enabled = false;
                if (meshCollider)
                    meshCollider.enabled = false;
                disableResource.Play();
                glitter.Stop();
                yield return new WaitForSeconds(disableResource.main.duration);
                go.SetActive(false);
                ToggleBadge(_mapMarkerShowing);
                if (meshRenderer)
                    meshRenderer.enabled = true;
                if (meshCollider)
                    meshCollider.enabled = true;
            }
        }

        public void ToggleBadge(bool show)
        {
            MapMarker.gameObject.SetActive(show);
            _mapMarkerShowing = show;
        }

        //IEnumerator Fade(GameObject go, bool fadeIn, float time)
        //{
        //    if (fadeIn)
        //        go.SetActive(true);

        //    Renderer sr = go.GetComponent<Renderer>();
        //    if (sr == null)
        //        yield break;

        //    var targetAlpha = fadeIn ? 1f : 0f;
        //    float diffAlpha = (targetAlpha - sr.material.color.a);

        //    float counter = 0;
        //    while (counter < time)
        //    {
        //        float alphaAmount = sr.material.color.a + (Time.deltaTime * diffAlpha) / time;
        //        sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b, alphaAmount);

        //        counter += Time.deltaTime;
        //        yield return null;
        //    }
        //    sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b, targetAlpha);

        //    if (!fadeIn && counter >= time)
        //        go.SetActive(false);







        //    //var rendererComponent = go.GetComponent<Renderer>();
        //    //var elapsedTime = 0f;
        //    //var startAlpha = fadeIn ? 0f : 1f;
        //    //var endAlpha = fadeIn ? 1f : 0f;
        //    //var startColor = rendererComponent.material.color;
        //    //while (elapsedTime < time)
        //    //{
        //    //    rendererComponent.material.color = new Color(startColor.r, startColor.g, startColor.b,  Mathf.Lerp(startAlpha, endAlpha, (elapsedTime / time)));
        //    //    elapsedTime += Time.deltaTime;

        //    //    yield return null;
        //    //}
        //    //    if(!fadeIn && elapsedTime >= time)
        //    //        go.SetActive(false);
        //}
    }
}