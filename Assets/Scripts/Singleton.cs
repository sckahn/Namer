using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool shuttingDown = false;
    static object m_lock = new object();
    private static T Instance;

    public static T GetInstance
    {
        get
        {
            if (shuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '"+ typeof(T)+
                                 "' already destroyed. Retruning null.");
                return null;
            }

            lock (m_lock)
            {
                if (Instance == null)
                { 
                    // 비활성화 되어있는 오브젝트도 탐색
                    Instance = (T)GetAllObjectsOnlyInScene<T>();
                    Debug.Log(Instance);
                    if (Instance == null)
                    {
                        Instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                        DontDestroyOnLoad(Instance);
                    }
                }
                return Instance;
            }
            Component GetAllObjectsOnlyInScene<T1>() where T1 : Component
            {
                var components = Resources.FindObjectsOfTypeAll(typeof(T1));
                foreach (UnityEngine.Object co in components)
                {
                    Component component = co as Component;
                    GameObject go = component.gameObject;
                    if (go.scene.name == null) // 씬에 있는 오브젝트가 아니므로 제외한다.
                        continue;

                    // HideFlags 이용하여 씬에 있는 오브젝트가 아닌경우 제외
                    if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave || go.hideFlags == HideFlags.HideInHierarchy)
                        continue;

                    return component;
                }

                return null;
            }
        }
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
