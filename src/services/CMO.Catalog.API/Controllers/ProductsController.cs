using CMO.Core.Controllers;
using CMO.Product.API.Application.DTO;
using CMO.Product.Domain.Entities;
using CMO.Product.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CMO.Products.API.Controllers

{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : BaseController
    {

        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/products
        /// <summary>
        /// Obtêm os produtos
        /// </summary>
        /// <returns>Coleção de objetos da classe Produto</returns>                
        /// <response code="200">Lista dos produtos</response>        
        /// <response code="400">Falha na requisição</response>                 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            var products = await _productRepository.GetAllAsync();

            return CustomResponse(products);
        }

        // GET: api/products/5
        /// <summary>
        /// Obtêm as informações do produto pelo seu Id
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Objetos da classe Produto</returns>                
        /// <response code="200">Informações do Produto</response>
        /// <response code="400">Falha na requisição</response>            
        /// <response code="404">O produto não foi localizado</response>         
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                AddProcessingError("Não foi possível localizar o produto!");
                return CustomResponse(isNotFound: true);
            }

            return CustomResponse(product);
        }

        // POST api/products/
        /// <summary>
        /// Grava o produto
        /// </summary>   
        /// <remarks>
        /// Exemplo request:
        ///
        ///     POST / Produto
        ///     {
        ///         "title": "Sandalia",
        ///         "description": "Sandália Preta Couro Salto Fino",
        ///         "price": 249.50,
        ///         "quantity": 100       
        ///     }
        /// </remarks>        
        /// <returns>Retorna objeto criado da classe Produto</returns>                
        /// <response code="201">O produto foi incluído corretamente</response>                
        /// <response code="400">Falha na requisição</response>         
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ActionName("NewProduct")]
        public async Task<IActionResult> PostAsync([FromBody] ProductDTO productDTO)
        {
            if (!productDTO.Validate()) return CustomResponse(productDTO.ValidationResult);

            var product = new ProductModel()
            {
                Title = productDTO.Title,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Quantity = productDTO.Quantity
            };

            await _productRepository.AddAsync(product);

            return CreatedAtAction("NewProduct", new { id = product.Id }, product);
        }

        // PUT: api/products/5
        /// <summary>
        /// Altera o produto
        /// </summary>        
        /// <param name="id">Código do produto</param>        
        /// <response code="200">O produto foi alterado corretamente</response>                
        /// <response code="400">Falha na requisição</response>
        /// <response code="404">O produto não foi localizado</response>         
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] ProductDTO productDTO)
        {
            if (!productDTO.Validate()) return CustomResponse(productDTO.ValidationResult);

            if (id != productDTO.Id)
            {
                AddProcessingError("O ID enviado está divergente do enviado no body da requisição!");
                return CustomResponse();
            }

            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    AddProcessingError("Não foi possível localizar o produto!");
                    return CustomResponse(isNotFound: true);
                }

                product.Update(productDTO.Title, productDTO.Description, productDTO.Price, productDTO.Quantity);

                var result = await _productRepository.UpdateAsync(product);
                return CustomResponse(result);
            }
            catch (DbUpdateConcurrencyException)
            {
                AddProcessingError("Houve um problema na atualização do registro no BD!");
                return CustomResponse();
            }
        }

        // DELETE: api/products/5
        /// <summary>
        /// Deleta o produto pelo seu Id
        /// </summary>        
        /// <param name="id">Código do produto</param>        
        /// <response code="200">O produto foi excluído corretamente</response>                
        /// <response code="400">Falha na requisição</response>         
        /// <response code="404">O produto não foi localizado</response>         
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var isProductExists = await ProductModelExistsAsync(id);

            if (!isProductExists)
            {
                AddProcessingError("Não foi possível localizar o produto!");
                return CustomResponse(isNotFound: true);
            }

            var result = await _productRepository.DeleteAsync(id);

            if (result.DeletedCount == 0)
            {
                AddProcessingError("Houve um problema na exclusão do registro no BD!");
                return CustomResponse();
            }

            return CustomResponse();
        }

        private async Task<bool> ProductModelExistsAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? true : false;
        }
    }
}
