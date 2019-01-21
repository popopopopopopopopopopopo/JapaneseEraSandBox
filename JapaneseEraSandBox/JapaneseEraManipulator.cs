using System;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;

namespace JapaneseEraSandBox
{
    public class JapaneseEraManipulator
    {
        public static void UpdateJapaneseEra (DateTimeFormatInfo dateTimeFormatInfo, object[,] NewEraInfoParameter) {

            var len = NewEraInfoParameter.GetLength(0);
   
            string[] new_EraNames = new string[len];
            string[] new_AbbrevEraNames = new string[len];
            string[] new_AbbrevEnglishEraNames = new string[len];
   
            for (int i = len-1; i >= 0; i--) {
                new_EraNames[i] =  (string)NewEraInfoParameter.GetValue(i, 0);
                new_AbbrevEraNames[i] = (string)NewEraInfoParameter.GetValue(i, 1);
                new_AbbrevEnglishEraNames[i] = (string)NewEraInfoParameter.GetValue(i, 2);
            }
   
            Type dateTimeFormatInfoType = dateTimeFormatInfo.GetType();
   
            FieldInfo eraNamesFieldInfo = dateTimeFormatInfoType.GetField("m_eraNames", BindingFlags.NonPublic | 
                                                                                        BindingFlags.Instance | 
                                                                                        BindingFlags.Static);
            eraNamesFieldInfo.SetValue(dateTimeFormatInfo, new_EraNames);
   
            FieldInfo abbrevEraNamesFieldInfo = dateTimeFormatInfoType.GetField("m_abbrevEraNames", BindingFlags.NonPublic | 
                                                                                                    BindingFlags.Instance | 
                                                                                                    BindingFlags.Static);
            abbrevEraNamesFieldInfo.SetValue(dateTimeFormatInfo, new_AbbrevEraNames);

            FieldInfo abbrevEnglishEraNamesFieldInfo = dateTimeFormatInfoType.GetField("m_abbrevEnglishEraNames", BindingFlags.NonPublic | 
                                                                                                                  BindingFlags.Instance | 
                                                                                                                  BindingFlags.Static);
            abbrevEnglishEraNamesFieldInfo.SetValue(dateTimeFormatInfo, new_AbbrevEnglishEraNames);
        }
        
        public static void UpdateJapaneseEra(JapaneseCalendar japaneseCalendar, object[,] NewEraInfoParameter)
        {
            Type japaneseCalendarType = japaneseCalendar.GetType();

            FieldInfo helperFieldInfo = japaneseCalendarType.GetField("helper", BindingFlags.NonPublic |
                                                                                BindingFlags.Instance |
                                                                                BindingFlags.Static);
            object helper = helperFieldInfo.GetValue(japaneseCalendar);

            Type helperType = helper.GetType();

            FieldInfo eraInfoFieldInfo = helperType.GetField("m_EraInfo", BindingFlags.NonPublic |
                                                                          BindingFlags.Instance |
                                                                          BindingFlags.Static);

            Array eraInfo = (Array) eraInfoFieldInfo.GetValue(helper);

            Type eraInfoType = eraInfo.GetValue(0).GetType();

            int len = NewEraInfoParameter.GetLength(0);

            Array new_EraInfo = Array.CreateInstance(eraInfoType, len);
            int[] new_eras = new int[len];

            // Build parameters
            int n = 0;
            int minYear = 1;
            int maxYear = 9999;
            for (int i = NewEraInfoParameter.GetLength(0) - 1; i >= 0; i--)
            {
                int Year = (int) NewEraInfoParameter.GetValue(i, 3);
                int Month = (int) NewEraInfoParameter.GetValue(i, 4);
                int Day = (int) NewEraInfoParameter.GetValue(i, 5);

                if (n == 0)
                {
                    maxYear = 9999 - Year - 1;
                }
                else
                {
                    maxYear = (int) NewEraInfoParameter.GetValue(i + 1, 3) - Year + 1;
                }

                
                //.NET4.7.1の場合（EraInfoのコンストラクタ引数が変更されているので、引数を変える）
                // https://referencesource.microsoft.com/#mscorlib/system/globalization/gregoriancalendarhelper.cs,35787f4d02d3112f
                var newEra = Activator.CreateInstance(eraInfoType, BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new object[] {i + 1, Year, Month, Day, Year-1, minYear, maxYear}, null);
                new_EraInfo.SetValue(newEra, n);
                
//                //.NET3.5までのコンストラクタ（ActivatorもしくはAssembly）
//                var newEra = Activator.CreateInstance(eraInfoType, BindingFlags.NonPublic | BindingFlags.Instance, null,
//                    new object[] {i + 1, (new DateTime(Year, Month, Day)).Ticks, Year - 1, minYear, maxYear}, null);
//                new_EraInfo.SetValue(newEra, n);
//                new_EraInfo.SetValue(eraInfoType.Assembly.CreateInstance(eraInfoType.FullName
//                        , false
//                        , BindingFlags.Public | BindingFlags.Instance 
//                        , null
//                        , new object[] {i + 1, (new DateTime(Year, Month, Day)).Ticks, Year - 1, minYear, maxYear}
//                        , null
//                        , null)
//                    , n);
                
                ////.NET2.0までのコンストラクタ（ActivatorもしくはAssembly）
                //var newEra = Activator.CreateInstance(eraInfoType, BindingFlags.NonPublic | BindingFlags.Instance, null,
                //    new object[] { i + 1, (new DateTime(Year, Month, Day)).Ticks, Year - 1, minYear, maxYear }, null);
                //new_EraInfo.SetValue(newEra, n);
                //new_EraInfo.SetValue(eraInfoType.Assembly.CreateInstance(eraInfoType.FullName,
                //    false, 
                //    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null,
                //    new object[] { i + 1, (new DateTime(Year, Month, Day)).Ticks, Year - 1, minYear, maxYear },
                //    null, null), n);

                new_eras[i] = len - i;
                n++;
            }

            eraInfoFieldInfo.SetValue(helper, new_EraInfo);
            FieldInfo erasFieldInfo = helperType.GetField("m_eras", BindingFlags.NonPublic |
                                                                    BindingFlags.Instance |
                                                                    BindingFlags.Static);

            ////.NET2.0の場合
            //FieldInfo erasFieldInfo = helperType.GetField("m_eras", BindingFlags.NonPublic |
            //                                                        BindingFlags.Instance |
            //                                                        BindingFlags.Static);

            erasFieldInfo.SetValue(helper, new_eras);
        }
    }
}