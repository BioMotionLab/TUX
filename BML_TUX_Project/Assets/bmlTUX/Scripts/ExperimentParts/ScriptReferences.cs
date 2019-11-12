using System;
using BML_Utilities.Extensions;
using bmlTUX.Scripts.ExperimentParts.SimpleExperimentParts;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Scripts.ExperimentParts {
    [Serializable]
    public class ScriptReferences {
        
        [SerializeField]
        [Tooltip("Script file that inherits from Trial class")]
        public MonoScript DragTrialScriptHere = default;
        
        [SerializeField]
        [Tooltip("Script file that inherits from Block class")]
        public MonoScript DragBlockScriptHere = default;
        
        [SerializeField]
        [Tooltip("Script file that inherits from Experiment class")]
        public MonoScript DragExperimentScriptHere = default;
        
        public Type TrialType      => GetScriptTypeFromInspector<Trial>(DragTrialScriptHere, true);
        public Type BlockType      => GetScriptTypeFromInspector<Block>(DragBlockScriptHere, true);
        public Type ExperimentType => GetScriptTypeFromInspector<Experiment>(DragExperimentScriptHere, true);

        Type GetScriptTypeFromInspector<T>(MonoScript monoScript, bool optional = false) where T : ExperimentPart {

            string typeName = typeof(T).LastPartOfName();

            if (monoScript == null) {
                if (optional) return GetDefaultExperimentPart<T>();
                throw new NullReferenceException($"{typeName} Script null. Create custom {typeName} script and drag into inspector");
            }
            
            Type returnType = monoScript.GetClass();
            if (!returnType.IsSubclassOf(typeof(T)))
                throw new NullReferenceException($"{typeName} Script that was dragged in is not subclass of {typeName} Class");
            
            Debug.Log($"Successfully linked with {returnType.LastPartOfName()} script");
            return returnType;
        }

        Type GetDefaultExperimentPart<T>() where T : ExperimentPart {
            Type type;
            if (typeof(T).IsEquivalentTo(typeof(Trial))) type = typeof(SimpleTrial);
            else if (typeof(T).IsEquivalentTo(typeof(Block))) type =  typeof(SimpleBlock);
            else if (typeof(T).IsEquivalentTo(typeof(Experiment))) type =  typeof(SimpleExperiment);
            else throw new ArgumentOutOfRangeException($"Type {typeof(T).FullName} not recognized");
            Debug.LogWarning($"No Custom class defined for {typeof(T).FullName}, reverting to default {type.FullName}");
            return type;
        }

        

    }
}