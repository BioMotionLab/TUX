using System.Collections.Generic;
using bmlTUX.Scripts.VariableSystem;
using UnityEngine;

namespace bmlTUX.Scripts.UI.Runtime {
    public class ParticipantVariablePanel : MonoBehaviour
    {
        [SerializeField]
        ParticipantVariableEntry ParticipantVariableEntryPrefab = default;

        [SerializeField]
        GameObject ContentPanel = default;
        
        readonly List<ParticipantVariableEntry> participantVariableEntries = new List<ParticipantVariableEntry>();
        public List<ParticipantVariableEntry> Entries => participantVariableEntries;

        public void ShowParticipantVariables(List<ParticipantVariable> participantVariables) {

            foreach (ParticipantVariable participantVariable in participantVariables) {
                
                ParticipantVariableEntry newParticipantVariableEntry = 
                    Instantiate(ParticipantVariableEntryPrefab, ContentPanel.transform);
                participantVariableEntries.Add(newParticipantVariableEntry);
                newParticipantVariableEntry.Display(participantVariable);
            }
        }
        
        
        
    }
}
