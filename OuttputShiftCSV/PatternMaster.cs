using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuttputShiftCSV
{
    internal class PatternMaster
    {
        const string FILE_NAME = "KOTスケジュールパターンリスト.xlsx";
        private XLWorkbook workBook;
        private List<ShiftPattern>shiftPatternList;
        private Dictionary<string, List<string>> sameStartEndDic = new Dictionary<string, List<string>>();

        public PatternMaster()
        {
            string masterFIlePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_NAME);
            if (!File.Exists(masterFIlePath))
            {
                throw new Exception(FILE_NAME + "がありません。");
            }
            workBook = new XLWorkbook(masterFIlePath);
            shiftPatternList = new List<ShiftPattern>();
        }

        /// <summary>
        /// シフトパターンの記載エクセルを読み取る
        /// </summary>
        public void ReadMasterExcel()
        {
            const int START_ROW_NUM = 13;
            const int PATTERN_CODE_COLOUMN_NUM = 2;
            const int START_TIME_COLOUMN_NUM = 9;
            const int END_TIME_COLOUMN_NUM = 10;

            IXLWorksheet workSheet = workBook.Worksheet("KOTスケジュールパターンリスト");

            //最大1000行まで読み取り
            //※1000行は超えない前提
            for (int i=START_ROW_NUM; i < 1000; i++)
            {
                string patternCode = workSheet.Cell(i, PATTERN_CODE_COLOUMN_NUM).Value.ToString();
                if(patternCode == "")
                {
                    break;
                }
                //baseTimeについてはエクセル上下記フォーマットで記載されている
                //ex) 当日09 時 00 分
                string startBaseTime = workSheet.Cell(i, START_TIME_COLOUMN_NUM).Value.ToString();
                string startTime = startBaseTime.Substring(2,2) + ":" + startBaseTime.Substring(7, 2);
                string endBaseTime = workSheet.Cell(i, END_TIME_COLOUMN_NUM).Value.ToString();
                string endTime = endBaseTime.Substring(2, 2) + ":" + endBaseTime.Substring(7, 2);
                ShiftPattern shiftPattern = new ShiftPattern(patternCode, new Shift(startTime,endTime));
                AddShiftPattern(shiftPattern);
                //Console.WriteLine(shiftPattern.ToString());
            }
        }

        /// <summary>
        /// シフトパターンの追加
        /// </summary>
        /// <param name="pattern"></param>
        /// <remarks>重複パターンは先勝ち。後から読み込まれたものは後続処理では使用しない</remarks>
        private void AddShiftPattern(ShiftPattern pattern)
        {
            //実際に使用されているパターンは、パターンコード違いで開始時間、終了時間が同じデータが存在するが、
            //開始時間、終了時間の組み合わせが同じものは登録しない。最初に読み込まれたものを後続の処理で使用する
            ShiftPattern sameStartEndPattern = shiftPatternList.Find(p => p.StartDateTime.ToString("HH:mm") == pattern.StartDateTime.ToString("HH:mm") &&
                                                            p.EndDateTime.ToString("HH:mm") == pattern.EndDateTime.ToString("HH:mm"));
            if(sameStartEndPattern == null)
            {
                shiftPatternList.Add(pattern);
            }
            else
            {
                //重複したものは情報を保持する
                string samePatternKey = sameStartEndPattern.StartDateTime.ToString("HH:mm") + "-" + sameStartEndPattern.EndDateTime.ToString("HH:mm");
                List<string> samePatternList = new List<string>();
                if (sameStartEndDic.Keys.Contains(samePatternKey))
                {
                    samePatternList = sameStartEndDic[samePatternKey];
                    samePatternList.Add(pattern.PatternCode);
                }
                else
                {
                    samePatternList.Add(sameStartEndPattern.PatternCode);
                    samePatternList.Add(pattern.PatternCode);
                    sameStartEndDic.Add(samePatternKey, samePatternList);
                }
            }
        }

        /// <summary>
        /// シフト情報からパターンコードを取得する
        /// </summary>
        /// <param name="shift">シフト情報</param>
        /// <returns></returns>
        public string GetShiftPatternCode(Shift shift)
        {
            string result = "";
            ShiftPattern patterInList = shiftPatternList.Find(p => p.StartDateTime.ToString("HH:mm") == shift.StartDateTime.ToString("HH:mm") &&
                                                p.EndDateTime.ToString("HH:mm") == shift.EndDateTime.ToString("HH:mm"));
            if (patterInList != null)
            {
                result = patterInList.PatternCode;
            }
            return result;
        }

        /// <summary>
        /// 出勤時刻と退勤時刻の重複のあったパターンコード情報を返却する
        /// 重複が無ければ空のリストを返却
        /// </summary>
        /// <returns>重複のあったパターンコード情報</returns>
        public List<string> GetSamePatternCodesInfomation()
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, List<string>> samePattern in sameStartEndDic)
            {
                string samePatternCodes = "";
                foreach (string patternCode in samePattern.Value)
                {
                    if (samePatternCodes != "")
                    {
                        samePatternCodes = samePatternCodes + ",";
                    }
                    samePatternCodes = samePatternCodes + patternCode;
                }
                result.Add(samePattern.Key + " - " + samePatternCodes);
            }

            return result;
        }
    }
}
