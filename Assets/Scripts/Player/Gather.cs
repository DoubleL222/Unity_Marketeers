using Systems;
using System.Collections.Generic;
using Entities;
using Market.Generics;
using UnityEngine;
using System;

namespace Player
{
    public class Gather : MonoBehaviour
    {
        PlayerInput _playerInput;
        const float GatherRayHeight = 1f;
        const float GatherDistance = 4f;
        
        private PlayerInventory _inventory;
        private Transform _transform;
        private Resource _currentResourceGatheredFrom;
        private List<Resource> _resourcesCollidingWith;
        private bool _isCollidingWithResource;
        private PlayerSfx _playerSfx;

        private int
            _woodGatherProficiency = 0,
            _ironGatherProficiency = 0,
            _spiceGatherProficiency = 0,
            _gemGatherProficiency = 0;

        private void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            _inventory = GetComponent<PlayerInventory>();
            _resourcesCollidingWith = new List<Resource>();
            _transform = transform;
            _playerSfx = GetComponent<PlayerSfx>();

            if (_playerInput)
            {
                _playerInput.OnButton1Held += TryGetYield;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger && other.gameObject.layer == LayerMask.NameToLayer("Resource"))
            {
                // Player is colliding with a resource collider. Make it the resource he is gathering from.
                _isCollidingWithResource = true;
                _currentResourceGatheredFrom = other.GetComponent<Resource>();

                _resourcesCollidingWith.Add(_currentResourceGatheredFrom);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger && other.gameObject.layer == LayerMask.NameToLayer("Resource"))
            {
                RemoveCollidingResource(other.GetComponent<Resource>());
            }
        }

        private void RemoveCollidingResource(Resource resource)
        {
            _resourcesCollidingWith.Remove(resource);

            if (resource.gameObject == _currentResourceGatheredFrom.gameObject)
            {
                // Player has exited the resource collider he was currently gathering from.
                if (_resourcesCollidingWith.Count > 0)
                {
                    // He is still colliding with another resource.
                    // Change to the last one in the list.
                    _currentResourceGatheredFrom = _resourcesCollidingWith[_resourcesCollidingWith.Count - 1];
                }
                else
                    _isCollidingWithResource = false;
            }
        }

        void TryGetYield()
        {
            // If the player isn't colliding with a resource, try to cast a ray to check whether there is a resource in front of the player.
            // If it hits something, use that as the _currentResourceGatheredFrom.
            if (!_isCollidingWithResource)
                _currentResourceGatheredFrom = GetResourcePointedAt();

            if (_currentResourceGatheredFrom != null)
            {
                // TODO Terrible hack to make grinding gems sound better.
                // We manually loop the gem-gathering sound, while you're holding down the gather-button.
                // This is because it takes a long time to gather gems, and the gem-gathering sound was designed to play in quick succession.
                if (_currentResourceGatheredFrom.Type == ResourceType.Gem && !_playerSfx.GatherAudioSource.isPlaying)
                    _playerSfx.PlayGemGather();

                var yield = _currentResourceGatheredFrom.GetYield();
                if (yield != null && _inventory != null)
                {
                    switch (yield.Type)
                    {
                        case ResourceType.Wood:
                            _playerSfx.PlayWoodGather();
                            _inventory.ChangeResourceAmount(yield.Type, Mathf.RoundToInt( yield.Amount * (1 + (_woodGatherProficiency * Globals.WoodGatherIncreasePerProficiencyPoint))));
                            _woodGatherProficiency++;
                            break;
                        case ResourceType.Iron:
                            _playerSfx.PlayIronGather();
                            _inventory.ChangeResourceAmount(yield.Type, Mathf.RoundToInt(yield.Amount * (1 + (_ironGatherProficiency * Globals.IronGatherIncreasePerProficiencyPoint))));
                            _ironGatherProficiency++;
                            break;
                        case ResourceType.Spice:
                            _playerSfx.PlaySpiceGather();
                            _inventory.ChangeResourceAmount(yield.Type, Mathf.RoundToInt(yield.Amount * (1 + (_spiceGatherProficiency * Globals.SpiceGatherIncreasePerProficiencyPoint))));
                            _spiceGatherProficiency++;
                            break;
                        case ResourceType.Gem:
                            //_playerSfx.PlayGemGather();
                            _inventory.ChangeResourceAmount(yield.Type, Mathf.RoundToInt(yield.Amount * (1 + (_gemGatherProficiency * Globals.GemGatherIncreasePerProficiencyPoint))));
                            _gemGatherProficiency++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (!_currentResourceGatheredFrom.IsActive)
                    RemoveCollidingResource(_currentResourceGatheredFrom);
            }
        }

        private Resource GetResourcePointedAt()
        {
            RaycastHit hit;
            Debug.DrawRay(_transform.position + new Vector3(0f, GatherRayHeight, 0f), _transform.forward * GatherDistance, Color.red);
            if (Physics.Raycast(_transform.position + new Vector3(0f, GatherRayHeight, 0f), _transform.forward * GatherDistance, out hit, GatherDistance, ~LayerMask.NameToLayer("Resource"), QueryTriggerInteraction.Collide)
                && hit.collider != null && hit.collider.isTrigger)
            {
                // The ray hit one of our resource trigger colliders...
                return hit.collider.GetComponent<Resource>();
            }

            return null;
        }
    }
}