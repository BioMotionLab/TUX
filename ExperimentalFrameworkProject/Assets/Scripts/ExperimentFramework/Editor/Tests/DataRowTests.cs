using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class DataRowTests {

    public class OutputStringMethod {
                  
        [Test]
        public void CallOnOneVariable_GetTheString() {
            simpleDataRow dataRow = new simpleDataRow();

            string result = dataRow.data;
            string expected = "data";
            Assert.AreEqual(result, expected);
        }

    }

   

    
}

public class simpleDataRow {
    public string data = "data";
}
