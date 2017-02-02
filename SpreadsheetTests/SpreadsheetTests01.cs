// Implemented by Snehashish Mishra for CS 3500, February 2016.
// uID: u0946268

using System;
using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace SpreadsheetTests01
{
    /// <summary>
    /// Provides testing to the extended Spreadsheet class.
    /// </summary>
    [TestClass]
    public class SpreadsheetTests01 : Spreadsheet
    {

        // MIRROR PROTECTED METHODS FOR TESTING PURPOSES

        /// <summary>
        /// Mirror of base constructor
        /// </summary>
        public SpreadsheetTests01() : base() { }

        /// <summary>
        /// Mirror of constructor with Regex
        /// </summary>
        public SpreadsheetTests01(Regex isValid) : base(isValid) { }

        /// <summary>
        /// Mirror of constructor with TextReader
        /// </summary>
        public SpreadsheetTests01(TextReader source) : base(source) { }

        /// <summary>
        /// Mirror of SetCellContents to text for protected testing.
        /// </summary>
        public new ISet<string> SetCellContents(string name, string text)
        {
            return base.SetCellContents(name, text);
        }

        /// <summary>
        /// Mirror of SetCellContents to number for protected testing.
        /// </summary>
        public new ISet<string> SetCellContents(string name, double number)
        {
            return base.SetCellContents(name, number);
        }

        /// <summary>
        /// Mirror of SetCellContents to Formula for protected testing.
        /// </summary>
        public new ISet<string> SetCellContents(string name, Formula formula)
        {
            return base.SetCellContents(name, formula);
        }

        /// <summary>
        /// Mirror of SetContentsOfCell for protected testing.
        /// </summary>
        public new ISet<string> SetContentsOfCell(string name, string content)
        {
            return base.SetContentsOfCell(name, content);
        }

        /// <summary>
        /// Mirror of GetDirectDependents for protected testing.
        /// </summary>
        public new IEnumerable<string> GetDirectDependents(string name)
        {
            return base.GetDirectDependents(name);
        }

        // START TESTING

        /// <summary>
        /// Tests what's returned from SetCellContents using double
        /// </summary>
        [TestMethod]
        public void SetCellContentsDouble01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("b1", new Formula("a1*2"));
            s1.SetCellContents("c1", new Formula("b1+a1"));
            HashSet<string> dependents = new HashSet<string>(s1.SetCellContents("a1", 1.0));

            Assert.IsTrue(dependents.Contains("A1"));
            Assert.IsTrue(dependents.Contains("B1"));
            Assert.IsTrue(dependents.Contains("C1"));
        }

        /// <summary>
        /// Tests InValidNameException - b1a1
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDouble02()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("b1a1", 0.8);
        }

        /// <summary>
        /// Tests InValidNameException - 1a
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDouble03()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("1a", 0.8);
        }

        /// <summary>
        /// Tests InValidNameException - null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDouble04()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents(null, 0.8);
        }

        /// <summary>
        /// Tests replacing the contents of the cell with double
        /// </summary>
        [TestMethod]
        public void SetCellContentsDouble05()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("a1", 0.8);
            Assert.AreEqual(s1.GetCellContents("a1"), 0.8);

            //Replace the contents
            s1.SetCellContents("a1", 1.8);
            Assert.AreEqual(s1.GetCellContents("a1"), 1.8);
        }

        /// <summary>
        /// Only dependents - itself.
        /// </summary>
        [TestMethod]
        public void SetCellContentsDouble06()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();

            s1.SetCellContents("b1", "c1 * 2");
            s1.SetCellContents("c1", "a1 * a1");
            s1.SetCellContents("d1", "2.0");
            s1.SetCellContents("z1", "Hello");

            HashSet<string> dependents = new HashSet<string>(s1.SetCellContents("A1", "19.0"));

            Assert.IsFalse(dependents.Contains("B1"));
            Assert.IsFalse(dependents.Contains("C1"));
            Assert.IsFalse(dependents.Contains("D1"));
            Assert.IsFalse(dependents.Contains("Z1"));
            Assert.IsTrue(dependents.Contains("A1"));
        }

        /// <summary>
        /// Tests InValidNameException - b1a1
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsText01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("b1a1", "Pikachu");
        }

        /// <summary>
        /// Tests InValidNameException - 1a
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsText02()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("1a", "Pikachu");
        }

        /// <summary>
        /// Tests null text
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsText03()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("A1", null);
        }

        /// <summary>
        /// Add text to empty cell
        /// </summary>
        [TestMethod]
        public void SetCellContentsText04()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("A1", "Pikachu");
            Assert.AreEqual(s1.GetCellContents("a1"), "Pikachu");
        }

        /// <summary>
        /// Re-write text in a cell
        /// </summary>
        [TestMethod]
        public void SetCellContentsText05()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("A1", "Pikachu");
            Assert.AreEqual(s1.GetCellContents("a1"), "Pikachu");

            s1.SetCellContents("A1", "Balbasaur");
            Assert.AreEqual(s1.GetCellContents("a1"), "Balbasaur");
        }

        /// <summary>
        /// Empty text in a cell
        /// </summary>
        [TestMethod]
        public void SetCellContentsText06()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("A1", "");
            Assert.AreEqual(s1.GetCellContents("a1"), "");
        }

        /// <summary>
        /// Multiple dependencies
        /// </summary>
        [TestMethod]
        public void SetCellContentsText07()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            
            s1.SetCellContents("b1", new Formula("c1 * 2"));
            s1.SetCellContents("c1", new Formula("a1 * a1"));
            s1.SetCellContents("d1", new Formula("c1 * b1"));
            s1.SetCellContents("z1", "Hello");

            HashSet<string> dependents = new HashSet<string>(s1.SetCellContents("A1", "2"));

            Assert.IsTrue(dependents.Contains("B1"));
            Assert.IsTrue(dependents.Contains("C1"));
            Assert.IsTrue(dependents.Contains("D1"));
            Assert.IsFalse(dependents.Contains("Z1"));
        }

        /// <summary>
        /// Only dependents - itself.
        /// </summary>
        [TestMethod]
        public void SetCellContentsText08()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();

            s1.SetCellContents("b1", "c1 * 2");
            s1.SetCellContents("c1", "a1 * a1");
            s1.SetCellContents("d1", "2.0");
            s1.SetCellContents("z1", "Hello");

            HashSet<string> dependents = new HashSet<string>(s1.SetCellContents("A1", "2"));

            Assert.IsFalse(dependents.Contains("B1"));
            Assert.IsFalse(dependents.Contains("C1"));
            Assert.IsFalse(dependents.Contains("D1"));
            Assert.IsFalse(dependents.Contains("Z1"));
            Assert.IsTrue(dependents.Contains("A1"));
        }

        /// <summary>
        /// Tests InValidNameException - b1a1
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetContentsOfCell("b1a1", "=a1 + B1");
        }

        /// <summary>
        /// Tests InValidNameException - 1a
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell02()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetContentsOfCell("1a", "=a1 + B1");
        }

        /// <summary>
        /// Tests InValidNameException - null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell03()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetContentsOfCell(null, "=a1 + B1");
        }

        /// <summary>
        /// Tests InValidNameException (false isValid regex)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell04()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.SetContentsOfCell("A9", "=a1 + B1");
        }

        /// <summary>
        /// Tests InValidNameException (false isValid regex)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell05()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.SetContentsOfCell("a2", "=a1 + B1");
        }
        
        /// <summary>
        /// Tests InValidNameException (false isValid regex)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell06()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.SetContentsOfCell("", "=a1 + B1");
        }

        /// <summary>
        /// Valid addition of formula - formula error value
        /// </summary>
        [TestMethod]
        public void SetContentsOfCell07()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.SetContentsOfCell("A2", "=A1 + B1");

            FormulaError err = (FormulaError) s1.GetCellValue("A2");
            Assert.AreEqual(new FormulaError(err.Reason), s1.GetCellValue("A2"));
        }

        /// <summary>
        /// Valid addition of formula - double value
        /// </summary>
        [TestMethod]
        public void SetContentsOfCell16()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.SetContentsOfCell("B1", "1.0");
            s1.SetContentsOfCell("A2", "=B1 + B1");
            Assert.AreEqual(2.0, s1.GetCellValue("A2"));
        }

        /// <summary>
        /// Valid addition of formula - default constructor and 
        /// different case cell name
        /// </summary>
        [TestMethod]
        public void SetContentsOfCell08()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetContentsOfCell("A52", "=A1 + B1");

            FormulaError err = (FormulaError)s1.GetCellValue("a52");
            Assert.AreEqual(new FormulaError(err.Reason), s1.GetCellValue("a52"));
        }

        /// <summary>
        /// Valid addition of formula - default constructor and 
        /// different case cell name
        /// </summary>
        [TestMethod]
        public void SetContentsOfCell09()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetContentsOfCell("a52", "=A1 + B1");

            FormulaError err = (FormulaError)s1.GetCellValue("A52");
            Assert.AreEqual(new FormulaError(err.Reason), s1.GetCellValue("A52"));
        }

        /// <summary>
        /// Invalid content
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsOfCell10()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetContentsOfCell("a52", null);
        }

        /// <summary>
        /// Valid addition of formula - default constructor
        /// </summary>
        [TestMethod]
        public void SetContentsOfCell11()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetContentsOfCell("azf23", "hello");
            Assert.AreEqual("hello", s1.GetCellValue("azF23"));
        }

        /// <summary>
        /// Valid addition of string
        /// </summary>
        [TestMethod]
        public void SetContentsOfCell12()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.SetContentsOfCell("A2", "hello");
            Assert.AreEqual("hello", s1.GetCellValue("A2"));
        }

        /// <summary>
        /// Valid addition of empty string
        /// </summary>
        [TestMethod]
        public void SetContentsOfCell13()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.SetContentsOfCell("A2", "");
            Assert.AreEqual("", s1.GetCellValue("A2"));
        }

        /// <summary>
        /// Valid addition of a double literal
        /// </summary>
        [TestMethod]
        public void SetContentsOfCell14()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.SetContentsOfCell("A2", "2.0");
            Assert.AreEqual(2.0, s1.GetCellValue("A2"));
        }

        /// <summary>
        /// Tests what's returned when a cell is stored with 
        /// double literal. Also tests if the dependents cells 
        /// were recalculated and if the independent cells were 
        /// left untouched.
        /// </summary>
        [TestMethod]
        public void SetContentsOfCell15()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            Assert.IsFalse(s1.Changed);

            s1.SetContentsOfCell("B2", "=A2 + c1");
            s1.SetContentsOfCell("c1", "=a2");
            s1.SetContentsOfCell("d1", "1.9");
            Assert.AreEqual(1.9, s1.GetCellValue("d1"));

            HashSet<string> dependents = new HashSet<string>(s1.SetContentsOfCell("A2", "2.0"));
            Assert.AreEqual(2.0, s1.GetCellValue("A2"));
            Assert.IsTrue(s1.Changed);

            Assert.IsTrue(dependents.Contains("A2"));
            Assert.IsTrue(dependents.Contains("B2"));
            Assert.IsTrue(dependents.Contains("C1"));
            Assert.IsFalse(dependents.Contains("D1"));

            // Were the dependent cells recalculated (and independent left untouched)?
            Assert.AreEqual(4.0, s1.GetCellValue("b2"));        // dependent
            Assert.AreEqual(2.0, s1.GetCellValue("c1"));        // dependent
            Assert.AreEqual(1.9, s1.GetCellValue("d1"));        // independent
        }

        /// <summary>
        /// Tests InValidNameException - b1a1
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormula01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("b1a1", new Formula("a1 + B1"));
        }

        /// <summary>
        /// Tests InValidNameException - 1a
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormula02()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("1a", new Formula("a1 + B1"));
        }

        /// <summary>
        /// Tests InValidNameException - null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormula03()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents(null, new Formula("a1 + B1"));
        }

        /// <summary>
        /// Tests null formula argument
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsFormula04()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("a57", new Formula(null));
        }

        /// <summary>
        /// Tests null formula argument
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsFormula10()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("a57", null);
        }

        /// <summary>
        /// Tests setting empty cell
        /// </summary>
        [TestMethod]
        public void SetCellContentsFormula05()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("a57", new Formula("b23 + a1 * g42"));
            Assert.AreEqual(s1.GetCellContents("A57").ToString(), new Formula("B23+A1*G42").ToString());
        }

        /// <summary>
        /// Tests re-setting formula cell
        /// </summary>
        [TestMethod]
        public void SetCellContentsFormula06()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("a57", new Formula("b23 + a1 * g42"));
            Assert.AreEqual(s1.GetCellContents("A57").ToString(), new Formula("B23+A1*G42").ToString());

            s1.SetCellContents("a57", new Formula("579"));
            Assert.AreEqual(s1.GetCellContents("A57").ToString(), "579");
        }

        /// <summary>
        /// Tests circular exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsFormula07()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("a1", new Formula("b1*2"));
            s1.SetCellContents("b1", new Formula("c1*2"));
            s1.SetCellContents("c1", new Formula("a1*2"));
        }

        /// <summary>
        /// Tests return value
        /// </summary>
        [TestMethod]
        public void SetCellContentsFormula08()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("b1", new Formula("c1*2"));
            s1.SetCellContents("c1", new Formula("a1*2"));
            s1.SetCellContents("d1", new Formula("b1*2"));
            HashSet<string> dependents = new HashSet<string>(s1.SetCellContents("a1", new Formula("2.0")));

            Assert.IsTrue(dependents.Contains("B1"));
            Assert.IsTrue(dependents.Contains("C1"));
            Assert.IsTrue(dependents.Contains("D1"));
        }

        /// <summary>
        /// Tests return value
        /// </summary>
        [TestMethod]
        public void SetCellContentsFormula09()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("b1", new Formula("c1*2"));
            s1.SetCellContents("c1", new Formula("a1*2"));
            s1.SetCellContents("d1", new Formula("2.2"));
            HashSet<string> dependents = new HashSet<string>(s1.SetCellContents("a1", new Formula("2.0")));

            Assert.IsTrue(dependents.Contains("B1"));
            Assert.IsTrue(dependents.Contains("C1"));
            Assert.IsFalse(dependents.Contains("D1"));
        }

        /// <summary>
        /// Tests GetCellContents when set contents to double
        /// </summary>
        [TestMethod]
        public void GetCellContents01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("zz1", 0.8);

            Assert.AreEqual(s1.GetCellContents("zz1"), 0.8);
        }

        /// <summary>
        /// Tests GetCellContents with invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents02()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("z1", 0.8);
            s1.GetCellContents("1z");
        }

        /// <summary>
        /// Tests GetCellContents with null name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents03()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("z1", 0.8);
            s1.GetCellContents(null);
        }

        /// <summary>
        /// Tests GetCellContents with invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents04()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.GetCellContents("z0");
        }

        /// <summary>
        /// Tests GetCellContents from empty cell
        /// </summary>
        [TestMethod]
        public void GetCellContents05()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            Assert.AreEqual("", s1.GetCellContents("zz1"));
        }

        /// <summary>
        /// Tests GetNamesOfAllNonemptyCells with empty spreadsheet
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCells01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            List<string> nonEmptyCells = new List<string>(s1.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(0, nonEmptyCells.Count);
        }

        /// <summary>
        /// Tests GetNamesOfAllNonemptyCells with some cells.
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCells02()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.SetCellContents("a1", "");
            s1.SetCellContents("b1", "Hi");
            s1.SetCellContents("c1", 2.0);

            List<string> nonEmptyCells = new List<string>(s1.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(2, nonEmptyCells.Count);
            Assert.IsTrue(nonEmptyCells.Contains("B1"));
            Assert.IsTrue(nonEmptyCells.Contains("C1"));
            Assert.IsFalse(nonEmptyCells.Contains("A1"));
        }

        /// <summary>
        /// Tests GetDirectDependents: name = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDirectDependents01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.GetDirectDependents(null);
        }

        /// <summary>
        /// Tests GetDirectDependents: invalid name format.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetDirectDependents02()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.GetDirectDependents("1a");
        }

        /// <summary>
        /// Tests GetDirectDependents: invalid name format.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetDirectDependents03()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.GetDirectDependents("a1");
        }

        /// <summary>
        /// Tests invalid cell name when getting its value - null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValue01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.GetCellValue(null);
        }

        /// <summary>
        /// Tests invalid cell name when getting its value - b1a1
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValue02()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01();
            s1.GetCellValue("b1a1");
        }

        /// <summary>
        /// Tests invalid cell name when getting its value - invalid regex
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValue03()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.GetCellValue("A23");
        }

        /// <summary>
        /// Tests Save() - null dest
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void Save01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.Save(null);
        }

        /// <summary>
        /// Tests Save() - empty dest
        /// </summary>
        [TestMethod]
        public void SaveAndDuplicate01()
        {
            SpreadsheetTests01 s1 = new SpreadsheetTests01(new Regex("^[A-Z]*[1-2]$"));
            s1.SetContentsOfCell("A1", "2.0");
            s1.SetContentsOfCell("B1", "Garfield");
            s1.SetContentsOfCell("C1", "=A1 * 2");
            s1.SetContentsOfCell("D1", "");

            s1.Save(File.CreateText("../../ spreadsheetTest01.xml"));

            SpreadsheetTests01 s2 = new SpreadsheetTests01(File.OpenText(@"../../ spreadsheetTest01.xml"));
            Assert.AreEqual(2.0, s2.GetCellContents("A1"));
            Assert.AreEqual("Garfield", s2.GetCellContents("B1"));
            Assert.AreEqual(new Formula("A1 * 2").ToString(), s2.GetCellContents("C1").ToString());
            Assert.IsFalse(s2.Changed);
        }
        
        /// <summary>
        /// Tests Constructor() - null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void SaveAndDuplicate02()
        {
            TextReader tr = null;
            SpreadsheetTests01 s2 = new SpreadsheetTests01(tr);
        }

        /// <summary>
        /// Tests dupicate cells in XML
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLExceptions01()
        {
            SpreadsheetTests01 s2 = new SpreadsheetTests01(File.OpenText(@"../../ duplicateCells.xml"));
        }

        /// <summary>
        /// Tests invalid cell name in XML
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLExceptions02()
        {
            SpreadsheetTests01 s2 = new SpreadsheetTests01(File.OpenText(@"../../ InValidCellName.xml"));
        }

        /// <summary>
        /// Tests invalid formula in XML
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLExceptions03()
        {
            SpreadsheetTests01 s2 = new SpreadsheetTests01(File.OpenText(@"../../ InValidForula.xml"));
        }

        /// <summary>
        /// Tests circular dependency in XML
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLExceptions04()
        {
            SpreadsheetTests01 s2 = new SpreadsheetTests01(File.OpenText(@"../../ Circular.xml"));
        }

        /// <summary>
        /// Tests invalid format of XML (validation error)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLExceptions05()
        {
            SpreadsheetTests01 s2 = new SpreadsheetTests01(File.OpenText(@"../../ ValidationError.xml"));
        }

        /// <summary>
        /// Tests empty source path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void XMLExceptions06()
        {
            SpreadsheetTests01 s2 = new SpreadsheetTests01(File.OpenText(""));
        }

        /// <summary>
        /// Tests illegal characters embedded in XML
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void XMLExceptions07()
        {
            SpreadsheetTests01 s2 = new SpreadsheetTests01(File.OpenText(@"../../ IllegalChars.xml"));
        }
    }
}
