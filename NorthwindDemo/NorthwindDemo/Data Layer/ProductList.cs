using System.Collections.ObjectModel;

namespace NorthwindDemo
{
    public class ProductList : ObservableCollection<Product>
    {
        public ProductList()
        {
        }
        public Product GetProductById(int id)
        {
            for(int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ProductID == id)
                {
                    return Items[i];
                }
            }
            return null;
        }
        public ProductList GetProductsByCategoryId(int id)
        {
            ProductList list = new ProductList();
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].CategoryId == id)
                {
                    list.Add(Items[i]);
                }
            }
            return list;
        }

    }
}
