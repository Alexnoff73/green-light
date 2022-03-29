#if (UNITY_EDITOR) 
using UnityEditor;
using UnityEngine;

namespace WaypointEditor
{
    /// <summary>
    /// Class of common utilitiles used by WaypointEditor, that can be accessed globally and without needing an instance.
    /// </summary>
    public static class Utilities
    {
         public static T SafeDestroy<T>(T obj) where T : Object
        {
            if (Application.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);

            return null;
        }
        public static T SafeDestroyWithUndo<T>(T obj) where T : Object
        {
            if (Application.isPlaying)
                Object.Destroy(obj);
            else
                Undo.DestroyObjectImmediate(obj);

            return null;
        }

        static T SafeDestroyGameObject<T>(T component) where T : Component
        {
            if (component != null)
                SafeDestroyWithUndo(component.gameObject);
            return null;
        }
        public static Color GetRandomBrightColor()
        {
            var color = new Color(
                         Random.Range(.6f, 1f),
                         Random.Range(.6f, 1f),
                         Random.Range(.6f, 1f));
            return color;
        }
    }
}
#endif