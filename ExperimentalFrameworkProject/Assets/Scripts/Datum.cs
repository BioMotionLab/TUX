using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using UnityEngine;

[Serializable]
public abstract class Datum<T> : Datum {

    public override Type Type => typeof(T);

    public List<T> Values;

    protected Datum() {
        Values = new List<T>();
        Name = $"Unnamed Variable (types:{typeof(T)})";
    }


    //public virtual string ValueAsString()

}

public enum VariableType {
    Independent,
    Dependent
}

public enum SupportedDataTypes {
    Int,
    Float,
    String,
    //GameObject,
    //Vector3,
    //Vector2,
    //CustomDatum,
    ChooseType,
}

public abstract class Datum {
    public    string         Name;
    public abstract SupportedDataTypes DataType { get; }
    public    VariableType   TypeOfVariable;
    public VariableMixingType MixingTypeOfVariable;
    public    bool           ShuffleOrder;
    public    bool           Block;

    public abstract Type Type { get; }
    
}

[Serializable]
public class DatumFactory {

    [SerializeField]
    public List<DatumInt> intData = new List<DatumInt>();

    [SerializeField]
    public List<DatumFloat> floatData = new List<DatumFloat>();

    [SerializeField]
    public List<DatumString> stringData = new List<DatumString>();

    //[SerializeField]
    //public List<DatumGameObject> GameObjectData = new List<DatumGameObject>();

    //[SerializeField]
    //public List<DatumVector3> Vector3Data = new List<DatumVector3>();

    //[SerializeField]
    //public List<DatumCustom> CustomData = new List<DatumCustom>();

    public List<Datum> AllData {
        get {
            List<Datum> data = new List<Datum>();
            foreach (var intDatum in intData) {
                data.Add(intDatum);
            }

            foreach (var floatDatum in floatData) {
                data.Add(floatDatum);
            }
            //TODO add others


            //Organize order of datums
            List<Datum> organizedDatums = new List<Datum>();
            foreach (var datum in data) {
                if (!datum.Block && datum.TypeOfVariable == VariableType.Independent) {
                    organizedDatums.Add(datum);
                }
            }

            foreach (var datum in data) {
                if (datum.Block && datum.TypeOfVariable == VariableType.Independent) {
                    organizedDatums.Add(datum);
                }
            }

            foreach (var datum in data) {
                if (datum.TypeOfVariable == VariableType.Dependent) {
                    organizedDatums.Add(datum);
                }
            }
            return organizedDatums;
        }
    }

    public SupportedDataTypes TypesToCreate;

    public Datum New() {
        return New(TypesToCreate);
    }

    public Datum New(SupportedDataTypes types) {
        switch (types) {
            case SupportedDataTypes.Int:
                DatumInt newDatumInt = new DatumInt();
                return newDatumInt;
            case SupportedDataTypes.Float:
                DatumFloat newDatumFloat = new DatumFloat();
                return newDatumFloat;
            //case SupportedDataTypes.String:
            //    stringData.Add(new DatumString());
            //    break;
            //case SupportedDataTypes.GameObject:
            //    GameObjectData.Add(new DatumGameObject());
            //    break;
            //case SupportedDataTypes.Vector3:
            //    Vector3Data.Add(new DatumVector3());
            //    break;
            //case SupportedDataTypes.Vector2:
            //    Vector2Data.Add(new DatumVector2());
            //    break;
            //case SupportedDataTypes.CustomDatum:
            //    CustomData.Add(new DatumCustom());
            //    break;
            case SupportedDataTypes.ChooseType:
                throw new InvalidEnumArgumentException("Trying to create new datum, but not types not yet chosen");
            default:
                throw new NotImplementedException("Support for this data types has not yet been defined." +
                                                  "You can customize it yourself in the Datum.cs class");
        }
    }

    public DataTable ToTable(bool shuffleTrialOrder, int numberRepetitions) {
        Debug.Log($"ToTable method in Datum: Alldata.count {AllData.Count}");
        return ExperimentTable.GetTable(AllData, shuffleTrialOrder, numberRepetitions);
    }

    public void Add(Datum datum) {
        if (datum.Type == typeof(int)) {
            intData.Add((DatumInt)datum);
        }
        else if (datum.Type == typeof(float)) {
            floatData.Add((DatumFloat) datum);
        }
        else {
            throw new NotImplementedException("Support for this data types has not yet been defined. " +
                                              "You can add support for it yourself in the Datum.cs class if needed");
        }

        Debug.Log($"added new datum of types {TypesToCreate}");
        TypesToCreate = SupportedDataTypes.ChooseType;
    }

    public DatumInt NewInt(string name, List<int> intList) {
        DatumInt newDatumInt = (DatumInt) New(SupportedDataTypes.Int);
        newDatumInt.Name = name;
        newDatumInt.Values = intList;
        return newDatumInt;
    }
}




public class IVs {
    public List<Datum> Blocked = new List<Datum>();
    public List<Datum> Regular = new List<Datum>();

    public void AddIn(List<Datum> list) {
        
    }
}



public enum VariableMixingType {
    Balanced,
    Looped,
    EvenProbability,
    CustomProbability
}

[Serializable]
public class DatumInt : Datum<int> {
    public override SupportedDataTypes DataType => SupportedDataTypes.Int;
}

[Serializable]
public class DatumFloat : Datum<float> {
    public override SupportedDataTypes DataType => SupportedDataTypes.Float;
}
[Serializable] public class DatumString : Datum<string> {
    public override SupportedDataTypes DataType => SupportedDataTypes.String;
}

//[Serializable]
//public class DatumGameObject : Datum<GameObject> {
//    //TODO
//}

//[Serializable]
//public class DatumVector3 : Datum<Vector3> {
//    //TODO
//}

//[Serializable]
//public class DatumVector2 : Datum<Vector2> {
//    //TODO
//}

//[Serializable]
//public class DatumCustom : Datum<CustomDatum> {
//    //TODO
//}

//public interface CustomDatum {
//    //TODO
//}

