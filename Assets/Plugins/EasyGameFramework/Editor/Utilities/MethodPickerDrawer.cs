using System.Collections.Generic;
using EasyFramework;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public class MethodPickerDrawer : MemberPickerDrawer<MethodPicker>
    {
        private InspectorProperty _parameters;

        private ReadOnlyVariantList Parameters
        {
            get => _parameters.ValueEntry.WeakSmartValue as ReadOnlyVariantList;
            set => _parameters.ValueEntry.WeakSmartValue = value;
        }

        protected override void Initialize()
        {
            _parameters = Property.Children["_parameters"];
            base.Initialize();

            if (Parameters == null)
            {
                Parameters = new ReadOnlyVariantList();
            }
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return true;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            base.DrawPropertyLayout(label);

            if (Parameters.IsNotNullOrEmpty())
            {
                _parameters.Draw();
            }
        }

        protected override bool MemberFilter(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Method)
                return false;
            var method = (MethodInfo)member;
            return method.GetParameters().All(p => Variant.IsAcceptedType(p.ParameterType));
        }

        protected override string GetMemberName(MemberInfo member)
        {
            if (member == null)
                return string.Empty;

            var m = (MethodInfo)member;
            return $"{member.Name} ({m.GetMethodParametersSignature()})";
        }

        protected override void OnTargetMemberChanged()
        {
            base.OnTargetMemberChanged();

            var newMethod = GetTargetMember() as MethodInfo;
            if (newMethod != null)
            {
                var ps = newMethod.GetParameters();
                if (ps.Length > Parameters.Count)
                {
                    var count = ps.Length - Parameters.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Parameters.Add(new Variant());
                    }
                }
                else if (ps.Length < Parameters.Count)
                {
                    Parameters.RemoveRange(ps.Length, Parameters.Count - ps.Length);
                }

                for (int i = 0; i < ps.Length; i++)
                {
                    var p = ps[i];
                    Parameters[i].Setup(p.ParameterType, p.Name);
                }
            }
            else
            {
                Parameters.Clear();
            }
        }
    }
}
