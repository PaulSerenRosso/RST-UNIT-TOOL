using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
 
    
    [Header("Camera Move")]
    [SerializeField] float _moveSpeed;
    [SerializeField] private float _borderMoveThickness;

    
    [Header("Variables for Detect Unit")]
    [SerializeField] private LayerMask _maskSquad;
    [SerializeField] private LayerMask _maskTarget;
    [SerializeField] private float _distanceRay;
 private bool hasRelease = true;

    public InputMaster Master;

    RaycastHit[] _hits;
    private TerrainData a;
    private Ray ray;
    private bool _isSelect;
    private bool _inInputSelect;
    private Squad _squadSelected;   
    private RaycastHit _hit;

    void Start()
    {
        Master = new InputMaster();
        Master.Player.Select.performed += context => Select(context);
        Master.Player.Select.canceled += context => EndSelect(context);
        Master.Player.Deselect.performed += context => Deselect(context);
        Master.Enable();
    }

    void Update()
    {
        UpdateSelect();
        MoveCamera();
    }

    void MoveCamera()
    {
        float3 pos = Camera.main.transform.position;
        int moveHeight = 0;
        int moveWidth = 0;

      
        if (Input.mousePosition.y >= Screen.height - _borderMoveThickness &&
            Input.mousePosition.y <= Screen.height) moveHeight = 1;
        else if (Input.mousePosition.y <= _borderMoveThickness && Input.mousePosition.y >= 0) moveHeight = -1;
        if (Input.mousePosition.x >= Screen.width - _borderMoveThickness &&
            Input.mousePosition.x <= Screen.width) moveWidth = 1;
        else if (Input.mousePosition.x <= _borderMoveThickness && Input.mousePosition.x >= 0) moveWidth = -1;
        if (moveHeight == 0 && moveWidth == 0)
            return;

        float speed = _moveSpeed;
        if (moveHeight != 0 && moveWidth != 0)
            speed /= 2;

        if (moveHeight == 1)
            pos.z += speed * Time.deltaTime;
        else if (moveHeight == -1)
            pos.z -= speed * Time.deltaTime;
        if (moveWidth == 1)
            pos.x += speed * Time.deltaTime;
        else if (moveWidth == -1)
            pos.x -= speed * Time.deltaTime;

        Camera.main.transform.position = pos;
    }

    void UpdateSelect()
    {
        if (_inInputSelect && hasRelease)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (_isSelect)
            {
                _hits = Physics.RaycastAll(ray, _distanceRay, _maskTarget);
                if (_hits.Length != 0)
                {
                        int index = -1;
                                    bool _detectUnit = false;
                                    for (int i = 0; i < _hits.Length; i++)
                                    {
                                        if (_hits[i].collider.CompareTag("Unit"))
                                        {
                                            UnitScript unit = _hits[i].collider.GetComponent<UnitScript>();
                                            if (PlayerManager.Instance.AllPlayers[_squadSelected.Player].EnemyPlayers
                                                .Contains(unit.Squad.Player))
                                            {
                                                _squadSelected.SetUnitDestination(unit);
                                                _detectUnit = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (index == -1)
                                                index = i;
                                        }
                                    }
                                    if (!_detectUnit)
                                    {
                                        if (index != 1)
                                        { 
                                            _squadSelected.SetPointDestination(GetDestinations(_hits[index].point,
                                                                    _squadSelected.movmentTypeUnitsIndex));
                                        }
                                        
                                    }
                }
            
            }
            else
            {
                if (Physics.Raycast(ray, out _hit, _distanceRay, _maskSquad))
                {
                    //                  Debug.Log(_hit.collider.gameObject.name);
                    UnitScript unit = _hit.collider.GetComponent<UnitScript>();
//                    Debug.Log(unit);

                    _squadSelected = unit.Squad;

                    _isSelect = true;
                }
            }

            hasRelease = false;
        }

        Debug.DrawRay(ray.origin, ray.direction * _distanceRay);
    }

    void Select(InputAction.CallbackContext context)
    {
        _inInputSelect = true;
    }

    void EndSelect(InputAction.CallbackContext context)
    {
        hasRelease = true;
        _inInputSelect = false;
    }

    void Deselect(InputAction.CallbackContext context)
    {
        _isSelect = false;
        if (_squadSelected != null)
            _squadSelected.TargetSquad = null;
        _squadSelected = null;
    }

    List<DestinationPoint> GetDestinations(float3 aimPosition, List<int> indices)
    {
        List<DestinationPoint> destinations = new List<DestinationPoint>();
        for (int i = 0; i < indices.Count; i++)
        {
            if (MapManager.Instance.UpdateDestinationWithTerrainNav(aimPosition, indices[i], out NavMeshHit navMeshHit))
            {
                destinations.Add(new DestinationPoint(navMeshHit.position, indices[i]));
            }
        }

        return destinations;
    }
}