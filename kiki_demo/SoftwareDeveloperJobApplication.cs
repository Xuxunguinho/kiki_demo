using System;

namespace kiki_demo
{
    public class  SoftwareDeveloperJobApplication
        {
            public int  CandidateId { get; set; }
           
            public string CandidateName { get; set; }
            public int SubjectId { get; set; }
            
            public string SubjectName { get; set; }
          
            public int SubjectCategoryId{ get; set; }
            public string SubjectCategoryName{ get; set; }

            public int ContributionsOnGithub { get; set; } = 0;
            public double  TestGrade { get; set; }

            public bool Helped { get; set; } = false;
            public string Obs { get; set; } = string.Empty;
            public string Result { get; set; }
        }
}