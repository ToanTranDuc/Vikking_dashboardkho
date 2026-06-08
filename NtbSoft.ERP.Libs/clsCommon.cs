using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Ports;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Xml;


namespace NtbSoft.ERP.Libs
{
    public class clsCommon
    {
        public static DateTime ConvertDateTime(string MMddyyyy)
        {
            return DateTime.Parse(Convert.ToDateTime(MMddyyyy).ToString("MM/dd/yyyy"));
        }
    }
    //    public static bool IsUnicode(string input)
    //    {
    //        var asciiBytesCount = Encoding.ASCII.GetByteCount(input);
    //        var unicodBytesCount = Encoding.UTF8.GetByteCount(input);
    //        return asciiBytesCount != unicodBytesCount;
    //    }

    //    public static string RemoveVnSymbol(string strVietnamese)
    //    {
    //        while (strVietnamese.IndexOfAny("áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ".ToCharArray()) != -1)
    //        {
    //            int index = strVietnamese.IndexOfAny("áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ".ToCharArray());
    //            object value = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ".IndexOf(strVietnamese[index]);
    //            strVietnamese = strVietnamese.Replace(strVietnamese[index], "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY"[Convert.ToInt32(value)]);
    //        }
    //        return strVietnamese;
    //    }

    //    public static IEnumerable<string> Split(string str, int chunkSize)
    //    {
    //        return Enumerable.Range(0, str.Length / chunkSize)
    //            .Select(i => str.Substring(i * chunkSize, chunkSize));
    //    }

    //    public static int fromFourBytes(List<string> Array)
    //    {
    //        if (Array.Count < 4)
    //            return 0;
    //                    int _temp1 = int.Parse(Array[0]) > 256 ? 0 : int.Parse(Array[0]),
    //            _temp2 = int.Parse(Array[1]) > 256 ? 0 : int.Parse(Array[1]),
    //            _temp3 = int.Parse(Array[2]) > 256 ? 0 : int.Parse(Array[2]),
    //            _temp4 = int.Parse(Array[3]) > 256 ? 0 : int.Parse(Array[3]);
    //        _temp1 <<= 24;
    //        _temp2 <<= 16;
    //        _temp3 <<= 8;

    //        return _temp1 | _temp2 | _temp3 | _temp4;
    //    }


    //    public static string toStringSerial(string SerialNo)
    //    {
    //        char separator = '-';
    //        //if (string.IsNullOrWhiteSpace(SerialNo))
    //        if (string.IsNullOrEmpty(SerialNo))
    //            return "";
    //        List<string> buffers = Split(SerialNo, 3).ToList();
    //        StringBuilder _sb = new StringBuilder();
    //        foreach (var item in buffers)
    //        {
    //            string _Str = formatString(item.ToString(), '0', 3) + separator;
    //            _sb.Append(_Str);
    //        }

    //        return _sb.ToString().TrimEnd(new char[] { separator });
    //    }

    //    /// <summary>
    //    /// Exp: Pattern=0,MaxLength=2 => cho ra chuỗi mới "000"
    //    /// </summary>
    //    /// <param name="text"></param>
    //    /// <param name="Pattern"></param>
    //    /// <param name="MaxLength"></param>
    //    /// <returns></returns>
    //    private static string formatString(string text, char Pattern, int MaxLength)
    //    {
    //        //Chuỗi định dạng (Ví dụ: c=0, MaxLenght=2 => cho ra chuỗi định dạng "000")
    //        string _strPattern = new string(Pattern, MaxLength);//.Replace("+", c.ToString());
    //        //if (string.IsNullOrWhiteSpace(text))
    //        if (string.IsNullOrEmpty(text))
    //            return _strPattern;

    //        int _txtLength = text.Length;   //Chiều dài của chỗi định muốn định dạng
    //        int _diff = MaxLength - _txtLength;
    //        if (_diff < 0)
    //            _diff = MaxLength - 1;

    //        //Tạo chuỗi mới
    //        string _NewText = _strPattern.Substring(0, _diff) + text;
    //        return _NewText;
    //    }

    //    /// <summary>
    //    /// Loại bỏ khoảng trắng
    //    /// </summary>
    //    /// <param name="input"></param>
    //    /// <returns></returns>
    //    public static string RemoveWhitespace(string input)
    //    {
    //        return new string(input.ToCharArray()
    //            .Where(c => !char.IsWhiteSpace(c))
    //            .ToArray());
    //    }

    //    /// <summary>
    //    /// Loại bỏ một ký tự bất kỳ
    //    /// </summary>
    //    /// <param name="input"></param>
    //    /// <param name="ch"></param>
    //    /// <returns></returns>
    //    public static string RemoveWhitespace(string input,char ch)
    //    {
    //        return new string(input.ToCharArray()
    //            .Where(c => !c.Equals(ch))
    //            .ToArray());
    //    }

    //    public static bool IsIPv4(string ipAddress)
    //    {
    //        return System.Text.RegularExpressions.Regex.IsMatch(ipAddress, @"^\d{1,3}(\-\d{1,3}){3}$") &&
    //            ipAddress.Split('-').SingleOrDefault(s => int.Parse(s) > 255) == null;
    //    }

    //    private static char[] _SpecialCharacters = new char[] 
    //                        {'`','~','@','$','%','^','&','*','(',')','_','{','}',':',';','\'','\"','\\','|','<','>','?','/',',','.' };

    //    public static int FindSpecialCharacter(char c)
    //    {
    //        return Array.IndexOf(_SpecialCharacters, c);
            
    //    }

    //    public static string StrTimestamp
    //    {
    //        get
    //        {
    //            DateTime utcNow = DateTime.UtcNow;
    //            long utcNowAsLong = utcNow.ToBinary();
    //            byte[] utcNowBytes = BitConverter.GetBytes(utcNowAsLong);

    //            return ToHex(utcNowBytes);
    //        }
    //    }
    //    public static string ToHex(byte[] _Buffers)
    //    {
    //        if (_Buffers == null || _Buffers.Length == 0)
    //            return "0x0";
    //        return "0x" + string.Concat(_Buffers.Select(b => b.ToString("X2")).ToArray());
    //    }

    //    public static string RepalceSpecialCharacters(string strInput)
    //    {
    //        string[] _pattern = new string[] { "`", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "/", "?", ",", "'", "\\", "|" };

    //        string[] ss = strInput.Split(_pattern, StringSplitOptions.RemoveEmptyEntries);

    //        return string.Join("-", ss);
    //    }

    //    public static string ConvertToUnSign(string text)
    //    {
    //        for (int i = 33; i < 48; i++)
    //        {
    //            text = text.Replace(((char)i).ToString(), "");
    //        }

    //        for (int i = 58; i < 65; i++)
    //        {
    //            text = text.Replace(((char)i).ToString(), "");
    //        }

    //        for (int i = 91; i < 97; i++)
    //        {
    //            text = text.Replace(((char)i).ToString(), "");
    //        }
    //        for (int i = 123; i < 127; i++)
    //        {
    //            text = text.Replace(((char)i).ToString(), "");
    //        }
    //        text = text.Replace(" ", "_");
    //        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\p{IsCombiningDiacriticalMarks}+");
    //        string strFormD = text.Normalize(System.Text.NormalizationForm.FormD);
    //        return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
    //    }
    //    /// <summary>
    //    /// Lam tron len
    //    /// </summary>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    public static int ConvertToInt(float value)
    //    {
    //        string _result = value.ToString("#");
    //        if (string.IsNullOrEmpty(_result))
    //            return 0;
            
    //        return int.Parse(_result);
    //    }

    //    /// <summary>
    //    /// 3.3
    //    /// </summary>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    public static string ConvertToString(float value)
    //    {
    //        string _msg = "0";
    //        if (value % 1 > 0)
    //        {
    //            int _index = value.ToString().IndexOf('.');
    //            if (_index == -1)
    //                _index = value.ToString().IndexOf(',');
    //            if (_index == -1)
    //                return "0";
    //            _msg = String.Format("{0:0.0}", value);
    //        }
    //        else if (value % 1 == 0)
    //            _msg = value.ToString("#");
    //        else
    //            _msg = String.Format("{0:0.0}", value);

    //        //if (string.IsNullOrWhiteSpace(_msg))
    //        if (string.IsNullOrEmpty(_msg))
    //            return "0";

    //        return _msg;
    //    }

    //    public static string CreateXML(Object ClassObject)
    //    {
    //        if (ClassObject == null) return "";
    //        XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
    //        // Initializes a new instance of the XmlDocument class.          
    //        XmlSerializer xmlSerializer = new XmlSerializer(ClassObject.GetType());
    //        // Creates a stream whose backing store is memory. 
    //        using (MemoryStream xmlStream = new MemoryStream())
    //        {
    //            xmlSerializer.Serialize(xmlStream, ClassObject);
    //            xmlStream.Position = 0;
    //            //Loads the XML document from the specified string.
    //            xmlDoc.Load(xmlStream);
    //            return xmlDoc.InnerXml;
    //        }
    //    }

    //    public static Object CreateObject(string XMLString, object ClassObject)
    //    {
    //        XmlSerializer oXmlSerializer = new XmlSerializer(ClassObject.GetType());
    //        //The StringReader will be the stream holder for the existing XML file 
    //        ClassObject = oXmlSerializer.Deserialize(new StringReader(XMLString));
    //        //initially deserialized, the data is represented by an object without a defined type 
    //        return ClassObject;
    //    }


    //    public static TimeSpan GetTime24()
    //    {
    //        return TimeSpan.Parse(DateTime.Now.ToString(clsConsts.FormatHour24));
    //    }

    //    public static TimeSpan GetTime24(int Seconds)
    //    {
    //        return TimeSpan.Parse(DateTime.Now.AddSeconds(Seconds).ToString(clsConsts.FormatHour24));
    //    }
    //    public static byte[] SerializeObject(object obj)
    //    {
    //        try
    //        {
    //            MemoryStream _setram = new MemoryStream();
    //            BinaryFormatter _formater = new BinaryFormatter();
    //            _formater.Serialize(_setram, obj);

    //            byte[] buffers = _setram.ToArray();
    //            _setram.Close();
    //            _setram.Dispose();

    //            return buffers;
    //        }
    //        catch { return null; }
    //    }

    //    public static object DeSerializeObject(byte[] buffers)
    //    {
    //        MemoryStream mStream = null;
    //        BinaryFormatter _Formatter = null;
    //        try
    //        {
    //            mStream = new MemoryStream(buffers);
    //            _Formatter = new BinaryFormatter();

    //            return _Formatter.Deserialize(mStream);
    //        }
    //        catch
    //        {
    //            if (mStream != null)
    //            {
    //                mStream.Flush();
    //                mStream.Close();
    //            }
    //            return null;
    //        }
    //        finally
    //        {
    //            if (mStream != null)
    //            {
    //                mStream.Flush();
    //                mStream.Close();
    //            }
    //        }
    //    }
        
    //    public static List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
    //    {
    //        List<DateTime> allDates = new List<DateTime>();
    //        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
    //            allDates.Add(date);
    //        return allDates;

    //    }
    //    public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
    //    {
    //        if (endDate < startDate)
    //            throw new ArgumentException("endDate must be greater than or equal to startDate");
    //        while (DateTime.Compare(startDate,endDate)<=0)
    //        {
    //            yield return startDate;
    //            startDate = startDate.AddDays(1);
    //        }
    //    }

    //    //public static IEnumerable<DateTime> DateRangesAdd1(DateTime fromDate, DateTime toDate)
    //    //{
    //    //    return Enumerable.Range(0, toDate.AddDays(1).Subtract(fromDate).Days + 1).Select(d => fromDate.AddDays(d));
    //    //}

    //    public static string LocalIPAddress()
    //    {
    //        string _IP = null;

    //        // Resolves a host name or IP address to an IPHostEntry instance.
    //        // IPHostEntry - Provides a container class for Internet host address information. 
    //        System.Net.IPHostEntry _IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

    //        // IPAddress class contains the address of a computer on an IP network. 
    //        foreach (System.Net.IPAddress _IPAddress in _IPHostEntry.AddressList)
    //        {
    //            // InterNetwork indicates that an IP version 4 address is expected 
    //            // when a Socket connects to an endpoint
    //            if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
    //            {
    //                _IP = _IPAddress.ToString();
    //            }
    //        }
    //        return _IP;
    //    }
    //    /// <summary>
    //    /// Lay ngay cuoi thang
    //    /// </summary>
    //    /// <param name="Today"></param>
    //    /// <returns></returns>
    //    public static DateTime LastDayOfMonth(DateTime Today)
    //    {
    //        return new DateTime(Today.Year, Today.Month, 1).AddMonths(1).AddDays(-1);
    //    }

    //    public static DateTime FirstDayOfMoth(DateTime Today)
    //    {
    //        return new DateTime(Today.Year, Today.Month, 1);
    //    }

    //    public bool writeObject(string pathFolder, string fileName, object obj)
    //    {
    //        if (!Directory.Exists(pathFolder))
    //            Directory.CreateDirectory(pathFolder);

    //        string _fullName = pathFolder + "\\" + fileName;
    //        if (File.Exists(_fullName))
    //            File.Delete(_fullName);

    //        FileStream _stream = new FileStream(_fullName, FileMode.Create);
    //        try
    //        {
    //            BinaryFormatter _formmater = new BinaryFormatter();
    //            _formmater.Serialize(_stream, obj);
    //            _stream.Close();
    //            return true;
    //        }
    //        catch
    //        {
    //            _stream.Close();
    //            return false;
    //        }

    //    }

    //    public object readObject(string fileName)
    //    {
    //        if (!File.Exists(fileName))
    //            return null;
    //        FileStream _stream = null;
    //        try
    //        {
    //            _stream = new FileStream(fileName, FileMode.Open);
    //            BinaryFormatter _formatter = new BinaryFormatter();
    //            object obj = _formatter.Deserialize(_stream);
    //            _stream.Close();
    //            GC.Collect();
    //            return obj;
    //        }
    //        catch
    //        {
    //            if (_stream != null)
    //                _stream.Close();
    //            return null;
    //        }
    //        finally { _stream = null; }
    //    }

    //    public bool writeXML(string fileName, object obj)
    //    {
    //        StreamWriter writer = null;
    //        try
    //        {
    //            FileInfo _fInfo = new FileInfo(fileName);
    //            if (!Directory.Exists(_fInfo.Directory.FullName))
    //                Directory.CreateDirectory(_fInfo.Directory.FullName);
    //            if (_fInfo.Exists)
    //                _fInfo.Delete();
    //            XmlSerializer xml = new XmlSerializer(typeof(object));
    //            writer = new StreamWriter(fileName);
    //            xml.Serialize(writer, obj);
    //            writer.Close();

    //            return true;
    //        }
    //        catch {
    //            if (writer != null)
    //                writer.Close();
    //            return false; }
    //    }

    //    public object readXML(string fileName)
    //    {
    //        StreamReader reader = null;
    //        try
    //        {
    //            XmlSerializer xml = new XmlSerializer(typeof(object));
    //            reader = new StreamReader(fileName);
    //            object obj = xml.Deserialize(reader);
    //            reader.Close();
    //            return obj;
    //        }
    //        catch { if (reader != null)reader.Close(); return null; }
    //    }

    //    public static void WriteLog(string errorPosition, string logMessage)
    //    {
    //        if (System.Web.HttpContext.Current == null)
    //            return;

    //        string path = System.Web.HttpContext.Current.Server.MapPath("~/Logs/");
    //        // check if directory exists
    //        if (!Directory.Exists(path))
    //        {
    //            Directory.CreateDirectory(path);
    //        }
    //        path = path + DateTime.Today.ToString("dd-MM-yyyy") + ".log";
    //        // check if file exist
    //        if (!File.Exists(path))
    //        {
    //            File.Create(path).Dispose();
    //        }

    //        FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write);
    //        StreamWriter streamWriter = new StreamWriter(fileStream);
    //        streamWriter.WriteLine("<Log Entry> : {0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
    //        streamWriter.WriteLine("<Position>  : " + errorPosition);
    //        streamWriter.WriteLine("<Message>   : " + logMessage);
    //        streamWriter.WriteLine("--------------------------------------------------------------------------------------");
    //        streamWriter.Flush();
    //        streamWriter.Close();
    //        fileStream.Close();
    //    }

    //    public static void WriteLog(string FileName, string errorPosition, string logMessage)
    //    {
    //        if (System.Web.HttpContext.Current == null)
    //            return;

            
    //        string _Folder = string.Format("~/Logs/{0}/", DateTime.Today.ToString("dd_MM_yyyy"));
    //        //string path = System.Web.HttpContext.Current.Server.MapPath("~/Logs/");
    //        string path = System.Web.HttpContext.Current.Server.MapPath(_Folder);

    //        // check if directory exists
    //        if (!Directory.Exists(path))
    //        {
    //            Directory.CreateDirectory(path);
    //        }
    //        //path = path + DateTime.Today.ToString("dd-MM-yyyy") + ".log";
    //        path = path + FileName + ".log";
    //        // check if file exist
    //        if (!File.Exists(path))
    //        {
    //            File.Create(path).Dispose();
    //        }

    //        FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write);
    //        StreamWriter streamWriter = new StreamWriter(fileStream);
    //        streamWriter.WriteLine("<Log Entry> : {0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
    //        streamWriter.WriteLine("<Position>  : " + errorPosition);
    //        streamWriter.WriteLine("<Message>   : " + logMessage);
    //        streamWriter.WriteLine("--------------------------------------------------------------------------------------");
    //        streamWriter.Flush();
    //        streamWriter.Close();
    //        fileStream.Close();

    //        //try
    //        //{
    //        //    string _FilderLog = string.Format(@"C:\Program Files\LogLineName\{0}\", DateTime.Today.ToString("dd_MM_yyyy"));
    //        //    if (!Directory.Exists(_FilderLog))
    //        //    {
    //        //        Directory.CreateDirectory(_FilderLog);
    //        //    }
    //        //    string _destFileName = _FilderLog + FileName + ".log";
    //        //    File.Copy(path, _destFileName, true);
    //        //}
    //        //catch { }
    //    }

    //    public static string ReadLogFile(string LineX, string StrDate)
    //    {
    //        if (System.Web.HttpContext.Current == null)
    //            return "";
    //        string _Folder = string.Format("~/Logs/{0}/", StrDate);
    //        string path = System.Web.HttpContext.Current.Server.MapPath(_Folder);
    //        string FileName = string.Format("Line_{0}", LineX);
    //        path = path + FileName + ".log";
    //        // check if file exist
    //        if (!File.Exists(path))
    //        {
    //            return "File không tồn tại!";
    //        }

    //        return File.ReadAllText(path);
    //    }

    //    /// <summary>
    //    /// On Windows Forms
    //    /// </summary>
    //    /// <param name="name"></param>
    //    /// <param name="errorMessage"></param>
    //    public static void WriteError(string name, string errorMessage)
    //    {
    //        try
    //        {
    //            string folder = System.Windows.Forms.Application.StartupPath + @"\Error\";
    //            string path = folder + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";
    //            if (!Directory.Exists(folder))
    //                Directory.CreateDirectory(folder);
    //            if (!File.Exists(path))
    //            {
    //                File.Create(path).Close();
    //            }
    //            using (StreamWriter w = File.AppendText(path))
    //            {
    //                w.WriteLine("\r\nLog Entry : ");
    //                w.WriteLine("{0}", DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
    //                string err = "Error in: " + name +
    //                              ". Error Message: " + errorMessage;
    //                w.WriteLine(err);
    //                w.WriteLine("__________________________");
    //                w.Flush();
    //                w.Close();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            WriteError("WriteError", ex.Message);
    //            return;
    //        }
    //    }

    //    public static void WriteLogFileAspNet(string name, string errorMessage)
    //    {
    //        try
    //        {
    //            if (System.Web.HttpContext.Current == null)
    //                return;
    //            string path = System.Web.HttpContext.Current.Server.MapPath("~/Error/");
    //            // check if directory exists
    //            if (!Directory.Exists(path))
    //            {
    //                Directory.CreateDirectory(path);
    //            }
    //            path = path + DateTime.Today.ToString("dd-MM-yyyy") + ".log";
    //            // check if file exist
    //            if (!File.Exists(path))
    //            {
    //                File.Create(path).Dispose();
    //            }

    //            using (StreamWriter w = File.AppendText(path))
    //            {
    //                w.WriteLine("\r\nLog Entry : ");
    //                w.WriteLine("{0}", DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
    //                string err = "- " + name +
    //                              ": " + errorMessage;
    //                w.WriteLine(err);
    //                w.WriteLine("__________________________");
    //                w.Flush();
    //                w.Close();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return;
    //        }
    //    }


    //    public static void WriteLogFileAspNet(System.Web.HttpServerUtility Server, string name, string errorMessage)
    //    {
    //        try
    //        {
    //            if (Server == null)
    //                return;
    //            string path = Server.MapPath("~/Error/");
    //            // check if directory exists
    //            if (!Directory.Exists(path))
    //            {
    //                Directory.CreateDirectory(path);
    //            }
    //            path = path + DateTime.Today.ToString("dd-MM-yyyy") + ".log";
    //            // check if file exist
    //            if (!File.Exists(path))
    //            {
    //                File.Create(path).Dispose();
    //            }

    //            using (StreamWriter w = File.AppendText(path))
    //            {
    //                w.WriteLine("\r\nLog Entry : ");
    //                w.WriteLine("{0}", DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
    //                string err = "- " + name +
    //                              " " + errorMessage;
    //                w.WriteLine(err);
    //                w.WriteLine("__________________________");
    //                w.Flush();
    //                w.Close();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return;
    //        }
    //    }

    //    /// <summary>
    //    /// Log the error and return (ASP.net)
    //    /// </summary>
    //    /// <param name="ex">The ee.</param>
    //    /// <param name="userFriendlyError">The user friendly error.</param>
    //    /// <returns></returns>
    //    public static string LogErrorToLogFile(Exception ex, string userFriendlyError)
    //    {
    //        try
    //        {
    //            if (System.Web.HttpContext.Current == null)
    //                return string.Empty;

    //            string path = System.Web.HttpContext.Current.Server.MapPath("~/Error/");
    //            // check if directory exists
    //            if (!Directory.Exists(path))
    //            {
    //                Directory.CreateDirectory(path);
    //            }
    //            path = path + DateTime.Today.ToString("dd-MM-yyyy") + ".log";
    //            // check if file exist
    //            if (!File.Exists(path))
    //            {
    //                File.Create(path).Dispose();
    //            }
    //            // log the error now
    //            using (StreamWriter writer = File.AppendText(path))
    //            {
    //                string error = "\r\n\"" + userFriendlyError + "\". Log written at : " + DateTime.Now.ToString() +
    //                "\r\nError occured on page : " + System.Web.HttpContext.Current.Request.Url.ToString() +
    //                "\r\n\r\nHere is the actual error :\n" + ex.ToString();
    //                writer.WriteLine(error);
    //                writer.WriteLine("==========================================");
    //                writer.Flush();
    //                writer.Close();
    //            }
    //            return userFriendlyError;
    //        }
    //        catch
    //        {
    //            return string.Empty;
    //        }
    //    }

    //    public static byte[] toBytes(int number)
    //    {
    //        string str = number.ToString();
    //        byte[] tmps = new byte[str.Length];

    //        for (int i = 0; i < str.Length; i++)
    //        {
    //            int kq = number % 10;
    //            number = number / 10;
    //            tmps[str.Length - 1 - i] = (byte)kq;
    //        }

    //        return tmps;
    //    }

    //    public static int getWeek(DateTime dt)
    //    {
    //        System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("no");
    //        System.Globalization.Calendar calen = culInfo.Calendar;

    //        return calen.GetWeekOfYear(dt, culInfo.DateTimeFormat.CalendarWeekRule, culInfo.DateTimeFormat.FirstDayOfWeek);
    //    }

    //    private void sendFile(string remoteHotIP, int remoteHotPort, string longFileName, string shortFileName, int number)
    //    {
    //        if (number <= 0)
    //            return;
    //        byte[] _fileNameByte = null;
    //        byte[] _fileData = null;
    //        byte[] _clientData = null;
    //        byte[] _fileNameLen = null;
    //        TcpClient _clientSocket = null;
    //        NetworkStream _networkStream = null;
    //        try
    //        {
    //            if (!string.IsNullOrEmpty(remoteHotIP))
    //            {
    //                _fileNameByte = Encoding.Unicode.GetBytes(shortFileName);
    //                _fileData = File.ReadAllBytes(longFileName);
    //                _clientData = new byte[4 + _fileNameByte.Length + _fileData.Length];
    //                _fileNameLen = BitConverter.GetBytes(_fileNameByte.Length);

    //                _fileNameLen.CopyTo(_clientData, 0);
    //                _fileNameByte.CopyTo(_clientData, 4);
    //                _fileData.CopyTo(_clientData, 4 + _fileNameByte.Length);

    //                _clientSocket = new TcpClient(remoteHotIP, remoteHotPort);
    //                _networkStream = _clientSocket.GetStream();
    //                _networkStream.Write(_clientData, 0, _clientData.GetLength(0));
    //                _networkStream.Flush();
    //                _networkStream.Close();
    //                _clientSocket.Close();
    //                _networkStream.Dispose();
    //                _networkStream = null;
    //                _clientSocket = null;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            clsCommon.WriteError("sendFile", ex.Message);
    //            return;
    //        }
    //        finally
    //        {
    //            _fileNameByte = null;
    //            _fileData = null;
    //            _clientData = null;
    //            _fileNameLen = null;
    //            _clientSocket = null;
    //            _networkStream = null;
    //        }
    //    }

    //    /// <summary>
    //    /// Convert to n bytes
    //    /// </summary>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    public static byte[] toNByteMa_Hang(string value)
    //    {
    //        if (string.IsNullOrEmpty(value))
    //            return new byte[] { };

    //        char[] id =value.ToCharArray();
    //        byte[] data = new byte[id.Length+1];
    //        data[0] = (byte)id.Length;

    //        for (int i = 0; i < id.Length; i++)
    //        {
    //            data[i + 1] = (byte)id[i];
    //        }

    //        return data; //n bytes
    //    }

    //    /// <summary>
    //    /// Convert number to 2 bytes
    //    /// </summary>
    //    /// <param name="number"></param>
    //    /// <returns></returns>
    //    public static byte[] twoBytes(int number)
    //    {
    //        byte[] data = new byte[2];

    //        data[0] = (byte)(number / 0x100);
    //        data[1] = (byte)(number % 0x100);

    //        return data;
    //    }

    //    /// <summary>
    //    /// Convert number to 3 bytes
    //    /// </summary>
    //    /// <param name="number"></param>
    //    /// <returns></returns>
    //    public static byte[] threeBytes(int number)
    //    {
    //        byte[] data = new byte[3];
    //        data[0] = (byte)(number / 0x10000);
    //        data[1] = (byte)((number % 0x10000) / 0x100);
    //        data[2] = (byte)((number % 0x10000) % 0x100);

    //        return data;
    //    }

    //    /// <summary>
    //    /// Convert number to 3 bytes
    //    /// </summary>
    //    /// <param name="number"></param>
    //    /// <returns></returns>
    //    public static byte[] fourBytes(int number)
    //    {
    //        byte[] data = new byte[4];
    //        data[0] = (byte)0;
    //        data[1] = (byte)(number / 0x10000);
    //        data[2] = (byte)((number % 0x10000) / 0x100);
    //        data[3] = (byte)((number % 0x10000) % 0x100);
    //        return data;
    //    }
    //    /// <summary>
    //    /// Exp: 98.92%
    //    /// </summary>
    //    /// <param name="number"></param>
    //    /// <returns></returns>
    //    public static string ConverPercent(float number)
    //    {
    //        string[] temps = number.ToString("0.00%").Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
    //        if (temps.Length >= 2)
    //        {
    //            int k = int.Parse(temps[1].TrimEnd(new char[1] { '%' }));
    //            if (k.Equals(0))
    //                return string.Format("{0}%", temps[0]);
    //            else
    //                return number.ToString("0.00%");
    //        }
    //        return string.Empty;
    //    }

    //    /// <summary>
    //    /// Exp: 98.9%
    //    /// </summary>
    //    /// <param name="number"></param>
    //    /// <returns></returns>
    //    public static string ConverPercent2(float number)
    //    {
    //        string msg = (number * 100).ToString();
            
    //        return  msg.Substring(0, msg.Length - 1);
    //    }
    //}

    //public struct strWarningLight
    //{
    //    public string LineX { get; set; }
    //    public string Name { get; set; }
    //    public int RedSeconds { get; set; }
    //    public int RedTimes { get; set; }

    //    public int YellowSeconds { get; set; }
    //    public int YellowTimes { get; set; }

    //    public int GreendSeconds { get; set; }
    //    public int GreendTimes { get; set; }

    //    public int BlueSeconds { get; set; }
    //    public int BlueTimes { get; set; }
    //    public int NDSX_KH { get; set; }
    //    public string MaDvcs { get; set; }
    //};
}
