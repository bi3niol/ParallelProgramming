using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.EX3.Console
{
    public static class Helpers
    {
        public static int[][] GetPositions(string filePath)
        {
            using (var file = File.OpenRead(filePath))
            {
                using (StreamReader streamReader = new StreamReader(file))
                {

                    var count = int.Parse(streamReader.ReadLine());
                    var res = new int[count][];
                    for (int i = 0; i < count; i++)
                        res[i] = streamReader.ReadLine().Split(' ').Select(s => int.Parse(s)).ToArray();
                    return res;
                }
            }
        }

        public static double Distance(int[] x1, int[] x2)
        {
            double res = 0;

            for (int i = 0; i < x1.Length; i++)
                res += Math.Pow(x1[i] - x2[i], 2);

            return Math.Sqrt(res);
        }
        
        public static bool CanHear(int[] x1,int[] x2, double range)
        {
            return Distance(x1,x2)<=range;
        }

        public static string GetJankielName(int[] x)
        {
            return $"_{x[0]}_{x[1]}_";
        }

        public static string[] GetNeighborsForI(int[][] positions, int i, double distance)
        {
            List<string> res = new List<string>();
            for (int j = 0; j < positions.GetLength(0); j++)
            {
                if (j == i)
                    continue;
                if (CanHear(positions[i], positions[j], distance))
                    res.Add(GetJankielName(positions[j]));
            }

            return res.ToArray();
        }
    }
}
