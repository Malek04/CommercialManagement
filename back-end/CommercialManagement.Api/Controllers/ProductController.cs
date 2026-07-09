using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommercialManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            var products = _productRepository.GetProduct();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<Product> GetProduct(Guid id)
        {
            var product = _productRepository.GetProductById(id);

            if (product == null)
                return NotFound(new { Message = $"Product with id {id} not found." });

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public IActionResult AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(product.Name))
                return BadRequest("Le nom du produit est obligatoire.");

            // Generate the reference server-side on creation
            product.Reference = $"REF-{DateTime.UtcNow:yyyyMMddHHmmss}";

            _productRepository.AddProduct(product);

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id:guid}")]
        public IActionResult UpdateProduct(Guid id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != product.Id)
                return BadRequest("ID mismatch.");

            var existingProduct = _productRepository.GetProductById(id);

            if (existingProduct == null)
                return NotFound(new { Message = $"Product with id {id} not found." });

            // Update Product properties
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.UnitPriceHT = product.UnitPriceHT;
            existingProduct.StockQuantity = product.StockQuantity;

            _productRepository.UpdateProduct(existingProduct);

            return Ok(existingProduct);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteProduct(Guid id)
        {
            var product = _productRepository.GetProductById(id);

            if (product == null)
                return NotFound(new { Message = $"Product with id {id} not found." });

            _productRepository.DeleteProduct(product);

            return Ok();
        }
    }
}