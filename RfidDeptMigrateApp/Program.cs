using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RfidDeptMigrateApp
{
    class Program
    {
        OdbcConnection con1 = new OdbcConnection("Driver=Sybase ASE ODBC Driver;SRVR=production;DB=kfnl;UID=sa;PWd=sybase1;");
        // OdbcConnection con2 = new OdbcConnection("Driver=Sybase ASE ODBC Driver;SRVR=idris;DB=kfnl;UID=sa;charset=cp1256");
        static string path = @"C:\Admin\text.txt";
        static StreamWriter sw = new StreamWriter(path);
        static void Main(string[] args)
        {
            new Program().ASD();
        }

        private void printDemarcText()
        {

            try
            {
                con1.Open();
                // drop BasicEmpData
                //fill BasicEmpData3 

                OdbcCommand commandItem = new OdbcCommand(@"select bib# , text from bib_trans_02_07_2019", con1);
                // processed like '%السعوديه%'
                // command.Parameters.AddWithValue();
                DataSet resultsItem = new DataSet();
                OdbcDataAdapter usersAdapterItem = new OdbcDataAdapter(commandItem);
                usersAdapterItem.Fill(resultsItem);
                DataTable dtItem = resultsItem.Tables[0];

                Encoding ansiEncoding = Encoding.GetEncoding(1256);

                foreach (DataRow rowItem in dtItem.Rows)
                {

                    OdbcCommand commandItem2 = new OdbcCommand(@"select bib#, text from bib where tag = '260' and bib# =? ", con1);
                    commandItem2.Parameters.Add(new OdbcParameter("@bib#", int.Parse(rowItem["bib#"].ToString())));
                    // processed like '%السعوديه%'
                    // command.Parameters.AddWithValue();
                    DataSet resultsItem2 = new DataSet();
                    OdbcDataAdapter usersAdapterItem2 = new OdbcDataAdapter(commandItem2);
                    usersAdapterItem2.Fill(resultsItem2);
                    DataTable dtItem2 = resultsItem2.Tables[0];
                    foreach (DataRow rowItem2 in dtItem2.Rows)
                    {
                        string title = "";
                        string pub1 = "";
                        string city1 = "";
                        string pub2 = "";
                        string city2 = "";
                        string hDate = "";
                        string gDate = "";



                        if (getRequredTag(rowItem["text"].ToString(), 'a').Count >= 1)
                        {
                            title = getRequredTag(rowItem["text"].ToString(), 'a')[0];
                        }

                        if (getRequredTag(rowItem2["text"].ToString(), 'a').Count >= 1)
                        {
                            pub1 = getRequredTag(rowItem2["text"].ToString(), 'a')[0];
                        }

                        if (getRequredTag(rowItem2["text"].ToString(), 'b').Count >= 1)
                        {
                            city1 = getRequredTag(rowItem2["text"].ToString(), 'b')[0];
                        }

                        if (getRequredTag(rowItem2["text"].ToString(), 'a').Count >= 2)
                        {
                            pub2 = getRequredTag(rowItem2["text"].ToString(), 'a')[1];
                        }

                        if (getRequredTag(rowItem2["text"].ToString(), 'b').Count >= 2)
                        {
                            city2 = getRequredTag(rowItem2["text"].ToString(), 'b')[1];
                        }

                        if (getRequredTag(rowItem2["text"].ToString(), 'c').Count >= 1)
                        {
                            hDate = getRequredTag(rowItem2["text"].ToString(), 'c')[0];
                        }

                        if (getRequredTag(rowItem2["text"].ToString(), 'm').Count >= 1)
                        {
                            gDate = getRequredTag(rowItem2["text"].ToString(), 'm')[0];
                        }
                        OdbcCommand commandItem3 = new OdbcCommand(@"insert into bib_trans_02_07_2019_ord (bib#, gdate, hdate) values(?,?,?) ", con1);
                        commandItem3.Parameters.Add(new OdbcParameter("@bib#", int.Parse(rowItem["bib#"].ToString())));
                        if (gDate.Length > 4)
                        {
                            commandItem3.Parameters.Add(new OdbcParameter("@bib#", dateReplace(gDate)));
                        }
                        else
                        {
                            commandItem3.Parameters.Add(new OdbcParameter("@bib#", ""));
                        }
                        if (hDate.Length > 4)
                        {
                            commandItem3.Parameters.Add(new OdbcParameter("@bib#", dateReplace(hDate)));
                        }
                        else
                        {
                            commandItem3.Parameters.Add(new OdbcParameter("@bib#", ""));
                        }

                        commandItem3.ExecuteNonQuery();

                        sw.WriteLine(rowItem["bib#"].ToString() + "\t" +
                            title + "\t" +
                            pub1 + "\t" +
                            city1 + "\t" +
                            pub2 + "\t" +
                            city2 + "\t" +
                            hDate + "\t" +
                            gDate);
                        Console.WriteLine(rowItem["bib#"].ToString() + "\t" + getRequredTag(rowItem["text"].ToString(), 'a'));
                    }


                }
                sw.Flush();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine();
                Console.ReadLine();
            }
            finally
            {
                con1.Close();
            }
        }
        string bib = "";
        private void ASD()
        {
            try
            {
                con1.Open();
                // drop BasicEmpData
                //fill BasicEmpData3 

                OdbcCommand commandItem2 = new OdbcCommand(@"select bib#,text,tagord from tempDept2", con1);
                // processed like '%السعوديه%'
                // command.Parameters.AddWithValue();
                DataSet resultsItem2 = new DataSet();
                OdbcDataAdapter usersAdapterItem2 = new OdbcDataAdapter(commandItem2);
                usersAdapterItem2.Fill(resultsItem2);
                DataTable dtItem2 = resultsItem2.Tables[0];
                string temp = "";
                string cleandept = "";
                foreach (DataRow rowItem2 in dtItem2.Rows)
                {
                    bib = rowItem2["bib#"].ToString();
                    Console.WriteLine(bib);
                    if (getRequredTag(rowItem2["text"].ToString(), 'a').Count >= 1)
                    {
                        temp = getRequredTag(rowItem2["text"].ToString(), 'a')[0];
                    }
                    else
                    {
                        OdbcCommand commandItem = new OdbcCommand(@"delete from  tempDept2 where bib# = ? and tagord = ?", con1);
                        commandItem.Parameters.Add(new OdbcParameter("@bib#", int.Parse(rowItem2["bib#"].ToString())));
                        commandItem.Parameters.Add(new OdbcParameter("@bib#", int.Parse(rowItem2["tagord"].ToString())));
                        commandItem.ExecuteNonQuery();
                    }


                    cleandept = cleanDeptNo(temp);
                    if (!string.IsNullOrEmpty(cleandept))
                    {
                        OdbcCommand commandItem = new OdbcCommand(@"update tempDept2 set text = ? where bib# = ? and tagord = ?", con1);
                        commandItem.Parameters.Add(new OdbcParameter("@text", cleandept));
                        commandItem.Parameters.Add(new OdbcParameter("@bib#", int.Parse(rowItem2["bib#"].ToString())));
                        commandItem.Parameters.Add(new OdbcParameter("@bib#", int.Parse(rowItem2["tagord"].ToString())));
                        commandItem.ExecuteNonQuery();
                    }
                    else
                    {
                        OdbcCommand commandItem = new OdbcCommand(@"delete from  tempDept2 where bib# = ? and tagord = ?", con1);
                        commandItem.Parameters.Add(new OdbcParameter("@bib#", int.Parse(rowItem2["bib#"].ToString())));
                        commandItem.Parameters.Add(new OdbcParameter("@bib#", int.Parse(rowItem2["tagord"].ToString())));
                        commandItem.ExecuteNonQuery();
                    }

                }


                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(bib);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine();
                Console.ReadLine();
            }
            finally
            {
                con1.Close();
            }
        }
        public string formatDate(string date)
        {
            string year;
            string mon;
            string day;
            year = date.Substring(0, 4);
            mon = date.Substring(4, 2);
            day = date.Substring(6, 2);
            return year + "/" + mon + "/" + day;
        }
        public string createTapSpace(int count)
        {
            string space = "";
            for (int i = 0; i < count; i++)
            {
                space += "\t";
            }
            return space;
        }
        public string dateReplace(string text)
        {
            Encoding ansiEncoding = Encoding.GetEncoding(1256);
            List<byte> bytes = ansiEncoding.GetBytes(text).ToList<byte>();
            List<byte> decodedTextArr = new List<byte>();

            for (int i = 0; i < bytes.Count; i++)
            {
                if (bytes[i] <= 57 && bytes[i] >= 48)
                {
                    decodedTextArr.Add(bytes[i]);
                }
            }
            if (ansiEncoding.GetChars(decodedTextArr.ToArray()).Length >= 4)
            {
                return new string(ansiEncoding.GetChars(decodedTextArr.ToArray())).Substring(0, 4);
            }
            else
            {
                return new string(ansiEncoding.GetChars(decodedTextArr.ToArray()));
            }


        }

        public List<string> getRequredTag(string text, char tag)
        {
            Encoding ansiEncoding = Encoding.GetEncoding(1256);
            byte[] bytes = ansiEncoding.GetBytes(text);
            bool isSelectedText = true;
            List<string> selectedTextArr = new List<string>();
            string selectedText = "";

            for (int i = 0; i < bytes.Length; i++)
            {

                if (bytes[i] == 31 && bytes[i + 1] == ansiEncoding.GetBytes(new char[] { tag })[0])
                {
                    isSelectedText = true;

                }
                else if (bytes[i] == 31 && bytes[i + 1] != ansiEncoding.GetBytes(new char[] { tag })[0])
                {
                    if (!String.IsNullOrEmpty(selectedText))
                    {
                        selectedTextArr.Add(selectedText);
                        selectedText = "";
                    }
                    isSelectedText = false;
                }
                else if (i == bytes.Length - 1 && isSelectedText) //if is the selected tag = last tag in the text 
                {
                    if (!String.IsNullOrEmpty(selectedText))
                    {
                        selectedText = selectedText + ansiEncoding.GetChars(new byte[] { bytes[i] })[0];
                        selectedTextArr.Add(selectedText);
                    }
                }
                else
                {
                    if (isSelectedText)
                    {
                        selectedText = selectedText + ansiEncoding.GetChars(new byte[] { bytes[i] })[0];

                    }
                }
            }
            return DBMarcToClient(selectedTextArr);
        }
        public List<string> DBMarcToClient(List<string> textArr)
        {
            byte AscChr;
            string Text2;
            List<string> decodedTextArr = new List<string>();
            Text2 = string.Empty;
            foreach (var text in textArr)
            {


                if (text.Trim().Length > 0)
                {
                    Encoding ansiEncoding = Encoding.GetEncoding(1256);
                    byte[] bytes = ansiEncoding.GetBytes(text);
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        AscChr = bytes[i];
                        switch (AscChr)
                        {
                            case 16:
                                //Text2 = Text2;
                                break;
                            case 136:
                                //Text2 = Text2;
                                break;
                            case 137:
                                //Text2 = Text2;
                                break;
                            case 172:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 161 })[0];
                                break;
                            case 173:
                                Text2 = Text2 + "-";
                                break;
                            case 187:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 186 })[0];
                                break;
                            case 219:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 215 })[0];
                                break;
                            case 215:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 216 })[0];
                                break;
                            case 216:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 217 })[0];
                                break;
                            case 217:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 218 })[0];
                                break;
                            case 218:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 219 })[0];
                                break;
                            case 225:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 221 })[0];
                                break;
                            case 226:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 222 })[0];
                                break;
                            case 227:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 223 })[0];
                                break;
                            case 221:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 224 })[0];
                                break;
                            case 228:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 225 })[0];
                                break;
                            case 222:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 226 })[0];
                                break;
                            case 229:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 227 })[0];
                                break;
                            case 230:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 228 })[0];
                                break;
                            case 231:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 229 })[0];
                                break;
                            case 232:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 230 })[0];
                                break;
                            case 223:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 231 })[0];
                                break;
                            case 224:
                                Text2 = Text2 + "_";
                                break;
                            case 236:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 233 })[0];
                                break;
                            case 237:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 234 })[0];
                                break;
                            case 233:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 236 })[0];
                                break;
                            case 234:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { 237 })[0];
                                break;
                            default:
                                Text2 = Text2 + ansiEncoding.GetChars(new byte[] { AscChr })[0];
                                break;
                        }
                    }

                }
                decodedTextArr.Add(Text2);
                Text2 = "";
            }
            return decodedTextArr;
        }
        public string cleanDeptNo(string text)
        {
            Encoding ansiEncoding = Encoding.GetEncoding(1256);
            text = text.Replace(" ", "");
            List<Byte> bytes = ansiEncoding.GetBytes(text).ToList<Byte>();
            List<Byte> tempBytes = ansiEncoding.GetBytes(text).ToList<Byte>(); ;

            foreach (var b in bytes)
            {
                if (b > 57 || b < 47)
                {
                    tempBytes.Remove(b);
                }
            }


            return ansiEncoding.GetString(tempBytes.ToArray());
        }
    }
}
