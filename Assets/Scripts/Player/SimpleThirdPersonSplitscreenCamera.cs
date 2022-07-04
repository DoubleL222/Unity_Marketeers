using UnityEngine;

namespace Player
{
    public class SimpleThirdPersonSplitscreenCamera : MonoBehaviour
    {
        public Transform PlayerTransform;
        private Vector3 _lastPlayerPosition;
        private Transform _cameraTransform;
        public PlayerInput PlayerInput;
        public Transform Island;
        public Vector3 IslandViewOffset;
        public Vector3 IslandViewRotation;
        public Vector3 PlayerViewOffset;
        public Vector3 PlayerViewRotation;
        public float FollowSmoothTime = 1f;
        public float ZoomSmoothTime = 0.5f;
        public float MaxFollowMoveSpeed = 15f;
        public float MaxZoomMoveSpeed = 65f;
        public float MaxRotationSpeed = 15f;
        public float LookAheadFactor = 100f;
        private Vector3 _currentDestination;
        private bool _zoomingOut;
        private Vector3 _refPositionSmoothDampVar, _refRotationSmoothDampVar;


        // TESTING
        public float turnSpeed = 10.0f;

        private Vector3 offsetX;
        private Vector3 offsetY;
        private Vector3 offset;
        public float DistanceUp = 3f, DistanceAway = 5f;

        void Start()
        {
            _cameraTransform = transform;
            _lastPlayerPosition = PlayerTransform.position;
            PlayerInput.OnZoomOutDown += () => { ToggleZoom(true); };
            PlayerInput.OnZoomOutUp += () => { ToggleZoom(false); };

            // TESTING
            //offsetX = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y + 5f, PlayerTransform.position.z + 7f);
            //offsetY = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y, PlayerTransform.position.z + 7f);
            offset = new Vector3(0, 5f, 7f);

            switch (PlayerInput.playerNumber)
            {
                case 1:
                    GetComponent<Camera>().rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                    break;
                case 2:
                    GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    break;
                case 3:
                    GetComponent<Camera>().rect = new Rect(0f, 0f, 0.5f, 0.5f);
                    break;
                case 4:
                    GetComponent<Camera>().rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                    break;
            }
        }

        void Update()
        {
            // TODO Hmmm...maybe split up zooming and follow position alterations? Not sure how, though...

            //if (ZoomingOut)
            //{
            //    // Rotate towards IslandViewRotation
            //    _cameraTransform.rotation = Quaternion.Euler(Vector3.SmoothDamp(_cameraTransform.rotation.eulerAngles,
            //        IslandViewRotation, ref _refRotationSmoothDampVar, FollowSmoothTime, MaxRotationSpeed));

            //    // Move towards IslandViewOffset
            //    if (_cameraTransform.position != _currentDestination)
            //        _cameraTransform.position = Vector3.SmoothDamp(_cameraTransform.position, _currentDestination,
            //            ref _refPositionSmoothDampVar, FollowSmoothTime, MaxZoomMoveSpeed);
            //}
            //else {
            //    // Set new destination depending on current player position
            //    _currentDestination = PlayerTransform.position - ((_lastPlayerPosition - PlayerTransform.position) * LookAheadFactor) + PlayerViewOffset;

            //    // Rotate towards PlayerViewRotation
            //    _cameraTransform.rotation = Quaternion.Euler(Vector3.SmoothDamp(_cameraTransform.rotation.eulerAngles, PlayerViewRotation, ref _refRotationSmoothDampVar, FollowSmoothTime, MaxRotationSpeed));

            //    if (_cameraTransform.position != _currentDestination){
            //        // If zooming in...
            //        if (_cameraTransform.position.y - PlayerTransform.position.y > PlayerViewOffset.y + 0.1f)
            //        {
            //            _cameraTransform.position = Vector3.SmoothDamp(_cameraTransform.position, _currentDestination, ref _refPositionSmoothDampVar, ZoomSmoothTime, MaxZoomMoveSpeed);
            //        }
            //        // If normally following player
            //        else {
            //            _cameraTransform.position = Vector3.SmoothDamp(_cameraTransform.position, _currentDestination, ref _refPositionSmoothDampVar, FollowSmoothTime, MaxFollowMoveSpeed);
            //        }
            //    }
            //}

            _lastPlayerPosition = PlayerTransform.position;
        }

        private void ToggleZoom(bool zoomOut)
        {
            if (zoomOut)
                _currentDestination = Island.position + IslandViewOffset;

            _zoomingOut = zoomOut;
        }

        // TESTING
        //void LateUpdate()
        //{
        //    var characterOffset = PlayerTransform.position + (DistanceUp * PlayerTransform.up) - (DistanceAway * _cameraTransform.forward);
        //    Vector3 rigToGoal = characterOffset - PlayerTransform.position;
        //    rigToGoal.y = 0f;

        //    _cameraTransform.RotateAround(characterOffset, PlayerTransform.up, 360 * PlayerInput.CameraMovementThisFrame.x * Time.deltaTime);
        //_cameraTransform.position = characterOffset + PlayerTransform.up * DistanceUp - rigToGoal * DistanceAway;


        //offsetX = Quaternion.AngleAxis(PlayerInput.CameraMovementThisFrame.x * turnSpeed, Vector3.up) * offsetX;
        //offsetY = Quaternion.AngleAxis(PlayerInput.CameraMovementThisFrame.y * turnSpeed, Vector3.right) * offsetY;
        //_cameraTransform.position = PlayerTransform.position + offsetX + offsetY;
        //_cameraTransform.LookAt(PlayerTransform.position);
        //}
        public GameObject target;

        public float rotateSpeed = 5;

        void LateUpdate()
        {
            //float horizontal = PlayerInput.CameraMovementThisFrame.x * rotateSpeed;
            //float vertical = PlayerInput.CameraMovementThisFrame.y * rotateSpeed;
            //PlayerTransform.Rotate(0, horizontal, vertical);
            //PlayerTransform.Rotate(0, horizontal, 0);

            float desiredAngle = PlayerTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
            transform.position = PlayerTransform.position + (rotation * offset);

            _cameraTransform.LookAt(PlayerTransform);
        }
    }
}
