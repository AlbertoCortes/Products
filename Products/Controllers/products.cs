﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Products.Models;

namespace Products.Controllers
{
    public class products : ApiController
    {
        private DataProductsEntities db = new DataProductsEntities();
        public List<ProductDTO> CastFromDB()
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
                    Image = null
                }
                );
            }

            return prod;
        }

        // GET: api/products
        public List<ProductDTO> GetProducts()
        {
            List<ProductDTO> prod = CastFromDB();
            return prod;
        }

        // GET: api/products/5
        [ResponseType(typeof(ProductDTO))]
        public IHttpActionResult GetProduct(int id)
        {
            List<ProductDTO> prod = CastFromDB();
            ProductDTO productByID = prod.FirstOrDefault(p => p.IdProduct == id);
            
            if (productByID == null)
            {
                return NotFound();
            }

            return Ok(productByID);
        }

        [ResponseType(typeof(List<ProductDTO>))]
        public IHttpActionResult GetProduct(string keyword)
        {
            List<ProductDTO> producstByKeyword = CastFromDB().Where(p => p.Name.Contains(keyword)).ToList();

            if (producstByKeyword == null)
            {
                return NotFound();
            }

            return Ok(producstByKeyword);
        }


        // PUT: api/products/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/products
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // DELETE: api/products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}