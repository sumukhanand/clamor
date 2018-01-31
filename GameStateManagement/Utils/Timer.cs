using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Utils
{
    public delegate void TimerDelegate();
    public class TimerInstance
    {
        public string sTimerName;
        public event TimerDelegate OnTimer;
        public bool bLooping;
        public bool bRemove;
        public float fTotalTime;
        public float fRemainingTime;
        public int iTriggerCount;

        public void Trigger() { OnTimer(); }
    }

    public class Timer
    {
        #region Fields

        private SortedList<string, TimerInstance> m_kTimers = new SortedList<string, TimerInstance>();

        #endregion

        #region Methods

        /// <summary>
        /// Update is called every frame by the owner of this timer class.
        /// It's responsible for updating every currently registered timer.
        /// If any timer has expired, it is triggered, and based on looping or not 
        /// it may either be removed or restarted
        /// Additionally, iTriggerCount for a timer should be incremented every time it triggers.
        /// </summary>
        /// <param name="gameTime">Only ElasedGameTime is used to update all registered timers</param>
        public void Update(GameTime gameTime)
        {
            IList<string> keys = m_kTimers.Keys;

            for (int i = 0; i < keys.Count; i++)
            {
                if (m_kTimers[keys[i]].bRemove == false)
                {
                    m_kTimers[keys[i]].fRemainingTime -= (float)(gameTime.ElapsedGameTime.TotalSeconds);
                    if (m_kTimers[keys[i]].fRemainingTime <= 0)
                    {
                        m_kTimers[keys[i]].iTriggerCount++;
                        m_kTimers[keys[i]].fRemainingTime = 0;
                        if (m_kTimers[keys[i]].bLooping == true)
                            m_kTimers[keys[i]].fRemainingTime = m_kTimers[keys[i]].fTotalTime;
                        else
                            m_kTimers[keys[i]].bRemove = true;
                        m_kTimers[keys[i]].Trigger();
                    }
                    // else
                    // m_kTimers[keys[i]].iTriggerCount++;
                }
            }
        }

        /// <summary>
        /// AddTimer will add a new timer provided a timer of the same name does not already exist.
        /// </summary>
        /// <param name="sTimerName">Name of timer to be added</param>
        /// <param name="fTimerDuration">Duration timer should last, in seconds</param>
        /// <param name="Callback">Call back delegate which should be called when the timer expires</param>
        /// <param name="bLooping">Whether the timer should loop infinitely, or should fire once and remove itself</param>
        /// <returns>Returns true if the timer was successfully added, false if it wasn't</returns>
        public bool AddTimer(string sTimerName, float fTimerDuration, TimerDelegate Callback, bool bLooping)
        {
            if (!m_kTimers.ContainsKey(sTimerName))
            {
                TimerInstance tempTimer = new TimerInstance();
                tempTimer.sTimerName = sTimerName;
                tempTimer.fTotalTime = fTimerDuration;
                tempTimer.fRemainingTime = fTimerDuration;
                tempTimer.OnTimer += Callback;
                tempTimer.bLooping = bLooping;

                m_kTimers.Add(sTimerName, tempTimer);

                return true;
            }
            return false;
        }

        /// <summary>
        /// RemoveTimer removes the timer with the specified name
        /// You must support being able to remove one timer from another timer's callback
        /// (But don't worry about removing the same timer from your callback, 'cause that's confusing)
        /// </summary>
        /// <param name="sTimerName">Name of timer to remove</param>
        /// <returns>True if successfully removed, false if not found</returns>
        public bool RemoveTimer(string sTimerName)
        {
            return m_kTimers.Remove(sTimerName);
        }

        /// <summary>
        /// GetTriggerCount gets the number of times the specified timer has been triggered
        /// </summary>
        /// <param name="sTimerName">Name of timer to get value for</param>
        /// <returns>iTriggerCount if found, otherwise -1</returns>
        public int GetTriggerCount(string sTimerName)
        {
            if (m_kTimers.ContainsKey(sTimerName))
                return m_kTimers[sTimerName].iTriggerCount;
            else
                return -1;
        }

        /// <summary>
        /// GetRemainingTime gets the remaining time on the specified timer
        /// </summary>
        /// <param name="sTimerName">Name of timer to get value for</param>
        /// <returns>fRemainingTime if found, otherwise -1.0f</returns>
        public float GetRemainingTime(string sTimerName)
        {
            if (m_kTimers.ContainsKey(sTimerName))
                return m_kTimers[sTimerName].fRemainingTime;
            else
                return -1;
        }
        #endregion
    }
}
