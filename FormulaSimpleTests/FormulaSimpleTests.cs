// Written by Joe Zachary for CS 3500, January 2016.
// Reapired error in Evaluate5.  Added TestMethod Attribute
// for Evaluate4 and Evaluate5 - JLZ January 25, 2016
//
// Additional unit test case were added by Snehashish Mishra 
// for CS 3500, January 2016.
// uID: u0946268

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;

namespace FormulaTestCases
{
    /// <summary>
    /// Provides testing to the base specification of the 
    /// Format struct.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// Invalid formula used to throw an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct1()
        {
            Formula f = new Formula("_");
        }

        /// <summary>
        /// This is another syntax error - Illegal operator
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct2()
        {
            Formula f = new Formula("2++3");
        }

        /// <summary>
        /// Another syntax error - Only digits.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct3()
        {
            Formula f = new Formula("2 3");
        }

        /// <summary>
        /// Tests ArgumentNullException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Construct4()
        {
            Formula f = new Formula(null);
        }

        /// <summary>
        /// Another syntax error - Starts with digits.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct5()
        {
            Formula f = new Formula("2x + 3y");
        }

        /// <summary>
        /// Another syntax error - starts with a special symbol
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct6()
        {
            Formula f = new Formula("_x + _y");
        }

        /// <summary>
        /// Another syntax error - Empty String.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct7()
        {
            Formula f = new Formula("");
        }

        /// <summary>
        /// Another syntax error - Only white-spaces 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct8()
        {
            Formula f = new Formula("  ");
        }

        /// <summary>
        /// Another syntax error - Too many closing brackets.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct9()
        {
            Formula f = new Formula("((x + 1) + y + 2) - 1))");
        }

        /// <summary>
        /// Another syntax error - Too many opening brackets.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct10()
        {
            Formula f = new Formula("(((((x + 1) + y + 2) - 1))");
        }

        /// <summary>
        /// Another syntax error - Operator following opening 
        /// brackets.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct11()
        {
            Formula f = new Formula("( + x )");
        }

        /// <summary>
        /// Another syntax error - Closing bracket following 
        /// opening bracket.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct12()
        {
            Formula f = new Formula("() + x");
        }

        /// <summary>
        /// Another syntax error - Literal following closing bracket.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct13()
        {
            Formula f = new Formula("( x + y ) 3");
        }

        /// <summary>
        /// Another syntax error - Opening bracket following 
        /// closing bracket.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct14()
        {
            Formula f = new Formula(")( + x");
        }

        /// <summary>
        /// Another syntax error - Variable following closing 
        /// bracket.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct15()
        {
            Formula f = new Formula("( x + y ) z");
        }

        /// <summary>
        /// Another syntax error - Literal preceding opening 
        /// bracket.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct16()
        {
            Formula f = new Formula(" x ( x + y )");
        }

        /// <summary>
        /// Another syntax error - Invalid second token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct17()
        {
            Formula f = new Formula("(12x)");
        }

        /// <summary>
        /// Another syntax error - Invalid last token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct18()
        {
            Formula f = new Formula("(12 + 1) +");
        }

        /// <summary>
        /// Another syntax error - invalid variable.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct19()
        {
            Formula f = new Formula("(_x)");
        }

        /// <summary>
        /// Another syntax error - Negative literal.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct20()
        {
            Formula f = new Formula("(-19)");
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2()
        {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// FormulaEvaluationException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below. We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4()
        {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token, with more variables.
        /// </summary>
        [TestMethod]
        public void Evaluate6()
        {
            Formula f = new Formula("(x + y) * (z / x) * (y / z) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 15.0, 1e-6);
        }

        /// <summary>
        /// This tests operator precedence of * and /.
        /// </summary>
        [TestMethod]
        public void Evaluate7()
        {
            Formula f = new Formula("x * y * z / x * y / z * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 36.0, 1e-6);
        }

        /// <summary>
        /// This tests whether the result obtained is 
        /// correct from a formula containing lots of white 
        /// spaces.
        /// </summary>
        [TestMethod]
        public void Evaluate8()
        {
            Formula f = new Formula("   x    *    2   *  z * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 64.0, 1e-6);
        }

        /// <summary>
        /// This tests whether the result obtained is 
        /// correct from a formula containing variables 
        /// with unusually long and mixed names.
        /// </summary>
        [TestMethod]
        public void Evaluate9()
        {
            Formula f = new Formula("x126xy99023843984329hcsdhdshdkh93284923 *"
                + " x126xy99023843984329hcsdhdshdkh93284923 / z  * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 0.5, 1e-6);
        }

        /// <summary>
        /// Division by zero
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate10()
        {
            Formula f = new Formula("x *y * z + x / v");
            f.Evaluate(Lookup4);
        }

        /// <summary>
        /// Division of a variable by 0
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate11()
        {
            Formula f = new Formula("x / s");
            f.Evaluate(Lookup4);
        }

        /// <summary>
        /// This tests operator precedence of + and -.
        /// </summary>
        [TestMethod]
        public void Evaluate12()
        {
            Formula f = new Formula("x + y + z - x - y + z - 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 15.0, 1e-6);
        }

        /// <summary>
        /// This tests operators and parenthesis with only literals.
        /// </summary>
        [TestMethod]
        public void Evaluate13()
        {
            Formula f = new Formula("(2 + 3 + 4.0) + ( ( 3 + 1.0 ) )");
            Assert.AreEqual(f.Evaluate(Lookup4), 13.0, 1e-6);
        }

        /// <summary>
        /// This tests a long formula.
        /// </summary>
        [TestMethod]
        public void Evaluate14()
        {
            Formula f = new Formula("x + y - z + (x - y) "
                    + "* 2 / 4 - (81 / 9 ) * 2 - z + (x * (12 /  "
                    + "(y - x) - 1) * 1.0) + ( 2 * (2 / 2) - (1 "
                    + "- 1) + 1) + 4");
            Assert.AreEqual(f.Evaluate(Lookup4), 2.0, 1e-6);
        }

        /// <summary>
        /// Division of a literal by 0
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate15()
        {
            Formula f = new Formula("2 / 0");
            f.Evaluate(Lookup4);
        }

        /// <summary>
        /// Division of a token by 0 after closing bracket
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate16()
        {
            Formula f = new Formula("x / (1 - 1)");
            f.Evaluate(Lookup4);
        }

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v)
        {
            switch (v)
            {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                case "v": return 0.0;
                case "x126xy99023843984329hcsdhdshdkh93284923": return 2.0;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}