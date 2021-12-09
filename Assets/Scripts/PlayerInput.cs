using TMPro;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private Camera Camera;
    [SerializeField]
    private LayerMask InteractableLayers;
    [SerializeField]
    private float UseDistance = 3f;
    [SerializeField]
    private TextMeshPro ExplodeText;

    private void Update()
    {
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out RaycastHit hit, UseDistance, InteractableLayers))
        {
            ExplodeText.gameObject.SetActive(true);
            ExplodeText.transform.position = hit.point - (hit.point - Camera.transform.position).normalized * 0.01f;
            ExplodeText.transform.rotation = Quaternion.LookRotation((hit.point - Camera.transform.position).normalized);
        }
        else
        {
            ExplodeText.gameObject.SetActive(false);
        }
    }

    public void OnUse()
    {
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out RaycastHit hit, UseDistance, InteractableLayers)
            && hit.collider.TryGetComponent<Destructible>(out Destructible destructible))
        {
            destructible.Explode();
        }
    }
}
