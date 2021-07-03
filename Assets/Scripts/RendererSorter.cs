using System.Collections.Generic;
using UnityEngine;

    public class RendererSorter : MonoBehaviour
    {
        public string sortLayerName = "Default";
        public int order = 0;

        public bool enableRecord = false;
        private Dictionary<Renderer, int> m_dict = null;

        void Start()
        {
            Apply();
        }

        [ContextMenu("Apply")][XLua.LuaCallCSharp]
        public void Apply()
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>(true);

            if (enableRecord)
            {
                if (m_dict == null)
                {
                    m_dict = new Dictionary<Renderer, int>(renders.Length);
                    foreach (var item in renders)
                    {
                        m_dict.Add(item, item.sortingOrder);
                    }
                }
            }

            foreach (Renderer render in renders)
            {
                //render.
                render.sortingLayerName = sortLayerName;
                if (!enableRecord)
                    render.sortingOrder = order;
                else
                {
                    int diff = 0;
                    if (m_dict != null && m_dict.TryGetValue(render, out diff))
                    {
                        render.sortingOrder = order + diff;
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.LogError("RendererSorter.dict error.");
#endif
                        render.sortingOrder = order;
                    }
                }
            }
        }
    }
