using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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

        #region Variable definitions
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
        string addedLines = "";
        int lineCount = 0;
        #endregion

        #region Common functions
        void writeToline(StreamWriter Writer)
        {
            
            if (addedLines != "") { 
                
            Writer.WriteLine(addedLines);
                addedLines = "";
            lineCount++;
        }
        }

        enum Operation
        {
            Alarm,
            Recipe
        }

        void addingToLine(String substr, Operation OP)
        {
            string input = inputRecete.Text;
            switch (OP)
            {
                case Operation.Alarm:
                    if (substr != "" && OP == Operation.Alarm)
                        addedLines = alarmString.Replace("AL01_GranulSurucuAriza", input + substr).Replace("<name>Alarm1</name>", "<name>Alarm" + lineCount + "</name>");
                    break;
                case Operation.Recipe:
                    if (substr != "")
                        addedLines = $"\"Element{lineCount}\",\"{input + substr.Replace(" ","")}\",\"-1\",\"\"" + ",";
                    break;
                default:
                    return;
            }



        }

        string currentLineStr(string input)
        {
            bool insideComment = false;
            int currentLineIndex = 0;
            int startIndex = 0;

            string result = "";
            while (currentLineIndex < input.Length)
            {
                if (!insideComment && currentLineIndex == input.IndexOf("(*", startIndex))
                {

                    startIndex = input.IndexOf("(*", startIndex) + 1;
                    insideComment = true;
                    currentLineIndex = currentLineIndex + 2;
                    result = result + ";";
                }
                else if (insideComment && currentLineIndex == input.IndexOf("*)", startIndex))
                {
                    startIndex = input.IndexOf("*)", startIndex) + 1;
                    insideComment = false;
                    currentLineIndex = currentLineIndex + 2;
                }

                else if (!insideComment && currentLineIndex != input.IndexOf("*)"))
                {

                    result = result + input[currentLineIndex];
                    currentLineIndex++;
                }
                else
                {
                    currentLineIndex++;

                }

            }
            return result;
        }

        string currentLineSt(string input)
        {
            bool insideComment = false;
            int currentLineIndex = 0;
            int startIndex = 0;

            string result = "";
            while (currentLineIndex < input.Length)
            {
                if (!insideComment && currentLineIndex == input.IndexOf("(*", startIndex))
                {

                    startIndex = input.IndexOf("(*", startIndex) + 1;
                    insideComment = true;
                    currentLineIndex = currentLineIndex + 2;
                    result = result + ";";
                }
                else if (insideComment && currentLineIndex == input.IndexOf("*)", startIndex))
                {
                    startIndex = input.IndexOf("*)", startIndex) + 1;
                    insideComment = false;
                    currentLineIndex = currentLineIndex + 2;
                }

                else if (!insideComment && currentLineIndex != input.IndexOf("*)"))
                {

                    result = result + input[currentLineIndex];
                    currentLineIndex++;
                }
                else
                {
                    currentLineIndex++;

                }

            }
            return result;
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Alarm
        private void InputAlarmReferenceFile(object sender, RoutedEventArgs e)
        {
            DialogFile.Title = "Hangi dosyadan aktarılacağını seçiniz(.txt)";
            DialogFile.ShowDialog(this);
            filePath = DialogFile.FileName;
            alarmInput.Content= filePath;
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
                alarmOutput.Content = exportedPath;

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
                lineCount = 0;
                using (StreamWriter WritingAlarm = new StreamWriter(exportedPath))
                {
                    
                    string[] lines = File.ReadAllLines(filePath);

                    if (!(filePath == "" && exportedPath == ""))
                    { int lineCount = 0;

                        WritingAlarm.WriteLine("<?xml version = \"1.0\" encoding = \"UTF-8\" ?> <alarms>");

                        for (int i = 0; i < lines.Length; i++)
                        { string currentLine = lines[i].Replace("\t","");
                            bool useInstead = false;
                            currentLine = currentLineSt(currentLine);
                            string PieceOfLine = currentLine;
                            string[] PiecesOfLine = { };
                            PiecesOfLine = PieceOfLine.Split(';');
                            
                          
                            if (currentLine.Contains(";"))// bulunulan satırda ; isareti var ise ; e göre ayrılır. ve hepsi farklı satır olarak eklenir.
                            {
                                useInstead = true;
                                PiecesOfLine = PieceOfLine.Split(';');
                                for (int j = 0; j < PiecesOfLine.Length; j++)
                                {
                                    if ((PiecesOfLine[j].Contains(" ")) && (PiecesOfLine[j] != ""))
                                    {
                                        if (PiecesOfLine[j].Contains("//"))
                                        {
                                            PiecesOfLine[j] = PiecesOfLine[j].Substring(0, PiecesOfLine[j].IndexOf("//"));
                                            addingToLine(PiecesOfLine[j], Operation.Alarm);
                                            if (PiecesOfLine[j] != " " && PiecesOfLine[j] != "")
                                            {
                                                writeToline(WritingAlarm);
                                            }
                                            break;

                                        }
                                        else if (PiecesOfLine[j].Contains(":"))
                                        {
                                            PiecesOfLine[j] = PiecesOfLine[j].Substring(0, PiecesOfLine[j].IndexOf(":"));
                                            addingToLine(PiecesOfLine[j].Replace(" ",""), Operation.Alarm);
                                            writeToline(WritingAlarm);
                                        }


                                        else
                                        {
                                            addingToLine(PiecesOfLine[j].Replace(" ", ""), Operation.Alarm);
                                            writeToline(WritingAlarm);

                                        }
                                    }
                                }
                            }
                            if (currentLine.Contains(";") == false)
                            {
                                useInstead = false;
                            }
                            if ((currentLine != "") && currentLine != ("\t")&& (useInstead == false))
                            {
                                if (currentLine.Contains("//"))
                                {
                                    currentLine = currentLine.Substring(0, currentLine.IndexOf("//"));
                                }
                                //currentLine.Replace(" ", "");
                                if (currentLine.IndexOf(":") >= 0)
                                {
                                    currentLine = currentLine.Substring(0, currentLine.IndexOf(":"));
                                    addingToLine(currentLine.Replace(" ", ""), Operation.Alarm);
                                    writeToline(WritingAlarm);
                                }
                                else { addingToLine(currentLine.Replace(" ", ""), Operation.Alarm);
                                    writeToline(WritingAlarm);
                                }

                            }


                        }
                    }WritingAlarm.WriteLine("</alarms>");

                }
                
                MessageBox.Show("Alarmlarınızın .txt formatından istenen .XML formatına dönüştürülmesi başarıyla tamamlandı!");
                
            }
            catch (Exception err)
            {
                lineCount = 0;

                MessageBox.Show(err.Message);
                //if (exportedPath == null)
                //{
                //    MessageBox.Show("Referans değişken dosyasını seçiniz");
                //    InputAlarmReferenceFile(null, null);
                //}
                //else
                //{
                //    MessageBox.Show("Lütfen hangi dosyadan hangi dosyaya aktarılacağını seçiniz.(İlki .txt ikincisi .xml uzantılı)");
                //    InputAlarmReferenceFile(null, null);
                //    InputAlarmExportFile(null, null);
                //}

            }
            
        }

        private void alarmSifirla(object sender, RoutedEventArgs e)
        {
            filePath = null;
            exportedPath = null;
        }
        #endregion

        #region Recipe 
        private void InputReceipeReferenceFile(object sender, RoutedEventArgs e)
        {
            DialogFile.Title = "Hangi dosyadan aktarılacağını seçiniz (.txt)";
            DialogFile.ShowDialog(this);
            inputFilePath = DialogFile.FileName;
            receteInput.Content = inputFilePath;
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
                receteOutput.Content = outputFilePath;

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
        string gecici = "";
        private void ExportReceipeFile(object sender, RoutedEventArgs e)
        {
            

            try
            {
                lineCount = 0;
                string[] lines = File.ReadAllLines(inputFilePath);
                
                using (StreamWriter writerRecipe = new StreamWriter(outputFilePath))
                {
                    
                    String formattedLine = "\"#Delimiter:,\"\r\nRecipeName:,Recipe0\r\nsetSize:,0\r\nid:,1\r\nArray Support:,true\r\nElementName,Tag,Array Index,Index Tag,";
                    writerRecipe.WriteLine(formattedLine);
                    String addedLines = "";
                    String[] PiecesOfLine = { };
                    String PieceOfLine = "";
                    bool useInstead = false;
                    int k = 0;
                   

                    
                    //Her satırı gez.
                    
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string currentLine = lines[i].Replace("\t","");
                        currentLine = currentLineStr(currentLine);
                        PieceOfLine = currentLine;
                        if (currentLine.Contains(";"))// bulunulan satırda ; isareti var ise ; e göre ayrılır. ve hepsi farklı satır olarak eklenir.
                        {
                            useInstead = true;
                            PiecesOfLine = PieceOfLine.Split(';');
                            for (k = 0; k < PiecesOfLine.Length; k++)
                            {
                                if ((PiecesOfLine[k]!=" "&& PiecesOfLine[k] != ""))
                                {
                                    if (PiecesOfLine[k].Contains("//"))
                                    {
                                        PiecesOfLine[k] = PiecesOfLine[k].Substring(0, PiecesOfLine[k].IndexOf("//"));
                                        addingToLine(PiecesOfLine[k].Replace(" ","").Replace("\t",""), Operation.Recipe);
                                        if (PiecesOfLine[k] !=" " && PiecesOfLine[k] != "" ) { 
                                        writeToline(writerRecipe);}
                                        break;
                                        
                                    }
                                    else if (PiecesOfLine[k].Contains(":"))
                                    {
                                        PiecesOfLine[k] = PiecesOfLine[k].Substring(0, PiecesOfLine[k].IndexOf(":"));
                                        addingToLine(PiecesOfLine[k].Replace(" ", ""), Operation.Recipe);
                                        writeToline(writerRecipe);
                                    }


                                    else
                                    {
                                        addingToLine(PiecesOfLine[k].Replace(" ", ""), Operation.Recipe);
                                        writeToline(writerRecipe);

                                    }

                                }
                            }
                        }
                        
                          
                          

                         if (currentLine.Contains(";") == false)
                            {
                        useInstead = false; 
                            }

                        // bulunulan line bos degilse // yoksa ve use instead false ise (splitlenme işlemi useinsteadin false olmasıyla giren sartta gerceklesir ardından yukaridaki sart ile falselanır.)
                        if ((currentLine != "") && (useInstead == false))
                        {
                            if (currentLine.Contains("//")) {
                                currentLine = currentLine.Substring(0, currentLine.IndexOf("//"));
                                
                            
                            
                            }
                            if (currentLine.IndexOf(":") >= 0)
                            {
                                
                                currentLine = currentLine.Substring(0, currentLine.IndexOf(":"));
                                addingToLine(currentLine.Replace(" ", ""), Operation.Recipe);
                                //addedLines = $"\"Element{lineCount}\",\"{currentLine.Replace(" ", "")}\",\"-1\",\"\"" + ",";
                                writeToline(writerRecipe);
                            }


                            else
                            {
                                addingToLine(currentLine.Replace(" ", ""), Operation.Recipe);
                               // addedLines = $"\"Element{lineCount}\",\"{currentLine.Replace(" ", "")}\",\"-1\",\"\"" + ",";
                                writeToline(writerRecipe);

                            }
                        }

                    }




                    MessageBox.Show("Reçetelerinizin .txt formatından istenen .csv formatına dönüştürülmesi başarıyla tamamlandı!");
                    lineCount = 0;
                }
            }
            catch (Exception err)
            {
                lineCount = 0;
                if (outputFilePath != "")
                {
                    MessageBox.Show(err.Message);
                    InputReceipeReferenceFile(null, null);
                }
                else
                {
                    MessageBox.Show("Lütfen hangi dosyadan alınacağını ve hangi dosyaya aktarılacağını seçiniz.");
                    InputReceipeReferenceFile(null, null);
                    InputReceipeExportFile(null, null);
                }
            };
        }

        private void receteSifirla(object sender, RoutedEventArgs e)
        {
            inputFilePath = null;
            outputFilePath = null;
        }
        #endregion

        private void inputRecete_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
    


