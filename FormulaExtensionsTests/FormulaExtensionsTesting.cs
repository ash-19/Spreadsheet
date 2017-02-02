// Written and implemented by Snehashish Mishra for CS 3500, January 2016.
// uID: u0946268

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System.Text.RegularExpressions;

namespace FormulaExtensionsTests
{
    /// <summary>
    /// This class provides testing on the <i>extensions</i>  
    /// made to the Formula struct.
    /// </summary>
    [TestClass]
    public class FormulaExtensionsTesting
    {
        /// <summary>
        /// Tests the default zero argument constructor 
        /// and the expected behavior from all the public 
        /// methods.
        /// </summary>
        [TestMethod]
        public void Construct01()
        {
            Formula f = new Formula();
            Assert.AreEqual("0", f.ToString());
            Assert.AreEqual(0.0, f.Evaluate(s => 0), 1e-6);
            Assert.AreEqual(0, f.GetVariables().Count);
            Assert.IsFalse(f.GetVariables().Contains("0"));
        }

        /// <summary>
        /// Constructor with all three arguments set to 
        /// null to throw an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Construct02()
        {
            Formula f1 = new Formula(null, null, null);
        }

        /// <summary>
        /// Only the Normalizer argument set to null to 
        /// throw an exception.
        /// </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Construct03()
        {
            Formula f1 = new Formula("x + y", null, s => true);
        }

        /// <summary>
        /// String and Normalizer set to null to throw an 
        /// exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Construct04()
        {
            Formula f1 = new Formula(null, null, s => true);
        }

        /// <summary>
        /// Validator and Normalizer set to null to throw an 
        /// exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Construct05()
        {
            Formula f1 = new Formula("x + y", null, null);
        }

        /// <summary>
        /// Formula passed in valid form before applying the 
        /// Normalizer which makes it invalid by turning it 
        /// into an empty string.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Validity01()
        {
            Formula f1 = new Formula("x + y", s => "", s => true);
        }

        /// <summary>
        /// Normalizer invalidates a valid formula by introducing  
        /// a special character in every variable.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Validity02()
        {
            Formula f1 = new Formula("x + y", s => "_" + s, s => true);
        }

        /// <summary>
        /// Normalizer produces a valid formula but Validator 
        /// invalidates it.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Validity03()
        {
            Formula f1 = new Formula("x + y", s => s + s.Length, s => false);
        }

        /// <summary>
        /// Normalizer porduces a valid formula but Validator 
        /// invalidates it since it only accepts lower case 
        /// variables.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Validity05()
        {
            Formula f1 = new Formula("X + Y",
                        s => s + s.Length,
                            s => Regex.IsMatch(s, "^[a-z][0-9a-z]*$"));
        }

        /// <summary>
        /// Normalizer invalidates a valid formula by replacing 
        /// every variable by literal 1.0.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Validity04()
        {
            Formula f1 = new Formula("x + y", s => "1.0", s => true);
        }

        /// <summary>
        /// Both Normalizer and Validator invalidate the formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Validity06()
        {
            Formula f1 = new Formula("x + y", s => "_" + s, s => false);
        }

        /// <summary>
        /// Both Normalizer and Validaotr produce a valid formula 
        /// but the passed formula was invalid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Validity07()
        {
            Formula f1 = new Formula("_x + _y", s => s + s.Length, s => true);
        }

        /// <summary>
        /// Tests Evaluate() on the zero constructor and 
        /// the other constructors with same functioning.
        /// </summary>
        [TestMethod]
        public void Evaluate01()
        {
            Formula f = new Formula();
            Formula f1 = new Formula("0");
            Formula f2 = new Formula("0", s => s, s => true);

            // Test all 3 evaluate to 0.0
            Assert.AreEqual(f1.Evaluate(s => 0), f.Evaluate(s => 0), 1e-6);
            Assert.AreEqual(f2.Evaluate(s => 0), f.Evaluate(s => 0), 1e-6);
            Assert.AreEqual(f1.Evaluate(s => 0), f2.Evaluate(s => 0), 1e-6);
        }

        /// <summary>
        /// Tests GetVariables() on zero constructor and 
        /// the other constructors with same functioning.
        /// </summary>
        [TestMethod]
        public void GetVariables01()
        {
            Formula f = new Formula();
            Formula f1 = new Formula("0");
            Formula f2 = new Formula("0", s => s, s => true);

            // Test if all have empty variables set
            Assert.IsFalse(f.GetVariables().Contains("0"));
            Assert.IsFalse(f1.GetVariables().Contains("0"));
            Assert.IsFalse(f2.GetVariables().Contains("0"));

            // Test # of variables in all 3 equal to zero
            Assert.AreEqual(f1.GetVariables().Count, f.GetVariables().Count);
            Assert.AreEqual(f2.GetVariables().Count, f.GetVariables().Count);
            Assert.AreEqual(f1.GetVariables().Count, f2.GetVariables().Count);
        }

        /// <summary>
        /// Tests GetVariables() on unformatted string argument 
        /// using single argument constructor.
        /// </summary>
        [TestMethod]
        public void GetVariables02()
        {
            Formula f = new Formula("x + y *(1      -2  )");

            Assert.IsTrue(f.GetVariables().Contains("x"));
            Assert.IsTrue(f.GetVariables().Contains("y"));

            Assert.IsFalse(f.GetVariables().Contains("+"));
            Assert.IsFalse(f.GetVariables().Contains("*"));
            Assert.IsFalse(f.GetVariables().Contains("("));
            Assert.IsFalse(f.GetVariables().Contains("1"));
            Assert.IsFalse(f.GetVariables().Contains("-"));
            Assert.IsFalse(f.GetVariables().Contains("2"));
            Assert.IsFalse(f.GetVariables().Contains(")"));
            Assert.IsFalse(f.GetVariables().Contains(" "));

            Assert.AreEqual(2, f.GetVariables().Count);
        }

        /// <summary>
        /// Tests toString() on unformatted string argument
        /// </summary>
        [TestMethod]
        public void ToString01()
        {
            Formula f = new Formula("x + y *(1      -2  )");
            Assert.AreEqual("x + y * ( 1 - 2 )", f.ToString());
        }

        /// <summary>
        /// Two toString() formulae comparison. The second formula 
        /// is created by passing 1st formula as a string.
        /// </summary>
        [TestMethod]
        public void ToString02()
        {
            Formula f1 = new Formula("x + y * (1-2  )");
            Assert.AreEqual("x + y * ( 1 - 2 )", f1.ToString());

            Formula f2 = new Formula(f1.ToString(), s => s, s => true);
            Assert.AreEqual(f2.ToString(), f1.ToString());
        }

        /// <summary>
        /// Tests toString() on a single variable formula
        /// </summary>
        [TestMethod]
        public void ToString03()
        {
            Formula f = new Formula("x");
            Assert.AreEqual("x", f.ToString());
        }

        /// <summary>
        /// Tests toString() on zero argument constructor and 
        /// the other constructors with same functioning.
        /// </summary>
        [TestMethod]
        public void ToString04()
        {
            Formula f = new Formula();
            Formula f1 = new Formula("0");
            Formula f2 = new Formula("0", s => s, s => true);

            // Test ToString() of all 3
            Assert.AreEqual(f1.ToString(), f.ToString());
            Assert.AreEqual(f2.ToString(), f.ToString());
            Assert.AreEqual(f1.ToString(), f2.ToString());
        }

        /// <summary>
        /// Tests toString() after normalizing variables 
        /// to caps.
        /// </summary>
        [TestMethod]
        public void ToString05()
        {
            string formula = "x + y * (1-2  )";

            Formula f1 = new Formula(formula, s => s.ToUpper(), s => true);
            Assert.AreEqual("X + Y * ( 1 - 2 )", f1.ToString());
        }
    }
}
