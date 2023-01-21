using System;

namespace narkdagas.tbcs.actions {
    public interface IInteractable {
        public void Interact(Action onInteractionComplete);
    }
}