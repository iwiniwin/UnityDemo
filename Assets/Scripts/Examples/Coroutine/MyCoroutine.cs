
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public class MyCoroutine : MonoBehaviour
    {
        public static MyCoroutine Instance{
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
        private Dictionary<int, bool> waitForNextFrameDic = new Dictionary<int, bool>();

        public MyYieldInstruction MyStartCoroutine(IEnumerator enumerator){
            enumerator.MoveNext(); // 没有unity的实现细腻的原因
            
            enumeratorList.Add(enumerator);
            if (enumerator.Current == null){
                    waitForNextFrameDic.Add(enumeratorList.Count - 1, true);
                }

            return new MyYieldInstruction();
        }

        private void LateUpdate() {
          

            for(int i = enumeratorList.Count - 1; i >= 0; i --){

                if (waitForNextFrameDic.ContainsKey(i) && waitForNextFrameDic[i]){
                    waitForNextFrameDic[i] = false;
                    continue;
                }

                if (enumeratorList[i].Current is MyYieldInstruction){
                    MyYieldInstruction instruction = (MyYieldInstruction)enumeratorList[i].Current;
                    if (!instruction.IsDone()){
                        continue;
                    }
                }

                
                
                if (!enumeratorList[i].MoveNext()){
                    enumeratorList.RemoveAt(i);
                    continue;
                }

                if (enumeratorList[i].Current is MyYieldInstruction){
                    MyYieldInstruction instruction = (MyYieldInstruction)enumeratorList[i].Current;
                    if (!instruction.IsDone()){
                        continue;
                    }
                }

            }
        }
    }
}


