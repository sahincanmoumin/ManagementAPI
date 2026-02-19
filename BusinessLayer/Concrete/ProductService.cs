using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;

namespace BusinessLayer.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAnimalRepository _animalRepository;

        public ProductService(IProductRepository productRepository, IUserRepository userRepository, IAnimalRepository animalRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _animalRepository = animalRepository;
        }

        public void SellProduct(int userId, int productId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new Exception("User not found");

            var product = _productRepository.GetById(productId);
            if (product == null)
                throw new Exception("Product not found");

            if (product.IsSold)
                throw new Exception("Product already sold");

            
            var animal = _animalRepository.GetById(product.AnimalId);
            

           
            user.Balance += product.Price;
            _userRepository.Update(user);

            product.IsSold = true;
            product.SoldAt = DateTime.Now;
            _productRepository.Update(product);
        }

        public List<Product> GetAnimalProducts(int animalId)
        {
            return _productRepository.GetByAnimalId(animalId);
        }

        public List<Product> GetUnsoldProducts()
        {
            return _productRepository.GetUnsoldProducts();
        }
    }
}