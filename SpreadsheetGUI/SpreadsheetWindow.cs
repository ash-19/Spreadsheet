// Developed by Snehashish Mishra, u0946268 on 8th March for
// CS 3500 offered by The University of Utah

using SSGui;
using System;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Includes the partial implemenation of the Spreadsheet view. 
    /// Fires events and manipulates the GUI.
    /// </summary>
    public partial class SpreadsheetWindow : Form, ISpreadsheetView
    {
        /// <summary>
        /// Constructs a new Spreadsheet window
        /// </summary>
        public SpreadsheetWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fired when a file is chosen from the file dialog.
        /// The parameter is the chosen filename.
        /// </summary>
        public event Action<string> FileChosenEvent;

        /// <summary>
        /// Fired when a close action is requested from the 
        /// File menu.
        /// </summary>
        public event Action CloseEvent;

        /// <summary>
        /// Fired when a new action is requested from the File 
        /// menu.
        /// </summary>
        public event Action NewEvent;

        /// <summary>
        /// Fired when the about action is requested from the 
        /// Help menu.
        /// </summary>
        public event Action HowToEvent;

        /// <summary>
        /// Fired when a new cell is selected. The parameter is the 
        /// current SpreadsheetPanel.
        /// </summary>
        public event Action<int, int> SelectionChanged;

        /// <summary>
        /// Fired when the Evaluate button is clicked. The parameter is 
        /// the cell name and its contents.
        /// </summary>
        public event Action<string, string> EvaluateClicked;

        /// <summary>
        /// Fired when a key is pressed. The parameters are the 
        /// key pressed, the cell name and the contents.
        /// </summary>
        public event Action<char, string, string> KeyPressed;

        /// <summary>
        /// Fired when the Save As is selected from the File 
        /// menu. The parameter is the Save file dialog.
        /// </summary>
        public event Action<SaveFileDialog> SaveAsEvent;

        /// <summary>
        /// Fired when the Save item is selected from the File 
        /// menu. The parameter the is the current file name.
        /// </summary>
        public event Action<string> SaveEvent;

        /// <summary>
        /// Fired when the user clicks the X close button. The 
        /// parameter is the Form closing arguments.
        /// </summary>
        public event Action<FormClosingEventArgs> ClosingForm;

        /// <summary>
        /// Closes this window
        /// </summary>
        public void DoClose()
        {
            Close();
        }

        /// <summary>
        /// Shows a message in the UI of the specified type.
        /// </summary>
        /// <param name="text">The contents in the message box</param>
        /// <param name="caption">The title of the box</param>
        /// <param name="boxButton">The type of the buttons available</param>
        /// <param name="boxIcon">The icon of the box</param>
        /// <returns></returns>
        public DialogResult Message(string text, string caption, 
                        MessageBoxButtons boxButton, MessageBoxIcon boxIcon)
        {
            DialogResult result = new DialogResult();

            if (boxButton.Equals(MessageBoxButtons.OK) && 
                    boxIcon.Equals(MessageBoxIcon.Error))
            {
                result = MessageBox.Show(text, caption, boxButton, boxIcon);
            }

            if (boxButton.Equals(MessageBoxButtons.YesNo) && 
                    boxIcon.Equals(MessageBoxIcon.Warning))
            {
                result = MessageBox.Show(text, caption, boxButton, boxIcon);
            }

            if (boxButton.Equals(MessageBoxButtons.OK) && 
                    boxIcon.Equals(MessageBoxIcon.Information))
            {
                result = MessageBox.Show(text, caption, boxButton, boxIcon);
            }

            if (boxButton.Equals(MessageBoxButtons.YesNoCancel) && 
                    boxIcon.Equals(MessageBoxIcon.Warning))
            {
                result = MessageBox.Show(text, caption, boxButton, boxIcon);
            }
            return result;
        }

        /// <summary>
        /// Performs a click on the Save menu item.
        /// </summary>
        public void PerformSaveClick()
        {
            saveAsToolStripMenuItem.PerformClick();
        }

        /// <summary>
        /// Sets the contents text box in the UI
        /// </summary>
        public string Contents
        {
            set { contentsTextBox.Text = value.ToString(); }
        }

        /// <summary>
        /// Sets the text in the selected cell in the Spreadsheet panel.
        /// </summary>
        /// <param name="row">The row index</param>
        /// <param name="col">The column index</param>
        /// <param name="value">The value to set in the cell</param>
        public void SetCellValue(int row, int col, string value)
        {
            spreadsheetPanel.SetValue(col, row, value);
        }

        /// <summary>
        /// Sets the value text box in the UI
        /// </summary>
        public string Value
        {
            set { valueTextBox.Text = value.ToString(); }
        }

        /// <summary>
        /// Sets the cell name text box in the UI
        /// </summary>
        public string CellName
        {
            set { cellNameTextBox.Text = value.ToString(); }
        }

        /// <summary>
        /// Opens a new Spreadsheet window.
        /// </summary>
        public void OpenNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        /// <summary>
        /// Fires an event when the close item is clicked in the File menu.
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The event arguments</param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }

        /// <summary>
        /// Fires an event when the new item is clicked in the File menu.
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The event arguments</param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NewEvent != null)
            {
                NewEvent();
            }
        }

        /// <summary>
        /// Fires an event when the open item is clicked in the File menu.
        /// Sets the properties of the pop-up open dialog box (like show 
        /// all files or only .ss files, need extension, etc. before firing 
        /// the event).
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The event arguments</param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Spreadsheet Files (*.ss)|*.ss|All Files (*.*)|*.*";
            openFileDialog1.DefaultExt = ".ss";
            openFileDialog1.Title = "Save As...";
            openFileDialog1.FileName = "";
            DialogResult result = openFileDialog1.ShowDialog();

            // if the selected file does not end with .ss extension when the user selected 
            // option 1, add it.
            if (openFileDialog1.FilterIndex == 1)
                openFileDialog1.AddExtension = true;
            
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (FileChosenEvent != null)
                {
                    FileChosenEvent(openFileDialog1.FileName);
                }
            }
        }

        /// <summary>
        /// Fires an event when the Save item is clicked in the File menu.
        /// If the file was never saved before, calls the Save As procedure.
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The event arguments</param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SpreadsheetWindow.ActiveForm.Text.Equals("Unsaved Spreadsheet"))
                saveAsToolStripMenuItem_Click(sender, e);

            if(SaveEvent != null)
            {
                SaveEvent(SpreadsheetWindow.ActiveForm.Text);
            }
        }

        /// <summary>
        /// Fires an event when the Evaluate button's clicked.
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The event arguments</param>
        private void evaluateButton_Click(object sender, EventArgs e)
        {
            if (EvaluateClicked != null)
            {
                EvaluateClicked(cellNameTextBox.Text, contentsTextBox.Text);
            }
        }

        /// <summary>
        /// Fires an event when the How To item is clicked in the Help menu.
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The event arguments</param>
        private void howToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HowToEvent != null)
            {
                HowToEvent();
            }
        }

        /// <summary>
        /// Sets the title in the UI
        /// </summary>
        public string Title
        {
            set { Text = value; }
        }

        /// <summary>
        /// Fires an event when a different cell is selected.
        /// </summary>
        /// <param name="sender">The spreadsheet panel of this window</param>
        private void spreadsheetPanel_SelectionChanged(SSGui.SpreadsheetPanel sender)
        {
            int row, col;
            spreadsheetPanel.GetSelection(out col, out row);

            if (SelectionChanged != null)
            {
                SelectionChanged(col, row);
            }
        }

        /// <summary>
        /// Fires an event when a key is pressed in the Contents text box. 
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The Key press event arguments</param>
        private void contentsTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(KeyPressed != null)
            {
                KeyPressed(e.KeyChar, cellNameTextBox.Text, contentsTextBox.Text);
            }
        }

        /// <summary>
        /// Fires an event when the Save as.. item is clicked in the File menu.
        /// Sets the properties of the pop-up save dialog box (like show 
        /// all files or only .ss files, need extension, etc. before firing 
        /// the event).
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The event arguments</param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Spreadsheet Files (*.ss)|*.ss|All Files (*.*)|*.*";
            saveFileDialog1.DefaultExt = ".ss";
            saveFileDialog1.Title = "Save As...";
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (SaveAsEvent != null)
                {
                    SaveAsEvent(saveFileDialog1);
                }
            }
        }

        /// <summary>
        /// Fires an event when the X close button is clicked on top right corner.
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The form closing event arguments received</param>
        private void SpreadsheetWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(ClosingForm != null)
            {
                ClosingForm(e);
            }
        }
    }
}
