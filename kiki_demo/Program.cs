using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kiki;

namespace kiki_demo
{
    static partial class Program
    {

        private static int tableWidth = 80;
        private static readonly Dictionary<string, string> SortedSubsets = new()
        {
            //considering negative grades all those 'lower than' 8 values
            {"negativeGrade", "{lt(TestGrade, 8)}"},
            //considering positive grades all those 'greater than'10 values
            {"positiveGrade", "{mte(TestGrade, 10)}"},
            //considering deficient grades all those 'greater than or equal' to 8 and 'less than' 10 values
            {"deficiencyGrade", "{&(mte(TestGrade,8),lt(TestGrade,10))}"}
        };

        private static readonly Dictionary<string, string> AuxilarSortedSubsets = new()
        {
            //considering 'Csharp' and 'Javascript' as the most important subjects
            //(I will use it in the script to help me find candidates who have
            //no negative or deficiency in this subject)
            {"mostImportant", "{or(eq(SubjectName,'Csharp'),eq(SubjectName,'Javascript'))}"},

        };

        private static void Main(string[] args)
        {
            var script = new StringBuilder();

            script.AppendLine("if(lt(sumT(ContributionsOnGithub),1000), {$R->('Failed GC')},");
            script.AppendLine("{ ");
            script.AppendLine("    if(&(eq(count(negativeGrades),0),eq(count(deficiencyGrades),0)),");
            script.AppendLine("    {");
            script.AppendLine("            if(Helped,{$R->('Passed H')},{$R->('Passed')})");
            script.AppendLine("  },");
            script.AppendLine(" {");
            script.AppendLine("       if(&(eq(count(negativeGrades),0), lte(count(deficiencyGrades),2),");
            script.AppendLine("       !=> (deficiencyGrades,mostImportants,SubjectId)),");
            script.AppendLine("                {");
            script.AppendLine("                  $R->('Recovery')");
            script.AppendLine("                }");
            script.AppendLine("               ,{");
            script.AppendLine("                   $R->('Failed')");
            script.AppendLine("                })");
            script.AppendLine("    })");
            script.AppendLine("})");

            var evaluator = new kiki.Evaluator<SoftwareDeveloperJobApplication>();
            var data = Repository.GetApplications();
            var evalResult = evaluator.Run(data, x => x.CandidateName,
                x => x.CandidateId,
                (x,
                    x1) => x.CandidateId == x1.CandidateId,
                x => x.TestGrade, x => x.SubjectId,
                x => x.SubjectName,
                x => x.Result, x=>x .Obs, script.ToString(),
                SortedSubsets,AuxilarSortedSubsets);

            if (evalResult.Type == KikiEvaluatorMessageType.Error)
            {
                Console.WriteLine(evalResult.Message);
                return;
            }

            Console.WriteLine(evaluator.ResultDescription);
           
            var fields = evaluator.Fields.Except(new[]
                {"Obs", "SubjectCategoryId", "SubjectId", "CandidateId", "CandidateName","Result"});
            var enumerable = fields as string[] ?? fields.ToArray();
            foreach (var x in  data.Distinct(x=> x.CandidateId))
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine($"{x.CandidateId } {x.CandidateName} -> {x.Result}");
                Console.WriteLine("");
                PrintLine();
                PrintRow(enumerable);
                foreach (var xs in data.Where( c=> c.CandidateId == x.CandidateId).OrderBy(x=> x.SubjectCategoryId))
                {
                    PrintLine();
                    PrintRow(enumerable.Select(field => xs.GetDynValue(new[] {field}).ToString()).ToArray());
                }
                
                Console.WriteLine(x.Obs);
                
            }
          
            Console.ReadLine();
        }
        
        
     // write table in console   
     // https://stackoverflow.com/questions/856845/how-to-best-way-to-draw-table-in-console-app-c  
        
        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            var width = (tableWidth - columns.Length) / columns.Length;
            var row = "|";

            foreach (var column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
        
        
    }
}