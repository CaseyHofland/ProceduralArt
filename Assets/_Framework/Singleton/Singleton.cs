using UnityEngine;

#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
#endif

#if ODIN_INSPECTOR
using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;
#endif

/// <summary>
/// Generic Implementation of a Singleton MonoBehaviour.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Returns the instance of this singleton.
    /// </summary>
    public static T instance;

    [Header("Singleton")]
    [SerializeField]
    private bool dontDestroyOnLoad = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = transform.GetComponent<T>();
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        if (instance == null)
        {
            instance = transform.GetComponent<T>();
        }
        else
        {
            GameObject gameObject = this.gameObject;
            DestroyImmediate(this);

            MemberInfo memberInfo = instance.GetType(); //monoInstanceCaller.GetType();
            RequireComponent[] requiredComponentsAtts = Attribute.GetCustomAttributes(
                memberInfo, typeof(RequireComponent), true) as RequireComponent[];
            Array.Reverse(requiredComponentsAtts);
            Component[] components = gameObject.GetComponents<Component>();

            foreach (RequireComponent rc in requiredComponentsAtts)
            {
                if (components[components.Length - 1].GetType() == rc.m_Type0)
                {
                    Array.Resize(ref components, components.Length - 1);
                    Undo.DestroyObjectImmediate(gameObject.GetComponent(rc.m_Type0));
                    Undo.IncrementCurrentGroup();
                }
            }
        }
    }
#endif
}
