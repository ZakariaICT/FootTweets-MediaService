using System;
using System.Collections.Generic;
using AutoMapper;
using MediaService.Controllers;
using MediaService.DTO;
using MediaService.Model;
using MediaService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MediaTests
{

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetAllPictures_ReturnsListOfPictures()
        {
            // Arrange
            var pictureItems = new List<Pictures>
            {
                new Pictures { Id = 1, Text = "Picture 1" },
                new Pictures { Id = 2, Text = "Picture 2" }
            };

            // Create an instance of your repository and pass the list
            var repository = new TestMediaRepo(pictureItems);

            // Act
            var result = repository.GetAllPictures();

            // Assert
            Assert.IsNotNull(result);

            // Ensure that result is an IEnumerable<Pictures>
            Assert.IsInstanceOfType(result, typeof(IEnumerable<Pictures>));

            // Check the count of items in the result
            Assert.AreEqual(2, result.Count());
        }



        [TestMethod]
        public void GetPictureByID_ExistingId_ReturnsPicture()
        {
            // Arrange
            var pictureItem = new Pictures { Id = 1, Text = "Picture 1" };
            var pictureItems = new List<Pictures> { pictureItem };
            var repository = new TestMediaRepo(pictureItems);

            // Act
            var result = repository.GetPictureByID(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pictureItem, result); // Check if the returned picture matches the expected picture
        }

        [TestMethod]
        public void GetPictureByID_NonExistingId_ReturnsNull()
        {
            // Arrange
            var repository = new TestMediaRepo(new List<Pictures>());

            // Act
            var result = repository.GetPictureByID(1);

            // Assert
            Assert.IsNull(result); // Expecting null for a non-existing picture
        }

    }
}
