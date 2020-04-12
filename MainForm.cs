using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MachineLanguageSimulator {
	public partial class MainForm : Form {
		public MainForm() {
			this.InitializeComponent();

			this.Text = "Linn-Benton Community College Machine Language Simulator";
			//this.Icon = new Icon();

			this.Size = new Size(665, 700);
			this.MaximumSize = this.Size;
			this.MinimumSize = this.Size;

			Size labelSize = new Size(35, 25);
			Size cellSize = new Size(27, 25);

			Font labelFont = new Font("Courier New", 10.0f);
			Font cellFont = new Font("Courier New", 12.0f);

			/*Create the panels for interactive components*/

			this.mainMemoryPanel = new Panel() {
				Location = new Point(0, 0),
				Size = new Size(595, 585),
			};
			this.Controls.Add(this.mainMemoryPanel);

			this.registersPanel = new Panel() {
				Location = new Point(612, 0),
				Size = new Size(40, 585),
			};
			this.Controls.Add(this.registersPanel);

			this.buttonsPanel = new Panel() {
				Location = new Point(34, 600),
				Size = new Size(440, 35),
			};
			this.Controls.Add(this.buttonsPanel);

			this.pcirPanel = new Panel() {
				Location = new Point(553, 585),
				Size = new Size(150, 65),
			};
			this.Controls.Add(this.pcirPanel);

			/*Main Memory and Registers*/

			this.positionLabels = new Label[0x20];

			this.mainMemory = new MaskedTextBox[0x100];
			this.registers = new MaskedTextBox[0x10];

			Byte currentCell;

			for(Byte c = 0; c < 0x10; c++) {

				//Create the row labels
				this.positionLabels[c] = new Label() {
					Location = new Point(0, c * 35 + 25),
					Font = labelFont,
					Size = labelSize,
					Text = "0x" + c.ToString("X"),
					TextAlign = ContentAlignment.MiddleCenter
				};
				this.mainMemoryPanel.Controls.Add(this.positionLabels[c]);

				//Create the column labels
				this.positionLabels[c + 0x10] = new Label() {
					Location = new Point(c * 35 + 32, 0),
					Font = labelFont,
					Size = labelSize,
					Text = c.ToString("X"),
					TextAlign = ContentAlignment.MiddleCenter
				};
				this.mainMemoryPanel.Controls.Add(this.positionLabels[c + 0x10]);

			}

			this.registersLabel = new Label() {
				Location = new Point(0, 0),
				Font = labelFont,
				Size = labelSize,
				Text = "REG",
				TextAlign = ContentAlignment.MiddleCenter
			};
			this.registersPanel.Controls.Add(this.registersLabel);

			for (UInt16 row = 0; row < 0x10; row++) {

				for (UInt16 column = 0; column < 0x10; column++) {

					//Create each main memory cell
					currentCell = (Byte)(row * 0x10 + column);
					this.mainMemory[currentCell] = new MaskedTextBox() {
						Font = cellFont,
						InsertKeyMode = InsertKeyMode.Overwrite,
						Location = new Point(column * 35 + 35, row * 35 + 25),
						Mask = "AA",
						PromptChar = '0',
						Size = cellSize,
						TextAlign = HorizontalAlignment.Center,
					};
					this.mainMemory[currentCell].KeyPress += CellInput_KeyPressed;
					this.mainMemory[currentCell].KeyDown += MainMemoryArrowKeys_KeyDown;
					this.mainMemoryPanel.Controls.Add(mainMemory[currentCell]);
				}

				//Create each register
				this.registers[row] = new MaskedTextBox() {
					Font = cellFont,
					InsertKeyMode = InsertKeyMode.Overwrite,
					Location = new Point(3, row * 35 + 25),
					Mask = "AA",
					PromptChar = '0',
					Size = cellSize,
					TextAlign = HorizontalAlignment.Center
				};
				this.registers[row].KeyPress += CellInput_KeyPressed;
				this.registers[row].KeyDown += RegisterArrowKeys_KeyDown;
				this.registersPanel.Controls.Add(this.registers[row]);

			}

			/*PCIR*/

			this.pcLabel = new Label() {
				Location = new Point(27, 35),
				Font = labelFont,
				Size = labelSize,
				Text = "PC",
				TextAlign = ContentAlignment.MiddleCenter
			};
			this.pcirPanel.Controls.Add(this.pcLabel);

			this.irLabel = new Label() {
				Location = new Point(0, 0),
				Font = labelFont,
				Size = labelSize,
				Text = "IR",
				TextAlign = ContentAlignment.MiddleCenter
			};
			this.pcirPanel.Controls.Add(this.irLabel);

			this.instructionRegister = new MaskedTextBox() {
				Font = cellFont,
				InsertKeyMode = InsertKeyMode.Overwrite,
				Location = new Point(35, 0),
				Mask = "AAAA",
				PromptChar = '0',
				Size = new Size(54, 25),
				Text = "0000",
				TextAlign = HorizontalAlignment.Center,
			};
			this.instructionRegister.KeyPress += InstructionRegister_KeyPressed;
			this.pcirPanel.Controls.Add(this.instructionRegister);

			this.programCounter = new MaskedTextBox() {
				Font = cellFont,
				InsertKeyMode = InsertKeyMode.Overwrite,
				Location = new Point(62, 35),
				Mask = "AA",
				PromptChar = '0',
				Size = cellSize,
				Text = "00",
				TextAlign = HorizontalAlignment.Center,
			};
			this.programCounter.KeyPress += CellInput_KeyPressed;
			this.pcirPanel.Controls.Add(this.programCounter);

			/*Buttons*/

			this.runProgram = new Button() {
				Font = new Font("Verdana", 12.0f),
				Location = new Point(0, 0),
				Size = new Size(100, 35),
				Text = "Run",
				TextAlign = ContentAlignment.MiddleCenter
			};

			this.runProgram.Click += TestButton_OnClick;

			this.buttonsPanel.Controls.Add(this.runProgram);

			this.stepProgram = new Button() {
				Font = new Font("Verdana", 12.0f),
				Location = new Point(110, 0),
				Size = new Size(100, 35),
				Text = "Step",
				TextAlign = ContentAlignment.MiddleCenter
			};
			this.buttonsPanel.Controls.Add(this.stepProgram);

			this.saveFile = new Button() {
				Font = new Font("Verdana", 12.0f),
				Location = new Point(220, 0),
				Size = new Size(100, 35),
				Text = "Save",
				TextAlign = ContentAlignment.MiddleCenter
			};
			this.buttonsPanel.Controls.Add(this.saveFile);

			this.loadFile = new Button() {
				Font = new Font("Verdana", 12.0f),
				Location = new Point(330, 0),
				Size = new Size(100, 35),
				Text = "Load",
				TextAlign = ContentAlignment.MiddleCenter
			};
			this.buttonsPanel.Controls.Add(this.loadFile);

			/*Components initialized, finalize form properties*/

			this.ActiveControl = this.pcirPanel;

		}

		private readonly Label[] positionLabels;
		private readonly Label registersLabel;
		private readonly Label pcLabel;
		private readonly Label irLabel;

		private readonly MaskedTextBox[] mainMemory;
		private readonly MaskedTextBox[] registers;

		private readonly MaskedTextBox programCounter;
		private readonly MaskedTextBox instructionRegister;

		private readonly Button runProgram;
		private readonly Button stepProgram;
		private readonly Button saveFile;
		private readonly Button loadFile;

		private readonly Panel mainMemoryPanel;
		private readonly Panel registersPanel;
		private readonly Panel buttonsPanel;
		private readonly Panel pcirPanel;


		private void TestButton_OnClick(Object sender, EventArgs e) {

		}

		private void CellInput_KeyPressed(Object sender, KeyPressEventArgs kpe) {

			MaskedTextBox cell = sender as MaskedTextBox;

			cell.Focus();

			//Only allow hex formatted characters
			if((kpe.KeyChar >= 'A' && kpe.KeyChar <= 'F') || (kpe.KeyChar >= 'a' && kpe.KeyChar <= 'f') || (kpe.KeyChar >= '0' && kpe.KeyChar <= '9') || kpe.KeyChar == 8) {
				
				kpe.KeyChar = Char.ToUpper(kpe.KeyChar);
				kpe.Handled = false;

			}
			else {
				kpe.KeyChar = (Char)0;
				kpe.Handled = true;
				return;
			}

			if(cell.SelectionStart > 0) {
				this.SelectNextControl(this.ActiveControl, true, true, true, true);
			}

		}

		private void InstructionRegister_KeyPressed(Object sender, KeyPressEventArgs kpe) {

			MaskedTextBox cell = sender as MaskedTextBox;

			cell.Focus();

			//Only allow hex formatted characters
			if ((kpe.KeyChar >= 'A' && kpe.KeyChar <= 'F') || (kpe.KeyChar >= 'a' && kpe.KeyChar <= 'f') || (kpe.KeyChar >= '0' && kpe.KeyChar <= '9') || kpe.KeyChar == 8) {

				kpe.KeyChar = Char.ToUpper(kpe.KeyChar);
				kpe.Handled = false;

			}
			else {
				kpe.KeyChar = (Char)0;
				kpe.Handled = true;
				return;
			}

			if (cell.SelectionStart > 2) {
				this.SelectNextControl(this.ActiveControl, true, true, true, true);
			}

		}

		private void MainMemoryArrowKeys_KeyDown(Object sender, KeyEventArgs ke) {

			MaskedTextBox memoryCell = sender as MaskedTextBox;

			Byte currentPosition = (Byte)Array.IndexOf(this.mainMemory, sender);

			Int16 offset = 0;
			Byte cursorPosition = (Byte)memoryCell.SelectionStart;

			switch(ke.KeyCode) {

				case Keys.Up:
					offset = -0x10;
					break;

				case Keys.Down:
					offset = 0x10;
					break;

				case Keys.Left:
					if(cursorPosition == 0) {
						cursorPosition = 2;
						offset = -1;
					}
					break;

				case Keys.Right:
					if(cursorPosition > 1) {
						cursorPosition = 0;
						offset = 1;
					}
					break;

				default:
					return;

			}

			this.mainMemory[(Byte)(currentPosition + offset)].Focus();
			this.mainMemory[(Byte)(currentPosition + offset)].SelectionStart = cursorPosition;

		}

		private void RegisterArrowKeys_KeyDown(Object sender, KeyEventArgs ke) {

			MaskedTextBox memoryCell = sender as MaskedTextBox;

			Byte currentPosition = (Byte)Array.IndexOf(this.registers, sender);

			Int16 offset = 0;
			Byte cursorPosition = (Byte)memoryCell.SelectionStart;

			switch (ke.KeyCode) {

				case Keys.Up:
					offset = -1;
					break;

				case Keys.Down:
					offset = 1;
					break;

				case Keys.Left:
					if (cursorPosition == 0) {
						cursorPosition = 2;
						offset = -1;
					}
					break;

				case Keys.Right:
					if (cursorPosition > 1) {
						cursorPosition = 0;
						offset = 1;
					}
					break;

				default:
					return;

			}

			this.registers[(Byte)(currentPosition + offset) % 0x10].Focus();
			this.registers[(Byte)(currentPosition + offset) % 0x10].SelectionStart = cursorPosition;

		}

	}
}
