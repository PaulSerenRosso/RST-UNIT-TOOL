using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private RaycastHit _hit;
    [SerializeField] private LayerMask _maskSquad;
    [SerializeField] private LayerMask _maskTarget;

    [SerializeField] float _moveSpeed;

    [SerializeField] private float _borderMoveThickness;

    [SerializeField] private bool hasRelease = true; 

    public InputMaster Master;

    private TerrainData a;

    private bool _isSelect;
    private bool _inInputSelect;
    private Squad _squadSelected;

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

        if (Input.mousePosition.y >= Screen.height - _borderMoveThickness) moveHeight = 1;
        else if (Input.mousePosition.y <= _borderMoveThickness) moveHeight = -1;
        if (Input.mousePosition.x >= Screen.width - _borderMoveThickness) moveWidth = 1;
        else if (Input.mousePosition.x <= _borderMoveThickness) moveWidth = -1;
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.Log(ray.direction);
            Debug.Log(ray.origin);
            Debug.Log("test");
            
            if (_isSelect )
            {
                if (Physics.Raycast(ray, out _hit, MapManager.Instance.CameraRayLength, _maskTarget))
                {
                    if (_hit.collider.CompareTag("Unit"))
                    {
                        UnitScript unit = _hit.collider.GetComponent<UnitScript>();
                        if (PlayerManager.Instance.AllPlayers[_squadSelected.Player].EnemyPlayers
                            .Contains(unit.Squad.Player))
                        {
                            _squadSelected.SetUnitDestination(GetDestinationsPosition(_hit.point, _squadSelected.movmentTypeUnitsIndex), unit);
                        }
                    }
                    else
                    { _squadSelected.SetPointDestination(GetDestinationsPosition(_hit.point, _squadSelected.movmentTypeUnitsIndex));
                    }
                }
            }
            else
            {
                if (Physics.Raycast(ray, out _hit, MapManager.Instance.CameraRayLength, _maskSquad))
                {
                    Debug.Log(_hit.collider.gameObject.name);
                    UnitScript unit = _hit.collider.GetComponent<UnitScript>();
                    Debug.Log(unit);
                    _squadSelected = unit.Squad;
                
                    _isSelect = true;
                }
            }    hasRelease = false;
        }
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
        _squadSelected.TargetSquad = null;
        _squadSelected.Destinations.Clear();
    }

    List<DestinationSquad> GetDestinationsPosition(float3 aimPosition, List<int> indices)
    {
        List<DestinationSquad> destinations = new List<DestinationSquad>();
        for (int i = 0; i < indices.Count; i++)
        {
            if (MapManager.Instance.UpdateDestinationWithTerrainNav(aimPosition, indices[i], out NavMeshHit navMeshHit))
            {
                destinations.Add(new DestinationSquad(navMeshHit.position, indices[i]));
            }
        }

        return destinations;
    }
}