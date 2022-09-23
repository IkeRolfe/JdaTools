using JdaTools.Connection;
using Xunit.Abstractions;

namespace JdaTools.Test
{
    public class UnitTest1
    {
        private ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async void Test1()
        {
            var client = new MocaClient("http://172.18.17.60:5020/service");
            var response = await client.ConnectAsync(new MocaCredentials
            {
                UserName = "ASUPRPL",
                Password = "1234"
            });
            var inv = await client.ExecuteQuery<TestClass>(
                "list usr available inventory " +
            "where prtnum = @prtnum " +
            "and wh_id = @wh_id " +
            "and in_warehouse = @in_warehouse " +
            "and prt_client_id = @prt_client_id",
                new
                {
                    prtnum = "2214236",
                    wh_id = "LOG1",
                    in_warehouse = 0,
                    prt_client_id = "PL"
                }
                );
            foreach (var i in inv)
            {
                _output.WriteLine($"{i.NewDate}\t{i.Flag}");
            }
        }
    }
}