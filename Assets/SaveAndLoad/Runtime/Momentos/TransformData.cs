using System;
using UnityEngine;

namespace SaveAndLoad.Momentos {
    [Serializable]
    public class TransformData : ISaveableData<TransformData, Transform> {
        public float xPosition;
        public float yPosition;
        public float zPosition;
        public float xRotation;
        public float yRotation;
        public float zRotation;
        
        public void Save(Transform data) {
            Vector3 position = data.position;
            Vector3 rotation = data.eulerAngles;
            this.xPosition = position.x;
            this.yPosition = position.y;
            this.zPosition = position.z;
            this.xRotation = rotation.x;
            this.yRotation = rotation.y;
            this.zRotation = rotation.z;
        }
        
        public void Load(Transform data) {
            data.position = new Vector3(this.xPosition, this.yPosition, this.zPosition);
            data.eulerAngles = new Vector3(this.xRotation, this.yRotation, this.zRotation);
        }
    }
}
