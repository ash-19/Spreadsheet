// Developed by Snehashish Mishra, u0946268 on 8th March for
// CS 3500 offered by The University of Utah

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using System.Windows.Forms;

namespace ControllerTester
{
    /// <summary>
    /// Provides testing on a stub version of the SpreadsheetWindow 
    /// written against the Controller/Model combination.
    /// </summary>
    [TestClass]
    public class ControllerTestCases
    {
        /// <summary>
        /// Tests DoClose() method on an empty sheet
        /// </summary>
        [TestMethod]
        public void CalledDoClose01()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireCloseEvent();
            Assert.IsTrue(stub.CalledDoClose);
        }

        /// <summary>
        /// Tests DoClose() method on a saved sheet (after opening it)
        /// </summary>
        [TestMethod]
        public void CalledDoClose02()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");

            stub.FireCloseEvent();
            Assert.IsTrue(stub.CalledDoClose);
        }

        /// <summary>
        /// Tests where FileChosenEvent was fired and if the data 
        /// in the sheet checks out.
        /// </summary>
        [TestMethod]
        public void FileChosenEvent01()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            stub.FireSelectionChangedEvent(0, 0);
            Assert.AreEqual("Doubles", stub.Value.ToString());
            Assert.AreEqual("A1", stub.CellName.ToString());
            Assert.AreEqual("Doubles", stub.Contents.ToString());

            stub.FireSelectionChangedEvent(0, 2);
            Assert.AreEqual("3", stub.Value.ToString());
            Assert.AreEqual("A3", stub.CellName.ToString());
            Assert.AreEqual("3", stub.Contents.ToString());

            stub.FireSelectionChangedEvent(0, 3);
            Assert.AreEqual("hi", stub.Value.ToString());
            Assert.AreEqual("A4", stub.CellName.ToString());
            Assert.AreEqual("hi", stub.Contents.ToString());
        }

        /// <summary>
        /// Tests the new file menu item.
        /// </summary>
        [TestMethod]
        public void NewFile01()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            
            stub.FireNewEvent();
            Assert.IsTrue(stub.CalledOpenNew);
        }

        /// <summary>
        /// Tests the Save file menu item
        /// </summary>
        [TestMethod]
        public void SaveFile01()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            stub.FireSaveEvent("../../demo1.ss");
            Assert.IsTrue(stub.CalledSaveEvent);
        }

        /// <summary>
        /// Tests the Save file menu item for a new sheet
        /// </summary>
        [TestMethod]
        public void SaveFile02()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireSaveEvent("Unsaved Spreadsheet");
            Assert.IsTrue(stub.CalledSaveEvent);
        }

        /// <summary>
        /// Tests the Save As... file menu item for an existing, 
        /// opened sheet.
        /// </summary>
        [TestMethod]
        public void SaveFile03()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            stub.FireSaveAsEvent(new SaveFileDialog());
            Assert.IsTrue(stub.CalledSaveAsEvent);
        }

        /// <summary>
        /// Tests the Save As... file menu item to save as a 
        /// new file.
        /// </summary>
        [TestMethod]
        public void SaveFile04()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = ".../.../SaveAs02.ss";

            stub.FireSaveAsEvent(sfd);
            Assert.IsTrue(stub.CalledSaveAsEvent);
        }

        /// <summary>
        /// Tests Save after modifying opened sheet.
        /// </summary>
        [TestMethod]
        public void SaveFile05()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            // Make a change
            stub.FireEvaluateClicked("A2", "4");

            // Save the file
            stub.FireSaveEvent("../../demo1.ss");
            Assert.IsTrue(stub.CalledSaveEvent);
        }

        /// <summary>
        /// Tests the evaluate button click.
        /// </summary>
        [TestMethod]
        public void EvaluateButton01()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            stub.FireEvaluateClicked("A1", "Doubles");
            Assert.IsTrue(stub.CalledEvaluateClick);
        }

        /// <summary>
        /// Tests the How To menu item click
        /// </summary>
        [TestMethod]
        public void HowTo01()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            stub.FireHowToEvent();
            Assert.IsTrue(stub.CalledHowTo);
            Assert.IsTrue(stub.CalledMessage);
        }

        /// <summary>
        /// Tests the return key press in contents text box 
        /// (should evaluate cells).
        /// </summary>
        [TestMethod]
        public void KeyPress01()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            stub.FireKeyPressedEvent('\r', "A1", "Doubles");
            Assert.IsTrue(stub.CalledKeyPressedEvent);
        }

        /// <summary>
        /// Tests closing an unchanged, opened sheet by clicking the 
        /// X button.
        /// </summary>
        [TestMethod]
        public void XClose01()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            stub.FireClosingForm(new FormClosingEventArgs(new CloseReason(), true));
            Assert.IsTrue(stub.CalledClosingForm);
        }

        /// <summary>
        /// Tests making a change in already open file and closing it. 
        /// When prompted to save, selected cancel.
        /// </summary>
        [TestMethod]
        public void XClose02()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            // Make a change
            stub.FireEvaluateClicked("A2", "4");

            // Set dialog selection to cancel (save warning)
            stub.customSetDialogResult = DialogResult.Cancel;
            stub.FireClosingForm(new FormClosingEventArgs(new CloseReason(), true));
            Assert.IsTrue(stub.CalledClosingForm);
        }

        /// <summary>
        /// Tests making a change in already open file and closing it. 
        /// When prompted to save, selected Yes.
        /// </summary>
        [TestMethod]
        public void XClose03()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            // Make a change
            stub.FireEvaluateClicked("A2", "4");

            // Set dialog selection to cancel (save warning)
            stub.customSetDialogResult = DialogResult.Yes;
            stub.FireClosingForm(new FormClosingEventArgs(new CloseReason(), true));
            Assert.IsTrue(stub.CalledClosingForm);
        }

        /// <summary>
        /// Tests closing an unchanged, loaded sheet via the 
        /// Close item in File menu
        /// </summary>
        [TestMethod]
        public void MenuClose01()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            stub.FireCloseEvent();
            Assert.IsTrue(stub.CalledCloseEvent);
        }

        /// <summary>
        /// Close from file menu when made a modification. Tests for 
        /// selecting Yes and Cancel in the pop-up message box.
        /// </summary>
        [TestMethod]
        public void MenuClose02()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            // Change value of A2
            stub.FireSelectionChangedEvent(0, 1);
            stub.Value = "4";
            stub.Contents = "4";
            Assert.AreEqual("A2", stub.CellName);

            stub.FireEvaluateClicked("A2", "4");

            // Cancel closing sheet
            stub.customSetDialogResult = DialogResult.Cancel;
            stub.FireCloseEvent();
            Assert.IsTrue(stub.CalledCloseEvent);

            // Save sheet
            stub.customSetDialogResult = DialogResult.Yes;
            stub.FireCloseEvent();
            Assert.IsTrue(stub.CalledCloseEvent);
        }

        /// <summary>
        /// Tests Evaluate button when the contents changed throws 
        /// exceptions.
        /// </summary>
        [TestMethod]
        public void EvaluateButton02()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            //Formula Format Exception
            stub.FireEvaluateClicked("E1", "=Z101");
            Assert.IsTrue(stub.CalledEvaluateClick);
            Assert.IsTrue(stub.CalledMessage);

            //Circular Exception
            stub.FireEvaluateClicked("A2", "=B2");
            Assert.IsTrue(stub.CalledEvaluateClick);
            Assert.IsTrue(stub.CalledMessage);
        }

        /// <summary>
        /// Tests opening a corrupted spreadsheet 
        /// </summary>
        [TestMethod]
        public void FileChosenEvent02()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            // IOException
            stub.FireFileChosenEvent("../../InvalidFormatSheet.ss");
            Assert.IsTrue(stub.CalledFileChosenEvent);
        }

        /// <summary>
        /// Tests opening a partially saved spreadsheet (emulates crashed 
        /// when saving, thus losing some data).
        /// </summary>
        [TestMethod]
        public void FileChosenEvent03()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            // Spreadsheet Read Exception
            stub.FireFileChosenEvent("../../IllegalFormat.ss");
            Assert.IsTrue(stub.CalledFileChosenEvent);
        }

        /// <summary>
        /// Tests opening a spreadsheet with "" filename
        /// </summary>
        [TestMethod]
        public void FileChosenEvent04()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            // Spreadsheet Read Exception
            stub.FireFileChosenEvent("");
            Assert.IsTrue(stub.CalledFileChosenEvent);
        }

        /// <summary>
        /// Tests opening a new file when working in an unsaved file. 
        /// When asked, do not save the current file and open the new file.
        /// </summary>
        [TestMethod]
        public void FileChosenEvent05()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            // Change value of A2
            stub.FireSelectionChangedEvent(0, 1);
            stub.Value = "4";
            stub.Contents = "4";
            Assert.AreEqual("A2", stub.CellName);

            stub.FireEvaluateClicked("A2", "4");

            // Selecting No in pop-up message prompting to save
            stub.customSetDialogResult = DialogResult.No;
            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.IsTrue(stub.CalledFileChosenEvent);
        }

        /// <summary>
        /// Tests opening a new file when working in an unsaved file. 
        /// When asked, save the current file and open the new file.
        /// </summary>
        [TestMethod]
        public void FileChosenEvent06()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.AreEqual("../../demo1.ss", stub.Title);

            // Change value of A2
            stub.FireSelectionChangedEvent(0, 1);
            stub.Value = "4";
            stub.Contents = "4";
            Assert.AreEqual("A2", stub.CellName);

            stub.FireEvaluateClicked("A2", "4");

            // Selecting Yes in pop-up message prompting to save
            stub.customSetDialogResult = DialogResult.Yes;
            stub.FireFileChosenEvent("../../demo1.ss");
            Assert.IsTrue(stub.CalledFileChosenEvent);
        }
    }
}
