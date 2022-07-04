using System.Collections.Generic;
using Systems;
using Entities;
using UnityEngine;

namespace Player
{
    public class WowCamera : MonoBehaviour
    {
        public Transform target;
        private Transform _transform;
        private PlayerInput _playerInput;

        public float targetHeight = 1.7f;
        public float distance = 5.0f;
        public float offsetFromWall = 0.1f;

        public float maxDistance = 20;
        public float minDistance = .6f;
        public float speedDistance = 5;

        public float xSpeed = 200.0f;
        public float ySpeed = 200.0f;

        public int yMinLimit = -40;
        public int yMaxLimit = 80;

        public bool autoCorrectRotationWhenMoving = true;

        public int zoomRate = 40;

        public float rotationDampening = 3.0f;
        public float zoomDampening = 5.0f;

        public LayerMask collisionLayers = -1;

        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        private float currentDistance;
        private float desiredDistance;
        private float correctedDistance;

        public bool ZoomingOut { get; private set; }
        public bool ZoomingIn { get; private set; }
        public bool MapViewEnabled { get; private set; }

        private Vector3 _islandZoomoutDestination;
        public Transform Island;
        public Vector3 IslandViewOffset;
        public Vector3 IslandViewRotation;

        public float ZoomSmoothTime = 0.2f, MaxZoomMoveSpeed = 150f, MaxZoomRotationSpeed = 60f, FollowSmoothTime = 0.35f, MaxFollowMoveSpeed = 100f, MaxFollowRotationSpeed = 90f;
        private Vector3 _refPositionSmoothDampVar, _refRotationSmoothDampVar;

        private Vector3 _currentDestinationPos;
        private Quaternion _currentDestinationRot;
        public GameObject MapMarker;
        public float MapMarkerOnDelay = 2f;
        public GameObject RoundBadge, ShipBadge;
        private Resource[] _islandResources;
        private bool _showingMapMarkers;

        void Start()
        {
            _transform = transform;
            _playerInput = target.GetComponent<PlayerInput>();
            _islandZoomoutDestination = Island.position + IslandViewOffset;

            ToggleMapView(true);
            
            switch (_playerInput.playerNumber)
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

            // Weird way of setting starting angle
            //Vector3 angles = transform.eulerAngles;
            //xDeg = angles.x;
            //yDeg = angles.y;

            // Pre-calculate camera vector
            xDeg = target.eulerAngles.y;
            yDeg = yMinLimit + ((yMaxLimit - yMinLimit) / 2);

            currentDistance = distance;
            desiredDistance = distance;
            correctedDistance = distance;

            // Make the rigid body not change rotation
            if (this.gameObject.GetComponent<Rigidbody>())
                this.gameObject.GetComponent<Rigidbody>().freezeRotation = true;

            MapMarker.GetComponent<SpriteRenderer>().color = MarketeersGameManager.Instance.playerColors[_playerInput.playerNumber];

            _islandResources = transform.parent.GetComponentsInChildren<Resource>();
        }

        private void ReadInput()
        {
            //if (GUIUtility.hotControl == 0)
            //{
            // If either mouse buttons are down, let the mouse govern camera position
            // NOTE: If-statement was: Input.GetMouseButton(0) || Input.GetMouseButton(1)
            if (_playerInput.CameraMovementThisFrame != Vector2.zero)
            {
                xDeg += _playerInput.CameraMovementThisFrame.x * xSpeed * 0.02f;
                yDeg -= _playerInput.CameraMovementThisFrame.y * ySpeed * 0.02f;
            }
            // otherwise, ease behind the target if the directional axis is used
            // NOTE: Else if-statement was: Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0
            else if (autoCorrectRotationWhenMoving && _playerInput.MovementThisFrame != Vector2.zero)
            {
                float targetRotationAngle = target.eulerAngles.y;
                float currentRotationAngle = transform.eulerAngles.y;
                xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
            }
            //}

            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
        }

        void DoRotationAndPositioning()
        {
            Vector3 vTargetOffset;

            // calculate the desired distance
            // NOTE: Commented out, because my controller doesn't have a scroll wheel. May choose to bind some buttons for it later.
            //desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance) * speedDistance;
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);

            // set camera rotation
            Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);
            correctedDistance = desiredDistance;

            // calculate desired camera position
            vTargetOffset = new Vector3(0, -targetHeight, 0);
            Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

            // check for collision using the true target's desired registration point as set by user using height
            RaycastHit collisionHit;
            Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y, target.position.z) - vTargetOffset;

            // if there was a collision, correct the camera position and calculate the corrected distance
            bool isCorrected = false;
            if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers.value))
            {
                // calculate the distance from the original estimated position to the collision location,
                // subtracting out a safety "offset" distance from the object we hit.  The offset will help
                // keep the camera from being right on top of the surface we hit, which usually shows up as
                // the surface geometry getting partially clipped by the camera's front clipping plane.
                correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
                isCorrected = true;
            }

            // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
            currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

            // keep within legal limits
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            // recalculate position based on the new currentDistance
            position = target.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);

            _currentDestinationPos = position;
            _currentDestinationRot = rotation;
        }

        private void ZoomOut()
        {
            // Rotate towards IslandViewRotation
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.Euler(IslandViewRotation), MaxZoomRotationSpeed * Time.deltaTime);

            // Move towards IslandViewOffset
            if (_transform.position != _islandZoomoutDestination)
                _transform.position = Vector3.SmoothDamp(_transform.position, _islandZoomoutDestination,
                    ref _refPositionSmoothDampVar, ZoomSmoothTime, MaxZoomMoveSpeed);
        }

        void Update()
        {
            if (ZoomingIn && _showingMapMarkers)
            {
                if(Vector3.Distance(transform.position, _playerInput.transform.position) < Vector3.Distance(_playerInput.transform.position, _islandZoomoutDestination) * 0.9f)
                    ToggleMapMarkers(false);
            }
            else if (ZoomingOut && !_showingMapMarkers)
            {
                if (Vector3.Distance(transform.position, _islandZoomoutDestination) < Vector3.Distance(_playerInput.transform.position, _islandZoomoutDestination) * 0.2f)
                    ToggleMapMarkers(true);
            }

            //if (_mapMarkerTimer > 0)
            //{
            //    _mapMarkerTimer -= Time.deltaTime;
            //    if (_mapMarkerTimer < 0)
            //    {
            //        ToggleMapMarkers(true);
            //    }
            //}
        }

        /**
     * Camera logic on LateUpdate to only update after all character movement logic has been handled.
     */
        void LateUpdate()
        {
            // Don't do anything if target is not defined
            if (!target)
                return;

            if (ZoomingOut)
            {
                ZoomOut();
            }
            else
            {
                ReadInput();
                DoRotationAndPositioning();
                if (ZoomingIn)
                {
                    _transform.rotation = Quaternion.RotateTowards(_transform.rotation, _currentDestinationRot, MaxZoomRotationSpeed * Time.deltaTime);
                    _transform.position = Vector3.SmoothDamp(_transform.position, _currentDestinationPos, ref _refPositionSmoothDampVar, 0.1f, MaxZoomMoveSpeed);
                    if (Vector3.Distance(_currentDestinationPos, _transform.position) < 1)
                        ZoomingIn = false;
                }
                else
                {
                    // For setting the rotation and movement instantly, instead of smoothly moving towards the destination
                    _transform.rotation = _currentDestinationRot;
                    _transform.position = _currentDestinationPos;

                    // For trying to smooth movement and rotation. It is REALLY hard to get the settings right.
                    //Transform.rotation = Quaternion.RotateTowards(Transform.rotation, _currentDestinationRot, MaxFollowRotationSpeed * Time.deltaTime);
                    //Transform.position = Vector3.SmoothDamp(Transform.position, _currentDestinationPos, ref _refPositionSmoothDampVar, FollowSmoothTime, MaxFollowMoveSpeed);
                }
            }
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            while (angle < -360)
                angle += 360;
            while (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        private void ToggleZoom(bool zoomOut)
        {
            ZoomingOut = zoomOut;
            ZoomingIn = !zoomOut;
            
            RoundBadge.SetActive(zoomOut);
            ShipBadge.SetActive(zoomOut);
        }

        private void ToggleZoomOn()
        {
            ToggleZoom(true);
        }

        private void ToggleZoomOff()
        {
            ToggleZoom(false);
        }

        public void ToggleMapView(bool on)
        {
            if (on && !MapViewEnabled)
            {
                _playerInput.OnZoomOutDown += ToggleZoomOn;
                _playerInput.OnZoomOutUp += ToggleZoomOff;
                MapViewEnabled = true;
            }
            else if (!on && MapViewEnabled)
            {
                _playerInput.OnZoomOutDown -= ToggleZoomOn;
                _playerInput.OnZoomOutUp -= ToggleZoomOff;
                MapViewEnabled = false;
            }
        }

        private void ToggleMapMarkers(bool enable)
        {
            if (_showingMapMarkers == enable)
                return;

            _showingMapMarkers = enable;
            MapMarker.SetActive(enable);

            for (int i = 0; i < _islandResources.Length; i++)
                _islandResources[i].ToggleBadge(enable);
        }
    }
}