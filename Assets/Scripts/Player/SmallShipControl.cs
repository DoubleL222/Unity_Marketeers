using UnityEngine;
using System;
using Systems;
using AssetStore.SplitScreenAudio.Code;

public class SmallShipControl : MonoBehaviour
{

    public Action onArrivedToDestination;

    public int boatID;

    public ParticleSystem arrivalParticle;
    public ParticleSystem teleportParticle;

    public Vector3[] playerPoints;
    public Transform[] smallWayPoints;
    public Transform[] portWayPoints;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public GameObject sendInventory;

    private Vector3 destination;
    private Vector3 currWayPoint;

    private int wayPointIndexer = 0;
    private bool shouldMove = false;

    public float thrust;
    public float rotationSpeed;
    public float maxSpeed;

    private Rigidbody rb;
    private float originalDrag;
    private bool startPlayerWPs = false;
    private int sendToPlayerWithIndex = 0;
    private bool leftPort = false;
    public AudioClip SendBoatAudio;
    private VirtualAudioSource _audioSource;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        teleportParticle = transform.Find("GodRays").GetComponent<ParticleSystem>();
        _audioSource = GetComponent<VirtualAudioSource>();
        switch (boatID)
        {
            case 1:
                _audioSource.mySource = SoundManager.Instance.Player1BoatFx;
                break;
            case 2:
                _audioSource.mySource = SoundManager.Instance.Player2BoatFx;
                break;
            case 3:
                _audioSource.mySource = SoundManager.Instance.Player3BoatFx;
                break;
            case 4:
                _audioSource.mySource = SoundManager.Instance.Player4BoatFx;
                break;
        }
        originalDrag = rb.drag;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        sendInventory.SetActive(false);

        playerPoints = new Vector3[4];
        for (int i = 0; i < playerPoints.Length; i++)
        {
            //Debug.Log(GameObject.Find("Player" + i + "Start").transform.name);
            playerPoints[i] = GameObject.Find("Player" + i + "Start").transform.position;
        }

        smallWayPoints = new Transform[8];
        for (int i = 0; i < smallWayPoints.Length; i++)
        {
            smallWayPoints[i] = GameObject.Find("SmallWayPoint (" + i + ")").transform;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            Quaternion targetRotation = Quaternion.LookRotation(destination - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (shouldMove)
        {
            rb.AddForce(transform.forward * thrust);
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
    }

    public int getBoatID()
    {
        return boatID;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WayPoint") && startPlayerWPs)
        {
            wayPointIndexer++;
            destination = smallWayPoints[wayPointIndexer].position;
            currWayPoint = transform.position;
        }
        else if (other.CompareTag("WayPoint") && !startPlayerWPs)
        {

            wayPointIndexer++;
            destination = portWayPoints[wayPointIndexer].position;
            currWayPoint = transform.position;
        }
        else if (other.CompareTag("StopWayPoint"))
        {
            resetShip();
        }
        else if (other.CompareTag("NextPlayerWayPoint"))
        {
            TeleportToPlayer(sendToPlayerWithIndex);
            startPlayerWPs = true;
        }
    }

    public void sendShip(int indexToSendTo)
    {
        leftPort = true;
        sendToPlayerWithIndex = indexToSendTo;
        destination = portWayPoints[0].position;
        currWayPoint = transform.position;
        shouldMove = true;
        sendInventory.SetActive(true);
        _audioSource.clip = SendBoatAudio;
        _audioSource.Play();
    }

    public bool getLeftPort()
    {
        return leftPort;
    }

    void TeleportToPlayer(int playerIndex)
    {
        if (playerIndex == 0)
        {
            transform.position = new Vector3(playerPoints[playerIndex].x, playerPoints[playerIndex].y - 2.4f, playerPoints[playerIndex].z);
            destination = smallWayPoints[0].position;
            wayPointIndexer = 0;
        }
        else if (playerIndex == 1)
        {
            transform.position = new Vector3(playerPoints[playerIndex].x, playerPoints[playerIndex].y - 2.4f, playerPoints[playerIndex].z);
            destination = smallWayPoints[2].position;
            wayPointIndexer = 2;

        }
        else if (playerIndex == 2)
        {
            transform.position = new Vector3(playerPoints[playerIndex].x, playerPoints[playerIndex].y - 2.4f, playerPoints[playerIndex].z);
            destination = smallWayPoints[4].position;
            wayPointIndexer = 4;
        }
        else if (playerIndex == 3)
        {
            transform.position = new Vector3(playerPoints[playerIndex].x, playerPoints[playerIndex].y - 2.4f, playerPoints[playerIndex].z);
            destination = smallWayPoints[6].position;
            wayPointIndexer = 6;
        }
        rb.drag = originalDrag;
        rb.velocity = Vector3.zero;

        Quaternion targetRotation = Quaternion.LookRotation(destination - transform.position);
        transform.rotation = targetRotation;
        teleportParticle.Play();
    }

    void resetShip()
    {
        //ADDING EVENT TO REACT TO
        onArrivedToDestination.Invoke();

        Debug.Log("Tour Ended, teleport smallship back");

        ParticleSystem puffParticle = Instantiate(arrivalParticle, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.rotation);
        puffParticle.Play();
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        shouldMove = false;
        startPlayerWPs = false;
        wayPointIndexer = 0;
        sendToPlayerWithIndex = 0;
        destination = originalPosition;
        currWayPoint = originalPosition;
        rb.drag = originalDrag;
        rb.velocity = Vector3.zero;
        sendInventory.SetActive(false);
        leftPort = false;
    }
}
