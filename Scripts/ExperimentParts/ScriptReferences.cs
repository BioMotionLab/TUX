using System;
using bmlTUX.SimpleExperimentParts;
using UnityEngine;

namespace bmlTUX {
    [Serializable]
    public class ScriptReferences {
        
        [SerializeField]
        [Tooltip("Script file that inherits from Trial class")]
        public TextAsset DragTrialScriptHere = default;
        
        [SerializeField]
        [Tooltip("Script file that inherits from Block class")]
        public TextAsset DragBlockScriptHere = default;
        
        [SerializeField]
        [Tooltip("Script file that inherits from Experiment class")]
        public TextAsset DragExperimentScriptHere = default;
        
        public Type TrialType      => GetScriptTypeFromInspector<Trial>(DragTrialScriptHere, true);
        public Type BlockType      => GetScriptTypeFromInspector<Block>(DragBlockScriptHere, true);
        public Type ExperimentType => GetScriptTypeFromInspector<Experiment>(DragExperimentScriptHere, true);

        Type GetScriptTypeFromInspector<T>(TextAsset textAsset, bool optional = false) where T : ExperimentPart {

            string typeName = typeof(T).LastPartOfTypeName();

            if (textAsset == null) {
                if (optional) return GetDefaultExperimentPart<T>();
                throw new NullReferenceException($"{typeName} Script null. Create custom {typeName} script and drag into inspector");
            }
            
            Type returnType = GetType(textAsset.name);
            
            if (!returnType.IsSubclassOf(typeof(T)))
                throw new NullReferenceException($"{typeName} Script that was dragged in is not subclass of {typeName} Class");
            
            Debug.Log(TuxLog.Good($"Successfully linked with {returnType.LastPartOfTypeName()} script"), textAsset);
            return returnType;
        }

        Type GetDefaultExperimentPart<T>() where T : ExperimentPart {
            Type type;
            if (typeof(T).IsEquivalentTo(typeof(Trial))) type = typeof(SimpleTrial);
            else if (typeof(T).IsEquivalentTo(typeof(Block))) type =  typeof(SimpleBlock);
            else if (typeof(T).IsEquivalentTo(typeof(Experiment))) type =  typeof(SimpleExperiment);
            else throw new ArgumentOutOfRangeException($"Type {typeof(T).FullName} not recognized");
            Debug.LogWarning($"{TuxLog.Prefix} No custom script referenced for {typeof(T).FullName}, reverting to default {type.FullName}. More info:\n" +
                             $"You likely broke a reference in your {nameof(ExperimentRunner)} GameObject. Check the Script References section in its inspector.\n" +
                             $"Make sure you drag your custom scripts into the correct fields");
            return type;
        }


       
Type GetType(string typeName) {
    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        foreach (Type type in assembly.GetTypes()) {
            if (type.LastPartOfTypeName() == typeName) {
                return type;
            }
        }
    }
    return null;
}

    }
}
