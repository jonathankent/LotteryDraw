using System;

namespace LotteryDrawApp.Models
{
    public class LotteryDrawModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DrawDate { get; set; }
        public int TotalPrimary { get; set; }
        public Tuple<int, int> RangePrimary { get; set; }
        public int TotalSecondary { get; set; }
        public Tuple<int, int> RangeSecondary { get; set; }
        public Tuple<int, int, int, int, int> WinningPrimary;
        public Tuple<int, int> WinningSecondary;
    }
}