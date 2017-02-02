// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 23, 2016
// 
// Skeleton implemented by Snehashish Mishra for CS 3500, 
// January 2016. uID: u0946268
//
// Extensions added by Snehashish Mishra, February 10, 2016.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard 
    /// precedence rules. Provides a means to evaluate Formulas. Formulas can 
    /// be composed of non-negative floating-point numbers, variables, left 
    /// and right parentheses, and the four binary operator symbols +, -, *, 
    /// and /.  (The unary operators + and - are not allowed.) </summary>
    public struct Formula
    {
        /// <summary>
        /// Contains all the tokens parsed from the passed formula string 
        /// from left to right. All tokens are valid without any whitespaces.
        /// </summary>
        private List<string> formulaTokensList;

        /// <summary>
        /// Contains all the variables inserted during the formula construction. 
        /// This instance variable allows for a constant run-time behavior when 
        /// invoking GetVariables() irrespective of the size of the passed formula 
        /// (the number of variables in it). This performance gain more than 
        /// compensates for the extra reference address stored by each Formula 
        /// struct.</summary>
        private HashSet<string> variablesSet;
        
        // A default zero argument constructor is provided by the compiler  
        // which behaves as Formula f = new Formula("0");
        
        /// <summary>
        /// <para>Creates a Formula from a string that consists of a standard 
        /// infix expression composed from non-negative floating-point numbers 
        /// (using C#-like syntax for double/int literals), variable symbols 
        /// (a letter followed by zero or more letters and/or digits), left and 
        /// right parentheses, and the four binary operator symbols +, -, *, and 
        /// /. White space is permitted between tokens, but is not required.
        /// </para>
        /// 
        /// <para>Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) +      8"
        ///     "x*y-2+35/9"
        /// </para>
        ///  
        /// <para>Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// </para>
        /// 
        /// <para>If the formula is syntacticaly invalid, throws a 
        /// FormulaFormatException with an explanatory Message. If the passed 
        /// formula string references to null, throws an ArgumentNullException.
        /// </para></summary>
        public Formula(String formula) : this(formula, s => s, t => true)
        {
        }

        /// <summary>
        /// Creates a Formula from the passed string just like the one argument 
        /// constructor, but with extra restrictions set by the caller. In 
        /// addition to taking a formula string, it also taken in two delegates, 
        /// namely, Normalizer and Validator.
        /// 
        /// <para>A Normalizer converts variables into a canonical form. For 
        /// example, a Normalizer can be made to change the case of every variable 
        /// in the passed formula string. Or it can be made to append digits to 
        /// similar variables, etc.</para>
        /// 
        /// <para>A Validator imposes extra restrictions over the standard rules 
        /// of formula validation. For example, a new Formula can be made to adhere 
        /// to more stringent restrictions in addition to the standard restrictions 
        /// like variables can only be 1 letter and 1 digit, etc.</para>
        /// 
        /// <para>If a formula that was valid before becomes invalid after applying 
        /// the passed Normalizer N(x) to it according to the standard Formula rules,  
        /// results in a FormulaFormatException with an explanatory message.</para>
        /// 
        /// <para>Similarly, even if both the passed formula string and the N(x) results 
        /// in a valid Formula format but the restrictions imposed by the Validator on the 
        /// normalized formula V(N(x)) makes it invalid, then a FormulaFormatException 
        /// is thrown with an explanatory message.</para></summary>
        /// 
        /// <param name="formula">The string containing the formula to be constructed.</param>
        /// 
        /// <param name="normalize">A delegate which normalizes the variables.</param>
        /// 
        /// <param name="validate">A validator which imposes more restrictions on the 
        /// syntactic format of the Formula.</param>
        /// 
        public Formula(string formula, Normalizer normalize, Validator validate)
        {
            // Keeps track of the previous token for valid format tests
            string previousToken = null;

            // Contains the parsed double value of the current string token
            double literal;

            // Opening parenthesis stack to check equal # of pairs seen so far
            Stack<string> parenthesisStack = new Stack<string>();

            // To count the # of opening and closing parentheses.
            int openingCount = 0; 
            int closingCount = 0;

            // Re-initialize the instance variable
            formulaTokensList = new List<string>();  
            variablesSet = new HashSet<string>();

            if (formula == null || normalize == null || validate == null)
            {
                throw new ArgumentNullException("One of the passed parameters "
                    + "references a null object!");
            }

            // Parse every token in the formula while checking for its validity.
            foreach (string currentToken in GetTokens(formula))
            {
                // Check if it's the first token. If so, check if it is of valid 
                // category or not. If so, add it to the formula list. Else throw 
                // an expection.
                if (previousToken == null)
                {
                    if (Double.TryParse(currentToken, out literal))
                    {
                        formulaTokensList.Add(literal.ToString());
                        previousToken = currentToken;
                    }

                    else if (IsValidVariable(currentToken))
                    {
                        // Attempt adding the current token as per specification, 
                        // and if successful, assign the returned 'normalized' 
                        // variable token string to 'previousToken'
                        previousToken = AttemptVarAddition(currentToken, 
                                                            normalize, validate);
                    }
                    else if (currentToken.Equals("("))
                    {
                        parenthesisStack.Push(currentToken); 
                        formulaTokensList.Add(currentToken);
                        previousToken = currentToken;
                        openingCount++;
                    }
                    else
                    {
                        throw new FormulaFormatException("First token must be an "
                                + "opening parenthesis, a variable or an int or "
                                + "double literal.");
                    }
                }

                // Else it is not the first token. Parse if valid, throw exception 
                // otherwise.
                else
                {
                    // The current token is a valid literal?
                    if (Double.TryParse(currentToken, out literal))
                    {
                        if (previousToken.Equals("(") || IsOperator(previousToken))
                        {
                            formulaTokensList.Add(literal.ToString());
                            previousToken = currentToken;
                        }
                        else
                            throw new FormulaFormatException(currentToken + " is not "
                                + "preceded by an operator or an opening parenthesis.");
                    }

                    // The current token is a valid variable?
                    else if (IsValidVariable(currentToken))
                    {
                        if (previousToken.Equals("(") || IsOperator(previousToken))
                        {
                            // Attempt adding the current token as per the specs, 
                            // and if successful, assign the returned normalized 
                            // variable token string to 'previousToken'
                            previousToken = AttemptVarAddition(currentToken, 
                                                                normalize, validate);
                        }
                        else
                        {
                            throw new FormulaFormatException(currentToken + " is not "
                                + "preceded by an operator or an opening parenthesis.");
                        }
                    }

                    // The current token is a valid operator?
                    else if (IsOperator(currentToken))
                    {
                        if (Double.TryParse(previousToken, out literal) || 
                                    IsValidVariable(previousToken) || 
                                        previousToken.Equals(")"))
                        {
                            formulaTokensList.Add(currentToken);
                            previousToken = currentToken;
                        }
                        else
                        {
                            throw new FormulaFormatException(currentToken + " is "
                                    + "not preceded by a literal, variable, or a "
                                    + "closing parenthesis.");
                        }
                    }

                    // The current token is an opening parenthesis?
                    else if (currentToken.Equals("("))
                    {
                        if (previousToken.Equals("(") || IsOperator(previousToken))
                        {
                            parenthesisStack.Push(currentToken);
                            formulaTokensList.Add(currentToken);
                            previousToken = currentToken;
                            openingCount++;
                        }
                        else
                        {
                            throw new FormulaFormatException(currentToken + " is not "
                                + "preceded by an operator or an opening parenthesis.");
                        }
                    }

                    // The current token is a closing parenthesis?
                    else if (currentToken.Equals(")"))
                    {
                        if (Double.TryParse(previousToken, out literal) 
                                || IsValidVariable(previousToken) 
                                    || previousToken.Equals(")"))
                        {
                            // Remove the opening pair of parenthesis used for 
                            // tracking equal pairs.
                            if (parenthesisStack.Count() != 0)
                            {
                                parenthesisStack.Pop();
                            }
                            else
                            {
                                throw new FormulaFormatException("More closing "
                                    + "parenthesis found so far compared to the opening "
                                    + "parenthesis.");
                            }
                            formulaTokensList.Add(currentToken);
                            previousToken = currentToken;
                            closingCount++;
                        }
                        else
                        {
                            throw new FormulaFormatException(currentToken + " is not preceded "
                                    + "by a literal, variable or a closing parenthesis.");
                        }
                    }

                    // If the current token is not a valid token, throw an exception
                    else
                    {
                        throw new FormulaFormatException(currentToken + " is an invalid token.");
                    }
                }
            }

            // If the passed formula had no tokens, throw exception
            if (previousToken == null)
            {
                throw new FormulaFormatException("No valid tokens found. "
                    + "Check the passed formula.");
            }

            // If the last token was of invalid type, throw exception
            if (!(Double.TryParse(previousToken, out literal) 
                            || IsValidVariable(previousToken) 
                                || previousToken.Equals(")")))
            {
                throw new FormulaFormatException("The last token must be a "
                    + "literal, variable or a closing parenthesis.");
            }

            if (openingCount != closingCount)
            {
                throw new FormulaFormatException("Imbalanced parenthesis found. "
                    + "Check and rebalance your formula.");
            }
        }

        /// <summary>
        /// Helper method that takes in a variable token, normalizes it, 
        /// checks the normalized variable against the standard rules of 
        /// Formula syntax format and against the passed Validator, and 
        /// if successful adds the normalized variable token to the 
        /// formulas list. And if it is not successful, throws a 
        /// FormulaFormatException with an explanatory message.</summary>
        /// 
        /// <param name="currentToken">The variable string token to be 
        /// normalized and validated before adding.</param>
        /// 
        /// <param name="normalize">The Normalizer to be used.</param>
        /// 
        /// <param name="validate"> The Validator to be used.</param>
        /// 
        /// <returns>The normalized variable token string.</returns>
        /// 
        private string AttemptVarAddition(string currentToken, 
                       Normalizer normalize, Validator validate)
        {
            string normalizedCurrentToken = normalize(currentToken);
            
            // If the normalized variable token is not valid according to   
            // the standard format rules, throw an exception.
            if (!IsValidVariable(normalizedCurrentToken))
            {
                throw new FormulaFormatException("The normalized variable '"
                    + normalizedCurrentToken + "' does not comply with the standard "
                    + "rules of Formula formatting. Revise your Normalizer()");
            }

            // If the normalized variable token is not valid according to  
            // the passed Validator, throw an exception.
            if (!validate(normalizedCurrentToken))
            {
                throw new FormulaFormatException("The normalized variable '"
                    + normalizedCurrentToken + "' is not valid according to the "
                    + "passed Validator()");
            }

            // The normalized variable token is valid. Add it.
            formulaTokensList.Add(normalizedCurrentToken);
            variablesSet.Add(normalizedCurrentToken);

            return normalizedCurrentToken;
        }

        /// <summary>
        /// Helper method which determines whether the passed string 
        /// token is a valid operator or not. A valid operator is defined 
        /// to be one of these four: + , - , * , / </summary>
        /// 
        /// <param name="token">
        /// The string operator to be checked for validity. </param>
        /// 
        /// <returns>
        /// true, if it is either + or - or * or /. false otherwise.
        /// </returns>
        private bool IsOperator(string token)
        {
            return (token.Equals("+") || token.Equals("-") || token.Equals("*") || 
                      token.Equals("/")) ? true : false;
        }

        /// <summary>
        /// Helper method which determines whether the passed string 
        /// variable has a valid format or not based on the standard 
        /// rules of Format validity. A valid variable name starts 
        /// with a letter followed by zero or more digits / letters.
        /// 
        /// Example: x1, x23, start, etc.</summary>
        /// 
        /// <param name="token"> The string variable to be checked for 
        /// validity. </param>
        /// 
        /// <returns>
        /// true, if it is of the valid form. false otherwise.
        /// </returns>
        private bool IsValidVariable(string token)
        {
            return Regex.IsMatch(token, "^[a-zA-Z][0-9a-zA-Z]*$");
        }

        /// <summary>
        /// <para>Evaluates this Formula, using the Lookup delegate to determine 
        /// the values of variables. (The delegate takes a variable name as a 
        /// parameter and returns its value (if it has one) or throws an 
        /// UndefinedVariableException (otherwise). Uses the standard precedence 
        /// rules when doing the evaluation. </para>
        /// 
        /// <para>If no undefined variables or divisions by zero are encountered 
        /// while evaluating this Formula, its value is returned. If the Formula 
        /// was constructed using the default zero constructor, 0.0 returned. 
        /// Otherwise, throws a FormulaEvaluationException with an explanatory 
        /// message. </para> </summary>
        /// 
        /// <returns>
        /// The computed value of the expression as a double literal.
        /// </returns>
        public double Evaluate(Lookup lookup)
        {
            // If constructed using zero constructor
            if( formulaTokensList == null)
            {
                return 0.0;
            }

            // Contains all the values being processed
            Stack<double> valuesStack = new Stack<double>();
            
            // Contains all the operators being processed
            Stack<string> operatorStack = new Stack<string>();

            // Contains the value of the parsed string int or double literal
            double literal;

            // Parse every token in the formula while performing 
            // necessary computations.
            foreach (string currentToken in formulaTokensList)
            {
                // The current token is a literal?
                if(Double.TryParse(currentToken, out literal))
                {
                    EvaluateLiteral(literal, operatorStack, valuesStack);
                }

                // If the current token is a variable, try to parse its literal 
                // value and evaluate the expression. If unsuccessful, throw an 
                // exception.
                else if ( IsValidVariable(currentToken) )
                {
                    try
                    {
                        literal = lookup(currentToken);
                    }
                    catch(UndefinedVariableException e)
                    {
                        throw new FormulaEvaluationException("\nCannot map a value to the " 
                                + literal + " variable.\n" + e);
                    }
                    EvaluateLiteral(literal, operatorStack, valuesStack);
                }

                // The current token is + or - operator?
                else if (currentToken.Equals("+") || currentToken.Equals("-"))
                {
                    // If there is an operator in the stack which is either + or -
                    // Evaluate the binary expression and update the stacks. And 
                    // in either case, push the operator into the operatorStack
                    if (operatorStack.Count() != 0 && (operatorStack.Peek().Equals("+") 
                                || operatorStack.Peek().Equals("-")))
                    {
                        EvaluateExpressions(valuesStack, operatorStack);
                    }
                    operatorStack.Push(currentToken);
                }

                // The current token is a * or / operator?
                else if (currentToken.Equals("*") || currentToken.Equals("/"))
                {
                    operatorStack.Push(currentToken);
                }

                // The current token is an opening parenthesis?
                else if (currentToken.Equals("("))
                {
                    operatorStack.Push(currentToken);
                }

                // The current token is a closing parenthesis?
                else if (currentToken.Equals(")"))
                {
                    if (operatorStack.Peek().Equals("+") || 
                            operatorStack.Peek().Equals("-"))
                    {
                        EvaluateExpressions(valuesStack, operatorStack); 
                    }
                    // Remove the opening parenthesis "(" from the stack 
                    operatorStack.Pop();

                    // If there is an operator in the stack which is either * or /
                    // Evaluate the binary expression and update the stacks.
                    if (operatorStack.Count() != 0 && (operatorStack.Peek().Equals("*") 
                            || operatorStack.Peek().Equals("/")) )
                    {
                        EvaluateExpressions(valuesStack, operatorStack);
                    }
                }
            }

            // If operator stack is not empty, there are exactly 2 values and 1 
            // operator left. Evaluate the binary expression and add the final 
            // anwser to the valuesStack.
            if (operatorStack.Count() != 0)    
            {
                EvaluateExpressions(valuesStack, operatorStack);
            }
            return valuesStack.Pop();                 // Return the final answer
        }

        /// <summary>
        /// Helper method that checks if there is an operator in the stack 
        /// which is either * or /. If there is, then temporarily pushes the 
        /// passed literal into the valuesStack for forthcoming computation, 
        /// evaluates the binary expression and updates the stacks.
        /// </summary>
        /// 
        /// <param name="literal">The literal value to be processed.</param>
        /// <param name="operatorStack">Reference to the stack of operators.</param>
        /// <param name="valuesStack">Reference to the stack of values.</param>
        private void EvaluateLiteral(double literal, Stack<string> operatorStack, Stack<double> valuesStack)
        {
            if (operatorStack.Count() != 0 && (operatorStack.Peek().Equals("*")
                    || operatorStack.Peek().Equals("/")))
            {
                valuesStack.Push(literal);
                EvaluateExpressions(valuesStack, operatorStack);
            }
            else
            {
                valuesStack.Push(literal);
            }
        }

        /// <summary>
        /// <para>Helper method which evalutes the top two values in the valuesStack 
        /// using the operator on top of the operatorStack.</para>
        /// 
        /// <para>It updates both the stacks according to the specifications of infix 
        /// evaluation.</para>
        /// 
        /// <para>If the denominator is zero when dividing, throws a 
        /// FormulaEvaluateException.</para></summary>
        /// 
        /// <param name="valuesStack">Stack containing all the values being processed
        /// </param>
        /// 
        /// <param name="operatorStack">Stack containing all the operators being 
        /// processed</param>
        /// 
        private void EvaluateExpressions(Stack<double> valuesStack, Stack<string> operatorStack)
        {
            double rhs = valuesStack.Pop();              // Contains the right side of the expression
            double lhs = valuesStack.Pop();              // Contains the left side of the expression
            string operatorToken = operatorStack.Pop();  // Contains the operator to be used

            if (operatorToken.Equals("*"))
            {
                valuesStack.Push(lhs * rhs);
            }

            else if (operatorToken.Equals("/"))
            {
                if (rhs == 0)
                {
                    throw new FormulaEvaluationException("Division by zero is illegal");
                }
                valuesStack.Push(lhs / rhs);
            }

            else if (operatorToken.Equals("+"))
            {
                valuesStack.Push(lhs + rhs);
            }

            else if (operatorToken.Equals("-"))
            {
                valuesStack.Push(lhs - rhs);
            }
        }

        /// <summary>
        /// Returns an ISet of string that contains each distinct 
        /// variable (in normalized form) that appears in this 
        /// Formula. If the zero constructor was used, returns an 
        /// empty ISet.
        /// </summary>
        public ISet<string> GetVariables()
        {
            return formulaTokensList == null ? 
                      new HashSet<string>() : 
                        new HashSet<string>(variablesSet);
        }

        /// <summary>
        /// Returns a string version of this Formula containing 
        /// the normalized variables. This method can be used as 
        /// an argument to generates other Formula objects that 
        /// are copies of this.
        /// </summary>
        /// 
        /// <returns>A string version of this Formula. If 
        /// constructed using zero argument constructor, 
        /// returns "0".</returns>
        public override string ToString()
        {
            // If constructed using zero constructor
            if (formulaTokensList == null)
            {
                return "0";
            }
            
            StringBuilder formula = new StringBuilder();
            int size = formulaTokensList.Count;
            
            if ( size != 0 )
            {
                for (int i = 0; i < size; i++)
                {
                    formula.Append(formulaTokensList[i]);
                    
                    // If next token isn't the last, add space
                    if( (i+1) < size)
                    {
                        formula.Append(" ");
                    }
                }
            }
            
            return formula.ToString();
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it. 
        /// Tokens are left paren, right paren, one of the four operator 
        /// symbols, a string consisting of a letter followed by zero or 
        /// more digits and/or letters, a double literal, and anything that 
        /// doesn't match one of those patterns.  There are no empty tokens, 
        /// and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                   lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values. 
    /// Given a string, such a function can either return a double (meaning 
    /// that the string maps to the double) or throw an UndefinedVariableException 
    /// (meaning that the string is unmapped to a value. Exactly how a Lookup 
    /// method decides which strings map to doubles and which don't is up to the 
    /// implementation of the method.
    /// </summary>
    public delegate double Lookup(string s);

    /// <summary>
    /// A Normalizer converts variables into a canonical form. For example, 
    /// a Normalizer can be made to change the case of every variable in the 
    /// passed formula string. Or it can be made to append digits to similar 
    /// variables, etc.
    /// 
    /// <para>If a formula that was valid before becomes invalid after applying 
    /// the a Normalizer N(x) to it (according to the standard Formula rules),  
    /// it throws a FormulaFormatException with an explanatory message.</para>
    /// </summary>
    /// 
    /// <param name="s">The variable token string to be normalized.</param>
    /// 
    /// <returns>A normalized version of the passed string.</returns>
    public delegate string Normalizer(string s);

    /// <summary>
    /// A Validator imposes extra restrictions over the standard rules 
    /// of formula validation. For example, a new Formula can be made to adhere 
    /// to more stringent restrictions in addition to the standard restrictions 
    /// like variables can only be 1 letter and 1 digit, etc.
    /// 
    /// <para>Even if the normalized version of variable string N(x) results in a 
    /// valid Formula format but the restrictions imposed by the Validator on the 
    /// normalized formula V(N(x)) makes it invalid, then a FormulaFormatException 
    /// is thrown with an explanatory message.</para></summary>
    /// 
    /// <param name="s">The variable token string to be normalized.</param>
    /// 
    /// <returns>true if the passed string meets the constraints. False otherwise.
    /// </returns>
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message 
        /// is the undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
