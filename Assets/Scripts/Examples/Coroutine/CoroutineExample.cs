
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Kit.Utils.Output;

namespace Example.Coroutine{
    public class MyYieldInstruction{
        public virtual bool IsDone(){
            return true;
        }
    }

    public class MyWaitForSeconds : MyYieldInstruction{
        private float delayTime;
        private float startTime;
        public MyWaitForSeconds(float delayTime){
            this.delayTime = delayTime;
            this.startTime = -1;
        }

        public override bool IsDone(){
            if (startTime < 0)
                startTime = Time.time;
            return (Time.time - startTime) >= delayTime;
        }
    }

    public class MyCoroutine{
        public IEnumerator enumerator{
            get;
            private set;
        }
        public MyCoroutine(IEnumerator enumerator){
            this.enumerator = enumerator;
        }
    }

    public class CoroutineExample : MonoBehaviour
    {
        public static CoroutineExample Instance{
            get;
            private set;
        }

        private void Awake() {
            if (Instance == null){
                Instance = this;
            }else{
                Debug.LogError("Multi MyCoroutine.Coroutine Instance");
            }
        }

        private List<IEnumerator> enumeratorList = new List<IEnumerator>();
        // private Dictionary<int, bool> waitForNextFrameDic = new Dictionary<int, bool>();

        private List<Stack<IEnumerator>> enumeratorStackList = new List<Stack<IEnumerator>>();

        public MyCoroutine MyStartCoroutine(IEnumerator enumerator){
            Dump(enumerator.Current, "curetnn");

            enumerator.MoveNext(); // 没有unity的实现细腻的原因

            Stack<IEnumerator> stack = new Stack<IEnumerator>();
            stack.Push(enumerator);
            enumeratorStackList.Add(stack);

            if (enumerator.Current is MyCoroutine){
                stack.Push(((MyCoroutine)enumerator.Current).enumerator);
            }

            if (enumerator.Current is MyYieldInstruction){
                MyYieldInstruction instruction = (MyYieldInstruction)enumerator.Current;
                    if (!instruction.IsDone()){
                    }
            }

            if (enumerator.Current == null){  // todo 处理第一帧的要延迟的问题
                // enumerator.Reset();
            }
            
            // enumeratorList.Add(enumerator);
            // if (enumerator.Current == null){
            //         waitForNextFrameDic.Add(enumeratorList.Count - 1, true);
            // }

            // return new MyYieldInstruction();
            // return enumerator;
            return new MyCoroutine(enumerator);
        }

        private void LateUpdate() {
            for(int i = enumeratorStackList.Count - 1; i >= 0; i --){

                // 如果是yield return null 应该晚一帧调用
                // if (waitForNextFrameDic.ContainsKey(i) && waitForNextFrameDic[i]){
                //     waitForNextFrameDic[i] = false;
                //     continue;
                // }

                if (enumeratorStackList[i].Peek().Current is MyYieldInstruction){
                    MyYieldInstruction instruction = (MyYieldInstruction)enumeratorStackList[i].Peek().Current;
                    if (!instruction.IsDone()){
                        continue;
                    }
                }

                bool tag = false;
                while(!enumeratorStackList[i].Peek().MoveNext()){
                    enumeratorStackList[i].Pop();
                    if (enumeratorStackList[i].Count <= 0){
                        tag = true;
                        break;
                    }
                }
                if (tag){
                    enumeratorStackList.RemoveAt(i);
                    continue;
                }

                // if (!enumeratorStackList[i].Peek().MoveNext()){
                //     enumeratorStackList[i].Pop();
                //     if (enumeratorStackList[i].Count <= 0){
                //         enumeratorStackList.RemoveAt(i);
                //     }else{
                //         enumeratorStackList[i].Peek().MoveNext();
                //     }
                //     // enumeratorList.RemoveAt(i);
                //     continue;
                // }

                if (enumeratorStackList[i].Peek().Current is MyCoroutine){
                    enumeratorStackList[i].Push(((MyCoroutine)enumeratorStackList[i].Peek().Current).enumerator);
                    enumeratorStackList.RemoveAt(enumeratorStackList.Count - 1);
                    continue;
                }

                if (enumeratorStackList[i].Peek().Current is MyYieldInstruction){
                    MyYieldInstruction instruction = (MyYieldInstruction)enumeratorStackList[i].Peek().Current;
                    if (!instruction.IsDone()){
                        continue;
                    }
                }

            }
        }
    }
}


