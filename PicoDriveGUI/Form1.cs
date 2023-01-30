using System.IO.Ports;
using System.Text;

namespace PicoDriveGUI
{
    public partial class Form1 : Form
    {
        static SerialPort serialPort = new SerialPort();
        bool serialMonitor = false;

        bool MotorEnable = false;
        bool PrevMotorEnable = false;
        int PrevMotorEnableSource = 0;
        int PrevSpeedRefSource = 0;
        float PrevSpeedRefReg = 0;
        float PrevSpeedRefRate = 0;
        float PrevAnalogMax = 0;
        float PrevAnalogZero = 0;
        float PrevAnalogMin = 0;
        float PrevInternalRef1 = 0;
        float PrevInternalRef2 = 0;
        float PrevInternalRef3 = 0;
        float PrevInternalRef4 = 0;
        float PrevInternalRef5 = 0;
        float PrevCurrentLimit = 0;
        int PrevDigitalFunc1 = 0;
        int PrevDigitalFunc2 = 0;
        int PrevDigitalFunc3 = 0;
        int PrevDigitalFunc4 = 0;
        int PrevDigitalFunc5 = 0;

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

            updatePrevs();
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

            updatePrevs();

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
        }

        bool getFloats = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                return;
            }

            // Check for changed registers
            // Send out changes
            if (MotorEnable != PrevMotorEnable)
                serialPort.WriteLine("setreg 10 " + (MotorEnable ? "1" : "0"));

            if (MotorEnableSource.SelectedIndex != PrevMotorEnableSource)
                serialPort.WriteLine("setreg 11 " + MotorEnableSource.SelectedIndex.ToString());

            if (SpeedReferenceSource.SelectedIndex != PrevSpeedRefSource)
                serialPort.WriteLine("setreg 0 " + SpeedReferenceSource.SelectedIndex.ToString());

            if ((float)SpeedReferenceRegister.Value != PrevSpeedRefReg)
                serialPort.WriteLine("setregfloat 1 " + SpeedReferenceRegister.Value.ToString());

            if ((float)SpeedReferenceRate.Value != PrevSpeedRefRate)
                serialPort.WriteLine("setregfloat 9 " + SpeedReferenceRate.Value.ToString());

            if ((float)AnalogInMax.Value != PrevAnalogMax)
                serialPort.WriteLine("setregfloat 19 " + AnalogInMax.Value.ToString());

            if ((float)AnalogInMin.Value != PrevAnalogMin)
                serialPort.WriteLine("setregfloat 20 " + AnalogInMin.Value.ToString());

            if ((float)AnalogInZero.Value != PrevAnalogZero)
                serialPort.WriteLine("setregfloat 21 " + AnalogInZero.Value.ToString());

            if ((float)InternalReference1.Value != PrevInternalRef1)
                serialPort.WriteLine("setregfloat 12 " + InternalReference1.Value.ToString());

            if ((float)InternalReference2.Value != PrevInternalRef2)
                serialPort.WriteLine("setregfloat 13 " + InternalReference2.Value.ToString());

            if ((float)InternalReference3.Value != PrevInternalRef3)
                serialPort.WriteLine("setregfloat 14 " + InternalReference3.Value.ToString());

            if ((float)InternalReference4.Value != PrevInternalRef4)
                serialPort.WriteLine("setregfloat 15 " + InternalReference4.Value.ToString());

            if ((float)InternalReference5.Value != PrevInternalRef5)
                serialPort.WriteLine("setregfloat 16 " + InternalReference5.Value.ToString());

            if (DigitalFunction1.SelectedIndex != PrevDigitalFunc1)
                serialPort.WriteLine("setreg 3 " + DigitalFunction1.SelectedIndex.ToString());

            if (DigitalFunction2.SelectedIndex != PrevDigitalFunc2)
                serialPort.WriteLine("setreg 4 " + DigitalFunction2.SelectedIndex.ToString());

            if (DigitalFunction3.SelectedIndex != PrevDigitalFunc3)
                serialPort.WriteLine("setreg 5 " + DigitalFunction3.SelectedIndex.ToString());

            if (DigitalFunction4.SelectedIndex != PrevDigitalFunc4)
                serialPort.WriteLine("setreg 6 " + DigitalFunction4.SelectedIndex.ToString());

            if (DigitalFunction5.SelectedIndex != PrevDigitalFunc5)
                serialPort.WriteLine("setreg 7 " + DigitalFunction5.SelectedIndex.ToString());

            if ((float)MotorCurrentLimit.Value != PrevCurrentLimit)
                serialPort.WriteLine("setregfloat 2 " + MotorCurrentLimit.Value.ToString());

            updatePrevs();

            // Wait a bit if anything wrote out
            Thread.Sleep(10);

            // Read all registers
            if (!getFloats)
            {
                serialPort.WriteLine("getregs 0 3 4 5 6 7 10 11");
                getFloats = true;
            }
            else
            {
                serialPort.WriteLine("getregsfloat 1 2 8 9 12 13 14 15 16 17 18 19 20 21 22");
                getFloats = false;
            }
        }

        string intermediate = "";
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();

            // check if indata contains newline character
            // if not then there's still more data incoming, store in intermediate
            /*if (!indata.Contains('\n'))
            {
                intermediate += indata;
                return;
            }

            indata = intermediate;
            intermediate = "";*/

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
                        SpeedReferenceRegister.Invoke((MethodInvoker)(() => SpeedReferenceRegister.Value = (decimal)string_to_float(registers[++i])));
                        break;

                    case 2:
                        MotorCurrentLimit.Invoke((MethodInvoker)(() => MotorCurrentLimit.Value = (decimal)string_to_float(registers[++i])));
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
                        SpeedReferenceRate.Invoke((MethodInvoker)(() => SpeedReferenceRate.Value = (decimal)string_to_float(registers[++i])));
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
                        InternalReference1.Invoke((MethodInvoker)(() => InternalReference1.Value = (decimal)string_to_float(registers[++i])));
                        break;

                    case 13:
                        InternalReference2.Invoke((MethodInvoker)(() => InternalReference2.Value = (decimal)string_to_float(registers[++i])));
                        break;

                    case 14:
                        InternalReference3.Invoke((MethodInvoker)(() => InternalReference3.Value = (decimal)string_to_float(registers[++i])));
                        break;

                    case 15:
                        InternalReference4.Invoke((MethodInvoker)(() => InternalReference4.Value = (decimal)string_to_float(registers[++i])));
                        break;

                    case 16:
                        InternalReference5.Invoke((MethodInvoker)(() => InternalReference5.Value = (decimal)string_to_float(registers[++i])));
                        break;

                    case 17:
                        i++;
                        break;

                    case 18:
                        SpeedReference.Invoke((MethodInvoker)(() => SpeedReference.Text = registers[++i]));
                        break;

                    case 19:
                        AnalogInMax.Invoke((MethodInvoker)(() => AnalogInMax.Value = (decimal)string_to_float(registers[++i])));
                        break;

                    case 20:
                        AnalogInMin.Invoke((MethodInvoker)(() => AnalogInMin.Value = (decimal)string_to_float(registers[++i])));
                        break;

                    case 21:
                        AnalogInZero.Invoke((MethodInvoker)(() => AnalogInZero.Value = (decimal)string_to_float(registers[++i])));
                        break;

                    case 22:
                        AnalogInValue.Invoke((MethodInvoker)(() => AnalogInValue.Text = registers[++i]));
                        break;
                }
            }

            updatePrevsInvoke();

            //byte[] bytes = Encoding.Default.GetBytes(indata);

            //Console.Write(indata);
        }

        private void updatePrevs()
        {
            PrevMotorEnable = MotorEnable;
            PrevMotorEnableSource = MotorEnableSource.SelectedIndex;
            PrevSpeedRefSource = SpeedReferenceSource.SelectedIndex;
            PrevSpeedRefReg = (float)SpeedReferenceRegister.Value;
            PrevSpeedRefRate = (float)SpeedReferenceRate.Value;
            PrevAnalogMax = (float)AnalogInMax.Value;
            PrevAnalogMin = (float)AnalogInMin.Value;
            PrevAnalogZero = (float)AnalogInZero.Value;
            PrevInternalRef1 = (float)InternalReference1.Value;
            PrevInternalRef2 = (float)InternalReference2.Value;
            PrevInternalRef3 = (float)InternalReference3.Value;
            PrevInternalRef4 = (float)InternalReference4.Value;
            PrevInternalRef5 = (float)InternalReference5.Value;
            PrevDigitalFunc1 = DigitalFunction1.SelectedIndex;
            PrevDigitalFunc2 = DigitalFunction2.SelectedIndex;
            PrevDigitalFunc3 = DigitalFunction3.SelectedIndex;
            PrevDigitalFunc4 = DigitalFunction4.SelectedIndex;
            PrevDigitalFunc5 = DigitalFunction5.SelectedIndex;
            PrevCurrentLimit = (float)MotorCurrentLimit.Value;
        }

        private void updatePrevsInvoke()
        {
            PrevMotorEnable = MotorEnable;
            MotorEnableSource.Invoke((MethodInvoker)(() => PrevMotorEnableSource = MotorEnableSource.SelectedIndex));
            SpeedReferenceSource.Invoke((MethodInvoker)(() => PrevSpeedRefSource = SpeedReferenceSource.SelectedIndex));
            SpeedReferenceRegister.Invoke((MethodInvoker)(() => PrevSpeedRefReg = (float)SpeedReferenceRegister.Value));
            SpeedReferenceRate.Invoke((MethodInvoker)(() => PrevSpeedRefRate = (float)SpeedReferenceRate.Value));
            AnalogInMax.Invoke((MethodInvoker)(() => PrevAnalogMax = (float)AnalogInMax.Value));
            AnalogInMin.Invoke((MethodInvoker)(() => PrevAnalogMin = (float)AnalogInMin.Value));
            AnalogInZero.Invoke((MethodInvoker)(() => PrevAnalogZero = (float)AnalogInZero.Value));
            InternalReference1.Invoke((MethodInvoker)(() => PrevInternalRef1 = (float)InternalReference1.Value));
            InternalReference2.Invoke((MethodInvoker)(() => PrevInternalRef2 = (float)InternalReference2.Value));
            InternalReference3.Invoke((MethodInvoker)(() => PrevInternalRef3 = (float)InternalReference3.Value));
            InternalReference4.Invoke((MethodInvoker)(() => PrevInternalRef4 = (float)InternalReference4.Value));
            InternalReference5.Invoke((MethodInvoker)(() => PrevInternalRef5 = (float)InternalReference5.Value));
            DigitalFunction1.Invoke((MethodInvoker)(() => PrevDigitalFunc1 = DigitalFunction1.SelectedIndex));
            DigitalFunction2.Invoke((MethodInvoker)(() => PrevDigitalFunc2 = DigitalFunction2.SelectedIndex));
            DigitalFunction3.Invoke((MethodInvoker)(() => PrevDigitalFunc3 = DigitalFunction3.SelectedIndex));
            DigitalFunction4.Invoke((MethodInvoker)(() => PrevDigitalFunc4 = DigitalFunction4.SelectedIndex));
            DigitalFunction5.Invoke((MethodInvoker)(() => PrevDigitalFunc5 = DigitalFunction5.SelectedIndex));
            MotorCurrentLimit.Invoke((MethodInvoker)(() => PrevCurrentLimit = (float)MotorCurrentLimit.Value));
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
    }
}