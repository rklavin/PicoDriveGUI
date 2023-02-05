using System.IO.Ports;
using System.Text;

namespace PicoDriveGUI
{
    public partial class Form1 : Form
    {
        static SerialPort serialPort = new SerialPort();
        bool serialMonitor = false;

        bool MotorEnable = false;
        bool streamRegisters = false;

        public Form1()
        {
            InitializeComponent();

            //GlobalOptions.loadSettings();

            baudRateComboBox.SelectedIndex = 4;
            serialPort.BaudRate = 9600;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
            serialPort.DtrEnable = true;

            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            timer1.Start();
        }

        private void commPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (commPortComboBox.SelectedIndex == -1) return;
            serialMonitorButton.Enabled = true;
        }

        private void commPortComboBox_DropDown(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            commPortComboBox.Items.Clear();
            commPortComboBox.Items.AddRange(ports);
        }

        private void serialMonitorButton_Click(object sender, EventArgs e)
        {
            if (commPortComboBox.Text == "")
            {
                return;
            }

            if (serialMonitor)
            {
                serialMonitorButton.Text = "Connect";

                serialPort.Close();

                serialMonitor = false;
                commPortComboBox.Enabled = true;
                baudRateComboBox.Enabled = true;
            }
            else
            {
                serialPort.PortName = commPortComboBox.Text;
                serialPort.BaudRate = int.Parse(baudRateComboBox.Text);
                try
                {
                    serialPort.Open();
                }
                catch
                {
                    return;
                }

                serialMonitorButton.Text = "Disconnect";
                serialMonitor = true;
                commPortComboBox.Enabled = false;
                baudRateComboBox.Enabled = false;
            }
        }

        private void MotorEnableButton_Click(object sender, EventArgs e)
        {
            MotorEnable = !MotorEnable;
            if (MotorEnable) MotorEnableButton.Text = "Disable";
            if (!MotorEnable) MotorEnableButton.Text = "Enable";

            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("setreg 10 " + (MotorEnable ? "1" : "0"));
            }
        }

        bool getFloats = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                return;
            }

            // Wait a bit if anything wrote out
            Thread.Sleep(10);

            // Read all registers
            if (!getFloats)
            {
                //serialPort.WriteLine("getregs 0 3 4 5 6 7 10 11");
                getFloats = true;
            }
            else
            {
                //serialPort.WriteLine("getregsfloat 1 2 8 9 12 13 14 15 16 17 18 19 20 21 22");
                getFloats = false;
            }
        }

        string intermediate = "";
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();

            // Received data should be in the format of register #, space, register data
            // e.g. 1 55.000000 19 10.000000 20 -10.00000 21 0.000000
            string[] registers = indata.Split(' ');

            for (int i = 0; i < registers.Length; i++)
            {
                if ((i + 1) >= registers.Length) break;

                switch (string_to_int(registers[i]))
                {
                    case 0:
                        SpeedReferenceSource.Invoke((MethodInvoker)(() => SpeedReferenceSource.SelectedIndex = string_to_int(registers[++i])));
                        break;

                    case 1:
                        SpeedReferenceRegister.Invoke((MethodInvoker)(() => SpeedReferenceRegister.Text = registers[++i]));
                        break;

                    case 2:
                        MotorCurrentLimit.Invoke((MethodInvoker)(() => MotorCurrentLimit.Text = registers[++i]));
                        break;

                    case 3:
                        DigitalFunction1.Invoke((MethodInvoker)(() => DigitalFunction1.SelectedIndex = string_to_int(registers[++i])));
                        break;

                    case 4:
                        DigitalFunction2.Invoke((MethodInvoker)(() => DigitalFunction2.SelectedIndex = string_to_int(registers[++i])));
                        break;

                    case 5:
                        DigitalFunction3.Invoke((MethodInvoker)(() => DigitalFunction3.SelectedIndex = string_to_int(registers[++i])));
                        break;

                    case 6:
                        DigitalFunction4.Invoke((MethodInvoker)(() => DigitalFunction4.SelectedIndex = string_to_int(registers[++i])));
                        break;

                    case 7:
                        DigitalFunction5.Invoke((MethodInvoker)(() => DigitalFunction5.SelectedIndex = string_to_int(registers[++i])));
                        break;

                    case 8:
                        MotorCurrent.Invoke((MethodInvoker)(() => MotorCurrent.Text = registers[++i]));
                        break;

                    case 9:
                        SpeedReferenceRate.Invoke((MethodInvoker)(() => SpeedReferenceRate.Text = registers[++i]));
                        break;

                    case 10:
                        MotorEnable = string_to_int(registers[++i]) == 0 ? false : true;
                        MotorEnabled.Invoke((MethodInvoker)(() => MotorEnabled.Text = MotorEnable ? "1" : "0"));
                        MotorEnableButton.Invoke((MethodInvoker)(() => MotorEnableButton.Text = MotorEnable ? "Disable" : "Enable"));
                        break;

                    case 11:
                        MotorEnableSource.Invoke((MethodInvoker)(() => MotorEnableSource.SelectedIndex = string_to_int(registers[++i])));
                        break;

                    case 12:
                        InternalReference1.Invoke((MethodInvoker)(() => InternalReference1.Text = registers[++i]));
                        break;

                    case 13:
                        InternalReference2.Invoke((MethodInvoker)(() => InternalReference2.Text = registers[++i]));
                        break;

                    case 14:
                        InternalReference3.Invoke((MethodInvoker)(() => InternalReference3.Text = registers[++i]));
                        break;

                    case 15:
                        InternalReference4.Invoke((MethodInvoker)(() => InternalReference4.Text = registers[++i]));
                        break;

                    case 16:
                        InternalReference5.Invoke((MethodInvoker)(() => InternalReference5.Text = registers[++i]));
                        break;

                    case 17:
                        i++;
                        break;

                    case 18:
                        SpeedReference.Invoke((MethodInvoker)(() => SpeedReference.Text = registers[++i]));
                        break;

                    case 19:
                        AnalogInMax.Invoke((MethodInvoker)(() => AnalogInMax.Text = registers[++i]));
                        break;

                    case 20:
                        AnalogInMin.Invoke((MethodInvoker)(() => AnalogInMin.Text = registers[++i]));
                        break;

                    case 21:
                        AnalogInZero.Invoke((MethodInvoker)(() => AnalogInZero.Text = registers[++i]));
                        break;

                    case 22:
                        AnalogInValue.Invoke((MethodInvoker)(() => AnalogInValue.Text = registers[++i]));
                        break;

                    case 23:
                        AnalogInDeadband.Invoke((MethodInvoker)(() => AnalogInDeadband.Text = registers[++i]));
                        break;

                    case 24:
                        RunForward.Invoke((MethodInvoker)(() => RunForward.Text = registers[++i]));
                        break;

                    case 25:
                        RunReverse.Invoke((MethodInvoker)(() => RunReverse.Text = registers[++i]));
                        break;

                    case 26:
                        ForwardInhibit.Invoke((MethodInvoker)(() => ForwardInhibit.Text = registers[++i]));
                        break;

                    case 27:
                        ReverseInhibit.Invoke((MethodInvoker)(() => ReverseInhibit.Text = registers[++i]));
                        break;

                    case 28:
                        DigitalInput1Status.Invoke((MethodInvoker)(() => DigitalInput1Status.Text = registers[++i]));
                        break;

                    case 29:
                        DigitalInput2Status.Invoke((MethodInvoker)(() => DigitalInput2Status.Text = registers[++i]));
                        break;

                    case 30:
                        DigitalInput3Status.Invoke((MethodInvoker)(() => DigitalInput3Status.Text = registers[++i]));
                        break;

                    case 31:
                        DigitalInput4Status.Invoke((MethodInvoker)(() => DigitalInput4Status.Text = registers[++i]));
                        break;

                    case 32:
                        DigitalInput5Status.Invoke((MethodInvoker)(() => DigitalInput5Status.Text = registers[++i]));
                        break;

                    default:
                        i++;
                        break;
                }
            }

            //updatePrevsInvoke();

            //byte[] bytes = Encoding.Default.GetBytes(indata);

            //Console.Write(indata);
        }

        // Convert string to integer
        private int string_to_int(string str)
        {
            int res = 0;
            int i = 0;
            bool neg = false;

            if (str[i] == '-')
            {
                neg = true;
                i += 1;
            }

            while (str[i] >= '0' && str[i] <= '9')
            {
                res = res * 10 + (str[i] - '0');
                i += 1;
                if (i >= str.Length) break;
            }

            if (neg) res = res * -1;

            return res;
        }

        // Convert string to float
        private float string_to_float(string str)
        {
            float res = 0;
            float dec = 0;
            int i = 0;
            float j = 1;
            bool neg = false;

            if (str[i] == '-')
            {
                neg = true;
                i += 1;
            }

            while (str[i] >= '0' && str[i] <= '9')
            {
                res = res * 10 + (str[i] - '0');
                i += 1;
                if (i >= str.Length) return res;
            }

            if (str[i] == '.')
            {
                i += 1;
                while (str[i] >= '0' && str[i] <= '9')
                {
                    dec = dec * 10 + (str[i] - '0');
                    i += 1;
                    j *= 10;
                    if (i >= str.Length) break;
                }

                dec = dec / j;
                res += dec;
            }

            if (neg) res = res * -1;

            return res;
        }

        private void SaveRegs_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("saveregs");
            }
        }

        private void LoadRegs_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("loadregs");
            }
        }

        private void ReadRegistersButton_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if (!streamRegisters)
                {
                    serialPort.WriteLine("streamregs on");
                    ReadRegistersButton.Text = "Stop Stream";
                    streamRegisters = true;
                }
                else
                {
                    serialPort.WriteLine("streamregs off");
                    ReadRegistersButton.Text = "Read Registers";
                    streamRegisters = false;
                }
            }
            else
            {
                streamRegisters = false;
                ReadRegistersButton.Text = "Read Registers";
            }
        }

        private void MotorEnableSourceSet_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("setreg 11 " + (MotorEnableSourceSet.SelectedIndex));
            }
        }

        private void SpeedReferenceSourceSet_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("setreg 0 " + (SpeedReferenceSourceSet.SelectedIndex));
            }
        }

        private void SpeedReferenceRegisterSet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 1 " + SpeedReferenceRegisterSet.Text);
                    e.Handled = true;
                }
            }
        }

        private void SpeedReferenceRateSet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 9 " + SpeedReferenceRateSet.Text);
                    e.Handled = true;
                }
            }
        }

        private void MotorCurrentLimitSet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 2 " + MotorCurrentLimitSet.Text);
                    e.Handled = true;
                }
            }
        }

        private void InternalReference1Set_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 12 " + InternalReference1Set.Text);
                    e.Handled = true;
                }
            }
        }

        private void InternalReference2Set_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 13 " + InternalReference2Set.Text);
                    e.Handled = true;
                }
            }
        }

        private void InternalReference3Set_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 14 " + InternalReference3Set.Text);
                    e.Handled = true;
                }
            }
        }

        private void InternalReference4Set_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 15 " + InternalReference4Set.Text);
                    e.Handled = true;
                }
            }
        }

        private void InternalReference5Set_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 16 " + InternalReference5Set.Text);
                    e.Handled = true;
                }
            }
        }

        private void DigitalFunction1Set_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("setreg 3 " + (DigitalFunction1Set.SelectedIndex));
            }
        }

        private void DigitalFunction2Set_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("setreg 4 " + (DigitalFunction2Set.SelectedIndex));
            }
        }

        private void DigitalFunction3Set_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("setreg 5 " + (DigitalFunction3Set.SelectedIndex));
            }
        }

        private void DigitalFunction4Set_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("setreg 6 " + (DigitalFunction4Set.SelectedIndex));
            }
        }

        private void DigitalFunction5Set_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("setreg 7 " + (DigitalFunction5Set.SelectedIndex));
            }
        }

        private void AnalogInMaxSet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 19 " + AnalogInMaxSet.Text);
                    e.Handled = true;
                }
            }
        }

        private void AnalogInZeroSet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 21 " + AnalogInZeroSet.Text);
                    e.Handled = true;
                }
            }
        }

        private void AnalogInMinSet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 20 " + AnalogInMinSet.Text);
                    e.Handled = true;
                }
            }
        }

        private void AnalogInDeadbandSet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("setregfloat 23 " + AnalogInDeadbandSet.Text);
                    e.Handled = true;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}