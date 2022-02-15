using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace jettnet.core
{
    public static class PortChecker
    {

        public static ushort FindOpenAfter(ushort startPort, ushort checkPortCount)
        {
            Assert.Check(startPort > 0, "startPort needs to be greater than 0");
            Assert.Check(checkPortCount > 0, "checkPortCount needs to be greater than 0");
            Assert.Check(startPort + checkPortCount <= ushort.MaxValue, "startPort + checkPortCount needs to be less than ushort.MaxValue");

            return FindOpenInRange(startPort, (ushort) (startPort + checkPortCount));
        }

        public static ushort FindOpenInRange(ushort startPort, ushort endPort)
        {
            Assert.Check(startPort > 0, "startPort needs to be greater than 0");
            Assert.Check(endPort > 0, "endPort needs to be greater than 0");
            Assert.Check(endPort < ushort.MaxValue, "endPort needs to be less than ushort.MaxValue");
            Assert.Check(endPort > startPort, "endPort needs to be greater than startPort");

            int startingAtPort = startPort;
            int checkPortCount = endPort - startPort;

            IEnumerable<int> range = Enumerable.Range(startingAtPort, checkPortCount);
            IEnumerable<int> portsInUse =
                from p in range
                join used in IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners()
                on p equals used.Port
                select p;

            int firstFreeUDPPortInRange = range.Except(portsInUse).FirstOrDefault();

            return (ushort) firstFreeUDPPortInRange;
        }

    }
}
