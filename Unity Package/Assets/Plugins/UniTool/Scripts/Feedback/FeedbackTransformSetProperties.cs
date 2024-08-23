using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [FeedbackHelper("修改Transform的各种属性")]
    [AddFeedbackMenu("Transform/Set Properties")]
    public class FeedbackTransformSetProperties : AbstractFeedback
    {
        [FoldoutGroup("Binding")]
        [Required]
        public Transform Target;

        [FoldoutGroup("Set Parent")]
        public bool ModifyParent = true;
        [FoldoutGroup("Set Parent")]
        [ShowIf("ModifyParent")]
        public Transform ParentToSet;
        [FoldoutGroup("Set Parent")]
        [ShowIf("ModifyParent")]
        public bool WorldPositionStay = true;

        [FoldoutGroup("Set Transform")]
        public bool ModifyTransform = true;
        [FoldoutGroup("Set Transform")]
        [ShowIf("ModifyTransform")]
        public bool Local;
        [FoldoutGroup("Set Transform")]
        [ShowIf("ModifyTransform")]
        public Vector3 PositionToSet;
        [FoldoutGroup("Set Transform")]
        [ShowIf("ModifyTransform")]
        public Vector3 RotationToSet;
        [FoldoutGroup("Set Transform")]
        [ShowIf("ModifyTransform")]
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
