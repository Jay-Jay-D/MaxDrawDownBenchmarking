using NUnit.Framework;
using Benchmarking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Tests
{
    [TestFixture()]
    public class SimpleDrawDownTests
    {
        [Test()]
        public void SimpleDrawDownTest()
        {
            var ccr = new double[] { 0.046788161, 0.132027256, 0.309089523, -0.244958762, -0.196348893, -0.154405238, 0.224612732, 0.014574299,
                                    0.357875556, -0.242004007, -0.312642845, -0.42792098, 0.250477014, -0.721070401, 0.008095096, 0.099439248, 0.117599883,
                                    -0.378105739, 0.141633223, 0.103927508, 0.071475447, 0.023747818, 0.282125595, 0.332507841, 0.314953987, 0.034313166,
                                    0.318890626, -0.29870781, -0.317915783, -0.225546257, 0.258915614, 0.307986254, -0.237346605, 0.116116266, -0.196192505,
                                    0.319325191, 0.03684832, 0.21858425, -0.109095494};
            double mdd = SimpleDrawDown.Run(ccr);
            Assert.AreEqual(0.79933787, mdd, 1e-8);
        }
        
    }
}