using System;
using System.Diagnostics;
using EasyFramework;
using UnityEngine;

namespace EasyGameFramework
{
    public enum EasyControlBindAccess
    {
        Public,
        PrivateWithSerializeFieldAttribute
    }

    [Serializable]
    public class EasyControlEditorArgs
    {
        [Serializable]
        public class ViewModelArgs
        {
            public string GenerateDir;
            public string Namespace;
            public bool ClassNameSameAsGameObjectName = true;
            public string ClassName;
            public SerializableType BaseClass;
            public bool IsInitialized;
            public string AssemblyQualifiedName;

            private Type _classType;

            public Type ClassType
            {
                get
                {
                    if (_classType != null)
                    {
                        if (_classType.Namespace == Namespace
                            && _classType.Name == ClassName
                            && _classType.AssemblyQualifiedName == AssemblyQualifiedName)
                        {
                            return _classType;
                        }

                        _classType = null;
                        AssemblyQualifiedName = string.Empty;
                    }

                    if (AssemblyQualifiedName.IsNullOrWhiteSpace())
                    {
                        var type = ReflectionUtility.FindType(Namespace, ClassName);
                        if (type == null)
                            return null;
                        AssemblyQualifiedName = type.AssemblyQualifiedName;
                        _classType = type;
                    }
                    else
                    {
                        _classType = Type.GetType(AssemblyQualifiedName);
                    }

                    return _classType;
                }
                set
                {
                    Namespace = value.Namespace;
                    ClassName = value.Name;
                    AssemblyQualifiedName = value.AssemblyQualifiedName;
                    BaseClass.Type = value.BaseType;
                }
            }
        }

        [Serializable]
        public class BounderArgs
        {
            public Transform Parent;
            public SerializableType TypeToBind;
            public bool NameSameAsGameObjectName = true;
            public string Name;
            public bool AutoAddCommentPara = true;
            public string Comment;
            public bool IsInitialized;
            public EasyControlBindAccess Access;
            public bool AutoNamingNotations = true;

            public string GetName()
            {
                if (Name.IsNullOrWhiteSpace())
                    return Name;

                if (!AutoNamingNotations)
                    return Name;

                //TODO 更多情况的处理
                if (Access == EasyControlBindAccess.Public)
                {
                    return char.ToUpper(Name[0]) + Name[1..];
                }

                var name = char.ToLower(Name[0]) + Name[1..];
                return '_' + name;
            }
        }

        public bool Expand = true;

        public bool DoViewModel;
        public bool DoBounder;

        public ViewModelArgs ViewModel = new ViewModelArgs();
        public BounderArgs Bounder = new BounderArgs();
    }
    
    [Conditional("UNITY_EDITOR")]
    public class EasyBounderControlAttribute : PropertyAttribute
    {
        public string OriginName;

        public EasyBounderControlAttribute(string originName)
        {
            OriginName = originName;
        }
    }

    public sealed class EasyControl : MonoBehaviour
    {
        [SerializeField]
        private EasyControlEditorArgs _easyControlEditorArgs = new EasyControlEditorArgs();
    }
}
