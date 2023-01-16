using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;


namespace CompressionProgram {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            TestOutput();
        }

        public void TestOutput() {
            int version = 3;            //version 1 read decompress and save // version 2 only read no save // version 3 decompression
            String filepath;                    // Setting file paths here for now later done in the GUI
            String SaveFilepath;
            Program.MainCompression test = new Program.MainCompression();
            if(version == 3) {
                filepath = "C:\\compressionFileTest\\test2.txt";                    // Setting file paths here for now later done in the GUI
                SaveFilepath = "C:\\compressionFileTest\\test3.txt";
            }
            else {
                filepath = "C:\\compressionFileTest\\test.txt";                    // Setting file paths here for now later done in the GUI
                SaveFilepath = "C:\\compressionFileTest\\test2.txt";
            }
            long n = 0;
            List<byte> content = new List<Byte>();
            test.setFilepath(filepath);  // Setting the filepath
            test.setSaveFilepath(SaveFilepath); // Setting where to save the compressed file and how the created file is named
            while(true) {
                content = test.PseudoMain(n, version);
                if(content == null) { //Abbruch falls keine Werte in dem returnten Array
                    text1.Text += "\nReading complete\n\n";
                    text1.Text += "Dictionary used:\n";
                    int b = 1;

                    if(test.getDictionary().Count() > 3000) {

                        text1.Text += "Size too big to display in string Size:" + test.getDictionary().Count() + "\n";

                    }
                    else {
                        Trace.WriteLine("---------------Count: " + test.getDictionary().Count() + "----------------");
                        foreach(String i in test.getDictionary()) { // Ausgabe des Dictionaries und der Compresscodierung die in diesem Fall benutzt wurde

                            int dicMaxBitLength = test.getdicMaxBitLength();

                            if(b < 10) {
                                text1.Text += "  ";
                            }
                            text1.Text += b + ". Entry: ";
                            //for (int g = 0; i.ToString().Length < dicMaxBitLength && g < dicMaxBitLength - i.ToString().Length; g++)
                            //{
                            //    text1.Text += "0";
                            //}
                            text1.Text += i.ToString() + " Binary used to compress this entry: ";
                            //for (int g = 0; Convert.ToString(b, 2).Length < dicMaxBitLength && g < dicMaxBitLength - Convert.ToString(b, 2).Length; g++)
                            //{
                            //    text1.Text += "0";
                            //}
                            text1.Text += Convert.ToString(b, 2) + "\n";
                            b++;
                        }
                    }
                    FileInfo fi = new FileInfo(filepath);
                    text1.Text += "\nOriginal File Size in KB: " + String.Format("{0:0.000}", (double)(fi.Length / 1024.00)) + "\n";
                    if(version == 1 || version == 3) {
                        fi = new FileInfo(SaveFilepath);
                        text1.Text += "Compressed File Size in KB: " + String.Format("{0:0.000}", (double)(fi.Length / 1024.00)) + "\n";
                    }


                    return;   // Programm wird beendet
                }
                if(content.Count == 1) { // Abbruch da Datei nicht auffindbar

                    text1.Text = "Dateipfad ungültig";

                    return;  // Programm wird beendet
                }
                else if(content.Count == 2) { // Abbruch da die Speicherdatei bereits existiert

                    text1.Text = "Der Dateiname der zum speichern eingegeben wurde existiert bereits in diesem Pfad";

                    return;  // Programm wird beendet
                }
                else { // Ausgame des Binarystreams
                    int i = 1;

                    if(content.Count < 10000) {

                        foreach(byte b in content) {

                            //Textfeld ist zum testen der richtigkeit des binairy streams
                            // Dies ist nur bei kleinen files möglich ~1mb max

                            text1.Text += Convert.ToString(b, 2).PadLeft(8, '0') + " ";   // Byte zu Binary

                            if(i % 8 == 0 && i != 0) {
                                text1.Text += "\n";
                            }

                            i++;
                        }
                    }


                    text1.Text += "\n------End of Bytearray Nr. " + (n + 1) + "--------\n";
                }
                n++;
                content.Clear();
            }
        }
    }
}


