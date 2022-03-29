using UnityEngine;

namespace WaypointEditor
{
    /// <summary>
    /// Stores graphical assets used by the WaypointEditor classes.
    /// </summary>
    public static class WaypointEditorAssets
    {
        static Mesh _pinMesh;
        public static Mesh PinMesh
        {
            get
            {
                if (_pinMesh == null)
                    _pinMesh = (Mesh)Resources.Load("Pin", typeof(Mesh));
                return _pinMesh;
            }
        }
        
        static Texture2D _deleteIcon;
        public static Texture2D DeleteIcon
        {
            get
            {
                if (_deleteIcon == null)
                    _deleteIcon = (Texture2D)Resources.Load("TrashIcon", typeof(Texture2D));
                return _deleteIcon;
            }
        }

    }


}