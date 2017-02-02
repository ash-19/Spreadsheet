// Implemented by Snehashish Mishra for CS 3500, February 2016.
// uID: u0946268

// Extended on February 24, 2016 by SM.

using System;
using System.Collections.Generic;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet. 
    /// A spreadsheet consists of a regular expression (called IsValid below) and 
    /// an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if (1) s consists of one or more 
    /// letters, followed by a non-zero digit, followed by zero or more digits AND 
    /// (2) the C# expression IsValid.IsMatch(s.ToUpper()) is true.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names, so long 
    /// as they also are accepted by IsValid.  On the other hand, "Z", "X07", and 
    /// "hello" are not valid cell names, regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in 
    /// a formula, must be normalized by converting all letters to upper case before 
    /// it is used by this spreadsheet.  For example, the Formula "x3+a5" should be 
    /// normalize to "X3+A5" before use.  Similarly, all cell names and Formulas that 
    /// are returned or written to a file must also be normalized.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction 
    /// is important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  
    /// If the contents is an empty string, we say that the cell is empty.  (By analogy, 
    /// the contents of a cell in Excel is what is displayed on the editing line when 
    /// the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's 
    /// position in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  
    /// The value of a Formula variable is the value of the spreadsheet cell it names 
    /// (if that cell's value is a double) or is undefined (otherwise).  If a Formula 
    /// depends on an undefined variable or on a division by zero, its value is a 
    /// FormulaError.  Otherwise, its value is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that 
    /// establish a circular dependency.  A circular dependency exists when a cell 
    /// depends on itself. For example, suppose that A1 contains B1*2, B1 contains 
    /// C1*2, and C1 contains A1*2. A1 depends on B1, which depends on C1, which 
    /// depends on A1.  That's a circular dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Contains all the dependencies in this spreadsheet. 
        /// Circular dependencies are considered illegal.
        /// </summary>
        private DependencyGraph dGraph;

        /// <summary>
        /// Maps the cell names to its contents. As such, keys  
        /// are the string type cell names that maps the corresponding 
        /// Cell objects containing the contents and values of that Cell.
        /// </summary>
        private Dictionary<string, Cell> sheet;
        
        /// <summary>
        /// Contains the regular expression passed by the constructor which 
        /// checks the validity of cell names.
        /// 
        /// For an empty constuctor, this regular expression accepts every 
        /// string.
        /// </summary>
        private Regex isValid;

        /// <summary>
        /// Regex to check the valid format of a cell name.
        /// </summary>
        private const string CELL_NAME_REGEX = "^[a-zA-Z]*[1-9][0-9]*$";
        
        /// <summary>
        /// True if this spreadsheet has been modified since it was 
        /// created or saved (whichever happened most recently); 
        /// false otherwise.
        /// </summary>
        public override bool Changed {  get;  protected set; }

        /// <summary>
        /// Creates an empty spreadsheet. The contents of every cell in 
        /// an empty spreadsheet is an empty string. The IsValid regex 
        /// accepts every string.
        /// </summary>
        public Spreadsheet() : this(new Regex(@".*\s*"))
        {
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular 
        /// expression is provided as the parameter.
        /// </summary>
        /// 
        /// <param name="isValid">Regex to check cell name 
        /// validity.</param>
        public Spreadsheet(Regex isValid)
        {
            dGraph = new DependencyGraph();
            sheet = new Dictionary<string, Cell>();
            Changed = false;
            this.isValid = isValid;
        }

        /// <summary>
        /// Creates a Spreadsheet that is a duplicate of the spreadsheet 
        /// saved in source. See the AbstractSpreadsheet.Save method and 
        /// Spreadsheet.xsd for the file format specification.  
        /// 
        /// If there's a problem reading source, throws an IOException
        /// 
        /// If the contents of source are not consistent with the schema 
        /// in Spreadsheet.xsd, throws a SpreadsheetReadException.  
        /// 
        /// If there is an invalid cell name, or a duplicate cell name, 
        /// or an invalid formula in the source, throws a SpreadsheetReadException.
        /// 
        /// If there's a Formula that causes a circular dependency, 
        /// throws a SpreadsheetReadException.
        /// </summary>
        /// <param name="source">The source file to read from</param>
        public Spreadsheet(TextReader source) : this()
        {
            if(source == null)
            {
                throw new IOException("Source file points to null");
            }

            // Setup XmlReader settings to use the Schema
            XmlSchemaSet sc = new XmlSchemaSet();
            sc.Add(null, "Spreadsheet.xsd");

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.CloseInput = true;
            settings.ValidationEventHandler += ValidationCallBack;

            // Read and translate the passed XML file to the internal sheet DS
            using (XmlReader reader = XmlReader.Create(source, settings))
            {
                try
                {
                    while (reader.Read())
                    {
                        string name = "";
                        string contents = "";
                        bool setContents = false;

                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    isValid = new Regex(reader["IsValid"]);
                                    break;

                                case "cell":
                                    name = reader["name"];
                                    contents = reader["contents"];
                                    setContents = true;
                                    break;
                            }
                        }

                        // Try adding the cell. Else throw the exceptions
                        if (setContents)
                        {
                            try
                            {
                                if (sheet.ContainsKey(name))
                                {
                                    throw new SpreadsheetReadException("Duplicate Cell name");
                                }
                                SetContentsOfCell(name, contents);
                            }
                            catch (FormulaFormatException)
                            {
                                throw new SpreadsheetReadException("Invalid Formula");
                            }
                            catch (CircularException)
                            {
                                throw new SpreadsheetReadException("Circular dependencies "
                                    + "detected!");
                            }
                            catch (InvalidNameException)
                            {
                                throw new SpreadsheetReadException("Invalid Cell name");
                            }
                        }
                    }
                }
                catch(XmlException)
                {
                    throw new IOException("An error occurred while parsing EntityName");
                }
            }
            Changed = false;
        }

        /// <summary>
        /// Throws a Validation error message if the XML file being 
        /// read did not satisfy the specified Schema.
        /// </summary>
        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            throw new SpreadsheetReadException("Validation Error: " + e.Message);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException. Otherwise, 
        /// returns the contents (as opposed to the value) of the named cell. The 
        /// return value should be either a string, a double, or a Formula.
        /// </summary>
        /// 
        /// <param name="name">Cell name</param>
        /// 
        /// <returns>
        /// the contents (as opposed to the value) of the named cell
        /// </returns>
        public override object GetCellContents(string name)
        {
            if (IsNameInvalid(name))
            {
                throw new InvalidNameException();
            }
            name = name.ToUpper();

            // If the Cell associated with the passed name does not exist, return 
            // an empty string. Else return the contents the of that Cell object.
            return !sheet.ContainsKey(name) ? string.Empty : sheet[name].Contents;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (KeyValuePair<string, Cell> currentCell in sheet)
            {
                if (!(currentCell.Value.Contents.Equals("")))
                {
                    yield return currentCell.Key;
                }
            }
        }

        /// <summary>
        /// If formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula 
        /// would cause a circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method 
        /// returns a Set consisting of name plus the names of all other cells whose 
        /// value depends, directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// 
        /// <param name="name">Cell name</param>
        /// <param name="formula">Formula object to be stored as cell's content</param>
        /// 
        /// <returns>
        /// A Set consisting of name plus the names of all other cells whose 
        /// value depends, directly or indirectly, on the named cell.
        /// </returns>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            // ArgumentNullException handled by SetCellContents(string, string) if 
            // passed argument itself was null. Else handled by the Formula constructor 
            // if the parameter passed to it was null: new Formula(null).

            if (IsNameInvalid(name))  {  throw new InvalidNameException();  }

            // Convert all cell names to upper case
            name = name.ToUpper();
            formula = new Formula(formula.ToString(), s => s.ToUpper(), s => !IsNameInvalid(s));

            // Create a copy of old dependees and update the cell's dependees
            DependencyGraph oldDependees = new DependencyGraph(dGraph);
            dGraph.ReplaceDependees(name, formula.GetVariables());

            // If no CircularException is thrown, add the formula to the cell. Else,
            // keep the contents of the cell unchanged and throw a CircularException.
            try
            {
                HashSet<string> dependents = new HashSet<string>(GetCellsToRecalculate(name));
                if (sheet.ContainsKey(name))
                {
                    sheet[name].Contents = formula;
                }
                else
                {
                    sheet.Add(name, new Cell(formula, LookUpCellValue));
                }
                Changed = true;
                return dependents;
            }
            catch (CircularException)
            {
                dGraph= new DependencyGraph(oldDependees);
                Changed = false;
                throw new CircularException();
            }
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method 
        /// returns a set consisting of name plus the names of all other cells whose 
        /// value depends, directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned. </summary>
        /// 
        /// <param name="name">Cell name</param>
        /// <param name="text">Cell's content - a string</param>
        /// 
        /// <returns>
        /// A Set consisting of name plus the names of all other cells whose value 
        /// depends, directly or indirectly, on the named cell.
        /// </returns>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if ( text == null )         {  throw new ArgumentNullException();  }
            if ( IsNameInvalid(name) )  {  throw new InvalidNameException();   }
            
            name = name.ToUpper();

            // Add the contents to cell
            if (sheet.ContainsKey(name))
            {
                sheet[name].Contents = text;
            }
            else
            {
                sheet.Add(name, new Cell(text));
            }

            // If empty cell, remove it from the sheet
            if (sheet[name].Contents.Equals(""))
            {
                sheet.Remove(name);
            }
            Changed = true;

            // Reset the dependees of passed cell name, add all the dependents of the 
            // cell name to a new set and return it.
            dGraph.ReplaceDependees(name, new HashSet<string>());
            HashSet<string> dependents = new HashSet<string>(GetCellsToRecalculate(name)); 
            return dependents;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method 
        /// returns a set consisting of name plus the names of all other cells whose 
        /// value depends, directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned. </summary>
        /// 
        /// <param name="name">Cell name</param>
        /// <param name="number">Contents of the cell - a double literal</param>
        /// 
        /// <returns>
        /// A Set consisting of name plus the names of all other cells whose 
        /// value depends, directly or indirectly, on the named cell.
        /// </returns>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (IsNameInvalid(name))  {  throw new InvalidNameException();  }

            name = name.ToUpper();

            if (sheet.ContainsKey(name))
            {
                sheet[name].Contents = number;
            }
            else
            {
                sheet.Add(name, new Cell(number));
            }
            Changed = true;

            // Reset the dependees of passed cell name, add all the dependents of the 
            // cell name to a new set and return it.
            dGraph.ReplaceDependees(name, new HashSet<string>());
            HashSet<string> dependents = new HashSet<string>(GetCellsToRecalculate(name));
            return dependents;
        }

        /// <summary>
        /// Checks the validity of the passed name. A string is a invalid 
        /// cell name if and only if (1) it violates the regex OR (2) 
        /// the C# expression IsValid.IsMatch(s.ToUpper()) is false.
        /// </summary>
        /// 
        /// <param name="name">string to be checked for validity.</param>
        /// 
        /// <returns>true if the string references to null or is of 
        /// invalid format. false otherwise.
        /// </returns>
        private bool IsNameInvalid(string name)
        {
            return name == null || !Regex.IsMatch(name, CELL_NAME_REGEX) 
                || !isValid.IsMatch(name);
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an 
        /// InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names 
        /// of all cells whose values depend directly on the value of the named 
        /// cell. In other words, returns an enumeration, without duplicates, 
        /// of the names of all cells that contain formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1 </summary>
        /// 
        /// <param name="name">Cell name</param>
        /// 
        /// <returns>
        /// An enumeration, without duplicates, of the names of all cells whose 
        /// values depend directly on the value of the named cell.
        /// </returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("Passed name references to null");
            }

            if (!Regex.IsMatch(name, CELL_NAME_REGEX) || !(isValid.IsMatch(name)) )
            {
                throw new InvalidNameException();
            }

            return dGraph.GetDependents(name);
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the isvalid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) 
        /// should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written 
        /// as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        /// 
        /// <param name="dest">The destination where the contents are to be written.</param>
        public override void Save(TextWriter dest)
        {
            if (dest == null)
            {
                throw new IOException("The destination points to null");
            }

            // Setup XmlWriter setting (indent, close O/P, etc.)
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;

            // Save the spreadsheet state to the dest XML
            using (XmlWriter writer = XmlWriter.Create(dest, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("IsValid", null, isValid.ToString());

                try
                {
                    foreach (string cellName in sheet.Keys)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteAttributeString("name", cellName);

                        object cellContents = sheet[cellName].Contents;
                        string strContents;

                        // Check the content type and then add it accordingly
                        if (cellContents is double)
                        {
                            strContents = cellContents.ToString();
                        }
                        else if (cellContents is Formulas.Formula)
                        {
                            strContents = "=" + cellContents.ToString();
                        }
                        else
                        {
                            strContents = cellContents.ToString();
                        }

                        writer.WriteAttributeString("contents", strContents);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                catch (InvalidOperationException)
                {
                    throw new IOException("The dest Writer is closed");
                }
                catch (ArgumentException)
                {
                    throw new IOException("An error occurced while parsing the EntityName");
                }
            }
            Changed = false;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the 
        /// named cell. The return value should be either a string, a double,
        /// or a FormulaError. </summary>
        /// 
        /// <param name="name">Name of the cell.</param>
        /// 
        /// <returns>
        /// The value of the named cell. An empty string if the cell is empty.
        /// </returns>
        public override object GetCellValue(string name)
        {
            if( IsNameInvalid(name) ) {  throw new InvalidNameException();  }

            name = name.ToUpper();

            return sheet.ContainsKey(name) ? sheet[name].Value : "";
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned. </summary>
        /// 
        /// <param name="name">The name of the cell.</param>
        /// <param name="content">The content to be stored in the cell.</param>
        /// 
        /// <returns>
        /// A set consisting of name plus the names of all other cells whose 
        /// value depends, directly or indirectly, on the named cell.
        /// </returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null) { throw new ArgumentNullException(); }
            if (IsNameInvalid(name)) { throw new InvalidNameException(); }

            name = name.ToUpper();
            double doubleValue;

            // Set the cell based on the content type...
            if (content.Equals(""))
            {
                SetCellContents(name, content);
            }

            else if (Double.TryParse(content, out doubleValue))
            {
                //dependents = new HashSet<string>(SetCellContents(name, doubleValue));
                SetCellContents(name, doubleValue);
                sheet[name].Value = doubleValue;
            }

            // If content is Formula, throw FormulaFormatException() if it
            // cannot be parsed. If changing the contents of cell causes 
            // CircularDependency, throw CircularException (sheet state unchanged). 
            // Else update the contents of the cell.
            else if (content.Substring(0, 1).Equals("="))
            {
                content = content.Substring(1);
                Formula f = new Formula(content, s => s.ToUpper(), s => !IsNameInvalid(s));
                SetCellContents(name, f);
                sheet[name].Value = content;
            }
            else
            {
                SetCellContents(name, content);
                sheet[name].Value = content;
            }

            //Unset, if the Circular Exception was thrown. Else sheet changed.
            Changed = true;

            // Re-evaluate all the cells dependent on the current modified cell, 
            // if they contain Formula.
            foreach (string cellName in GetCellsToRecalculate(name))
            {
                Cell currentCell;
                if (sheet.TryGetValue(cellName, out currentCell))
                {
                    // Re-evaluate the cell if it contains a Formula using 
                    // a Lookup delegate that returns the double value of 
                    // each cell name variable in the formula or throws an expcetion.
                    currentCell.ReEvaluateCellValue(LookUpCellValue);
                }
            }
            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// A Lookup delegate that looks up the value of the passed cell name. 
        /// If the value associated with the cell is not a double, throws an 
        /// UndefinedVariableException.
        /// </summary>
        /// <param name="cellName">The name of the cell</param>
        /// <returns>The double value of the cell if it exists. Else, throws 
        /// an UndefinedVariableException.</returns>
        private double LookUpCellValue(string cellName)
        {
            Cell currentCell;

            if (sheet.TryGetValue(cellName, out currentCell))
            {
                if (currentCell.Value is double)
                {
                    return (double)currentCell.Value;
                }
                throw new UndefinedVariableException("Value of cell: '" + cellName +
                     "' is not mapped to a double");
            }
            throw new UndefinedVariableException("Value of cell: '" + cellName +
                     "' is not mapped to a double");
        }

        /// <summary>
        /// A Cell object represents an individual cell in a spreadsheet. 
        /// Every Cell object has some content and value in it. The 
        /// content can be string, double, Formula. The value can be 
        /// string, double, FormulaError object.
        /// </summary>
        private class Cell
        {
            /// <summary>
            /// The contents of this Cell object. Any type of object 
            /// can be passed as contents, but the spreadsheet object 
            /// will check for the validity of the Contents as per its 
            /// specification before using it.
            /// As such, valid contents can be a string, double or a 
            /// Formula object.
            /// </summary>
            public object Contents { get; set; }

            /// <summary>
            /// The value of this Cell object. The value of a cell can be 
            /// (1) a string, (2) a double, or (3) a FormulaError.  
            /// 
            /// If a cell's contents is a string, its value is that string.
            /// 
            /// If a cell's contents is a double, its value is that double.
            /// 
            /// If a cell's contents is a Formula, its value is either a double 
            /// or a FormulaError. The value of a Formula can depend on the 
            /// values of variables.  The value of a Formula variable is the 
            /// value of the spreadsheet cell it names (if that cell's 
            /// value is a double) or is undefined (otherwise).  If a Formula 
            /// depends on an undefined variable or on a division by zero, 
            /// its value is a FormulaError.  Otherwise, its value is a double, as 
            /// specified in Formula.Evaluate.
            /// </summary>
            public object Value  {  get; set;  }

            /// <summary>
            /// Creates a Cell object from passed 'string' contents.
            /// </summary>
            /// <param name="stringContents">The string contents of this 
            /// cell</param>
            public Cell(string stringContents)
            {
                this.Contents = stringContents;
                this.Value = stringContents;
            }

            /// <summary>
            /// Creates a Cell object from passed double literal.
            /// </summary>
            /// <param name="literalContents">The literal contents of this
            /// cell</param>
            public Cell(double literalContents)
            {
                this.Contents = literalContents;
                this.Value = literalContents;
            }

            /// <summary>
            /// Creates a Cell object from passed formula. The value of the 
            /// cell becomes the value of the parsed Formula. If the formula 
            /// can't be parsed (division by zero or undefined variable), 
            /// the value of this cell is a FormulaError object.
            /// </summary>
            /// 
            /// <param name="formulaContents">The formula contents of this 
            /// cell</param>
            /// 
            /// <param name="lookup">The Lookup delegate that associates this 
            /// Cell object with a value.</param>
            public Cell(Formula formulaContents, Lookup lookup)
            {
                this.Contents = formulaContents;

                // If the value of the formula can't be parsed, the value 
                // of this cell becomes a FormulaError object. Else the double.
                try
                {
                    this.Value = formulaContents.Evaluate(lookup);
                }
                catch(FormulaEvaluationException)
                {
                    this.Value = new FormulaError("The value of this Cell's contents "
                        + "cannot be parsed.");
                }
            }

            /// <summary>
            /// Re-evaluates the value of a single Cell if it contains a Formula. 
            /// If the formula evalutes to a literal, the value of this Cell object 
            /// is set to that double literal. Else, if the evaluation of the Formula 
            /// is unsuccessful (division by zero or undefined variables), sets the 
            /// value of this Cell object to a FormulaError object.
            /// 
            /// <note>Must be used in a loop to re-evaluate the values of multiple Cell
            /// objects.</note></summary>
            /// 
            /// <param name="lookup">Lookup delegate to find the values of the cells 
            /// in the passed formula.</param>
            public void ReEvaluateCellValue(Lookup lookup)
            {
                if(this.Contents is Formulas.Formula )
                {
                    Formula f = (Formula)Contents;
                    try
                    {
                        this.Value = f.Evaluate(lookup);
                    }
                    catch (FormulaEvaluationException)
                    {
                        this.Value = new FormulaError("The value of this Cell's contents "
                            + "cannot be evaluated.");
                    }
                }
            }

        } // End of cell class
        
    } // End of Spreadsheet class

} // End of namespace
