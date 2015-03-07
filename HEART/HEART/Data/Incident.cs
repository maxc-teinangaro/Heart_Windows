using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEART.Data
{
    public enum eClassification
    {
        INVALID = -1,
        LOW = 0,
        MEDIUM,
        HIGH
    };

    public class Incident
    {
        public Incident()
        { 
        }

        public Incident(DateTime _dt, int _i, eClassification _e)
        {
            m_dtInicidentTime = _dt;
            m_iHeartRate = _i;
            eRiskClassification = _e;
        }

        public void GetAll(ref DateTime _dt, ref int _i, ref eClassification _e)
        {
            _dt = m_dtInicidentTime;
            _i = m_iHeartRate;
            _e =  eRiskClassification;
        }

        DateTime m_dtInicidentTime;
        int m_iHeartRate;
        eClassification eRiskClassification;

    }
}
