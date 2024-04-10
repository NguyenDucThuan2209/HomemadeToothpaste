using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
public class ComposeCanvasElementVisibilityController : MonoBehaviour, IUIVisibilityController
{
    public SubscriptionEvent GetOnEndHideEvent()
    {
        return visibilityControllers[0].GetComponent<IUIVisibilityController>().GetOnEndHideEvent();
    }

    public SubscriptionEvent GetOnEndShowEvent()
    {
        return visibilityControllers[0].GetComponent<IUIVisibilityController>().GetOnEndShowEvent();
    }

    public SubscriptionEvent GetOnStartHideEvent()
    {
        return visibilityControllers[0].GetComponent<IUIVisibilityController>().GetOnStartHideEvent();
    }

    public SubscriptionEvent GetOnStartShowEvent()
    {
        return visibilityControllers[0].GetComponent<IUIVisibilityController>().GetOnStartShowEvent();
    }

    [Header("Require IUIVisibilityController component")]
    [SerializeField]
    private List<GameObject> visibilityControllers = new List<GameObject>();

    public void Hide()
    {
        visibilityControllers.ForEach(controller => controller.GetComponent<IUIVisibilityController>()?.Hide());
    }

    public void HideImmediately()
    {
        visibilityControllers.ForEach(controller => controller.GetComponent<IUIVisibilityController>()?.HideImmediately());
    }

    public void Show()
    {
        visibilityControllers.ForEach(controller => controller.GetComponent<IUIVisibilityController>()?.Show());
    }

    public void ShowImmediately()
    {
        visibilityControllers.ForEach(controller => controller.GetComponent<IUIVisibilityController>()?.ShowImmediately());
    }

    private void OnValidate()
    {
        List<GameObject> rejectList = new List<GameObject>();
        foreach (var item in visibilityControllers)
        {
            var controller = item.GetComponent<IUIVisibilityController>();
            if (controller == null)
                rejectList.Add(item);
        }
        foreach (var item in rejectList)
        {
            visibilityControllers.Remove(item);
        }
    }
}
}