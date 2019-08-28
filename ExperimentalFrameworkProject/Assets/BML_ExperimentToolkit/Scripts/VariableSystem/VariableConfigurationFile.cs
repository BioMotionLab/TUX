using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Settings;
using BML_Utilities.Extensions;
using Malee;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [CreateAssetMenu]
    public class VariableConfigurationFile : ScriptableObject {

        
        [Header("Randomization and Repetition settings:")]
        public RandomizationMode RandomizationMode;
        
        [Range(1,50)]
        public int  RepeatTrialsInBlock = 1;
        
        [Range(1,20)]
        public int RepeatAllBlocks = 1;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public ColumnNamesSettings ColumnNamesSettings;
        public ControlSettings ControlSettings;
        public GuiSettings GuiSettings;
        
        public void Validate() {
            
            if (ColumnNamesSettings == null) {
                throw new NullReferenceException(
                                                 "Configuration file does not have column name settings defined. " + 
                                                 "Please drag column name settings into the proper place in the config file");
            }

            if (ControlSettings == null) {
                throw new NullReferenceException(
                                                 "Configuration file does not have Control Settings defined. " +
                                                 "Please drag control settings into the proper place in the config file");
            }
        }

        public Variables Variables => Factory.Variables;
        
        
        [SerializeField]
        public TrialTableGenerationMode GenerateExperimentTable = TrialTableGenerationMode.OnTheFly;
        
        [SerializeField]
        public List<RowHolder> OrderConfigurations = new List<RowHolder>();


    }


[CustomPropertyDrawer(typeof(RowHolder))]
public class RowHolderDrawer : PropertyDrawer {
    
    protected float CustomPropertyHeight;

    public override float GetPropertyHeight(SerializedProperty mainProperty, GUIContent label) {
        float totalPropertyHeight = CustomPropertyHeight;
        return totalPropertyHeight;
    }
}

[Serializable]
public class RowHolder {

    public int[] Order {
        get {
            switch (Randomize) {
                case true:
                    List<int> orderList = new List<int>();
                    for (int i = 0; i < experimentDesign.BaseBlockTable.Rows.Count; i++) {
                        orderList.Add(i);
                    }
                    orderList.Shuffle();
                    return orderList.ToArray();
                
                case false:
                    int[] order = new int[List.Count];
                    for (int i = 0; i < List.Count; i++) {
                        BlockRow blockRow = List[i];
                        order[i] = blockRow.Index;
                    }
                    return order;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    [SerializeField]
    public bool Randomize = false;

    [SerializeField]
    [Reorderable(elementNameOverride = "block")]
    public BlockRowList List;

    static ExperimentDesign experimentDesign;

    RowHolder() {
        List = new BlockRowList();
    }
    public static RowHolder BlockOrderFrom(VariableConfigurationFile configFile) {
        RowHolder newHolder = new RowHolder();
        experimentDesign = ExperimentDesign.CreateFrom(configFile);
        for (int rowIndex = 0; rowIndex < experimentDesign.BaseBlockTable.Rows.Count; rowIndex++) {
            DataRow row = experimentDesign.BaseBlockTable.Rows[rowIndex];
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn column in experimentDesign.BaseBlockTable.Columns) {
                sb.Append(column.ColumnName + "= " + row[column.ColumnName]);
            }

            
            newHolder.List.Add(new BlockRow(rowIndex, sb.ToString()));
        }

        return newHolder;
    }
    
   
}

[Serializable]
public class BlockRowList : ReorderableArray<BlockRow> {
}

[Serializable]
public class BlockRow {
    int index;
    public string row;
    public int Index => index;
    public BlockRow(int index, string rowString) {
        this.index = index;
        this.row = rowString;
    }
    
}
}