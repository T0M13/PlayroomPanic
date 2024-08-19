using UnityEngine;
    public interface MoveBehaviour
    {
        public void Move(Rigidbody rb, Vector2 movement, bool isSprinting);
    }
