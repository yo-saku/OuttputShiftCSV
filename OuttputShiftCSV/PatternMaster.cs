using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuttputShiftCSV
{
    internal class PatternMaster
    {
        private XLWorkbook workBook;
        private List<ShiftPattern>shiftPatternList;

        public PatternMaster()
        {
            throw new NotImplementedException();
        }
        public PatternMaster(string filePath)
        {
            workBook = new XLWorkbook(filePath);
            shiftPatternList = new List<ShiftPattern>();
        }

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
                Console.WriteLine(shiftPattern.ToString());
            }
        }

        private void AddShiftPattern(ShiftPattern pattern)
        {
            //実際に使用されているパターンは、パターンコード違いで開始時間、終了時間が同じデータが存在するが、
            //開始時間、終了時間の組み合わせが同じものは登録しない
            ShiftPattern patterInList = shiftPatternList.Find(p => p.StartDateTime.ToString("HH:mm") == pattern.StartDateTime.ToString("HH:mm") &&
                                                            p.EndDateTime.ToString("HH:mm") == pattern.EndDateTime.ToString("HH:mm"));
            if(patterInList == null)
            {
                shiftPatternList.Add(pattern);
            }
        }

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
    }
}
