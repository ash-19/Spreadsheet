// Developed by Snehashish Mishra, u0946268 on 8th March for
// CS 3500 offered by The University of Utah

using System;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// The interface providing access to view (GUI) for the 
    /// Controller.
    /// </summary>
    public interface ISpreadsheetView
    {
        /// <summary>
        /// Provides a open menu item event
        /// </summary>
        event Action<string> FileChosenEvent;

        /// <summary>
        /// Provides a close menu item event
        /// </summary>
        event Action CloseEvent;

        /// <summary>
        /// Provides a new menu item event
        /// </summary>
        event Action NewEvent;

        /// <summary>
        /// Provides a how to menu item event
        /// </summary>
        event Action HowToEvent;

        /// <summary>
        /// Provides a cell SelectionChanged event. The 
        /// parameter are row and column indices.
        /// </summary>
        event Action<int, int> SelectionChanged;

        /// <summary>
        /// Provides a Evaluate button clicked event. The 
        /// parameter are cell name and contents.
        /// </summary>
        event Action<string, string> EvaluateClicked;

        /// <summary>
        /// Provides a key pressed event. The parameters are 
        /// the char key pressed, cell name, contents.
        /// </summary>
        event Action<char, string, string> KeyPressed;

        /// <summary>
        /// Provides a save as menu item event
        /// </summary>
        event Action<SaveFileDialog> SaveAsEvent;

        /// <summary>
        /// Provides a save menu item event
        /// </summary>
        event Action<string> SaveEvent;

        /// <summary>
        /// Provides a X button pressed event.
        /// </summary>
        event Action<FormClosingEventArgs> ClosingForm;

        /// <summary>
        /// Provides the title of current window
        /// </summary>
        string Title { set; }

        /// <summary>
        /// Provides the contents of current cell
        /// </summary>
        string Contents { set; }

        /// <summary>
        /// Provides the value of current cell
        /// </summary>
        string Value { set; }

        /// <summary>
        /// Provides the cell name of current cell
        /// </summary>
        string CellName { set; }

        /// <summary>
        /// Closes the current window
        /// </summary>
        void DoClose();

        /// <summary>
        /// Opens a new window
        /// </summary>
        void OpenNew();

        /// <summary>
        /// Emulates clicking Save menu item
        /// </summary>
        void PerformSaveClick();

        /// <summary>
        /// Provides the MessageBox defined by with passed 
        /// parameters.
        /// </summary>
        DialogResult Message(string text, string caption, 
            MessageBoxButtons boxButton, MessageBoxIcon boxIcon);

        /// <summary>
        /// Sets the value of the cell in the spreadsheetPanel.
        /// </summary>
        /// <param name="row">Row index of cell</param>
        /// <param name="col">Column index of cell</param>
        /// <param name="value">Value to be set</param>
        void SetCellValue(int row, int col, string value);
    }
}
