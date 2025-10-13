using System;
using UnityEngine;

namespace Apple.GameKit.Sample
{
    public class PanelBase : MonoBehaviour
    {
        public bool IsPrefabInstance { get; private set;}
        public bool ShouldDestroyWhenPopped { get; private set; }

        public virtual PanelBase Instantiate(GameObject parent)
        {
            var panelBase = Instantiate(this, parent.transform, worldPositionStays: false);
            panelBase.IsPrefabInstance = true;
            panelBase.ShouldDestroyWhenPopped = true;
            return panelBase;
        }

        public virtual void Destroy()
        {
            Destroy(this.gameObject);
        }

        // helper function to destroy all the children of a GameObject
        protected void DestroyChildren(GameObject parent)
        {
            foreach (Transform transform in parent.transform)
            {
                Destroy(transform.gameObject);
            }
            parent.transform.DetachChildren();
        }
    }

    public class PanelBase<T> : PanelBase where T : MonoBehaviour
    {
        public new virtual T Instantiate(GameObject parent)
        {
            return base.Instantiate(parent).gameObject.GetComponent<T>();
        }
    }
}
