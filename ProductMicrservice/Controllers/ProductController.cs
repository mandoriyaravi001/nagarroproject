using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductMicroservice.Models;
using ProductMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using UserService.Authentiction;
//using UserService.Authentiction;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductMicroservice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    
    public class ProductController : ControllerBase
    {

        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [Authorize]
        
        [HttpGet]
        public IActionResult Get()
        {
            var products = _productRepository.GetProducts();
            return new OkObjectResult(products);
        }

        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            Product product = null;
            try
            {

                product = _productRepository.GetProductByID(id);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
                return new OkObjectResult(product);
         

        }
        [Authorize(Roles =UserRoles.Admin)]
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    _productRepository.InsertProduct(product);
                    scope.Complete();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);

        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        public IActionResult Put([FromBody] Product product)
        {
            try
            {
                if (product != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        _productRepository.UpdateProduct(product);
                        scope.Complete();
                        return new OkResult();
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);

            }
            return new NoContentResult();
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _productRepository.DeleteProduct(id);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return new OkResult();
        }
    }
}
