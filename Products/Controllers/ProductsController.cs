using Newtonsoft.Json.Linq;
using Products.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;

namespace Products.Controllers
{
    public class ProductsController : ApiController
    {
        private DataProductsEntities db = new DataProductsEntities();
        private string RandomImageForProduct()
        {
            string ApiUrl = "https://api.unsplash.com/photos/random?client_id=RR8zTp6LTR2TmVYodb76GyD0Z5SaXaGUoYxX3lr4TJg";
            var request = (HttpWebRequest)WebRequest.Create(ApiUrl);
            var content = string.Empty;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }
            JObject objs = JObject.Parse(content);
            string URL = objs["urls"]["small"].ToString();
            return URL;
        }

        //GET api/products/{id}
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
                    Image = db.ImagesProduct.FirstOrDefault(i => i.IdImageProduct == id).Decription
                })
                .FirstOrDefault();

            if (productById == null)
            {
                return NotFound();
            }

            return Ok(productById);
        }

        //GET api/products
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
                Image = db.ImagesProduct.FirstOrDefault(i => i.IdImageProduct == p.Id).Decription
            });

            if (prod.Count() == 0)
            {
                return NotFound();
            }

            return Ok(prod);
        }

        //GET api/products?name={keyword}
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
                    Image = db.ImagesProduct.FirstOrDefault(i => i.IdImageProduct == p.Id).Decription
                });

            if (productByName.Count() == 0)
            {
                return NotFound();
            }

            return Ok(productByName);
        }

        //POST api/products
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
                Models.ImagesProduct newImage = new Models.ImagesProduct
                {
                    IdImageProduct = newProd.Id,
                    Image = Encoding.ASCII.GetBytes(""),
                    Decription = RandomImageForProduct(),
                    DateUpdate = DateTime.Now.Date.ToString(),
                    IsEnabled = 1.ToString()
                };
                db.ImagesProduct.Add(newImage);
                prod.Image = newImage.Decription;
                db.SaveChanges();
                prod.IdProduct = newProd.Id;
                return Ok(prod);
            }
            catch (Exception e)
            {
                return BadRequest("Product  not inserted on DB error: " + e.ToString());
            }
        }

        //PUT api/products/5
        [HttpPut]
        [ResponseType(typeof(ProductDTO))]
        public IHttpActionResult Update(int id, [FromBody]ProductDTO prod)
        {
            try
            {
                //verify that the user does not include the id within json file
                if (prod.IdProduct != 0)
                    return BadRequest("The id should be included only in the URL");

                //updating the product info
                var prodModified = db.Products.Where(p => p.Id == id && p.IsEnabled == true).FirstOrDefault();
                if (prodModified == null)
                    return StatusCode(HttpStatusCode.NotFound);
                prodModified.Nombre = prod.Name;
                prodModified.Description = prod.Description;
                prodModified.PriceClient = prod.Price;

                //updating the product image
                if (prod.Image != null)
                {
                    var image = db.ImagesProduct.FirstOrDefault(i => i.IdImageProduct == id);
                    image.Decription = prod.Image;
                }

                db.SaveChanges();

                //return the updated ProductDTO
                prod.IdProduct = id;
                return Ok(prod);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        //DELETE api/products/5
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var productOnDB = db.Products.FirstOrDefault(p => p.Id == id);

                //validate that the product exist
                if (productOnDB == null || productOnDB.IsEnabled == false)
                    return StatusCode(HttpStatusCode.NotFound);

                //deleting the product and it's images
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
