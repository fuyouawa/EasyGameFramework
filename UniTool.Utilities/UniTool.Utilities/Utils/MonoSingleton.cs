﻿using System;
using UnityEngine;

namespace UniTool.Utilities
{
    internal partial class SingletonCreator
    {
        public static T CreateMonoSingleton<T>() where T : Component, ISingleton
        {
            if (!Application.isPlaying)
                return null;

            var instances = UnityEngine.Object.FindObjectsOfType<T>();
            if (instances.Length > 1)
            {
                throw new Exception($"Mono单例({typeof(T).Name})在场景中存在的实例只能有1个!");
            }
            if (instances.Length == 1)
            {
                instances[0].OnSingletonInit();
                return instances[0];
            }

            var obj = new GameObject(typeof(T).Name);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            var inst = obj.AddComponent<T>();

            inst.OnSingletonInit();
            return inst;
        }
    }

    public class MonoSingleton<T> : MonoBehaviour, ISingleton
        where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = SingletonCreator.CreateMonoSingleton<T>();
                }
                return _instance;
            }
        }

        public virtual void OnSingletonInit()
        {
        }

        protected virtual void OnApplicationQuit()
        {
            if (_instance == null) return;
            Destroy(_instance.gameObject);
            _instance = null;
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}