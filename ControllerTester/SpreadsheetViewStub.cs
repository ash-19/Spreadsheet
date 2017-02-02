// Developed by Snehashish Mishra, u0946268 on 8th March for
// CS 3500 offered by The University of Utah

using System;
using SSGui;
using System.Windows.Forms;

namespace ControllerTester
{
    /// <summary>
    /// Provides a stubbed version of SpreadsheetView to test the 
    /// model/controller combination against.
    /// </summary>
    public class SpreadsheetViewStub : SpreadsheetGUI.ISpreadsheetView
    {
        /// <summary>
        /// Instance variable to emulate selecting the button in 
        /// a message dialog.
        /// </summary>
        public DialogResult customSetDialogResult { get; set;  }

        /// <summary>
        /// Property to record whether DoClose has been called.
        /// </summary>
        public bool CalledDoClose
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether OpenNew has been called.
        /// </summary>
        public bool CalledOpenNew
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether PerformSaveClick 
        /// has been called.
        /// </summary>
        public bool CalledPerformSaveClick
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether SetCellValue has been called.
        /// </summary>
        public bool CalledSetCellValue
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether Message has been called.
        /// </summary>
        public bool CalledMessage
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether SaveEvent has been called.
        /// </summary>
        public bool CalledSaveEvent
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether HowToEvent has been called.
        /// </summary>
        public bool CalledHowTo
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether SaveAsEvent has been called.
        /// </summary>
        public bool CalledSaveAsEvent
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether Evaluate button has been clicked.
        /// </summary>
        public bool CalledEvaluateClick
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether a key is pressed in contents text box.
        /// </summary>
        public bool CalledKeyPressedEvent
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether X button is clicked to close.
        /// </summary>
        public bool CalledClosingForm
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether Close menu item is selected.
        /// </summary>
        public bool CalledCloseEvent
        {
            get; private set;
        }

        /// <summary>
        /// Property to record whether Open menu item is clicked.
        /// </summary>
        public bool CalledFileChosenEvent
        {
            get; private set;
        }

        /// <summary>
        /// This property implements the interface.
        /// </summary>
        public string CellName  {  get;  set;  }

        /// <summary>
        /// This property implements the interface.
        /// </summary>
        public string Contents  { get; set; }

        /// <summary>
        /// This property implements the interface.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// This property implements the interface.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action CloseEvent;

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action<FormClosingEventArgs> ClosingForm;

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action<string, string> EvaluateClicked;

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action<string> FileChosenEvent;

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action HowToEvent;

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action<char, string, string> KeyPressed;

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action NewEvent;

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action<SaveFileDialog> SaveAsEvent;

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action<string> SaveEvent;

        /// <summary>
        /// This event implements the interface
        /// </summary>
        public event Action<int, int> SelectionChanged;
        
        /// <summary>
        /// Provides method to fire close event.
        /// </summary>
        public void FireCloseEvent()
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
            CalledCloseEvent = true;
        }

        /// <summary>
        /// Provides method to fire HowTo event.
        /// </summary>
        public void FireHowToEvent()
        {
            if (HowToEvent != null)
            {
                HowToEvent();
            }
            CalledHowTo = true;
        }

        /// <summary>
        /// Provides method to fire X button event.
        /// </summary>
        public void FireClosingForm(FormClosingEventArgs e)
        {
            if (ClosingForm != null)
            {
                ClosingForm(e);
            }
            CalledClosingForm = true;
        }

        /// <summary>
        /// Provides method to fire evaluate clicked event.
        /// </summary>
        public void FireEvaluateClicked(string cellname, string contents)
        {
            if (EvaluateClicked != null)
            {
                EvaluateClicked(cellname, contents);
            }
            CalledEvaluateClick = true;
        }

        /// <summary>
        /// Provides method to fire open menu event.
        /// </summary>
        public void FireFileChosenEvent(string filename)
        {
            if (FileChosenEvent != null)
            {
                FileChosenEvent(filename);
            }
            CalledFileChosenEvent = true;
        }

        /// <summary>
        /// Provides method to fire key pressed event.
        /// </summary>
        public void FireKeyPressedEvent(char key, string cellname, string contents)
        {
            if (KeyPressed != null)
            {
                KeyPressed(key, cellname, contents);
            }
            CalledKeyPressedEvent = true;
        }

        /// <summary>
        /// Provides method to fire new menu item clicked event.
        /// </summary>
        public void FireNewEvent()
        {
            if (NewEvent != null)
            {
                NewEvent();
            }
        }

        /// <summary>
        /// Provides method to fire Save As... event.
        /// </summary>
        public void FireSaveAsEvent(SaveFileDialog s)
        {
            if (SaveAsEvent != null)
            {
                SaveAsEvent(s);
            }
            CalledSaveAsEvent = true;
        }

        /// <summary>
        /// Provides method to fire Save event.
        /// </summary>
        public void FireSaveEvent(string filename)
        {
            if (SaveEvent != null)
            {
                SaveEvent(filename);
            }
            CalledSaveEvent = true;
        }

        /// <summary>
        /// Provides method to fire selection changed event.
        /// </summary>
        public void FireSelectionChangedEvent(int col, int row)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(col, row);
            }
        }

        /// <summary>
        /// Provides method to invoke DoClose().
        /// </summary>
        public void DoClose()
        {
            CalledDoClose = true;
        }

        /// <summary>
        /// Provides method invoke Message().
        /// </summary>
        public DialogResult Message(string text, string caption, 
                    MessageBoxButtons boxButton, MessageBoxIcon boxIcon)
        {
            DialogResult result = customSetDialogResult;
            CalledMessage = true;
            return result;
        }

        /// <summary>
        /// Provides method open new window.
        /// </summary>
        public void OpenNew()
        {
            CalledOpenNew = true;
        }

        /// <summary>
        /// Provides method to perform save click.
        /// </summary>
        public void PerformSaveClick()
        {
            CalledPerformSaveClick = true;
        }

        /// <summary>
        /// Provides method to Set cell value.
        /// </summary>
        public void SetCellValue(int row, int col, string value)
        {
            SpreadsheetPanel s = new SpreadsheetPanel();
            s.SetValue(row, col, value);
            CalledSetCellValue = true;
        }
    }
}
