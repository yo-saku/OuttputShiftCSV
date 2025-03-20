using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace OuttputShiftCSV
{
    internal class ShiftExcel
    {
        private XLWorkbook workBook;

        public ShiftExcel()
        {
            throw new Exception();
        }
        public ShiftExcel(string filePath) 
        {
            workBook = new XLWorkbook(filePath);
        }

        public void ReadShiftExcel()
        {
            const int START_ROW_NUM = 4;
            const int START_COLOUMN_NUM = 4;
            const int WORKER_CODE_COLUMN_NUM = 2;

            //読み取り対象のエクセルオブジェクト作成
            IXLWorksheet workSheet = workBook.Worksheet("先生");
            //保育士の行終了位置特定
            int endRowNum = GetEndNurseryTeacherShiftStartRowNum();
            for (int i = START_ROW_NUM; i < endRowNum; i = i + 2)
            {
                //先生の名前の出力
                Console.WriteLine(workSheet.Cell(i, 2).Value.ToString());
                // 人ごとにID指定があるかを確認。なければ処理対象から飛ばす
                if (workSheet.Cell(i + 1, WORKER_CODE_COLUMN_NUM).Value.ToString() == "")
                {
                    continue;
                }

                //各日程の勤務予定の確認
                int endDateColoumnNum = GetEndDateColoumnNum();
                for (int k = START_COLOUMN_NUM; k < endDateColoumnNum; k++)
                {
                    string startDateStr = workSheet.Cell(i, k).Value.ToString();
                    if (startDateStr == "")
                    {
                        continue;
                    }
                    string endDateStr = workSheet.Cell(i + 1, k).Value.ToString();

                    string shiftStr = "開始:" + startDateStr + " " + "終了：" + endDateStr;
                    Console.WriteLine(shiftStr);

                }

            }

            //調理師の行開始位置特定
        }


        private int GetEndNurseryTeacherShiftStartRowNum()
        {
            return 35;
        }

        private int GetEndDateColoumnNum()
        {
            return 34;
        }

    }
}
