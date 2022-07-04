using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        public bool MovementStopped { get; set; }
        private Transform PlayerTransform;
        private PlayerInput Input;
        public float MovementSpeed = 20f;
        public float RotationSpeed = 60f;
        public Camera PlayerCamera;
        private WowCamera _playerWowCamera;
        public CharacterController PlayerCharacterController;
        private ParticleSystem runParticle;
        private bool playingRunParticle = false;

        // Use this for initialization
        void Start()
        {
            runParticle = transform.Find("RunParticle").GetComponent<ParticleSystem>();
            MovementStopped = false;
            PlayerTransform = transform;
            Input = GetComponent<PlayerInput>();
            _playerWowCamera = PlayerCamera.GetComponent<WowCamera>();

            Input.OnMovement += v => { Move(new Vector2(v.x, v.y)); };
            //Input.OnMovement += v => { v.Normalize(); Move(new Vector3(v.x * Time.deltaTime, 0, -v.y * Time.deltaTime)); };

            // Make sure the player is positioned on top of whatever collider he is hovering over.
            RaycastHit hit;
            if (Physics.Raycast(PlayerTransform.position - (PlayerTransform.up * PlayerCharacterController.height / 2), -PlayerTransform.up, out hit, 50))
                PlayerTransform.position = hit.point + (PlayerTransform.up * PlayerCharacterController.height / 2);
        }

        // Update is called once per frame
        //void Update ()
        //{
        // For using the built-in controller config.
        //float horizInput = Input.GetAxis(_horizontalAxis);
        // For key-based controls which can be set in the inspector (uncomment the key-fields in this class)
        //float horizInput = Input.GetKey(key_left) ? -1 : Input.GetKey(key_right) ? 1 : 0;

        //Vector2 input = new Vector2(Input.GetAxis(_horizontalAxis), Input.GetAxis(_verticalAxis));
        //}

        void Update()
        {
            if (Input.MovementThisFrame == Vector2.zero)
            {
                if (playingRunParticle)
                {
                    playingRunParticle = false;
                    runParticle.Stop();
                }
            }

        }


        void Move(Vector2 moveVector)
        {
            if (MovementStopped || _playerWowCamera.ZoomingIn || _playerWowCamera.ZoomingOut)
                return;

            // When using an analog controller, the diagonal vectors result in a vector (1f, 1f), which is longer than 1.
            if (moveVector.magnitude > 1)
            {
                //Debug.Log("Fixing moveVector from " + moveVector + " to " + (moveVector/moveVector.magnitude));
                moveVector /= moveVector.magnitude;
            }
            //Debug.Log(moveVector);


            if (!playingRunParticle)
            {
                playingRunParticle = true;
                runParticle.Play();
            }

            var moveDirection = new Vector3(moveVector.x, 0, -moveVector.y);
            //moveDirection = PlayerTransform.TransformDirection(moveDirection) * MovementSpeed * Time.deltaTime;
            var cameraTransformedMoveDirection = PlayerCamera.transform.TransformDirection(moveDirection);

            // NOTE: Use Move() instead, if you want to move the controller in the y-axis as well.
            // SimpleMove applies gravity automatically, but ignores any given y-axis movement, so only collisions make the controller move up.
            // Also, SimpleMove applies deltaTime by itself, so you should not multiply your velocity by deltaTime manually.
            PlayerCharacterController.SimpleMove(cameraTransformedMoveDirection * MovementSpeed);
            //PlayerCharacterController.Move(moveDirection * MovementSpeed * Time.deltaTime);
            //_playerRigidbody.MovePosition(transform.position + (moveDirection * MovementSpeed * Time.deltaTime));

            // THIS IS THE ROTATION THAT WORKS! This rotates the player in the direction of the camera.
            PlayerTransform.rotation = Quaternion.Euler(0, PlayerCamera.transform.eulerAngles.y, 0);
            // This adds the rotation of the player in his current movement direction.
            PlayerTransform.rotation *= Quaternion.LookRotation(moveDirection);

            // THE SIMPLEST MOVEMENT! ONLY FOR DEBUGGING PURPOSES!
            //PlayerTransform.position += moveDirection * MovementSpeed * Time.deltaTime;

            //_playerRigidbody.rotation = Quaternion.Euler(0, PlayerCamera.transform.eulerAngles.y, 0);
            //transform.Rotate(0, Input.MovementThisFrame.x * RotationSpeed * Time.deltaTime, 0);


            //PlayerTransform.Translate(moveDirection);
            //PlayerTransform.Rotate(0, moveVector.x * RotationSpeed, 0);
            //PlayerTransform.Translate(moveDirection * MovementSpeed * Time.deltaTime);
            //PlayerTransform.Translate(PlayerTransform.forward * MovementSpeed);
            //PlayerTransform.Translate(moveVector * MovementSpeed);
        }

        // WEIRD ALTERNATIVE - START
        //public float speed = 2f;
        //public float turnSmoothing = 15f;

        //private Vector3 movement;

        //void Move(Vector3 moveVector)
        //{
        //    movement.Set(moveVector.x, 0f, moveVector.y);
        //    movement = PlayerCamera.transform.TransformDirection(movement);

        //    //movement.y = 0f;

        //    movement = movement.normalized * speed * Time.deltaTime;

        //    _playerRigidbody.MovePosition(_playerRigidbody.position + movement);

        //    if (moveVector.x != 0f || moveVector.y != 0f)
        //    {
        //        Rotating(moveVector.x, moveVector.y);
        //    }
        //}

        //void Rotating(float lh, float lv)
        //{
        //    Vector3 targetDirection = new Vector3(lh, 0f, lv);

        //    Quaternion targetRotation = Quaternion.LookRotation(movement.normalized, _playerRigidbody.transform.up);

        //    Quaternion newRotation = Quaternion.Lerp(_playerRigidbody.transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);

        //    _playerRigidbody.MoveRotation(newRotation);
        //}
        // WEIRD ALTERNATIVE - END
    }
}
