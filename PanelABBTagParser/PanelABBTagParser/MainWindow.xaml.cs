using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace PanelABBTagParser
{
 
    public partial class MainWindow : Window
    {
        string inputFilePath = ""; //.txt dosya uzantılı recete database
        string outputFilePath = ""; //.csv dosya uzantılı nereye yükleneceği
        String filePath = ""; //.txt dosya uzantılı alarm database
        string exportedPath = ""; //.xml dosya uzantılı nereye yükleneceği
        OpenFileDialog DialogFile = new OpenFileDialog();

        String alarmString = "    <alarm eventBuffer=\"AlarmBuffer1\" " +
     "logToEventArchive=\"true\" " +
     "eventType=\"14\" subType=\"1\" " +
     "storeAlarmInfo=\"true\">\r\n        " +
     "<name>Alarm1</name>\r\n        " +
     "<groups></groups>\r\n        " +
     "<source>Application/Alarmlar/AL01_GranulSurucuAriza</source>\r\n        " +
     "<alarmType>bitMaskAlarm</alarmType>\r\n        <bitMask>1</bitMask>\r\n        " +
     "<enableTag></enableTag>\r\n        <remoteAck></remoteAck>\r\n        " +
     "<ackNotify></ackNotify>\r\n        <touchAckNotify></touchAckNotify>\r\n        " +
     "<enabled>true</enabled>\r\n        <requireAck>false</requireAck>\r\n        " +
     "<blinkTxt>false</blinkTxt>\r\n        <requireReset>false</requireReset>\r\n       " +
     " <severity>1</severity>\r\n        <priority>3</priority>\r\n        <logMask>76</logMask>\r\n  " +
     "      <notifyMask>76</notifyMask>\r\n        <actionMask>1</actionMask>\r\n       " +
     " <printMask>1</printMask>\r\n        <customFields>\r\n            <customField_1>\r\n           " +
     "     <L1 langName=\"Lang1\"></L1>\r\n            </customField_1>\r\n          " +
     "  <customField_2>\r\n                <L1 langName=\"Lang1\"></L1>\r\n            </customField_2>\r\n    " +
     "    </customFields>\r\n        <colors>\r\n            <ackTxtColor>#ff0000</ackTxtColor>\r\n          " +
     "  <ackBgColor>#ffff00</ackBgColor>\r\n            <disabledTxtColor>#000000</disabledTxtColor>\r\n        " +
     "    <disabledBgColor>#ffffff</disabledBgColor>\r\n            <triggeredTxtColor>#000000</triggeredTxtColor>\r\n          " +
     "  <triggeredBgColor>#ffffff</triggeredBgColor>\r\n            <notTriggeredTxtColor>#000000</notTriggeredTxtColor>\r\n         " +
     "   <notTriggeredBgColor>#ffffff</notTriggeredBgColor>\r\n            <triggeredAckedTxtColor>#000000</triggeredAckedTxtColor>\r\n      " +
     "      <triggeredAckedBgColor>#ffffff</triggeredAckedBgColor>\r\n            <triggeredNotAckedTxtColor>#000000</triggeredNotAckedTxtColor>\r\n    " +
     "        <triggeredNotAckedBgColor>#ffffff</triggeredNotAckedBgColor>\r\n            <notTriggeredAckedTxtColor>#000000</notTriggeredAckedTxtColor>\r\n   " +
     "         <notTriggeredAckedBgColor>#ffffff</notTriggeredAckedBgColor>\r\n            " +
     "<notTriggeredNotAckedTxtColor>#000000</notTriggeredNotAckedTxtColor>\r\n            " +
     "<notTriggeredNotAckedBgColor>#ffffff</notTriggeredNotAckedBgColor>\r\n        </colors>\r\n      " +
     "  <actions/>\r\n        <useractions/>\r\n        <description>\r\n          " +
     "  <L1 langName=\"Lang1\">AL01_GranulSurucuAriza</L1>\r\n        </description>\r\n      " +
     "  <enableAudit auditBuff=\"\" subT=\"1\" eventT=\"18\">false</enableAudit>\r\n    </alarm>";


        public MainWindow()
        {
            InitializeComponent();
        }




        private void InputAlarmReferenceFile(object sender, RoutedEventArgs e)
        {
            DialogFile.Title = "Hangi dosyadan aktarılacağını seçiniz(.txt)";
            DialogFile.ShowDialog(this);
            filePath = DialogFile.FileName;
        }

        private void InputAlarmExportFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Hangi dosyaya aktarılacağını seçiniz (.csv)";

            // Optional: Set default extension and filter
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

            // Show the dialog to the user
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the file path chosen by the user
                exportedPath = saveFileDialog.FileName;

                // Create the file if it doesn’t already exist
                if (!File.Exists(exportedPath))
                {
                    File.Create(exportedPath).Dispose(); // Dispose to release the file handle
                    MessageBox.Show($"File created at: {exportedPath}");
                }
                else
                {
                    MessageBox.Show("File already exists.");
                }
            }
        }
        private void ExportAlarmFile(object sender, RoutedEventArgs e)
        {

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    if (!(filePath == "" && exportedPath == ""))
                    {
                        File.AppendAllText(exportedPath, "<?xml version = \"1.0\" encoding = \"UTF-8\" ?> <alarms>");
                        int i = 0;
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] values = line.Split(';'); // Split by semicolon
                            string valuenow = "";
                            foreach (var value in values)
                            {
                                if ((value != "") && (value.Contains("/")==false))
                                {
                                    value.Replace(" ", "");
                                    if (value.IndexOf(":") >= 0)
                                    {
                                        valuenow = value.Substring(0, value.IndexOf(":"));
                                        File.AppendAllText(exportedPath, alarmString.Replace("AL01_GranulSurucuAriza", valuenow).Replace("<name>Alarm1</name>", "<name>Alarm" + i + "</name>"));
                                        i++;
                                    }
                                    else { File.AppendAllText(exportedPath, alarmString.Replace("AL01_GranulSurucuAriza", value).Replace("<name>Alarm1</name>", "<name>Alarm" + i + "</name>")); i++; }
                                    
                                }
                                
                                
                            }
                        }
                        File.AppendAllText(exportedPath, "</alarms>");
                        MessageBox.Show("Alarmlarınızın .txt formatından istenen .XML formatına dönüştürülmesi başarıyla tamamlandı!");
                    }
                     };
                
            }
            catch(Exception err)
            {
                if (exportedPath != null)
                {
                    MessageBox.Show("Referans değişken dosyasını seçiniz");
                    InputAlarmReferenceFile(null, null);
                }
                else
                {
                    MessageBox.Show("Lütfen hangi dosyadan hangi dosyaya aktarılacağını seçiniz.(İlki .txt ikincisi .xml uzantılı)");
                    InputAlarmReferenceFile(null, null);
                    InputAlarmExportFile(null, null);
                }
                
            }
            
        }

        // Path to your existing text file with lines to read


        // Read all lines from the input file


        private void InputReceipeReferenceFile(object sender, RoutedEventArgs e)
        {
            DialogFile.Title = "Hangi dosyadan aktarılacağını seçiniz (.txt)";
            DialogFile.ShowDialog(this);
            inputFilePath = DialogFile.FileName;
        }
        private void InputReceipeExportFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Hangi dosyaya aktarılacağını seçiniz (.csv)";

            // Optional: Set default extension and filter
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            // Show the dialog to the user
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the file path chosen by the user
                outputFilePath = saveFileDialog.FileName;

                // Create the file if it doesn’t already exist
                if (!File.Exists(outputFilePath))
                {
                    File.Create(outputFilePath).Dispose(); // Dispose to release the file handle
                    MessageBox.Show($"File created at: {outputFilePath}");
                }
                else
                {
                    MessageBox.Show("File already exists.");
                }
            }

        }
        private void ExportReceipeFile(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] lines = File.ReadAllLines(inputFilePath);
                int lineCount = 0;
                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    String formattedLine = "\"#Delimiter:,\"\r\nRecipeName:,Recipe0\r\nsetSize:,0\r\nid:,1\r\nArray Support:,true\r\nElementName,Tag,Array Index,Index Tag,";
                    writer.WriteLine(formattedLine);
                    String addedLines = "";
                    // Loop through each line and format it
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string currentLine = lines[i];
                        
                         
                        
                        // Write the formatted line to the output file
                        if ((currentLine != "") && currentLine.Contains("//")!=true)
                        {
                            if (currentLine.IndexOf(":") >= 0)
                            {
                                
                                currentLine = currentLine.Substring(0, currentLine.IndexOf(":"));
                                
                                addedLines = $"\"Element{lineCount}\",\"{currentLine.Replace(" ", "")}\",\"-1\",\"\"" + ",";
                                writer.WriteLine(addedLines);
                                
                                lineCount++;
                            }
                            else {
                                addedLines = $"\"Element{lineCount}\",\"{currentLine.Replace(" ", "")}\",\"-1\",\"\"" + ","; 
                                writer.WriteLine(addedLines); 
                                lineCount++; }

                        }
                        
                    }
                   

                }

                MessageBox.Show("Reçetelerinizin .txt formatından istenen .csv formatına dönüştürülmesi başarıyla tamamlandı!");
            }catch(Exception err) { if (outputFilePath != "") { MessageBox.Show(err.Message);
                    InputReceipeReferenceFile(null, null);
                }
            else { MessageBox.Show("Lütfen hangi dosyadan alınacağını ve hangi dosyaya aktarılacağını seçiniz.");
                    InputReceipeReferenceFile(null, null);
                    InputReceipeExportFile(null, null); }
            };
        }

        private void alarmSifirla(object sender, RoutedEventArgs e)
        {
            filePath = null;
            exportedPath = null;
        }

        private void receteSifirla(object sender, RoutedEventArgs e)
        {
            inputFilePath = null;
            outputFilePath = null;
        }
    }
}
    


