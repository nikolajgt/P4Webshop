using P4Webshop.Interface.Generic;
using P4Webshop.Models.Products;

namespace P4Webshop.Extensions.ModelConverter
{
    public static class GenericModelConverters
    {

        public static Product CreateProductGeneric<T>(T p) where T : class, IProductEntities
        {
            try
            {
                return new Product(p.Category, p.Id, p.ProductName, p.ProductPrice, p.ProductQuantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateProductGeneric error", ex.Message, typeof(T));
                return null;
            }
        }
    }
}
