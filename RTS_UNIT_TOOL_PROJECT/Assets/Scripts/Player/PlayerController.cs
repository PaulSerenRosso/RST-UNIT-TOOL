using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   
   private RaycastHit _hit;
   [SerializeField]
   private LayerMask _maskSquad;
   [SerializeField]
   private LayerMask _maskTarget;
   
   public InputMaster Master;

   private TerrainData a;
   
   private bool _isSelect;
   private bool _inInputSelect;
   private Squad _squadSelected;
   void Start()
   {
      
       Master.Player.Select.performed += context => Select(context);
       Master.Player.Select.canceled += context => EndSelect(context);
       Master.Player.Deselect.performed += context => Deselect(context);
   }
   void Update()
   {
       if (_inInputSelect)
       {  
           Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         
           if (_isSelect)
           {
               if (Physics.Raycast(ray, out _hit, _maskTarget))
               {
                   if (_hit.collider.CompareTag("Unit"))
                   {
                       UnitScript unit = _hit.collider.GetComponent<UnitScript>();
                    
                   }
               }
           }
           else
           {
               if (Physics.Raycast(ray, out _hit, _maskSquad))
               {
                UnitScript unit = _hit.collider.GetComponent<UnitScript>();
                _squadSelected = unit.Squad;
                   _squadSelected.IsSelected = true;
               }
           }
       }
   }

   void Select(InputAction.CallbackContext context)
   {
       _inInputSelect = true;
   }

   void EndSelect(InputAction.CallbackContext context)
   {
        _inInputSelect = false;
   }

   void Deselect(InputAction.CallbackContext context)
   {
       _isSelect = false;
       _squadSelected.IsSelected = false;
   }
   
}
