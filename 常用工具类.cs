using System;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;

namespace YDL.Utility
{
    public static class Ext
    {
        public static readonly string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private static readonly DateTime dateTimeDefault;

        #region 字符串加密算法

        private static readonly string Key = "YDL-THINK";



        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime != dateTimeDefault ? dateTime.ToString(Constant.DateFormat) : null;
        }

        public static string ToLinkId(string value)
        {
            if (value != null)
            {
                return value.Split(Constant.Chr_Comma)[0];
            }
            return null;
        }

        public static string ToLinkName(string value)
        {
            if (value != null)
            {
                return value.Split(Constant.Chr_Comma)[1];
            }
            return null;
        }

        public static string Encrypt(this string value)
        {
            if (value.Length == 0)
            {
                return value;
            }
            int klen, i, j;
            char[] temp = value.ToCharArray();
            StringBuilder con = new StringBuilder();
            klen = Key.Length;
            for (i = 0; i < temp.Length;)
            {
                for (j = 0; j < klen; j++)
                {
                    if (i + j < temp.Length)
                    {
                        temp[i + j] = (char)((byte)temp[i + j] ^ (byte)Key[j]);
                        if ((byte)temp[i + j] == 0)
                        {
                            temp[i + j] = Key[j];
                        }
                    }
                }
                i += klen;
            }
            for (i = 0; i < temp.Length; i++)
            {
                con.Append((byte)temp[i] + 13);
                con.Append('+');
            }
            return con.ToString();
        }

        public static string Dencrypt(this string value)
        {
            if (value.Length == 0)
            {
                return value;
            }
            int i = 0, j = 0, klen = Key.Length;
            //根据Encrypt方法的加密规则，拆解字符串
            char[] encryptChars = (from bytchar in value.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries) select (char)(Convert.ToInt32(bytchar) - 13)).ToArray();
            char[] srcChars = new char[encryptChars.Length];

            for (i = 0; i < encryptChars.Length;)
            {
                for (j = 0; j < klen; j++)
                {
                    if (i + j < encryptChars.Length)
                    {
                        srcChars[i + j] = (char)((byte)encryptChars[i + j] ^ (byte)Key[j]);
                        if ((byte)srcChars[i + j] == 0)
                        {
                            srcChars[i + j] = Key[j];
                        }
                    }
                }
                i += klen;
            }

            return string.Join(string.Empty, srcChars);
        }
        #endregion

        #region 深度拷贝
        public static T DeepClone<T>(T obj)
        {
            if (obj != null)
            {
                return DeserializeByDataContract<T>(SerializeByDataContract<T>(obj));
            }
            return default(T);
        }
        #endregion

        #region 序列化
        public static Stream SerializeByDataContract<T>(object graph)
        {
            var memoryStream = new MemoryStream();
            var formatter = new DataContractSerializer(typeof(T));
            formatter.WriteObject(memoryStream, graph);
            return memoryStream;
        }

        public static T DeserializeByDataContract<T>(Stream stream)
        {
            stream.Position = 0;
            DataContractSerializer formatter = new DataContractSerializer(typeof(T));
            return (T)formatter.ReadObject(stream);
        }
        #endregion

        #region DateTime
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToDbString(this DateTime? dt)
        {
            if (dt != null)
            {
                return ToDbString((DateTime)dt);
            }
            return null;
        }
        /// <summary>
        /// yyyy-MM-dd HH:mm
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToDbDateTimeString(this DateTime? dt)
        {
            if (dt != null)
            {
                return ToDbDateTimeString((DateTime)dt);
            }
            return null;
        }

        public static string ToDbString(this DateTime dt)
        {
            return dt.ToString(Constant.DateDbFormat);
        }

        public static string ToDbDateTimeString(this DateTime dt)
        {
            return dt.ToString(Constant.DateTimeFormat);
        }

        /// <summary>
        /// 将日期格式化成 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 将日期格式化成 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime? dt)
        {
            if (dt != null)
                return ((DateTime)dt).ToString("yyyy-MM-dd HH:mm:ss");
            return null;
        }
        #endregion

        #region List
        //静态方法
        public static List<TResult> ToList<T, TResult>(this IEnumerable<T> value) where TResult : class
        {
            var result = new List<TResult>();
            if (value != null)
            {
                foreach (var obj in value)
                {
                    TResult temp = obj as TResult;
                    if (temp == null)
                    {
                        throw new Exception("出错：在ListEx.ToList方法中,对象转型失败.");
                    }
                    result.Add(temp);
                }
            }

            return result;
        }
        //静态方法
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return value != null && value.Count() > 0;
        }
        //静态方法
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return !IsNotNullOrEmpty(value);
        }

        //扩展方法
        public static string ToStringByProperty<T>(this IEnumerable<T> value, string propertyName, char separate)
        {
            var sb = new StringBuilder();
            foreach (var obj in value)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separate);
                }
                string section = obj.GetValueByRef<string>(propertyName);
                sb.Append(section == null ? string.Empty : section);
            }

            return sb.ToString();
        }
        //扩展方法
        public static string ToStringByValue(this Dictionary<string, string> value, char separate)
        {
            var sb = new StringBuilder();
            foreach (var obj in value)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separate);
                }
                sb.Append(obj.Value == null ? string.Empty : obj.Value);
            }

            return sb.ToString();
        }
        //扩展方法
        public static string ToStringByKey(this Dictionary<string, string> value, char separate)
        {
            var sb = new StringBuilder();
            foreach (var obj in value)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separate);
                }
                sb.Append(obj.Key == null ? string.Empty : obj.Key);
            }

            return sb.ToString();
        }
        #endregion

        #region Object
        public static bool IsNotNull(this object obj)
        {
            return !IsNull(obj);
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static void SetValueByRef(this object obj, string propertyName, object value)
        {
            if (propertyName.IsNotNullOrEmpty())
            {
                var pt = obj.GetType().GetProperty(propertyName);
                if (pt != null)
                {
                    pt.SetValue(obj, value, null);
                }
            }
        }

        public static T GetValueByRef<T>(this object obj, string propertyName)
        {
            if (propertyName.IsNotNullOrEmpty())
            {
                var pt = obj.GetType().GetProperty(propertyName);
                if (pt != null)
                {
                    return (T)pt.GetValue(obj, null);
                }
                else
                {
                    throw new System.Exception(string.Format("没有找到指定属性“{0}”！", propertyName));
                }
            }
            return default(T);
        }

        public static T CreateObj<T>(this object source) where T : class, new()
        {
            T result = new T();
            source.ModifyObj<T>(result);

            return result;
        }

        public static void ModifyObj<T>(this object source, T result) where T : class
        {
            if (result != null)
            {
                var sameProperties = from s in source.GetType().GetProperties()
                                     join t in result.GetType().GetProperties() on new { s.Name, s.PropertyType } equals new { t.Name, t.PropertyType }
                                     select new { Source = s, Target = t };
                foreach (var pt in sameProperties)
                {
                    if (pt.Source.CanRead && pt.Target.CanWrite)
                    {
                        pt.Target.SetValue(result, pt.Source.GetValue(source, null), null);
                    }
                }
            }
        }

        public static object DeepClone(this object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.Clone);
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }
        #endregion

        #region String

        public static string ToIdString(this List<string> list, string split = Constant.Str_Comma)
        {
            StringBuilder sb = new System.Text.StringBuilder();
            if (list.IsNotNullOrEmpty())
            {
                foreach (var s in list)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(split);
                    }
                    sb.Append(s);
                }
            }
            return sb.ToString().Trim(split.ToArray());
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> expr)
        {
            var rtn = string.Empty;
            if (expr.Body is UnaryExpression)
            {
                rtn = ((MemberExpression)((UnaryExpression)expr.Body).Operand).Member.Name;
            }
            else if (expr.Body is MemberExpression)
            {
                rtn = ((MemberExpression)expr.Body).Member.Name;
            }
            else if (expr.Body is ParameterExpression)
            {
                rtn = ((ParameterExpression)expr.Body).Type.Name;
            }
            return rtn;
        }

        public static string EncryptByMD5(this string str)
        {
            var md5Hasher = System.Security.Cryptography.MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static List<T> ToListByJson<T>(this String json)
        {
            return new JsonSerializer().Deserialize<List<T>>(new JsonTextReader(new System.IO.StringReader(json)));
        }

        public static T ToEntityByJson<T>(this String json)
        {
            return new JsonSerializer().Deserialize<T>(new JsonTextReader(new System.IO.StringReader(json)));
        }

        public static int[] ToIntArray(this string value, char separator = Constant.Chr_Comma)
        {
            return (from str in value.Split(separator) select Convert.ToInt32(str)).ToArray();
        }

        public static string GetLinkId(this string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                return value.Split(Constant.Chr_Comma)[0];
            }
            return string.Empty;
        }

        public static string GetLinkName(this string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                var arr = value.Split(Constant.Chr_Comma);
                return arr.Length == 1 ? arr[0] : value.Replace(arr[0] + Constant.Chr_Comma, string.Empty);
            }
            return string.Empty;
        }

        public static string GetId(this string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                return value.Split(Constant.Chr_Comma)[0];
            }
            return string.Empty;
        }

        public static string GetName(this string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                var arr = value.Split(Constant.Chr_Comma);
                return arr.Length == 1 ? arr[0] : value.Replace(arr[0] + Constant.Chr_Comma, string.Empty);
            }
            return string.Empty;
        }

        public static string Link(this string id, string name)
        {
            if (id != null && name != null)
            {
                return id + Constant.Str_Comma + name;
            }
            return id;
        }

        public static string LinkIdName(this string id, string name)
        {
            if (id != null && name != null)
            {
                return id + Constant.Str_Comma + name;
            }
            return id;
        }

        public static bool ContainValue(this string value, string key, char separator = Constant.Chr_Comma)
        {
            if (value.IsNotNullOrEmpty())
            {
                var arr = value.Split(separator);
                var result = arr.Contains(key);
                return result;
            }
            return false;
        }

        public static string Part(this string value, int index = 0, char separator = Constant.Chr_Comma)
        {
            if (value.IsNotNullOrEmpty())
            {
                var temp = value.Split(separator);
                if (temp.Length > index)
                {
                    return value.Split(separator)[index];
                }
            }
            return value;
        }

        public static bool IsSecureText(this string value)
        {
            return !(value.IsNotNullOrEmpty() && value.Contains("'"));
        }

        public static bool NotContains(this string str, string value)
        {
            return !str.Contains(value);
        }

        public static string ReplaceDangerousText(this string value)
        {
            if (!IsSecureText(value))
            {
                return value.Replace("'", "''");
            }
            return value;
        }

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        //扩展方法，转枚举
        public static T ToEnum<T>(this string value) where T : struct
        {
            T result;
            if (Enum.TryParse<T>(value, true, out result))
            {
                return result;
            }
            else
            {
                throw new Exception("枚举类型转换失败！");
            }
        }

        public static void Replace(this string strValue, char value, int index)
        {
            if (strValue.IsNotNullOrEmpty() && index < strValue.Length)
            {
                if (strValue.Length == 1)
                {
                    strValue = value.ToString();
                }
                else if (index == 0)
                {
                    strValue = value + strValue.Substring(index + 1);
                }
                else if (index == strValue.Length - 1)
                {
                    strValue = strValue.Substring(0, strValue.Length - 1) + value;
                }
                else
                {
                    strValue = strValue.Substring(0, index) + value + strValue.Substring(index + 1);
                }
            }
        }
        #endregion

        #region Type
        public static bool IsBaseType(this Type obj, Type baseType)
        {
            if (obj.BaseType != null)
            {
                if (obj.BaseType == baseType)
                {
                    return true;
                }
                else
                {
                    return IsBaseType(obj.BaseType, baseType);
                }
            }
            return false;
        }

        public static string GetEntityField(this Type type)
        {
            return string.Format("{0} '{1}' AS {2} ", Constant.Str_Comma, type.FullName, Constant.EntityFullName);
        }
        #endregion

        #region INT
        /// <summary>
        /// 整数1，2，3。。。转换成A,B,C。。。
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToUpperCase(this int number)
        {
            if (1 <= number && 36 >= number)
            {
                int num = number + 64;
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] btNumber = new byte[] { (byte)num };
                return asciiEncoding.GetString(btNumber);
            }
            throw new Exception("数字不在转换范围内。");
        }

        #endregion

        #region 序号
        /// <summary>
        /// 有序GUID
        /// http://stackoverflow.com/questions/1752004/sequential-guid-generator
        /// </summary>
        /// <returns></returns>
        public static string NewId()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            var baseDate = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;
            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            TimeSpan msecs = now.TimeOfDay;

            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray).ToString().Replace(Constant.Str_Guid_Delimiter, string.Empty);
        }
        /// <summary>
        /// 生成帖子序号
        /// </summary>
        /// <returns></returns>
        public static string GetSerialNumber(string createTypeId)
        {
            StringBuilder sb = new StringBuilder(512);
            sb.Append("OA-");
            sb.Append(createTypeId.Substring(createTypeId.Length - 5, 5));
            sb.Append("-");
            sb.Append(DateTime.Now.ToString("yyyy-MM-dd"));
            sb.Append(new Random().NextDouble().ToString());

            return sb.ToString().Substring(0, 30);
        }
        /// <summary>
        /// 生成以日期(yyyyMMddHHmm)开头的20位随机数字符(此方法不适用于短时间频繁操作)
        /// </summary>
        /// <returns></returns>
        public static string GetNewId()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("yyyyMMddHHmmssffff").ToString());
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            sb.Append(rd.Next(10, 99).ToString());

            return sb.ToString();
        }
        /// <summary>
        /// 生成以日期开头的有序ID
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNewId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= (b + 1);
            }
            return DateTime.Now.ToString("yyyyMMddHHmmss")+string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        #endregion

        #region 字符串压缩解压
        public static string Compress(string value)
        {
            return Encoding.UTF8.GetString(Compress(Encoding.UTF8.GetBytes(value)));
        }
        public static Byte[] Compress(Byte[] data)
        {
            //Encoding.UTF8.GetString(test);
            // 压缩入这个内存流
            using (MemoryStream target = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(target, CompressionMode.Compress, true))
                {
                    // 把数据写入压缩流
                    gs.Write(data, 0, data.Length);
                }
                return target.ToArray();
            }
        }
        //解压数据

        public static string DeCompress(string value)
        {
            return Encoding.UTF8.GetString((DeCompress(Encoding.UTF8.GetBytes(value))));
        }

        public static Byte[] DeCompress(Byte[] data)
        {
            using (MemoryStream source = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(new MemoryStream(data), CompressionMode.Decompress, true))
                {
                    //从压缩流中读出所有数据
                    byte[] bytes = new byte[4096];
                    int n;
                    while ((n = gs.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        source.Write(bytes, 0, n);
                    }
                }
                return source.ToArray();
            }
        }

        #endregion
        #region 验证手机号
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool ValMobile(string mobile)
        {
            return Regex.IsMatch(mobile, @"^0{0,1}1[3-9]\d{9}$");
        }
        #endregion
        #region 邮箱格式验证
        public static bool ValMaill(string mail)
        {
            Regex r = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            return r.IsMatch(mail);
        }
        #endregion
        #region 身份证号操作相关
        /// <summary>
        /// 身份证号码验证
        /// </summary>
        /// <param name="cardno"></param>
        /// <returns></returns>
        public static bool ValCardId(string cardno)
        {
            Regex r = new Regex(@"(^\d{17}[\d|X]$)|(^\d{15}$)");
            return r.IsMatch(cardno.ToUpper());
        }
        /// <summary>
        /// 获取身份证号码中的生日及性别数组:比如["1978-08-11","男"]
        /// </summary>
        /// <param name="cardno"></param>
        /// <returns></returns>
        public static string[] GetBirthdayAndSex(string cardno)
        {
            List<string> result = new List<string>();
            string temp;
            try
            {
                temp = cardno.Substring(6, 8);
                temp = temp.Substring(0, 4) + "-" + temp.Substring(4, 2) + "-" + temp.Substring(6, 2);
                result.Add(temp);
                if (cardno.Length == 15)
                    temp = cardno.Substring(14, 1);
                else
                    temp = cardno.Substring(16, 1);
                if (int.Parse(temp) % 2 == 0)
                    temp = "女";
                else
                    temp = "男";
                result.Add(temp);
            }
            catch
            {
                result.Add("");
                result.Add("");
            }

            return result.ToArray();
        }
        #endregion
        #region 中文 "1小时30分钟"转换为分
        /// <summary>
        /// "1小时30分钟"时间字符串转换为分
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
        public static string TimeStrToMin(string timeStr)
        {
            int temp = 0;
            Match match = Regex.Match(timeStr, @"((\d*)小时)*((\d*)分)*", RegexOptions.None);
            if (match.Success)
            {
                if (match.Groups[2].Success)
                    temp = temp + int.Parse(match.Groups[2].Value) * 60;
                if (match.Groups[4].Success)
                    temp = temp + int.Parse(match.Groups[4].Value);

            }

            return temp.ToString();
        }
        #endregion
        #region 获取刚好大于或小于某个数的2 n次方的数 
        /// <summary>
        /// 获取刚好大于某个数的2 n次方的数 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetPow(int number)
        {
            double pow = Math.Log(number, 2);
            int floor = (int)Math.Floor(pow);
            int ceil = (int)Math.Ceiling(pow);

            int result = 0;
            if (number - Math.Pow(2, floor) < Math.Pow(2, ceil) - number)
                result = floor;
            else
                result = ceil;
            return (int)Math.Pow(2, result);
        }
        #endregion
        #region 返回Newtonsoft Json JsonSerializerSettings
        public static JsonSerializerSettings GetSerializerSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            settings.Converters.Add(dateTimeConverter);
            return settings;
        }
        #endregion
        /// <summary>
        /// 获取悦动力体育APP下载链接
        /// </summary>
        /// <returns></returns>
        public static string GetAppUrl()
        {
            return "http://a.app.qq.com/o/simple.jsp?pkgname=com.ydlsoft.client";
        }
        /// <summary>
        /// 获取悦动力体育APP下载短链接
        /// </summary>
        /// <returns></returns>
        public static string GetSortAppUrl()
        {
            return "t.cn/RshB8Hq";
        }
        /// <summary>
        /// 数字字符串转换为中文数字字符串,比如:12->十二
        /// </summary>
        /// <param name="number">阿拉伯数字字符串</param>
        /// <param name="result">初始传""</param>
        /// <returns></returns>
        public static string NumberToStr(string number,  string result="")
        {
            if (!string.IsNullOrEmpty(number))
            {
                string temp = number.Substring(0, 1);
                switch (temp)
                {
                    case "1":
                        result = result + "一";
                        break;
                    case "2":
                        result = result + "二";
                        break;
                    case "3":
                        result = result + "三";
                        break;
                    case "4":
                        result = result + "四";
                        break;
                    case "5":
                        result = result + "五";
                        break;
                    case "6":
                        result = result + "六";
                        break;
                    case "7":
                        result = result + "七";
                        break;
                    case "8":
                        result = result + "作";
                        break;
                    case "9":
                        result = result + "九";
                        break;
                    case "0":
                        result = result + "零";
                        break;
                }
                switch (number.Length)
                {
                    case 1:
                        break;
                    case 2:
                    case 6:
                        result = result + "十";
                        break;
                    case 3:
                    case 7:
                        result = result + "百";
                        break;
                    case 8:
                    case 4:
                        result = result + "千";
                        break;
                    case 5:
                        result = result + "万";
                        break;
                    case 9:
                        result = result + "亿";
                        break;
                }
            }
            number = number.Substring(1, number.Length - 1);
            if (!string.IsNullOrEmpty(number))
                result= NumberToStr(number, result);

            return result;
        }
    }
}
