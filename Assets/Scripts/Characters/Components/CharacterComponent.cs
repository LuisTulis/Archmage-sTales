using UnityEngine;

public abstract class CharacterComponent : MonoBehaviour
{

    public virtual void Despawn() 
    {
        Destroy(this.gameObject);
    }

}
