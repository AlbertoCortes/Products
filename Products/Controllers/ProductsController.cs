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

        //private List<ProductDTO> CastFromDB()
        //{
        //    List<ProductDTO> prod = new List<ProductDTO>();
        //    foreach (var item in db.Products)
        //    {
        //        prod.Add(new ProductDTO
        //        {
        //            IdProduct = item.Id,
        //            Name = item.Nombre,
        //            Description = item.Description,
        //            Price = item.PriceClient,
        //             Image = db.ImagesProduct.Where(p => p.IdImageProduct == item.Id).Select(s => s.Image).FirstOrDefault()
        //            //Image = Convert.FromBase64String(db.ImagesProduct.Where(p => p.IdImageProduct == item.Id).Select(s => s.Image).FirstOrDefault())
        //        }
        //        );
        //    }

        //    return prod;
        //}

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
        [ResponseType(typeof(List<ProductDTO>))]
        public IHttpActionResult GetProducts()
        {
            //      List<ProductDTO> prod = CastFromDB();
            var prod = db.Products
            .Where(p => p.IsEnabled == true)
            .Select(p => new ProductDTO {
                IdProduct = p.Id,
                Name = p.Nombre,
                Description = p.Description,
                Price = p.PriceClient,
                Image = db.ImagesProduct.FirstOrDefault(i => i.IdImageProduct == p.Id).Image
            });
            if (prod.Count() == 0)
            {
                return NotFound();
            }

            return Ok(prod);
        }


        [HttpGet]
        [ResponseType(typeof(List<ProductDTO>))]
        public IHttpActionResult GetProduct(string name)
        {
         //   List<ProductDTO> producstByKeyword = CastFromDB().Where(p => p.Name.Contains(name)).ToList();
            var productByName = db.Products
                .Where(p => p.Nombre.Contains(name) && p.IsEnabled == true)
                .Select(p => new ProductDTO
                {
                    IdProduct = p.Id,
                    Name = p.Nombre,
                    Description = p.Description,
                    Price = p.PriceClient,
                    Image = db.ImagesProduct.FirstOrDefault(i => i.IdImageProduct == p.Id).Image
                });

            if (productByName.Count() == 0)
            {
                return NotFound();
            }

            return Ok(productByName);
        }


        // POST: api/Products
        [HttpPost]
        public IHttpActionResult Post([FromBody]ProductDTO prod)
        {
            
            Models.Products newProd = new Models.Products {
                IdCatalog = 3,
                Nombre = prod.Name,
                Description = prod.Description,
                PriceClient = prod.Price,
                Title= "",
                Observations = "",
                Keywords = "",
                IsEnabled = true,
                DateUpdate = DateTime.Now.Date
                
            };
            db.Products.Add(newProd);
            //db.SaveChanges();
            Models.ImagesProduct newImage = new Models.ImagesProduct {
                IdImageProduct = newProd.Id,
                Image = prod.Image,
                Decription = "image",
                DateUpdate = DateTime.Now.Date.ToString(),
                IsEnabled = 1.ToString()
                
            };
            db.ImagesProduct.Add(newImage);
            db.SaveChanges();
            prod.IdProduct = newProd.Id;
            return Ok(prod);

        }

        // PUT: api/Products/5
        [HttpPut]
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
        [HttpDelete]
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
