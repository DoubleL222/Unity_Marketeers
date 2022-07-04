using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class Guide : MonoBehaviour
    {
        private bool _canSkip = false;
        public Text Loading;

        void Awake()
        {
            SceneManager.sceneLoaded += (arg0, mode) =>
            {
                _canSkip = true;
            };
        }

        void Update()
        {
            if (!_canSkip)
                return;

            if (
                Input.GetAxis("P1_Button1") != 0
                || Input.GetAxis("P1_Button2") != 0
                || Input.GetAxis("P2_Button1") != 0
                || Input.GetAxis("P2_Button2") != 0
                || Input.GetAxis("P3_Button1") != 0
                || Input.GetAxis("P3_Button2") != 0
                || Input.GetAxis("P4_Button1") != 0
                || Input.GetAxis("P4_Button2") != 0
            )
            {
                Loading.gameObject.SetActive(true);
                _canSkip = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}