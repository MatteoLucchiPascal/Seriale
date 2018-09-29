using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports; //gestione parta seriale
using System.Windows.Threading;
using System.Threading;//thread per la lettura della porta

namespace Seriale
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //delegate void SetTextCallback(string text);
        private Thread readThread = null;
        

        public MainWindow()
        {
            InitializeComponent();
            //per far comparire sulla comboBox le com attive sul PC e poter selezionare quella d'interesse
            string[] serialPorts = SerialPort.GetPortNames();
            foreach (string comPort in serialPorts)
                cbSerialPort.Items.Add(comPort);

            cbSerialSpeed.Items.Add("9600");
          
        }

     
          SerialPort com;
         
       

        // creo l'oggetto riferito alla connessione seriale
        private void btnOpenPort_Click(object sender, RoutedEventArgs e)
        {
            com = new SerialPort(cbSerialPort.Text, Convert.ToInt16(cbSerialSpeed.Text), Parity.None, 8, StopBits.One);


            com.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            com.Open();
           // readThread = new Thread(new ThreadStart(this.Read));
           // readThread.Start();
         
           
            btnOpenPort.IsEnabled = false;
           
              }

        private delegate void SetTextDeleg(string text);

        void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            string data = com.ReadLine();
            // Invokes the delegate on the UI thread, and sends the data that was received to the invoked method.
            // ---- The "si_DataReceived" method will be executed on the UI thread which allows populating of the textbox.
           this.Dispatcher.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { data });
            
            
        }

        string[] autovelox;
        int[] velocità = new int[10];
        string[] verso = new string[10];
        int i=0;
        double media=0;
        

        private void si_DataReceived(string data) { 
            
            txtMsg.Text = data;

            if (i < 10)
            {
                     autovelox = data.Split(new char[] {',','\r','\n'});

                     verso[i] = autovelox[0];
                     velocità[i] = Convert.ToInt16(autovelox[1]);
                i++;

                if (i > 9)
                {

                    for (int j = 0; j < 10; j++)
                        media = media + velocità[j];

                    media = media / 10;

                    lbl1.Content = "La velocità media riscontrata è " + media;
                
                }
            }

            


        }

     

       

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            com.Close();
        }

        
    }

        }
