using System.Collections.Generic;
using Entities;
using Market.Generics;
using Player;
using UnityEngine;
using Utils;
using World;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

namespace Systems
{
    public class MarketeersGameManager : SingletonBehaviour<MarketeersGameManager> {

        //PLAYER COLORS
        public Color player1Color, player2Color, player3Color, player4Color;

        //PLAYER ID TO COLOR MAP
        public Dictionary<int, Color> playerColors;

        //Player inventories
        public Dictionary<int, PlayerInventory> playerInventories;

        // Players
        public List<PlayerInput> _playerInputs;

        //PUBLIC ASSET REFERENCES
        public Sprite multiplierSprite;

        // PRIVATE OBJECT REFERENCES
        public ShipMovement Ship;

        // PUBLIC GAME SETTING VARIABLES
        public int ShipRoundsPerGame = 15;
        public float GameEndedRestartDelay = 3f;

        private bool _gameEnded;
        private float _gameEndedTimer;
        [Range(1,3)]
        public int ResourcesPerIsland = 2;
        
        protected override void OnAwake()
        {
            // Seed Random
            UnityEngine.Random.InitState(Environment.TickCount);

            // Find all playerInput scripts, and order them by playernumber.
            _playerInputs = FindObjectsOfType<PlayerInput>().OrderByDescending(o => o.playerNumber).ToList();
            
            playerInventories = new Dictionary<int, PlayerInventory>();
            for (int i = 0; i < _playerInputs.Count; i++)
            {
                playerInventories.Add(_playerInputs[i].playerNumber, _playerInputs[i].gameObject.GetComponent<PlayerInventory>());
            }

            playerColors = new Dictionary<int, Color>();
            playerColors.Add(1, player1Color);
            playerColors.Add(2, player2Color);
            playerColors.Add(3, player3Color);
            playerColors.Add(4, player4Color);

            RandomizeIslandResources();
        }
        // Use this for initialization

        //INSTANTLY SENDS RESOURECES TO ANOTHER PLAYER FOR TRADE
        public void SendResourcesToPlayer(Dictionary<ResourceType, int> TradeOrders, int _goldAmount, int _playerDestinations)
        {
            foreach (KeyValuePair<ResourceType, int> _currOrder in TradeOrders)
            {
                playerInventories[_playerDestinations].ChangeResourceAmount(_currOrder.Key, _currOrder.Value);
            }
            playerInventories[_playerDestinations].ChangeGold(_goldAmount);
            playerInventories[_playerDestinations].UpdateResourceUI();
            
        }

        //RETURN TRUE IF SHIP IS AT PORT OF THE PLAYER WITH INDEX _PLAYERINDEX
        public bool IsShipAtPlayer(int _playerIndex)
        {
            if (Ship.IsInPort())
            {
                return Ship.GetCurrentTargetPlayerNumber() == _playerIndex;
            }
            else
            {
                return false;
            }
            
        }

    
        void Start ()
        {
            Ship = FindObjectOfType<ShipMovement>();
            Ship.OnLeavePort += ShipLeftPlayerPort;

            SoundManager.Instance.Music.Play();
            SoundManager.Instance.Gulls.Play();
        }

        private void Update()
        {
            if(_gameEnded)
            {
                if(_gameEndedTimer > 0)
                    _gameEndedTimer -= Time.deltaTime;

                if (_gameEndedTimer < 0)
                {
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
                        SceneManager.LoadScene(0);
                }
            }
        }

        void RandomizeIslandResources()
        {
            // Set up temporary array, so we can make sure that exactly two of each resource is delegated.
            var resourcesAvailable = new byte[] { (byte)ResourcesPerIsland, (byte)ResourcesPerIsland, (byte)ResourcesPerIsland, (byte)ResourcesPerIsland };

            // Create temporary list of SpawnResource scripts.
            var spawnResourceScripts = new List<SpawnResource>(FindObjectsOfType<SpawnResource>());
            short numOfIterations = 0;
            do
            {
                // Reset all their spawn-bools to false.
                // NOTE: I could get around this, if I made a flag-enum to store the results,
                // and then set the bools using the enum flags...but CBA changing it now.
                for (int i = 0; i < spawnResourceScripts.Count; i++)
                {
                    spawnResourceScripts[i].spawnWood = 
                    spawnResourceScripts[i].spawnIron = 
                    spawnResourceScripts[i].spawnSpice = 
                    spawnResourceScripts[i].spawnGem = false;
                }

                // Reset available resources
                for (int i = 0; i < resourcesAvailable.Length; i++)
                    resourcesAvailable[i] = 2;

                // Each island needs two resources.
                for (int i = 0; i < ResourcesPerIsland; i++)
                {
                    // We shuffle the order that the resource scripts are filled.
                    spawnResourceScripts.Shuffle();

                    // Run through the resource scripts one by one.
                    for (int j = 0; j < 4; j++)
                    {
                        // Delegate one wood (0), one iron (1) one spice (3) or one gem (4) resource to each script, per run.
                        for (int k = 0; k < resourcesAvailable.Length; k++)
                        {
                            // Skip any unavailable resource, and any that the spawnResourceScript already has.
                            if (resourcesAvailable[k] == 0
                                || k == 0 && spawnResourceScripts[j].spawnWood
                                || k == 1 && spawnResourceScripts[j].spawnIron
                                || k == 2 && spawnResourceScripts[j].spawnSpice
                                || k == 3 && spawnResourceScripts[j].spawnGem)
                                continue;

                            // NOTE! FAIL! j is always the same each run...I need to add the resultBits to the right
                            // number, not just to the index at j. The resultBits aren't shuffled, like the spawnResourcesScripts...
                            switch (k)
                            {
                                case 0:
                                    spawnResourceScripts[j].spawnWood = true;
                                    break;
                                case 1:
                                    spawnResourceScripts[j].spawnIron = true;
                                    break;
                                case 2:
                                    spawnResourceScripts[j].spawnSpice = true;
                                    break;
                                case 3:
                                    spawnResourceScripts[j].spawnGem = true;
                                    break;
                            }
                            resourcesAvailable[k]--;
                            break;
                        }
                    }
                }
                numOfIterations++;

                //Debug.Log("----------------------");

                //for (int i = 0; i < spawnResourceScripts.Count; i++)
                //{
                //    Debug.Log("Island " + i + ": " +
                //        (spawnResourceScripts[i].spawnWood ? "Wood" : "") +
                //        (spawnResourceScripts[i].spawnIron ? "Iron" : "") +
                //        (spawnResourceScripts[i].spawnSpice ? "Spice" : "") +
                //        (spawnResourceScripts[i].spawnGem ? "Gem" : "")
                //        );
                //}
                //Debug.Log("----------------------");
            } while (!CheckDistinctDistribution(spawnResourceScripts));
            //Debug.Log("Distribution iterations: " + numOfIterations);
        }

        private bool CheckDistinctDistribution(List<SpawnResource> spawners)
        {
            for (int i = 0; i < spawners.Count; i++)
            {
                for (int j = 0; j < spawners.Count; j++)
                {
                    if (i == j)
                        continue;
                    if (
                        spawners[i].spawnWood == spawners[j].spawnWood
                        && spawners[i].spawnIron == spawners[j].spawnIron
                        && spawners[i].spawnSpice == spawners[j].spawnSpice
                        && spawners[i].spawnGem == spawners[j].spawnGem
                        )
                        return false;
                }
            }
            return true;
        }

        void ShipLeftPlayerPort(int playerIndexOfPort)
        {
            // If the game has ended...
            if (Ship.NumOfRoundsLeft == 0)
            {
                _gameEnded = true;
                _gameEndedTimer = GameEndedRestartDelay;

                _playerInputs[0].IgnoreInput = true;
                _playerInputs[1].IgnoreInput = true;
                _playerInputs[2].IgnoreInput = true;
                _playerInputs[3].IgnoreInput = true;
                Ship.StopShipMovement();

                // Update and show the win/lose screens.
                // Sort player(inventory)s by their gold amount. Handles draws by tallying up their remaining resources,
                // using the current prices on the market. If that is also a draw, it chooses randomly.
                // The sorting is done using an IComparable on PlayerInventory.
                var playerStanding = new List<PlayerInventory>()
                {
                    playerInventories[1],
                    playerInventories[2],
                    playerInventories[3],
                    playerInventories[4]
                };

                playerStanding.Sort();

                // Access WinLoseScreen via the public canvas variable on the PlayerInventory, and update the text.
                playerStanding[0].myCanvas.GetComponentInChildren<WinLoseScreen>(true).UpdateText(1, playerStanding[0].gold);
                playerStanding[1].myCanvas.GetComponentInChildren<WinLoseScreen>(true).UpdateText(2, playerStanding[1].gold);
                playerStanding[2].myCanvas.GetComponentInChildren<WinLoseScreen>(true).UpdateText(3, playerStanding[2].gold);
                playerStanding[3].myCanvas.GetComponentInChildren<WinLoseScreen>(true).UpdateText(4, playerStanding[3].gold);
            }

            //Disable Canvas for player of the port we just left.
            playerInventories[playerIndexOfPort+1].myCanvas.DisableShipCanvases(false);
        }
    }
}
