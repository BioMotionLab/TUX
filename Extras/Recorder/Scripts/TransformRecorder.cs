using System;
using System.IO;
using UnityEngine;

namespace bmlTUX.Recorder
{
    public class TransformRecorder : RecordableComponent<TransformSnapshotBase> {

        Transform cachedTransform;
        
        [SerializeField] bool onlyPosition = false;
        [SerializeField] bool shouldRecord = true;
        
    
        public bool ShouldRecord {
            get => shouldRecord;
            set => shouldRecord = value;
        }

        void Awake() {
            RecordingManager = FindObjectOfType<RecordingManager>();
        }

   

        protected void OnEnable() {
            this.cachedTransform = transform;
        }

        /// <inheritdoc />
        protected override string CsvFileNameSuffix => "TransformRecording";

        protected override string GetHeader() {
            return $"ID,Name,Frame,Time,PosX,PosY,PosZ," +
                   $"RotX,RotY,RotZ,RotW," +
                   $"ScaleX,ScaleY,ScaleZ";
        }
        
        protected override string DataToCsvLine(float time, int frame) {
            Transform cached = transform;
            Vector3 position = cached.position;
            Quaternion rotation = cached.rotation;
            Vector3 localScale = cached.localScale;
            if (onlyPosition)
            {
                return
                    $"{cached.gameObject.GetInstanceID()},{cached.gameObject.name},{frame},{time},{position.x},{position.y},{position.z},,,,,,,";
            
            }
            else {
                return $"{cached.gameObject.GetInstanceID()},{cached.gameObject.name},{frame},{time},{position.x},{position.y},{position.z}," +
                       $"{rotation.x},{rotation.y},{rotation.z},{rotation.w}," +
                       $"{localScale.x},{localScale.y},{localScale.z}";
            }
        }

        protected override TransformSnapshotBase BuildSnapShot(float time, int frame) {
            if (onlyPosition) {
                return new TransformSnapshotOnlyPosition(cachedTransform, time, frame);
            }
            else {
                return new TransformSnapshot(cachedTransform, time, frame);
            }
        
        
        }

        public override bool ShouldUpdate(int frame) => shouldRecord;
    
        
    }
}