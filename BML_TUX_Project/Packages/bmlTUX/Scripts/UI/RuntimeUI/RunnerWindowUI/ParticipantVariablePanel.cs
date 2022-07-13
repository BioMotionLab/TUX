using System.Collections.Generic;
using bmlTUX.Scripts.UI.RuntimeUI.SessionSetupWindowUI;
using bmlTUX.Scripts.VariableSystem;
using UnityEngine;

namespace bmlTUX.UI.RuntimeUI {
    public class ParticipantVariablePanel : MonoBehaviour
    {
        [SerializeField]
        ParticipantVariableEntry ParticipantVariableEntryPrototype = default;

        [SerializeField]
        GameObject ContentPanel = default;
        
        readonly List<ParticipantVariableEntry> participantVariableEntries = new List<ParticipantVariableEntry>();
        public List<ParticipantVariableEntry> Entries => participantVariableEntries;

        public void ShowParticipantVariables(List<ParticipantVariable> participantVariables) {

            foreach (ParticipantVariable participantVariable in participantVariables) {
                
                ParticipantVariableEntry newParticipantVariableEntry = 
                    Instantiate(ParticipantVariableEntryPrototype, ContentPanel.transform);
                newParticipantVariableEntry.gameObject.SetActive(true);
                participantVariableEntries.Add(newParticipantVariableEntry);
                newParticipantVariableEntry.Display(participantVariable);
            }
        }

    }
}
