using System;
using System.Collections;
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
        Name = $"Unnamed Variable (type:{typeof(T)})";
    }


    //public virtual string ValueAsString()

}

public enum VariableType {
    Independent,
    Dependent
}

public enum DataType {
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
    public    VariableType   TypeOfVariable;
    public    bool           RandomizeOrder;
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

    [SerializeField]
    public List<DatumGameObject> GameObjectData = new List<DatumGameObject>();

    [SerializeField]
    public List<DatumVector3> Vector3Data = new List<DatumVector3>();

    [SerializeField]
    public List<DatumVector2> Vector2Data = new List<DatumVector2>();

    [SerializeField]
    public List<DatumCustom> CustomData = new List<DatumCustom>();

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


    public DataType TypeToCreate;

    public Datum New() {
        return New(TypeToCreate);
    }

    public Datum New(DataType type) {
        switch (TypeToCreate) {
            case DataType.Int:
                DatumInt newDatumInt = new DatumInt();
                return newDatumInt;
            case DataType.Float:
                DatumFloat newDatumFloat = new DatumFloat();
                return newDatumFloat;
            //case DataType.String:
            //    stringData.Add(new DatumString());
            //    break;
            //case DataType.GameObject:
            //    GameObjectData.Add(new DatumGameObject());
            //    break;
            //case DataType.Vector3:
            //    Vector3Data.Add(new DatumVector3());
            //    break;
            //case DataType.Vector2:
            //    Vector2Data.Add(new DatumVector2());
            //    break;
            //case DataType.CustomDatum:
            //    CustomData.Add(new DatumCustom());
            //    break;
            case DataType.ChooseType:
                throw new InvalidEnumArgumentException("Trying to create new datum, but not type not yet chosen");
            default:
                throw new NotImplementedException("Support for this data type has not yet been defined. " +
                                                  "You can customize it yourself in the Datum.cs class");
        }

        

    }

    public DataTable ToTable() {
        return ExperiementTable.GetTable(AllData);
    }

    public void Add(Datum datum) {
        if (datum.Type == typeof(int)) {
            intData.Add((DatumInt)datum);
        }
        else if (datum.Type == typeof(float)) {
            floatData.Add((DatumFloat) datum);
        }
        else {
            throw new NotImplementedException("Support for this data type has not yet been defined. " +
                                              "You can add support for it yourself in the Datum.cs class if needed");
        }

        Debug.Log($"added new datum of type {TypeToCreate}");
        TypeToCreate = DataType.ChooseType;
    }

    public DatumInt NewInt(List<int> intList) {
        DatumInt newDatumInt = (DatumInt) New(DataType.Int);
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

public enum SupportedTypes {
    Int,
    Float,
    String,
    GameObject,
    Vector3,
    Vector2,
    Custom
}

[Serializable]
public class DatumInt : Datum<int> {

}

[Serializable]
public class DatumFloat : Datum<float> {

}
[Serializable] public class DatumString : Datum<string> {//TODO
                                                         }

[Serializable]
public class DatumGameObject : Datum<GameObject> {
    //TODO
}

[Serializable]
public class DatumVector3 : Datum<Vector3> {
    //TODO
}

[Serializable]
public class DatumVector2 : Datum<Vector2> {
    //TODO
}

[Serializable]
public class DatumCustom : Datum<CustomDatum> {
    //TODO
}

public interface CustomDatum {
    //TODO
}

