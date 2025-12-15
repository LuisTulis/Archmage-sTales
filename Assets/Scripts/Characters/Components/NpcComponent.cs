using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class NpcComponent : CharacterComponent {

    protected NpcLocomotion locomotion;
    protected NpcModel model;
    [SerializeField] protected Transform talkPoint;
    protected CustomerObjective customerObjective;
    public int greatingsIndex = 0;

    [SerializeField] protected bool readyToTalk = false;

    [SerializeField] protected Dialogue[] greatingsDialogue;

    [SerializeField] protected bool hasTalked = false;

    private Animator animator;

    protected override void Awake() {
        base.Awake();
        locomotion = GetComponent<NpcLocomotion>();
        model = GetComponent<NpcModel>();
        this.animator = this.gameObject.GetComponentInChildren<Animator>();

        customerObjective = this.gameObject.GetComponentInChildren<CustomerObjective>();
    }

    private void Start() {
    }



    private void Update() {

        UpdateWalkingAnimation();
        if (hasTalked && GameManager.Instance.isPlaying) {
            if (customerObjective != null) customerObjective.objective = "";
            readyToTalk = false;
            Despawn();
            return;
        }

        if (talkPoint != null) {
            float arriveDistance = 3f;
            if (Vector3.Distance(transform.position, talkPoint.transform.position) < arriveDistance) {
                if (customerObjective != null) customerObjective.objective = "exclamacion";
                if (!readyToTalk) {
                    readyToTalk = true;
                    OnArrived();
                }
            }
        }
    }
    private void UpdateWalkingAnimation()
    {
        if (animator == null) return;

        bool isWalking = locomotion.agent.velocity.magnitude > 0.1f;
        animator.SetBool("walking", isWalking);
    }



    public abstract void MoveToTalkPoint();

    protected virtual void OnArrived() { }

    public override void OnPointerClick(PointerEventData eventData) {
        if (readyToTalk) {
            Talk();
        }
    }

    protected virtual void Talk() {
        if (greatingsDialogue != null && greatingsDialogue.Length > 0 && DialogueManager.Instance != null) {
            DialogueManager.Instance.showDialoge(greatingsDialogue[0]);

            // Esto solo tiene que ser true cuando queramos que el npc se vaya.
            hasTalked = true;
        }
    }

    public override void Despawn() {
        if (locomotion != null && GlobalLocomotionManager.Instance != null) {
            StartCoroutine(MoveToDespawnAndDestroy());
        } else {
            if (GlobalNpcManager.Instance != null) GlobalNpcManager.Instance.NpcLeft(this);
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator MoveToDespawnAndDestroy() {
        Vector3 despawnPos = GlobalLocomotionManager.Instance.despawnPoint.position;

        if (locomotion != null) locomotion.MoveTo(despawnPos);

        while (Vector3.Distance(transform.position, despawnPos) > 0.1f) {
            yield return null;
        }

        if (GlobalNpcManager.Instance != null) GlobalNpcManager.Instance.NpcLeft(this);

        base.Despawn();
    }

    public override Dictionary<string, object> GetStats() {
        var stats = new Dictionary<string, object>
        {
            { "Name", model.CharacterName },
            { "Speed", model.Speed.ToString("F1") },
            { "Icon", model.Icon }
        };
        return stats;
    }

    public void SetTarget(Transform targetPoint) {
        talkPoint = targetPoint;
    }

}

