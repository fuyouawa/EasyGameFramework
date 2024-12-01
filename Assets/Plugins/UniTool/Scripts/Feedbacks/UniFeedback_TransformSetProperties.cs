using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [UniFeedbackHelper("修改Transform的各种属性")]
    [AddUniFeedbackMenu("变换/设置属性")]
    public class UniFeedback_TransformSetProperties : AbstractUniFeedback
    {
        [FoldoutGroup("Binding")]
        [Required]
        public Transform Target;

        [FoldoutGroup("Set Parent")]
        public bool ModifyParent = true;
        [FoldoutGroup("Set Parent")]
        [ShowIf(nameof(ModifyParent))]
        public Transform ParentToSet;
        [FoldoutGroup("Set Parent")]
        [ShowIf(nameof(ModifyParent))]
        public bool WorldPositionStay = true;

        [FoldoutGroup("Set Transform")]
        public bool ModifyTransform = true;
        [FoldoutGroup("Set Transform")]
        [ShowIf(nameof(ModifyTransform))]
        public bool Local;
        [FoldoutGroup("Set Transform")]
        [ShowIf(nameof(ModifyTransform))]
        public Vector3 PositionToSet;
        [FoldoutGroup("Set Transform")]
        [ShowIf(nameof(ModifyTransform))]
        public Vector3 RotationToSet;
        [FoldoutGroup("Set Transform")]
        [ShowIf(nameof(ModifyTransform))]
        public Vector3 LocalScaleToSet;

        protected override void OnFeedbackPlay()
        {
            if (ModifyParent)
            {
                Target.SetParent(ParentToSet, WorldPositionStay);
            }

            if (ModifyTransform)
            {
                if (Local)
                {
                    Target.SetLocalPositionAndRotation(PositionToSet, Quaternion.Euler(RotationToSet));
                }
                else
                {
                    Target.SetPositionAndRotation(PositionToSet, Quaternion.Euler(RotationToSet));
                }

                Target.localScale = LocalScaleToSet;
            }
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}