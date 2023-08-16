using Entities.Universal.MainData;

namespace Services
{
    public class MainDataServices
    {
        public Product test()
        {
            var x = new Product();
            x.ProductId = 1;
            x.ProductName = "Test";

            return x;
        }
    }
}