using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public static class DataTableExtension
{
    public static void PrintToConsole(this DataTable dt) {
        foreach (DataRow row in dt.Rows) {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            foreach (DataColumn col in dt.Columns)
                sw.Write(row[col] + "\t");
            string output = sw.ToString();
            Debug.Log(output);
        }
        //Console.ReadKey();
    }

    public static string PrintToString(this DataTable dt) {
        StringWriter main = new StringWriter();
        foreach (DataRow row in dt.Rows) {
            StringWriter sw = new StringWriter();
            foreach (DataColumn col in dt.Columns)
                sw.Write(row[col] + "\t");
            sw.Write("\n");
            main.Write(sw);
        }

        string mainString = main.ToString();
        return mainString;
        //Console.ReadKey();
    }

}
