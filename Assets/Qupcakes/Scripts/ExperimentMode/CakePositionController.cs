using UnityEngine;
using System.Collections;

///* Manages drag and drop */
namespace Qupcakery
{
    public class CakePositionController : MonoBehaviour
    {
        public enum CakeStatus
        {
            OnPanel, OnBelt, InDelivery, OnDrag
        }

        public CakeStatus status { get; private set; } = CakeStatus.OnPanel;

        //    // Drag cake with mouse
        //    private void OnMouseDrag()
        //    {
        //        switch (status)
        //        {
        //            case CakeStatus.InDelivery:
        //                return;
        //            case CakeStatus.OnBelt:
        //                CakeSlots.Instance.RemoveCakeFromSlot(gameObject,
        //                    transform.position);
        //                status = CakeStatus.OnDrag;
        //                break;
        //            case CakeStatus.OnPanel:
        //                GameObject panelCake = 
        //                    Object.Instantiate(gameObject, transform.position,
        //                    Quaternion.identity);
        //                ExperimentCakeBoxController exController=
        //                    panelCake.GetComponent<ExperimentCakeBoxController>();
        //                exController.SetCakeBoxState(new
        //                    Cake(GetComponent<ExperimentCakeBoxController>().cake.GetCakeType()));
        //                status = CakeStatus.OnDrag;
        //                break;
        //            default:
        //                break;
        //        }

        //        // Move gate position with mouse 
        //        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //        transform.Translate(mousePosition);
        //    }

        //    private void OnMouseUp()
        //    {
        //        if (status != CakeStatus.OnDrag)
        //            return;

        //        Vector2 position = transform.position;
        //        bool rc = CakeSlots.Instance.PlaceCakeInSlot(gameObject, position);
        //        if (rc)
        //        {
        //            status = CakeStatus.OnBelt;
        //        } else
        //        {
        //            Destroy(gameObject);
        //        }
        //    }
    }
}