using System.Collections.Generic;
using System.Linq;
using bmlTUX.Scripts.UI.RuntimeUI.UIUtilities;
using bmlTUX.Scripts.VariableSystem;
using TMPro;
using UnityEngine;

namespace bmlTUX.Scripts.UI.RuntimeUI.SessionSetupWindowUI {
    public class ParticipantVariableEntry : MonoBehaviour {
    
        public TextMeshProUGUI Label;

        public TMP_InputField ValueInput;
        public TMP_Dropdown   Dropdown;


        public delegate void ConfirmValueStrategy();
        public ConfirmValueStrategy ConfirmValue;
        public ParticipantVariable Variable { get; private set; }

        public void Display(ParticipantVariable participantVariable) {
            Variable = participantVariable;
        
            Label.text = participantVariable.Name;

            if (participantVariable.ValuesAreConstrained) {
                ConfirmValue = ConstrainedGetValueStrategy;
            
                Dropdown.gameObject.SetActive(true);
                SetupConstrainedValues(participantVariable);
            }
            else {
                ValueInput.gameObject.SetActive(true);
                ConfirmValue = DefaultGetValueStrategy;
            }
        }

        void SetupConstrainedValues(ParticipantVariable participantVariable) {
            List<string> possibleValues = participantVariable.PossibleValuesStringArray.ToList();
            possibleValues.Insert(0, SelectText);
            Dropdown.options.Clear();
            foreach (string value in possibleValues) {
                Dropdown.options.Add(new TMP_Dropdown.OptionData() {text = value});
            }
        }

        const string SelectText = "Choose...";

        void ConstrainedGetValueStrategy() {

            string selectedText = Dropdown.options[Dropdown.value].text;
        
            if (selectedText == SelectText) {
                throw new NoValueSelectedException($"Need to select value for {Variable.Name}");
            
            }
            Variable.SelectValue(selectedText);
        }

        void DefaultGetValueStrategy() {
        
            if (string.IsNullOrEmpty(ValueInput.text)) throw new NoValueSelectedException();
        
            Variable.SelectValue(ValueInput.text);
        }
    
    
    
        /*switch (participantVariable.DataType) {
        case SupportedDataTypes.Int:
            ParticipantVariableInt intVariable = (ParticipantVariableInt) participantVariable;
            //intVariable.Value = EditorGUILayout.IntField(intVariable.Value);
            break;
        case SupportedDataTypes.Float:
            ParticipantVariableFloat floatVariable = (ParticipantVariableFloat) participantVariable;
            //floatVariable.Value = EditorGUILayout.FloatField(floatVariable.Value);
            break;
        case SupportedDataTypes.String:
            ParticipantVariableString stringVariable =
                (ParticipantVariableString) participantVariable;
            //stringVariable.Value = EditorGUILayout.TextField(stringVariable.Value);
            break;
        case SupportedDataTypes.Bool:
            ParticipantVariableBool boolVariable = (ParticipantVariableBool) participantVariable;
            //boolVariable.Value = EditorGUILayout.Toggle(boolVariable.Value);
            break;
                    
        case SupportedDataTypes.GameObject:
        case SupportedDataTypes.Vector3:
        case SupportedDataTypes.CustomDataType_NotYetImplemented:
        case SupportedDataTypes.ChooseType:
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }*/
        //participantVariable.SelectedValue = EditorGUILayout.Dropdown(participantVariable.SelectedValue, possibleValues);

    }
}
