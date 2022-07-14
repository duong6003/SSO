using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Helpers
{
    public class TimeCaculator
    {
        public static bool GetHours(TimeSpan start, TimeSpan end, TimeSpan startTarget, TimeSpan endTarget)
        {
            if (startTarget >= start && endTarget <= end)
            {
                return true;
            }

            if ((startTarget >= start && startTarget < end) && endTarget > end)
            {
                return true;
            }

            if (startTarget < start && (endTarget > start && endTarget <= end))
            {
                return true;
            }

            if (startTarget < start && endTarget > end)
            {
                return true;
            }
            return false;
        }
        public static bool GetHoursByDateTime(DateTime start, DateTime end, DateTime startTarget, DateTime endTarget)
        {
            if (startTarget >= start && endTarget <= end)
            {
                return true;
            }

            if ((startTarget >= start && startTarget < end) && endTarget > end)
            {
                return true;
            }

            if (startTarget < start && (endTarget > start && endTarget <= end))
            {
                return true;
            }

            if (startTarget < start && endTarget > end)
            {
                return true;
            }
            return false;
        }
        public static bool GetHoursByListDateTime(List<DateTime> start, List<DateTime> end, DateTime startTarget, DateTime endTarget)
        {
            for (int i = 0; i < start.Count; i++)
            {
                if (!GetHoursByDateTime(start[i], end[i], startTarget, endTarget))
                {
                    continue;
                }
                else
                {
                    return true;

                }
            }
            return false;
        }
        public static bool GetHoursByListTimeSpan(List<TimeSpan> start, List<TimeSpan> end, TimeSpan startTarget, TimeSpan endTarget)
        {
            for (int i = 0; i < start.Count; i++)
            {
                if (!GetHours(start[i], end[i], startTarget, endTarget))
                {
                    continue;
                }
                else
                {
                    return true;

                }
            }
            return false;
        }
    }
}
