﻿using BookApp.Helper;
using Interfaces.Repositories;
using Interfaces.Services;
using Models.DomainModels;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BookApp.Controllers
{
    [System.Web.Http.RoutePrefix("api/account")]
    [EnableCors(origins: "*", headers: "accept,Auth-Key", methods: "*")]
    public class UserController : ApiController
    {
        private IBookService BookService;
        private IUserService UserService;

        public UserController() {
        }
        public UserController(IUserService userService, IBookService bookService) {
            UserService = userService;
            BookService = bookService;
        }

        [HttpGet]
        [Route("GetUserById")]
        public HttpResponseMessage GetUserById(Guid userId) {
            if (userId == null || userId == Guid.Empty)
                throw new APIException() {
                    ErrorCode = (int) HttpStatusCode.BadRequest,
                    ErrorDescription = "Bad Request. Provide valid userId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            var user = UserService.GetUserById(userId);
            if (user != null)
                return Request.CreateResponse(HttpStatusCode.OK, user, JsonFormatter);
            else
                throw new APIDataException(4, "No user found", HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("CreateUser")]
        public HttpResponseMessage SaveUser([FromBody]User user) {
            if (user == null)
                throw new APIException() {
                    ErrorCode = (int) HttpStatusCode.BadRequest,
                    ErrorDescription = "Bad Request. Provide valid user object. Object can't be null.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            user = UserService.AddUser(user);
            if (user != null)
                return Request.CreateResponse(HttpStatusCode.OK, user, JsonFormatter);
            else
                throw new APIDataException(5, "Error Saving User", HttpStatusCode.NotFound);
        }

        [HttpPut]
        [Route("UpdateUser")]
        public HttpResponseMessage UpdateUser([FromBody]User user) {
            if (user == null)
                throw new APIException() {
                    ErrorCode = (int) HttpStatusCode.BadRequest,
                    ErrorDescription = "Bad Request. Provide valid user object. Object can't be null.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            user = UserService.UpdateUser(user);
            if (user != null)
                return Request.CreateResponse(HttpStatusCode.OK, user, JsonFormatter);
            else
                throw new APIDataException(6, "Error Updating User", HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("DeleteUser")]
        public HttpResponseMessage DeleteUser([FromBody]Guid userId) {
            if (userId == null || userId == Guid.Empty)
                throw new APIException() {
                    ErrorCode = (int) HttpStatusCode.BadRequest,
                    ErrorDescription = "Bad Request. Provide valid userId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            var user = UserService.GetUserById(userId);
            if (user != null) {
                var result = UserService.DeleteUser(user);
                if (result)
                    return Request.CreateResponse(HttpStatusCode.OK, "Book was deleted", JsonFormatter);
                else
                    throw new APIDataException(7, "Error Deleting User", HttpStatusCode.NotFound);
            } else
                throw new APIDataException(4, "No user found", HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("CreateUserBook")]
        public HttpResponseMessage SaveBook([FromUri]Guid userId, [FromBody]Book book) {
            if (book == null)
                throw new APIException() {
                    ErrorCode = (int) HttpStatusCode.BadRequest,
                    ErrorDescription = "Bad Request. Provide valid book object. Object can't be null.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            if (userId == null || userId == Guid.Empty)
                throw new APIException() {
                    ErrorCode = (int) HttpStatusCode.BadRequest,
                    ErrorDescription = "Bad Request. Provide valid userId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            BookService.AddBook(book);
            var result = BookService.GetBookById(book.Id);
            if (result != null)
                return Request.CreateResponse(HttpStatusCode.OK, result, JsonFormatter);
            else
                throw new APIDataException(2, "Error Saving Book", HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("GetUserBooks")]
        public HttpResponseMessage GetUserBooks(Guid userId) {
            if (userId == null || userId == Guid.Empty)
                throw new APIException() {
                    ErrorCode = (int) HttpStatusCode.BadRequest,
                    ErrorDescription = "Bad Request. Provide valid userId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            var books = BookService.GetBooksByUserId(userId);
            if (books != null)
                return Request.CreateResponse(HttpStatusCode.OK, books, JsonFormatter);
            else
                throw new APIDataException(1, "No books found", HttpStatusCode.NotFound);
        }



        protected JsonMediaTypeFormatter JsonFormatter {
            get {
                var formatter = new JsonMediaTypeFormatter();
                var json = formatter.SerializerSettings;

                json.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                json.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                json.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                json.Formatting = Newtonsoft.Json.Formatting.Indented;
                json.ContractResolver = new CamelCasePropertyNamesContractResolver();
                json.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                return formatter;
            }

        }
    }
}
