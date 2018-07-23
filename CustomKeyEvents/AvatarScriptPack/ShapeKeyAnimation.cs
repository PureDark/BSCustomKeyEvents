using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AvatarScriptPack
{
    public class ShapeKeyAnimation : CustomKeyEvent
    {

        [Tooltip("Triggers by what button event type.")]
        public ButtonEventType triggerEvent = ButtonEventType.Click;

        [Tooltip("Duration of the animation, in seconds.")]
        public float duration = 1f;

        [Tooltip("Indexes of the shape key.")]
        public List<int> shapeKeyIndex = new List<int>();

        [Tooltip("Values for the shape keys. Must match the indexes.")]
        public List<int> shapeKeyValue = new List<int>();

        private SkinnedMeshRenderer skinnedMeshRenderer;

        private Coroutine currentCoroutine;

        private List<int> originValue = new List<int>();

        void Start()
        {
            Console.WriteLine("ShapeKeyAnimation loaded!");
            skinnedMeshRenderer = FindObjectOfType<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                if (shapeKeyIndex.Count() <= skinnedMeshRenderer.sharedMesh.blendShapeCount)
                {
                    foreach (int index in shapeKeyIndex)
                    {
                        originValue.Add((int)skinnedMeshRenderer.GetBlendShapeWeight(index));
                        Debug.Log(index + "'s value :" + (int)skinnedMeshRenderer.GetBlendShapeWeight(index) + " target : " + shapeKeyValue[originValue.Count() - 1]);
                    }
                }
                int diffCount = shapeKeyIndex.Count() - shapeKeyValue.Count();
                for (int i = 0; i < diffCount; i++)
                {
                    shapeKeyValue.Add(0);
                }
            }
            if(triggerEvent == ButtonEventType.Click)
            {
                clickEvents.AddListener(new UnityAction(StartAnimation));
            }
            else if (triggerEvent == ButtonEventType.DoubleClick)
            {
                doubleClickEvents.AddListener(new UnityAction(StartAnimation));
            }
            else if (triggerEvent == ButtonEventType.LongClick)
            {
                longClickEvents.AddListener(new UnityAction(StartAnimation));
            }
            else if (triggerEvent == ButtonEventType.Press)
            {
                pressEvents.AddListener(new UnityAction(StartAnimation));
            }
            else if (triggerEvent == ButtonEventType.Hold)
            {
                holdEvents.AddListener(new UnityAction(StartAnimation));
            }
            else if (triggerEvent == ButtonEventType.Release)
            {
                releaseEvents.AddListener(new UnityAction(StartAnimation));
            }
            else if (triggerEvent == ButtonEventType.Release)
            {
                releaseAfterLongClickEvents.AddListener(new UnityAction(StartAnimation));
            }
        }

        protected void StartAnimation()
        {
            if(skinnedMeshRenderer != null)
            {
                if (currentCoroutine != null)
                {
                    this.StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(CoroutineShapeKeyAnimation());
            }
        }

        private IEnumerator<WaitForEndOfFrame> CoroutineShapeKeyAnimation()
        {
            if (skinnedMeshRenderer == null || shapeKeyIndex.Count() > skinnedMeshRenderer.sharedMesh.blendShapeCount || shapeKeyIndex.Count() > originValue.Count())
            {
                yield break;
            }
            float startTime = Time.time;
            bool reverse = true;
            for (int i = 0; i < shapeKeyIndex.Count(); i++)
            {
                int currentValue = (int)skinnedMeshRenderer.GetBlendShapeWeight(shapeKeyIndex[i]);
                int originDist = Mathf.Abs(originValue[i] - currentValue);
                int targetDist = Mathf.Abs(shapeKeyValue[i] - currentValue);
                reverse &= (targetDist < originDist);
            }

            do
            {
                bool isDone = false;
                for (int i = 0; i < shapeKeyIndex.Count(); i++)
                {
                    int targetWeight = (reverse) ? originValue[i] : shapeKeyValue[i];
                    int originWeight = (reverse) ? shapeKeyValue[i] : originValue[i];
                    int deltaWeight = (int)((Time.time - startTime) / duration * (targetWeight - originWeight));
                    int result = originWeight + deltaWeight;
                    skinnedMeshRenderer.SetBlendShapeWeight(shapeKeyIndex[i], result);
                    isDone = (Time.time - startTime) >= duration;
                }

                if (isDone)
                    yield break;

                yield return new WaitForEndOfFrame();
            }
            while (true);
        }

    }
}

