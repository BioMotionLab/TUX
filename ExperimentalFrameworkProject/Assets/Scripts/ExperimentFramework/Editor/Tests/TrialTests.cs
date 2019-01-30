using NUnit.Framework;

public class TrialTests {

    public class RunMethod {
        SimpleDataRow simpleData;
        Trial simpleTrial;


        [SetUp]
        public void Setup() {
            simpleData = new SimpleDataRow();
            //simpleTrial = new Trial();
        }

        [Test]
        public void SimpleDataRowOfString_GetsOutput() {
            string output = "";
            string expected = "the data";
            //simpleTrial.Run(ref output, simpleData);
            Assert.AreEqual(expected, output);

        }


    }




}

public class SimpleDataRow {
    public string data = "the data";
}


