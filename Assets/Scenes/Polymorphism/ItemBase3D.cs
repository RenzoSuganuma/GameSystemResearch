using UnityEngine;

[RequireComponent( typeof(Rigidbody), typeof(AudioSource))]
public abstract class ItemBase3D : MonoBehaviour
{
    [SerializeField, Header ("効果音"), Tooltip ("効果音")] AudioClip _audClip;
    GameObject _trigredObj;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null & other.gameObject.CompareTag("Player"))
        {
            AudioSource audSrc = GetComponent<AudioSource>();
            audSrc.PlayOneShot(_audClip);
            GotItem();
        }
    }
    public abstract void GotItem();
}