// Developed by Snehashish Mishra, u0946268 on 8th March for
// CS 3500 offered by The University of Utah

using Formulas;
using SS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Provides the Controller for the Spreadsheet application. 
    /// The model and view do not know about the state in the 
    /// Controller. The Controller accesses the SpreadsheetWindow 
    /// (View) via an interface (ISpreadsheetView).
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// Contains the window being controlled, accessing view via 
        /// the interface
        /// </summary>
        private ISpreadsheetView window;

        /// <summary>
        /// Contains the filename of the currently active Spreadsheet file
        /// </summary>
        private string filename;

        /// <summary>
        /// Contains the backing spreadsheet data structure (model)
        /// </summary>
        private Spreadsheet spreadsheet;

        /// <summary>
        /// Tracks whether the active spreadsheet window was existing from the 
        /// File menu or the X button on top.
        /// </summary>
        private bool closedFromMenu;

        /// <summary>
        /// Tracks if the user cancelled the save prompt
        /// </summary>
        private bool saveCancelled;

        /// <summary>
        /// Begins controlling the active window. Valid cell names are 
        /// in the range [A1-Z99].
        /// </summary>
        public Controller(ISpreadsheetView window)
        {
            // Initialize instance variables.
            this.window = window;
            this.spreadsheet = new Spreadsheet(new Regex(@"^[a-zA-Z]{1}[1-9]{1}\d?$"));
            closedFromMenu = false;
            saveCancelled = false;
            
            // Setup the window
            window.Title = "Unsaved Spreadsheet";
            UpdateSpreadsheetWindow("A1");

            // Register the event handlers
            window.FileChosenEvent += HandleFileChosen;
            window.CloseEvent += HandleClose;
            window.NewEvent += HandleNew;
            window.HowToEvent += HandleHowTo;
            window.KeyPressed += HandleKeyPress;
            window.EvaluateClicked += HandleEvaluate;
            window.SaveAsEvent += HandleSaveAs;
            window.SaveEvent += HandleSave;
            window.ClosingForm += HandleXClose;
            window.SelectionChanged += HandleSelection;
        }

        /// <summary>
        /// This method handles the case where the user decides to close the 
        /// currently active spreadsheet by clicking the X icon on top-right.
        /// When prompted to save the sheet (if it is unsaved), if the user clicks 
        /// cancel, exists. If the user clicks Yes, saves the file. And if the user 
        /// clicks Cancel, the program remains open.
        /// </summary>
        /// <param name="e">Provides the Form closing event arguments</param>
        private void HandleXClose(FormClosingEventArgs e)
        {
            if (spreadsheet.Changed && !closedFromMenu)
            {
                DialogResult result = window.Message("There are some unsaved changes."
                    + " Would you like to save your changes before closing?", 
                    "Save Before Exiting?", MessageBoxButtons.YesNoCancel, 
                                                MessageBoxIcon.Warning);

                // if seleced 'Yes', then save the file. If the change is still true 
                // (i.e, user selected cancel in the Save dialog), don't close the sheet.
                if (result == DialogResult.Yes)
                {
                    window.PerformSaveClick(); 
                    if (spreadsheet.Changed)
                       e.Cancel = true;
                }
                // else if selected cancel, don't close the window. Else, let the sheet 
                // window close.
                else if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// This method handles the event where the user clicks the Save option 
        /// from the File menu. Is invoked after checking if the file was never 
        /// saved before and presenting a Save as... dialog box to the user.
        /// </summary>
        /// <param name="fileName">The name of the file of the saved</param>
        private void HandleSave(string fileName)
        {
            // If the file is still not saved, return (since Save As dialog was cancelled).
            if (fileName.Equals("Unsaved Spreadsheet"))
            {
                saveCancelled = true;
                return;
            }
    
            // If the spreadsheet has been changed then save it
            if (spreadsheet.Changed)
                spreadsheet.Save(File.CreateText(fileName));

            // If spreadsheet now becomes unchanged, show Save Successful dialog.
            if (!spreadsheet.Changed)
                window.Message("Spreadsheet Saved.", "Save complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// This method handles the event where the user clicks the Save As... 
        /// option from the File menu. It automatically adds the .ss extension 
        /// to the user entered file name.
        /// </summary>
        /// <param name="saveFileDialog1">The save file dialog being processed</param>
        private void HandleSaveAs(SaveFileDialog saveFileDialog1)
        {
            string saveFileName = saveFileDialog1.FileName;

            // if the file name is empty, return.
            if (saveFileName.Equals(""))   {  return;  }

            // if only .ss files are being showed and the user did not add the .ss
            // extension, append it
            if (saveFileDialog1.FilterIndex == 1)
                saveFileDialog1.AddExtension = true;

            spreadsheet.Save(File.CreateText(saveFileName));
            window.Title = saveFileName;
            filename = saveFileName;
        }

        /// <summary>
        /// This method handles the return key press. If pressed, it evalutes the 
        /// spreadsheet.
        /// </summary>
        /// <param name="key">The key pressed</param>
        /// <param name="cellname">The name of the currently active cell</param>
        /// <param name="contents">The contents of the currently active cell</param>
        private void HandleKeyPress(char key, string cellname, string contents)
        {
            if(key.Equals('\r'))
            {
                HandleEvaluate(cellname, contents);
            }
        }

        /// <summary>
        /// This method handles the evalution (or re-evaluation) of the entire 
        /// spreadsheet when the contents of a cell is changed. If the user inputs 
        /// invalid Formula or one that leads to a Circular Dependecy, a message box 
        /// warns the user about it (the spreadsheet remains unchanged in this case).
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="contents"></param>
        private void HandleEvaluate(string cellName, string contents)
        {
            // to hold the dependents of the selected cell
            ISet<String> dependents = null;

            // Try adding the modifying the cell in the spreadsheet and retrieve 
            // all its dependents. Handle exceptions accordingly.
            try
            {
                dependents = spreadsheet.SetContentsOfCell(cellName, contents);
            }
            catch (FormulaFormatException)
            {
                window.Message("Invalid formula detected! Your changes will not"
                        + " be saved.", "Invlaid Formula!", MessageBoxButtons.OK, 
                               MessageBoxIcon.Error);
            }
            catch (CircularException)
            {
                window.Message("This Formula will lead to a circular dependency. "
                    + "All changes will be reverted.", "Circular Dependency Detected!", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateSpreadsheetWindow(cellName);

            // Exception occurced. No modifications are made.
            if (dependents == null)
                return;

            // Refresh all the dependent cells.
            foreach (string cell in dependents)
                UpdateCell(cell);
        }

        /// <summary>
        /// Update the text inside the cell in the spreadsheet to 
        /// reflect the evaluated value.
        /// </summary>
        /// <param name="cellName">The name of the cell being refreshed.</param>
        private void UpdateCell(string cellName)
        {
            int colIndex;
            int rowIndex;
            string cellValue;

            // Helper methods to decode the index of the cell's row and column
            colIndex = GetColumnIndex(cellName);
            rowIndex = GetRowIndex(cellName);

            // Refresh the value of the cell (a FormulaError or a value)
            if ((spreadsheet.GetCellValue(cellName) is FormulaError))
            {
                FormulaError error = (FormulaError)spreadsheet.GetCellValue(cellName);
                cellValue = error.Reason;
            }
            else
            {
                cellValue = spreadsheet.GetCellValue(cellName).ToString();
            }
            window.SetCellValue(rowIndex, colIndex, cellValue);
        }

        /// <summary>
        /// Helper method to retrieve the row index from the passed cell name.
        /// This is accomplished by parsing the numerical part of the cell name 
        /// to obtain the row number and coverting it to a numerical index.
        /// </summary>
        /// <param name="cellName">The name of the cell</param>
        /// <returns>The row index of the passed cell name</returns>
        private int GetRowIndex(string cellName)
        {
            int row;
                     
            int.TryParse(cellName.Substring(1), out row);
            row--;

            return row;
        }

        /// <summary>
        /// Helper method to retrieve the column index from the passed cell name.
        /// This is accomplished by parsing the alphabetical part of the cell name 
        /// and coverting it to a numerical index.
        /// </summary>
        /// <param name="cellName">The name of the cell</param>
        /// <returns>The column index of the passed cell name</returns>
        private int GetColumnIndex(string cellName)
        {
            return cellName.ToCharArray(0, 1)[0] - 'A';
        }

        /// <summary>
        /// This method handles the event when a user clicked the How to menu item 
        /// from the Help menu. It display a message box on how to use this app.
        /// </summary>
        private void HandleHowTo()
        {
            window.Message("This spreadsheet app allows users to create spreasheets "
                +"by entering data of the type double, string or Formula. The range of "
                +"the spreadsheet cells is [A1-Z99]. Anything referenced outside of that "
                +"range is invalid. The file extension used is (.ss). \n\nTips on how to "
                +"use this app:\n"
                +"1. You can close the currently active window by clicking the X button "
                +"above or by navigating to File->Close. Unsaved spreadsheets will be "
                +"prompted to be saved before closing. \n" 
                +"2. There are two ways of saving a spreadsheet file. Save and Save As... "
                +"work as expected, with Save prompting with a dialog box if the sheet was new."
                +"\n3. File->New will open a blank spreadsheet in a new window, while File->Open will "
                +"open an existing file. ", "How to use this app?", MessageBoxButtons.OK, 
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// This method handles the event of user clicking on a cell. It updates the 
        /// cell name, value, contents text boxes according to the data contianed in the 
        /// clicked cell.
        /// </summary>
        /// <param name="col">The column number</param>
        /// <param name="row">The row number</param>
        private void HandleSelection(int col, int row)
        {
            UpdateSpreadsheetWindow(GetCellName(row, col));
        }

        /// <summary>
        /// This method handles the request of opening a file. It re-draws the current 
        /// active window with the contents of the file to be opened. If the current 
        /// sheet has any unsaved changes, warns the user and asks what to do, before 
        /// continuing.
        /// </summary>
        private void HandleFileChosen(String openFileName)
        {
            if (openFileName.Equals(""))
                return;

            // Currently active sheet has some changes. Ask user to save it or cancel.
            if (spreadsheet.Changed)
            {
                DialogResult result = window.Message("Some changes have not been saved. "
                    + "Opening this file will result in loss of those changes. Open anyways?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                // if 'No', cancel the open file operation
                if (result == DialogResult.No)
                    return;
            }

            // Try to load the sheet from the selected file. Display an error message 
            // if unable to open the file. 
            try
            {
                // clear each cell value in the spreadsheet
                foreach (string s in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    int colIndex = GetColumnIndex(s);
                    int rowIndex = GetRowIndex(s);
                    window.SetCellValue(rowIndex, colIndex, "");
                    UpdateSpreadsheetWindow(s);
                }

                // Update the spreadsheet with new file data
                spreadsheet = new Spreadsheet(File.OpenText(openFileName));
                filename = openFileName;
                window.Title = openFileName;                

                // Update the relevant cells
                foreach (string s in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    UpdateCell(s);
                    UpdateSpreadsheetWindow(s);
                }
            }
            catch (SpreadsheetReadException e)
            {
                window.Message("Problem reading the source file. \n" 
                    + e.Message, "Error reading Spreadsheet", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException e)
            {
                window.Message("Problem reading the source file. \n"
                    + e.Message, "Error reading Spreadsheet",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method handles the request to close the currently active window, 
        /// when the user selects the Close menu item in the File menu. It prompts 
        /// the user to save the file (if made any unsaved changes) before closing it. 
        /// </summary>
        private void HandleClose()
        {
            if (!spreadsheet.Changed)
            {
                window.DoClose();
            }
            else
            {
                DialogResult result = window.Message("Do you wish to save your" 
                    + " changes before closing this file?", "Save Before Exiting?", 
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                // if 'Yes' then save the file. If the save was cancelled, then dont close the 
                // window.
                if (result == DialogResult.Yes)
                {
                    window.PerformSaveClick();
                    //HandleSave(filename);
                    if (saveCancelled)
                        return;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }

                // Notify the closing method that the file was asked to be closed 
                // from the File menu. Then close it.
                closedFromMenu = true;
                window.DoClose();
            }
        }

        /// <summary>
        /// Handles the request to open a new window.
        /// </summary>
        private void HandleNew()
        {
            window.OpenNew();
        }

        /// <summary>
        /// Updates the Spreadsheet window.
        /// </summary>
        /// <param name="cellname">The name of the cell to be updated.</param>
        private void UpdateSpreadsheetWindow(string cellname)
        {
            object cellContents = spreadsheet.GetCellContents(cellname);
            object cellValue = spreadsheet.GetCellValue(cellname);

            window.CellName = cellname;

            if(cellContents is Formula)
            {
                window.Contents = "=" + cellContents.ToString();
            }
            else
            {
                window.Contents = cellContents.ToString();
            }

            if (cellValue is FormulaError)
            {
                FormulaError error = (FormulaError)cellValue;
                window.Value = error.Reason;
            }
            else
            {
                window.Value = cellValue.ToString();
            }
            
        }

        /// <summary>
        /// Helper method to obtain the cell name from the row and column 
        /// indices.
        /// </summary>
        /// <param name="row">The row index of the cell</param>
        /// <param name="col">The column index of the cell</param>
        /// <returns>The name of the cell</returns>
        private string GetCellName(int row, int col)
        {
            int cellRow = ++row;
            char cellCol = (char)('A' + col);
            return "" + cellCol + cellRow;
        }
    }
}
