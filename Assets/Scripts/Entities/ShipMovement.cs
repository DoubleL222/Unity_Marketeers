using System;
using Market;
using UnityEngine;
using Systems;
using AssetStore.SplitScreenAudio.Code;
using Random = UnityEngine.Random;

namespace Entities
{
    public class ShipMovement : MonoBehaviour
    {
        enum ShipState
        {
            Stopped,
            WaitingBetweenPlayers,
            WaitingAtPort,
            FollowingWaypoints
        }
        ShipState _state;
        
        public Transform[] playerStartPositions;
        public GameObject[] shipShopColliders;

        private Vector3 destination;
        private Vector3 originalRotation;
        private Vector3 currWayPoint;
        private Vector3[] wayPoints;
        private int _wayPointIndexer = 0;
        private int _playerIndexer = 0;

        public float DelayBeforeStart;
        public float DelayBetweenPlayers;
        public float TimeToWaitAtPort;
        private float _waitTimer;
        private float _timeSinceRoundStarted, _timeSinceStartedAtPlayer;

        public AudioClip bellSound, leavePortSound;

        private Quaternion targetRotation;
        private Rigidbody _rb;
        private float originalDrag;
        private float tW;
        public float thrust;
        public float rotationSpeed;
        public float maxSpeed;

        private int _startPlayer;
        public int NumOfRoundsLeft { get; private set; }
        public bool MovementStopped { get; set; }

        private VirtualAudioSource _audioSourceLocal;

        private GameObject shipMeshContainer;

        public Action OnStartAtNextPlayerWaypoint, OnDockedAtPort;
        /// <summary>
        /// Parameter is the playerindex of the port the ship just left.
        /// </summary>
        public Action<int> OnLeavePort;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _audioSourceLocal = GetComponent<VirtualAudioSource>();

            originalDrag = _rb.drag;
            originalRotation = transform.eulerAngles;
            targetRotation = Quaternion.LookRotation(destination - transform.position);
            transform.rotation = targetRotation;
            _rb.velocity = Vector3.zero;

            _startPlayer = Random.Range(0, 4);
            //_startPlayer = 0;
            Debug.Log("Ship set to start at player " + (_startPlayer + 1));
            _playerIndexer = _startPlayer;
            NumOfRoundsLeft = MarketeersGameManager.Instance.ShipRoundsPerGame;
        }

        // Use this for initialization
        void Start()
        {
            _audioSourceLocal.mySource = SoundManager.Instance.ShipLocalFx;
            wayPoints = new Vector3[24];
            for (int i = 0; i < wayPoints.Length; i++)
            {
                wayPoints[i] = GameObject.Find("WayPoint (" + i + ")").GetComponent<Transform>().position;
            }

            shipMeshContainer = GameObject.Find("watercraftPack_007");

            if (DelayBeforeStart > 0)
                StartWaitingBetweenPlayers(DelayBeforeStart);
            else
                // Move ship to starting position, and set waypoint etc.
                TeleportAndStartMovingTowardsPlayerIsland(_playerIndexer);
        }

        // Update is called once per frame
        void Update()
        {
            _timeSinceRoundStarted += Time.deltaTime;
            _timeSinceStartedAtPlayer += Time.deltaTime;

            switch (_state)
            {
                case ShipState.WaitingBetweenPlayers:
                    _waitTimer -= Time.deltaTime;
                    if(_waitTimer < 0)
                        TeleportAndStartMovingTowardsPlayerIsland(_playerIndexer);
                    break;

                case ShipState.WaitingAtPort:
                    _waitTimer -= Time.deltaTime;
                    if (_waitTimer < 0)
                        LeavePort();
                    break;

                case ShipState.FollowingWaypoints:
                    break;

                default:
                    break;
            }
        }

        void FixedUpdate()
        {
            switch (_state)
            {
                case ShipState.WaitingBetweenPlayers:
                    break;
                case ShipState.WaitingAtPort:
                    break;
                case ShipState.FollowingWaypoints:
                    targetRotation = Quaternion.LookRotation(destination - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

                    _rb.AddForce(transform.forward * thrust);

                    if (_rb.velocity.magnitude > maxSpeed)
                        _rb.velocity = _rb.velocity.normalized * maxSpeed;
                    break;
                default:
                    break;
            }
        }

        void StartWaitingBetweenPlayers(float time)
        {
            Debug.Log("Waiting " + time.ToString("F1") + " seconds between players...");
            _state = ShipState.WaitingBetweenPlayers;

            // Make ship mesh invisible.
            shipMeshContainer.SetActive(false);

            _waitTimer = time;
        }

        private void TeleportAndStartMovingTowardsPlayerIsland(int playerIndex)
        {
            _state = ShipState.FollowingWaypoints;
            
            // Make ship mesh visible.
            shipMeshContainer.SetActive(true);

            if(playerIndex == _startPlayer)
                _timeSinceRoundStarted = 0.0f;

            _timeSinceStartedAtPlayer = 0f;
            Debug.Log("Ship moving towards player " + (playerIndex + 1));

            transform.position = playerStartPositions[playerIndex].position;
            transform.eulerAngles = originalRotation;
            _rb.velocity = Vector3.zero;
            _rb.drag = originalDrag;
            _wayPointIndexer = _playerIndexer * 6;
            destination = wayPoints[_wayPointIndexer];
            currWayPoint = transform.position;

            targetRotation = Quaternion.LookRotation(destination - transform.position);
            transform.rotation = targetRotation;

            // Play bell sound
            SoundManager.Instance.ShipGlobalFx.clip = bellSound;
            SoundManager.Instance.ShipGlobalFx.Play();

            if (OnStartAtNextPlayerWaypoint != null)
                OnStartAtNextPlayerWaypoint.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("WayPoint"))
            {
                SetNextWaypoint();
            }

            else if (other.CompareTag("StopWayPoint"))
            {
                StartWaitingAtPort();
            }

            else if (other.CompareTag("NextPlayerWayPoint"))
            {
                OnFinalPlayerWaypointReached();
            }
        }

        void SetNextWaypoint()
        {
            _wayPointIndexer++;
            destination = wayPoints[_wayPointIndexer];
            currWayPoint = transform.position;
        }

        void StartWaitingAtPort()
        {
            _state = ShipState.WaitingAtPort;
            Debug.Log("Waiting at player " + GetCurrentTargetPlayerNumber() + " port!");

            _waitTimer = TimeToWaitAtPort;
            _rb.drag = 1.1f;

            var shopCollider = shipShopColliders[_playerIndexer];
            if (shopCollider != null)
                shopCollider.SetActive(true);

            _wayPointIndexer++;

            destination = wayPoints[_wayPointIndexer];
            currWayPoint = transform.position;

            if (OnDockedAtPort != null)
                OnDockedAtPort.Invoke();
        }

        void LeavePort()
        {
            _state = ShipState.FollowingWaypoints;
            //Debug.Log("Leaving Port!");

            _rb.drag = originalDrag;
            //maxSpeed += 10;
            //thrust += 4;
            
            var shopCollider = shipShopColliders[_playerIndexer];
            if (shopCollider != null)
                shopCollider.SetActive(false);

            _rb.drag = originalDrag;

            var lastPlayerIndex = _playerIndexer;

            // Find the next player to move towards
            _playerIndexer = GetNextPlayerIndex();

            if (_playerIndexer == _startPlayer)
                NumOfRoundsLeft--;

            // Play leaving port sound
            _audioSourceLocal.clip = leavePortSound;
            _audioSourceLocal.Play();

            if (OnLeavePort != null)
                OnLeavePort.Invoke(lastPlayerIndex);
        }

        public void OnFinalPlayerWaypointReached()
        {
            //Debug.Log("SHIP => Time spent from starting at player " + GetCurrentTargetPlayerNumber() + " to the last waypoint: " + _timeSinceStartedAtPlayer);
            _timeSinceStartedAtPlayer = 0f;

            if (_playerIndexer == _startPlayer)
            {
                //Debug.Log("SHIP => Sailing time for a full round: " + _timeSinceRoundStarted);
                // Start over on the waypoints index.
                _wayPointIndexer = 0;
            }

            // If the ship is set to wait for a time between players, make it wait.
            if (DelayBetweenPlayers > 0)
                StartWaitingBetweenPlayers(DelayBetweenPlayers);
            else
                // Move ship to starting position, and set waypoint etc.
                TeleportAndStartMovingTowardsPlayerIsland(_playerIndexer);
        }

        private int GetNextPlayerIndex()
        {
            if (_playerIndexer == playerStartPositions.Length - 1)
                return 0;
            return _playerIndexer + 1;
        }

        public int GetCurrentTargetPlayerNumber()
        {
            return _playerIndexer + 1;
        }

        public void StopShipMovement()
        {
            Debug.Log("Ship was stopped!");
            _state = ShipState.Stopped;
        }

        public bool IsInPort()
        {
            return _state == ShipState.WaitingAtPort;
        }
    }
}