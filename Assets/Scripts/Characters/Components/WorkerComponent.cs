public class WorkerComponent : BaseWorkerComponent {

    public override void Despawn() {
        GlobalCharactersManager.Instance.FireWorker(this.gameObject.GetComponent<WorkerModel>());
        base.Despawn();
    }

}

