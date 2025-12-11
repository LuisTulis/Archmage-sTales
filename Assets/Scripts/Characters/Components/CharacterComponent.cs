using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CharacterComponent : MonoBehaviour, ISelectable, IPointerClickHandler, IStatProvider
{

    protected bool isSelected = false;
    private Renderer rend;

    protected virtual void Awake() {
        rend = GetComponent<Renderer>();
    }

    public virtual void Despawn() {
        Destroy(this.gameObject);
    }


    public virtual void OnPointerClick(PointerEventData eventData) {
        Debug.Log($"Click sobre {gameObject.name}");
        GlobalCharactersManager.Instance.SelectCharacter(this);
    }

    public virtual void OnSelect() {
        isSelected = true;

        if (rend != null)
            rend.material.color = Color.yellow;
    }

    public virtual void OnDeselect() {
        isSelected = false;

        if (rend != null)
            rend.material.color = Color.white;
    }

    public abstract Dictionary<string, object> GetStats();
}
