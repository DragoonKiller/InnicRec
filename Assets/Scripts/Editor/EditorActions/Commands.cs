using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorExt
{

    public static class Commands
    {
        [MenuItem("Command/ClearCache", false, 102)]
        public static void ClearCache()
        {
            Caching.ClearCache();
            PlayerPrefs.DeleteAll();
        }
    }

}
