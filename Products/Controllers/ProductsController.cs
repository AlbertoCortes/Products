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

        //GET api/Products/{id}
        [HttpGet]
        [ResponseType(typeof(ProductDTO))]
        public IHttpActionResult GetById(int id)
        {
            var productById = db.Products
                .Where(p => p.Id == id && p.IsEnabled == true)
                .Select(p => new ProductDTO
                {
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

        //GET api/Products
        [HttpGet]
        [ResponseType(typeof(List<ProductDTO>))]
        public IHttpActionResult GetAll()
        {
            var prod = db.Products
            .Where(p => p.IsEnabled == true)
            .Select(p => new ProductDTO
            {
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

        //GET api/Products?name={keyword}
        [HttpGet]
        [ResponseType(typeof(List<ProductDTO>))]
        public IHttpActionResult GetByName(string name)
        {
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

        //POST api/Products
        [HttpPost]
        [ResponseType(typeof(ProductDTO))]
        public IHttpActionResult Add([FromBody]ProductDTO prod)
        {
            try
            {
            Models.Products newProd = new Models.Products
            {
                IdCatalog = 3,
                Nombre = prod.Name,
                Description = prod.Description,
                PriceClient = prod.Price,
                Title = "",
                Observations = "",
                Keywords = "",
                IsEnabled = true,
                DateUpdate = DateTime.Now.Date

            };
            db.Products.Add(newProd);
            if (prod.Image != null)
            {
                Models.ImagesProduct newImage = new Models.ImagesProduct
                {
                    IdImageProduct = newProd.Id,
                    Image = prod.Image,
                    Decription = "image",
                    DateUpdate = DateTime.Now.Date.ToString(),
                    IsEnabled = 1.ToString()

                };
                db.ImagesProduct.Add(newImage);
            }

            db.SaveChanges();
            prod.IdProduct = newProd.Id;

            return Ok(prod);
            }
            catch (Exception e)
            {
                return BadRequest("Product  not inserted on DB error: " + e.ToString());
            }
        }

        //PUT api/Products/5
        [HttpPut]
        [ResponseType(typeof(ProductDTO))]
        public IHttpActionResult Update([FromBody]ProductDTO prod)
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
            }
        }

        //DELETE api/Products/5
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var productOnDB = db.Products.FirstOrDefault(p => p.Id == id);
                productOnDB.IsEnabled = false;
                var images = db.ImagesProduct.Where(i => i.IdImageProduct == id);
                foreach (ImagesProduct image in images)
                    image.IsEnabled = "0";
                db.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
