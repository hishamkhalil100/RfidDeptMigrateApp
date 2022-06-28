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
            new Program().printDemarcText();
        }

        private void printDemarcText()
        {

            try
            {
                con1.Open();
                // drop BasicEmpData
                //fill BasicEmpData3 

                OdbcCommand commandItem0 = new OdbcCommand(@"select  distinct bib# , create_date from DepositedBibs_19_21 where create_date between 17897 and 18261 ", con1);
                // processed like '%السعوديه%'
                // command.Parameters.AddWithValue();
                DataSet resultsItem0 = new DataSet();
                OdbcDataAdapter usersAdapterItem0 = new OdbcDataAdapter(commandItem0);
                usersAdapterItem0.Fill(resultsItem0);
                DataTable dtItem0 = resultsItem0.Tables[0];

                Encoding ansiEncoding = Encoding.GetEncoding(1256);

                foreach (DataRow rowItem0 in dtItem0.Rows)
                {
                    OdbcCommand commandItem = new OdbcCommand(@"select bib# ,tag, text,cat_link_xref# from bib where bib# =?", con1);
                    commandItem.Parameters.Add(new OdbcParameter("@bib#", int.Parse(rowItem0["bib#"].ToString())));
                    // processed like '%السعوديه%'
                    // command.Parameters.AddWithValue();
                    DataSet resultsItem = new DataSet();
                    OdbcDataAdapter usersAdapterItem = new OdbcDataAdapter(commandItem);
                    usersAdapterItem.Fill(resultsItem);
                    DataTable dtItem = resultsItem.Tables[0];


                    string deptNo = "";//
                    string author = "";//
                    string title = "";//
                    string pub1 = "";//
                    string city1 = "";//
                    string pub2 = "";//
                    string city2 = "";
                    string hDate = "";//
                    string gDate = "";//
                    string callNo = "";//
                    string depADate = new DateTime(1970, 1, 1).AddDays(long.Parse(rowItem0["create_date"].ToString())).ToString("yyyy/MM/dd");
                    List<string> subjects = new List<string>();

                    foreach (DataRow rowItem in dtItem.Rows)
                    {



                        //title
                        if (rowItem["tag"].Equals("245"))
                        {
                            title = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'a').FirstOrDefault<string>().RemoveSubFeildLitter();
                            if (string.IsNullOrEmpty(title))
                                title = GetTitleFromTitleTable(int.Parse(rowItem0["bib#"].ToString()));
                        }
                        if (rowItem["tag"].Equals("100"))
                        {
                            if (!string.IsNullOrEmpty(rowItem["cat_link_xref#"].ToString()))
                                author = GetTextFromAuthTable(int.Parse(rowItem["cat_link_xref#"].ToString().CleanEncryptedText()), "100", 'a').FirstOrDefault<string>().RemoveSubFeildLitter();
                            else
                                author = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'a').FirstOrDefault<string>().RemoveSubFeildLitter();
                        }
                        //deptNo
                        if (rowItem["tag"].Equals("017"))
                        {
                            deptNo = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'a').FirstOrDefault<string>().RemoveSubFeildLitter();
                        }
                        //City
                        if (rowItem["tag"].ToString().Equals("260"))
                        {
                            city1 = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'a').FirstOrDefault<string>().RemoveSubFeildLitter();
                            if (getRequredTag(rowItem["text"].ToString(), 'b').Count > 1)
                            {
                                pub1 = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'b')[0].RemoveSubFeildLitter();
                                pub2 = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'b')[1].RemoveSubFeildLitter();
                            }
                            else
                            {
                                pub1 = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'b').FirstOrDefault<string>().RemoveSubFeildLitter();
                            }
                            hDate = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'c').FirstOrDefault<string>().RemoveSubFeildLitter();
                            gDate = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'm').FirstOrDefault<string>().RemoveSubFeildLitter();

                        }

                        if (rowItem["tag"].ToString().Equals("082"))
                        {
                            callNo = getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'a').FirstOrDefault<string>().RemoveSubFeildLitter();
                        }
                        // subjects
                        if (rowItem["tag"].ToString().Equals("650"))
                        {
                            if (!string.IsNullOrEmpty(rowItem["cat_link_xref#"].ToString()))
                                subjects.Add(GetTextFromAuthTable(int.Parse(rowItem["cat_link_xref#"].ToString().CleanEncryptedText()), "150", 'a').FirstOrDefault<string>());
                            else
                                subjects.Add(getRequredTag(rowItem["text"].ToString().CleanEncryptedText(), 'a').FirstOrDefault<string>().RemoveSubFeildLitter());
                        }


                    }
                    string subject = string.Empty;
                    foreach (var sub in subjects)
                    {
                        subject += sub + ",";
                    }
                    sw.WriteLine(rowItem0["bib#"].ToString() + "\t" +
                        deptNo + "\t" +
                        title + "\t" +
                        author + "\t" +
                        pub1 + "\t" +
                        city1 + "\t" +
                        callNo + "\t" +
                        hDate + "\t" +
                        gDate + "\t" +
                        depADate + "\t" +
                        subject);


                }

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
                sw.Flush();
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
        public List<string> GetTextFromAuthTable(int authNo, string tag, char subTag)
        {
            OdbcCommand commandItem2 = new OdbcCommand(@"select auth#, text from auth where tag = ? and auth# =? ", con1);
            commandItem2.Parameters.Add(new OdbcParameter("@tag", tag));
            commandItem2.Parameters.Add(new OdbcParameter("@auth", authNo));
            // processed like '%السعوديه%'
            // command.Parameters.AddWithValue();
            DataSet resultsItem2 = new DataSet();
            OdbcDataAdapter usersAdapterItem2 = new OdbcDataAdapter(commandItem2);
            usersAdapterItem2.Fill(resultsItem2);
            DataTable dtItem2 = resultsItem2.Tables[0];
            List<string> list = new List<string>();
            foreach (DataRow rowItem2 in dtItem2.Rows)
            {
                list.Add(getRequredTag(rowItem2["text"].ToString(), subTag).FirstOrDefault<string>().RemoveSubFeildLitter());
            }
            return list;
        }
        public string GetTitleFromTitleTable(int bibNo)
        {
            OdbcCommand commandItem2 = new OdbcCommand(@"select processed from title where bib# = ?", con1);
            commandItem2.Parameters.Add(new OdbcParameter("@bib#", bibNo));
            // processed like '%السعوديه%'
            // command.Parameters.AddWithValue();
            DataSet resultsItem2 = new DataSet();
            OdbcDataAdapter usersAdapterItem2 = new OdbcDataAdapter(commandItem2);
            usersAdapterItem2.Fill(resultsItem2);
            DataTable dtItem2 = resultsItem2.Tables[0];
            List<string> list = new List<string>();
            foreach (DataRow rowItem2 in dtItem2.Rows)
            {
                return rowItem2["processed"].ToString();
            }
            return "";
        }

    }


    public static class ExtensionMethods
    {
        public static string RemoveSubFeildLitter(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            return text.Remove(0, 1).Replace("", string.Empty);
        }
        public static string CleanEncryptedText(this string text)
        {
            char newLine = (char)10;
            if (string.IsNullOrEmpty(text))
                return "";

            return text.Replace("\r\n", string.Empty).Replace("\r", string.Empty);
        }
    }

}
