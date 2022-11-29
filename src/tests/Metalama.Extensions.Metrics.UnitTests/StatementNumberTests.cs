using Metalama.Framework.Engine.Services;
using Metalama.Framework.Engine.Testing;
using Metalama.Framework.Metrics;
using System.Linq;
using Xunit;

namespace Metalama.Extensions.Metrics.UnitTests
{
    public class StatementNumberTests : TestBase
    {
        [Fact]
        public void SimpleTest()
        {
            var services = new AdditionalServiceCollection( new StatementNumberMetricProvider() );
            using var testContext = this.CreateTestContext( services );

            var code = @"
class C 
{
  void M1 () {}
  void M2()
  {
    var x = 0;
    x++; 
  }
}

";

            var compilation = testContext.CreateCompilationModel( code );

            var type = compilation.Types.OfName( "C" ).Single();
            
            var m1 = type.Methods.OfName( "M1" ).Single();
            Assert.Equal( 0, m1.Metrics().Get<StatementNumber>().Value );

            var m2 = type.Methods.OfName( "M2" ).Single();
            Assert.Equal( 2, m2.Metrics().Get<StatementNumber>().Value );
            
            Assert.Equal( 2, type.Metrics().Get<StatementNumber>().Value );
        }
    }
}