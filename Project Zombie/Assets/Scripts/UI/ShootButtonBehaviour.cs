using UnityEngine;
using UnityEngine.EventSystems;

public class ShootButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Shoot shoot;

    public void OnPointerDown(PointerEventData eventData) {
        shoot.FirePressedDown();
    }
    
    public void OnPointerUp(PointerEventData eventData) {
        shoot.FirePressedUp();
    }
}
