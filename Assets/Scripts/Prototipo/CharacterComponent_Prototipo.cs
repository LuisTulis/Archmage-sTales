using UnityEngine;

public abstract class CharacterComponent_Prototipo : MonoBehaviour
{

    public virtual void Despawn() {
        Destroy(this.gameObject);
    }

}
