using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerContent
{
    public class LTimer
    {
        Stopwatch m_StopWatch = Stopwatch.StartNew();
        public static double m_GameTimer = 0.0;
        public static double m_SPF = 0.0;
        double m_PreviousTime = 0.0;
        double m_FPS = 0.0;
        int m_iFPS = 0;
        int iFPS = 0;
        bool viewStart = false;

        public void Init()
        {
            m_PreviousTime = m_StopWatch.Elapsed.TotalSeconds;
            
        }

        public double GetFPS()
        {
            viewStart = false;
            if (m_FPS >= 1.0)
            {
                m_iFPS = iFPS;
                iFPS = 0;
                m_FPS -= 1.0f;
                viewStart = true;
            }

            ++iFPS;

            return m_iFPS;
        }

        public void Frame()
        {
            double currentTime = m_StopWatch.Elapsed.TotalSeconds;
            double elapsedTime = (currentTime - m_PreviousTime);
            m_SPF = elapsedTime;
            m_FPS += elapsedTime;
            m_GameTimer += elapsedTime;
            m_PreviousTime = currentTime;
            GetFPS();
        }

        public void Render()
        {
            if (viewStart)
            {
                Console.WriteLine($"GT : {m_GameTimer}\n");
                Console.WriteLine($"FPS : {GetFPS()}\n");
                Console.WriteLine($"SPF : {m_SPF}\n");
            }
        }
    }
}
