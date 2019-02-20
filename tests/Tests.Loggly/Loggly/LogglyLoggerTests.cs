using AutoFixture.Idioms;
using EMG.Extensions.Logging.Loggly;
using NUnit.Framework;

namespace Tests.Loggly
{
    [TestFixture]
    public class LogglyLoggerTests
    {
        [Test, AutoMoqData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(LogglyLogger).GetConstructors());
        }
    }
}