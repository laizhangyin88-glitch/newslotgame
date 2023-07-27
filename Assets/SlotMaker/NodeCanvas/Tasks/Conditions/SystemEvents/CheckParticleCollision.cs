using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

    [Category("System Events")]
    [Name("Check Particle Collision")]
    [EventReceiver("OnParticleCollision")]
    public class CheckParticleCollision : ConditionTask<Transform> {

        public bool specifiedTagOnly;
        [TagField]
        public string objectTag = "Untagged";
        
        [BlackboardOnly]
        public BBParameter<GameObject> saveGameObjectAs;
        [BlackboardOnly]
        public BBParameter<Vector3> saveContactDirection;

        protected override string info{
            get {return "ParticleCollision" + ( specifiedTagOnly? (" '" + objectTag + "' tag") : "" );}
        }

        protected override bool OnCheck(){
            return false;
        }

        public void OnParticleCollision(GameObject other){
            
            if (!specifiedTagOnly || other.gameObject.tag == objectTag){
                saveGameObjectAs.value = other.gameObject;
                Vector3 direction = agent.transform.position - other.transform.position;
                direction.Normalize();
                saveContactDirection.value = direction;
                YieldReturn(true);
            }
        }
    }
}
