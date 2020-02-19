using Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Products.Controllers
{
    public class ProductsController : ApiController
    {
        private DataProductsEntities db = new DataProductsEntities();

        private List<ProductDTO> CastFromDB()
        {
            List<ProductDTO> prod = new List<ProductDTO>();
            foreach (var item in db.Products)
            {
                prod.Add(new ProductDTO
                {
                    IdProduct = item.Id,
                    Name = item.Nombre,
                    Description = item.Description,
                    Price = item.PriceClient,
                     Image = db.ImagesProduct.Where(p => p.IdImageProduct == item.Id).Select(s => s.Image).FirstOrDefault()
                    //Image = Convert.FromBase64String(db.ImagesProduct.Where(p => p.IdImageProduct == item.Id).Select(s => s.Image).FirstOrDefault())
                }
                );
            }

            return prod;
        }

        [HttpGet]
        [ResponseType(typeof(ProductDTO))]
        public IHttpActionResult GetProduct(int id)
        {
            var productById = db.Products
                .Where(p => p.Id == id && p.IsEnabled == true)
                .Select(p => new ProductDTO {
                    IdProduct = p.Id,
                    Name = p.Nombre,
                    Description = p.Description,
                    Price = p.PriceClient,
                    Image = db.ImagesProduct.FirstOrDefault(i => i.IdImageProduct == id).Image
                })
                .FirstOrDefault();

            if (productById == null)
            {
                return NotFound();
            }

            return Ok(productById);
        }


        // GET: api/Products
        [HttpGet]
        public List<ProductDTO> GetProducts()
        {
            List<ProductDTO> prod = CastFromDB();
            return prod;
        }


        [HttpGet]
        [ResponseType(typeof(List<ProductDTO>))]
        public IHttpActionResult GetProduct(string name)
        {
            List<ProductDTO> producstByKeyword = CastFromDB().Where(p => p.Name.Contains(name)).ToList();

            if (producstByKeyword.Count == 0)
            {
                return NotFound();
            }

            return Ok(producstByKeyword);
        }


        // POST: api/Products
        public IHttpActionResult Post([FromBody]ProductDTO prod)
        {
            Models.Products newProd = new Models.Products {
                Nombre = prod.Name,
                Description = prod.Description,
                PriceClient = prod.Price,
                
            };
            db.Products.Add(newProd);
            db.SaveChanges();
            return Ok(newProd);

        }

        // PUT: api/Products/5
        public IHttpActionResult Put([FromBody]ProductDTO prod)
        {
            try
            {
                var prodmodif = db.Products.FirstOrDefault(p => p.Id == prod.IdProduct);
                prodmodif.Nombre = prod.Name;
                prodmodif.Description = prod.Description;
                prodmodif.PriceClient = prod.Price;
                db.SaveChanges();
                return Ok(prod);

            }
            catch (Exception)
            {
                return BadRequest("Product do not match with any in DB");
                throw;
            }
           
        }

        // DELETE: api/Products/5
        public bool Delete(int id)
        {
            try
            {
                db.Products.Remove(db.Products.FirstOrDefault(p => p.Id == id));
                db.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;
                throw;
            }
           
            
        }
    }
}
