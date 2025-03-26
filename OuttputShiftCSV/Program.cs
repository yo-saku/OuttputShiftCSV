using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OuttputShiftCSV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //パターンコードの設定ファイルの読み取り
                PatternMaster patternMaster = new PatternMaster(@Console.ReadLine());
                patternMaster.ReadMasterExcel();
                //シフトエクセルの読み取り
                //TODO 最終的にはファイルをドラッグアンドドロップに変更予定
                //for (int i = 0; i < args.Length; i++)
                //{
                //    Console.WriteLine(args[i]);
                //}
                ShiftExcel shiftExcel = new ShiftExcel(@Console.ReadLine(), patternMaster);
                shiftExcel.ReadShiftExcel();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                string name = Console.ReadLine();
            }

        }
    }
}
