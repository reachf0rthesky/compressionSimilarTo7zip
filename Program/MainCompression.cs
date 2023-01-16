using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CompressionProgram.Program {



    public class MainCompression {

        private List<String> dictionary = new List<String>();
        private String filepath = "";
        private String saveFilepath = "";
        private List<Byte> byteList = new List<byte>();
        private int dicMaxBitLength = 5;
        private FileStream fileSave;
        private bool validSaveChecked = false;
        private int version;

        public List<Byte> PseudoMain(long amount, int version) {  //Methode die alle anderen Methoden der Klasse zusammenfügt

            this.version = version;

            fillStaticDic();

            if(validSavePathCheck() == 1) {

                return byteList = new List<byte>() { 0, 0 };

            }

            return DataReader(filepath, amount);

        }

        private int validSavePathCheck() { //Checken ob es bereits eine Datei mit dem Gleichen Namen in dem Pfad gibt und bricht Operation mit Fehlercode
                                           //ab falls dies der Fall ist.

            if(validSaveChecked == true || version == 2)  // Does nothing if Filepath already checked
            {
                return 0;
            }

            //if (File.Exists(saveFilepath))
            //{

            //    validSaveChecked = true;
            //    return 1;
            //}
            //else
            //{

            fileSave = File.Create(saveFilepath);
            validSaveChecked = true;
            return 2;

            //}

        }

        private void fillStaticDic() {  // Generation of static dictionary with the size x times y
            if(dictionary.Count == 0) {
                for(int i = 0; i < Math.Pow(2, 4); i++) {
                    dictionary.Add(Convert.ToString(i, 2).PadLeft(dicMaxBitLength, '0'));
                    Trace.WriteLine("dictinary add: " + Convert.ToString(i, 2).PadLeft(4, '0'));
                }
            }


        }


        private List<Byte> DataReader(String path, long amount) { //Bytes aus der Datei lesen und in einen Bytearray speichern dieser wird dann returned
                                                                  // Falls die File nicht gefunden wird, wird eine Arraylist mit Size 1 zurückgegeben

            if(File.Exists(path)) {


                using(FileStream file = File.OpenRead(path)) { // Öffnet Datei und füllt einen Byte Array mit den Daten und gibt diesen an die Bytereader Methode
                                                               // weiter
                    byte[] byteArray = new byte[536870912 / 2];  // Array Größe ca. 530MB*2 Peak Memory usage: 1,5 gb
                    file.Position = (long)(amount * byteArray.Length);

                    if(file.Position >= File.OpenRead(path).Length) { // Postion in dem Filestream ist größer als die Filestreamlänge = Auslesen zu ende
                        byteArray = null;
                        if(version == 1) {
                            fileSave.Close();
                        }

                        return null;

                    }

                    if(file.Position + byteArray.Length > File.OpenRead(path).Length) { // Filestream lesen ist im letzen Durchlauf

                        file.Read(byteArray, 0, byteArray.Length);

                        byteList = byteArray.ToList();  // In List kopieren

                        int value = (int)(File.OpenRead(path).Length - (amount * byteArray.Length));

                        //Trace.WriteLine("----fgssf");

                        byteArray = null;

                        byteList.RemoveRange(value, byteList.Count - value);  // Removing 0 byte's at the end

                        //DataSaver(byteList.ToArray()); // Writing data to new file
                        if(version == 1) {
                            Compression(byteList);
                        }
                        else if(version == 3) {
                            Decompression(byteList);
                        }
                        file.Close();
                        fileSave.Close();
                        return byteList;

                    }

                    file.Read(byteArray, 0, byteArray.Length);  // Normales Filestream lesen von der Anzahl bits = Größe des Bytearray
                    byteList = byteArray.ToList();  // In List kopieren
                                                    //DataSaver(byteList.ToArray()); // Writing data to new file
                    if(version == 1) {
                        Compression(byteList);
                    }
                    else if(version == 3) {
                        Decompression(byteList);
                    }
                }

            }
            else {

                byteList = new List<byte>() { 0 };

            }

            return byteList;

        }

        private void Compression(List<byte> byteList) { //Daten werden bearbeitet und komprimiert

            string toCompress = "";
            string compressed = "";
            string toWrite = "";
            int count = 0;

            while(true) {    // Solange die byteList Einträge hat wird hier komprimiert

                if(Math.Pow(2, dicMaxBitLength) < dictionary.Count) {  // Bits in denen gespeichert wird hochzählen

                    dicMaxBitLength++;

                    for(int i = 0; i < 16; i++) {
                        dictionary[i] = dictionary[i].PadLeft(dicMaxBitLength, '0');
                    }

                }


                while(toCompress.Length < 10000 && byteList.Count > 0) { //Der zu koprimirende Sting wird aufgefüllt
                    toCompress += Convert.ToString(byteList[0], 2).PadLeft(8, '0');
                    byteList.RemoveRange(0, 1);
                }

                //Start variablen der kompression
                if(toCompress.Length > 0) {
                    if(toCompress == "01100100011001100111001101110011") {
                        string test = "";
                    }
                    compressed = toCompress.Substring(0, 4);
                    toCompress = toCompress.Substring(4);

                }

                // Hier passiert die compression
                while(toCompress.Length > 0) {

                    if(!dictionary.Contains(compressed + toCompress.Substring(0, 4))) {
                        dictionary.Add(compressed + toCompress.Substring(0, 4));
                        break;
                    }
                    else if(dictionary.Contains(compressed.PadLeft(dicMaxBitLength, '0'))) {
                        compressed += toCompress.Substring(0, 4);
                        toCompress = toCompress.Substring(4);
                    }

                }
                //checking what gets written into the new file
                if(toCompress.Length > 0 && toWrite.Length >= 0) {
                    Trace.WriteLine("---------" + compressed + " dictionary entry: " + dictionary[dictionary.Count - 1]);
                }

                toWrite += Convert.ToString(dictionary.FindIndex(x => x == compressed.PadLeft(dicMaxBitLength, '0')), 2).PadLeft(dicMaxBitLength, '0');


                while(toWrite.Length >= 8) {
                    count++;
                    if(count == 140) {
                        string test = "";
                    }
                    Trace.WriteLine(count + ". Byte Written: " + toWrite.Substring(0, 8) + " in bytes :" + Convert.ToByte(toWrite.Substring(0, 8), 2));
                    fileSave.WriteByte(Convert.ToByte(toWrite.Substring(0, 8), 2));
                    toWrite = toWrite.Substring(8);

                    if(toCompress.Length == 0 && toWrite.Length < 8) {
                        if(toWrite.Length == 0) {
                            return;
                        }
                        Trace.WriteLine("Byte Written: " + toWrite.Substring(0, toWrite.Length).PadRight(8, '0') + " in bytes :" + Convert.ToByte(toWrite.Substring(0, toWrite.Length), 2));
                        fileSave.WriteByte(Convert.ToByte(toWrite.Substring(0, toWrite.Length).PadRight(8, '0'), 2));
                        return;
                    }
                }




            }



        }

        public void Decompression(List<byte> byteList) {

            int counter = 5;
            string toDecompress = "";
            string decompressed = "";
            string toWrite = "";
            bool skip = false;
            int count = 0;
            //toDecompress += Convert.ToString(byteList[0], 2).PadLeft(8, '0');
            //byteList.RemoveRange(0, 1);
            string recentDecompressed = "";

            while(true) { // Solange die byteList Einträge hat wird hier komprimiert

                if(Math.Pow(2, counter) - 1 < dictionary.Count) {  // Bits in denen gespeichert wird hochzählen
                    counter++;
                    dicMaxBitLength++;
                    for(int i = 0; i < 16; i++) {
                        dictionary[i] = dictionary[i].PadLeft(dicMaxBitLength, '0');
                    }
                }




                //Start variablen der kompression 

                while(toDecompress.Length < 10000 && byteList.Count > 0) {  //Der zu komprimirende Sting wird aufgefüllt
                    toDecompress += Convert.ToString(byteList[0], 2).PadLeft(8, '0');
                    byteList.RemoveRange(0, 1);
                }

                if(toDecompress.Length < 8 && toWrite.Length == 0) {
                    bool check = true;
                    foreach(Char c in toDecompress) {
                        if(c == '1') {
                            check = false;
                        }
                    }
                    if(check == true) {
                        return;
                    }
                }



                if(toDecompress.Length > 6) {  // Falls nix mehr zu komprimieren werden keine start variablen mehr aufgesetzt
                    if(Convert.ToInt32(toDecompress.Substring(0, dicMaxBitLength), 2) == dictionary.Count) {
                        decompressed = recentDecompressed + decompressed.Substring(0, 4);
                        dictionary.Add(recentDecompressed + decompressed.Substring(0, 4));
                        skip = true;
                    }
                    else {
                        decompressed = dictionary[Convert.ToInt32(toDecompress.Substring(0, dicMaxBitLength), 2)];
                    }

                    toDecompress = toDecompress.Substring(dicMaxBitLength);
                }


                // Hier passiert die decompression
                if(skip == false) {
                    while(toDecompress.Length > 4) {
                        if(recentDecompressed == "")  // Erster Durchlauf kein Dic Eintrag
                        {
                            decompressed = decompressed.Substring(dicMaxBitLength - 4);
                            break;
                        }
                        else if(dictionary.Contains(decompressed) && (!dictionary.Contains(recentDecompressed + decompressed.Substring(0, dicMaxBitLength)))) // es versucht 00001 zu finden hat aber nur 0001
                          {       // Falls das dic decompre

                            if(decompressed.Length < 8) {
                                decompressed = decompressed.Substring(dicMaxBitLength - 4);
                            }

                            dictionary.Add(recentDecompressed + decompressed.Substring(0, 4));
                            break;


                        }
                        else {

                            toDecompress = toDecompress.Substring(dicMaxBitLength);
                        }

                    }
                }
                else {
                    skip = false;
                }

                if(recentDecompressed != "" && toDecompress.Length > 4) {
                    Trace.WriteLine(dictionary.Count + ". ---------" + decompressed + " dictionary entry: " + dictionary[dictionary.Count - 1]);
                }

                if(toDecompress.Length > 1 && toDecompress.Contains("1")) {
                    recentDecompressed = decompressed;
                    toWrite += decompressed;
                }

                if(toDecompress.Length < 4 && toWrite.Length <= 8) {
                    if(toWrite.Length == 0) {
                        return;
                    }
                    Trace.WriteLine("Ending Byte Written: " + toWrite.Substring(0, toWrite.Length) + " in bytes :" + Convert.ToByte(toWrite.Substring(0, toWrite.Length), 2));

                    fileSave.WriteByte(Convert.ToByte(toWrite.Substring(0, toWrite.Length), 2));
                    return;
                }
                else if(toWrite.Length >= 8) {
                    while(toWrite.Length >= 8) {
                        count++;
                        Trace.WriteLine("Count: " + count + " Byte Written: " + toWrite.Substring(0, 8) + " in bytes :" + Convert.ToByte(toWrite.Substring(0, 8), 2));
                        if(count == 190) {
                            string test = "df";
                        }
                        fileSave.WriteByte(Convert.ToByte(toWrite.Substring(0, 8), 2));
                        toWrite = toWrite.Substring(8);
                    }
                }



            }


        }
        private void DataSaver(byte[] byteStream) { // Daten werden nach Bearbeitung in einer anderen Datei gespeichert

            fileSave.Write(byteStream, 0, byteStream.Length);

        }
        // GETTER SETTER
        public void setFilepath(String path) {

            filepath = path;

        }

        public void setSaveFilepath(String path) {

            saveFilepath = path;

        }

        public List<String> getDictionary() {
            return dictionary;
        }

        public int getdicMaxBitLength() {

            return dicMaxBitLength;

        }
    }




}
